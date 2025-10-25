﻿using HumanAPI;
using Multiplayer;
using Steamworks;
using System;
using System.Collections;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;
using Voronoi2;
using YxModDll.Mod;
using YxModDll.Patches;
using static MenuCameraEffects;
using static UnityEngine.UI.Image;


namespace YxModDll.Patches
{
    public class Patcher_NetGame : MonoBehaviour
    {
        //private static object stateLock = new object();

        private static FieldInfo _stateLock;
        //private static FieldInfo _speed;
        //private static FieldInfo _contents;
        //private static FieldInfo _dismissIn;

        //private static FieldInfo _serverCommands;
        //private static FieldInfo _clientCommands;

        //private static MethodInfo ShowMethod;

        public static bool pingbiyansedaima;

        private void Awake()
        {
            _stateLock = typeof(NetGame).GetField("stateLock", BindingFlags.NonPublic | BindingFlags.Static);
            //ShowMethod = typeof(NetChat).GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Instance);
            //_phase = typeof(NetChat).GetField("phase", BindingFlags.NonPublic | BindingFlags.Instance);
            //_speed = typeof(NetChat).GetField("speed", BindingFlags.NonPublic | BindingFlags.Instance);
            //_contents = typeof(NetChat).GetField("contents", BindingFlags.NonPublic | BindingFlags.Static);
            //_dismissIn = typeof(NetChat).GetField("dismissIn", BindingFlags.NonPublic | BindingFlags.Instance);
            //_serverCommands = typeof(NetChat).GetField("serverCommands", BindingFlags.Public | BindingFlags.Static);
            //_clientCommands = typeof(NetChat).GetField("clientCommands", BindingFlags.Public | BindingFlags.Static);

            Patcher2.MethodPatch(typeof(NetGame), "OnDisconnect", new[] { typeof(object), typeof(bool) }, typeof(Patcher_NetGame), "OnDisconnect", new[] { typeof(NetGame), typeof(NetHost), typeof(object), typeof(bool) });
            Patcher2.MethodPatch(typeof(NetGame), "OnServerDisconnect", new[] { typeof(NetHost) }, typeof(Patcher_NetGame), "OnServerDisconnect", new[] { typeof(NetGame), typeof(NetHost) });
            Patcher2.MethodPatch(typeof(NetGame), "OnClientHelo", new[] { typeof(NetHost), typeof(NetStream) }, typeof(Patcher_NetGame), "OnClientHelo", new[] { typeof(NetGame), typeof(NetHost), typeof(NetStream) });
            Patcher2.MethodPatch(typeof(NetGame), "OnReceiveChatClient", new[] { typeof(NetStream) }, typeof(Patcher_NetGame), "OnReceiveChatClient", new[] { typeof(NetGame), typeof(NetStream) });
            Patcher2.MethodPatch(typeof(NetGame), "OnReceiveChatServer", new[] { typeof(NetHost), typeof(NetStream) }, typeof(Patcher_NetGame), "OnReceiveChatServer", new[] { typeof(NetGame), typeof(NetHost), typeof(NetStream) });

            ////破解软趴趴
            //Patcher2.MethodPatch(typeof(NetGame), "IsUserAllowed", null, typeof(Patcher_NetGame), "IsUserAllowed", new[] { typeof(NetGame) });
            //Patcher2.MethodPatch(typeof(NetGame), "CheckUserAccess", null, typeof(Patcher_NetGame), "CheckUserAccess", new[] { typeof(NetGame) });
        }


