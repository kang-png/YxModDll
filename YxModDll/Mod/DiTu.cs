using Multiplayer;
using System.Collections.Generic;
using UnityEngine;
using YxModDll.Mod;
using YxModDll.Patches;

namespace YxModDll.Mod
{
    internal class DiTu : MonoBehaviour
    {
        public static bool Show;

        private int statusBarHeight = 20;
        private static float kuan = 250f;
        private static float gao = 400f;
        public static Rect windowRect = new Rect(0, Screen.height - gao, kuan, gao);
        private Vector2 gundong_Position;
        // 获取所有订阅关卡并显示为按钮
        public static List<HumanAPI.WorkshopLevelMetadata> subscribedLevels = new List<HumanAPI.WorkshopLevelMetadata>();
        private HumanAPI.WorkshopLevelMetadata nextlevel;
        public static Texture2D texture2D;

        private bool huantu = false;


        public static GUIStyle WindowStyle;
        private void Awake()
        {
            //windowRect = new Rect();
        }
        private void Start()
        {

            Debug.Log("地图模块加载成功！");
        }
        private void Update()
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

            //Show = (YxMod.Show && YxMod.dituxianshiid == 1) || YxMod.dituxianshiid == 0;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("地图加载！");
                Show = !Show;
            }


            if (Show)
            {
                BianYuanJianCe();
            }

            if (huantu && App.state == AppSate.ServerLobby)
            {
                StartGameServer(nextlevel.workshopId, nextlevel.levelType);
                huantu = false;
            }
        }

        private void OnGUI()
        {
            if (Show)
            {
                SetGUIStyle();
                windowRect = GUI.Window(10, windowRect, DrawWindow, "<color=yellow>= 订阅地图 =</color>", WindowStyle);
                //if (YxMod.beijingTexture)
                //{
                //    Rect beijingRect = new Rect(windowRect.xMin + 2, windowRect.yMin + 2, windowRect.width - 4, windowRect.height - 4);
                //    GUI.DrawTexture(beijingRect, YxMod.beijingTexture, ScaleMode.StretchToFill);

                //}

            }
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.Label($"订阅地图：{subscribedLevels.Count}个");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("刷新"))
            {
                Options.multiplayerLobbyLevelStore = 0;
                WorkshopRepository.instance.ReloadSubscriptions(); // 加载订阅的关卡
                subscribedLevels = WorkshopRepository.instance.levelRepo.BySource(WorkshopItemSource.Subscription);
            }
            GUILayout.EndHorizontal();

            gundong_Position = GUILayout.BeginScrollView(gundong_Position, GUILayout.ExpandWidth(true)); //开始滚动视图区域

            if (subscribedLevels.Count > 0)
            {
                for (int i = 0; i < subscribedLevels.Count; i++)
                {
                    HumanAPI.WorkshopLevelMetadata level = subscribedLevels[i];
                    //Chat.TiShi($"{level.title}", TiShiMsgId.GongNengBianGeng);
                    if (GUILayout.Button(level.title))
                    {
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
                                    Chat.TiShi(null,"当前地图已加载");
                                    return;
                                }
                                huantu = true;
                                TuiDaoDaTing();
                                //StartGameServer(level.workshopId, level.levelType);
                            }
                        }
                        else if (NetGame.isClient)
                        {
                            Chat.TiShi("只有房主才可以换图哦");
                        }
                    }
                    Rect rect = GUILayoutUtility.GetLastRect();
                    if (JinRuRect(rect))
                    {
                        texture2D = level.thumbnailTexture;
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();



            //DrawStatusBar();
            GUI.DragWindow(new Rect(0, 0, windowRect.width, windowRect.height)); // 指定拖动区域的大小和位置

            //for (int i = 0; i < hoveredButtons.Count; i++)
            //{
            //    if (subscribedLevels[i].thumbnailTexture && hoveredButtons[i])
            //    {

            //        //beijingTexture = level.thumbnailTexture;
            //        Rect beijingRect = new Rect(0, 0, 200, 200);
            //        GUI.DrawTexture(beijingRect, subscribedLevels[i].thumbnailTexture, ScaleMode.StretchToFill);

            //    }
            //}

        }

        private void TuiDaoDaTing()
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

        public void StartGameServer(ulong level, WorkshopItemSource levelType)
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
        private bool JinRuRect(Rect rect)
        {
            return GUIUtility.hotControl == GUIUtility.keyboardControl && rect.Contains(Event.current.mousePosition);
        }


        /// <summary>
        /// 初始化GUIStyle样式
        /// </summary>
        private void SetGUIStyle()
        {
            if (WindowStyle == null)
            {
                WindowStyle = new GUIStyle(GUI.skin.window);
                // 去掉激活状态和焦点状态的视觉效果
                WindowStyle.normal.background = GUI.skin.window.normal.background;
                WindowStyle.active = WindowStyle.normal;
                WindowStyle.focused = WindowStyle.normal;
                WindowStyle.hover = WindowStyle.normal;
                WindowStyle.onNormal = WindowStyle.normal;
                WindowStyle.onActive = WindowStyle.normal;
                WindowStyle.onFocused = WindowStyle.normal;
                WindowStyle.onHover = WindowStyle.normal;
            }
        }


        /// <summary>
        /// 设置窗体不能超出屏幕
        /// </summary>
        private void BianYuanJianCe()
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
