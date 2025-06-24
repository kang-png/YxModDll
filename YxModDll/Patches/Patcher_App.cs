using HumanAPI;
using Multiplayer;
using Steamworks;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;
using YxModDll.Mod;
using YxModDll.Patches;
using static MenuCameraEffects;


namespace YxModDll.Patches
{
    public class Patcher_App : MonoBehaviour
    {
        private static FieldInfo _previousLobbyID;
        private static FieldInfo _queueAfterLevelLoad;

        public static Patcher_App instance;
        //	private Action queueAfterLevelLoad;
        //private ulong previousLobbyID;
        //public static bool BeiTi;//被踢
        private void Awake()
        {
            instance = this;
            _previousLobbyID = typeof(App).GetField("previousLobbyID", BindingFlags.NonPublic | BindingFlags.Instance);
            _queueAfterLevelLoad = typeof(App).GetField("queueAfterLevelLoad", BindingFlags.NonPublic | BindingFlags.Instance);

            Patcher2.MethodPatch(typeof(App), "ServerKicked", null, typeof(Patcher_App), "ServerKicked", new Type[] { typeof(App) });

            //Patcher2.MethodPatch(typeof(App), "ExitLobby", new[] { typeof(ulong) }, typeof(Patcher_App), "ExitLobby", new[] { typeof(ulong) });
            //Patcher2.MethodPatch(typeof(App), "LaunchGame", new[] { typeof(ulong), typeof(WorkshopItemSource), typeof(int), typeof(int), typeof(Action) }, typeof(Patcher_App), "LaunchGame", new[] { typeof(ulong), typeof(WorkshopItemSource), typeof(int), typeof(int), typeof(Action) });

        }

        private void OnGUI()
        {
            //if (GUILayout.Button("开关"))
            //{

            //}
        }
        public static void ServerKicked(App instance)
        {
            Chat.TiShi(null, "被踢了，呜呜~");
            return;
            //////修改
            //RatingMenu.instance.LevelOver();
            //KillGame();
            //if (RatingMenu.instance.ShowRatingMenu())
            //{
            //    GotoRatingsMenu(RatingMenu.RatingDestination.kLostConnection, showLoading: false);
            //    return;
            //}
            //Dialogs.ConnectionKicked(delegate
            //{
            //    MenuSystem.instance.ShowMainMenu();
            //});
        }

        public void LaunchGame(ulong level, WorkshopItemSource levelType, int checkpoint, int subObjectives, Action onComplete)
        {
            var previousLobbyID = (ulong)_previousLobbyID.GetValue(App.instance);


            StopPlaytimeForItem(previousLobbyID);
            previousLobbyID = 0uL;
            MenuSystem.instance.HideMenus();
            CheatCodes.cheatMode = false;
            if (levelType == WorkshopItemSource.BuiltIn && level == 0 && checkpoint == 0)
            {
                Game.instance.singleRun = true;
            }
            SuspendDeltasForLoad();
            StartCoroutine(LaunchGame(null, level, levelType, checkpoint, subObjectives, delegate
            {
                onComplete();
            }));
        }
        private IEnumerator LaunchGame(string levelPath, ulong level, WorkshopItemSource type, int checkpoint, int subObjectives, Action onComplete)
        {
            var queueAfterLevelLoad = (Action)_queueAfterLevelLoad.GetValue(App.instance);


            lock (App.stateLock)
            {
                if (App.state == AppSate.Menu || App.state == AppSate.PlayLevel || App.state == AppSate.ClientJoin || App.state == AppSate.ClientLobby || App.state == AppSate.ClientPlayLevel || App.state == AppSate.ClientWaitServerLoad || App.state == AppSate.ServerLobby || App.state != AppSate.ServerPlayLevel)
                {
                }
                bool ui = App.state == AppSate.Menu || App.state == AppSate.ServerLobby || App.state == AppSate.ClientLobby || App.state == AppSate.ClientJoin;
                if (ui)
                {
                    MenuSystem.instance.FadeOutActive();
                }
                if (App.isServer || App.isClient)
                {
                    MenuCameraEffects.FadeToBlack((!ui) ? 0.02f : 0.2f);
                    Dialogs.ShowLoadLevelProgress(level);
                }
                if (App.isServer)
                {
                    App.state = AppSate.ServerLoadLevel;
                }
                else if (App.isClient)
                {
                    App.state = AppSate.ClientLoadLevel;
                }
                else
                {
                    App.state = AppSate.LoadLevel;
                }
                queueAfterLevelLoad = null;
                if (ui)
                {
                    yield return new WaitForSeconds(0.2f);
                }
                NetStream.DiscardPools();
                Game.instance.BeginLoadLevel(levelPath, level, checkpoint, subObjectives, delegate
                {
                    lock (App.stateLock)
                    {
                        if (App.state == AppSate.LoadLevel || App.state == AppSate.ServerLoadLevel || App.state != AppSate.ClientLoadLevel)
                        {
                        }
                        MenuSystem.instance.ExitMenus();
                        NetStream.DiscardPools();
                        App.instance.ResumeDeltasAfterLoad();
                        if (onComplete != null)
                        {
                            onComplete();
                        }
                        if (queueAfterLevelLoad != null)
                        {
                            Action action = queueAfterLevelLoad;
                            queueAfterLevelLoad = null;
                            if (NetGame.netlog)
                            {
                                UnityEngine.Debug.Log("Executing queue");
                            }
                            action();
                        }
                    }
                }, type);
            }
        }
        private static void StopPlaytimeForItem(ulong item)
        {
            if (item != 0)
            {
                SteamUGC.StopPlaytimeTracking(new PublishedFileId_t[1]
                {
                new PublishedFileId_t(item)
                }, 1u);
            }
        }
        private void SuspendDeltasForLoad()
        {
            NetGame.instance.ignoreDeltasMask |= 2u;
        }
        //公开修改
        public void ExitLobby(bool startingGame = false)
        {
            lock (App.stateLock)
            {
                UpdateJoinable(startingGame);
                MultiplayerLobbyController.Teardown();
            }
        }
        private void UpdateJoinable(bool startingGame = false)
        {
            if (!NetGame.isServer)
            {
                return;
            }
            NetTransport transport = NetGame.instance.transport;
            if (NetGame.instance.players.Count < Options.lobbyMaxPlayers)
            {
                UnityEngine.Debug.Log("UpdateJoinable : hasSlots");
                if (App.state == AppSate.ServerLobby && !startingGame)
                {
                    UnityEngine.Debug.Log("UpdateJoinable : in lobby");
                    transport.SetJoinable(joinable: true, haveStarted: false);
                }
                else
                {
                    UnityEngine.Debug.Log("UpdateJoinable : join in progress = " + Options.lobbyJoinInProgress);
                    transport.SetJoinable(Options.lobbyJoinInProgress != 0, haveStarted: true);
                }
            }
            else
            {
                UnityEngine.Debug.Log("UpdateJoinable : noSlots");
                transport.SetJoinable(joinable: false, App.state != AppSate.ServerLobby || startingGame);
            }
        }
    }
}

