using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using MAB_Spark.Models;

namespace MAB_Spark.Services
{
    public class TextHookService
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_SPACE = 0x20;
        private const int VK_RETURN = 0x0D;
        private const int VK_TAB = 0x09;

        private IntPtr _hookId = IntPtr.Zero;
        private LowLevelKeyboardProc? _proc;
        private DatabaseService _dbService;
        private SoundService _soundService;
        private StringBuilder _currentWord = new StringBuilder();
        private Action<string, string>? _onShortcutExpanded;

        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        [DllImport("user32.dll")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        private static extern int ToUnicode(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)] StringBuilder pwszBuff, int cchBuff, uint wFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public TextHookService(DatabaseService dbService, SoundService soundService)
        {
            _dbService = dbService;
            _soundService = soundService;
        }

        public void StartHooking(Action<string, string> onShortcutExpanded)
        {
            _onShortcutExpanded = onShortcutExpanded;
            _proc = HookCallback;

            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                if (curModule != null)
                {
                    var moduleHandle = GetModuleHandle(curModule.ModuleName);
                    _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, moduleHandle, 0);
                }
            }
        }

        public void StopHooking()
        {
            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                try
                {
                    var vkCode = Marshal.ReadInt32(lParam);
                    var keyChar = GetCharFromVirtualKey((uint)vkCode);

                    // Space, Enter, Tab → word delimiters
                    if (vkCode == VK_SPACE || vkCode == VK_RETURN || vkCode == VK_TAB)
                    {
                        if (_currentWord.Length > 0)
                        {
                            var word = _currentWord.ToString();
                            var shortcut = _dbService.GetShortcutByText(word);

                            if (shortcut != null && shortcut.IsEnabled)
                            {
                                ExpandShortcut(word, shortcut.ExpandedText);
                            }

                            _currentWord.Clear();
                        }
                    }
                    else if (char.IsLetterOrDigit(keyChar) || keyChar == '_' || keyChar == '-')
                    {
                        _currentWord.Append(keyChar);
                    }
                    else
                    {
                        _currentWord.Clear();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Hata: {ex.Message}");
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private char GetCharFromVirtualKey(uint vkCode)
        {
            var keyboardState = new byte[256];
            GetKeyboardState(keyboardState);

            var scanCode = MapVirtualKey(vkCode, 0);
            var result = new StringBuilder(2);

            var resultLength = ToUnicode(vkCode, scanCode, keyboardState, result, 2, 0);

            if (resultLength == 1)
                return result[0];

            return '\0';
        }

        private void ExpandShortcut(string shortText, string expandedText)
        {
            try
            {
                // Metni clipboard'a kopyala
                System.Windows.Forms.Clipboard.SetText(expandedText);

                // Ctrl+V ile yapıştır (SendKeys kullan)
                System.Windows.Forms.SendKeys.Send("^v");

                // Ses çal
                _soundService.PlaySuccessSound();

                // Event tetikle
                _onShortcutExpanded?.Invoke(shortText, expandedText);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Genişletme hatası: {ex.Message}");
            }
        }
    }
}
