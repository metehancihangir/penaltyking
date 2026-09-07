using NUnit.Framework;
using UnityEngine;
namespace PenaltyKing.Tests
{
 [SetUpFixture]
 public sealed class GuidePreferenceFixture
 {
  bool existed;int value;
  [OneTimeSetUp] public void Preserve(){existed=PlayerPrefs.HasKey(FirstPlayGuide.CompletedKey);value=PlayerPrefs.GetInt(FirstPlayGuide.CompletedKey);PlayerPrefs.SetInt(FirstPlayGuide.CompletedKey,1);}
  [OneTimeTearDown] public void Restore(){if(existed)PlayerPrefs.SetInt(FirstPlayGuide.CompletedKey,value);else PlayerPrefs.DeleteKey(FirstPlayGuide.CompletedKey);PlayerPrefs.Save();}
 }
}
