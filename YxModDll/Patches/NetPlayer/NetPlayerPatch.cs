//using HarmonyLib;
using Multiplayer;
using YxModDll.Mod;

namespace YxModDll.Patches
{
    //[HarmonyPatch(typeof(NetPlayer), nameof(NetPlayer.PreFixedUpdate))]
    public static class NetPlayer_PreFixedUpdate_Patch
    {
        //public static bool Prefix(NetPlayer __instance)
        //{
        //    // 将原来的 if 判断逻辑放入 Prefix 中
        //    if (__instance.isLocalPlayer &&
        //        ((UI_Main.ShowShuBiao && UI_SheZhi.noKong_xianshishubiao) || FreeRoamCam.allowFreeRoam))
        //    {
        //        return false; // 阻止原始 PreFixedUpdate 执行
        //    }

        //    return true; // 继续执行原方法
        //}
    }
}
