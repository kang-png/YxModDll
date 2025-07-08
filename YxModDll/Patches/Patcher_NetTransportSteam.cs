using Multiplayer;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using YxModDll.Mod;
using YxModDll.Patches;
using static Multiplayer.NetTransport;
using static Multiplayer.NetTransportSteam;



namespace YxModDll.Patches
{
    public class Patcher_NetTransportSteam : MonoBehaviour
    {

        private static FieldInfo _lobbyID;
        private static FieldInfo _lobbyStatus;
        private static FieldInfo _startHostCallback;
        private static FieldInfo _onListLobbies;

        private static FieldInfo __name;
        private static FieldInfo __lobbyId;


        //public static string fangjianmingzi = SteamFriends.GetPersonaName();
        //public static int xujiarenshu = 0;
        //public static bool dangqianrenshupaixu;
        //public static bool simifangjian;
        //public static bool danyexianshi;
        //public static bool haoyoufangjian;
        private void Awake()
        {
            //ShowMethod = typeof(NetChat).GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Instance);
            _lobbyID = typeof(NetTransportSteam).GetField("lobbyID", BindingFlags.Public | BindingFlags.Instance);
            _lobbyStatus = typeof(NetTransportSteam).GetField("lobbyStatus", BindingFlags.NonPublic | BindingFlags.Instance);
            _startHostCallback = typeof(NetTransportSteam).GetField("startHostCallback", BindingFlags.NonPublic | BindingFlags.Instance);
            _onListLobbies = typeof(NetTransportSteam).GetField("onListLobbies", BindingFlags.NonPublic | BindingFlags.Instance);

            __name = typeof(NetTransportSteam.FriendInfo).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
            __lobbyId = typeof(NetTransportSteam.FriendInfo).GetField("_lobbyId", BindingFlags.NonPublic | BindingFlags.Instance);


            Patcher2.MethodPatch(typeof(NetTransportSteam), "UpdateUsersLobbyData", new[] { typeof(int) }, typeof(Patcher_NetTransportSteam), "UpdateUsersLobbyData", new[] { typeof(NetTransportSteam), typeof(int) });
            Patcher2.MethodPatch(typeof(NetTransportSteam), "UpdateOptionsLobbyData", null, typeof(Patcher_NetTransportSteam), "UpdateOptionsLobbyData", new[] { typeof(NetTransportSteam) });
            Patcher2.MethodPatch(typeof(NetTransportSteam), "OnLobbyCreated", new[] { typeof(LobbyCreated_t) }, typeof(Patcher_NetTransportSteam), "OnLobbyCreated", new[] { typeof(NetTransportSteam), typeof(LobbyCreated_t) });
            Patcher2.MethodPatch(typeof(NetTransportSteam), "OnLobbyList", new[] { typeof(LobbyMatchList_t), typeof(bool) }, typeof(Patcher_NetTransportSteam), "OnLobbyList", new[] { typeof(NetTransportSteam), typeof(LobbyMatchList_t), typeof(bool) });
        }


        private void OnGUI()
        {
            //if (GUILayout.Button("开关"))
            //{

            //}
        }

