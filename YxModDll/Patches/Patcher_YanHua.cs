using HumanAPI;
using InControl;
using Multiplayer;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using YxModDll.Mod;
using static UnityEngine.EventSystems.StandaloneInputModule;

namespace YxModDll.Patches

{
    public class Patcher_YanHua : MonoBehaviour
    {
        //private float cooldown;
        private static FieldInfo _cooldown;

        //private Queue<FireworksProjectile> queue = new Queue<FireworksProjectile>();
        private static FieldInfo _queue;


        private void Awake()
        {
            _cooldown = typeof(FireworksWeapon).GetField("cooldown", BindingFlags.NonPublic | BindingFlags.Instance);
            _queue = typeof(Fireworks).GetField("queue", BindingFlags.NonPublic | BindingFlags.Instance);

            Patcher2.MethodPatch(typeof(FireworksWeapon), "Shoot", null, typeof(Patcher_YanHua), "Shoot", new[] { typeof(FireworksWeapon) });
            Patcher2.MethodPatch(typeof(Fireworks), "Start", null, typeof(Patcher_YanHua), "Fireworks_Start", new[] { typeof(Fireworks) });
        }

        public static void Shoot(FireworksWeapon instance)
        {
            var cooldown = (float)_cooldown.GetValue(instance);

            FireworksProjectile projectile = Fireworks.instance.GetProjectile();
            if (projectile != null)
            {
                projectile.Shoot(instance.muzzle);
                //cooldown = Time.time + 1f;
                cooldown = Time.time + 0.1f;
                _cooldown.SetValue(instance, cooldown);

            }
        }
        public static void Fireworks_Start(Fireworks instance)
        {
            var queue = (Queue<FireworksProjectile>)_queue.GetValue(instance);

            NetGame.instance.preUpdate += instance.OnPreFixedUpdate;
            queue.Enqueue(instance.prefab);
            instance.prefab.GetComponent<NetIdentity>().sceneId = 0u;
            //修改
            //预创建 20 个烟花弹
            //for (int i = 1; i < 20; i++)
            for (int i = 1; i < 100; i++)
            {
                FireworksProjectile fireworksProjectile = Object.Instantiate(instance.prefab, Vector3.down * 200f, Quaternion.identity, instance.transform);
                fireworksProjectile.GetComponent<NetIdentity>().sceneId = (uint)i;
                queue.Enqueue(fireworksProjectile);
            }
            instance.StartNetwork();
        }
    }
}

