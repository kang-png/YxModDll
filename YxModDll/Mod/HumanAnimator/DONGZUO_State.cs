using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace YxModDll.Mod.HumanAnimator
{
    public enum DONGZUO_State
    {
        TuoMaSi,
        PiLiWuDongJie,
        JiaoChaTiaoYue,
        YangWoQiZuo,
        FuWoCheng,
        XiHaWu,
        XiHaWu2,
        XiHaWu3,
        TouXuan,
        MuMaTi,
        KaiHeTiao,
        YaoBaiWu,
        SangBaWu,
        SangBaWu2,
        DianTunWu,
        QuanJi,
        QiMaWu,
        HeiYingTaoWuBu,
        ManPao,

    }
    public enum BoneId : byte
    {
        Hips,
        Waist,
        Chest,
        Head,

        LeftArm,
        LeftForearm,
        LeftHand,

        RightArm,
        RightForearm,
        RightHand,

        LeftThigh,
        LeftLeg,
        LeftFoot,

        RightThigh,
        RightLeg,
        RightFoot,

        Count // 总数
    }

    public struct BoneFrameData
    {
        public BoneId Bone;        
        public Vector3 Position;
        public Quaternion Rotation;
    }
}
