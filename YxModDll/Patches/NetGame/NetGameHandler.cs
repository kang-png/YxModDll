using Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YxModDll.Mod;

namespace YxModDll.Patches
{
    internal class NetGameHandler
    {
        private static int kick_times = 0;
        public static void OnServerReceive(NetGame instance, NetHost client, NetStream stream)
        {
            if (!NetGame.isServer)
                return;

            NetMsgId netMsgId = stream.ReadMsgId();

            if (NetGame.netlog &&
                netMsgId != NetMsgId.Container &&
                netMsgId != NetMsgId.Delta &&
                netMsgId != NetMsgId.Event &&
                netMsgId != NetMsgId.Move)
            {
                Debug.LogFormat("msg {0}", netMsgId);
            }

            switch (netMsgId)
            {
                case NetMsgId.Container:
                    {
                        uint num = instance.ignoreDeltasMask;
                        instance.ignoreDeltasMask &= 0xFFFFFFFEu;
                        uint num2 = stream.ReadUInt32(4);
                        if (num2 != NetGame.nextLevelInstanceID)
                        {
                            instance.ignoreDeltasMask |= 1u;
                        }
                        client.readFrameId = (int)stream.ReadUInt32(22);
                        NetStream netStream = stream.ReadStream();
                        try
                        {
                            while (netStream != null)
                            {
                                OnServerReceive(instance, client, netStream);
                                netStream = netStream.Release();
                                netStream = stream.ReadStream();
                            }
                        }
                        finally
                        {
                            if (netStream != null)
                            {
                                netStream = netStream.Release();
                            }
                        }
                        instance.ignoreDeltasMask = (instance.ignoreDeltasMask & 0xFFFFFFFEu) | (num & 1u);
                        break;
                    }
                case NetMsgId.Helo:
                    instance.OnClientHelo(client, stream);
                    if (Chat.JinRuLiKai_XiaoXi)
                    {
                        Chat.TiShi($"玩家 {client.name} 进来了", TiShiMsgId.Join);
                        Chat.YxModHelloServer(client);
                        Chat.SendTiShiStr(client);
                    }
                    break;
                case NetMsgId.AddPlayer:
                    NetGameReflection.OnRequestAddPlayerServer(instance, client, stream);
                    break;
                case NetMsgId.RemovePlayer:
                    NetGameReflection.OnRequestRemovePlayerServer(instance, client, stream);
                    break;
                case NetMsgId.Move:
                    {
                        uint id2 = stream.ReadNetId();
                        NetPlayer netPlayer = client.FindPlayer(id2);
                        if (netPlayer != null)
                        {
                            netPlayer.ReceiveMove(stream);
                        }
                        break;
                    }
                case NetMsgId.Kick:
                    if (App.state == AppSate.ServerPlayLevel)
                    {
                        Game.instance.RespawnAllPlayers(client);
                    }
                    break;
                case NetMsgId.Delta:
                    {
                        uint id3 = stream.ReadNetId();
                        NetScope netScope2 = NetScope.Find(id3);
                        if ((instance.ignoreDeltasMask == 0 || netScope2 is NetPlayer) && netScope2 != null)
                        {
                            netScope2.OnReceiveAck(client, stream, client.GetReadFrameId());
                        }
                        break;
                    }
                case NetMsgId.Event:
                    {
                        uint id = stream.ReadNetId();
                        NetScope netScope = NetScope.Find(id);
                        if ((instance.ignoreDeltasMask == 0 || netScope is NetPlayer) && netScope != null)
                        {
                            netScope.OnReceiveEventAck(client, stream, client.GetReadFrameId());
                        }
                        break;
                    }
                case NetMsgId.LoadLevel:
                    NetGameReflection.OnReceiveLevelAck(instance, client, stream);
                    break;
                case NetMsgId.RequestSkin:
                    NetGameReflection.OnRequestSkinServer(instance, client, stream);
                    break;
                case NetMsgId.SendSkin:
                    NetGameReflection.OnReceiveSkin(instance, stream);
                    break;
                case NetMsgId.Chat:
                    Chat.OnReceiveChatServer(client, stream);
                    break;
                case NetMsgId.AddHost:
                case NetMsgId.RemoveHost:

                default:
                    break;
            }
        }

