using System.IO;
using System.Xml;
using UnityEditor.Android;
namespace PenaltyKing.Editor
{
 public sealed class HapticsAndroidManifest : IPostGenerateGradleAndroidProject
 {
  public int callbackOrder=>0;
  public void OnPostGenerateGradleAndroidProject(string path)
  {
   var file=Path.Combine(path,"src/main/AndroidManifest.xml");var xml=new XmlDocument();xml.Load(file);
   const string ns="http://schemas.android.com/apk/res/android";
   foreach(XmlNode node in xml.SelectNodes("/manifest/uses-permission"))if(node.Attributes["name",ns]?.Value=="android.permission.VIBRATE")return;
   var permission=xml.CreateElement("uses-permission");var name=xml.CreateAttribute("android","name",ns);name.Value="android.permission.VIBRATE";permission.Attributes.Append(name);xml.DocumentElement.AppendChild(permission);xml.Save(file);
  }
 }
}
