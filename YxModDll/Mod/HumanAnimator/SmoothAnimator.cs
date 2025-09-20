using System.Collections.Generic;
using UnityEngine;
using YxModDll.Patches;

namespace YxModDll.Mod.HumanAnimator
{
    public class SmoothAnimator : MonoBehaviour
    {
        public Human human;

        private List<BoneFrameData[]> _frameBoneArray;

        private float gaodu;
       
        public float playbackSpeed = 1.0f;
        public bool loop = false;

        private Vector3 JiZhunPos = new Vector3();
        private Quaternion JiZhunRot = new Quaternion();
        private Rigidbody[] boneRigidbodies = new Rigidbody[(int)BoneId.Count];
        private float playbackTime = 0f;

        // 总帧数
        private int _totalFrameCount;
        // 每帧的时间间隔
        private float _frameInterval;

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

                if (PlayAnimator.cache.TryGetValue(state, out var frames) && frames.Count > 0)
                {

                    _frameBoneArray = frames;
                    _totalFrameCount = frames.Count;
                    _frameInterval = Time.fixedDeltaTime;

                    gaodu = GetStateHeightOffset(state);
                    //Play();
                    isPlaying = true;
                }
                else
                {

                    isPlaying = false;
                    Debug.LogWarning($"动画 {state} 无数据，无法播放");
                }
            }
        }
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
        private void Start()
        {
            if (human == null)
            {
                Debug.LogError("没有绑定 Human，脚本停用");
                enabled = false;
                return;
            }
            var ragdoll = human.ragdoll;
            boneRigidbodies[(int)BoneId.Hips] = ragdoll.partHips.rigidbody;
            boneRigidbodies[(int)BoneId.Waist] = ragdoll.partWaist.rigidbody;
            boneRigidbodies[(int)BoneId.Chest] = ragdoll.partChest.rigidbody;
            boneRigidbodies[(int)BoneId.Head] = ragdoll.partHead.rigidbody;

            boneRigidbodies[(int)BoneId.LeftArm] = ragdoll.partLeftArm.rigidbody;
            boneRigidbodies[(int)BoneId.LeftForearm] = ragdoll.partLeftForearm.rigidbody;
            boneRigidbodies[(int)BoneId.LeftHand] = ragdoll.partLeftHand.rigidbody;

            boneRigidbodies[(int)BoneId.RightArm] = ragdoll.partRightArm.rigidbody;
            boneRigidbodies[(int)BoneId.RightForearm] = ragdoll.partRightForearm.rigidbody;
            boneRigidbodies[(int)BoneId.RightHand] = ragdoll.partRightHand.rigidbody;

            boneRigidbodies[(int)BoneId.LeftThigh] = ragdoll.partLeftThigh.rigidbody;
            boneRigidbodies[(int)BoneId.LeftLeg] = ragdoll.partLeftLeg.rigidbody;
            boneRigidbodies[(int)BoneId.LeftFoot] = ragdoll.partLeftFoot.rigidbody;

            boneRigidbodies[(int)BoneId.RightThigh] = ragdoll.partRightThigh.rigidbody;
            boneRigidbodies[(int)BoneId.RightLeg] = ragdoll.partRightLeg.rigidbody;
            boneRigidbodies[(int)BoneId.RightFoot] = ragdoll.partRightFoot.rigidbody;
        }

        private Vector3[] now_rigi_positions ;
        private Quaternion[] now_rigi_rotations ;

        public void Play()
        {
            JiZhunPos = human.transform.position + new Vector3(0, gaodu, 0) ;
            JiZhunRot = human.ragdoll.partHead.transform.rotation;


            now_rigi_positions = new Vector3[human.rigidbodies.Length];
            now_rigi_rotations = new Quaternion[human.rigidbodies.Length];
            
            for (int i = 0; i < human.rigidbodies.Length; i++)
            {
                now_rigi_positions[i] = human.rigidbodies[i].transform.position;
                now_rigi_rotations[i] = human.rigidbodies[i].transform.rotation;
                human.rigidbodies[i].isKinematic = true;
            }

            playbackTime = 0f;
        }
        private void Stop()
        {

            for (int i = 0; i < human.rigidbodies.Length; i++)
            {
                Rigidbody rb = human.rigidbodies[i];
                rb.position = now_rigi_positions[i];
                rb.rotation = now_rigi_rotations[i];
                
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                rb.isKinematic = false;
            }
            now_rigi_positions = null;
            now_rigi_rotations = null;
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

        private void FixedUpdate()
        {
            isPlaying = human.GetExt().bofangdonghua;
            if (!isPlaying || _frameBoneArray == null || _totalFrameCount == 0)  return;
            playbackTime += Time.fixedDeltaTime * playbackSpeed;
            float frameTime = playbackTime / _frameInterval;
            int prevIndex = Mathf.FloorToInt(frameTime);
            if (prevIndex >= _totalFrameCount)
            {
                if (!loop)
                {
                    isPlaying=false;
                    return;
                }
                playbackTime = 0f;
                prevIndex = 0;
            }
            int nextIndex = prevIndex + 1 < _totalFrameCount ? prevIndex + 1 : prevIndex;
            float t = frameTime - prevIndex;
            var prevFrame = _frameBoneArray[prevIndex];
            var nextFrame = _frameBoneArray[nextIndex];
            for (int i = 0; i < (int)BoneId.Count; i++)
            {
                Rigidbody rb = boneRigidbodies[i];
                if (rb == null) continue;
                var prevData = prevFrame[i];
                var nextData = nextFrame[i];
                Vector3 localPos = Vector3.Lerp(prevData.Position, nextData.Position, t);
                Quaternion localRot = Quaternion.Lerp(prevData.Rotation, nextData.Rotation, t);
                Vector3 worldPos = JiZhunPos + JiZhunRot * localPos;
                Quaternion worldRot = JiZhunRot * localRot;
                //rb.MovePosition(worldPos);
                //rb.MoveRotation(worldRot);
                rb.transform.SetPositionAndRotation(worldPos, worldRot);

            }
            
        }
    }
}
