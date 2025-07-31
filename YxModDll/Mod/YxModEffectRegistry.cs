using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using YxModDll.Patches;

namespace YxModDll.Mod
{
    // ------------------------
    // YxModEffect.cs
    // ------------------------

    public class YxModEffect
    {
        public string key;
        public string label;
        public string desc;

        public Func<Human, bool> getter;
        public Action<Human, bool> setter;
        public Action<Human> applyEffect;
        public Action<Human> onEnable;

        public Func<bool> systemEnabled; // 指向系统开关的委托，例如 () => UI_GongNeng.yulexitong_KaiGuan
    }



    public static class YxModEffectRegistry
    {
        public static readonly List<YxModEffect> all = new();

        // 存放 Human 对应的 key-value 状态字典
        private static readonly ConditionalWeakTable<Human, Dictionary<string, object>> stateTable =
            new ConditionalWeakTable<Human, Dictionary<string, object>>();

        public static void Register(
            string key,
            Action<Human> applyEffect = null,
            string label = "",
            string desc = "",
            Action<Human> onEnable = null,
            Func<bool> systemEnabled = null
        )
        {
            all.Add(new YxModEffect
            {
                key = key,
                label = label,
                desc = desc,
                applyEffect = applyEffect,
                onEnable = onEnable,
                systemEnabled = systemEnabled,

                getter = h => {
                    var dict = stateTable.GetOrCreateValue(h);
                    return dict.TryGetValue(key, out var val) && val is bool b && b;
                },
                setter = (h, v) => {
                    var dict = stateTable.GetOrCreateValue(h);
                    bool before = dict.TryGetValue(key, out var oldVal) && oldVal is bool b && b;

                    dict[key] = v;
                    if (!before && v && onEnable != null)
                        onEnable(h);
                }
            });
        }

        public static YxModEffect Get(string key) => all.FirstOrDefault(e => e.key == key);

        public static bool GetState(Human h, string key) => Get(key)?.getter(h) ?? false;

        public static void SetState(Human h, string key, bool value) => Get(key)?.setter(h, value);

        public static void ApplyEffects(Human h)
        {
            foreach (var eff in all)
            {
                if (eff.getter(h))
                    eff.applyEffect?.Invoke(h);
            }
        }
    }


    // ------------------------
    // YxModPermission.cs
    // ------------------------

    public static class YxModPermission
    {
        public static bool AllowControl(Human netHostHuman, int targetIndex, YxModEffect eff, out string reason)
        {
            reason = null;

            if (!UI_GongNeng.kejiquanxian_KaiGuan)
            {
                reason = "客机权限系统已关闭";
                return false;
            }

            if (eff.systemEnabled != null && !eff.systemEnabled())
            {
                reason = $"{eff.label} 所在系统已关闭";
                return false;
            }

            Human target = Human.all[targetIndex];

            if (UI_SheZhi.fangzhububeikong && targetIndex == 0)
            {
                reason = "房主不让你控制他";
                return false;
            }

            if (netHostHuman.GetExt().jinzhibeikong && netHostHuman != target)
            {
                reason = "你禁止其他客机控制你，所以你也无法控制他人";
                return false;
            }

            if (target.GetExt().jinzhibeikong && netHostHuman != Human.all[0])
            {
                reason = $"玩家 {target.name} 禁止其他客机控制他";
                return false;
            }

            return true;
        }
    }


    // ------------------------
    // 示例注册：YxModEffectSetup.cs
    // ------------------------
    public static class YxModEffectSetup
    {
        public static void Init()
        {
            YxModEffectRegistry.Register("pangxie",
                applyEffect: human =>
                {
                    Vector3 a = Quaternion.Euler(0f, human.controls.cameraYawAngle - 90f, 0f) * Vector3.forward;
                    human.ragdoll.partLeftThigh.rigidbody.AddForce(a * 1000f, ForceMode.Force);
                    human.ragdoll.partRightThigh.rigidbody.AddForce(a * -1000f, ForceMode.Force);
                    human.ragdoll.partLeftLeg.rigidbody.AddForce(a * 2000f, ForceMode.Force);
                    human.ragdoll.partRightLeg.rigidbody.AddForce(a * -2000f, ForceMode.Force);
                    human.ragdoll.partLeftFoot.rigidbody.AddForce(a * 3000f, ForceMode.Force);
                    human.ragdoll.partRightFoot.rigidbody.AddForce(a * -3000f, ForceMode.Force);
                },
                label: "螃蟹",
                desc: "横着走",
                systemEnabled: () => UI_GongNeng.yulexitong_KaiGuan
            );

            YxModEffectRegistry.Register("shouhua",
                applyEffect: h =>
                {
                    h.ReleaseGrab(1f);
                    Vector3 up = Vector3.up * 200f;
                    h.ragdoll.partLeftHand.rigidbody.AddForce(up, ForceMode.Impulse);
                    h.ragdoll.partRightHand.rigidbody.AddForce(up, ForceMode.Impulse);
                },
                label: "手滑",
                desc: "松手+手抖",
                systemEnabled: () => UI_GongNeng.yulexitong_KaiGuan
            );

            YxModEffectRegistry.Register("wupeng",
                applyEffect: null,
                label: "无碰撞",
                desc: "穿墙自由",
                systemEnabled: () => UI_GongNeng.wupengzhuang_KaiGuan
            );
        }
    }
}