        public static void OnLobbyList(NetTransportSteam instance, LobbyMatchList_t param, bool bIOFailure)
        {
            //Debug.Log("123");

            //NetTransportSteam instance = FindObjectOfType<NetTransportSteam>();
            var onListLobbies = (Action<List<ILobbyEntry>>)_onListLobbies.GetValue(instance);


            if (onListLobbies == null)
            {
                return;
            }
            //好友房
            List<ILobbyEntry> haoyoulist = new List<ILobbyEntry>();
            if (UI_SheZhi.danyexianshi)
            {
                //this.onListLobbies = null;
                CGameID cGameID = new CGameID(SteamUtils.GetAppID());

                int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
                for (int i = 0; i < friendCount; i++)
                {
                    CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagAll);
                    if (SteamFriends.GetFriendGamePlayed(friendByIndex, out var pFriendGameInfo) && !(pFriendGameInfo.m_gameID != cGameID) && pFriendGameInfo.m_steamIDLobby.IsValid())
                    {
                        NetTransportSteam.FriendInfo friendInfo = new FriendInfo();
                        friendInfo.steamId = friendByIndex;

                        var _lobbyId = (CSteamID)__lobbyId.GetValue(friendInfo);
                        _lobbyId = pFriendGameInfo.m_steamIDLobby;
                        __lobbyId.SetValue(friendInfo, _lobbyId);
                        //friendInfo._lobbyId = pFriendGameInfo.m_steamIDLobby;


                        var _name = (string)__name.GetValue(friendInfo);
                        _name = (UI_SheZhi.haoyoufangjian ? "@" : "") + SteamFriends.GetFriendPersonaName(friendByIndex);
                        __name.SetValue(friendInfo, _name);
                        //friendInfo._name = (haoyoufangjian ? "@" : "") + SteamFriends.GetFriendPersonaName(friendByIndex);
                        FriendInfo friend = friendInfo;
                        SteamMatchmaking.RequestLobbyData(pFriendGameInfo.m_steamIDLobby);
                        GetNewLobbyData(pFriendGameInfo.m_steamIDLobby, ref friend);
                        //if (!friend.inviteOnly)
                        //{
                        haoyoulist.Add(friend);
                        //}
                    }
                }
                if (UI_SheZhi.dangqianrenshupaixu)
                {
                    // 对yxmodlist进行排序
                    //haoyoulist.Sort((x, y) => ((FriendInfo)y).playersCurrent.CompareTo(((FriendInfo)x).playersCurrent));
                    // 手动实现降序排序
                    for (int i = 0; i < haoyoulist.Count - 1; i++)
                    {
                        for (int j = 0; j < haoyoulist.Count - i - 1; j++)
                        {
                            FriendInfo current = (FriendInfo)haoyoulist[j];
                            FriendInfo next = (FriendInfo)haoyoulist[j + 1];

                            if (current.playersCurrent < next.playersCurrent)
                            {
                                // 交换元素
                                var temp = haoyoulist[j];
                                haoyoulist[j] = haoyoulist[j + 1];
                                haoyoulist[j + 1] = temp;
                            }
                        }
                    }
                }


                //onListLobbies(list);
                //好友房
            }

