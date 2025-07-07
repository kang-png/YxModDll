using Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Doorstop
{
    public static class Entrypoint
    {
        public static void Start()
        {
            // 你的原始线程创建逻辑，不动
            new Thread(() =>
            {
                Thread.Sleep(3000);
                // 跨线程创建GameObject和挂组件
                UnityEngine.Object.DontDestroyOnLoad(new GameObject("YxModLoader").AddComponent<ModEntry>());
            }).Start();
        }
    }

    // Unity主线程调度器，确保跨线程调用能跑到主线程执行
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> _executionQueue = new Queue<Action>();
        private static UnityMainThreadDispatcher _instance;

        public static UnityMainThreadDispatcher Instance()
        {
            if (_instance == null)
            {
                var obj = new GameObject("UnityMainThreadDispatcher");
                UnityEngine.Object.DontDestroyOnLoad(obj);
                _instance = obj.AddComponent<UnityMainThreadDispatcher>();
            }
            return _instance;
        }

        public void Enqueue(Action action)
        {
            if (action == null) return;
            lock (_executionQueue)
            {
                _executionQueue.Enqueue(action);
            }
        }

        private void Update()
        {
            while (true)
            {
                Action action = null;
                lock (_executionQueue)
                {
                    if (_executionQueue.Count > 0)
                        action = _executionQueue.Dequeue();
                }
                if (action == null) break;
                action.Invoke();
            }
        }
    }

    public class ModEntry : MonoBehaviour
    {
        private static bool _attached = false;

        private void Awake()
        {
            Debug.Log("[YxMod] ModEntry Awake - scheduling AttachWithTimeout coroutine on main thread");

            // 确保自己激活（一般没问题，但加个保险）
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            // 用主线程调度器跑协程，避免跨线程StartCoroutine无效
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                StartCoroutine(AttachWithTimeout());
            });
        }

        private IEnumerator AttachWithTimeout()
        {
            float timeout = 30f;
            float elapsed = 0f;

            Debug.Log("[YxMod] AttachWithTimeout coroutine started");

            while (elapsed < timeout)
            {
                if (_attached)
                {
                    Debug.Log("[YxMod] Already attached, exiting coroutine");
                    yield break;
                }

                if (NetGame.instance != null)
                {
                    Debug.Log("[YxMod] NetGame.instance found, attaching mod");
                    AttachMod();
                    yield break;
                }

                yield return null;
                elapsed += Time.deltaTime;
            }

            Debug.Log("[YxMod] Timeout reached, forcing mod attachment.");
            AttachMod();
        }

        private void AttachMod()
        {
            if (_attached)
            {
                Debug.Log("[YxMod] AttachMod called but already attached");
                return;
            }

            if (NetGame.instance != null)
            {
                var mod = NetGame.instance.gameObject.AddComponent<YxModDll.Mod.YxMod>();
                Debug.Log($"[YxMod] Attaching YxMod to NetGame: {(mod != null ? "Success" : "Failed")}");
            }
            else
            {
                var dummy = new GameObject("YxMod_Dummy");
                dummy.AddComponent<YxModDll.Mod.YxMod>();
                Debug.Log("[YxMod] NetGame not found, attaching YxMod to dummy GameObject.");
            }

            _attached = true;
        }
    }
}
