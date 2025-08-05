using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace YxModDll.Mod
{
    public class RagdollAnimatorSync : MonoBehaviour
    {
        public Animator sourceAnimator;
        public Human human;
        public bool useRigidbodySync = true; // 是否使用 Rigidbody.MovePosition
        public bool makeKinematic = true;    // 是否自动将 ragdoll 变为 kinematic


        // 存储源骨骼相对于目标骨骼的初始位置偏移（目标 = 源 + 偏移）
        private Dictionary<HumanBodyBones, Vector3> initialPosOffsets = new Dictionary<HumanBodyBones, Vector3>();
        // 存储源骨骼相对于目标骨骼的初始旋转偏移（目标 = 源 * 偏移）
        private Dictionary<HumanBodyBones, Quaternion> initialRotOffsets = new Dictionary<HumanBodyBones, Quaternion>();

        private Dictionary<HumanBodyBones, Transform> sourceBones = new Dictionary<HumanBodyBones, Transform>();
        private Dictionary<HumanBodyBones, Transform> targetBones = new Dictionary<HumanBodyBones, Transform>();
        private Dictionary<HumanBodyBones, Rigidbody> targetRigidbodies = new Dictionary<HumanBodyBones, Rigidbody>();

        private readonly HumanBodyBones[] boneList = new HumanBodyBones[]
        {
        HumanBodyBones.Hips,
        HumanBodyBones.Spine,
        HumanBodyBones.Chest,
        HumanBodyBones.Head,
        HumanBodyBones.LeftUpperArm,
        HumanBodyBones.LeftLowerArm,
        HumanBodyBones.LeftHand,
        HumanBodyBones.RightUpperArm,
        HumanBodyBones.RightLowerArm,
        HumanBodyBones.RightHand,
        HumanBodyBones.LeftUpperLeg,
        HumanBodyBones.LeftLowerLeg,
        HumanBodyBones.LeftFoot,
        HumanBodyBones.RightUpperLeg,
        HumanBodyBones.RightLowerLeg,
        HumanBodyBones.RightFoot
        };

        public void Initialize(Animator source, Human targetHuman)
        {
            //StopSync(); // 先停止旧的

            sourceAnimator = source;
            human = targetHuman;
            //enabled = true;

            if (sourceAnimator == null || !sourceAnimator.isHuman || human == null)
            {
                Debug.LogError("初始化失败：Animator 或 Human 无效！");
                enabled = false;
                return;
            }

            // 获取源骨骼
            foreach (var bone in boneList)
            {
                var src = sourceAnimator.GetBoneTransform(bone);
                if (src != null)
                {
                    sourceBones[bone] = src;
                }
            }

            // 映射 Human ragdoll 的骨骼 + 刚体
            targetBones[HumanBodyBones.Hips] = human.ragdoll.partHips.transform;
            targetBones[HumanBodyBones.Spine] = human.ragdoll.partWaist.transform;
            targetBones[HumanBodyBones.Chest] = human.ragdoll.partChest.transform;
            targetBones[HumanBodyBones.Head] = human.ragdoll.partHead.transform;

            targetBones[HumanBodyBones.LeftUpperArm] = human.ragdoll.partLeftArm.transform;
            targetBones[HumanBodyBones.LeftLowerArm] = human.ragdoll.partLeftForearm.transform;
            targetBones[HumanBodyBones.LeftHand] = human.ragdoll.partLeftHand.transform;

            targetBones[HumanBodyBones.RightUpperArm] = human.ragdoll.partRightArm.transform;
            targetBones[HumanBodyBones.RightLowerArm] = human.ragdoll.partRightForearm.transform;
            targetBones[HumanBodyBones.RightHand] = human.ragdoll.partRightHand.transform;

            targetBones[HumanBodyBones.LeftUpperLeg] = human.ragdoll.partLeftThigh.transform;
            targetBones[HumanBodyBones.LeftLowerLeg] = human.ragdoll.partLeftLeg.transform;
            targetBones[HumanBodyBones.LeftFoot] = human.ragdoll.partLeftFoot.transform;

            targetBones[HumanBodyBones.RightUpperLeg] = human.ragdoll.partRightThigh.transform;
            targetBones[HumanBodyBones.RightLowerLeg] = human.ragdoll.partRightLeg.transform;
            targetBones[HumanBodyBones.RightFoot] = human.ragdoll.partRightFoot.transform;

            foreach (var kvp in targetBones)
            {
                var rb = kvp.Value.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    targetRigidbodies[kvp.Key] = rb;

                    if (makeKinematic)
                        rb.isKinematic = true;
                }
            }

            // 计算初始偏移（核心校准步骤）
            foreach (var bone in boneList)
            {
                if (sourceBones.TryGetValue(bone, out var src) && targetBones.TryGetValue(bone, out var tgt))
                {
                    // 初始位置偏移 = 目标初始位置 - 源初始位置（目标 = 源 + 偏移）
                    initialPosOffsets[bone] = tgt.position - src.position;
                    // 初始旋转偏移 = 目标初始旋转 * 源初始旋转的逆（目标 = 源 * 偏移）
                    initialRotOffsets[bone] = tgt.rotation * Quaternion.Inverse(src.rotation);
                }
            }


            //Debug.Log("✅ 骨骼同步初始化完成。");
            //NetChat.Print("✅ 骨骼同步初始化完成。");
        }
        private int ii = 0;

        void FixedUpdate()
        {
            if (sourceAnimator == null || sourceBones.Count == 0 || targetBones.Count == 0)
                return;
            ii++;
            //Debug.Log($"第 {ii} 帧");
            foreach (var bone in sourceBones.Keys)
            {
                if (!targetBones.ContainsKey(bone)) continue;

                var src = sourceBones[bone];
                var tgt = targetBones[bone];

                // ✅ 打印源骨骼的位置和旋转
                //NetChat.Print($"[源] {bone}: Position = {src.position}, Rotation = {src.rotation}");
                //Debug.Log($"[动作数据] {bone}: Position = {src.position}, Rotation = {src.rotation}");

                if (useRigidbodySync && targetRigidbodies.TryGetValue(bone, out var rb))
                {
                    // 同步刚体旋转（可选同步位置）
                    rb.MoveRotation(src.rotation);
                    rb.MovePosition(src.position - (Vector3.up * 3f));
                    rb.velocity = Vector3.zero; // 禁用速度继承，避免物理干扰
                                                //                               // 计算速度（可选）
                                                //Vector3 velocity = (src.position - rb.position) / Time.fixedDeltaTime;
                                                //rb.velocity = Vector3.zero; //velocity;

                }
                else
                {
                    tgt.rotation = src.rotation;
                    tgt.position = src.position - (Vector3.up * 3f);

                }

                // ✅ 打印目标骨骼的位置和旋转
                //NetChat.Print($"[目标] {bone}: Position = {tgt.position}, Rotation = {tgt.rotation}");
                //Debug.Log($"[动作数据] {bone}: Position = {tgt.position}, Rotation = {tgt.rotation}");
            }
        }
        public void StopSync(bool restorePhysics = true)
        {
            if (restorePhysics)
            {
                foreach (var kvp in targetRigidbodies)
                {
                    var rb = kvp.Value;
                    rb.isKinematic = false;
                    // 重置刚体速度，避免残留运动影响下一次同步
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
            enabled = false;
        }

    }

}
