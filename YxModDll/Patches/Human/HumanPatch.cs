using HarmonyLib;
using UnityEngine;

namespace YxModDll.Patches
{
    [HarmonyPatch(typeof(Human))]
    public static class HumanPatch
    {
        // Patch Human.Initialize
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Human.Initialize))]
        public static void Initialize_Postfix(Human __instance)
        {
            var ext = HumanStateExtHelper.GetExt(__instance);
            ext.human = __instance; // ✅ 必须设置 human 引用！
            ext.YxModChuShiHua(); // 可选：调用你的初始化逻辑
        }

        //// 完全替换 FixedUpdate
        //[HarmonyPrefix]
        //[HarmonyPatch("FixedUpdate")]
        //public static bool FixedUpdate_Prefix(Human __instance)
        //{
        //    var ext = HumanStateExtHelper.GetExt(__instance);

        //    // 确保 ext.human 和 ext.dingdian 初始化过
        //    if (ext.human == null || ext.dingdian == null)
        //    {
        //        Debug.LogWarning("[YxMod] ext.human 或 dingdian 未初始化，正在执行懒加载初始化");
        //        ext.human = __instance;
        //        ext.YxModChuShiHua();
        //        return false; // 可选：本次跳过 FixedUpdate 逻辑
        //    }

        //    ext.YxModFixedUpdate();
        //    return true;
        //}

        //// 完全替换 ProcessInput
        //[HarmonyPrefix]
        //[HarmonyPatch("ProcessInput")]
        //public static bool ProcessInput_Prefix(Human __instance)
        //{
        //    var ext = HumanStateExtHelper.GetExt(__instance);
        //    ext.human = __instance; // ✅ 必须设置 human 引用！

        //    // 假设你把numY、yititui、titui、quanji字段也放到了ext里
        //    ext.YxModProcessInput();

        //    return false; // 阻止原 ProcessInput 执行
        //}

    }
}