using Multiplayer;
using System.Collections.Generic;
using UnityEngine;
using YxModDll.Patches;

namespace YxModDll.Mod
{
    internal class UI_HuanTu
    {
        //private static Vector2 huantu_scrollPosition;
        //public static int humanID = -1;
        //public static int yuan_humanID =1;
        //public static int left = UI_Main.chuansongUI_left;
        //public static int top = UI_Main.chuansongUI_top;
        private static float kuan = 260f;
        private static float gao = 400f;
        public static Rect windowRect = new Rect(0, Screen.height - gao, kuan, gao);
        private static Vector2 gundong_Position;
        // 获取所有订阅关卡并显示为按钮
        public static List<HumanAPI.WorkshopLevelMetadata> subscribedLevels = new List<HumanAPI.WorkshopLevelMetadata>();
        private static HumanAPI.WorkshopLevelMetadata nextlevel;
        public static Texture2D texture2D;

        private static bool huantu = false;

        public static void Update()
        {
            if (App.state == AppSate.Startup)
            {
                try
                {
                    WorkshopRepository.instance.ReloadSubscriptions(); // 加载订阅的关卡
                    subscribedLevels = WorkshopRepository.instance.levelRepo.BySource(WorkshopItemSource.Subscription);
                }
                catch { }
            }

            //if (UI_Main.ShowHuanTuUI)
            //{
            //    BianYuanJianCe();
            //}

            if (huantu && App.state == AppSate.ServerLobby)
            {
                StartGameServer(nextlevel.workshopId, nextlevel.levelType);
                huantu = false;
            }
        }
        public static void CreatUI()//创建菜单功能区
        {

            float kehuquHeight = UI_Main.huantuUI_gao - UI_Windows.biaotiUiheight;
            float kehuquWeith = UI_Main.huantuUI_kuan;// - UI_Windows.biankuangsize * 2;

            Rect listRect = new Rect(0, 0, kehuquWeith, kehuquHeight);
            UI.CreatUiBox(listRect, UI_Windows.gongnengquTexture);
            GUILayout.BeginArea(listRect);



            //huantu_scrollPosition = GUILayout.BeginScrollView(huantu_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域



            DrawWindow();



            //GUILayout.EndScrollView();
            GUILayout.EndArea();
        }


        private static void DrawWindow()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            //GUILayout.Label($"订阅地图：{subscribedLevels.Count}个");
            GUILayout.Label(ColorfulSpeek.colorshows($"订阅地图：{subscribedLevels.Count}个"));
            GUILayout.FlexibleSpace();
            UI.CreatAnNiu("刷新", false, ()=>
            {
                Options.multiplayerLobbyLevelStore = 0;
                WorkshopRepository.instance.ReloadSubscriptions(); // 加载订阅的关卡
                subscribedLevels = WorkshopRepository.instance.levelRepo.BySource(WorkshopItemSource.Subscription);
            });

            //if (GUILayout.Button("刷新"))
            //{
            //    Options.multiplayerLobbyLevelStore = 0;
            //    WorkshopRepository.instance.ReloadSubscriptions(); // 加载订阅的关卡
            //    subscribedLevels = WorkshopRepository.instance.levelRepo.BySource(WorkshopItemSource.Subscription);
            //}
            GUILayout.EndHorizontal();

            gundong_Position = GUILayout.BeginScrollView(gundong_Position, GUILayout.ExpandWidth(true)); //开始滚动视图区域

            if (subscribedLevels.Count > 0)
            {
                for (int i = 0; i < subscribedLevels.Count; i++)
                {
                    HumanAPI.WorkshopLevelMetadata level = subscribedLevels[i];
                    //Chat.TiShi($"{level.title}", TiShiMsgId.GongNengBianGeng);

                    string title = level.title;
                    if (title.Length > 12) title = title.Substring(0, 12) + "...";
                    UI.CreatAnNiu_Left(title, false, () =>
                    {
                        //if (GUILayout.Button(level.title))
                        //{
                        nextlevel = level;
                        if (NetGame.isLocal)
                        {
                            YxMod.ChuangJianFangJian();
                            huantu = true;
                            //StartGameServer(level.workshopId, level.levelType);

                        }
                        else if (NetGame.isServer)
                        {
                            if (App.state == AppSate.ServerLobby)
                            {
                                StartGameServer(level.workshopId, level.levelType);
                            }
                            else if (App.state == AppSate.ServerPlayLevel)
                            {
                                if (level.workshopId == NetGame.instance.currentLevel)
                                {
                                    Chat.TiShi(NetGame.instance.local, "当前地图已加载");
                                    return;
                                }
                                huantu = true;
                                TuiDaoDaTing();
                                //StartGameServer(level.workshopId, level.levelType);
                            }
                        }
                        else if (NetGame.isClient)
                        {
                            Chat.TiShi(NetGame.instance.local, "只有房主才可以换图哦");
                        }
                    });
                    Rect rect = GUILayoutUtility.GetLastRect();
                    if (JinRuRect(rect))
                    {
                        texture2D = level.thumbnailTexture;
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

        }

        private static void TuiDaoDaTing()
        {
            Options.multiplayerLobbyLevelStore = 0;
            App.instance.PauseLeave();
            NetGame.instance.transport.SetLobbyStatus(status: false);
        }


        public void StartGameClick(ulong level, WorkshopItemSource levelType)
        {
            Options.multiplayerLobbyLevelStore = level;
            StartGameServer(level, levelType);
            NetGame.instance.transport.SetLobbyStatus(status: true);

            //Options.multiplayerLobbyLevelStore = Game.multiplayerLobbyLevel;
            //StartGameServer(NetGame.instance.currentLevel, NetGame.instance.currentLevelType);
            //NetGame.instance.transport.SetLobbyStatus(status: true);

        }

        public static void StartGameServer(ulong level, WorkshopItemSource levelType)
        {
            App.instance.startedCheckpoint = 0;
            lock (App.stateLock)
            {
                if (App.state == AppSate.ServerLobby)
                {
                    Patcher_App.instance.ExitLobby(startingGame: true);
                }

                NetGame.instance.ServerLoadLevel(level, levelType, start: true, 0u);
                Patcher_App.instance.LaunchGame(level, levelType, 0, 0, delegate
                {
                    App.instance.LevelLoadedServer(level, levelType, Game.currentLevel.netHash);
                });

            }
        }

        /// <summary>
        /// 检查鼠标是否进入RECT
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private static bool JinRuRect(Rect rect)
        {
            return GUIUtility.hotControl == GUIUtility.keyboardControl && rect.Contains(Event.current.mousePosition);
        }



        /// <summary>
        /// 设置窗体不能超出屏幕
        /// </summary>
        private static void BianYuanJianCe()
        {
            if (windowRect.xMin < 0)
            {
                windowRect = new Rect(0, windowRect.yMin, windowRect.width, windowRect.height);
            }
            if (windowRect.yMin < 0)
            {
                windowRect = new Rect(windowRect.xMin, 0, windowRect.width, windowRect.height);
            }
            if (windowRect.xMin + windowRect.width > Screen.width)
            {
                windowRect = new Rect(Screen.width - windowRect.width, windowRect.yMin, windowRect.width, windowRect.height);
            }
            if (windowRect.yMin + windowRect.height > Screen.height)
            {
                windowRect = new Rect(windowRect.xMin, Screen.height - windowRect.height, windowRect.width, windowRect.height);
            }
        }


    }
}
