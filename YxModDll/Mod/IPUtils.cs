using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace YxModDll.Mod
{
    internal class IPUtils : MonoBehaviour
    {
        // 同步获取IP地址（Steam API是同步的）
        public static string TryGetPlayerIPAddress(CSteamID steamID)
        {
            P2PSessionState_t sessionState;
            if (!SteamNetworking.GetP2PSessionState(steamID, out sessionState))
                return null;

            uint ip = sessionState.m_nRemoteIP;
            if (ip == 0)
                return null;

            return ConvertUIntToIP(ip);
        }

        // 协程异步获取省份
        public IEnumerator GetIPLocationAsync(string ip, Action<string> onResult)
        {
            if (string.IsNullOrEmpty(ip))
            {
                onResult?.Invoke(null);
                yield break;
            }

            string url = $"https://api.vore.top/api/IPdata?ip={ip}";
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogWarning($"[IP DEBUG] 请求省份失败: {request.error}");
                    onResult?.Invoke(null);
                    yield break;
                }

                string json = request.downloadHandler.text;
                var ipInfo = JsonUtility.FromJson<IPApiResponse>(json);
                string province = ipInfo?.adcode?.p;
                onResult?.Invoke(province);
            }
        }

        // 批量获取玩家列表字符串（格式：1.玩家名[省份]、2.玩家名[省份]、...）
        public IEnumerator GetPlayerListStringAsync(Action<string> onComplete)
        {
            List<string> entries = new List<string>();
            int index = 1;

            foreach (var human in Human.all)
            {
                string playerName = human.player.host.name;

                ulong ulSteamID = 0UL;
                ulong.TryParse(human.player.skinUserId, out ulSteamID);
                string ip = TryGetPlayerIPAddress(new CSteamID(ulSteamID));

                string province = null;

                // 等待异步获取省份结果
                yield return GetIPLocationAsync(ip, result => province = result);

                if (string.IsNullOrEmpty(province))
                    province = "未知";

                entries.Add($"{index}.{playerName}[{province}]");
                index++;
            }

            string resultStr = string.Join("、", entries);
            onComplete?.Invoke(resultStr);
        }
        public static string ConvertUIntToIP(uint ip)
        {
            byte[] bytes = BitConverter.GetBytes(ip);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return new IPAddress(bytes).ToString();
        }
        [Serializable]
        public class IPApiResponse
        {
            public Adcode adcode;
        }

        [Serializable]
        public class Adcode
        {
            public string p;
        }
    }
}
