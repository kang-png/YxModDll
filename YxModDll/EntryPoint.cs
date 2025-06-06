using Multiplayer;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Doorstop
{
    public class ModEntry : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("[YxMod] ModEntry Awake - starting delayed injection");
            StartCoroutine(InjectWhenReady());
        }

        private static bool _injected = false;

        private IEnumerator InjectWhenReady()
        {
            if (_injected) yield break;

            while (NetGame.instance == null || Human.all.Count == 0)
                yield return null;

            if (_injected) yield break; // 防止双重注入
            _injected = true;

            Debug.Log("[YxMod] Injecting YxMod.");
            NetGame.instance.gameObject.AddComponent<YxModDll.Mod.YxMod>();
        }

    }

    public static class Entrypoint
    {
        public static void Start()
        {
            // 延迟一点点时间，确保 UnityEngine 初始化
            new Thread(() =>
            {
                Thread.Sleep(3000); // 可根据需要调整，或换成其他信号机制
                UnityEngine.Object.DontDestroyOnLoad(new GameObject("YxModLoader").AddComponent<ModEntry>());
            }).Start();
        }
    }
}
