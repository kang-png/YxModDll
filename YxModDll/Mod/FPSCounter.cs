using UnityEngine;
using Multiplayer;
using HumanAPI;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using I2.Loc;
using Steamworks;

namespace YxModDll.Mod
{
    public class FPSCounter : MonoBehaviour
    {
        private float elapsedTime;
        private int frameCount;
        private int fps;
        private float latency;

        public static bool showFPS;

        private string levelTitle = "加载中...";
        private Text LevelTextRef;

        private GUIStyle style;


        private void GetlevelTitle()
        {
            if (App.state == AppSate.ServerPlayLevel || App.state == AppSate.ClientPlayLevel)
            {
                if (levelTitle == "加载中...")
                {
                    if (Game.instance.workshopLevel != null)
                    {
                        levelTitle = Game.instance.workshopLevel.title;
                        return;
                    }

                    string path;
                    switch (Game.instance.currentLevelType)
                    {
                        case WorkshopItemSource.BuiltIn:
                            WorkshopRepository.instance.LoadBuiltinLevels();
                            path = "builtin:" + Game.instance.currentLevelNumber;//  LevelID
                            break;
                        case WorkshopItemSource.EditorPick:
                            WorkshopRepository.instance.LoadEditorPickLevels();
                            path = "editorpick:" + Game.instance.currentLevelNumber; //dispInfo.LevelID;
                            break;
                        default:
                            path = "ws:" + Game.instance.currentLevelNumber; //dispInfo.LevelID + "/";
                            break;
                    }
                    WorkshopLevelMetadata item = WorkshopRepository.instance.levelRepo.GetItem(path);
                    if (item != null)
                    {
                        levelTitle = item.title;
                        //LevelImage.texture = item.thumbnailTexture;
                        //LevelImage.enabled = LevelImage.texture != null;
                    }
                    else if (Game.instance.currentLevelNumber == (Game.instance.levels.Length - 1))
                    {
                        levelTitle = ScriptLocalization.Get("LEVEL/" + Game.instance.levels[Game.instance.levels.Length - 1]);
                        //LevelImage.texture = HFFResources.instance.FindTextureResource("LevelImages/" + Game.instance.levels[Game.instance.levels.Length - 1]);
                        //LevelImage.enabled = true;
                    }
                }
            }
            else
            {
                levelTitle = "加载中...";
            }
        }

        void Update()
        {
            if (!showFPS) return;


            elapsedTime += Time.deltaTime;
            frameCount++;

            if (elapsedTime >= 0.5f)
            {
                fps = Mathf.RoundToInt(frameCount / elapsedTime);
                elapsedTime = 0f;
                frameCount = 0;

                latency = NetGame.instance?.clientLatency?.latency ?? 0;

                GetlevelTitle();

                //if (Game.instance.workshopLevel == null)
                //{

                //        List<string> list = TutorialRepository.instance.ListShownItems(Game.instance.currentLevelNumber, Game.instance.currentCheckpointNumber + 1);
                //        while (list.Count > 5)
                //        {
                //            list.RemoveAt(0);
                //        }
                //    levelTitle = string.Join("\r\n-----\r\n", list.ToArray());

                //}
                //else
                //{
                //    levelTitle = Game.instance.workshopLevel.title;
                //}



                //// 每帧尝试绑定一次 LevelText，如果还没找到
                //if (LevelTextRef == null)
                //{
                //    var infoBox = GameObject.FindObjectOfType<LevelInformationBox>();
                //    if (infoBox != null)
                //    {
                //        LevelTextRef = infoBox.LevelText;
                //    }
                //}

                //// 读取标题
                //if (LevelTextRef != null)
                //{
                //    levelTitle = LevelTextRef.text;
                //}


            }
        }

        private void OnGUI()
        {
            if (!showFPS) return;
            if (style == null)
            {
                style = new GUIStyle
                {
                    fontSize = 22,
                    normal = { textColor = Color.white }
                };
            }
            GUILayout.BeginArea(new Rect(10, 10, 500, 200));

            GUILayout.Label($"帧数: {fps} FPS", style);
            GUILayout.Label($"延迟: {latency:F0} ms", style);
            GUILayout.Label($"时间: {DateTime.Now:HH:mm:ss}", style);
            GUILayout.Label($"图名: {levelTitle}", style);

            if (Game.instance != null && Game.currentLevel != null && Game.currentLevel.checkpoints != null)
            {
                int total = Game.currentLevel.checkpoints.Length;
                int cur = Mathf.Clamp(Game.instance.currentCheckpointNumber, 0, total - 1);
                GUILayout.Label($"进度: {cur + 1}/{total}", style);
            }

            GUILayout.EndArea();
        }
    }
}
