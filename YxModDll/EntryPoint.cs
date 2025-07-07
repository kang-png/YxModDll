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
            // 保留子线程延迟，防止太早注入
            new Thread(() =>
            {
                Thread.Sleep(3000); // 3秒延迟，给Unity留初始化时间

                // 使用主线程调度器安全执行Mod启动逻辑
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Debug.Log("[YxMod] Starting mod initialization on main thread.");
                    ModEntry.CreateAndAttach();
                });
            }).Start();
        }
    }

    // 主线程调度器，确保所有Unity API调用在主线程执行
    public class UnityMainThreadDispatcher : MonoBehaviour
    {
        private static UnityMainThreadDispatcher _instance;
        private static readonly Queue<Action> _executionQueue = new Queue<Action>();

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

                if (action == null)
                    break;

                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError("[YxMod] Exception in UnityMainThreadDispatcher: " + ex);
                }
            }
        }
    }

    public class ModEntry : MonoBehaviour
    {
        private static bool _initialized = false;

        // 安全的创建ModEntry GameObject并添加组件入口
        public static void CreateAndAttach()
        {
            if (_initialized)
            {
                Debug.Log("[YxMod] ModEntry already initialized.");
                return;
            }

            var obj = new GameObject("YxModEntry");
            UnityEngine.Object.DontDestroyOnLoad(obj);
            var modEntry = obj.AddComponent<ModEntry>();

            Debug.Log("[YxMod] ModEntry GameObject created and component added.");
            _initialized = true;
        }

        private void Start()
        {
            Debug.Log("[YxMod] ModEntry Start() called. Starting initialization coroutine.");
            StartCoroutine(WaitForUnityReadyAndAttach());
        }

        private IEnumerator WaitForUnityReadyAndAttach()
        {
            Debug.Log("[YxMod] Coroutine started: Waiting for Unity to be ready...");

            // 等待Unity初始化完成，检测条件可按需调整
            while (!IsUnityReady())
            {
                yield return null;
            }

            Debug.Log("[YxMod] Unity is ready, attaching YxMod component.");

            try
            {
                var mod = gameObject.AddComponent<YxModDll.Mod.YxMod>();
                Debug.Log("[YxMod] YxMod component attached: " + (mod != null ? "Success" : "Failed"));
            }
            catch (Exception ex)
            {
                Debug.LogError("[YxMod] Exception when attaching YxMod: " + ex);
            }
        }

        private bool IsUnityReady()
        {
            // 简单检测图形设备是否已初始化，可以根据实际需求扩展检测逻辑
            return SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null;
        }
    }
}
