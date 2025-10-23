using Multiplayer;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using YxModDll.Mod.Features;

namespace YxModDll.Mod
{
    internal class HeiMingDan
    {
        // 黑名单配置
        public static bool Enabled = false;
        public static List<string> SteamIDs = new List<string>();
        private static string configPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "YxMod_HeiMingDan.txt");

        // 初始化黑名单
        public static void Initialize()
        {
            LoadHeiMingDan();
        }

        // 加载黑名单
        public static void LoadHeiMingDan()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string[] lines = File.ReadAllLines(configPath);
                    SteamIDs.Clear();
                    
                    foreach (string line in lines)
                    {
                        string trimmedLine = line.Trim();
                        if (!string.IsNullOrEmpty(trimmedLine) && !trimmedLine.StartsWith("#"))
                        {
                            if (trimmedLine.StartsWith("Enabled="))
                            {
                                Enabled = bool.Parse(trimmedLine.Substring(8));
                            }
                            else if (IsValidSteamID(trimmedLine))
                            {
                                SteamIDs.Add(trimmedLine);
                            }
                        }
                    }
                    
                    Debug.Log($"[黑名单] 已加载 {SteamIDs.Count} 个黑名单条目，状态: {(Enabled ? "启用" : "禁用")}");
                }
                else
                {
                    // 创建默认配置文件
                    SaveHeiMingDan();
                    Debug.Log("[黑名单] 创建默认配置文件");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[黑名单] 加载配置文件失败: {e.Message}");
            }
        }

        // 保存黑名单
        public static void SaveHeiMingDan()
        {
            try
            {
                List<string> lines = new List<string>();
                lines.Add("# YxMod 黑名单配置文件");
                lines.Add("# 每行一个SteamID，格式为纯数字");
                lines.Add("# 启用/禁用黑名单功能");
                lines.Add($"Enabled={Enabled}");
                lines.Add("");
                lines.Add("# 黑名单列表:");
                
                foreach (string steamID in SteamIDs)
                {
                    lines.Add(steamID);
                }
                
                File.WriteAllLines(configPath, lines);
                Debug.Log($"[黑名单] 已保存 {SteamIDs.Count} 个黑名单条目");
            }
            catch (Exception e)
            {
                Debug.LogError($"[黑名单] 保存配置文件失败: {e.Message}");
            }
        }

        // 检查是否为有效的SteamID
        public static bool IsValidSteamID(string steamID)
        {
            if (string.IsNullOrEmpty(steamID)) return false;
            
            // SteamID应该是17位数字
            if (steamID.Length != 17) return false;
            
            return long.TryParse(steamID, out _);
        }

        // 检查玩家是否在黑名单中
        public static bool IsPlayerBanned(string steamID)
        {
            if (!Enabled) return false;
            if (!IsValidSteamID(steamID)) return false;
            
            return SteamIDs.Contains(steamID);
        }

        // 检查玩家是否在黑名单中（通过NetPlayer）
        public static bool IsPlayerBanned(NetPlayer player)
        {
            if (!Enabled || player == null) return false;
            
            string steamID = player.skinUserId;
            return IsPlayerBanned(steamID);
        }

        // 检查玩家是否在黑名单中（通过Human）
        public static bool IsPlayerBanned(Human human)
        {
            if (!Enabled || human == null || human.player == null) return false;
            
            string steamID = human.player.skinUserId;
            return IsPlayerBanned(steamID);
        }

        // 添加玩家到黑名单
        public static bool AddPlayer(string steamID)
        {
            if (!IsValidSteamID(steamID))
            {
                Debug.LogWarning($"[黑名单] 无效的SteamID: {steamID}");
                return false;
            }
            
            if (SteamIDs.Contains(steamID))
            {
                Debug.LogWarning($"[黑名单] SteamID {steamID} 已在黑名单中");
                return false;
            }
            
            SteamIDs.Add(steamID);
            SaveHeiMingDan();
            
            // 如果玩家当前在房间中，立即踢出
            if (NetGame.isServer)
            {
                foreach (Human human in Human.all)
                {
                    if (human.player != null && human.player.skinUserId == steamID)
                    {
                        KickBannedPlayer(human);
                        break;
                    }
                }
            }
            
            Debug.Log($"[黑名单] 已添加 SteamID: {steamID}");
            return true;
        }

        // 从黑名单中移除玩家
        public static bool RemovePlayer(string steamID)
        {
            if (SteamIDs.Remove(steamID))
            {
                SaveHeiMingDan();
                Debug.Log($"[黑名单] 已移除 SteamID: {steamID}");
                return true;
            }
            
            Debug.LogWarning($"[黑名单] SteamID {steamID} 不在黑名单中");
            return false;
        }

        // 踢出黑名单玩家
        public static void KickBannedPlayer(Human human)
        {
            if (!NetGame.isServer || human == null || human.player == null) return;
            
            string playerName = human.player.host.name;
            string steamID = human.player.skinUserId;
            
            // 发送踢出消息
            Chat.TiShi($"玩家 {playerName} (SteamID: {steamID}) 在黑名单中，已被踢出房间", TiShiMsgId.XiTongTiShi);
            
            // 踢出玩家
            try
            {
                NetHost host = human.player.host;
                if (host != null && NetGame.instance.readyclients.Contains(host))
                {
                    NetGame.instance.Kick(host);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[黑名单] 踢出玩家失败: {e.Message}");
            }
        }

        // 拦截玩家加入
        public static bool ShouldBlockPlayerJoin(string steamID, string playerName)
        {
            if (!Enabled) return false;
            
            if (IsPlayerBanned(steamID))
            {
                Debug.Log($"[黑名单] 拦截玩家加入: {playerName} (SteamID: {steamID})");
                
                // 发送拦截消息
                if (NetGame.isServer)
                {
                    Chat.TiShi($"已拦截黑名单玩家 {playerName} 加入房间", TiShiMsgId.XiTongTiShi);
                }
                
                return true;
            }
            
            return false;
        }

        // 打开Steam个人资料
        public static void OpenSteamProfile(string steamID)
        {
            if (!IsValidSteamID(steamID))
            {
                Debug.LogWarning($"[黑名单] 无法打开无效的SteamID: {steamID}");
                return;
            }
            
            try
            {
                ulong ulSteamID = ulong.Parse(steamID);
                SteamFriends.ActivateGameOverlayToUser("steamid", new CSteamID(ulSteamID));
                Debug.Log($"[黑名单] 已打开 SteamID {steamID} 的个人资料");
            }
            catch (Exception e)
            {
                Debug.LogError($"[黑名单] 打开Steam个人资料失败: {e.Message}");
            }
        }

        // 获取如何查看SteamID的说明
        public static string GetSteamIDHelpText()
        {
            return @"如何获取SteamID：
1. 在游戏中按Shift+Tab打开Steam界面
2. 点击""查看玩家""或""最近游戏""
3. 右键点击玩家头像，选择""复制网页URL""
4. URL中的数字就是SteamID（17位数字）

或者：
1. 在Steam客户端中查看好友列表
2. 右键点击好友，选择""查看个人资料""
3. 在浏览器地址栏中可以看到SteamID

示例SteamID格式：76561198000000000";
        }
    }
}