using Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using YxModDll.Patches;

namespace YxModDll.Mod.HumanAnimator
{
    internal class PlayAnimator : MonoBehaviour
    {
        private static string path = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Animator");
        public static Dictionary<DONGZUO_State, List<BoneFrameData[]>> cache = new();

        // 骨骼名称映射
        private static readonly Dictionary<string, BoneId> BoneNameToId = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Hips", BoneId.Hips },
            { "Waist", BoneId.Waist },
            { "Chest", BoneId.Chest },
            { "Head", BoneId.Head },

            { "LeftArm", BoneId.LeftArm },
            { "LeftForearm", BoneId.LeftForearm },
            { "LeftHand", BoneId.LeftHand },

            { "RightArm", BoneId.RightArm },
            { "RightForearm", BoneId.RightForearm },
            { "RightHand", BoneId.RightHand },

            { "LeftThigh", BoneId.LeftThigh },
            { "LeftLeg", BoneId.LeftLeg },
            { "LeftFoot", BoneId.LeftFoot },

            { "RightThigh", BoneId.RightThigh },
            { "RightLeg", BoneId.RightLeg },
            { "RightFoot", BoneId.RightFoot },
        };

        // 从二进制文件加载动作
        public bool LoadFromBinary(DONGZUO_State state)
        {
            string dhpath = GetPath(state);
            if (!File.Exists(dhpath))
                return false;

            var frames = new List<BoneFrameData[]>();
            using (BinaryReader reader = new(File.Open(dhpath, FileMode.Open)))
            {
                Dictionary<int, BoneFrameData[]> frameDict = new();

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    try
                    {
                        int frameIndex = reader.ReadInt32();
                        string boneName = reader.ReadString();

                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();
                        float qx = reader.ReadSingle();
                        float qy = reader.ReadSingle();
                        float qz = reader.ReadSingle();
                        float qw = reader.ReadSingle();

                        if (!BoneNameToId.TryGetValue(boneName, out BoneId boneId))
                            continue; // 未知骨骼，直接丢弃

                        if (!frameDict.TryGetValue(frameIndex, out var arr))
                        {
                            arr = new BoneFrameData[(int)BoneId.Count];
                            frameDict[frameIndex] = arr;
                        }

                        arr[(int)boneId] = new BoneFrameData
                        {
                            Bone = boneId,
                            Position = new Vector3(x, y, z),
                            Rotation = new Quaternion(qx, qy, qz, qw)
                        };
                    }
                    catch { break; }
                }

                // 按帧顺序存到 List
                foreach (var kv in frameDict)
                    frames.Add(kv.Value);

                frames.Sort((a, b) => Array.IndexOf(frames.ToArray(), a) - Array.IndexOf(frames.ToArray(), b));
            }

