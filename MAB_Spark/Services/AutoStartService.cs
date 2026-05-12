using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace MAB_Spark.Services
{
    public class AutoStartService
    {
        private const string REGISTRY_KEY = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string APP_NAME = "MAB_Spark";

        public bool IsAutoStartEnabled()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY))
                {
                    return key?.GetValue(APP_NAME) != null;
                }
            }
            catch
            {
                return false;
            }
        }

        public void EnableAutoStart()
        {
            try
            {
                var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, true))
                {
                    key?.SetValue(APP_NAME, exePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Auto-start aktivasyon hatası: {ex.Message}");
            }
        }

        public void DisableAutoStart()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY, true))
                {
                    key?.DeleteValue(APP_NAME, false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Auto-start deaktivasyon hatası: {ex.Message}");
            }
        }
    }
}
