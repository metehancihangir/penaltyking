using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using static PenaltyKing.Editor.PhaseOneSetup;
namespace PenaltyKing.Editor
{
    public static class UpdatePhaseFourSetup
    {
        private static Color C(string hex) => MenuPixelArt.Hex(hex);
        [MenuItem("Penalty King/Updates/Phase 4 - Build full screen HUD")]
        public static void Build()
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var game = Object.FindFirstObjectByType<GameplayController>();
            var canvas = GameObject.Find("Gameplay Canvas").transform;
            var stage = Object.FindFirstObjectByType<GameplayStageLayout>();
            stage.transform.SetParent(canvas,false); stage.transform.SetSiblingIndex(1);
            foreach (var name in new[] { "Mode", "Score", "Shots", "Feedback", "Directions" }) canvas.Find("Safe Area/" + name).gameObject.SetActive(false);
            foreach (var zone in new[] { game.Left, game.Center, game.Right })
                zone.transform.Find("Direction").gameObject.SetActive(false);
            // Preserve the existing exit button when rebuilding the HUD.
            game.Exit.transform.SetParent(canvas,false);
            foreach (var name in new[] { "Live HUD", "In Game Settings", "Match Controls" })
            { var old = canvas.Find(name); if (old != null) Object.DestroyImmediate(old.gameObject); }
            var safe = Rect("Live HUD",canvas,Vector2.zero,Vector2.zero); Stretch(safe); safe.gameObject.AddComponent<SafeAreaPanel>();
            var board = Image("Scoreboard",safe,new Vector2(760,124),Vector2.zero,C("101E2B"));
            board.rectTransform.anchorMin = board.rectTransform.anchorMax = new Vector2(.5f,1);
            board.rectTransform.pivot = new Vector2(.5f,1); board.rectTransform.anchoredPosition = new Vector2(0,-12);
            Image("Top edge",board.transform,new Vector2(760,4),new Vector2(0,60),C("3A6170"));
            var first = Label("PLAYER 1",board.transform,"PLAYER 1",new Vector2(184,40),new Vector2(-173,24),27,C("BAEB71"));
            var second = Label("PLAYER 2",board.transform,"PLAYER 2",new Vector2(184,40),new Vector2(173,24),27,C("E3EEE6"));
            var score = Label("Score",board.transform,"0 - 0",new Vector2(140,54),new Vector2(0,18),40,C("E3EEE6"));
            score.resizeTextForBestFit = true; score.resizeTextMinSize=18; score.resizeTextMaxSize=40;
            var role = Label("Role",board.transform,"ŞUT",new Vector2(140,30),new Vector2(0,-30),18,C("AEC4C0"));
            var a1=Image("First active",board.transform,new Vector2(180,4),new Vector2(-173,-56),C("BAEB71"));
            var a2=Image("Second active",board.transform,new Vector2(180,4),new Vector2(173,-56),C("304957"));
            var shots1 = Shots(board.transform,-173,out var marks1); var shots2=Shots(board.transform,173,out var marks2);
            var scoreboard = board.gameObject.AddComponent<MatchScoreboard>();
            scoreboard.Configure(game,first,second,score,role,a1,a2,shots1,shots2,marks1,marks2);
            var exitRect=(RectTransform)game.Exit.transform; exitRect.SetParent(board.transform,false);
            exitRect.anchorMin=exitRect.anchorMax=exitRect.pivot=new Vector2(.5f,.5f); exitRect.anchoredPosition=new Vector2(-326,0);
            exitRect.sizeDelta=new Vector2(90,90); game.Exit.Face.sizeDelta=new Vector2(90,90);
            var label=game.Exit.Face.Find("Label").GetComponent<Text>(); label.text="Menü"; label.fontSize=21; label.rectTransform.sizeDelta=new Vector2(88,70);
            var gearRoot=Rect("Settings Button",board.transform,new Vector2(90,90),new Vector2(326,0));
            var face=Image("Face",gearRoot,new Vector2(90,90),Vector2.zero,Color.white); face.raycastTarget=true;
            var gear=gearRoot.gameObject.AddComponent<PixelMenuButton>();gear.Face=face.rectTransform;gear.targetGraphic=face;gear.colors=game.Exit.colors;gear.navigation=new Navigation{mode=Navigation.Mode.None};
            DrawGear(face.transform);

