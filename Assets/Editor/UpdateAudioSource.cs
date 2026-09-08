using System.IO;
using UnityEditor;
using UnityEngine;
namespace PenaltyKing.Editor
{
 public static class UpdateAudioSource
 {
  public static void Decode()
  {
   foreach(var name in new[]{"chant-drums","crowd-ooh"})
   {
    var path="Assets/Audio/Source/"+name+".mp3";AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
    var importer=(AudioImporter)AssetImporter.GetAtPath(path);var sample=importer.defaultSampleSettings;sample.loadType=AudioClipLoadType.DecompressOnLoad;sample.compressionFormat=AudioCompressionFormat.PCM;sample.preloadAudioData=true;importer.defaultSampleSettings=sample;importer.SaveAndReimport();
    var clip=AssetDatabase.LoadAssetAtPath<AudioClip>(path);clip.LoadAudioData();var data=new float[clip.samples*clip.channels];if(!clip.GetData(data,0))throw new System.Exception("Decode failed");
    Write("docs/audio-source/"+name+".wav",data,clip.frequency,clip.channels);
   }
  }
  public static void Write(string path,float[] data,int rate,int channels)
  {
   using(var w=new BinaryWriter(File.Create(path))){w.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));w.Write(36+data.Length*2);w.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));w.Write(16);w.Write((short)1);w.Write((short)channels);w.Write(rate);w.Write(rate*channels*2);w.Write((short)(channels*2));w.Write((short)16);w.Write(System.Text.Encoding.ASCII.GetBytes("data"));w.Write(data.Length*2);foreach(var v in data)w.Write((short)Mathf.RoundToInt(Mathf.Clamp(v,-1,1)*32767));}
  }
 }
}

