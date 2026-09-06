using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PenaltyKing.Editor
{
    public static class PhaseSevenSetup
    {
        private const string Root = "Assets/Audio/Stadium/";
        [MenuItem("Penalty King/Phase 7/Add stadium audio")]
        public static void Build()
        {
            Directory.CreateDirectory("Assets/Audio/Source");
            foreach (var name in new[] { "ambience", "kick", "goal", "save" })
            {
                var path = "Assets/Audio/Source/" + name + ".mp3";
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                PrepareImport(path);
            }
            MakeClip("ambience", "CrowdLoop", 10, 24.5f, .035f, .16f, true);
            MakeClip("kick", "Kick", 0, .4f, .17f, .8f, false);
            MakeClip("goal", "GoalCheer", 0, 1.18f, .16f, .7f, false);
            MakeClip("save", "SaveReaction", 0, 1.18f, .12f, .6f, false);
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var game = UnityEngine.Object.FindFirstObjectByType<GameplayController>();
            if (game.Presentation == null) throw new InvalidOperationException("Phase 6 scene is required.");
            var audio = game.GetComponent<GameplayAudio>();
            if (audio == null) audio = game.gameObject.AddComponent<GameplayAudio>();
            audio.Configure(Load("CrowdLoop"), Load("Kick"), Load("GoalCheer"), Load("SaveReaction"));
            EditorUtility.SetDirty(audio); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
            Debug.Log("[Phase 7] Four preloaded PCM clips connected to animation events. Crowd uses SFX bus.");
        }

        private static AudioClip Load(string name) => AssetDatabase.LoadAssetAtPath<AudioClip>(Root + name + ".wav");
        private static void PrepareImport(string path)
        {
            var importer = (AudioImporter)AssetImporter.GetAtPath(path);
            var settings = importer.defaultSampleSettings;
            settings.loadType = AudioClipLoadType.DecompressOnLoad;
            settings.compressionFormat = AudioCompressionFormat.PCM;
            settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
            settings.preloadAudioData = true;
            importer.defaultSampleSettings = settings; importer.loadInBackground = false;
            importer.SaveAndReimport();
        }

        private static void MakeClip(string source, string output, float startSeconds, float duration, float rmsTarget, float ceiling, bool loop)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Source/" + source + ".mp3");
            clip.LoadAudioData();
            var channels = clip.channels; var rate = clip.frequency;
            var raw = new float[clip.samples * channels];
            if (!clip.GetData(raw, 0)) throw new InvalidOperationException("Cannot decode " + source);
            var start = Mathf.FloorToInt(startSeconds * rate);
            if (!loop)
            {
                float peak = 0; foreach (var sample in raw) peak = Mathf.Max(peak, Mathf.Abs(sample));
                for (var i = 0; i < raw.Length; i++)
                    if (Mathf.Abs(raw[i]) > peak * .075f) { start = Mathf.Max(0, i / channels - (int)(rate * .004f)); break; }
            }
            var frames = Mathf.Min((int)(duration * rate), clip.samples - start);
            var data = new float[frames * channels]; Array.Copy(raw, start * channels, data, 0, data.Length);
            if (loop)
            {
                var overlap = (int)(rate * .5f);
                var seamless = new float[(frames - overlap) * channels];
                Array.Copy(data, overlap * channels, seamless, 0, (frames - 2 * overlap) * channels);
                for (var i = 0; i < overlap; i++) for (var c = 0; c < channels; c++)
                {
                    var mix = i / (float)(overlap - 1);
                    seamless[(frames - 2 * overlap + i) * channels + c] =
                        data[(frames - overlap + i) * channels + c] * (1 - mix) + data[i * channels + c] * mix;
                }
                data = seamless;
            }
            else
            {
                var fadeIn = (int)(rate * .003f); var fadeOut = (int)(rate * (source == "kick" ? .06f : .18f));
                for (var i = 0; i < frames; i++) for (var c = 0; c < channels; c++)
                    data[i * channels + c] *= Mathf.Min(1, Mathf.Min(i / (float)fadeIn, (frames - 1 - i) / (float)fadeOut));
            }
            double energy = 0; float max = 0;
            foreach (var sample in data) { energy += sample * sample; max = Mathf.Max(max, Mathf.Abs(sample)); }
            var gain = Mathf.Min(rmsTarget / (float)Math.Sqrt(energy / data.Length), ceiling / Mathf.Max(max, .001f));
            for (var i = 0; i < data.Length; i++) data[i] *= gain;
            var path = Root + output + ".wav";
            using (var writer = new BinaryWriter(File.Create(path)))
            {
                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF")); writer.Write(36 + data.Length * 2);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt ")); writer.Write(16);
                writer.Write((short)1); writer.Write((short)channels); writer.Write(rate);
                writer.Write(rate * channels * 2); writer.Write((short)(channels * 2)); writer.Write((short)16);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data")); writer.Write(data.Length * 2);
                foreach (var sample in data) writer.Write((short)Mathf.RoundToInt(sample * 32767));
            }
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport); PrepareImport(path);
            Debug.Log($"[Phase 7] {output}: {data.Length / (float)(channels * rate):F3}s; peak {max * gain:F3}; RMS {Math.Sqrt(energy / data.Length) * gain:F3}");
        }
    }
}