        public static void OnDisconnect(NetGame instance, object connection, bool suppressMessage = false)
        {
            if (NetGame. isServer)
            {
                NetHost netHost = FindAnyClient(connection);
                if (netHost != null)
                {
                    foreach(NetPlayer player in netHost.players)
                    {
                        Human human =player.human;
                        if (human == null) continue; // 防止空引用
                        if (human.GetExt().bei_human != null)
                        {
                            c_BeiRen.QuXiaoBeiRen(human);
                        }
                        //然后获取 human.ragdoll.partChest 的所有子物体，如果有Human，则 SetParent(null)
                        // 2. 处理反向情况：如果当前角色是被背者（即其他角色的子物体）
                        // 遍历胸部骨骼的所有子物体，检查是否有其他Human对象
                        foreach (Transform child in human.ragdoll.partChest.transform)
                        {
                            // 尝试获取子物体上的Human组件
                            Human childHuman = child.GetComponent<Human>();
                            if (childHuman != null)
                            {
                                c_BeiRen.QuXiaoBeiRen(childHuman);
                            }
                        }
                    }

                    DestroyHostObjects(netHost);
                    OnServerDisconnect(instance,netHost);
                }
            }
            else
            {
                OnLostConnection(suppressMessage);
            }
        }
        public static void OnLostConnection(bool suppressMessage = false)
        {
            App.instance.OnLostConnection(suppressMessage);
        }
        private static void DestroyHostObjects(NetHost client)
        {
            NetPlayer[] array = client.players.ToArray();
            foreach (NetPlayer netPlayer in array)
            {
                NetGame.instance.players.Remove(netPlayer);
                netPlayer.DespawnPlayer();
            }
            NetGame.instance.allclients.Remove(client);
            NetGame.instance.readyclients.Remove(client);
            if (client.notificationQueue != null)
            {
                client.notificationQueue.clients.Remove(client);
            }
            NetScope.RemoveAllRemoteState(client, setDying: true);
        }
        private static NetHost FindAnyClient(object connection)
        {
            int i = 0;
            for (int count = NetGame.instance. allclients.Count; i < count; i++)
            {
                if (connection != null && NetGame.instance.transport.ConnectionEquals(connection, NetGame.instance.allclients[i]))
                {
                    return NetGame.instance.allclients[i];
                }
            }
            return null;
        }
        public static bool IsUserAllowed()
        {
            return true;
        }
        public static bool CheckUserAccess()
        {
            
            return true;
        }
        public static void OnClientHelo(NetGame instance, NetHost client, NetStream msg)
        {
            if (client == null)
            {
                return;
            }
            uint num = msg.ReadNetId();
            string text = msg.ReadString();
            if (num == VersionDisplay.netCode)
            {
                client.name = msg.ReadString();

                // 黑名单源头拦截：在握手阶段根据连接ID/名称直接拒绝
                try
                {
                    if (NetGame.isServer && HeiMingDan.Enabled)
                    {
                        string steamID = client?.connection != null ? client.connection.ToString() : null;
                        string playerName = client?.name ?? "玩家";
                        if (!string.IsNullOrEmpty(steamID) && HeiMingDan.ShouldBlockPlayerJoin(steamID, playerName))
                        {
                            // 不下发 AddHost/AddPlayer 等后续消息，直接踢出，避免出现“正在进入”
                            NetGame.instance.Kick(client);
                            return; // 提前返回，阻断后续握手流程
                        }
                    }
                }
                catch { }

                if (NetGame.instance.transport.IsRelayed(client))
                {
                    App.instance.OnRelayConnection(client);
                }
                NetStream netStream = NetGame.BeginMessage(NetMsgId.LoadLevel);
                try
                {
                    netStream.Write(v: false);
                    netStream.Write((uint)NetGame.instance.currentLevel, 4, 32);
                    netStream.Write(NetGame.instance.currentLevelStarted);
                    netStream.Write(NetGame.instance.currentLevelHash, 32);
                    netStream.Write((int)NetGame.instance.currentLevelType, 8);
                    netStream.Write(NetGame.nextLevelInstanceID, 4);
                    NetGame.instance.SendReliable(client, netStream);
                }
                finally
                {
                    if (netStream != null)
                    {
                        netStream = netStream.Release();
                    }
                }
                netStream = null;
                netStream = NetGame.BeginMessage(NetMsgId.AddHost);
                try
                {
                    for (int i = 0; i < NetGame.instance.readyclients.Count; i++)
                    {
                        if (NetGame.instance.readyclients[i] != client)
                        {
                            netStream.WriteNetId(NetGame.instance.readyclients[i].hostId);
                            netStream.Write(NetGame.instance.readyclients[i].name);
                        }
                    }
                    netStream.WriteNetId(0u);
                    NetGame.instance.SendReliable(client, netStream);
                }
                finally
                {
                    if (netStream != null)
                    {
                        netStream = netStream.Release();
                    }
                }
                netStream = null;
                netStream = NetGame.BeginMessage(NetMsgId.AddPlayer);
                try
                {
                    for (int j = 0; j < NetGame.instance.players.Count; j++)
                    {
                        netStream.WriteNetId(NetGame.instance.players[j].netId);
                        netStream.WriteNetId(NetGame.instance.players[j].host.hostId);
                        netStream.WriteNetId(NetGame.instance.players[j].localCoopIndex);
                        netStream.Write(NetGame.instance.players[j].skinUserId);
                        netStream.WriteArray(NetGame.instance.players[j].skinCRC, 8);
                    }
                    netStream.WriteNetId(0u);
                    NetGame.instance.SendReliable(client, netStream);
                }
                finally
                {
                    if (netStream != null)
                    {
                        netStream = netStream.Release();
                    }
                }
                if (!client.isReady)
                {
                    client.isReady = true;
                    NetGame.instance.readyclients.Add(client);
                }
                netStream = null;
                netStream = NetGame.BeginMessage(NetMsgId.AddHost);
                try
                {
                    netStream.WriteNetId(client.hostId);
                    netStream.Write(client.name);
                    netStream.WriteNetId(0u);
                    for (int k = 0; k < NetGame.instance.readyclients.Count; k++)
                    {
                        NetGame.instance.SendReliable(NetGame.instance.readyclients[k], netStream);
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

            App.instance.OnClientCountChanged();
            if (Chat.JinRuLiKai_XiaoXi && client.name != "RequestAddPlayer")
            {
                Chat.TiShi($"玩家 {client.name} 进来了", TiShiMsgId.Join);      //进场消息
                Chat.YxModHelloServer(client);//服务器发送 [YxMod]  等待客户端响应
                Chat.SendTiShiStr(client);//欢迎提示
            }
        }

        public static void OnReceiveChatClient(NetGame instance, NetStream stream)
        {
            Chat.OnReceiveChatClient(stream);
        }

        public static void OnReceiveChatServer(NetGame instance, NetHost client, NetStream stream)
        {
            Chat.OnReceiveChatServer(client, stream);
        }
        private static void SetMultiplayerLobbyID(string lobbyIDString)
        {
            ulong multiplayerLobbyLevel = 0uL;
            try
            {
                multiplayerLobbyLevel = ulong.Parse(lobbyIDString);
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogException(exception);
            }
            Game.multiplayerLobbyLevel = multiplayerLobbyLevel;
        }
        public static void OnServerDisconnect(NetGame instance, NetHost client)
        {
            NetStream netStream = NetGame.BeginMessage(NetMsgId.RemoveHost);
            try
            {
                netStream.WriteNetId(client.hostId);
                for (int i = 0; i < NetGame.instance.readyclients.Count; i++)
                {
                    if (NetGame.instance.readyclients[i] != client)
                    {
                        NetGame.instance.SendReliable(NetGame.instance.readyclients[i], netStream);
                    }
                }
            }
            finally
            {
                if (netStream != null)
                {
                    netStream = netStream.Release();
                }
            }
            App.instance.OnClientCountChanged();
            if (Chat.JinRuLiKai_XiaoXi && client.name != "RequestAddPlayer")
            {
                Chat.TiShi($"玩家 {client.name} 离开了", TiShiMsgId.Join);
            }
        }

    }
}

