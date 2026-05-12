using System;
using System.Media;

namespace MAB_Spark.Services
{
    public class SoundService
    {
        public void PlayNotificationSound()
        {
            try
            {
                // Windows sistem sesini kullan
                SystemSounds.Beep.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ses çalma hatası: {ex.Message}");
            }
        }

        public void PlaySuccessSound()
        {
            try
            {
                // Ding sesi (başarılı işlem)
                SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ses çalma hatası: {ex.Message}");
            }
        }
    }
}
