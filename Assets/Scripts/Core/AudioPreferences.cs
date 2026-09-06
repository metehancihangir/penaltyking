using UnityEngine;

namespace PenaltyKing
{
    public static class AudioPreferences
    {
        public const string MusicKey = "PenaltyKing.Audio.MusicVolume.v1";
        public const string SfxKey = "PenaltyKing.Audio.SfxVolume.v1";

        public static void Load(out float music, out float sfx)
        {
            music = Mathf.Clamp01(PlayerPrefs.GetFloat(MusicKey, 0.7f));
            sfx = Mathf.Clamp01(PlayerPrefs.GetFloat(SfxKey, 0.8f));
        }

        public static void Save(float music, float sfx)
        {
            PlayerPrefs.SetFloat(MusicKey, Mathf.Clamp01(music));
            PlayerPrefs.SetFloat(SfxKey, Mathf.Clamp01(sfx));
            PlayerPrefs.Save();
        }
    }
}
