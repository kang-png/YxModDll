using CurveExtended;
using HumanAPI;
using Multiplayer;
using System;
using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;
using YxModDll.Mod;
using YxModDll.Patches;


namespace YxModDll.Patches
{
    public class Patcher_Human : MonoBehaviour
    {
        private static FieldInfo _thisFrameHit;
        private static FieldInfo _lastFrameHit;
        private static FieldInfo _jumpDelay;
        private static FieldInfo _groundDelay;
        private static FieldInfo _groundManager;
        private static FieldInfo _grabManager;
        private static FieldInfo _grabStartPosition;
        private static FieldInfo _velocities;
        private static FieldInfo _isFallSpeedInitialized;
        private static FieldInfo _isFallSpeedLimited;
        //private static MethodInfo _ProcessFall;

        private static FieldInfo _lastGroundAngle;
        private static FieldInfo _groundAnglesSum;
        private static FieldInfo _groundAngles;
        private static FieldInfo _groundAnglesIdx;

        private static FieldInfo _slideTimer;
        private static FieldInfo _fallTimer;


        private void Awake()
        {
            _thisFrameHit = typeof(Human).GetField("thisFrameHit", BindingFlags.NonPublic | BindingFlags.Instance);
            _lastFrameHit = typeof(Human).GetField("lastFrameHit", BindingFlags.NonPublic | BindingFlags.Instance);
            _jumpDelay = typeof(Human).GetField("jumpDelay", BindingFlags.NonPublic | BindingFlags.Instance);
            _groundDelay = typeof(Human).GetField("groundDelay", BindingFlags.NonPublic | BindingFlags.Instance);
            _groundManager = typeof(Human).GetField("groundManager", BindingFlags.NonPublic | BindingFlags.Instance);
            _grabManager = typeof(Human).GetField("grabManager", BindingFlags.NonPublic | BindingFlags.Instance);
            _grabStartPosition = typeof(Human).GetField("grabStartPosition", BindingFlags.NonPublic | BindingFlags.Instance);
            _velocities = typeof(Human).GetField("velocities", BindingFlags.NonPublic | BindingFlags.Instance);
            _isFallSpeedInitialized = typeof(Human).GetField("isFallSpeedInitialized", BindingFlags.NonPublic | BindingFlags.Instance);
            _isFallSpeedLimited = typeof(Human).GetField("isFallSpeedLimited", BindingFlags.NonPublic | BindingFlags.Instance);
            // 获取私有方法信息
            //_ProcessFall = typeof(Human).GetMethod("ProcessFall", BindingFlags.NonPublic | BindingFlags.Instance);

            _lastGroundAngle = typeof(Human).GetField("lastGroundAngle", BindingFlags.NonPublic | BindingFlags.Instance);
            _groundAnglesSum = typeof(Human).GetField("groundAnglesSum", BindingFlags.NonPublic | BindingFlags.Instance);
            _groundAngles = typeof(Human).GetField("groundAngles", BindingFlags.NonPublic | BindingFlags.Instance);
            _groundAnglesIdx = typeof(Human).GetField("groundAnglesIdx", BindingFlags.NonPublic | BindingFlags.Instance);

            _slideTimer = typeof(Human).GetField("slideTimer", BindingFlags.NonPublic | BindingFlags.Instance);
            _fallTimer = typeof(Human).GetField("fallTimer", BindingFlags.NonPublic | BindingFlags.Instance);


            Patcher2.MethodPatch(typeof(Human), "FixedUpdate", null, typeof(Patcher_Human), "Human_FixedUpdate", new Type[] { typeof(Human) });

        }

