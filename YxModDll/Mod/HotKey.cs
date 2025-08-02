using Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;
using YxModDll.Mod;


namespace YxModDll.Mod
{
    internal class HotKey : MonoBehaviour
    {
        public static KuaiJieJian_Type CunDian = new KuaiJieJian_Type();
        public static KuaiJieJian_Type UP = new KuaiJieJian_Type();
        public static KuaiJieJian_Type IFG = new KuaiJieJian_Type();
        public static KuaiJieJian_Type ZaiRuZiJi = new KuaiJieJian_Type();
        public static KuaiJieJian_Type JiHe = new KuaiJieJian_Type();
        public static KuaiJieJian_Type ChongZhiWuPin = new KuaiJieJian_Type();
        public static KuaiJieJian_Type ShangYiGuan = new KuaiJieJian_Type();
        public static KuaiJieJian_Type XiaYiGuan = new KuaiJieJian_Type();
        public static KuaiJieJian_Type ChuangJianFangJian = new KuaiJieJian_Type();
        public static KuaiJieJian_Type QuanYuanFeiTian = new KuaiJieJian_Type();
        public static KuaiJieJian_Type QuanYuanChaoRen = new KuaiJieJian_Type();
        public static KuaiJieJian_Type QuanYuanShanXian = new KuaiJieJian_Type();
        public static KuaiJieJian_Type QuanJiZiShi = new KuaiJieJian_Type();
        public static KuaiJieJian_Type WuPinGuaJian = new KuaiJieJian_Type();
        
        //一个数字键
        public static KuaiJieJian_Type KongZhiFenShen = new KuaiJieJian_Type();
        public static KuaiJieJian_Type QieHuanFenShen = new KuaiJieJian_Type();
        public static KuaiJieJian_Type ZhiDingFeiTian = new KuaiJieJian_Type();
        public static KuaiJieJian_Type ZhiDingChaoRen = new KuaiJieJian_Type();
        public static KuaiJieJian_Type ZhiDingShanXian = new KuaiJieJian_Type();
        //两个数字键
        public static KuaiJieJian_Type ChuanSong = new KuaiJieJian_Type();
        public static KuaiJieJian_Type XuanFu = new KuaiJieJian_Type();
        public static KuaiJieJian_Type QianShou = new KuaiJieJian_Type();

        private void Start()
        {
            DuQuKuaiJieJian();
        }
        private static void DuQuKuaiJieJian()
        {
            CunDian = INI.GetKuaiJieJian("存点", new KuaiJieJian_Type(KeyCode.Q));
            UP = INI.GetKuaiJieJian("UP", new KuaiJieJian_Type(KeyCode.Mouse2));
            IFG = INI.GetKuaiJieJian("IFG", new KuaiJieJian_Type(KeyCode.B));
            ZaiRuZiJi = INI.GetKuaiJieJian("载入自己", new KuaiJieJian_Type(KeyCode.LeftControl, KeyCode.F));
            JiHe = INI.GetKuaiJieJian("召集玩家", new KuaiJieJian_Type(KeyCode.F2));
            ChongZhiWuPin = INI.GetKuaiJieJian("重置物品", new KuaiJieJian_Type(KeyCode.F3));
            ShangYiGuan = INI.GetKuaiJieJian("上一关", new KuaiJieJian_Type(KeyCode.PageUp));
            XiaYiGuan = INI.GetKuaiJieJian("下一关", new KuaiJieJian_Type(KeyCode.PageDown));
            ChuangJianFangJian = INI.GetKuaiJieJian("创建房间", new KuaiJieJian_Type(KeyCode.F7));
            QuanYuanFeiTian = INI.GetKuaiJieJian("全员飞天", new KuaiJieJian_Type(KeyCode.F4));
            QuanYuanChaoRen = INI.GetKuaiJieJian("全员超人", new KuaiJieJian_Type(KeyCode.F5));
            QuanYuanShanXian = INI.GetKuaiJieJian("全员闪现", new KuaiJieJian_Type(KeyCode.F6));
            QuanJiZiShi = INI.GetKuaiJieJian("拳击姿势", new KuaiJieJian_Type(KeyCode.R));
            WuPinGuaJian = INI.GetKuaiJieJian("物品挂件", new KuaiJieJian_Type(KeyCode.M));


            //一个数字键
            KongZhiFenShen = INI.GetKuaiJieJian("控制分身", new KuaiJieJian_Type(KeyCode.LeftAlt));
            QieHuanFenShen = INI.GetKuaiJieJian("切换分身", new KuaiJieJian_Type(KeyCode.LeftControl));
            ZhiDingFeiTian = INI.GetKuaiJieJian("指定飞", new KuaiJieJian_Type(KeyCode.F));
            ZhiDingChaoRen = INI.GetKuaiJieJian("指定超人", new KuaiJieJian_Type(KeyCode.C));
            ZhiDingShanXian = INI.GetKuaiJieJian("指定闪现", new KuaiJieJian_Type(KeyCode.B));

            //两个数字键
            ChuanSong = INI.GetKuaiJieJian("传送至", new KuaiJieJian_Type(KeyCode.Mouse0));
            XuanFu = INI.GetKuaiJieJian("悬浮于", new KuaiJieJian_Type(KeyCode.Mouse1));
            QianShou = INI.GetKuaiJieJian("牵手", new KuaiJieJian_Type(KeyCode.Z));
        }
        public static ShuZi_Type Is1(KuaiJieJian_Type kuaijiejian)
        {
            bool fuzhujian=false;
            if (kuaijiejian.keyCode1 == KeyCode.None)
            {
                fuzhujian=false;
            }

            if (kuaijiejian.keyCode2 == KeyCode.None && Input.GetKey(kuaijiejian.keyCode1))
            {
                fuzhujian = true;
            }

            if (Input.GetKey(kuaijiejian.keyCode1) && Input.GetKey(kuaijiejian.keyCode2))
            {
                fuzhujian = true;
            }

            ShuZi_Type shuZi_Type = new ShuZi_Type() ;
            if (fuzhujian)
            {
                shuZi_Type = ShuZiZuHe1();
            }
            return shuZi_Type;
        }

