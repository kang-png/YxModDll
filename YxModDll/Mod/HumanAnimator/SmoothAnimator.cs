using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using YxModDll.Patches;

namespace YxModDll.Mod.HumanAnimator
{
    public class SmoothAnimator : MonoBehaviour
    {
        private static string path = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Animator");
        public Human human;

        private DONGZUO_State _state;
        public DONGZUO_State state
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;

                dongzuopath = GetPath(_state);
                if (enabled)
                {
                    if (LoadFromBinary(dongzuopath))
                    {
                        Play();
                        isPlaying = true;
                    }
                }
            }
        }
        private string dongzuopath;

        public float playbackSpeed = 1.0f;
        public bool loop = false;

        private List<BoneFrameData> frames = new List<BoneFrameData>();
        private Dictionary<string, Rigidbody> boneRigidbodies = new Dictionary<string, Rigidbody>();

        private float playbackTime = 0f;
        private float frameInterval = 1f / 60f; 
        private int currentFrameIndex = 0;
        private bool _isPlaying;
        public bool isPlaying
        {
            get { return _isPlaying; }
            set
            {
                if (!_isPlaying && value)
                {
                    Play();
                }
                if (_isPlaying && !value)
                {
                    Stop();
                }
                _isPlaying = value;

            }
        }

        [Serializable]
        private struct BoneFrameData
        {
            public int Frame;
            public string BoneName;
            public Vector3 Position;
            public Quaternion Rotation;
        }

        private void Start()
        {
            //缓存所有动作
            string[] allStateNames = Enum.GetNames(typeof(DONGZUO_State));
            foreach (string name in allStateNames)
            {
                if (Enum.TryParse(name, out DONGZUO_State stateEnum))
                {
                    string stmpath = GetPath(stateEnum);
                    
                    if (!cache.ContainsKey(stateEnum))
                    {
                        StartCoroutine(PreloadState(stateEnum, stmpath));
                    }
                }
            }

            if (human == null)
            {
                Debug.LogError("没有绑定 Human，脚本停用");
                enabled = false;
                return;
            }


            // 初始化刚体映射
            var ragdoll = human.ragdoll;
            boneRigidbodies["Hips"] = ragdoll.partHips.rigidbody;
            boneRigidbodies["Waist"] = ragdoll.partWaist.rigidbody;
            boneRigidbodies["Chest"] = ragdoll.partChest.rigidbody;
            boneRigidbodies["Head"] = ragdoll.partHead.rigidbody;

            boneRigidbodies["LeftArm"] = ragdoll.partLeftArm.rigidbody;
            boneRigidbodies["LeftForearm"] = ragdoll.partLeftForearm.rigidbody;
            boneRigidbodies["LeftHand"] = ragdoll.partLeftHand.rigidbody;

            boneRigidbodies["RightArm"] = ragdoll.partRightArm.rigidbody;
            boneRigidbodies["RightForearm"] = ragdoll.partRightForearm.rigidbody;
            boneRigidbodies["RightHand"] = ragdoll.partRightHand.rigidbody;

            boneRigidbodies["LeftThigh"] = ragdoll.partLeftThigh.rigidbody;
            boneRigidbodies["LeftLeg"] = ragdoll.partLeftLeg.rigidbody;
            boneRigidbodies["LeftFoot"] = ragdoll.partLeftFoot.rigidbody;

            boneRigidbodies["RightThigh"] = ragdoll.partRightThigh.rigidbody;
            boneRigidbodies["RightLeg"] = ragdoll.partRightLeg.rigidbody;
            boneRigidbodies["RightFoot"] = ragdoll.partRightFoot.rigidbody;

        }
        private Vector3 JiZhunPos =new Vector3();
        private Quaternion JiZhunRot=new Quaternion();
        public void Play()
        {

            // 基准高度调整
            float gaodu = GetStateHeightOffset(state);

            JiZhunPos = human.transform.position + new Vector3(0, gaodu, 0) ;
            JiZhunRot = human.ragdoll.partHead.transform.rotation;

            foreach (var rb in human.rigidbodies)
            {
                rb.isKinematic = true;
            }
            playbackTime = 0f;
            currentFrameIndex = 0;

        }
        private float GetStateHeightOffset(DONGZUO_State s)
        {
            switch (s)
            {
                case DONGZUO_State.TuoMaSi: return -0.1f;
                case DONGZUO_State.PiLiWuDongJie: return -0.2f;
                case DONGZUO_State.JiaoChaTiaoYue: return -0.3f;
                case DONGZUO_State.YangWoQiZuo: return -0.2f;
                case DONGZUO_State.FuWoCheng: return -0.1f;
                case DONGZUO_State.XiHaWu: return -0.3f;
                case DONGZUO_State.XiHaWu2: return -0.3f;
                case DONGZUO_State.XiHaWu3: return -0.2f;
                case DONGZUO_State.TouXuan: return -0.1f;
                case DONGZUO_State.MuMaTi: return -0.2f;
                case DONGZUO_State.KaiHeTiao: return -0.2f;
                case DONGZUO_State.YaoBaiWu: return -0.2f;
                case DONGZUO_State.SangBaWu: return -0.2f;
                case DONGZUO_State.SangBaWu2: return -0.2f;
                case DONGZUO_State.DianTunWu: return -0.2f;
                case DONGZUO_State.QuanJi: return -0.2f;
                case DONGZUO_State.QiMaWu: return -0.2f;
                case DONGZUO_State.HeiYingTaoWuBu: return -0.3f;
                case DONGZUO_State.ManPao: return -0.2f;


                default: return 0f;
            }
        }
        private void Stop()
        {
            foreach (var rb in human.rigidbodies)
            {
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        
        private static Dictionary<DONGZUO_State, List<BoneFrameData>> cache
        = new Dictionary<DONGZUO_State, List<BoneFrameData>>();
        public bool LoadFromBinary(string path)
        {
            if (cache.TryGetValue(state, out var cachedFrames))
            {
                frames = cachedFrames;
            }
            else
            {
                var newFrames = new List<BoneFrameData>();
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        try
                        {
                            BoneFrameData frame = new BoneFrameData();
                            frame.Frame = reader.ReadInt32();
                            frame.BoneName = reader.ReadString();
                            frame.Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            frame.Rotation = new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            newFrames.Add(frame);
                        }
                        catch { break; }
                    }
                }
                frames = newFrames;
                cache[state] = newFrames; 
            }

            if (frames.Count == 0) return false;

            groupedFrames = frames.GroupBy(f => f.Frame).OrderBy(g => g.Key).ToList();
            if (groupedFrames.Count > 1)
            {
                float totalDuration = (groupedFrames.Last().Key - groupedFrames.First().Key) * Time.fixedDeltaTime;
                frameInterval = totalDuration / (groupedFrames.Count - 1);
            }
            else
            {
                frameInterval = Time.fixedDeltaTime; 
            }

            return true;
        }
        private static string GetPath(DONGZUO_State type)
        {
            return Path.Combine(path, type.ToString());
        }
        // 预缓存每个动作
        private IEnumerator PreloadState(DONGZUO_State stateEnum, string path)
        {
            if (File.Exists(path))
            {
                LoadFromBinary(path); 
            }
            else
            {
                string filename = stateEnum.ToString().ToLower();
                if (_fileDownloadUrls.TryGetValue(filename, out string url))
                {
                    Debug.Log($"📥 动画 {filename} 不存在，开始下载...");
                    yield return DownloadFile(url, path);

                    if (File.Exists(path))
                    {
                        LoadFromBinary(path);
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


        private List<IGrouping<int, BoneFrameData>> groupedFrames;
        private void FixedUpdate()
        {
            isPlaying = human.GetExt().bofangdonghua;

            if (!isPlaying || frames.Count == 0) return;

            playbackTime += Time.fixedDeltaTime * playbackSpeed;

            int frameCount = groupedFrames.Count;

            float frameTime = playbackTime / frameInterval;
            int prevIndex = Mathf.FloorToInt(frameTime);
            int nextIndex = prevIndex + 1;
            float t = frameTime - prevIndex;

            if (prevIndex >= frameCount)
            {
                if (loop)
                {
                    playbackTime = 0f;
                    currentFrameIndex = 0;
                }
                else
                {
                    Stop();
                    isPlaying = false;
                }
                return;
            }

            var prevFrameGroup = groupedFrames[prevIndex];
            var nextFrameGroup = nextIndex < frameCount ? groupedFrames[nextIndex] : prevFrameGroup;

            var prevDict = prevFrameGroup.ToDictionary(f => f.BoneName, f => f);
            var nextDict = nextFrameGroup.ToDictionary(f => f.BoneName, f => f);

            foreach (var rbKvp in boneRigidbodies)
            {
                string boneName = rbKvp.Key;
                Rigidbody rb = rbKvp.Value;

                if (prevDict.TryGetValue(boneName, out var prev) && nextDict.TryGetValue(boneName, out var next))
                {
                    Vector3 localPos = Vector3.Lerp(prev.Position, next.Position, t);
                    Quaternion localRot = Quaternion.Slerp(prev.Rotation, next.Rotation, t);


                    Vector3 worldPos = JiZhunPos + JiZhunRot * localPos;
                    Quaternion worldRot = JiZhunRot * localRot;

                    rb.MovePosition(worldPos);
                    rb.MoveRotation(worldRot);

                }
                else if (prevDict.TryGetValue(boneName, out var onlyPrev))
                {

                    Vector3 worldPos = JiZhunPos + JiZhunRot * onlyPrev.Position;
                    Quaternion worldRot = JiZhunRot * onlyPrev.Rotation;

                    rb.MovePosition(worldPos);
                    rb.MoveRotation(worldRot);
                }

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        private BoneFrameData? ParseLine(string line)
        {
            try
            {
                string[] parts = line.Split(',');
                if (parts.Length != 9) return null;

                int frame = int.Parse(parts[0]);
                string boneName = parts[1];

                Vector3 pos = new Vector3(float.Parse(parts[2]), float.Parse(parts[3]), float.Parse(parts[4]));
                Quaternion rot = new Quaternion(float.Parse(parts[5]), float.Parse(parts[6]), float.Parse(parts[7]), float.Parse(parts[8]));

                return new BoneFrameData { Frame = frame, BoneName = boneName, Position = pos, Rotation = rot };
            }
            catch
            {
                return null;
            }
        }
    }
}
