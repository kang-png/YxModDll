using Multiplayer;
using System;
using UnityEngine;
using YxModDll.Mod;

namespace YxModDll.Patches
{
    public class Patcher_NetGame : MonoBehaviour
    {
        private void Awake()
        {
            try
            {
                //Patcher2.MethodPatch(
                //    typeof(NetGame),
                //    "OnServerReceive",
                //    new[] { typeof(NetHost), typeof(NetStream) },
                //    typeof(NetGameHandler),
                //    nameof(NetGameHandler.OnServerReceive),
                //    new[] { typeof(NetGame), typeof(NetHost), typeof(NetStream) }
                //);

                //Patcher2.MethodPatch(
                //    typeof(NetGame),
                //    "OnClientReceive",
                //    new[] { typeof(object), typeof(NetStream) },
                //    typeof(NetGameHandler),
                //    nameof(NetGameHandler.OnClientReceive),
                //    new[] { typeof(NetGame), typeof(object), typeof(NetStream) }
                //);

                Debug.Log("[YxMod] NetGame patched successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError("[YxMod] NetGame patch failed: " + ex);
            }
        }
    }
}