        public static ShuZi_Type Is2(KuaiJieJian_Type kuaijiejian)
        {

            bool fuzhujian = false;
            if (kuaijiejian.keyCode1 == KeyCode.None)
            {
                fuzhujian = false;
            }

            if (kuaijiejian.keyCode2 == KeyCode.None && Input.GetKey(kuaijiejian.keyCode1))
            {
                fuzhujian = true;
            }

            if (Input.GetKey(kuaijiejian.keyCode1) && Input.GetKey(kuaijiejian.keyCode2))
            {
                fuzhujian = true;
            }

            ShuZi_Type shuZi_Type = new ShuZi_Type();
            if (fuzhujian)
            {
                shuZi_Type = ShuZiZuHe2();
            }
            return shuZi_Type;
        }
        private static ShuZi_Type ShuZiZuHe1()
        {
            ShuZi_Type shuZi_Type = new ShuZi_Type();
            for (int i = 0; i < 10; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i) || Input.GetKeyDown(KeyCode.Keypad1 + i))
                {
                    shuZi_Type.num1 = i;
                   
                }
            }
            return shuZi_Type;
        }
        private static ShuZi_Type ShuZiZuHe2()
        {
            ShuZi_Type shuZi_Type = new ShuZi_Type();
            for (int i = 0; i < 10; i++)
            {
                if (Input.GetKey(KeyCode.Alpha1 + i) || Input.GetKey(KeyCode.Keypad1 + i))
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (j != i)
                        {
                            if (Input.GetKeyDown(KeyCode.Alpha1 + j) || Input.GetKeyDown(KeyCode.Keypad1 + j))
                            {
                                if (i < Human.all.Count && j < Human.all.Count)
                                {
                                    shuZi_Type.num1 = i;
                                    shuZi_Type.num2 = j;
                                }
                            }
                        }
                    }
                }
            }
            return shuZi_Type;
        }
        public static bool Is(KuaiJieJian_Type kuaijiejian)
        {
            if (kuaijiejian.keyCode1 == KeyCode.None)
            {
                return false;
            }

            if (Input.GetKeyDown(kuaijiejian.keyCode1) && kuaijiejian.keyCode2 == KeyCode.None )
            {
                return true;
            }

            if (Input.GetKey(kuaijiejian.keyCode1) && Input.GetKeyDown(kuaijiejian.keyCode2))
            {
                return true;
            }
            return false;
        }


        public static void Draw()
        {

            using (UI.Vertical())
            {
                using (UI.Horizontal())
                {
                    UI.CreatAnNiu("修改快捷键", false, () =>
                    {
                        INI.OpenIni();
                    });
                    UI.CreatAnNiu("重新读取快捷键", false, () =>
                    {
                        DuQuKuaiJieJian();
                    });
                }
                ;

                //using (UI.ScrollView(ref scrollPosition, GUILayout.ExpandWidth(true)))
                //{
                UI.CreateHotKey("存点：", ref CunDian);
                UI.CreateHotKey("创建房间：", ref ChuangJianFangJian);
                UI.CreateHotKey("UP：", ref UP);
                    UI.CreateHotKey("IFG：", ref IFG);
                UI.CreateHotKey("载入自己：", ref ZaiRuZiJi);
                UI.CreateHotKey("集合：", ref JiHe);
                UI.CreateHotKey("重置物品：", ref ChongZhiWuPin);
                UI.CreateHotKey("全员飞天：", ref QuanYuanFeiTian);
                UI.CreateHotKey("全员超人：", ref QuanYuanChaoRen);
                UI.CreateHotKey("全员闪现：", ref QuanYuanShanXian);
                UI.CreateHotKey("上一关：", ref ShangYiGuan);
                UI.CreateHotKey("下一关：", ref XiaYiGuan);
                UI.CreateHotKey("拳击姿势：", ref QuanJiZiShi);
                UI.CreateHotKey("物体挂件：", ref WuPinGuaJian);

                //一个数字键
                UI.CreateHotKey("控制分身：", ref KongZhiFenShen, 1);
                UI.CreateHotKey("切换分身：", ref QieHuanFenShen, 1);
                UI.CreateHotKey("指定飞天：", ref ZhiDingFeiTian, 1);
                UI.CreateHotKey("指定超人：", ref ZhiDingChaoRen, 1);
                UI.CreateHotKey("指定闪现：", ref ZhiDingShanXian, 1);
                //两个数字键
                UI.CreateHotKey("传送至：", ref ChuanSong, 2);
                UI.CreateHotKey("悬浮于：", ref XuanFu, 2);
                UI.CreateHotKey("牵手：", ref QianShou, 2);

            }
            ;

        }
    }
    public class KuaiJieJian_Type
    {
        public KeyCode keyCode1;
        public KeyCode keyCode2;
        public KuaiJieJian_Type()
        {
            keyCode1 = KeyCode.None;
            keyCode2 = KeyCode.None;
        }
        public KuaiJieJian_Type(KeyCode key)
        {
            keyCode1 = key;
            keyCode2 = KeyCode.None;
        }
        public KuaiJieJian_Type(KeyCode key1, KeyCode key2)
        {
            keyCode1 = key1;
            keyCode2 = key2;
        }
    }

    public class ShuZi_Type
    {
        public int num1;
        public int num2;
        public ShuZi_Type()
        {
            num1 = -1;
            num2 = -1;
        }

    }

}
