using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace PenaltyKing.Tests
{
    public sealed class CharacterAnatomyTests
    {
        [UnityTest]
        public IEnumerator LimbLengthsAndBendLimitsRemainValidAcrossAllSixSaves()
        {
            RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.Endless);
            yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
            var p=Object.FindFirstObjectByType<ShotPresentation>();
            var rigs=Object.FindObjectsByType<FootballRig>(FindObjectsSortMode.None);
            foreach(ShotDirection d in System.Enum.GetValues(typeof(ShotDirection)))
            for(var frame=0;frame<=80;frame++)
            {
                p.Sample(new ShotResult(d,d),frame/30f);
                foreach(var rig in rigs)
                {
                    foreach(var side in new[]{"Left","Right"})
                    {
                        var leg=rig.transform.Find("Hips/"+side+" Hip");
                        CheckChain(rig,leg,leg.Find(side+" Knee"),leg.Find(side+" Knee/"+side+" Ankle"),48,48,8,145,rig.Goalkeeper?side=="Left":true);
                        var arm=rig.transform.Find("Hips/Chest/"+side+" Shoulder");
                        CheckChain(rig,arm,arm.Find(side+" Elbow"),arm.Find(side+" Elbow/"+side+" Hand"),35,34,12,145,rig.Goalkeeper?side=="Left":side=="Right");
                    }
                    var waist=rig.transform.Find("Waist Overlap").GetComponent<Image>();
                    Assert.That(waist.sprite,Is.Not.Null);Assert.That(waist.material,Is.SameAs(rig.transform.Find("Art Chest").GetComponent<Image>().material));
                }
            }
            yield return SceneManager.LoadSceneAsync("MainMenu");
        }

        private static void CheckChain(FootballRig rig,Transform a,Transform b,Transform c,float first,float second,float minimum,float maximum,bool right)
        {
            Vector2 Local(Transform joint)=>rig.transform.InverseTransformPoint(joint.position);
            var upper=Local(b)-Local(a);var lower=Local(c)-Local(b);
            Assert.That(upper.magnitude,Is.EqualTo(first).Within(.003f));
            Assert.That(lower.magnitude,Is.EqualTo(second).Within(.003f));
            Assert.That(Vector2.Angle(upper,lower),Is.InRange(minimum-.1f,maximum+.1f));
            Assert.That((upper.x*lower.y-upper.y*lower.x)*(right?-1:1),Is.GreaterThan(0),"IK must not flip its bend side");
        }

        [UnityTest]
        public IEnumerator RecoveryReturnsBodyPivotsHomeAndSaveStillHoldsBall()
        {
            RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.Endless);
            yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
            var p=Object.FindFirstObjectByType<ShotPresentation>();var rigs=Object.FindObjectsByType<FootballRig>(FindObjectsSortMode.None);
            foreach(var rig in rigs)
            {
                p.ResetPose();var position=rig.transform.localPosition;var rotation=rig.transform.localRotation;
                foreach(ShotDirection d in System.Enum.GetValues(typeof(ShotDirection)))
                {
                    p.Sample(new ShotResult(d,d),ShotPresentation.Duration-.01f);
                    Assert.That(Vector3.Distance(rig.transform.localPosition,position),Is.LessThan(.01f));
                    Assert.That(Quaternion.Angle(rig.transform.localRotation,rotation),Is.LessThan(.01f));
                    Assert.That(Vector2.Distance(p.BallPosition,p.GripPosition),Is.LessThan(.01f));
                }
            }
            yield return SceneManager.LoadSceneAsync("MainMenu");
        }
    }
}
