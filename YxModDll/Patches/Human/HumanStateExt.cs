using HumanAPI;
using Multiplayer;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using YxModDll.Mod;

namespace YxModDll.Patches
{
    /// <summary>
    /// 这个类存放所有扩展的状态字段，专门给Human扩展用。
    /// </summary>
    public class HumanStateExt
    {
        public HumanStateExt()
        {
            // 这里可以留空或者初始化一些默认值
        }
        /// <增加的>
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
        public bool fanchibang;
        public bool fanchibangY8;
        public float baibiTime;
        public Vector3 shengkongGaoDu = Vector3.zero;
        public bool ninjarun; //忍者跑
        public bool naosini;
        public float naosini_zhuanquanTime;
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
        internal HumanReflectionAccessor accessor;
        internal bool shouhua;

        public bool wutiguajian;//物体挂件
        public Rigidbody ntp_wuti;

        public bool bofangdonghua;
        public Human bei_human;

        // public bool PiLiWuDongJie;
        // public bool JiaoChaTiaoYue;
        // public bool YangWoQiZuo;
        // public bool FuWoCheng;
        // public bool XiHaWu;
        // public bool XiHaWu2;
        // public bool XiHaWu3;
        // public bool TouXuan;
        // public bool MuMaTi;
        // public bool KaiHeTiao;
        // public bool YaoBaiWu;
        // public bool SangBaWu;
        // public bool SangBaWu2;
        //public bool DianTunWu;
        // public bool QuanJi;
        // public bool QiMaWu;
        // public bool HeiYingTaoWuBu;

        //public bool BoFangDongHua;


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
                ext.dingdian.baoLiuDangQianSuDu = UI_SheZhi.baoLiuDangQianSuDu;
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
    }

    /// <summary>
    /// 这个静态类负责关联 Human 实例和 HumanStateExt 的状态。
    /// 通过 ConditionalWeakTable 实现扩展字段，避免修改原类。
    /// </summary>
    public static class HumanStateExtHelper
    {
        // 通过 ConditionalWeakTable 把 Human 和扩展状态关联起来
        private static readonly ConditionalWeakTable<Human, HumanStateExt> table = new ConditionalWeakTable<Human, HumanStateExt>();

        /// <summary>
        /// 获取指定 Human 实例对应的扩展状态对象，如果没有则自动创建
        /// </summary>
        public static HumanStateExt GetExt(this Human human)
        {
            if (human == null)
            {
                Debug.LogError("[YxMod] GetExt: human 是 null");
                return null;
            }

            var ext = table.GetValue(human, h => {
                Debug.Log("[YxMod] 创建 HumanStateExt 实例: " + h.name);
                return new HumanStateExt(h);
            });

            if (ext.human == null || ext.dingdian == null)
            {
                Debug.LogWarning("[YxMod] ext.human 或 dingdian 未初始化，在 GetExt 内懒加载初始化");
                ext.human = human;
                ext.YxModChuShiHua();
            }

            return ext;
        }

    }

}
