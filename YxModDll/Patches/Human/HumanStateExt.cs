using HumanAPI;
using Multiplayer;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;  


    /// <summary>
    /// 这个类存放所有扩展的状态字段，专门给Human扩展用。
    /// </summary>
    public class HumanStateExt
    {    /// <增加的>
        public bool isClient;

        //屏蔽炸房相关
        public string lastFaYanStr;
        public int lastFaYanCount = 1;
        public float lastFaYanTimer;
        //挂机提醒
        public bool YiGuaJi;
        public float lastCaoZuoTime = Time.time;

        public float cameraPitchAngle;
        public float cameraYawAngle;
        public Human guajiNtpHuman;

        public bool liaotiankuangquanxian;
        public bool kejiquanxian;
        public bool jinzhibeikong;
        public bool wujiasi;
        public bool wupengzhuang;
        public DingDian dingdian;//个人定点

        public bool zuoxia;//坐下
        public bool yizuoxia;
        public float zuoxiaTime;
        public bool guixia;//跪下
        public float guixiaTime;
        public bool yizima;//一字马
        public bool titui;//踢腿
        public bool yititui;//
        public bool quanji;//Y5拳击
        public bool chuquan;//出拳
        public int numY;//Y   0 晕倒    1 坐下     2 跪下    3 一字马   4 踢腿   5拳王

        public Human ntp_human;
        public bool ntp;
        public bool fzntp;//房主的ntphuman
        public Vector3 ntp_Offset;

        public bool Fly; //飞天
        public bool shanxian; //闪现
        public bool flowing;
        public bool feitian;
        public float flowing_speed = 300f;
        public float extend_time_rush;
        public bool rush;

        public bool chaoren;//咸蛋超人
        public bool enshan;//咸蛋超人

        public bool qianshui;//潜水
        public bool pangxie;//螃蟹
        public bool tuique;//腿瘸
        public bool tuiguai;//腿拐
        public bool bengdi;//蹦迪
                           //public int tiaowu_i;
                           //public bool camPitch_mode;
        public bool diantun;//电臀
        public int diantun_i;
        public bool Diantun_Mod;
        public bool qiqiu;//气球
        public bool qiqiuxifa;//气球戏法
        public bool sanjitiao;//三级跳
        public bool tiaoing;//正在三级跳
        public int Jump_Times;
        public bool chaojitiao;//超级跳


        public float groundDelay2;
        public float jumpDelay2;



        public bool daoli;
        public bool zhuanquan; //转圈圈
        public bool ketouguai;
        public bool diaosigui;
        public float diaosiguiTime;

        public bool chaichu;

        public bool kongqipao;//空气炮
        public bool cannon_used;
        public float extend_time;
        public bool tuoluo;  //陀螺

        public bool dongjie;//冻结

        //public bool qianshou;//牵手
        public bool qianshou_zuo;//牵手左
                                 //public bool beiqianshou_zuo;//被牵手左 
        public Human qianshou_zuo_human;//牵手左
        public HumanSegment qianshou_zuo_humanHand;//牵手左humanHand



        public bool qianshou_you;//牵手右
                                 //public bool beiqianshou_you;//被牵手左
        public Human qianshou_you_human;//牵手右
        public HumanSegment qianshou_you_humanHand;//牵手左humanHand

        public bool banshen;

        public Human human;  // 保存对应Human实例
        private HumanReflectionAccessor accessor;

        public HumanStateExt(Human human)
        {
            this.human = human;
            accessor = new HumanReflectionAccessor(human);
        }

        internal void YxModChuShiHua()
        {
            dingdian = new DingDian();
            var ext = HumanStateExtHelper.GetExt(human);
            if (human == Human.all[0])
            {
                ext.dingdian.huisu = UI_SheZhi.huisu;
                ext.dingdian.guanxing = UI_SheZhi.guanxing;
                ext.dingdian.q = UI_SheZhi.q;
                ext.dingdian.se = UI_SheZhi.se;
                ext.dingdian.gaodu = UI_SheZhi.gaodu;
                ext.dingdian.geshu = UI_SheZhi.geshu;//m_geshu = geshu;
                ext.dingdian.tishiStr = UI_SheZhi.tishiStr;
            }

            lastCaoZuoTime = Time.time;
            qianshou_zuo_humanHand = new HumanSegment();
            qianshou_you_humanHand = new HumanSegment();

            kejiquanxian = UI_WanJia.allkejiquanxian;
            liaotiankuangquanxian = UI_WanJia.allliaotiankuangquanxian;
            dingdian.kaiguan = UI_WanJia.alldingdian;
            wujiasi = UI_WanJia.allwujiasi;
            wupengzhuang = UI_WanJia.allwupengzhuang;
            feitian = UI_WanJia.allfeitian;
            if (feitian)
            {
                YxMod.SetFeiTian(human);
            }
            chaoren = UI_WanJia.allchaoren;
            shanxian = UI_WanJia.allshanxian;
            dongjie = UI_WanJia.alldongjie;
            if (dongjie)
            {
                YxMod.DongJie(human);
            }
            banshen = UI_WanJia.allbanshen;
            YxMod.BanShen(human);

            bengdi = UI_WanJia.allbengdi;
            if (bengdi)
            {
                YxMod.BengDi(human);
            }
            sanjitiao = UI_WanJia.allsanjitiao;
            diantun = UI_WanJia.alldiantun;
            if (diantun)
            {
                YxMod.DianTun(human);
            }
            chaojitiao = UI_WanJia.allchaojitiao;
            if (chaojitiao)
            {
                YxMod.chaojitiao(human);

            }
            qiqiu = UI_WanJia.allqiqiu;
            if (qiqiu)
            {
                YxMod.QiQiu(human);
            }
            qiqiuxifa = UI_WanJia.allqiqiuxifa;
            daoli = UI_WanJia.alldaoli;
            if (daoli)
            {
                YxMod.DaoLi(human);
            }
            zhuanquan = UI_WanJia.allzhuanquan;
            tuoluo = UI_WanJia.alltuoluo;

            ketouguai = UI_WanJia.allketouguai;
            diaosigui = UI_WanJia.alldiaosigui;
            if (diaosigui)
            {
                YxMod.DiaoSiGui(human);
            }
            pangxie = UI_WanJia.allpangxie;
            qianshui = UI_WanJia.allqianshui;

            tuique = UI_WanJia.alltuique;
            if (tuique)
            {
                YxMod.TuiQue(human);
            }
            tuiguai = UI_WanJia.alltuiguai;
            if (tuiguai)
            {
                YxMod.TuiGuai(human);
            }
            chaichu = UI_WanJia.allchaichu;
            kongqipao = UI_WanJia.allkongqipao;


        }
        public void YxModFixedUpdate()
        {
            if (accessor.thisFrameHit + accessor.lastFrameHit > 30f)
            {
                human.MakeUnconscious();
                human.ReleaseGrab(3f);
            }
            accessor.lastFrameHit = accessor.thisFrameHit;
            accessor.thisFrameHit = 0f;
            accessor.jumpDelay -= Time.fixedDeltaTime;
            accessor.groundDelay -= Time.fixedDeltaTime;
            if (!human.disableInput)
            {
                accessor.ProcessInput();
            }
            accessor.LimitFallSpeed();
            Quaternion quaternion = Quaternion.Euler(human.controls.targetPitchAngle, human.controls.targetYawAngle, 0f);
            human.targetDirection = quaternion * Vector3.forward;
            human.targetLiftDirection = Quaternion.Euler(Mathf.Clamp(human.controls.targetPitchAngle, -70f, 80f), human.controls.targetYawAngle, 0f) * Vector3.forward;
            if (NetGame.isClient || ReplayRecorder.isPlaying)
            {
                return;
            }
            if (human.state == HumanState.Dead || human.state == HumanState.Unconscious || human.state == HumanState.Spawning)
            {
                human.controls.leftGrab = (human.controls.rightGrab = false);
                human.controls.shootingFirework = false;
            }
            human.groundAngle = 90f;
            human.groundAngle = Mathf.Min(human.groundAngle, human.ragdoll.partBall.sensor.groundAngle);
            human.groundAngle = Mathf.Min(human.groundAngle, human.ragdoll.partLeftFoot.sensor.groundAngle);
            human.groundAngle = Mathf.Min(human.groundAngle, human.ragdoll.partRightFoot.sensor.groundAngle);
            bool flag = human.hasGrabbed;

            if (sanjitiao && UI_GongNeng.yulexitong_KaiGuan)
            {
                YxMod.SanJiTiao_Fun(human);
            }
            else
            {
                human.onGround = accessor.groundDelay <= 0f && accessor.groundManager.onGround;
            }

            human.hasGrabbed = accessor.grabManager.hasGrabbed;
            human.ragdoll.partBall.sensor.groundAngle = (human.ragdoll.partLeftFoot.sensor.groundAngle = (human.ragdoll.partRightFoot.sensor.groundAngle = 90f));
            if (human.hasGrabbed && human.transform.position.y < accessor.grabStartPosition.y)
            {
                accessor.grabStartPosition = human.transform.position;
            }
            if (human.hasGrabbed && human.transform.position.y - accessor.grabStartPosition.y > 0.5f)
            {
                human.isClimbing = true;
            }
            else
            {
                human.isClimbing = false;
            }
            if (flag != human.hasGrabbed && human.hasGrabbed)
            {
                accessor.grabStartPosition = human.transform.position;
            }
            if (human.state == HumanState.Spawning)
            {
                human.spawning = true;
                if (human.onGround)
                {
                    human.MakeUnconscious();
                }
            }
            else
            {
                human.spawning = false;
            }
            accessor.ProcessUnconscious();
            if (human.state != HumanState.Dead && human.state != HumanState.Unconscious && human.state != HumanState.Spawning)
            {
                accessor.ProcessFall();
                if (human.onGround)
                {
                    if (chaojitiao && UI_GongNeng.yulexitong_KaiGuan)
                    {
                        if (human.controls.walkSpeed > 0f)
                        {
                            human.state = HumanState.Walk;
                        }
                        else
                        {
                            human.state = HumanState.Idle;
                        }
                    }
                    else if (human.controls.jump && accessor.jumpDelay <= 0f)
                    {
                        human.state = HumanState.Jump;
                        human.jump = true;
                        accessor.jumpDelay = 0.5f;
                        accessor.groundDelay = 0.2f;
                    }
                    else if (human.controls.walkSpeed > 0f)
                    {
                        human.state = HumanState.Walk;
                    }
                    else
                    {
                        human.state = HumanState.Idle;
                    }
                }
                else if (human.ragdoll.partLeftHand.sensor.grabObject != null || human.ragdoll.partRightHand.sensor.grabObject != null)
                {
                    human.state = HumanState.Climb;
                }
            }
            if (human.skipLimiting)
            {
                human.skipLimiting = false;
                return;
            }
            for (int i = 0; i < human.rigidbodies.Length; i++)
            {
                Vector3 vector = accessor.velocities[i];
                Vector3 vector2 = human.rigidbodies[i].velocity;
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
                    human.rigidbodies[i].velocity = vector2;
                }
                accessor.velocities[i] = vector2;
            }

            // 你之前加的所有YxMod相关调用
            UI_SheZhi.GuaJiTiXing_Fun(human);
            YxMod.ZuoXia_Fun(human);
            YxMod.YiZiMa_Fun(human);
            YxMod.JiFei_Fun(human);
            YxMod.QuanJi_Fun(human);
            dingdian.DingDian_Fun(human);
            YxMod.WuPengZhuang_Fun(human);
            YxMod.GuaJian_Fun(human);
            YxMod.WuJiaSi_Fun(human);

            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                if (sanjitiao) { sanjitiao = false; }
                if (bengdi) { YxMod.BengDi(human); }
                if (qianshui) { qianshui = false; }
                if (pangxie) { pangxie = false; }
                if (zhuanquan) { zhuanquan = false; }
                if (tuoluo) { tuoluo = false; }
                if (ketouguai) { ketouguai = false; }
                if (diaosigui) { YxMod.DiaoSiGui(human); }
                if (daoli) { YxMod.DaoLi(human); }
                if (chaichu) { chaichu = false; }
                if (kongqipao) { kongqipao = false; }
                if (tuique) { YxMod.TuiQue(human); }
                if (tuiguai) { YxMod.TuiGuai(human); }
                if (diantun) { YxMod.DianTun(human); }
                if (qiqiu) { YxMod.QiQiu(human); }

                if (chaojitiao) { YxMod.chaojitiao(human); }
            }
            YxMod.QianShou_Fun(human);
            YxMod.QianShui_Fun(human);
            YxMod.PangXie_Fun(human);
            YxMod.ZhuanQuan_Fun(human);
            YxMod.TuoLuo_Fun(human);
            YxMod.KeTouGuai_Fun(human);
            YxMod.DiaoSiGui_Fun(human);
            YxMod.DaoLi_Fun(human);
            YxMod.ChaiChu(human);
            YxMod.KongQiPao_Fun(human);
            YxMod.TuiQue_Fun(human);
            YxMod.DianTun_Fun(human);
            YxMod.QiQiu_Fun(human);
            YxMod.QiQiuXiFa_Fun(human);
            YxMod.chaojitiao(human);
            YxMod.FeiTian_Fun(human);
            YxMod.ShanXian_Fun(human);
            YxMod.ChaoRen_Fun(human);

            //YxMod.BanShen_Fun(human);
            YxMod.QuXiaoBanShen(human);
        }

        internal void YxModProcessInput()
        {
            if (!NetGame.isClient && !ReplayRecorder.isPlaying)
            {
                if (human.controls.unconscious)
                {
                    if (numY == 0)
                    {
                        human.MakeUnconscious();
                    }
                    else if (numY == 1)//坐下
                    {
                        YxMod.ZuoXia(human);
                    }
                    else if (numY == 2)//跪下
                    {
                        YxMod.ZuoXia(human, true);
                    }
                    else if (numY == 3)//一字马
                    {
                        YxMod.YiZiMa(human);
                    }
                    else if (numY == 4)//踢腿
                    {
                        YxMod.TiTui(human);
                    }
                    else if (numY == 5)//拳王
                    {
                        quanji = true;
                        YxMod.QuanJiAnimation(human);
                    }
                }
                else
                {
                    yititui = false;
                    titui = false;
                    quanji = false;
                }
                //if (controls.unconscious)
                //{
                //	MakeUnconscious();
                //}
                if (human.motionControl2.enabled)
                {
                    human.motionControl2.OnFixedUpdate();
                }
            }
        }
    }

    /// <summary>
    /// 这个静态类负责关联 Human 实例和 HumanStateExt 的状态。
    /// 通过 ConditionalWeakTable 实现扩展字段，避免修改原类。
    /// </summary>
    public static class HumanStateExtHelper
    {
        // 通过 ConditionalWeakTable 把 Human 和扩展状态关联起来
        private static readonly ConditionalWeakTable<Human, HumanStateExt> _extTable = new ConditionalWeakTable<Human, HumanStateExt>();

        /// <summary>
        /// 获取指定 Human 实例对应的扩展状态对象，如果没有则自动创建
        /// </summary>
        public static HumanStateExt GetExt(this Human human)
        {
            return _extTable.GetOrCreateValue(human);
        }
    }