            cache[state] = frames;
            return true;
        }

        private void Start()
        {
            // 缓存所有动作
            foreach (DONGZUO_State stateEnum in Enum.GetValues(typeof(DONGZUO_State)))
            {
                if (!cache.ContainsKey(stateEnum))
                {
                    StartCoroutine(PreloadState(stateEnum));
                }
            }
        }
        // 预缓存每个动作
        private IEnumerator PreloadState(DONGZUO_State stateEnum)
        {
            string dzpath=  GetPath(stateEnum);
            if (File.Exists(dzpath))
            {
                LoadFromBinary(stateEnum);
            }
            else
            {
                string filename = stateEnum.ToString().ToLower();
                if (_fileDownloadUrls.TryGetValue(filename, out string url))
                {
                    Debug.Log($"📥 动画 {filename} 不存在，开始下载...");
                    yield return DownloadFile(url, dzpath);

                    if (File.Exists(dzpath))
                    {
                        LoadFromBinary(stateEnum);
                        Debug.Log($"✅ {filename} 下载并缓存完成");
                    }
                }
                else
                {
                    Debug.LogWarning($"⚠ 没有 {filename} 的下载地址");
                }
            }
        }

        private static IEnumerator DownloadFile(string url, string savePath)
        {

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();

                bool isError = false;
#if UNITY_2020_1_OR_NEWER
                isError = webRequest.result != UnityWebRequest.Result.Success;
#else
                isError = webRequest.isNetworkError || webRequest.isHttpError;
#endif

                if (isError)
                {
                    Debug.LogError($"❌ 下载失败：{webRequest.error}（URL：{url}）");
                    yield break;
                }

                try
                {
                    string dir = Path.GetDirectoryName(savePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    File.WriteAllBytes(savePath, webRequest.downloadHandler.data);
                    Debug.Log($"📥 文件下载成功（大小：{webRequest.downloadHandler.data.Length}字节）");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"❌ 保存文件失败：{ex.Message}（路径：{savePath}）");
                }
            }
        }

        private static readonly Dictionary<string, string> _fileDownloadUrls = new Dictionary<string, string>
        {
            { "tuomasi", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156013806/TuoMaSi.txt" },
            { "piliwudongjie", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156009662/PiLiWuDongJie.txt" },
            { "yaobaiwu", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156016437/YaoBaiWu.txt" },
            { "yangwoqizuo", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156015975/YangWoQiZuo.txt" },
            { "xihawu3", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156015404/XiHaWu3.txt" },
            { "xihawu2", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156014785/XiHaWu2.txt" },
            { "xihawu", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156014282/XiHaWu.txt" },
            { "touxuan", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156013450/TouXuan.txt" },
            { "sangbawu2", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156013028/SangBaWu2.txt" },
            { "sangbawu", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156012205/SangBaWu.txt" },
            { "quanji", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156011309/QuanJi.txt" },
            { "qimawu", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156010643/QiMaWu.txt" },
            { "mumati", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156008759/MuMaTi.txt" },
            { "kaihetiao", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156008235/KaiHeTiao.txt" },
            { "jiaochatiaoyue", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156007705/JiaoChaTiaoYue.txt" },
            { "heiyingtaowubu", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156007145/HeiYingTaoWuBu.txt" },
            { "fuwocheng", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156006696/FuWoCheng.txt" },
            { "diantunwu", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757156006256/DianTunWu.txt" },
            { "manpao", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1757165013211/ManPao.txt" }


        };
        private static string GetPath(DONGZUO_State type)
        {
            return Path.Combine(path, type.ToString());
        }

        public void Update()
        {
            if (NetGame.isServer)
            {
                foreach (Human human in Human.all)
                {
                    if (human.controls.unconscious)
                    {
                        if (human.GetExt().ntp)
                            continue;

                        if (!UI_GongNeng.Y_KaiGuan)
                        {
                            human.GetExt().numY = 0;
                            if (human.GetExt().bofangdonghua)
                            {
                                human.GetExt().bofangdonghua = false;
                            }
                        }
                        switch (human.GetExt().numY)
                        {
                            case 10:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.TuoMaSi);
                                }
                                break;
                            case 11:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.PiLiWuDongJie);
                                }
                                break;
                            case 12:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.JiaoChaTiaoYue);
                                }
                                break;
                            case 13:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.YangWoQiZuo);
                                }
                                break;
                            case 14:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.FuWoCheng);
                                }
                                break;
                            case 15:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.XiHaWu);
                                }
                                break;
                            case 16:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.XiHaWu2);
                                }
                                break;
                            case 17:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.XiHaWu3);
                                }
                                break;
                            case 18:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.TouXuan);
                                }
                                break;
                            case 19:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.MuMaTi);
                                }
                                break;
                            case 20:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.KaiHeTiao);
                                }
                                break;
                            case 21:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.YaoBaiWu);
                                }
                                break;
                            case 22:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.SangBaWu);
                                }
                                break;
                            case 23:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.SangBaWu2);
                                }
                                break;
                            case 24:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.DianTunWu);
                                }
                                break;
                            case 25:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.QuanJi);
                                }
                                break;
                            case 26:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.QiMaWu);
                                }
                                break;
                            case 27:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.HeiYingTaoWuBu);
                                }
                                break;
                            case 28:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.ManPao,1.5f);
                                }
                                break;
                        }
                    }
                    else 
                    {
                        if (human.GetExt().bofangdonghua)
                        {
                            human.GetExt().bofangdonghua = false;
                        }
                    }
                }
            }



        }
        public void PlayAnimationFromFile(Human human, DONGZUO_State type, float speed = 1.0f, bool loop = true)
        {
            if (human == null)
            {
                Debug.LogError("Human 为空！");
                return;
            }


            // ✅ 动态添加组件
            SmoothAnimator animator = human.gameObject.GetComponent<SmoothAnimator>();
            if (animator == null)
            {
                animator = human.gameObject.AddComponent<SmoothAnimator>();

            }
            animator.human = human;
            animator.playbackSpeed = speed;
            animator.loop = loop;
            animator.state = type;


        }

    }
}
