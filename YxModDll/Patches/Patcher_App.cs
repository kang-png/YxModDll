using Multiplayer;
using Steamworks;
using System;
using System.Reflection;
using UnityEngine;

namespace YxModDll.Patches
{
    public class Patcher_App : MonoBehaviour
    {
        private static FieldInfo field_serverLoadedLevel;
        private static FieldInfo field_loadedLevel;
        private static FieldInfo field_serverLoadedHash;
        private static FieldInfo field_loadedHash;
        private static FieldInfo field_stateLock;
        private static FieldInfo field_state;
        private static FieldInfo field_queueAfterLevelLoad;

        private static MethodInfo method_ExitGame;
        private static MethodInfo method_EnterMenu;

        private void Awake()
        {
            Debug.Log("[YxMod] 初始化 LevelLoadedClient Patch");

            Type appType = typeof(App);
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            field_serverLoadedLevel = appType.GetField("serverLoadedLevel", flags);
            field_loadedLevel = appType.GetField("loadedLevel", flags);
            field_serverLoadedHash = appType.GetField("serverLoadedHash", flags);
            field_loadedHash = appType.GetField("loadedHash", flags);
            field_stateLock = appType.GetField("stateLock", flags);
            field_state = appType.GetField("state", flags);
            field_queueAfterLevelLoad = appType.GetField("queueAfterLevelLoad", flags);

            method_ExitGame = appType.GetMethod("ExitGame", flags);
            method_EnterMenu = appType.GetMethod("EnterMenu", flags);

            Patcher2.MethodPatch(
                appType,
                "LevelLoadedClient",
                Type.EmptyTypes,
                typeof(Patcher_App),
                nameof(LevelLoadedClient_Patched),
                Type.EmptyTypes
            );
        }

        public static bool LevelLoadedClient_Patched(App __instance)
        {
            object stateLock = field_stateLock.GetValue(__instance);

            lock (stateLock)
            {
                int serverLoadedLevel = (int)field_serverLoadedLevel.GetValue(__instance);
                int loadedLevel = (int)field_loadedLevel.GetValue(__instance);
                int serverLoadedHash = (int)field_serverLoadedHash.GetValue(__instance);
                int loadedHash = (int)field_loadedHash.GetValue(__instance);

                if (serverLoadedLevel == loadedLevel)
                {
                    Dialogs.HideProgress();
                    MenuCameraEffects.FadeOut(1f);

                    if (loadedHash == serverLoadedHash)
                    {
                        field_state.SetValue(__instance, AppSate.ClientPlayLevel);
                    }
                    else
                    {
                        if (serverLoadedHash == 57005)
                        {
                            NetGame.instance.LeaveGame();
                            // 调用 private 方法
                            method_ExitGame.Invoke(__instance, null);
                            method_EnterMenu.Invoke(__instance, null);

                            Dialogs.ConnectionFailed("LevelNotNetReady", delegate
                            {
                                MenuSystem.instance.ShowMainMenu<MainMenu>();
                            });

                            field_queueAfterLevelLoad.SetValue(__instance, null);
                        }
                        else
                        {
                            Debug.Log("服务器上的版本与此关卡不同");
                            Debug.LogError("Incompatible level. Server hash: " + serverLoadedHash + ". Client Hash: " + loadedHash);
                        }
                    }

                    return true;
                }

                return false;
            }
        }
    }
}