        public static void OnClientReceive(NetGame instance, object connection, NetStream stream)
        {
            NetMsgId netMsgId = stream.ReadMsgId();

            if (NetGame.netlog &&
                netMsgId != NetMsgId.Container &&
                netMsgId != NetMsgId.Delta &&
                netMsgId != NetMsgId.Event &&
                netMsgId != NetMsgId.Move &&
                netMsgId != NetMsgId.Achievement)
            {
                Debug.LogFormat("msg {0}", netMsgId);
            }

            if (!NetGame.isClient)
            {
                if (netMsgId != NetMsgId.Helo && netMsgId != NetMsgId.Container && netMsgId != NetMsgId.Kick)
                    return;

                lock (NetGameReflection.GetStateLock())
                {
                    if (!NetGame.isNetStarting && !NetGame.isClient)
                        return;
                }
            }

            switch (netMsgId)
            {
                case NetMsgId.Container:
                    {
                        uint num = instance.ignoreDeltasMask;
                        instance.ignoreDeltasMask &= 0xFFFFFFFEu;
                        uint num2 = stream.ReadUInt32(4);
                        if (num2 != NetGame.currentLevelInstanceID)
                        {
                            instance.ignoreDeltasMask |= 1u;
                        }
                        instance.server.readFrameId = (int)stream.ReadUInt32(22);
                        NetStream netStream = stream.ReadStream();
                        try
                        {
                            while (netStream != null)
                            {
                                OnClientReceive(instance, connection, netStream);
                                netStream = netStream.Release();
                                netStream = stream.ReadStream();
                            }
                        }
                        finally
                        {
                            if (netStream != null)
                            {
                                netStream = netStream.Release();
                            }
                        }
                        instance.ignoreDeltasMask = (instance.ignoreDeltasMask & 0xFFFFFFFEu) | (num & 1u);
                        break;
                    }

                case NetMsgId.Helo:
                    instance.OnHelo(connection, stream);
                    break;

                case NetMsgId.Kick:
                    kick_times++;
                    Chat.TiShi(instance.local, $"房主第 {kick_times} 次尝试踢你,已拦截");
                    if (kick_times == 999)
                    {
                        kick_times = 0;
                        Chat.TiShi(instance.local, "房主已踢了你999次");
                        App.instance.ServerKicked();
                    }
                    break;
                case NetMsgId.AddHost:
                    {
                        uint num3 = stream.ReadNetId();
                        if (instance.server != null)
                        {
                            instance.server.isReady = true;
                        }
                        while (num3 != 0)
                        {
                            string text = stream.ReadString();
                            if (instance.local != null && num3 == instance.local.hostId)
                            {
                                if (!instance.local.isReady)
                                {
                                    instance.local.isReady = true;
                                    instance.readyclients.Add(instance.local);
                                }
                            }
                            else
                            {
                                NetHost netHost2 = new NetHost(null, text);
                                netHost2.isLocal = false;
                                netHost2.hostId = num3;
                                netHost2.isReady = true;
                                NetHost item = netHost2;
                                instance.allclients.Add(item);
                                instance.readyclients.Add(item);
                            }
                            num3 = stream.ReadNetId();
                        }
                        break;
                    }
                case NetMsgId.RemoveHost:
                    {
                        uint hostId = stream.ReadNetId();
                        NetHost netHost = NetGame.instance.FindAnyHost(hostId);
                        if (netHost != null)
                        {
                            NetGameReflection.DestroyHostObjects(NetGame.instance, netHost);
                        }
                        break;
                    }
                case NetMsgId.AddPlayer:
                    {
                        NetGameReflection.OnAddPlayerClient(NetGame.instance, stream);
                        break;
                    }
                case NetMsgId.RemovePlayer:
                    {
                        NetGameReflection.OnRemovePlayerClient(NetGame.instance, stream);
                        break;
                    }
                case NetMsgId.RequestSkin:
                    {
                        uint localCoopIndex = stream.ReadNetId();
                        NetGameReflection.OnRequestSkinClient(NetGame.instance, localCoopIndex);
                        break;
                    }
                case NetMsgId.SendSkin:
                    {
                        NetGameReflection.OnReceiveSkin(NetGame.instance, stream);
                        break;
                    }

                case NetMsgId.Delta:
                    {
                        uint id3 = stream.ReadNetId();
                        NetScope netScope2 = NetScope.Find(id3);
                        if ((instance.ignoreDeltasMask == 0 || netScope2 is NetPlayer) && netScope2 != null)
                        {
                            netScope2.OnReceiveDelta(stream, instance.server.GetReadFrameId());
                        }
                        break;
                    }
                case NetMsgId.Event:
                    {
                        uint id2 = stream.ReadNetId();
                        NetScope netScope = NetScope.Find(id2);
                        if ((instance.ignoreDeltasMask == 0 || netScope is NetPlayer) && netScope != null)
                        {
                            netScope.OnReceiveEvents(stream, instance.server.GetReadFrameId());
                        }
                        break;
                    }
                case NetMsgId.Move:
                    {
                        uint id = stream.ReadNetId();
                        NetPlayer netPlayer = instance.local.FindPlayer(id);
                        if (netPlayer != null)
                        {
                            netPlayer.ReceiveMoveAck(stream);
                        }
                        break;
                    }
                case NetMsgId.LoadLevel:
                    NetGameReflection.OnLoadLevel(NetGame.instance, stream);
                    break;
                case NetMsgId.Chat:
                    //OnReceiveChatClient(stream);
                    Chat.OnReceiveChatClient(stream);
                    break;
                case NetMsgId.Achievement:
                    instance.OnUnlockAchievementClient(stream);
                    break;
                default:
                    break;
            }
        }
    }
}
