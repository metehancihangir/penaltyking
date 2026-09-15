using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace PenaltyKing.Tests
{
    public sealed class DetailedAnimationTests
    {
        [UnityTest]
        public IEnumerator SixContactPosesExtendLeadArmAndUseNativeSpriteSkin()
        {
            RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.Endless);
            yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
            var p=Object.FindFirstObjectByType<ShotPresentation>();var keeper=Object.FindObjectsByType<FootballRig>(FindObjectsSortMode.None).First(r=>r.Goalkeeper);
            var skin=keeper.GetComponentsInChildren<FootballSkin>().First();
            Assert.That(skin.NativeSkin,Is.Not.Null);Assert.That(skin.NativeSkin.boneTransforms.Length,Is.EqualTo(17));
            foreach(ShotDirection d in System.Enum.GetValues(typeof(ShotDirection)))
            {
                p.Sample(new ShotResult(d,d),ShotPresentation.ImpactTime);
                Assert.That(Quaternion.Angle(keeper.transform.localRotation,Quaternion.identity),Is.LessThan(.01f),"Direction is authored in joints, not a rotated root");
                Assert.That(Vector2.Distance(p.GripPosition,p.TargetPoint(d)),Is.LessThan(.01f));
                var pose=KeeperPoses.Contact(d);
                foreach(var side in new[]{"Left","Right"})
                {
                    if((side=="Left"?pose.leftReach:pose.rightReach)<.9f)continue;
                    var shoulder=keeper.transform.Find("Hips/Lower Spine/Upper Spine/Chest/"+side+" Shoulder");
                    var elbow=shoulder.Find(side+" Elbow");var hand=elbow.Find(side+" Hand");
                    Assert.That(Vector3.Angle(elbow.position-shoulder.position,hand.position-elbow.position),Is.LessThan(1.5f),d+" leading elbow");
                }
                if(ShotTargets.Column(d)!=1)
                {
                    var longest=0f;
                    foreach(var side in new[]{"Left","Right"})
                    {var hip=keeper.transform.Find("Hips/"+side+" Hip");var knee=hip.Find(side+" Knee");var foot=knee.Find(side+" Ankle");longest=Mathf.Max(longest,Vector3.Distance(hip.position,foot.position)/(Vector3.Distance(hip.position,knee.position)+Vector3.Distance(knee.position,foot.position)));}
                    Assert.That(longest,Is.GreaterThan(.93f),d+" push leg must extend");
                }
                yield return null;yield return null;
                skin.SetVerticesDirty();Canvas.ForceUpdateCanvases();
                Assert.That(skin.UsedNativeDeformation,Is.True,"Canvas must display the actual Unity Sprite Skin result");
                Capture("detailed-contact-"+d);
            }
            p.ResetPose();yield return null;yield return null;Capture("detailed-ready");
            yield return SceneManager.LoadSceneAsync("MainMenu");
        }
        private static void Capture(string name)
        {
            var camera=Camera.main;var target=new RenderTexture(1280,720,24);var previous=RenderTexture.active;
            camera.targetTexture=target;Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;
            var image=new Texture2D(1280,720,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1280,720),0,0);image.Apply();
            Directory.CreateDirectory("docs/previews");File.WriteAllBytes("docs/previews/"+name+".png",image.EncodeToPNG());
            camera.targetTexture=null;RenderTexture.active=previous;Object.Destroy(image);target.Release();Object.Destroy(target);
        }
    }
}
