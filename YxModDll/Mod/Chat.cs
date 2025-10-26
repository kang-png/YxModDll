using HumanAPI;
using Multiplayer;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Interop;
using UnityEngine;
using YxModDll.Mod.Features;
using YxModDll.Patches;

namespace YxModDll.Mod
{
    public class Chat
    {
        private static readonly System.Random _random = new System.Random();//全局随机种子，只创建一个

        public static bool JinRuLiKai_XiaoXi = true;
        public static bool JinRuLiKai_XiaoXi_CuTi;
        public static bool JinRuLiKai_XiaoXi_XieTi;
        public static string JinRuLiKai_XiaoXi_DaXiao = "16";
        public static string JinRuLiKai_XiaoXi_YanSe = "#FF9600";

        public static bool GongNengBianGeng_XiaoXi = true;
        public static bool GongNengBianGeng_XiaoXi_CuTi;
        public static bool GongNengBianGeng_XiaoXi_XieTi;
        public static string GongNengBianGeng_XiaoXi_DaXiao = "16";
        public static string GongNengBianGeng_XiaoXi_YanSe = "#FFFF8F";

        public static bool GeRenTiShi_XiaoXi=true;
        public static bool GeRenTiShi_XiaoXi_CuTi;
        public static bool GeRenTiShi_XiaoXi_XieTi;
        public static string GeRenTiShi_XiaoXi_DaXiao = "16";
        public static string GeRenTiShi_XiaoXi_YanSe = "#FFFFFF";

        public static bool XiTongTiShi_XiaoXi = true;
        public static bool XiTongTiShi_XiaoXi_CuTi;
        public static bool XiTongTiShi_XiaoXi_XieTi;
        public static string XiTongTiShi_XiaoXi_DaXiao = "16";
        public static string XiTongTiShi_XiaoXi_YanSe = "#00FF00";

        //public static bool GeRenLiaoTian_MingZi = false;
        //public static bool GeRenLiaoTian_MingZi_CuTi = true;
        //public static bool GeRenLiaoTian_MingZi_XieTi;
        //public static string GeRenLiaoTian_MingZi_Str;
        //public static int GeRenLiaoTian_MingZi_YanSe_FangShi = 1;
        //public static string GeRenLiaoTian_MingZi_DaXiao = "16";
        //public static string GeRenLiaoTian_MingZi_DanSe = "#FF00FF";
        //public static string GeRenLiaoTian_MingZi_JianBianSe1 = "#FF00FF";
        //public static string GeRenLiaoTian_MingZi_JianBianSe2 = "#9F2EFF";
        //public static string GeRenLiaoTian_MingZi_LiangDu = "50";

        //public static bool GeRenLiaoTian_XiaoXi = false;
        //public static bool GeRenLiaoTian_XiaoXi_CuTi;
        //public static bool GeRenLiaoTian_XiaoXi_XieTi;
        //public static int GeRenLiaoTian_XiaoXi_YanSe_FangShi = 2;
        //public static string GeRenLiaoTian_XiaoXi_DaXiao = "16";
        //public static string GeRenLiaoTian_XiaoXi_DanSe = "#FF00FF";
        //public static string GeRenLiaoTian_XiaoXi_JianBianSe1 = "#FF00FF";
        //public static string GeRenLiaoTian_XiaoXi_JianBianSe2 = "#9F2EFF";
        //public static string GeRenLiaoTian_XiaoXi_LiangDu = "50";
        public static void SendYxModMsgClient(string YxModMsg)//客机向服务器发送功能
        {
            string humanID = "";
            SendYxModMsgClient(YxModMsg, humanID);
        }
        public static void SendYxModMsgClient(string YxModMsg,string humanID)//客机向服务器发送功能
        {
            string name = YxModMsg;
            string msg = humanID;
            if (!NetGame.isClient)
            {
                return;
            }

            NetStream netStream = NetGame.BeginMessage(NetMsgId.Chat);
            try
            {
                netStream.WriteNetId(NetGame.instance.local.hostId);
                netStream.Write(name);
                netStream.Write(msg);
                if (NetGame.instance.server != null && NetGame.instance.server.isReady)
                {
                    NetGame.instance.SendReliableToServer(netStream);
                }
            }
            finally
            {
                if (netStream != null)
                {
                    netStream = netStream.Release();
                }
            }
        }
        public static void SendYxModMsgServer(NetHost netHost, string YxModMsg)//服务端 向 netHost 发送消息
        {
            string msg = "";
            SendYxModMsgServer(netHost, YxModMsg, msg);
        }
        public static void SendYxModMsgServer(NetHost netHost, string YxModMsg, string msg)//服务端 向 netHost 发送消息
        {
            if (netHost == null)
            {
                //Debug.Log("SendChatMessageToClient：netHost为空");
                return;
            }
            NetStream netStream = NetGame.BeginMessage(NetMsgId.Chat);
            try
            {
                //string msg = "";
                netStream.WriteNetId(netHost.hostId);
                netStream.Write(YxModMsg);
                netStream.Write(msg);
                if (NetGame.isServer)
                {
                    NetGame.instance.SendReliable(netHost, netStream);
                }
            }
            finally
            {
                if (netStream != null)
                {
                    netStream = netStream.Release();
                }
            }
        }
        public static void OnReceiveChatClient(NetStream stream)//客户端接收消息   修改
        {

            uint clientId = stream.ReadNetId();
            string nick = stream.ReadString();
            string msg = stream.ReadString();

            if (nick == "[YxMod]" && msg.Length == 0)//YxModHello
            {
                //Debug.Log("客户端收到Hello1");
                YxMod.YxModServer = true;
                YxModHelloClient();//say holle
            }
            else if (nick == YxModMsgStr("kejiquanxian") && msg.Length != 0)
            {
                int result;
                if (int.TryParse(msg, out result))
                {
                    YxMod.KeJiQuanXian = result == 1;
                }
            }
            else if (nick == YxModMsgStr("scale") && msg.Length != 0)
            {
                string[] split = msg.Split(';');
                if (split.Length == 2)
                {
                    int targetId = int.Parse(split[0]);
                    string[] parts = split[1].Split('|');
                    if (parts.Length == 7)
                    {
                        float head = float.Parse(parts[0]);
                        float torso = float.Parse(parts[1]);
                        float leftArm = float.Parse(parts[2]);
                        float rightArm = float.Parse(parts[3]);
                        float leftLeg = float.Parse(parts[4]);
                        float rightLeg = float.Parse(parts[5]);
                        float ball = float.Parse(parts[6]);

                        // 找目标人
                        NetHost targetHost = NetGame.instance.FindReadyHost((uint)targetId);
                        if (targetHost != null && targetHost.players.Count > 0)
                        {
                            Human target = targetHost.players[targetHost.players.Count - 1].human;


                            if (target.player.isLocalPlayer) 
                            {
                                // 更新扩展数据
                                var ext = target.GetExt();
                                ext.scaleHead = head;
                                ext.scaleTorso = torso;
                                ext.scaleLeftArm = leftArm;
                                ext.scaleRightArm = rightArm;
                                ext.scaleLeftLeg = leftLeg;
                                ext.scaleRightLeg = rightLeg;
                                ext.scaleBall = ball;
                            }
                            // 应用变形
                            UI_WanJia.SetHumanScaleByPart(target,
                                head: head, torso: torso,
                                leftArm: leftArm, rightArm: rightArm,
                                leftLeg: leftLeg, rightLeg: rightLeg,
                                ball: ball);
                        }
                    }
                }
            }
            else if (nick == YxModMsgStr("bufasudu") && msg.Length != 0)
            {
                float result;
                if (float.TryParse(msg, out result))
                {
                    UI_WanJia.bufasudu = result;
                }
            }

            //发送一些客机记录的至给服务器
            /////////////////////////
            else//正常的消息 正常接收
            {
                if (msg == "q" || msg == "up" || msg == "ifg")
                {
                    return;
                }
                if (UI_SheZhi.pingbiyansedaima)
                {
                    nick = QuDiaoDaiMa(nick, "");
                }
                NetChat.OnReceive(clientId, nick, msg);

                string msg1 = QuDiaoDaiMa($"{nick} : {msg}", "");
                string time = DateTime.Now.ToString("HH:mm:ss");
                Debug.Log($"[{time}] [消息]{clientId}.{msg1}");
            }


        }

        private static readonly HashSet<string> DevSteamIds =
            new HashSet<string>
            {
                "76561198243558272", // feiyuyu
                "76561198416472650", // 愤怒的小小鸟
                "76561198966195020", // 飒
                "76561198217183185", // 噔噔Dunn                
            };
        public static bool IsDeveloper(string steamId)
        {
            return DevSteamIds.Contains(steamId);
        }


