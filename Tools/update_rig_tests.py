from pathlib import Path
p=Path('Assets/Tests/PlayMode/SixTargetTests.cs');s=p.read_text(encoding='utf-8-sig')
a=s.index('                var scale=keeperImage.rectTransform.rect.height/');b=s.index('\n            }',a)
s=s[:a]+'''                Assert.That(Vector2.Distance(p.GripPosition,p.BallPosition),Is.LessThan(.01f),"Two hands must meet "+direction);'''+s[b:]
a=s.index('            foreach(var zone in game.Targets)');b=s.index('            for(var i=0;i<6;i++)',a)
s=s[:a]+'''            foreach(var zone in game.Targets)Assert.That(zone.GetComponentInChildren<TargetReticle>(),Is.Null,"Aim markers were removed");
'''+s[b:];p.write_text(s,encoding='utf-8')
p=Path('Assets/Tests/PlayMode/ActionRevisionTests.cs');s=p.read_text(encoding='utf-8-sig')
s=s.replace('Assert.That(player.sprite.name,Is.EqualTo("StrikerShot12_03"));','Assert.That(p.UsesBoneRigs,Is.True);')
s=s.replace('Assert.That(player.sprite.name,Is.EqualTo("StrikerShot12_08"));','Assert.That(player.enabled,Is.False,"Bone artwork replaces the pose image");')
s=s.replace('Assert.That(player.sprite.name,Is.EqualTo("Adult_10"));','Assert.That(p.GoalCelebrating,Is.True);')
s=s.replace('Assert.That(player.sprite.name,Is.EqualTo("StrikerShot12_00"));','Assert.That(p.UsesBoneRigs,Is.True);');p.write_text(s,encoding='utf-8')
p=Path('Assets/Tests/PlayMode/StrikerWideTests.cs');s=p.read_text(encoding='utf-8-sig')
s=s.replace('AllGoalsRestOnTheirShadowBehindThePlayerAndShotHasEightPoses','AllGoalsRestOnTheirShadowBehindThePlayerAndBonesMoveContinuously')
s=s.replace('var frames=new HashSet<string>();','var frames=new HashSet<float>();')
s=s.replace('frames.Add(player.sprite.name);','frames.Add(player.transform.Find("Hips/Right Hip/Right Knee").eulerAngles.z);')
s=s.replace('Assert.That(frames.Count,Is.EqualTo(8));','Assert.That(frames.Count,Is.GreaterThanOrEqualTo(6));');p.write_text(s,encoding='utf-8')
p=Path('Assets/Tests/PlayMode/ShotAnimationTests.cs');s=p.read_text(encoding='utf-8-sig')
a=s.index('                Assert.That(animation.KeeperSprite.name, Is.EqualTo(direction');b=s.index('\n            }',a)
s=s[:a]+'''                Assert.That(animation.UsesBoneRigs,Is.True);
                Assert.That(Vector2.Distance(animation.GripPosition,animation.BallPosition),Is.LessThan(.01f));
                Assert.That(animation.CrowdCelebrating,Is.False);
                animation.Sample(new ShotResult(direction,direction),ShotPresentation.ImpactTime+.8f);
                Assert.That(Vector2.Distance(animation.GripPosition,animation.BallPosition),Is.LessThan(.01f),"Ball stays caught during landing");'''+s[b:];p.write_text(s,encoding='utf-8')
