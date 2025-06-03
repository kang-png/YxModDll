using HarmonyLib;
using UnityEngine;

    [HarmonyPatch(typeof(Human))]
    public static class HumanPatch
    {
        // Patch Human.Initialize
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Human.Initialize))]
        public static void Initialize_Postfix(Human __instance)
        {
            var ext = HumanStateExtHelper.GetExt(__instance);
            ext.YxModChuShiHua(); // <== 调用你放在 HumanStateExt 里的初始化方法
        }

        // 完全替换 FixedUpdate
        [HarmonyPrefix]
        [HarmonyPatch("FixedUpdate")]
        public static bool FixedUpdate_Prefix(Human __instance)
        {
            var ext = HumanStateExtHelper.GetExt(__instance);

            // 调用你扩展类的替代 FixedUpdate 逻辑
            ext.YxModFixedUpdate();

            // 返回 false，阻止执行原 FixedUpdate
            return false;
        }

        // 完全替换 ProcessInput
        [HarmonyPrefix]
        [HarmonyPatch("ProcessInput")]
        public static bool ProcessInput_Prefix(Human __instance)
        {
            var ext = HumanStateExtHelper.GetExt(__instance);

            // 假设你把numY、yititui、titui、quanji字段也放到了ext里
            ext.YxModProcessInput();

            return false; // 阻止原 ProcessInput 执行
        }

    }