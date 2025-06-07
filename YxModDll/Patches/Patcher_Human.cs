using System.Reflection;
using UnityEngine;

namespace YxModDll.Patches
{
    public class Patcher_Human : MonoBehaviour
    {
        private static MethodInfo _initializeBodies;
        private static FieldInfo _humanHead;
        private void Awake()
        {
            _initializeBodies = typeof(Human).GetMethod("InitializeBodies", BindingFlags.Instance | BindingFlags.NonPublic);
            _humanHead = typeof(Human).GetField("humanHead", BindingFlags.Instance | BindingFlags.NonPublic);
            try
            {
                Patcher2.MethodPatch(
                    typeof(Human),
                    "Initialize",
                    new System.Type[] { },
                    typeof(Patcher_Human),
                    nameof(Patcher_Human.Initialize_Replace),
                    new System.Type[] { typeof(Human) }
                );

                Patcher2.MethodPatch(
                    typeof(Human),
                    "FixedUpdate",
                    new System.Type[] { },
                    typeof(Patcher_Human),
                    nameof(Patcher_Human.FixedUpdate_Replace),
                    new System.Type[] { typeof(Human) }
                );

                Patcher2.MethodPatch(
                    typeof(Human),
                    "ProcessInput",
                    new System.Type[] { },
                    typeof(Patcher_Human),
                    nameof(Patcher_Human.ProcessInput_Replace),
                    new System.Type[] { typeof(Human) }
                );

                Debug.Log("[YxMod] Human patch (full replace) success.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[YxMod] Human patch failed: " + ex);
            }
        }

        public static void Initialize_Replace(Human __instance)
        {
            __instance.ragdoll = __instance.GetComponentInChildren<Ragdoll>();
            __instance.motionControl2.Initialize();

            ServoSound sound = __instance.GetComponentInChildren<ServoSound>();

            GameObject headObj = __instance.ragdoll.partHead.transform.gameObject;
            HumanHead head = headObj.AddComponent<HumanHead>();
            head.sounds = sound;
            head.humanAudio = __instance.GetComponentInChildren<HumanAudio>();
            sound.transform.SetParent(head.transform, false);

            _initializeBodies?.Invoke(__instance, null);
            _humanHead?.SetValue(__instance, head);

            var ext = HumanStateExtHelper.GetExt(__instance);
            ext.human = __instance;
            ext.YxModChuShiHua();
        }

        public static void FixedUpdate_Replace(Human __instance)
        {
            var ext = HumanStateExtHelper.GetExt(__instance);
            if (ext.human == null || ext.dingdian == null)
            {
                Debug.LogWarning("[YxMod] ext.human 或 dingdian 未初始化，正在执行懒加载初始化");
                ext.human = __instance;
                ext.YxModChuShiHua();
                return;
            }

            ext.YxModFixedUpdate();
        }

        public static void ProcessInput_Replace(Human __instance)
        {
            var ext = HumanStateExtHelper.GetExt(__instance);
            ext.human = __instance;
            ext.YxModProcessInput();
        }
    }
}