        private void OnGUI()
        {
            //if (GUILayout.Button("开关"))
            //{

            //}
        }
        public static void Human_FixedUpdate(Human instance)
        {


            var ext1 = HumanStateExtHelper.GetExt(instance);

            // 确保 ext.human 和 ext.dingdian 初始化过
            if (ext1.human == null || ext1.dingdian == null)
            {
                Debug.LogWarning("[YxMod] ext.human 或 dingdian 未初始化，正在执行懒加载初始化");
                ext1.human = instance;
                ext1.YxModChuShiHua();
            }

            var thisFrameHit = (float)_thisFrameHit.GetValue(instance);
            var lastFrameHit = (float)_lastFrameHit.GetValue(instance);
            var jumpDelay = (float)_jumpDelay.GetValue(instance);
            var groundDelay = (float)_groundDelay.GetValue(instance);
            var groundManager = (GroundManager)_groundManager.GetValue(instance);
            var grabManager = (GrabManager)_grabManager.GetValue(instance);
            var grabStartPosition = (Vector3)_grabStartPosition.GetValue(instance);
            var velocities = (Vector3[])_velocities.GetValue(instance);


            if (thisFrameHit + lastFrameHit > 30f)
            {
                instance.MakeUnconscious();
                instance.ReleaseGrab(3f);
            }
            lastFrameHit = thisFrameHit;
            _lastFrameHit.SetValue(instance, lastFrameHit);
            thisFrameHit = 0f;
            _thisFrameHit.SetValue(instance, thisFrameHit);
            jumpDelay -= Time.fixedDeltaTime;
            _jumpDelay.SetValue(instance, jumpDelay);
            groundDelay -= Time.fixedDeltaTime;
            _groundDelay.SetValue(instance, groundDelay);
            if (!instance.disableInput)
            {
                ProcessInput(instance);
            }
            LimitFallSpeed(instance);
            Quaternion quaternion = Quaternion.Euler(instance.controls.targetPitchAngle, instance.controls.targetYawAngle, 0f);
            instance.targetDirection = quaternion * Vector3.forward;
            instance.targetLiftDirection = Quaternion.Euler(Mathf.Clamp(instance.controls.targetPitchAngle, -70f, 80f), instance.controls.targetYawAngle, 0f) * Vector3.forward;
            if (NetGame.isClient || ReplayRecorder.isPlaying)
            {
                return;
            }
            if (instance.state == HumanState.Dead || instance.state == HumanState.Unconscious || instance.state == HumanState.Spawning)
            {
                instance.controls.leftGrab = (instance.controls.rightGrab = false);
                instance.controls.shootingFirework = false;
            }
            instance.groundAngle = 90f;
            instance.groundAngle = Mathf.Min(instance.groundAngle, instance.ragdoll.partBall.sensor.groundAngle);
            instance.groundAngle = Mathf.Min(instance.groundAngle, instance.ragdoll.partLeftFoot.sensor.groundAngle);
            instance.groundAngle = Mathf.Min(instance.groundAngle, instance.ragdoll.partRightFoot.sensor.groundAngle);
            bool flag = instance.hasGrabbed;
            //修改  三级跳
            //instance.onGround = groundDelay <= 0f && groundManager.onGround;

            if (instance.GetExt().sanjitiao && UI_GongNeng.yulexitong_KaiGuan)
            {
                YxMod.SanJiTiao_Fun(instance);
            }
            else
            {
                instance.onGround = (groundDelay <= 0f && groundManager.onGround);
            }
            ///
            //onGround = groundDelay <= 0f && groundManager.onGround;
            instance.hasGrabbed = grabManager.hasGrabbed;
            instance.ragdoll.partBall.sensor.groundAngle = (instance.ragdoll.partLeftFoot.sensor.groundAngle = (instance.ragdoll.partRightFoot.sensor.groundAngle = 90f));
            if (instance.hasGrabbed && instance.transform.position.y < grabStartPosition.y)
            {
                grabStartPosition = instance.transform.position;
                _grabStartPosition.SetValue(instance, grabStartPosition);
            }
            if (instance.hasGrabbed && instance.transform.position.y - grabStartPosition.y > 0.5f)
            {
                instance.isClimbing = true;
            }
            else
            {
                instance.isClimbing = false;
            }
            if (flag != instance.hasGrabbed && instance.hasGrabbed)
            {
                grabStartPosition = instance.transform.position;
                _grabStartPosition.SetValue(instance, grabStartPosition);
            }
            if (instance.state == HumanState.Spawning)
            {
                instance.spawning = true;
                if (instance.onGround)
                {
                    instance.MakeUnconscious();
                }
            }
            else
            {
                instance.spawning = false;
            }
            ProcessUnconscious(instance);
            if (instance.state != HumanState.Dead && instance.state != HumanState.Unconscious && instance.state != HumanState.Spawning)
            {
                ProcessFall(instance);
                //_ProcessFall.Invoke(instance, null);
                if (instance.onGround)
                {
                    if (instance.GetExt().chaojitiao && UI_GongNeng.yulexitong_KaiGuan)
                    {
                        if (instance.controls.walkSpeed > 0f)
                        {
                            instance.state = HumanState.Walk;
                        }
                        else
                        {
                            instance.state = HumanState.Idle;
                        }
                    }
                    else if (instance.controls.jump && jumpDelay <= 0f)
                    {
                        instance.state = HumanState.Jump;
                        instance.jump = true;
                        jumpDelay = 0.5f;
                        _jumpDelay.SetValue(instance, jumpDelay);
                        groundDelay = 0.2f;
                        _groundDelay.SetValue(instance, groundDelay);
                    }
                    else if (instance.controls.walkSpeed > 0f)
                    {
                        instance.state = HumanState.Walk;
                    }
                    else
                    {
                        instance.state = HumanState.Idle;
                    }
                }
                else if (instance.ragdoll.partLeftHand.sensor.grabObject != null || instance.ragdoll.partRightHand.sensor.grabObject != null)
                {
                    instance.state = HumanState.Climb;
                }
            }
            if (instance.skipLimiting)
            {
                instance.skipLimiting = false;
                return;
            }
            for (int i = 0; i < instance.rigidbodies.Length; i++)
            {
                Vector3 vector = velocities[i];
                Vector3 vector2 = instance.rigidbodies[i].velocity;
                Vector3 vector3 = vector2 - vector;
                if (Vector3.Dot(vector, vector3) < 0f)
                {
                    Vector3 normalized = vector.normalized;
                    float magnitude = vector.magnitude;
                    float value = 0f - Vector3.Dot(normalized, vector3);
                    float num = Mathf.Clamp(value, 0f, magnitude);
                    vector3 += normalized * num;
                }
                float num2 = 1000f * Time.deltaTime;
                if (vector3.magnitude > num2)
                {
                    Vector3 vector4 = Vector3.ClampMagnitude(vector3, num2);
                    vector2 -= vector3 - vector4;
                    instance.rigidbodies[i].velocity = vector2;
                }
                //var velocities = (Vector3[])_velocities.GetValue(instance);
                velocities[i] = vector2;
                _velocities.SetValue(instance, velocities);
            }

            // 你之前加的所有YxMod相关调用
            UI_SheZhi.GuaJiTiXing_Fun(instance);
            YxMod.ZuoXia_Fun(instance);
            YxMod.YiZiMa_Fun(instance);
            YxMod.JiFei_Fun(instance);
            YxMod.QuanJi_Fun(instance);
            instance.GetExt().dingdian.DingDian_Fun(instance);
            YxMod.WuPengZhuang_Fun(instance);
            YxMod.GuaJian_Fun(instance);
            YxMod.WuJiaSi_Fun(instance);

            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                var ext = instance.GetExt();

                if (ext.sanjitiao) { ext.sanjitiao = false; }
                if (ext.bengdi) { YxMod.BengDi(instance); }
                if (ext.qianshui) { ext.qianshui = false; }
                if (ext.pangxie) { ext.pangxie = false; }
                if (ext.zhuanquan) { ext.zhuanquan = false; }
                if (ext.tuoluo) { ext.tuoluo = false; }
                if (ext.ketouguai) { ext.ketouguai = false; }
                if (ext.diaosigui) { YxMod.DiaoSiGui(instance); }
                if (ext.daoli) { YxMod.DaoLi(instance); }
                if (ext.chaichu) { ext.chaichu = false; }
                if (ext.kongqipao) { ext.kongqipao = false; }
                if (ext.tuique) { YxMod.TuiQue(instance); }
                if (ext.tuiguai) { YxMod.TuiGuai(instance); }
                if (ext.diantun) { YxMod.DianTun(instance); }
                if (ext.qiqiu) { YxMod.QiQiu(instance); }
                if (ext.chaojitiao) { YxMod.chaojitiao(instance); }
            }
            YxMod.QianShou_Fun(instance);
            YxMod.QianShui_Fun(instance);
            YxMod.PangXie_Fun(instance);
            YxMod.ZhuanQuan_Fun(instance);
            YxMod.TuoLuo_Fun(instance);
            YxMod.KeTouGuai_Fun(instance);
            YxMod.DiaoSiGui_Fun(instance);
            YxMod.DaoLi_Fun(instance);
            YxMod.ChaiChu(instance);
            YxMod.KongQiPao_Fun(instance);
            YxMod.TuiQue_Fun(instance);
            YxMod.DianTun_Fun(instance);
            YxMod.QiQiu_Fun(instance);
            YxMod.QiQiuXiFa_Fun(instance);
            YxMod.chaojitiao(instance);
            YxMod.FeiTian_Fun(instance);
            YxMod.ShanXian_Fun(instance);
            YxMod.ChaoRen_Fun(instance);

