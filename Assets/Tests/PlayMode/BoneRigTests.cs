using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace PenaltyKing.Tests
{
    public sealed class BoneRigTests
    {
        [UnityTest] public IEnumerator EverySaveKeepsVisibleBallBetweenHandsUntilReset()
        {
            RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.Endless);
            yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
            var p=Object.FindFirstObjectByType<ShotPresentation>();var stage=Object.FindFirstObjectByType<GameplayStageLayout>().transform;
            Assert.That(p.UsesBoneRigs,Is.True);
            var keeper=stage.Find("06 Keeper");var held=keeper.Find("Held Football").GetComponent<Image>();var flying=stage.Find("08 Ball").GetComponent<Image>();
            foreach(ShotDirection direction in System.Enum.GetValues(typeof(ShotDirection)))
            {
                var shot=new ShotResult(direction,direction);
                foreach(var age in new[]{0f,.1f,.3f,.6f,1.1f})
                {
                    p.Sample(shot,ShotPresentation.ImpactTime+age);
                    Assert.That(held.gameObject.activeSelf,Is.True);Assert.That(flying.color.a,Is.Zero);
                    Assert.That(Vector2.Distance(p.BallPosition,p.GripPosition),Is.LessThan(.001f));
                    Assert.That(Vector2.Distance((Vector2)stage.InverseTransformPoint(held.transform.position),p.GripPosition),Is.LessThan(.001f));
                    Assert.That(flying.rectTransform.localRotation,Is.EqualTo(Quaternion.identity));
                }
                p.ResetPose();Assert.That(held.gameObject.activeSelf,Is.False);Assert.That(flying.color.a,Is.EqualTo(1));
            }
            var striker=stage.Find("07 Shooter");var art=striker.Find("Art Chest").GetComponent<Image>();var sprite=art.sprite;var knee=striker.Find("Hips/Right Hip/Right Knee");
            var previous=knee.rotation;var changed=0;
            for(var frame=0;frame<=120;frame++)
            {
                p.Sample(new ShotResult(ShotDirection.Left,ShotDirection.Right),frame/120f);
                var difference=Quaternion.Angle(previous,knee.rotation);
                if(frame>0){Assert.That(difference,Is.LessThan(30),"No discontinuous pose changes");if(difference>.01f)changed++;}
                Assert.That(art.sprite,Is.SameAs(sprite));previous=knee.rotation;
            }
            Assert.That(changed,Is.GreaterThan(80),"Bones move between old sprite frame times");
            p.Sample(new ShotResult(ShotDirection.Left,ShotDirection.Right),ShotPresentation.ContactTime);
            Assert.That(Vector2.Distance(p.ContactBootPosition,p.BallPosition),Is.LessThan(.001f));
            yield return SceneManager.LoadSceneAsync("MainMenu");
        }
        [UnityTest] public IEnumerator CrowdBandsHaveNoUnanimatedHorizontalGaps()
        {
            RuntimeBootstrap.EnsureServices();GameManager.Instance.SelectLocalMultiplayer();GameManager.Instance.SelectMode(GameMode.Endless);
            yield return SceneManager.LoadSceneAsync("Gameplay");yield return null;
            var fans=Object.FindFirstObjectByType<CrowdCelebration>();Assert.That(fans.Sections.Length,Is.EqualTo(12));
            for(var row=0;row<2;row++)
            {
                var edge=0f;
                for(var column=0;column<6;column++){var uv=fans.Sections[row*6+column].GetComponent<RawImage>().uvRect;Assert.That(uv.xMin,Is.EqualTo(edge).Within(.00001f));edge=uv.xMax;}
                Assert.That(edge,Is.EqualTo(1).Within(.00001f));
            }
            fans.Sample(.2f,true);foreach(var section in fans.Sections)Assert.That(section.anchoredPosition.y,Is.GreaterThan(0));
            Assert.That(Object.FindObjectsByType<TargetReticle>(FindObjectsSortMode.None),Is.Empty);
            yield return SceneManager.LoadSceneAsync("MainMenu");
        }
    }
}