            List<ILobbyEntry> list = new List<ILobbyEntry>();
            List<ILobbyEntry> similist = new List<ILobbyEntry>();///增加私密房间列表
            List<ILobbyEntry> yxmodlist = new List<ILobbyEntry>();///
            for (int i = 0; i < param.m_nLobbiesMatching; i++)
            {
                CSteamID lobbyByIndex = SteamMatchmaking.GetLobbyByIndex(i);
                uint result = 0u;
                uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyByIndex, "net"), out result);
                uint result2 = 0u;
                uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyByIndex, "io"), out result2);
                if (result == VersionDisplay.netCode)
                {
                    CSteamID lobbyOwner = SteamMatchmaking.GetLobbyOwner(lobbyByIndex);
                    string lobbyData = SteamMatchmaking.GetLobbyData(lobbyByIndex, "name");
                    string lobbyData2 = SteamMatchmaking.GetLobbyData(lobbyByIndex, "version");
                    string lobbyData3 = SteamMatchmaking.GetLobbyData(lobbyByIndex, "cp");
                    FriendInfo friendInfo = new FriendInfo();
                    friendInfo.steamId = lobbyOwner;

                    //friendInfo._lobbyId = lobbyByIndex;
                    //friendInfo._name = (UI_SheZhi.simifangjian ? (result2 == 1 ? "*" : "") : "") + lobbyData; ////修改
                    //friendInfo._name = lobbyData;
                    var _lobbyId = (CSteamID)__lobbyId.GetValue(friendInfo);
                    _lobbyId = lobbyByIndex;
                    __lobbyId.SetValue(friendInfo, _lobbyId);

                    var _name = (string)__name.GetValue(friendInfo);
                    _name = (UI_SheZhi.simifangjian ? (result2 == 1 ? "*" : "") : "") + lobbyData; ////修改
                    __name.SetValue(friendInfo, _name);

                    friendInfo.inviteOnly = result2 == 1;
                    friendInfo.version = lobbyData2;
                    friendInfo.netcode = result;
                    FriendInfo friend = friendInfo;
                    GetNewLobbyData(lobbyByIndex, ref friend);


                    //var _lobbyId = (CSteamID)__lobbyId.GetValue(friend);
                    var name = (string)__name.GetValue(friend);

                    if (name.StartsWith("★") || name.StartsWith("*★"))
                    {
                        yxmodlist.Add(friend);
                    }

                    else if (!friend.inviteOnly) ////正常房间
                    {
                        list.Add(friend);
                    }
                    else if (friend.inviteOnly)////仅限邀请房间
                    {
                        similist.Add(friend);
                    }
                }
            }
            if (UI_SheZhi.dangqianrenshupaixu)
            {
                // 对yxmodlist进行排序
                //yxmodlist.Sort((x, y) => ((FriendInfo)y).playersCurrent.CompareTo(((FriendInfo)x).playersCurrent));
                for (int i = 0; i < yxmodlist.Count - 1; i++)
                {
                    for (int j = 0; j < yxmodlist.Count - i - 1; j++)
                    {
                        FriendInfo current = (FriendInfo)yxmodlist[j];
                        FriendInfo next = (FriendInfo)yxmodlist[j + 1];

                        if (current.playersCurrent < next.playersCurrent)
                        {
                            // 交换元素
                            var temp = yxmodlist[j];
                            yxmodlist[j] = yxmodlist[j + 1];
                            yxmodlist[j + 1] = temp;
                        }
                    }
                }
                // 对list进行排序
                //list.Sort((x, y) => ((FriendInfo)y).playersCurrent.CompareTo(((FriendInfo)x).playersCurrent));
                for (int i = 0; i < list.Count - 1; i++)
                {
                    for (int j = 0; j < list.Count - i - 1; j++)
                    {
                        FriendInfo current = (FriendInfo)list[j];
                        FriendInfo next = (FriendInfo)list[j + 1];

                        if (current.playersCurrent < next.playersCurrent)
                        {
                            // 交换元素
                            var temp = list[j];
                            list[j] = list[j + 1];
                            list[j + 1] = temp;
                        }
                    }
                }
                // 对similist进行排序
                //similist.Sort((x, y) => ((FriendInfo)y).playersCurrent.CompareTo(((FriendInfo)x).playersCurrent));
                for (int i = 0; i < similist.Count - 1; i++)
                {
                    for (int j = 0; j < similist.Count - i - 1; j++)
                    {
                        FriendInfo current = (FriendInfo)similist[j];
                        FriendInfo next = (FriendInfo)similist[j + 1];

                        if (current.playersCurrent < next.playersCurrent)
                        {
                            // 交换元素
                            var temp = similist[j];
                            similist[j] = similist[j + 1];
                            similist[j + 1] = temp;
                        }
                    }
                }
            }

            List<ILobbyEntry> combinedList = new List<ILobbyEntry>();
            //yxmodlist = yxmodlist.OrderByDescending(friend => friend.playersCurrent).ToList();
            combinedList.AddRange(yxmodlist); // 添加yxmodlist中的所有元素到combinedList
            if (UI_SheZhi.danyexianshi)
            {
                combinedList.AddRange(haoyoulist); // 添加yxmodlist中的所有元素到combinedList
            }

            combinedList.AddRange(list);      // 添加list中的所有元素到combinedList
            if (UI_SheZhi.simifangjian)
            {
                combinedList.AddRange(similist); // 添加similist中的所有元素到combinedList
            }


            onListLobbies(combinedList);
            onListLobbies = null;
        }

        private static void GetNewLobbyData(CSteamID lobbyID, ref FriendInfo friend)
        {
            uint result = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "mp"), out result);
            uint result2 = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "cp"), out result2);
            ulong result3 = 0uL;
            ulong.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "li"), out result3);
            uint result4 = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "io"), out result4);
            WorkshopItemSource workshopItemSource = WorkshopItemSource.BuiltIn;
            uint result5 = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "lt"), out result5);
            workshopItemSource = (WorkshopItemSource)result5;
            uint result6 = 0u;
            uint.TryParse(SteamMatchmaking.GetLobbyData(lobbyID, "fl"), out result6);
            friend.playersMax = result;
            friend.playersCurrent = result2;
            friend.levelID = result3;
            friend.inviteOnly = result4 == 1;
            friend.levelType = workshopItemSource;
            friend.lobbyTitle = SteamMatchmaking.GetLobbyData(lobbyID, "ll");
            friend.flags = result6;
        }
        public static void OnLobbyCreated(NetTransportSteam instance, LobbyCreated_t param)
        {
            //NetTransportSteam instance = FindObjectOfType<NetTransportSteam>();
            var lobbyID = (CSteamID)_lobbyID.GetValue(instance);
            var startHostCallback = (OnStartHostDelegate)_startHostCallback.GetValue(instance);



            if (startHostCallback != null)
            {
                if (param.m_eResult == EResult.k_EResultOK)
                {
                    SavePreviousLobby(param.m_ulSteamIDLobby);
                    lobbyID = new CSteamID(param.m_ulSteamIDLobby);
                    _lobbyID.SetValue(instance, lobbyID);
                    //SteamMatchmaking.SetLobbyData(lobbyID, "name", SteamFriends.GetPersonaName());
                    SteamMatchmaking.SetLobbyData(lobbyID, "name", $"★{UI_SheZhi.fangming}");////修改房名
                    SteamMatchmaking.SetLobbyData(lobbyID, "ll", $"{UI_SheZhi.datingming}");
                    SteamMatchmaking.SetLobbyData(lobbyID, "version", VersionDisplay.fullVersion);
                    SteamMatchmaking.SetLobbyData(lobbyID, "net", VersionDisplay.netCode.ToString());
                    UpdateOptionsLobbyData(instance);
                    //SteamMatchmaking.SetLobbyData(lobbyID, "cp", "0");
                    SteamMatchmaking.SetLobbyData(lobbyID, "cp", $"{UI_SheZhi.xujiarishu}"); //UI_SheZhi.xujiarishu
                    SteamMatchmaking.SetLobbyData(lobbyID, "li", NetGame.instance.currentLevel.ToString());
                    CSteamID steamIDLobby = lobbyID;
                    uint currentLevelType = (uint)NetGame.instance.currentLevelType;
                    SteamMatchmaking.SetLobbyData(steamIDLobby, "lt", currentLevelType.ToString());
                    // SteamMatchmaking.SetLobbyData(lobbyID, "ll", NetGame.instance.lobbyTitle);
                    startHostCallback(null);
                    //_startHostCallback.Invoke(instance, null);
                    //_startHostCallback(instance,null);
                }
                else
                {
                    startHostCallback(param.m_eResult.ToString());

                }
                startHostCallback = null;
                _startHostCallback.SetValue(instance, null);
            }
        }

        private static void SavePreviousLobby(ulong lobbyID)
        {
            PlayerPrefs.SetString("LastLobby", lobbyID.ToString());
        }
        public static void UpdateUsersLobbyData(NetTransportSteam instance, int userCount)
        {
            //NetTransportSteam instance = (NetTransportSteam)NetGame.instance.transport;//FindObjectOfType<NetTransportSteam>();
            //NetTransportSteam instance = FindObjectOfType<NetTransportSteam>();
            var lobbyID = (CSteamID)_lobbyID.GetValue(instance);

            SteamMatchmaking.SetLobbyData(lobbyID, "cp", (UI_SheZhi.xujiarishu + userCount).ToString());//userCount.ToString()  ///虚假房间人数
            SteamMatchmaking.SetLobbyData(lobbyID, "name", $"★{UI_SheZhi.fangming}");///修改房名
            SteamMatchmaking.SetLobbyData(lobbyID, "ll", $"{UI_SheZhi.datingming}");
            SteamMatchmaking.SetLobbyData(lobbyID, "mp", (UI_SheZhi.xujiarishu + Options.lobbyMaxPlayers).ToString());///虚假最大人数
        }

        public static void UpdateOptionsLobbyData(NetTransportSteam instance)
        {
            //NetTransportSteam instance = FindObjectOfType<NetTransportSteam>();
            var lobbyID = (CSteamID)_lobbyID.GetValue(instance);

            int num = BuildLobbyFlagsFromOptions();
            if ((object)lobbyID != null && lobbyID.IsValid())
            {
                //SteamMatchmaking.SetLobbyData(lobbyID, "mp", Options.lobbyMaxPlayers.ToString());
                SteamMatchmaking.SetLobbyData(lobbyID, "mp", (UI_SheZhi.xujiarishu + Options.lobbyMaxPlayers).ToString());
                SteamMatchmaking.SetLobbyData(lobbyID, "fl", num.ToString());
                SteamMatchmaking.SetLobbyData(lobbyID, "io", Options.lobbyInviteOnly.ToString());
            }
        }
        private static int BuildLobbyFlagsFromOptions()
        {
            var lobbyStatus = (bool)_lobbyStatus.GetValue(NetGame.instance.transport);
            int num = 0;
            if (Options.lobbyLockLevel > 0)
            {
                num |= 2;
            }
            if (Options.lobbyJoinInProgress > 0)
            {
                num |= 1;
            }
            if (Options.lobbyInviteOnly > 0)
            {
                num |= 4;
            }
            if (lobbyStatus)
            {
                num |= 8;
            }
            return num;
        }
    }
}