            //YxMod.BanShen_Fun(instance);
            YxMod.QuXiaoBanShen(instance);
        }
        private static void LimitFallSpeed(Human instance)
        {
            var isFallSpeedLimited = (bool)_isFallSpeedLimited.GetValue(instance);
            var isFallSpeedInitialized = (bool)_isFallSpeedInitialized.GetValue(instance);

            bool flag = Game.instance.state != GameState.PlayingLevel;
            if (isFallSpeedLimited != flag || !isFallSpeedInitialized)
            {
                isFallSpeedInitialized = true;
                isFallSpeedLimited = flag;
                if (flag)
                {
                    instance.SetDrag(0.1f, external: false);
                }
                else
                {
                    instance.SetDrag(0.05f, external: false);
                }
            }
        }
        private static void ProcessUnconscious(Human instance)
        {
            if (instance.state == HumanState.Unconscious)
            {
                instance.unconsciousTime -= Time.fixedDeltaTime;
                if (instance.unconsciousTime <= 0f)
                {
                    instance.state = HumanState.Fall;
                    instance.wakeUpTime = 2f;//maxWakeUpTime
                    instance.unconsciousTime = 0f;
                }
            }
            if (instance.wakeUpTime > 0f)
            {
                instance.wakeUpTime -= Time.fixedDeltaTime;
                if (instance.wakeUpTime <= 0f)
                {
                    instance.wakeUpTime = 0f;
                }
            }
        }

