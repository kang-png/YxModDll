using HarmonyLib;
using Multiplayer;
using System;

namespace YxModDll.Mod
{
    // 高优先级前缀：在任何生成玩家逻辑前做黑名单拦截
    [HarmonyPatch(typeof(NetPlayer), nameof(NetPlayer.SpawnPlayer))]
    public static class BlacklistJoinPatch
    {
        [HarmonyPrefix]
        [HarmonyPriority(Priority.High)]
        public static bool Prefix(uint id, NetHost host, bool isLocal, string skinUserId, uint localCoopIndex, byte[] skinCRC, ref NetPlayer __result)
        {
            try
            {
                // 仅主机侧且非本地玩家时拦截
                UnityEngine.Debug.Log($"[黑名单] SpawnPlayer: isServer={NetGame.isServer}, isLocal={isLocal}, Enabled={HeiMingDan.Enabled}, host={(host!=null?host.name:"null")}, skinUserId={skinUserId}");
                if (!NetGame.isServer || isLocal) return true;
                if (!HeiMingDan.Enabled) return true;

                string playerName = host != null ? host.name : "玩家";
                if (HeiMingDan.ShouldBlockPlayerJoin(skinUserId, playerName))
                {
                    if (host != null)
                    {
                        NetGame.instance.Kick(host);
                    }
                    __result = null;
                    return false; // 阻止原方法与其他前缀继续执行
                }
            }
            catch (Exception)
            {
                // 忽略拦截中的异常，保证不影响正常入房流程
            }

            return true;
        }
    }
}