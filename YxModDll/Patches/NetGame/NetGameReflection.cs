using HarmonyLib;
using Multiplayer;
using System;
using System.Reflection;


    namespace YxModDll.Patches
{
    public static class NetGameReflection
    {
        private static readonly Type NetGameType = typeof(NetGame);

        // 1. 获取 private static object stateLock
        public static object GetStateLock()
        {
            return AccessTools.Field(NetGameType, "stateLock").GetValue(null);
        }

        // 2. 封装反射调用器
        private static void InvokePrivateMethod(NetGame instance, string methodName, params object[] args)
        {
            var method = AccessTools.Method(NetGameType, methodName);
            if (method == null)
            {
                UnityEngine.Debug.LogError($"[YxMod] 未找到方法: {methodName}");
                return;
            }
            method.Invoke(instance, args);
        }

        // 单独封装每个调用（你也可以选择只用上面那种通用调用）
        public static void OnReceiveLevelAck(NetGame instance, NetHost client, NetStream stream)
            => InvokePrivateMethod(instance, "OnReceiveLevelAck", client, stream);

        public static void OnLoadLevel(NetGame instance, NetStream stream)
            => InvokePrivateMethod(instance, "OnLoadLevel", stream);

        public static void OnRequestAddPlayerServer(NetGame instance, NetHost client, NetStream stream)
            => InvokePrivateMethod(instance, "OnRequestAddPlayerServer", client, stream);

        public static void OnRequestRemovePlayerServer(NetGame instance, NetHost client, NetStream stream)
            => InvokePrivateMethod(instance, "OnRequestRemovePlayerServer", client, stream);

        public static void DestroyHostObjects(NetGame instance, NetHost client)
            => InvokePrivateMethod(instance, "DestroyHostObjects", client);

        public static void OnAddPlayerClient(NetGame instance, NetStream stream)
            => InvokePrivateMethod(instance, "OnAddPlayerClient", stream);

        public static void OnRemovePlayerClient(NetGame instance, NetStream stream)
            => InvokePrivateMethod(instance, "OnRemovePlayerClient", stream);

        public static void OnRequestSkinClient(NetGame instance, uint localCoopIndex)
            => InvokePrivateMethod(instance, "OnRequestSkinClient", localCoopIndex);

        public static void OnRequestSkinServer(NetGame instance, NetHost client, NetStream stream)
            => InvokePrivateMethod(instance, "OnRequestSkinServer", client, stream);

        public static void OnReceiveSkin(NetGame instance, NetStream stream)
            => InvokePrivateMethod(instance, "OnReceiveSkin", stream);
    }
}