        private static void ProcessInput(Human instance)
        {
            if (!NetGame.isClient && !ReplayRecorder.isPlaying)
            {
                //if (instance.controls.unconscious)
                //{
                //    instance. MakeUnconscious();
                //}
                if (instance.motionControl2.enabled)
                {
                    instance.motionControl2.OnFixedUpdate();
                }
            }
        }

        private static void PushGroundAngle(Human instance)
        {
            var lastGroundAngle = (float)_lastGroundAngle.GetValue(instance);
            var groundAnglesSum = (float)_groundAnglesSum.GetValue(instance);
            var groundAnglesIdx = (int)_groundAnglesIdx.GetValue(instance);
            var groundAngles = (float[])_groundAngles.GetValue(instance);


            float num = (lastGroundAngle = ((!instance.onGround || !(instance.groundAngle < 80f)) ? lastGroundAngle : instance.groundAngle));
            _lastGroundAngle.SetValue(instance, lastGroundAngle);
            groundAnglesSum -= groundAngles[groundAnglesIdx];
            _groundAnglesSum.SetValue(instance, groundAnglesSum);

            groundAnglesSum += num;
            _groundAnglesSum.SetValue(instance, groundAnglesSum);

            groundAngles[groundAnglesIdx] = num;
            _groundAngles.SetValue(instance, groundAngles);
            groundAnglesIdx = (groundAnglesIdx + 1) % groundAngles.Length;
            _groundAnglesIdx.SetValue(instance, groundAnglesIdx);
        }

        private static void ProcessFall(Human instance)
        {
            var groundAnglesSum = (float)_groundAnglesSum.GetValue(instance);
            var groundAngles = (float[])_groundAngles.GetValue(instance);

            var slideTimer = (float)_slideTimer.GetValue(instance);
            var fallTimer = (float)_fallTimer.GetValue(instance);

            PushGroundAngle(instance);
            bool flag = false;
            if (groundAnglesSum / (float)groundAngles.Length > 45f)
            {
                flag = true;
                slideTimer = 0f;
                _slideTimer.SetValue(instance, slideTimer);
                instance.onGround = false;
                instance.state = HumanState.Slide;
            }
            else if (instance.state == HumanState.Slide && groundAnglesSum / (float)groundAngles.Length < 37f && instance.ragdoll.partBall.rigidbody.velocity.y > -1f)
            {
                slideTimer += Time.fixedDeltaTime;
                _slideTimer.SetValue(instance, slideTimer);
                if (slideTimer < 0.003f)
                {
                    instance.onGround = false;
                }
            }
            if (!instance.onGround && !flag)
            {
                if (fallTimer < 5f)
                {
                    fallTimer += Time.deltaTime;
                    _fallTimer.SetValue(instance, fallTimer);
                }
                if (instance.state == HumanState.Climb)
                {
                    fallTimer = 0f;
                    _fallTimer.SetValue(instance, fallTimer);
                }
                if (fallTimer > 3f)
                {
                    instance.state = HumanState.FreeFall;
                }
                else if (fallTimer > 1f)
                {
                    instance.state = HumanState.Fall;
                }
            }
            else
            {
                fallTimer = 0f;
                _fallTimer.SetValue(instance, fallTimer);
            }
        }



    }
}

