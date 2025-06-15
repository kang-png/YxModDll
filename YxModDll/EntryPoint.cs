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
            StartCoroutine(AttachWhenReady());
            Debug.Log("[YxMod] ModEntry Awake - starting delayed attachment");
        }

        private static bool _attached = false;

        private IEnumerator AttachWhenReady()
        {
            if (_attached) yield break;

            while (NetGame.instance == null)
            {
                Debug.Log("[YxMod] Waiting for NetGame.instance...");
                yield return null;
            }

            if (_attached) yield break; // 防止双重加载
            var mod = NetGame.instance.gameObject.AddComponent<YxModDll.Mod.YxMod>();
            _attached = true;

            Debug.Log($"[YxMod] Attaching YxMod: {(mod != null ? "Success" : "Failed")}");
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
