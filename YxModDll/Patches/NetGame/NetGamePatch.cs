using HarmonyLib;
using Multiplayer;
using UnityEngine;

[HarmonyPatch(typeof(NetGame), "Awake")]
public static class NetGame_Awake_Patch
{
    static void Postfix(NetGame __instance)
    {
        Debug.Log("[YxMod] NetGame Awake - Injecting YxMod");
        __instance.gameObject.AddComponent<YxMod>();
    }
}

