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
            Debug.Log("[YxMod] ModEntry Awake - starting delayed attachment");
            StartCoroutine(AttachWhenReady());
        }

        private static bool _attached = false;

        private IEnumerator AttachWhenReady()
        {
            if (_attached) yield break;

            while (NetGame.instance == null)
                yield return null;

            if (_attached) yield break; // 防止双重加载
            _attached = true;

            Debug.Log("[YxMod] Attaching YxMod.");
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
