//using HarmonyLib;
using Multiplayer;
using UnityEngine;
using YxModDll.Mod;

namespace YxModDll.Patches
{
    //[HarmonyPatch(typeof(NetGame))]
    public static class NetGame_Patches
    {
        //// NetGame.Awake 注入 YxMod 组件
        //[HarmonyPostfix]
        //[HarmonyPatch("Awake")]
        //public static void NetGameAwake_Postfix(NetGame __instance)
        //{
        //    Debug.Log("[YxMod] NetGame Awake - Injecting YxMod");
        //    __instance.gameObject.AddComponent<YxMod>();
        //}

        //// 完全替换 OnServerReceive
        //[HarmonyPrefix]
        //[HarmonyPatch("OnServerReceive")]
        //public static bool OnServerReceive_Prefix(NetGame __instance, NetHost client, NetStream stream)
        //{
        //    NetGameHandler.OnServerReceive(__instance, client, stream);
        //    // 返回 false 阻止执行原方法，实现完全替换
        //    return false;
        //}

        //// 完全替换 OnClientReceive
        //[HarmonyPrefix]
        //[HarmonyPatch("OnClientReceive")]
        //public static bool OnClientReceive_Prefix(NetGame __instance, object connection, NetStream stream)
        //{
        //    NetGameHandler.OnClientReceive(__instance, connection, stream);
        //    return false;
        //}
    }
}
