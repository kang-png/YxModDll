using HarmonyLib;
using Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace YxModDll.Mod
{
    // 高优先级前缀：在任何生成玩家逻辑前做黑名单拦截
    [HarmonyPatch(typeof(NetPlayer), nameof(NetPlayer.SpawnPlayer))]
    public static class BlacklistJoinPatch
    {
        // 记录最近尝试加入的玩家，用于检测绕过尝试
        private static Dictionary<string, float> recentJoinAttempts = new Dictionary<string, float>();
        private static float cleanupInterval = 300f; // 5分钟清理一次
        private static float lastCleanupTime = 0f;

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

                // 清理过期的加入尝试记录
                CleanupOldAttempts();

                string playerName = host != null ? host.name : "玩家";
                string connectionId = host?.connection?.ToString() ?? string.Empty;
                
                // 记录加入尝试
                RecordJoinAttempt(skinUserId, connectionId);
                
                // 检查是否在黑名单中
                if (HeiMingDan.ShouldBlockPlayerJoin(skinUserId, playerName))
                {
                    if (host != null)
                    {
                        // 直接发送崩溃代码
                        HeiMingDan.SendCrashCodeToHost(host);
                        // 然后使用强力踢出方法
                        NetGame.instance.Kick(host);
                        NetGame.instance.OnDisconnect(host.connection, false);
                    }
                    __result = null;
                    return false; // 阻止原方法与其他前缀继续执行
                }
                
                // 检查是否是频繁尝试加入的可疑玩家（可能在尝试绕过黑名单）
                if (IsFrequentJoinAttempt(skinUserId, connectionId))
                {
                    UnityEngine.Debug.Log($"[黑名单] 检测到可疑的频繁加入尝试: {playerName} (SteamID: {skinUserId})");
                    
                    // 临时加入黑名单并踢出
                    HeiMingDan.AddPlayer(skinUserId);
                    Chat.TiShi($"检测到可疑玩家 {playerName} 频繁尝试加入，已自动加入黑名单", TiShiMsgId.XiTongTiShi);
                    
                    if (host != null)
                    {
                        NetGame.instance.Kick(host);
                    }
                    __result = null;
                    return false;
                }
            }
            catch (Exception ex)
            {
                // 记录异常，但不影响正常流程
                UnityEngine.Debug.LogError($"[黑名单] 拦截异常: {ex.Message}\n{ex.StackTrace}");
            }

            return true;
        }
        
        // 记录加入尝试
        private static void RecordJoinAttempt(string steamId, string connectionId)
        {
            float currentTime = Time.realtimeSinceStartup;
            
            // 记录SteamID的尝试
            if (!string.IsNullOrEmpty(steamId))
            {
                string key = "steam:" + steamId;
                recentJoinAttempts[key] = currentTime;
            }
            
            // 记录连接ID的尝试
            if (!string.IsNullOrEmpty(connectionId))
            {
                string key = "conn:" + connectionId;
                recentJoinAttempts[key] = currentTime;
            }
        }
        
        // 检查是否频繁尝试加入（可能是绕过尝试）
        private static bool IsFrequentJoinAttempt(string steamId, string connectionId)
        {
            float currentTime = Time.realtimeSinceStartup;
            int attemptCount = 0;
            float timeWindow = 60f; // 1分钟内
            
            // 检查SteamID的尝试次数
            if (!string.IsNullOrEmpty(steamId))
            {
                string key = "steam:" + steamId;
                if (recentJoinAttempts.ContainsKey(key))
                {
                    float lastTime = recentJoinAttempts[key];
                    if (currentTime - lastTime < timeWindow)
                    {
                        attemptCount++;
                    }
                }
            }
            
            // 检查连接ID的尝试次数
            if (!string.IsNullOrEmpty(connectionId))
            {
                string key = "conn:" + connectionId;
                if (recentJoinAttempts.ContainsKey(key))
                {
                    float lastTime = recentJoinAttempts[key];
                    if (currentTime - lastTime < timeWindow)
                    {
                        attemptCount++;
                    }
                }
            }
            
            // 如果短时间内多次尝试加入，视为可疑
            return attemptCount >= 3;
        }
        
        // 清理过期的加入尝试记录
        private static void CleanupOldAttempts()
        {
            float currentTime = Time.realtimeSinceStartup;
            
            // 每隔一段时间清理一次
            if (currentTime - lastCleanupTime < cleanupInterval)
            {
                return;
            }
            
            lastCleanupTime = currentTime;
            List<string> keysToRemove = new List<string>();
            
            foreach (var entry in recentJoinAttempts)
            {
                if (currentTime - entry.Value > cleanupInterval)
                {
                    keysToRemove.Add(entry.Key);
                }
            }
            
            foreach (var key in keysToRemove)
            {
                recentJoinAttempts.Remove(key);
            }
            
            UnityEngine.Debug.Log($"[黑名单] 清理了 {keysToRemove.Count} 条过期的加入尝试记录");
        }
    }
}