        public static void OnReceiveChatServer(NetHost client, NetStream stream)//服务端处理消息
        {
            //if (Options.parental == 0)
            //{
            uint clientId = stream.ReadNetId();
            string nick = stream.ReadString();
            string msg = stream.ReadString();
            NetHost netHost = NetGame.instance.FindReadyHost(clientId);
            Human human = netHost.players[netHost.players.Count - 1].human;
            bool isDev = IsDeveloper(netHost.players[netHost.players.Count - 1].skinUserId);

            if (nick == "[YxMod]" && msg.Length == 0)//客机有Mod
            {
                //Debug.Log("服务端收到Hello2");
                human.GetExt().isClient = true;
                string value = (isDev || human.GetExt().kejiquanxian) ? "1" : "0";
                SendYxModMsgServer(client, YxModMsgStr("kejiquanxian"), value);
                return;
            }
            else if (nick == YxModMsgStr("kick") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!Human.all[result].GetExt().isClient)
                    {
                        TiShi($"玩家 {Human.all[result].player.host.name} 被 {netHost.name} 踢出去了");
                        UI_WanJia.TiChuFangJian(Human.all[result]);
                    }
                    else
                    {
                        TiShi(netHost, $"踢出失败,玩家 {Human.all[result].player.host.name} 有YxMod文件");
                    }
                }
                return;
            }
            else if (nick == YxModMsgStr("cxks") && msg.Length == 0)//重新开始
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                Game.instance.RestartLevel();
                Game.instance.Resume();
                Chat.TiShi($"玩家 {netHost.name} 重新开始了游戏");
                return;
            }
            else if (nick == YxModMsgStr("syg") && msg.Length == 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                YxMod.ShangYiGuan();
                Chat.TiShi($"玩家 {netHost.name} 使游戏进入到第 {Game.instance.currentCheckpointNumber + 1} 关");
                return;
            }
            else if (nick == YxModMsgStr("xyg") && msg.Length == 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                YxMod.XiaYiGuan();
                Chat.TiShi($"玩家 {netHost.name} 使游戏进入到第 {Game.instance.currentCheckpointNumber + 1} 关");
                return;
            }
            else if (nick == YxModMsgStr("czwp") && msg.Length == 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                YxMod.ChongZhiWuPin();
                Chat.TiShi($"玩家 {netHost.name} 重置了所有物品");
                return;
            }
            else if (nick == YxModMsgStr("czdh") && msg.Length == 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                UI_CaiDan.ResetAllAnimations();
                Chat.TiShi($"玩家 {netHost.name} 重置了所有动画");
                return;
            }
            else if (nick == YxModMsgStr("jihe") && msg.Length == 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                YxMod.JiHe(human);
                Chat.TiShi($"玩家 {netHost.name} 召集了所有玩家");
                return;
            }
            else if (nick == YxModMsgStr("csz") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.chuansongxitong_KaiGuan)
                {
                    TiShi(netHost, $"传送系统已关闭");
                    return;
                }
                //Debug.Log(msg);
                string[] canshu = msg.Split('|');

                int result1; int result2;
                if (int.TryParse(canshu[0], out result1) && int.TryParse(canshu[1], out result2))
                {

                    if (!isDev && UI_SheZhi.fangzhububeikong && result1 == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result1])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所以你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result1].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result1].name} 禁止其他客机控制他");
                        return;
                    }

                    YxMod.ChuanSong(Human.all[result1], Human.all[result2]);
                }


                //int result;
                //if (int.TryParse(msg, out result))
                //{
                //    YxMod.ChuanSong(human, Human.all[result]);
                //}
                return;
            }
            else if (nick == YxModMsgStr("xfy") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.guajianxitong_KaiGuan)
                {
                    TiShi(netHost, $"挂件系统已关闭");
                    return;
                }
                string[] canshu = msg.Split('|');

                int result1; int result2;
                if (int.TryParse(canshu[0], out result1) && int.TryParse(canshu[1], out result2))
                {

                    if (!isDev && UI_SheZhi.fangzhububeikong && result1 == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result1])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result1].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result1].name} 禁止其他客机控制他");
                        return;
                    }

                    if (result1 == result2)
                    {
                        Chat.TiShi(netHost, "不能当自己的头部挂件");
                        return;
                    }

                    YxMod.SetGuaJian(Human.all[result2], Human.all[result1]);
                }
                return;
            }
            else if (nick == YxModMsgStr("qxxf") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.guajianxitong_KaiGuan)
                {
                    TiShi(netHost, $"挂件系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }


                    YxMod.QuXiaoGuaJian(Human.all[result]);
                }
                return;
            }
            else if (nick == YxModMsgStr("qs") && msg.Length != 0)//牵手
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.qianshouxitong_KaiGuan)
                {
                    TiShi(netHost, $"牵手系统已关闭");
                    return;
                }
                string[] canshu = msg.Split('|');

                int result1; int result2;
                if (int.TryParse(canshu[0], out result1) && int.TryParse(canshu[1], out result2))
                {

                    if (!isDev && UI_SheZhi.fangzhububeikong && result1 == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result1])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result1].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result1].name} 禁止其他客机控制他");
                        return;
                    }

                    //if (result1 == result2)
                    //{
                    //    Chat.TiShi(netHost, "不能当自己的头部挂件");
                    //    return;
                    //}

                    YxMod.SetQianShou(Human.all[result1], Human.all[result2]);
                }
                return;
            }
            else if (nick == YxModMsgStr("beiren") && msg.Length != 0)//牵手
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.guajianxitong_KaiGuan)
                {
                    TiShi(netHost, $"挂件系统已关闭");
                    return;
                }
                string[] canshu = msg.Split('|');

                int result1; int result2;
                if (int.TryParse(canshu[0], out result1) && int.TryParse(canshu[1], out result2))
                {

                    if (!isDev && UI_SheZhi.fangzhububeikong && result1 == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result1])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result1].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result1].name} 禁止其他客机控制他");
                        return;
                    }

                    //if (result1 == result2)
                    //{
                    //    Chat.TiShi(netHost, "不能当自己的头部挂件");
                    //    return;
                    //}

                    c_BeiRen.BeiRen(Human.all[result1], Human.all[result2]);
                }
                return;
            }
            else if (nick == YxModMsgStr("qxqs") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.qianshouxitong_KaiGuan)
                {
                    TiShi(netHost, $"牵手系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }

                    YxMod.QuXiaoQianShou(Human.all[result]);
                }
                return;
            }
            else if (nick == YxModMsgStr("qxbeiren") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.guajianxitong_KaiGuan)
                {
                    TiShi(netHost, $"挂件系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }

                    c_BeiRen.QuXiaoBeiRen(Human.all[result]);
                }
                return;
            }
            else if (nick == YxModMsgStr("huabing") && msg.Length == 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                UI_CaiDan.HuaBing = !UI_CaiDan.HuaBing;
                UI_CaiDan.HuaBingTu_Fun(UI_CaiDan.HuaBing);
                if (UI_CaiDan.HuaBing)
                {
                    Chat.TiShi($"玩家 {netHost.name} 把地图变成了滑冰蹦迪地图");
                }
                else
                {
                    Chat.TiShi($"玩家 {netHost.name} 把地图恢复了原状");
                }
                return;
            }
            else if (nick == YxModMsgStr("gerendingdian") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && !UI_GongNeng.dingdian_KaiGuan)
                    {
                        Chat.TiShi(netHost, $"定点系统已关闭");
                        return;
                    }

                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }

                    Human.all[result].GetExt().dingdian.kaiguan = !Human.all[result].GetExt().dingdian.kaiguan;
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().dingdian.kaiguan ? "打开" : "关闭")}了 {Human.all[result].player.host.name} 的个人定点");
                }
                return;
            }
            else if (nick == YxModMsgStr("wujiasi") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.wujiasi_KaiGuan)
                {
                    TiShi(netHost, $"无假死系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().wujiasi = !Human.all[result].GetExt().wujiasi;
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().wujiasi ? "打开" : "关闭")}了 {Human.all[result].player.host.name} 的无假死");
                }
                return;
            }
            else if (nick == YxModMsgStr("wupengzhuang") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.wupengzhuang_KaiGuan)
                {
                    TiShi(netHost, $"无碰撞系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().wupengzhuang = !Human.all[result].GetExt().wupengzhuang;
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().wupengzhuang ? "打开" : "关闭")}了 {Human.all[result].player.host.name} 的无碰撞");
                }
                return;
            }
            else if (nick == YxModMsgStr("feitian") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.feitianxitong_KaiGuan)
                {
                    TiShi(netHost, $"飞天系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().feitian = !Human.all[result].GetExt().feitian;
                    YxMod.SetFeiTian(Human.all[result]);
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().feitian ? "赋予" : "取消了")} {Human.all[result].player.host.name} 飞天能力");
                    if (!isDev && Human.all[result].GetExt().feitian)
                    {
                        Chat.TiShi(Human.all[result].player.host, "普通情况下是正常飞天。按住左键，W，空格，保持两秒，可进入超人状态。");
                    }
                }
                return;
            }

            else if (nick == YxModMsgStr("chaoren") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.feitianxitong_KaiGuan)
                {
                    TiShi(netHost, $"飞天系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().chaoren = !Human.all[result].GetExt().chaoren;
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().chaoren ? "赋予" : "取消了")} {Human.all[result].player.host.name} 超人能力");
                }
                return;
            }
            else if (nick == YxModMsgStr("shanxian") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.shanxianxitong_KaiGuan)
                {
                    TiShi(netHost, $"闪现系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().shanxian = !Human.all[result].GetExt().shanxian;
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().shanxian ? "赋予" : "取消了")} {Human.all[result].player.host.name} 闪现能力");
                }
                return;
            }
            else if (nick == YxModMsgStr("dongjie") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().dongjie = !Human.all[result].GetExt().dongjie;
                    YxMod.DongJie(Human.all[result]);
                    Chat.TiShi($"玩家 {netHost.name} 把 {Human.all[result].player.host.name} {(Human.all[result].GetExt().dongjie ? "冻结了" : "解冻了")}");
                }
                return;
            }
            else if (nick == YxModMsgStr("banshen") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().banshen = !Human.all[result].GetExt().banshen;
                    YxMod.BanShen(Human.all[result]);
                    //Chat.TiShi($"玩家 {netHost.name} 把 {Human.all[result].player.host.name} {(Human.all[result].GetExt().banshen ? "冻结了" : "解冻了")}");
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().banshen ? "赋予了" : "取消了")} {Human.all[result].player.host.name} 半身不遂");
                }
                return;
            }
            else if (nick == YxModMsgStr("bengdi") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().bengdi = !Human.all[result].GetExt().bengdi;
                    YxMod.BengDi(Human.all[result]);
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().bengdi ? "赋予" : "取消了")} {Human.all[result].player.host.name} 蹦迪能力");
                }
                return;
            }
            else if (nick == YxModMsgStr("chaojitiao") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().chaojitiao = !Human.all[result].GetExt().chaojitiao;
                    YxMod.chaojitiao(Human.all[result]);
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().chaojitiao ? "赋予" : "取消了")} {Human.all[result].player.host.name} 超级跳能力");
                }
                return;
            }

            else if (nick == YxModMsgStr("sanjitiao") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().sanjitiao = !Human.all[result].GetExt().sanjitiao;
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().sanjitiao ? "赋予" : "取消了")} {Human.all[result].player.host.name} 三级跳能力");
                }
                return;
            }
            else if (nick == YxModMsgStr("diantun") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().diantun = !Human.all[result].GetExt().diantun;
                    YxMod.DianTun(Human.all[result]);
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().diantun ? "赋予" : "取消了")} {Human.all[result].player.host.name} 电臀能力");
                }
                return;
            }
            else if (nick == YxModMsgStr("qiqiu") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().qiqiu = !Human.all[result].GetExt().qiqiu;
                    YxMod.QiQiu(Human.all[result]);
                    Chat.TiShi($"玩家 {netHost.name} 把 {Human.all[result].player.host.name} {(Human.all[result].GetExt().qiqiu ? "变成了气球" : "恢复正常")}");
                }
                return;
            }
            else if (nick == YxModMsgStr("qiqiuxifa") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().qiqiuxifa = !Human.all[result].GetExt().qiqiuxifa;
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().qiqiuxifa ? "赋予" : "取消了")} {Human.all[result].player.host.name} 左键抓住物品可以起飞的气球戏法能力");
                }
                return;
            }
            else if (nick == YxModMsgStr("daoli") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().daoli = !Human.all[result].GetExt().daoli;
                    YxMod.DaoLi(Human.all[result]);
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().daoli ? "赋予" : "取消了")} {Human.all[result].player.host.name} 双手抓地使用倒立能力");
                }
                return;
            }
            else if (nick == YxModMsgStr("zhuanquan") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().zhuanquan = !Human.all[result].GetExt().zhuanquan;
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().zhuanquan ? "赋予" : "取消了")} {Human.all[result].player.host.name} 按住空格可以转圈圈能力");
                }
                return;
            }
            else if (nick == YxModMsgStr("tuoluo") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().tuoluo = !Human.all[result].GetExt().tuoluo;
                    Chat.TiShi($"玩家 {netHost.name} 把 {Human.all[result].player.host.name} {(Human.all[result].GetExt().tuoluo ? "变成了小陀螺" : "恢复正常")}");
                }
                return;
            }
            else if (nick == YxModMsgStr("ketouguai") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().ketouguai = !Human.all[result].GetExt().ketouguai;
                    Chat.TiShi($"玩家 {netHost.name} 把 {Human.all[result].player.host.name} {(Human.all[result].GetExt().ketouguai ? "变成了磕头怪" : "恢复正常")}");
                }
                return;
            }
            else if (nick == YxModMsgStr("diaosigui") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().diaosigui = !Human.all[result].GetExt().diaosigui;
                    YxMod.DiaoSiGui(Human.all[result]);
                    Chat.TiShi($"玩家 {netHost.name} 把 {Human.all[result].player.host.name} {(Human.all[result].GetExt().diaosigui ? "变成了吊死鬼" : "恢复正常")}");
                }
                return;
            }
            else if (nick == YxModMsgStr("pangxie") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().pangxie = !Human.all[result].GetExt().pangxie;
                    Chat.TiShi($"玩家 {netHost.name} 把 {Human.all[result].player.host.name} {(Human.all[result].GetExt().pangxie ? "变成了大闸蟹" : "恢复正常")}");
                }
                return;
            }
            else if (nick == YxModMsgStr("shouhua") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }

                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所以你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }

                    // 切换目标玩家的“手滑”状态
                    Human.all[result].GetExt().shouhua = !Human.all[result].GetExt().shouhua;

                    // 聊天提示
                    Chat.TiShi($"玩家 {netHost.name} 让 {Human.all[result].player.host.name} {(Human.all[result].GetExt().shouhua ? "手滑啦！" : "重新稳住了")}");
                }
                return;
            }
            else if (nick == YxModMsgStr("qianshui") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().qianshui = !Human.all[result].GetExt().qianshui;
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().qianshui ? "赋予" : "取消了")} {Human.all[result].player.host.name} 潜水能力");
                }
                return;
            }
            else if (nick == YxModMsgStr("tuique") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().tuique = !Human.all[result].GetExt().tuique;
                    YxMod.TuiQue(Human.all[result]);
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().tuique ? "打瘸了" : "治好了")} {Human.all[result].player.host.name} 一条腿");
                }
                return;
            }
            else if (nick == YxModMsgStr("tuiguai") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().tuiguai = !Human.all[result].GetExt().tuiguai;
                    YxMod.TuiGuai(Human.all[result]);
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().tuiguai ? "送给" : "收回了")} {Human.all[result].player.host.name} 一支拐杖");
                }
                return;
            }

            else if (nick == YxModMsgStr("chaichu") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().chaichu = !Human.all[result].GetExt().chaichu;
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().chaichu ? "赋予" : "取消了")} {Human.all[result].player.host.name} 拆除能力");
                    if (!isDev && Human.all[result].GetExt().chaichu)
                    {
                        Chat.TiShi(Human.all[result].player.host, "开启拆除,左手抓住目标，即可拆卸。");
                    }
                }
                return;
            }
            else if (nick == YxModMsgStr("bufasudu") && msg.Length != 0)
            {
                float result;
                if (float.TryParse(msg, out result))
                {
                    human.GetExt().bufasudu = result;
                }
                return;
            }
            else if (nick == YxModMsgStr("kongqipao") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                {
                    TiShi(netHost, $"娱乐系统已关闭");
                    return;
                }
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    Human.all[result].GetExt().kongqipao = !Human.all[result].GetExt().kongqipao;
                    Chat.TiShi($"玩家 {netHost.name} {(Human.all[result].GetExt().kongqipao ? "赋予" : "取消了")} {Human.all[result].player.host.name} 空气炮能力");
                    if (!isDev && Human.all[result].GetExt().kongqipao)
                    {
                        Chat.TiShi(Human.all[result].player.host, "长按 鼠标左键 向前方打出空气炮，被击中的物体会被击飞。");
                    }
                }
                return;
            }
            else if (nick == YxModMsgStr("wutiguajian") && msg.Length == 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                if (!isDev && !UI_GongNeng.wutiguajian_KaiGuan)
                {
                    TiShi(netHost, $"物体挂件系统已关闭");
                    return;
                }
                WuTiGuaJian.SetWuTiGuaJian(human);
                return;
            }
            else if (nick == YxModMsgStr("quxiaowutiguajian") && msg.Length != 0)
            {
                if (!isDev && !UI_GongNeng.kejiquanxian_KaiGuan)
                {
                    TiShi(netHost, $"客机权限系统已关闭");
                    return;
                }
                //if (!isDev && !UI_GongNeng.yulexitong_KaiGuan)
                //{
                //    TiShi(netHost, $"娱乐系统已关闭");
                //    return;
                //}
                int result;
                if (int.TryParse(msg, out result))
                {
                    if (!isDev && UI_SheZhi.fangzhububeikong && result == 0)
                    {
                        Chat.TiShi(netHost, $"房主不让你控制他");
                        return;
                    }
                    if (human.GetExt().jinzhibeikong && human != Human.all[result])
                    {
                        Chat.TiShi(netHost, $"你禁止其他客机控制你,所有你也无法控制他人");
                        return;
                    }
                    if (!isDev && Human.all[result].GetExt().jinzhibeikong && human != Human.all[0])
                    {
                        Chat.TiShi(netHost, $"玩家 {Human.all[result].name} 禁止其他客机控制他");
                        return;
                    }
                    WuTiGuaJian.QuXiaoWuTiGuaJian(Human.all[result]);
                }
                return;
            }
            else if (nick == YxModMsgStr("huisudingdian") && msg.Length != 0)
            {
                int result;
                if (int.TryParse(msg, out result))
                {
                    human.GetExt().dingdian.huisu = result == 1;
                }
                return;
            }
            else if (nick == YxModMsgStr("guanxingdingdian") && msg.Length != 0)
            {
                int result;
                if (int.TryParse(msg, out result))
                {
                    human.GetExt().dingdian.guanxing = result == 1;
                }
                return;
            }
            else if (nick == YxModMsgStr("baosudingdian") && msg.Length != 0)
            {
                int result;
                if (int.TryParse(msg, out result))
                {
                    human.GetExt().dingdian.baoLiuDangQianSuDu = result == 1;
                }
                return;
            }
            else if (nick == YxModMsgStr("qdingdian") && msg.Length != 0)
            {
                int result;
                if (int.TryParse(msg, out result))
                {
                    human.GetExt().dingdian.q = result == 1;
                }
                return;
            }
            else if (nick == YxModMsgStr("sedingdian") && msg.Length != 0)
            {
                int result;
                if (int.TryParse(msg, out result))
                {
                    human.GetExt().dingdian.se = result == 1;
                }
                return;
            }
            else if (nick == YxModMsgStr("dingdiangaodu") && msg.Length != 0)
            {
                float result;
                if (float.TryParse(msg, out result))
                {
                    human.GetExt().dingdian.gaodu = result;
                }
                return;
            }

            else if (nick == YxModMsgStr("dingdiangeshu") && msg.Length != 0)
            {
                int result;
                if (int.TryParse(msg, out result))
                {
                    human.GetExt().dingdian.geshu = result;
                }
                return;
            }

            else if (nick == YxModMsgStr("dingdiantishi"))//&& msg.Length != 0    //有时候会有空值
            {
                human.GetExt().dingdian.tishiStr = msg;
                return;
            }

            else if (nick == YxModMsgStr("q") && msg.Length == 0)
            {

                if (!isDev && !UI_GongNeng.dingdian_KaiGuan)
                {
                    Chat.TiShi(netHost, $"定点系统已关闭");
                    return;
                }
                if (!human.GetExt().dingdian.kaiguan)
                {
                    Chat.TiShi(netHost, $"你的个人定点已关闭");
                    return;
                }

                //if (human.dingdian.GetExt().DingDianFangShi == 0 || human.dingdian.GetExt().DingDianFangShi == 2)//q定点
                if (human.GetExt().dingdian.q)
                {
                    human.GetExt().dingdian.CunDian(human, true);
                }

                return;
            }

            else if (nick == YxModMsgStr("jinzhibeikong") && msg.Length != 0)
            {
                int result;
                if (int.TryParse(msg, out result))
                {
                    human.GetExt().jinzhibeikong = result == 1;
                }
                return;
            }
            else if (nick == YxModMsgStr("up") && msg.Length == 0)
            {
                YxMod.Up(human);
                return;
            }
            else if (nick == YxModMsgStr("ifg") && msg.Length == 0)
            {
                YxMod.Ifg(human);
                return;
            }
            else if (nick == YxModMsgStr("wjlb") && msg.Length == 0)
            {

                Chat.TiShi(netHost, "玩家列表");
                return;
            }
            else if (nick == YxModMsgStr("scale") && msg.Length != 0)
            {
                string[] split = msg.Split(';');
                if (split.Length != 2) return;

                if (!int.TryParse(split[0], out int targetId)) return;

                string[] parts = split[1].Split('|');
                if (parts.Length != 7) return;

                if (!float.TryParse(parts[0], out float head)) return;
                if (!float.TryParse(parts[1], out float torso)) return;
                if (!float.TryParse(parts[2], out float leftArm)) return;
                if (!float.TryParse(parts[3], out float rightArm)) return;
                if (!float.TryParse(parts[4], out float leftLeg)) return;
                if (!float.TryParse(parts[5], out float rightLeg)) return;
                if (!float.TryParse(parts[6], out float ball)) return;

                NetHost targetHost = NetGame.instance.FindReadyHost((uint)targetId);
                Human target = targetHost?.players.LastOrDefault()?.human;
                if (target == null) return;

                var ext = target.GetExt();
                ext.scaleHead = head;
                ext.scaleTorso = torso;
                ext.scaleLeftArm = leftArm;
                ext.scaleRightArm = rightArm;
                ext.scaleLeftLeg = leftLeg;
                ext.scaleRightLeg = rightLeg;
                ext.scaleBall = ball;

                UI_WanJia.SetHumanScaleByPart(target, head, torso, leftArm, rightArm, leftLeg, rightLeg, ball);

                // 转发给其他装 Mod 的客机
                foreach (var host in NetGame.instance.readyclients)
                {
                    if (host.players.Count > 0)
                    {
                        Human h = host.players[0].human;
                        if (h != null && h.GetExt().isClient)
                        {
                            Chat.SendYxModMsgServer(host, YxModMsgStr("scale"),
                                $"{targetId};{head}|{torso}|{leftArm}|{rightArm}|{leftLeg}|{rightLeg}|{ball}");
                        }
                    }
                }
                return;
            }



            bool isZhaFang = false;
            if (!isDev && UI_SheZhi.pingbizhafang)
            {
                if (msg.Length == 0)
                {
                    if (nick.Length != 0)
                    {
                        string text = QuDiaoDaiMa(nick, netHost.name);
                        isZhaFang = PanDuanZhaFang(text, human);
                    }
                }
                else
                {
                    string text2 = QuDiaoDaiMa(msg, "");
                    isZhaFang = PanDuanZhaFang(text2, human); // 只对 msg 做完整判定
                }
            }
            if (isZhaFang)
            {
                Chat.TiShi(NetGame.instance.local, $"玩家 {netHost.name} 炸房,已强制他掉线");
                NetGame.instance.OnDisconnect(netHost.connection, false);
                return;
            }
            NetChat.OnReceive(clientId, nick, msg);

            //}
            for (int i = 0; i < NetGame.instance.readyclients.Count; i++)
            {
                if (NetGame.instance.readyclients[i] != client)
                {
                    NetGame.instance.SendReliable(NetGame.instance.readyclients[i], stream);
                }
            }
            /////////////////////////////
            ///


            if (msg.Length == 0)//用的是名字,有可能有颜色代码
            {
                if (nick.Length != 0)
                {
                    string text = QuDiaoDaiMa(nick, netHost.name);
                    MsgSetGongNeng(text, netHost, human);
                }
            }
            else//用的是msg
            {
                string text = QuDiaoDaiMa(msg, netHost.name); // 去掉富文本代码
                MsgSetGongNeng(text, netHost, human);
            }
            /////////////////////////////
        }
        private static void SendStr(string str, NetHost netHost = null)
        {
            if (netHost != null)
            {
                TiShi(netHost, str, TiShiMsgId.Null);
            }
            else
            {
                TiShi(str, TiShiMsgId.Null);
            }
        }
        public static void YxModHelloServer(NetHost netHost)
        {
            SendYxModMsgServer(netHost, "[YxMod]");
        }
        public static void YxModHelloClient() ///客户机向主机打招呼
        {
            SendYxModMsgClient( "[YxMod]");
        }
        public static void SendTiShiStr(NetHost netHost)
        {
            //if (!YxMod.YanZheng_OK)
            //{ return; }
            SendStr("房间提示:", netHost);
            if (UI_GongNeng.dingdian_KaiGuan)
            {
                SendStr("发送q/Q/S+E 存点，E键取点，E+A键取上一个点，E+D键取下一个点。", netHost);
            }

            SendStr("发送 “Y+数字(0-28)” 更改 Y 键动作，如 “y1” 更改 Y 键为坐下。", netHost);
            SendStr("发送 “帮助” 查看更多功能。当前版本：YxMod3.0。", netHost);
            SendStr("文件下载地址: yxmod.cc，交流群: 385272989", netHost);
            SendStr("=== 作者：一心、飞鱼鱼    贡献者：plcc 噔噔Dunn ===", netHost);
            SendStr("游戏愉快！", netHost);
        }
        public static string YxModMsgStr(string strKey)
        {
            return  $"[YxMod][{strKey}]";
        }


        private static bool PanDuanZhaFang(string text, Human human)
        {
            string lowerText = text.Trim().ToLower();

            //白名单词提前放行，直接退出函数
            if (lowerText == "q" || lowerText == "up" || lowerText == "ifg")
            {
                return false;
            }

            string[] pingbicis = UI_SheZhi.pingbici.Split('|');
            foreach (string ci in pingbicis)
            {
                if (text.Contains(ci))
                {
                    Debug.Log($"玩家 {human.player.host.name} 发送屏蔽词:{ci}");
                    return true;
                }
            }

            if (text.Length > UI_SheZhi.pingbizishu && UI_SheZhi.pingbizishu != 0)
            {
                Debug.Log($"玩家 {human.player.host.name} 发言字数:{text.Length},限制字数:{UI_SheZhi.pingbizishu}");
                return true;
            }
            //Debug.Log($"{human.player.host.name} 发言内容:{text} 上一次发言内容:{human.GetExt().lastFaYanStr}");
            if (text != human.GetExt().lastFaYanStr)
            {
                human.GetExt().lastFaYanCount = 1;
                human.GetExt().lastFaYanStr = text;
            }
            else
            {
                human.GetExt().lastFaYanCount++;
                if (human.GetExt().lastFaYanCount > UI_SheZhi.pingbicishu && UI_SheZhi.pingbicishu != 0)
                {
                    Debug.Log($"玩家 {human.player.host.name} 重复发言次数:{human.GetExt().lastFaYanCount},限制次数:{UI_SheZhi.pingbicishu}");
                    return true;
                }
            }

            float jiange = Time.time - human.GetExt().lastFaYanTimer;

            if (human.GetExt().lastFaYanTimer != 0 &&
                jiange < UI_SheZhi.fayanjiange &&
                UI_SheZhi.fayanjiange != 0)
            {
                Debug.Log($"玩家 {human.player.host.name} 发言间隔时间太短:{jiange}");
                return true;
            }

            human.GetExt().lastFaYanTimer = Time.time;
            return false;
        }


        private static string QuDiaoDaiMa(string daimaStr,string name)
        {
            string text = daimaStr;//.ToLower();
                               //Debug.Log(text);
            text = Regex.Replace(text, @"<i>|</i>", "");
            text = Regex.Replace(text, @"<b>|</b>", "");
            text = Regex.Replace(text, @"<size=\d+?>|</size>", "");
            text = Regex.Replace(text, @"<#", "<color=#");
            //Debug.Log(text);
            text = Regex.Replace(text, @"<color=#[\da-fA-F]+?>|</color>", "");
            //text = Regex.Replace(text, $"{netHost}", "");
            if(name != "")
            {
                text = text.Replace($"{name}", "").Trim();
            }
        
            return text;
        }

        public static void Help()
        {
            TiShi("所有命令：", TiShiMsgId.Null);
            TiShi("定点设置/定点/回溯/惯性/q定点/se定点/all定点/定点高度1/定点个数/定点提示 已存点", TiShiMsgId.Null);
            TiShi("up/ifg/无假死/无碰撞/飞天/闪现/超人/挂件/传送/牵手/三级跳/空气炮/拆除/潜水/超级跳", TiShiMsgId.Null);
            TiShi("冻结/转圈圈/陀螺/蹦迪/电臀/气球/气球戏法/倒立/磕头怪/吊死鬼/螃蟹/腿瘸/腿拐", TiShiMsgId.Null);
        }
        private static void MsgSetGongNeng(string msg, NetHost netHost, Human human)
        {
            //if (!YxMod.YanZheng_OK)
            //{ return; }

            if (msg.StartsWith("定点提示 "))
            {
                if (!UI_GongNeng.dingdian_KaiGuan)
                {
                    TiShi(netHost, "定点系统已关闭");
                    return;
                }
                human.GetExt().dingdian.tishiStr = msg.Substring(5);
                TiShi($"玩家 {netHost.name} 的个人定点提示修改为“{human.GetExt().dingdian.tishiStr}”");
            }
            else if (msg.StartsWith("挂件") && msg.Length > 2)
            {
                if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                {
                    TiShi(netHost, $"聊天框权限系统已关闭");
                    return;
                }
                if (!human.GetExt().liaotiankuangquanxian)
                {
                    TiShi(netHost, $"你没有聊天框权限");
                    return;
                }
                if (!UI_GongNeng.guajianxitong_KaiGuan)
                {
                    TiShi(netHost, "挂件系统已关闭");
                    return;
                }

                string strnum = msg.Substring(2);
                if (string.IsNullOrEmpty(strnum))
                {
                    return;
                }
                if (!int.TryParse(strnum, out int num))
                {
                    return;
                }

                if (num == 0)
                {
                    YxMod.QuXiaoGuaJian(human);//放弃成为挂件
                    return;
                }

                Human human2 = YxMod.ChaHumanId(strnum, human);
                YxMod.SetGuaJian(human2, human);//成为挂件
            }
            else if (msg.StartsWith("传送") && msg.Length > 2)
            {
                if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                {
                    TiShi(netHost, $"聊天框权限系统已关闭");
                    return;
                }
                if (!human.GetExt().liaotiankuangquanxian)
                {
                    TiShi(netHost, $"你没有聊天框权限");
                    return;
                }

                if (!UI_GongNeng.chuansongxitong_KaiGuan)
                {
                    TiShi(netHost, $"传送系统已关闭");
                    return;
                }

                string strnum = msg.Substring(2);
                if (string.IsNullOrEmpty(strnum))
                {
                    return;
                }
                if (!int.TryParse(strnum, out int num))
                {
                    return;
                }

                if (num == 0)
                {
                    YxMod.ChuanSong(human, human);//传送自己到自己
                    return;
                }

                Human human2 = YxMod.ChaHumanId(strnum, human);
                YxMod.ChuanSong(human, human2);
            }
            else if (msg.StartsWith("牵手") && msg.Length > 2)
            {
                if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                {
                    TiShi(netHost, $"聊天框权限系统已关闭");
                    return;
                }
                if (!human.GetExt().liaotiankuangquanxian)
                {
                    TiShi(netHost, $"你没有聊天框权限");
                    return;
                }

                if (!UI_GongNeng.qianshouxitong_KaiGuan)
                {
                    TiShi(netHost, $"牵手系统已关闭");
                    return;
                }

                string strnum = msg.Substring(2);
                if (string.IsNullOrEmpty(strnum))
                {
                    return;
                }
                if (!int.TryParse(strnum, out int num))
                {
                    return;
                }

                if (num == 0)
                {
                    //human.qianshou = false;
                    human.GetExt().qianshou_zuo = human.GetExt().qianshou_you = false;
                    human.GetExt().qianshou_zuo_human = human.GetExt().qianshou_you_human = null;
                    human.GetExt().qianshou_zuo_humanHand = human.GetExt().qianshou_you_humanHand = null;
                    return;
                }

                Human human2 = YxMod.ChaHumanId(strnum, human);

                //human.qianshou = true;
                YxMod.SetQianShou(human, human2);
            }
            else if (msg.StartsWith("背") && msg.Length > 1)
            {
                if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                {
                    TiShi(netHost, $"聊天框权限系统已关闭");
                    return;
                }
                if (!human.GetExt().liaotiankuangquanxian)
                {
                    TiShi(netHost, $"你没有聊天框权限");
                    return;
                }

                if (!UI_GongNeng.guajianxitong_KaiGuan)
                {
                    TiShi(netHost, $"挂件系统已关闭");
                    return;
                }

                string strnum = msg.Substring(1);
                if (string.IsNullOrEmpty(strnum))
                {
                    return;
                }
                if (!int.TryParse(strnum, out int num))
                {
                    return;
                }

                if (num == 0)
                {
                    //human.qianshou = false;
                    c_BeiRen.QuXiaoBeiRen(human);
                    return;
                }

                Human human2 = YxMod.ChaHumanId(strnum, human);

                //human.qianshou = true;
                c_BeiRen.BeiRen(human, human2);
            }
            else if (msg.StartsWith("定点高度") && msg.Length > 4)
            {
                if (!UI_GongNeng.dingdian_KaiGuan)
                {
                    TiShi(netHost, $"定点系统已关闭");
                    return;
                }

                string strnum = msg.Substring(4);
                if (string.IsNullOrEmpty(strnum))
                {
                    return;
                }
                if (!int.TryParse(strnum, out int num))
                {
                    return;
                }

                human.GetExt().dingdian.gaodu = num;
                TiShi($"玩家 {netHost.name} 的个人普通定点高度修改为 {human.GetExt().dingdian.gaodu}");
            }
            else if (msg.StartsWith("定点个数") && msg.Length > 4)
            {
                if (!UI_GongNeng.dingdian_KaiGuan)
                {
                    TiShi(netHost, $"定点系统已关闭");
                    return;
                }

                string strnum = msg.Substring(4);
                if (string.IsNullOrEmpty(strnum))
                {
                    return;
                }
                if (!int.TryParse(strnum, out int num))
                {
                    return;
                }

                human.GetExt().dingdian.geshu = num;
                TiShi($"玩家 {netHost.name} 的个人定点个数修改为 {human.GetExt().dingdian.geshu}");
            }
            else if ((msg.StartsWith("Y") || msg.StartsWith("y")) && msg.Length > 1)
            {
                string strnum = msg.Substring(1);
                if (string.IsNullOrEmpty(strnum))
                {
                    return;
                }
                if (!int.TryParse(strnum, out int num))
                {
                    return;
                }
                if(!UI_GongNeng.Y_KaiGuan)
                {
                    string str = $"Y键自定义系统已关闭";
                    Chat.TiShi(netHost, str);
                    return;
                }

                if (num == 0)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 晕倒");
                }
                else if (num == 1)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 坐下");
                }
                else if (num == 2)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 跪下");
                }
                else if (num == 3)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 一字马");
                }
                else if (num == 4)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 大力金刚脚");
                    if (!UI_GongNeng.jifeixitong_KaiGuan)
                    {
                        Chat.TiShi(human.player.host, "Y键击飞系统已关闭");
                    }
                }
                else if (num == 5)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 拳王");
                    if (!UI_GongNeng.jifeixitong_KaiGuan)
                    {
                        Chat.TiShi(human.player.host, "Y键击飞系统已关闭");
                    }
                }
                else if (num == 6)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 扇翅膀");
                }
                else if (num == 7)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 忍者跑");
                }
                else if (num == 8)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 我不活啦");
                }
                else if (num == 9)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 挠死你");
                }
                else if (num == 10)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 托马斯");
                }
                else if (num == 11)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 霹雳舞冻结");
                }
                else if (num == 12)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 交叉跳跃");
                }
                else if (num == 13)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 仰卧起坐");
                }
                else if (num == 14)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 俯卧撑");
                }
                else if (num == 15)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 嘻哈舞");
                }
                else if (num == 16)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 嘻哈舞2");
                }
                else if (num == 17)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 嘻哈舞3");
                }
                else if (num == 18)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 头旋");
                }
                else if (num == 19)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 姆玛踢");
                }
                else if (num == 20)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 开合跳");
                }
                else if (num == 21)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 摇摆舞");
                }
                else if (num == 22)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 桑巴舞");
                }
                else if (num == 23)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 桑巴舞2");
                }
                else if (num == 24)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 电臀舞");
                }
                else if (num == 25)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 拳击");
                }
                else if (num == 26)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 骑马舞");
                }
                else if (num == 27)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 黑鹰舞步");
                }
                else if (num == 28)
                {
                    human.GetExt().numY = num;
                    Chat.TiShi(human.player.host, "Y键动作已更改为 慢跑");
                }
            }

            else
            {
                switch (msg)
                {
                    case "q":
                    case "Q":

                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        if (!human.GetExt().dingdian.kaiguan)
                        {
                            TiShi(netHost, $"你的个人定点已关闭");
                            break;
                        }
                        if (human.GetExt().dingdian.kaiguan)
                        {
                            if (human.GetExt().dingdian.q)//q定点
                            {
                                human.GetExt().dingdian.CunDian(human, true);
                                return;
                            }
                        }
                        break;
                    case "帮助":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        Help();
                        break;
                    case "up":
                        YxMod.Up(human);
                        break;
                    case "ifg":
                        YxMod.Ifg(human);
                        break;
                    case "挂件":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.guajianxitong_KaiGuan)
                        {
                            TiShi(netHost, $"挂件系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.挂件);
                        if (!human.GetExt().ntp)
                        {
                            YxMod.SetGuaJian(NetGame.instance.server.players[0].human, human);
                        }
                        else
                        {
                            YxMod.QuXiaoGuaJian(human);
                        }
                        break;
                    case "传送":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.chuansongxitong_KaiGuan)
                        {
                            TiShi(netHost, $"传送系统已关闭");
                            break;
                        }
                        YxMod.ChuanSong(human, NetGame.instance.local.players[0].human);
                        break;
                    case "牵手":
                    case "松手":
                    case "取消牵手":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.qianshouxitong_KaiGuan)
                        {
                            TiShi(netHost, $"牵手系统已关闭");
                            break;
                        }

                        YxMod.QuXiaoQianShou(human);

                        break;
                    case "背":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.guajianxitong_KaiGuan)
                        {
                            TiShi(netHost, $"挂件系统已关闭");
                            break;
                        }
                        c_BeiRen.BeiRen(human, NetGame.instance.server.players[0].human);
                        break;
                    case "取消背":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.guajianxitong_KaiGuan)
                        {
                            TiShi(netHost, $"挂件系统已关闭");
                            break;
                        }

                        c_BeiRen.QuXiaoBeiRen(human);

                        break;
                    case "定点设置":
                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }

                        TiShi($"你的个人定点已{(human.GetExt().dingdian.kaiguan ? "打开" : "关闭")}。发送以下命令进行设置。", TiShiMsgId.Null);
                        TiShi($"“定点”           \t---\t打开/关闭 个人定点。", TiShiMsgId.Null);
                        TiShi($"“回溯”           \t---\t定点模式改为回溯模式。", TiShiMsgId.Null);
                        TiShi($"“惯性”           \t---\t带惯性定点已打开", TiShiMsgId.Null);
                        TiShi($"“Q定点”          \t---\t定点方式改为Q方式，没有文件只能发送q。", TiShiMsgId.Null);
                        TiShi($"“SE定点”         \t---\t定点方式改为SE方式。", TiShiMsgId.Null);
                        TiShi($"“ALL定点”        \t---\t定点方式改为Q方式/SE方式。", TiShiMsgId.Null);

                        break;
                    case "定点":
                    case "个人定点":
                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.定点);
                        human.GetExt().dingdian.kaiguan = !human.GetExt().dingdian.kaiguan;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().dingdian.kaiguan ? "打开了" : "关闭了")} 个人定点");
                        break;
                    case "打开定点":
                    case "打开个人定点":
                    case "定点打开":
                    case "定点开启":
                    case "开启定点":
                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.定点);
                        human.GetExt().dingdian.kaiguan = true;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().dingdian.kaiguan ? "打开了" : "关闭了")} 个人定点");
                        break;
                    case "关闭定点":
                    case "关闭个人定点":
                    case "定点关闭":
                    case "个人定点关闭":
                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.定点);
                        human.GetExt().dingdian.kaiguan = false;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().dingdian.kaiguan ? "打开了" : "关闭了")} 个人定点");
                        break;
                    case "关闭回溯":
                    case "回溯关闭":
                    case "取消回溯":
                    case "回溯取消":
                    case "普通定点":
                    case "常规定点":
                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        human.GetExt().dingdian.huisu=false;
                        TiShi($"玩家 {netHost.name} 的个人定点模式修改为 普通模式");
                        break;
                    case "huisu":
                    case "回溯":
                    case "回溯定点":
                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        human.GetExt().dingdian.huisu = !human.GetExt().dingdian.huisu;
                        TiShi($"玩家 {netHost.name} 的个人定点模式修改为 回溯模式");
                        break;
                    case "打开回溯定点":
                    case "打开回溯":
                    case "开启回溯":
                    case "开启回溯定点":
                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        human.GetExt().dingdian.huisu=true;
                        TiShi($"玩家 {netHost.name} 的个人定点模式修改为 回溯模式");
                        break;
                    case "惯性":
                    case "惯性定点":
                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        human.GetExt().dingdian.guanxing = !human.GetExt().dingdian.guanxing;
                        TiShi($"玩家 {netHost.name} 的带惯性定点已 {(human.GetExt().dingdian.guanxing ? "打开" : "关闭")}");
                        break;
                    case "打开惯性定点":
                    case "打开惯性":
                    case "开启惯性定点":
                    case "开启惯性":
                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        human.GetExt().dingdian.guanxing = true;
                        TiShi($"玩家 {netHost.name} 的带惯性定点已 打开");
                        break;
                    case "关闭惯性定点":
                    case "关闭惯性":
                    case "取消惯性定点":
                    case "取消惯性":
                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        human.GetExt().dingdian.guanxing = false;
                        TiShi($"玩家 {netHost.name} 的带惯性定点已 关闭");
                        break;
                    case "Q定点":
                    case "q定点":
                    case "Q存点":
                    case "q存点":
                    case "发送q定点":
                    case "发送Q定点":
                    case "q键定点":
                    case "Q键定点":
                    case "发送q存点":
                    case "发送Q存点":
                    case "q键存点":
                    case "Q键存点":
                    case "存点q":
                    case "定点q":
                    case "存点Q":
                    case "定点Q":
                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        human.GetExt().dingdian.q=true;
                        TiShi($"玩家 {netHost.name} 的个人定点方式修改为 Q模式");
                        break;
                    case "SE定点":
                    case "se定点":
                    case "Se定点":
                    case "sE定点":
                    case "SE存点":
                    case "se存点":
                    case "Se存点":
                    case "sE存点":
                    case "定点SE":
                    case "定点se":
                    case "定点Se":
                    case "定点sE":
                    case "存点SE":
                    case "存点se":
                    case "存点Se":
                    case "存点sE":

                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        human.GetExt().dingdian.se=true;
                        TiShi($"玩家 {netHost.name} 的个人定点方式修改为 SE模式");
                        break;
                    case "ALL定点":
                    case "all定点":
                    case "所有方式定点":
                    case "所有定点方式":
                    case "q se定点":
                    case "qse定点":
                    case "q+se定点":
                    case "q和se定点":
                    case "两种定点":
                    case "ALL存点":
                    case "all存点":
                    case "所有方式存点":
                    case "所有存点方式":
                    case "q se存点":
                    case "qse存点":
                    case "q+se存点":
                    case "q和se存点":
                    case "两种存点":

                        if (!UI_GongNeng.dingdian_KaiGuan)
                        {
                            TiShi(netHost, $"定点系统已关闭");
                            break;
                        }
                        human.GetExt().dingdian.q = human.GetExt().dingdian.se = true;
                        TiShi($"玩家 {netHost.name} 的个人定点方式修改为 Q模式/SE模式");
                        break;

                    case "无假死":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.wujiasi_KaiGuan)
                        {
                            TiShi(netHost, $"无假死系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.无假死);
                        human.GetExt().wujiasi = !human.GetExt().wujiasi;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().wujiasi ? "打开了" : "关闭了")} 无假死");
                        break;
                    case "无碰撞":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.wupengzhuang_KaiGuan)
                        {
                            TiShi(netHost, $"无碰撞系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.无碰撞);
                        human.GetExt().wupengzhuang = !human.GetExt().wupengzhuang;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().wupengzhuang ? "打开了" : "关闭了")} 无碰撞");
                        break;
                    case "飞行":
                    case "飞天":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.feitianxitong_KaiGuan)
                        {
                            TiShi(netHost, $"飞天系统已关闭");
                            break;
                        }
                        human.GetExt().feitian = !human.GetExt().feitian;
                        YxMod.SetFeiTian(human);
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().feitian ? "学会了" : "忘记了")} 个人飞天");
                        if (human.GetExt().feitian)
                        {
                            Chat.TiShi(human.player.host, "普通情况下是正常飞天。按住左键，W，空格，保持两秒，可进入超人状态。");
                        }
                        break;
                    case "超人":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.feitianxitong_KaiGuan)
                        {
                            TiShi(netHost, $"飞天系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.超人);
                        human.GetExt().chaoren = !human.GetExt().chaoren;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().chaoren ? "学会了" : "忘记了")} 超人");
                        break;

                    case "闪现":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.shanxianxitong_KaiGuan)
                        {
                            TiShi(netHost, $"闪现系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.闪现);
                        human.GetExt().shanxian = !human.GetExt().shanxian;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().shanxian ? "学会了" : "忘记了")} 闪现");
                        break;
                    case "转圈":
                    case "转圈圈":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.转圈圈);
                        human.GetExt().zhuanquan = !human.GetExt().zhuanquan;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().zhuanquan ? "学会了" : "忘记了")} 转圈圈{(human.GetExt().zhuanquan ? "，按住空格可以转圈圈" : "")}");
                        break;
                    case "陀螺":
                    case "小陀螺":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.陀螺);
                        human.GetExt().tuoluo = !human.GetExt().tuoluo;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().tuoluo ? "变成了" : "忘记了")} 小陀螺");
                        break;
                    case "跳舞":
                    case "蹦迪":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        human.GetExt().bengdi = !human.GetExt().bengdi;
                        YxMod. BengDi(human);
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().bengdi ? "学会了" : "忘记了")} 蹦迪");
                        break;
                    case "冻结":
                    case "冻住":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        human.GetExt().dongjie = !human.GetExt().dongjie;
                        YxMod.DongJie(human);
                        Chat.TiShi($"玩家 {human.player.host.name} 被 {(human.GetExt().dongjie ? "冻结了" : "解冻了")}");
                        break;
                    case "半身":
                    case "半身不遂":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        human.GetExt().banshen = true;
                        YxMod.BanShen(human);
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().banshen ? "学会了" : "取消了")} 半身不遂");
                        break;
                    case "三段跳":
                    case "三级跳":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.三级跳);
                        human.GetExt().sanjitiao = !human.GetExt().sanjitiao;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().sanjitiao ? "学会了" : "忘记了")} 三级跳");
                        break;
                    case "电臀":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        human.GetExt().diantun = !human.GetExt().diantun;
                        YxMod.DianTun(human);
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().diantun ? "学会了" : "忘记了")} 电臀");
                        break;
                    case "气球":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        human.GetExt().qiqiu = !human.GetExt().qiqiu;
                        YxMod.QiQiu(human);
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().qiqiu ? "变成了" : "忘记了")} 气球");
                        break;
                    case "气球戏法":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.气球戏法);
                        human.GetExt().qiqiuxifa = !human.GetExt().qiqiuxifa;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().qiqiuxifa ? "学会了" : "忘记了")} 左键抓住物品可以起飞的气球戏法");
                        break;
                    case "倒立":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        human.GetExt().daoli = !human.GetExt().daoli;
                        YxMod.DaoLi(human);
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().daoli ? "学会了" : "忘记了")} 双手抓地使用倒立");
                        break;
                    case "磕头怪":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.磕头怪);
                        human.GetExt().ketouguai = !human.GetExt().ketouguai;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().ketouguai ? "变成了" : "忘记了")} 磕头怪");

                        break;
                    case "吊死鬼":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        human.GetExt().diaosigui = !human.GetExt().diaosigui;
                        YxMod.DiaoSiGui(human);
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().diaosigui ? "变成了" : "忘记了")} 吊死鬼");
                        break;
                    case "螃蟹":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.螃蟹);
                        human.GetExt().pangxie = !human.GetExt().pangxie;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().pangxie ? "变成了" : "忘记了")} 一只大螃蟹");
                        break;
                    case "腿瘸":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        human.GetExt().tuique = !human.GetExt().tuique;
                        YxMod. TuiQue(human);
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().tuique ? "变成了" : "治好了")} 瘸腿");
                        break;
                    case "腿拐":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        human.GetExt().tuiguai = !human.GetExt().tuiguai;
                        YxMod.TuiGuai(human);
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().tuiguai ? "变成了" : "治好了")} 拐腿");
                        break;
                    case "潜水":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.潜水);
                        human.GetExt().qianshui = !human.GetExt().qianshui;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().qianshui ? "学会了" : "忘记了")} 潜水");
                        break;
                    case "拆除":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.拆除);
                        human.GetExt().chaichu = !human.GetExt().chaichu;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().chaichu ? "学会了" : "忘记了")} 拆除");
                        if (human.GetExt().chaichu)
                        {
                            Chat.TiShi(human.player.host, "开启拆除,左手抓住目标，即可拆卸。");
                        }
                        break;
                    case "空气炮":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.空气炮);
                        human.GetExt().kongqipao = !human.GetExt().kongqipao;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().kongqipao ? "获取了" : "丢掉了")} 空气炮");
                        if (human.GetExt().kongqipao)
                        {
                            Chat.TiShi(human.player.host, "长按 鼠标左键 向前方打出空气炮，被击中的物体会被击飞。");
                        }
                        break;
                    case "超级跳":
                        if (!UI_GongNeng.liaotiankuangquanxian_KaiGuan)
                        {
                            TiShi(netHost, $"聊天框权限系统已关闭");
                            break;
                        }
                        if (!human.GetExt().liaotiankuangquanxian)
                        {
                            TiShi(netHost, $"你没有聊天框权限");
                            break;
                        }
                        if (!UI_GongNeng.yulexitong_KaiGuan)
                        {
                            TiShi(netHost, $"娱乐系统已关闭");
                            break;
                        }
                        //YxMod.SetHumanKaiGuan(human, NengLi.超级跳);
                        human.GetExt().chaojitiao = !human.GetExt().chaojitiao;
                        Chat.TiShi($"玩家 {human.player.host.name} {(human.GetExt().chaojitiao ? "学会了" : "忘记了")} 超级跳");
                        break;


                }
            }
        }
        public static void Send(string msg, bool formatMsg = true)
        {
            if (NetGame.isNetStarted)
            {
                if (!UI_SheZhi.MingZiSheZhi && !UI_SheZhi.FaYanSheZhi)
                    //if (!GeRenLiaoTian_MingZi && !GeRenLiaoTian_XiaoXi)
                {
                    NetGame.instance.SendChatMessage(msg);
                }
                else
                {
                    Chat.FaYan(msg, formatMsg);
                }

                if (NetGame.isServer)
                {
                    //Debug.Log(msg);
                    MsgSetGongNeng(msg, NetGame.instance.server, NetGame.instance.server.players[0].human);
                }

                string msg1 = QuDiaoDaiMa(msg,"");
                string time = DateTime.Now.ToString("HH:mm:ss");
                Debug.Log($"[{time}] [消息]我 : {msg1}");
            }
        }
        public static void FaYan(string msg, bool formatMsg = true)
        {
            string name = "";
            if (UI_SheZhi.MingZiSheZhi)
            {
                if (UI_SheZhi.MingZiZiDingYi)
                {
                    name = UI_SheZhi.MingZiDaiMa;
                }
                else
                {
                    name = UI_SheZhi.MingZiStr;
                    name = SetDaXiaoYanSe(name, UI_SheZhi.MingZiDaXiaoID, UI_SheZhi.MingZiYanSeID, mingzi: true);
                }


                if (UI_SheZhi.MingZiCuTi)
                {
                    name = SetTxtCuTi(name);
                }
                if (UI_SheZhi.MingZiXieTi)
                {
                    name = SetTxtXieTi(name);
                }
            }
            else if (!UI_SheZhi.MingZiSheZhi)
            {
                name = SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID());
            }

            if (UI_SheZhi.FaYanSheZhi && formatMsg)
            {
                msg = SetDaXiaoYanSe(msg, UI_SheZhi.FaYanDaXiaoID, UI_SheZhi.FaYanYanSeID, mingzi: false);
                if (UI_SheZhi.FaYanCuTi)
                {
                    msg = SetTxtCuTi(msg);
                }
                if (UI_SheZhi.FaYanXieTi)
                {
                    msg = SetTxtXieTi(msg);
                }
                SendChatMessage(name + " " + msg, "");

            }
            else
            {
                SendChatMessage(name, msg);
            }

        }
        public static void TiShi(NetHost netHost, string neirong, TiShiMsgId chatMsgId = TiShiMsgId.GeRenTiShi)//服务端发送单人通知消息
        {
            if (neirong.Length == 0)
            {
                //Debug.Log("TiShi:neirong不能为空");
                return;
            }

            switch (chatMsgId)
            {
                case TiShiMsgId.GeRenTiShi:
                    if (GeRenTiShi_XiaoXi)
                    {
                        neirong = SetTxtYanSe(neirong, GeRenTiShi_XiaoXi_YanSe);
                        neirong = SetTxtSize(neirong, Convert.ToInt32(GeRenTiShi_XiaoXi_DaXiao));
                        if (GeRenTiShi_XiaoXi_CuTi)
                        {
                            neirong = SetTxtCuTi(neirong);
                        }
                        if (GeRenTiShi_XiaoXi_XieTi)
                        {
                            neirong = SetTxtXieTi(neirong);
                        }
                        SendChatMessageToClient(netHost, $"{neirong}");
                    }
                    break;
                case TiShiMsgId.Null:
                    neirong = SetTxtSize(neirong, 16);
                    SendChatMessageToClient(netHost, $"{neirong}");
                    break;
            }
        }
        public static void TiShi(string neirong, TiShiMsgId chatMsgId = TiShiMsgId.GongNengBianGeng)//全局提示消息
        {
            if (neirong.Length == 0)
            {
                //Debug.Log("TiShi:neirong不能为空");
                return;
            }

            switch (chatMsgId)
            {
                case TiShiMsgId.Join://进场消息
                    if (JinRuLiKai_XiaoXi)
                    {
                        neirong = SetTxtYanSe(neirong, JinRuLiKai_XiaoXi_YanSe);
                        neirong = SetTxtSize(neirong, Convert.ToInt32(JinRuLiKai_XiaoXi_DaXiao));
                        if (JinRuLiKai_XiaoXi_CuTi)
                        {
                            neirong = SetTxtCuTi(neirong);
                        }
                        if (JinRuLiKai_XiaoXi_XieTi)
                        {
                            neirong = SetTxtXieTi(neirong);
                        }
                        SendChatMessage(neirong, "");
                    }
                    break;
                case TiShiMsgId.GongNengBianGeng:
                    if (GongNengBianGeng_XiaoXi)
                    {
                        neirong = SetTxtYanSe(neirong, GongNengBianGeng_XiaoXi_YanSe);
                        neirong = SetTxtSize(neirong, Convert.ToInt32(GongNengBianGeng_XiaoXi_DaXiao));
                        if (GongNengBianGeng_XiaoXi_CuTi)
                        {
                            neirong = SetTxtCuTi(neirong);
                        }
                        if (GongNengBianGeng_XiaoXi_XieTi)
                        {
                            neirong = SetTxtXieTi(neirong);
                        }
                        SendChatMessage(neirong, "");
                    }
                    break;
                case TiShiMsgId.XiTongTiShi:
                    if (XiTongTiShi_XiaoXi)
                    {
                        neirong = SetTxtYanSe(neirong, XiTongTiShi_XiaoXi_YanSe);
                        neirong = SetTxtSize(neirong, Convert.ToInt32(XiTongTiShi_XiaoXi_DaXiao));
                        if (XiTongTiShi_XiaoXi_CuTi)
                        {
                            neirong = SetTxtCuTi(neirong);
                        }
                        if (XiTongTiShi_XiaoXi_XieTi)
                        {
                            neirong = SetTxtXieTi(neirong);
                        }
                        SendChatMessage(neirong, "");
                    }
                    break;
                case TiShiMsgId.Null:
                    neirong = SetTxtSize(neirong, 16);
                    SendChatMessage(neirong, "");
                    break;

            }
        }
    

        public static void SendChatMessage(string name, string msg)
        {
            //string friendPersonaName = SteamFriends.GetFriendPersonaName(SteamUser.GetSteamID());
            NetChat.OnReceive(NetGame.instance.local.hostId, name, msg);
            NetStream netStream = NetGame.BeginMessage(NetMsgId.Chat);
            try
            {
                netStream.WriteNetId(NetGame.instance.local.hostId);
                netStream.Write(name);
                netStream.Write(msg);
                if (NetGame.isServer)
                {
                    for (int i = 0; i < NetGame.instance.readyclients.Count; i++)
                    {
                        NetGame.instance.SendReliable(NetGame.instance.readyclients[i], netStream);
                    }
                }
                else if (NetGame.instance.server != null && NetGame.instance.server.isReady)
                {
                    NetGame.instance.SendReliableToServer(netStream);
                }
            }
            finally
            {
                if (netStream != null)
                {
                    netStream = netStream.Release();
                }
            }
        }

        public static void SendChatMessageToClient(NetHost netHost, string name)//服务端 向 netHost 发送消息
        {
            if (netHost == null)
            {
                //Debug.Log("SendChatMessageToClient：netHost为空");
                return;
            }


            if (netHost == NetGame.instance.local)//包括服务端和客户端
            {
                NetChat.Print(name);
                return;
            }
            NetStream netStream = NetGame.BeginMessage(NetMsgId.Chat);
            try
            {
                string msg = "";
                netStream.WriteNetId(netHost.hostId);
                netStream.Write(name);
                netStream.Write(msg);
                if (NetGame.isServer)
                {
                    NetGame.instance.SendReliable(netHost, netStream);
                }
            }
            finally
            {
                if (netStream != null)
                {
                    netStream = netStream.Release();
                }
            }
        }

        private static string SuiJiLiangSe(int liangdu = 50)
        {
            // 生成随机红、绿、蓝三原色分量
            int red;
            int green;
            int blue;
            double brightnessThreshold = Convert.ToDouble(liangdu) / 100;
            double brightness;
            do
            {
                red = _random.Next(0, 256);
                green = _random.Next(0, 256);
                blue = _random.Next(0, 256);

                // 计算亮度
                brightness = (red * 0.3 + green * 0.59 + blue * 0.11) / 255;
            } while (brightness <= brightnessThreshold);

            return "#" + red.ToString("X2") + green.ToString("X2") + blue.ToString("X2");
        }
        private static int GetColorStep(int textLength)
        {
            // 这里仅做演示，实际应根据颜色转换算法和文本长度计算合理的步长
            return textLength - 1;
        }

        public static Color32 Hex_Color32(string hex)
        {
            try
            {
                hex = hex.Replace("#", "");

                byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                byte a = 255; // 默认全透明
                              //if (hex.Length == 8) a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                return new Color32(r, g, b, a);
            }
            catch
            {
                return new Color32(0xFF, 0xFF, 0xFF, 0xFF);
            }

        }

        // 将Color32对象转换为十六进制颜色字符串
        private static string Color32_Hex(Color32 color)
        {
            return "#" + color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
        }
        private static string InterpolateColor(string startColorHex, string endColorHex, int position, int colorStep)
        {
            Color32 startColor = Hex_Color32(startColorHex);
            Color32 endColor = Hex_Color32(endColorHex);

            // 计算位置比例
            float ratio = (float)position / colorStep;

            // RGB空间的颜色插值
            byte r = (byte)Mathf.Lerp(startColor.r, endColor.r, ratio);
            byte g = (byte)Mathf.Lerp(startColor.g, endColor.g, ratio);
            byte b = (byte)Mathf.Lerp(startColor.b, endColor.b, ratio);

            // Alpha通道通常也可以进行插值，这里假设Alpha不变
            byte a = startColor.a; // 或者使用endColor.a

            // 创建并返回新的Color32对象
            return Color32_Hex(new Color32(r, g, b, a));
        }
        private static int InterpolateSize(int startSize, int endSize, int position, int colorStep)
        {
            // 计算位置比例
            float ratio = (float)position / colorStep;
            int ret= (int)Mathf.Lerp(startSize, endSize, ratio);
            // 创建并返回新的Color32对象
            return ret;
        }
        private static string SetTxtYanSe(string text, string Color)
        {
            // 新增检查：如果text是null或空字符串，直接返回空字符串
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append($"<color={Color}>{text}</color>");
            return sb.ToString();
        }

        private static string SetTxtSize(string text, int size)
        {
            // 新增检查：如果text是null或空字符串，直接返回空字符串
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append($"<size={size}>{text}</size>");
            return sb.ToString();
        }

        internal static string SetDaXiaoYanSe(string str, int daxiaoID,int yanseID,bool mingzi=true)
        {
            //int size0 =-1 ;
            int size1=-1;
            int size2 = -1;
            //string yanse0="#FFFFFF";
            string yanse1 = "#FFFFFF";
            string yanse2 = "#FFFFFF";
            int liangdu=50;

            string retStr= "";

            // 新增检查：如果text是null或空字符串，直接返回空字符串
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            switch (daxiaoID)
            {
                case 0://大小固定
                    size1 = mingzi ? UI_SheZhi.MingZiDaXiao : UI_SheZhi.FaYanDaXiao;
                    switch (yanseID)
                    {
                        case 0:
                            yanse1 = mingzi ? UI_SheZhi.MingZiYanSe : UI_SheZhi.FaYanYanSe;
                            break;
                        case 1:
                            yanse1 = mingzi ? UI_SheZhi.MingZiJianBianYanSe1 : UI_SheZhi.FaYanJianBianYanSe1;
                            yanse2 = mingzi ? UI_SheZhi.MingZiJianBianYanSe2 : UI_SheZhi.FaYanJianBianYanSe2;
                            break;
                        case 2:
                            yanse1 = mingzi ? UI_SheZhi.MingZiTiaoYueYanSe1 : UI_SheZhi.FaYanTiaoYueYanSe1;
                            yanse2 = mingzi ? UI_SheZhi.MingZiTiaoYueYanSe2 : UI_SheZhi.FaYanTiaoYueYanSe2;
                            break;
                        case 3:
                            liangdu = mingzi ? UI_SheZhi.MingZiSuiJiYanSeLiangDu : UI_SheZhi.FaYanSuiJiYanSeLiangDu;
                            break;
                    }
                    break;
                case 1://大小渐变
                    size1 = mingzi ? UI_SheZhi.MingZiJianBianDaXiao1 : UI_SheZhi.FaYanJianBianDaXiao1;
                    size2 = mingzi ? UI_SheZhi.MingZiJianBianDaXiao2 : UI_SheZhi.FaYanJianBianDaXiao2;
                    switch (yanseID)
                    {
                        case 0:
                            yanse1 = mingzi ? UI_SheZhi.MingZiYanSe : UI_SheZhi.FaYanYanSe;
                            break;
                        case 1:
                            yanse1 = mingzi ? UI_SheZhi.MingZiJianBianYanSe1 : UI_SheZhi.FaYanJianBianYanSe1;
                            yanse2 = mingzi ? UI_SheZhi.MingZiJianBianYanSe2 : UI_SheZhi.FaYanJianBianYanSe2;
                            break;
                        case 2:
                            yanse1 = mingzi ? UI_SheZhi.MingZiTiaoYueYanSe1 : UI_SheZhi.FaYanTiaoYueYanSe1;
                            yanse2 = mingzi ? UI_SheZhi.MingZiTiaoYueYanSe2 : UI_SheZhi.FaYanTiaoYueYanSe2;
                            break;
                        case 3:
                            liangdu = mingzi ? UI_SheZhi.MingZiSuiJiYanSeLiangDu : UI_SheZhi.FaYanSuiJiYanSeLiangDu;
                            break;
                    }
                    break;
                case 2://大小跳跃
                    size1 = mingzi ? UI_SheZhi.MingZiTiaoYueDaXiao1 : UI_SheZhi.FaYanTiaoYueDaXiao1;
                    size2 = mingzi ? UI_SheZhi.MingZiTiaoYueDaXiao2 : UI_SheZhi.FaYanTiaoYueDaXiao2;
                    switch (yanseID)
                    {
                        case 0:
                            yanse1 = mingzi ? UI_SheZhi.MingZiYanSe : UI_SheZhi.FaYanYanSe;
                            break;
                        case 1:
                            yanse1 = mingzi ? UI_SheZhi.MingZiJianBianYanSe1 : UI_SheZhi.FaYanJianBianYanSe1;
                            yanse2 = mingzi ? UI_SheZhi.MingZiJianBianYanSe2 : UI_SheZhi.FaYanJianBianYanSe2;
                            break;
                        case 2:
                            yanse1 = mingzi ? UI_SheZhi.MingZiTiaoYueYanSe1 : UI_SheZhi.FaYanTiaoYueYanSe1;
                            yanse2 = mingzi ? UI_SheZhi.MingZiTiaoYueYanSe2 : UI_SheZhi.FaYanTiaoYueYanSe2;
                            break;
                        case 3:
                            liangdu = mingzi ? UI_SheZhi.MingZiSuiJiYanSeLiangDu : UI_SheZhi.FaYanSuiJiYanSeLiangDu;
                            break;
                    }
                    break;
                case 3://大小随机
                    size1 = mingzi ? UI_SheZhi.MingZiSuiJiDaXiao1 : UI_SheZhi.FaYanSuiJiDaXiao1;
                    size2 = mingzi ? UI_SheZhi.MingZiSuiJiDaXiao2 : UI_SheZhi.FaYanSuiJiDaXiao2;
                    switch (yanseID)
                    {
                        case 0:
                            yanse1 = mingzi ? UI_SheZhi.MingZiYanSe : UI_SheZhi.FaYanYanSe;
                            break;
                        case 1:
                            yanse1 = mingzi ? UI_SheZhi.MingZiJianBianYanSe1 : UI_SheZhi.FaYanJianBianYanSe1;
                            yanse2 = mingzi ? UI_SheZhi.MingZiJianBianYanSe2 : UI_SheZhi.FaYanJianBianYanSe2;
                            break;
                        case 2:
                            yanse1 = mingzi ? UI_SheZhi.MingZiTiaoYueYanSe1 : UI_SheZhi.FaYanTiaoYueYanSe1;
                            yanse2 = mingzi ? UI_SheZhi.MingZiTiaoYueYanSe2 : UI_SheZhi.FaYanTiaoYueYanSe2;
                            break;
                        case 3:
                            liangdu = mingzi ? UI_SheZhi.MingZiSuiJiYanSeLiangDu : UI_SheZhi.FaYanSuiJiYanSeLiangDu;
                            break;
                    }
                    break;
            }

            //StringBuilder sb = new StringBuilder();
            if (str.Length <= 1)
            {
                // 单字符：颜色/大小渐变应取中值而不是使用起始色
                int daxiao;
                if (daxiaoID == 3)
                {
                    daxiao = _random.Next(size1, size2 + 1);
                }
                else if (daxiaoID == 1)
                {
                    // 大小渐变：取中间值
                    daxiao = (size1 + size2) / 2;
                }
                else
                {
                    daxiao = size1;
                }

                string yanse;
                if (yanseID == 1)
                {
                    // 颜色渐变：取中间颜色（t=0.5）
                    var gradient = mingzi ? UI_SheZhi.MingZiSeed.gradient : UI_SheZhi.FaYanSeed.gradient;
                    Color color = gradient.Evaluate(0.5f);
                    yanse = $"#{ColorUtility.ToHtmlStringRGB(color)}";
                }
                else if (yanseID == 2)
                {
                    // 跳跃颜色，单字符用第一色
                    yanse = yanse1;
                }
                else if (yanseID == 3)
                {
                    // 随机颜色
                    yanse = SuiJiLiangSe(liangdu);
                }
                else
                {
                    // 固定颜色
                    yanse = yanse1;
                }

                retStr = $"<size={daxiao}><color={yanse}>{str}</color></size>";
            }
            else
            {
                if(daxiaoID==0 )
                {
                    if(yanseID == 0)
                    {
                        retStr = $"<color={yanse1}>{str}</color>";
                    }
                    else
                    {
                        int Length = str.Length;
                        int colorStep = GetColorStep(Length);
                        for (int i = 0; i < Length; i++)
                        {
                            string currentColor="#FFFFFF";
                            if (yanseID == 1)
                            {
                                float t = Length == 1 ? 0.5f : i / (float)(Length - 1); // 单字符时使用中间颜色，避免除零
                                Color color = Color.white; 
                                if (mingzi)
                                {
                                    color = UI_SheZhi.MingZiSeed.gradient.Evaluate(t);
                                }else{
                                    color = UI_SheZhi.FaYanSeed.gradient.Evaluate(t);
                                }
                                currentColor = $"#{ColorUtility.ToHtmlStringRGB(color)}";
                            }
                            else if (yanseID == 2)
                            {
                                 currentColor = i % 2 == 0 ? yanse1 : yanse2;
                            }
                            else if(yanseID==3)
                            {
                                 currentColor = SuiJiLiangSe(liangdu);
                            }
                            retStr = retStr + $"<color={currentColor}>{str[i]}</color>";
                            //Debug.Log(retStr);
                        }
                    }
                    retStr = $"<size={size1}>{retStr}</size>";
                    //Debug.Log ("最终" + retStr);
                }
                else
                {
                    if (yanseID == 0)
                    {
                        int Length = str.Length;
                        int colorStep = GetColorStep(Length);
                        for (int i = 0; i < Length; i++)
                        {
                            int currentSize = 0;
                            if (daxiaoID == 1)
                            {
                                currentSize = InterpolateSize(size1, size2, i, colorStep);
                            }
                            else if (daxiaoID == 2)
                            {
                                currentSize = i % 2 == 0 ? size1 : size2;
                            }
                            else if (daxiaoID == 3)
                            {
                                currentSize = _random.Next(size1, size2 + 1);
                            }
                            retStr = retStr + $"<size={currentSize}>{str[i]}</size>";
                        }
                        retStr = $"<color={yanse1}>{retStr}</color>";
                    }
                    else
                    {
                        int Length = str.Length;
                        int colorStep = GetColorStep(Length);
                        for (int i = 0; i < Length; i++)
                        {
                            int currentSize = 0;
                        
                            if (daxiaoID == 1)
                            {
                                currentSize = InterpolateSize(size1, size2, i, colorStep);
                            }
                            else if (daxiaoID == 2)
                            {
                                currentSize = i % 2 == 0 ? size1 : size2;
                            }
                            else if (daxiaoID == 3)
                            {
                                currentSize = _random.Next(size1, size2 + 1);
                            }
                            string currentColor = "#FFFFFF";
                            if (yanseID == 1)
                            {
                                float t = Length == 1 ? 0.5f : i / (float)(Length - 1); // 单字符时使用中间颜色，避免除零
                                Color color = Color.white;
                                if (mingzi)
                                {
                                    color = UI_SheZhi.MingZiSeed.gradient.Evaluate(t);
                                }
                                else
                                {
                                    color = UI_SheZhi.FaYanSeed.gradient.Evaluate(t);
                                }
                                currentColor = $"#{ColorUtility.ToHtmlStringRGB(color)}";
                            }
                            else if (yanseID == 2)
                            {
                                currentColor = i % 2 == 0 ? yanse1 : yanse2;
                            }
                            else if (yanseID == 3)
                            {
                                currentColor = SuiJiLiangSe(liangdu);
                            }
                            retStr = retStr + $"<size={currentSize}><color={currentColor}>{str[i]}</color></size>";
                        }
                    }
                }
            }
            return retStr;

        }

        private static string SetTxtCuTi(string text)
        {
            // 新增检查：如果text是null或空字符串，直接返回空字符串
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append($"<b>{text}</b>");
            return sb.ToString();
        }
        private static string SetTxtXieTi(string text)
        {
            // 新增检查：如果text是null或空字符串，直接返回空字符串
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append($"<i>{text}</i>");
            return sb.ToString();
        }

    }

}
