using Multiplayer;
using System;
using System.Reflection;
using UnityEngine;
using YxModDll.Patches;

namespace YxModDll.Patches
{
    public class Patcher_App : MonoBehaviour
    {
        private static FieldInfo _serverLoadedLevel;
        private static FieldInfo _loadedLevel;
        private static FieldInfo _serverLoadedHash;
        private static FieldInfo _loadedHash;
        private static FieldInfo _stateLock;

        private void Awake()
        {
            Debug.Log("[YxMod] 初始化 LevelLoadedClient Patch");

            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            Type appType = typeof(App);

            _serverLoadedLevel = appType.GetField("serverLoadedLevel", flags);
            _loadedLevel = appType.GetField("loadedLevel", flags);
            _serverLoadedHash = appType.GetField("serverLoadedHash", flags);
            _loadedHash = appType.GetField("loadedHash", flags);
            _stateLock = appType.GetField("stateLock", flags);

            Patcher2.MethodPatch(
                appType,
                "LevelLoadedClient",
                Type.EmptyTypes,
                typeof(Patcher_App),
                "LevelLoadedClient_Patched",
                Type.EmptyTypes
            );
        }

        public static bool LevelLoadedClient_Patched(App __instance)
        {
            object stateLock = _stateLock.GetValue(__instance);
            lock (stateLock)
            {
                int serverLevel = (int)_serverLoadedLevel.GetValue(__instance);
                int clientLevel = (int)_loadedLevel.GetValue(__instance);
                int serverHash = (int)_serverLoadedHash.GetValue(__instance);
                int clientHash = (int)_loadedHash.GetValue(__instance);

                if (serverLevel == clientLevel)
                {
                    Dialogs.HideProgress();
                    MenuCameraEffects.FadeOut(1f);

                    if (clientHash != serverHash)
                    {
                        if (serverHash == 57005)
                        {
                            Debug.LogWarning("[YxMod] Hash 为 57005，服务器未准备好，默认允许继续！");
                            // 可以选择 return false; 或其他处理
                        }
                        else
                        {
                            Debug.LogWarning($"[YxMod] Hash 不一致，Server: {serverHash} Client: {clientHash}，强制进入游戏");
                        }
                    }

                    App.state = AppSate.ClientPlayLevel;
                    return true;
                }

                return false;
            }
        }
    }
}
