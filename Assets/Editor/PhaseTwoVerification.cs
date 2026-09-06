using System;
using System.IO;
using UnityEngine;

namespace PenaltyKing.Editor
{
    // Two separate Unity processes prove PlayerPrefs survives process termination.
    // Back up only this game's two keys; never clear unrelated user preferences.
    public static class PhaseTwoVerification
    {
        private const string BackupPath = "TestResults/phase2-preferences-backup.json";
        [Serializable] private sealed class Backup
        {
            public bool hadMusic, hadSfx;
            public float music, sfx;
        }

        public static void SetupAndWrite()
        {
            PhaseTwoSetup.CreateOptions();
            if (File.Exists(BackupPath)) throw new InvalidOperationException("Pending preference backup must be restored first.");
            Directory.CreateDirectory("TestResults");
            var backup = new Backup
            {
                hadMusic = PlayerPrefs.HasKey(AudioPreferences.MusicKey),
                hadSfx = PlayerPrefs.HasKey(AudioPreferences.SfxKey),
                music = PlayerPrefs.GetFloat(AudioPreferences.MusicKey),
                sfx = PlayerPrefs.GetFloat(AudioPreferences.SfxKey)
            };
            File.WriteAllText(BackupPath, JsonUtility.ToJson(backup));
            AudioPreferences.Save(0.23f, 0.67f);
            Debug.Log("[Phase 2 persistence] Process A saved 0.23 / 0.67.");
        }

        public static void ReadRestoreAndPreview()
        {
            var backup = JsonUtility.FromJson<Backup>(File.ReadAllText(BackupPath));
            try
            {
                AudioPreferences.Load(out var music, out var sfx);
                if (Mathf.Abs(music - 0.23f) > 0.001f || Mathf.Abs(sfx - 0.67f) > 0.001f)
                    throw new InvalidOperationException("Saved preferences did not survive process restart.");
                File.WriteAllText("TestResults/phase2-restart.txt", "PASS: Separate Unity process loaded Music=0.23 and SFX=0.67. Original preferences restored.");
                Debug.Log("[Phase 2 persistence] Process B restored expected values: PASS.");
            }
            finally
            {
                if (backup.hadMusic) PlayerPrefs.SetFloat(AudioPreferences.MusicKey, backup.music);
                else PlayerPrefs.DeleteKey(AudioPreferences.MusicKey);
                if (backup.hadSfx) PlayerPrefs.SetFloat(AudioPreferences.SfxKey, backup.sfx);
                else PlayerPrefs.DeleteKey(AudioPreferences.SfxKey);
                PlayerPrefs.Save();
                File.Delete(BackupPath);
            }
            MenuPreview.CaptureOptions();
        }
    }
}