            var settingsRoot=Rect("In Game Settings",canvas,Vector2.zero,Vector2.zero); Stretch(settingsRoot);
            var shade=Image("Settings Shade",settingsRoot,Vector2.zero,Vector2.zero,new Color(.02f,.05f,.08f,.92f)); Stretch(shade.rectTransform);shade.raycastTarget=true;
            var settingsSafe=Rect("Safe Area",shade.transform,Vector2.zero,Vector2.zero);Stretch(settingsSafe);settingsSafe.gameObject.AddComponent<SafeAreaPanel>();
            var optionsScene=EditorSceneManager.OpenScene("Assets/Scenes/Options.unity",OpenSceneMode.Additive);
            var source=GameObject.Find("Options Panel");
            var panel=Object.Instantiate(source,settingsSafe);panel.name="Options Panel";
            EditorSceneManager.CloseScene(optionsScene,true);
            panel.AddComponent<FitPanelToSafeArea>();
            var options=panel.AddComponent<OptionsController>();
            var back=panel.transform.Find("Geri Button").GetComponent<PixelMenuButton>();back.name="Continue Button";back.Face.Find("Label").GetComponent<Text>().text="Devam Et";
            options.Configure(panel.transform.Find("Music Volume Group/Music Volume Slider").GetComponent<Slider>(),panel.transform.Find("SFX Volume Group/SFX Volume Slider").GetComponent<Slider>(),panel.transform.Find("Music Volume Group/Percentage").GetComponent<Text>(),panel.transform.Find("SFX Volume Group/Percentage").GetComponent<Text>(),back);
            options.ConfigureVibration(panel.transform.Find("Vibration Toggle").GetComponent<Toggle>());options.ConfigureEmbedded();
            var controller=settingsRoot.gameObject.AddComponent<InGameSettings>();controller.Configure(game,gear,shade.gameObject,options);
            shade.gameObject.SetActive(false);
            Canvas.ForceUpdateCanvases();stage.Fit();
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            Capture();
            EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
            Debug.Log("[Update Phase 4] Full screen field, scoreboard and embedded settings saved.");
        }
        private static Image[] Shots(Transform parent,float x,out Text[] marks)
        {
            var boxes=new Image[5];marks=new Text[5];
            for(var i=0;i<5;i++)
            {
                boxes[i]=Image("Attempt",parent,new Vector2(24,24),new Vector2(x+(i-2)*34,-24),C("304957"));
                marks[i]=Label("Mark",boxes[i].transform,"",new Vector2(24,32),Vector2.zero,20,C("101E2B"));
            }
            return boxes;
        }
        private static void DrawGear(Transform parent)
        {
            var rows=new[]{"000111000","010111010","111111111","111000111","111000111","111000111","111111111","010111010","000111000"};
            for(var y=0;y<9;y++)for(var x=0;x<9;x++)if(rows[y][x]=='1')Image("Gear pixel",parent,new Vector2(4,4),new Vector2((x-4)*4,(4-y)*4),C("E3EEE6"));
        }
        public static void Capture()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
            var scoreboard=Object.FindFirstObjectByType<MatchScoreboard>();
            var round=new PenaltyRound(GameMode.FixedRound,5);
            foreach(var save in new[]{false,true,false,false}){round.ChooseShot(ShotDirection.Left);round.ChooseKeeper(save?ShotDirection.Left:ShotDirection.Right);}
            scoreboard.Render(round,PlayState.Ready);
            MenuPreview.Render(390,844,"portrait","update4-gameplay");
            MenuPreview.Render(1280,720,"landscape","update4-gameplay");
            var settings=Object.FindFirstObjectByType<InGameSettings>();settings.transform.Find("Settings Shade").gameObject.SetActive(true);
            MenuPreview.Render(390,844,"portrait","update4-settings");
            MenuPreview.Render(1280,720,"landscape","update4-settings");
        }
    }
}
