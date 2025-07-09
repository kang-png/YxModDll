using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Doorstop
{
    public static class Entrypoint
    {
        private static bool _initialized = false;

        public static void Start()
        {
            // 注册监听场景切换事件
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private static void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            if (_initialized)
                return;

            try
            {
                var host = new GameObject("YxModHost");
                host.hideFlags = HideFlags.HideAndDontSave;
                UnityEngine.Object.DontDestroyOnLoad(host);
                host.AddComponent<YxModDll.Mod.YxMod>();

                Debug.Log("[YxMod] Mod injected successfully after scene change.");
                _initialized = true;

                // 注入完成后注销事件，避免重复执行
                SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            }
            catch (Exception ex)
            {
                Debug.LogError("[YxMod] Failed to inject mod: " + ex);
            }
        }
    }
}
