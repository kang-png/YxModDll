using Multiplayer;
using YxModDll.Mod;

namespace YxModDll.Patches
{
    internal class NetGameHandler
    {
        public void OnServerReceive(NetHost client, NetStream stream)
        {
            if (!isServer)
            {
                return;
            }
            NetMsgId netMsgId = stream.ReadMsgId();
            if (netlog && netMsgId != NetMsgId.Container && netMsgId != NetMsgId.Delta && netMsgId != NetMsgId.Event && netMsgId != NetMsgId.Move)
            {
                UnityEngine.Debug.LogFormat("msg {0}", netMsgId);
            }
            switch (netMsgId)
            {
                case NetMsgId.Container:
                    {
                        uint num = ignoreDeltasMask;
                        ignoreDeltasMask &= 4294967294u;
                        uint num2 = stream.ReadUInt32(4);
                        if (num2 != nextLevelInstanceID)
                        {
                            ignoreDeltasMask |= 1u;
                        }
                        client.readFrameId = (int)stream.ReadUInt32(22);
                        NetStream netStream = stream.ReadStream();
                        try
                        {
                            while (netStream != null)
                            {
                                OnServerReceive(client, netStream);
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
                        ignoreDeltasMask = (ignoreDeltasMask & 0xFFFFFFFEu) | (num & 1u);
                        break;
                    }
                case NetMsgId.Helo:
                    OnClientHelo(client, stream);
                    //修改  增加欢迎词
                    if (Chat.JinRuLiKai_XiaoXi)
                    {
                        Chat.TiShi($"玩家 {client.name} 进来了", TiShiMsgId.Join);      //进场消息
                        Chat.YxModHelloServer(client);//服务器发送 [YxMod]  等待客户端响应
                        Chat.SendTiShiStr(client);//欢迎提示
                    }
                    break;
                case NetMsgId.AddPlayer:
                    OnRequestAddPlayerServer(client, stream);
                    break;
                case NetMsgId.RemovePlayer:
                    OnRequestRemovePlayerServer(client, stream);
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
                        if ((ignoreDeltasMask == 0 || netScope2 is NetPlayer) && netScope2 != null)
                        {
                            netScope2.OnReceiveAck(client, stream, client.GetReadFrameId());
                        }
                        break;
                    }
                case NetMsgId.Event:
                    {
                        uint id = stream.ReadNetId();
                        NetScope netScope = NetScope.Find(id);
                        if ((ignoreDeltasMask == 0 || netScope is NetPlayer) && netScope != null)
                        {
                            netScope.OnReceiveEventAck(client, stream, client.GetReadFrameId());
                        }
                        break;
                    }
                case NetMsgId.LoadLevel:
                    OnReceiveLevelAck(client, stream);
                    break;
                case NetMsgId.RequestSkin:
                    OnRequestSkinServer(client, stream);
                    break;
                case NetMsgId.SendSkin:
                    OnReceiveSkin(stream);
                    break;
                case NetMsgId.Chat:
                    //OnReceiveChatServer(client, stream);
                    Chat.OnReceiveChatServer(client, stream);//替换成自己的,处理命令消息   修改
                    break;
                case NetMsgId.AddHost:
                case NetMsgId.RemoveHost:
                    break;
            }
        }

        public void OnClientReceive(object connection, NetStream stream)
        {
            NetMsgId netMsgId = stream.ReadMsgId();
            if (netlog && netMsgId != NetMsgId.Container && netMsgId != NetMsgId.Delta && netMsgId != NetMsgId.Event && netMsgId != NetMsgId.Move && netMsgId != NetMsgId.Achievement)
            {
                UnityEngine.Debug.LogFormat("msg {0}", netMsgId);
            }
            if (!isClient)
            {
                if (netMsgId != NetMsgId.Helo && netMsgId != NetMsgId.Container && netMsgId != NetMsgId.Kick)
                {
                    return;
                }
                lock (stateLock)
                {
                    if (!isNetStarting && !isClient)
                    {
                        return;
                    }
                }
            }
            switch (netMsgId)
            {
                case NetMsgId.Container:
                    {
                        uint num = ignoreDeltasMask;
                        ignoreDeltasMask &= 4294967294u;
                        uint num2 = stream.ReadUInt32(4);
                        if (num2 != currentLevelInstanceID)
                        {
                            ignoreDeltasMask |= 1u;
                        }
                        server.readFrameId = (int)stream.ReadUInt32(22);
                        NetStream netStream = stream.ReadStream();
                        try
                        {
                            while (netStream != null)
                            {
                                OnClientReceive(connection, netStream);
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
                        ignoreDeltasMask = (ignoreDeltasMask & 0xFFFFFFFEu) | (num & 1u);
                        break;
                    }
                case NetMsgId.Helo:
                    OnHelo(connection, stream);
                    break;
                case NetMsgId.Kick:
                    //App.instance.ServerKicked();
                    kick_times++;
                    Chat.TiShi(local, $"房主第 {kick_times} 次尝试踢你,已拦截");
                    if (kick_times == 999)
                    {
                        kick_times = 0;
                        Chat.TiShi(local, "房主已踢了你999次");
                        App.instance.ServerKicked();
                    }
                    break;
                case NetMsgId.AddHost:
                    {
                        uint num3 = stream.ReadNetId();
                        if (server != null)
                        {
                            server.isReady = true;
                        }
                        while (num3 != 0)
                        {
                            string text = stream.ReadString();
                            if (local != null && num3 == local.hostId)
                            {
                                if (!local.isReady)
                                {
                                    local.isReady = true;
                                    readyclients.Add(local);
                                }
                            }
                            else
                            {
                                NetHost netHost2 = new NetHost(null, text);
                                netHost2.isLocal = false;
                                netHost2.hostId = num3;
                                netHost2.isReady = true;
                                NetHost item = netHost2;
                                allclients.Add(item);
                                readyclients.Add(item);
                            }
                            num3 = stream.ReadNetId();
                        }
                        break;
                    }
                case NetMsgId.RemoveHost:
                    {
                        uint hostId = stream.ReadNetId();
                        NetHost netHost = FindAnyHost(hostId);
                        if (netHost != null)
                        {
                            DestroyHostObjects(netHost);
                        }
                        break;
                    }
                case NetMsgId.AddPlayer:
                    OnAddPlayerClient(stream);
                    break;
                case NetMsgId.RemovePlayer:
                    OnRemovePlayerClient(stream);
                    break;
                case NetMsgId.RequestSkin:
                    {
                        uint localCoopIndex = stream.ReadNetId();
                        OnRequestSkinClient(localCoopIndex);
                        break;
                    }
                case NetMsgId.SendSkin:
                    OnReceiveSkin(stream);
                    break;
                case NetMsgId.Delta:
                    {
                        uint id3 = stream.ReadNetId();
                        NetScope netScope2 = NetScope.Find(id3);
                        if ((ignoreDeltasMask == 0 || netScope2 is NetPlayer) && netScope2 != null)
                        {
                            netScope2.OnReceiveDelta(stream, instance.server.GetReadFrameId());
                        }
                        break;
                    }
                case NetMsgId.Event:
                    {
                        uint id2 = stream.ReadNetId();
                        NetScope netScope = NetScope.Find(id2);
                        if ((ignoreDeltasMask == 0 || netScope is NetPlayer) && netScope != null)
                        {
                            netScope.OnReceiveEvents(stream, instance.server.GetReadFrameId());
                        }
                        break;
                    }
                case NetMsgId.Move:
                    {
                        uint id = stream.ReadNetId();
                        NetPlayer netPlayer = local.FindPlayer(id);
                        if (netPlayer != null)
                        {
                            netPlayer.ReceiveMoveAck(stream);
                        }
                        break;
                    }
                case NetMsgId.LoadLevel:
                    OnLoadLevel(stream);
                    break;
                case NetMsgId.Chat:
                    //OnReceiveChatClient(stream);
                    Chat.OnReceiveChatClient(stream);//客户端接收消息
                    break;
                case NetMsgId.Achievement:
                    OnUnlockAchievementClient(stream);
                    break;
            }
        }
    }
}
