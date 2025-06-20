using Multiplayer;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Doorstop
{
    public class ModEntry : MonoBehaviour
    {
        private static bool _attached = false;

        private void Awake()
        {
            StartCoroutine(AttachWithTimeout());
            Debug.Log("[YxMod] ModEntry Awake - starting attach with timeout");
        }

        private IEnumerator AttachWithTimeout()
        {
            float timeout = 30f;
            float elapsed = 0f;

            while (elapsed < timeout)
            {
                if (_attached) yield break;

                if (NetGame.instance != null)
                {
                    AttachMod();
                    yield break;
                }

                yield return null;
                elapsed += Time.deltaTime;
            }

            // 超时后仍然尝试加载
            Debug.Log("[YxMod] Timeout reached, forcing mod attachment.");
            AttachMod();
        }

        private void AttachMod()
        {
            if (_attached) return;

            if (NetGame.instance != null)
            {
                var mod = NetGame.instance.gameObject.AddComponent<YxModDll.Mod.YxMod>();
                Debug.Log($"[YxMod] Attaching YxMod: {(mod != null ? "Success" : "Failed")}");
            }
            else
            {
                // NetGame 仍未出现，暂时直接在场景挂载一个空物体，避免挂载失败
                var dummy = new GameObject("YxMod_Dummy");
                dummy.AddComponent<YxModDll.Mod.YxMod>();
                Debug.Log("[YxMod] NetGame not found, attaching YxMod to dummy GameObject.");
            }

            _attached = true;
        }
    }

    public static class Entrypoint
    {
        public static void Start()
        {
            new Thread(() =>
            {
                Thread.Sleep(3000);
                UnityEngine.Object.DontDestroyOnLoad(new GameObject("YxModLoader").AddComponent<ModEntry>());
            }).Start();
        }
    }
}
