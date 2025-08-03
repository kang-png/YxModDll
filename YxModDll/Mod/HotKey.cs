using Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;
using YxModDll.Mod;


namespace YxModDll.Mod
{
    internal class HotKey : MonoBehaviour
    {
        public static List<KuaiJieJian_Type> CunDian;
        public static List<KuaiJieJian_Type> UP;
        public static List<KuaiJieJian_Type> IFG;
        public static List<KuaiJieJian_Type> ZaiRuZiJi;
        public static List<KuaiJieJian_Type> JiHe;
        public static List<KuaiJieJian_Type> ChongZhiWuPin;
        public static List<KuaiJieJian_Type> ShangYiGuan;
        public static List<KuaiJieJian_Type> XiaYiGuan;
        public static List<KuaiJieJian_Type> ChuangJianFangJian;
        public static List<KuaiJieJian_Type> QuanYuanFeiTian;
        public static List<KuaiJieJian_Type> QuanYuanChaoRen;
        public static List<KuaiJieJian_Type> QuanYuanShanXian;
        public static List<KuaiJieJian_Type> QuanJiZiShi;
        public static List<KuaiJieJian_Type> WuPinGuaJian;

        // 一个数字键
        public static List<KuaiJieJian_Type> KongZhiFenShen;
        public static List<KuaiJieJian_Type> QieHuanFenShen;
        public static List<KuaiJieJian_Type> ZhiDingFeiTian;
        public static List<KuaiJieJian_Type> ZhiDingChaoRen;
        public static List<KuaiJieJian_Type> ZhiDingShanXian;

        // 两个数字键
        public static List<KuaiJieJian_Type> ChuanSong;
        public static List<KuaiJieJian_Type> XuanFu;
        public static List<KuaiJieJian_Type> QianShou;

        private void Start()
        {
            DuQuKuaiJieJian();
        }

        private static List<KuaiJieJian_Type> KJ(params object[] keys)
        {
            var list = new List<KuaiJieJian_Type>();
            foreach (var item in keys)
            {
                if (item is KeyCode k)
                {
                    list.Add(new KuaiJieJian_Type(k));
                }
                else if (item is ValueTuple<KeyCode, KeyCode> pair)
                {
                    list.Add(new KuaiJieJian_Type(pair.Item1, pair.Item2));
                }
            }
            return list;
            //使用示例：ZaiRuZiJi = INI.GetKuaiJieJianList("载入自己", KJ((KeyCode.LeftControl, KeyCode.F), (KeyCode.RightControl, KeyCode.F)));
        }
        private static void DuQuKuaiJieJian()
        {
            CunDian = INI.GetKuaiJieJianList("存点", new KuaiJieJian_Type(KeyCode.Q));
            UP = INI.GetKuaiJieJianList("UP", KJ(KeyCode.Mouse2, KeyCode.Mouse3));
            IFG = INI.GetKuaiJieJianList("IFG", KJ(KeyCode.B, KeyCode.Mouse4));
            ZaiRuZiJi = INI.GetKuaiJieJianList("载入自己", new KuaiJieJian_Type(KeyCode.LeftControl, KeyCode.F));
            JiHe = INI.GetKuaiJieJianList("召集玩家", new KuaiJieJian_Type(KeyCode.F2));
            ChongZhiWuPin = INI.GetKuaiJieJianList("重置物品", new KuaiJieJian_Type(KeyCode.F3));
            ShangYiGuan = INI.GetKuaiJieJianList("上一关", new KuaiJieJian_Type(KeyCode.PageUp));
            XiaYiGuan = INI.GetKuaiJieJianList("下一关", new KuaiJieJian_Type(KeyCode.PageDown));
            ChuangJianFangJian = INI.GetKuaiJieJianList("创建房间", new KuaiJieJian_Type(KeyCode.F7));
            QuanYuanFeiTian = INI.GetKuaiJieJianList("全员飞天", new KuaiJieJian_Type(KeyCode.F4));
            QuanYuanChaoRen = INI.GetKuaiJieJianList("全员超人", new KuaiJieJian_Type(KeyCode.F5));
            QuanYuanShanXian = INI.GetKuaiJieJianList("全员闪现", new KuaiJieJian_Type(KeyCode.F6));
            QuanJiZiShi = INI.GetKuaiJieJianList("拳击姿势", new KuaiJieJian_Type(KeyCode.R));
            WuPinGuaJian = INI.GetKuaiJieJianList("物品挂件", new KuaiJieJian_Type(KeyCode.M));

            KongZhiFenShen = INI.GetKuaiJieJianList("控制分身", new KuaiJieJian_Type(KeyCode.LeftAlt));
            QieHuanFenShen = INI.GetKuaiJieJianList("切换分身", new KuaiJieJian_Type(KeyCode.LeftControl));
            ZhiDingFeiTian = INI.GetKuaiJieJianList("指定飞", new KuaiJieJian_Type(KeyCode.F));
            ZhiDingChaoRen = INI.GetKuaiJieJianList("指定超人", new KuaiJieJian_Type(KeyCode.C));
            ZhiDingShanXian = INI.GetKuaiJieJianList("指定闪现", new KuaiJieJian_Type(KeyCode.B));

            ChuanSong = INI.GetKuaiJieJianList("传送至", new KuaiJieJian_Type(KeyCode.Mouse0));
            XuanFu = INI.GetKuaiJieJianList("悬浮于", new KuaiJieJian_Type(KeyCode.Mouse1));
            QianShou = INI.GetKuaiJieJianList("牵手", new KuaiJieJian_Type(KeyCode.Z));
        }
        public static ShuZi_Type Is1(List<KuaiJieJian_Type> kuaijiejianList)
        {
            foreach (var kuaijiejian in kuaijiejianList)
            {
                bool fuzhujian = false;
                if (kuaijiejian.keyCode1 == KeyCode.None) continue;

                if (kuaijiejian.keyCode2 == KeyCode.None && Input.GetKey(kuaijiejian.keyCode1))
                {
                    fuzhujian = true;
                }
                else if (Input.GetKey(kuaijiejian.keyCode1) && Input.GetKey(kuaijiejian.keyCode2))
                {
                    fuzhujian = true;
                }

                if (fuzhujian)
                {
                    return ShuZiZuHe1();
                }
            }
            return new ShuZi_Type();
        }

        public static ShuZi_Type Is2(List<KuaiJieJian_Type> kuaijiejianList)
        {
            foreach (var kuaijiejian in kuaijiejianList)
            {
                bool fuzhujian = false;
                if (kuaijiejian.keyCode1 == KeyCode.None) continue;

                if (kuaijiejian.keyCode2 == KeyCode.None && Input.GetKey(kuaijiejian.keyCode1))
                {
                    fuzhujian = true;
                }
                else if (Input.GetKey(kuaijiejian.keyCode1) && Input.GetKey(kuaijiejian.keyCode2))
                {
                    fuzhujian = true;
                }

                if (fuzhujian)
                {
                    return ShuZiZuHe2();
                }
            }
            return new ShuZi_Type();
        }

        private static ShuZi_Type ShuZiZuHe1()
        {
            ShuZi_Type shuZi_Type = new ShuZi_Type();
            for (int i = 0; i < 10; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i) || Input.GetKeyDown(KeyCode.Keypad1 + i))
                {
                    if (i < Human.all.Count)
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
        public static bool Is(List<KuaiJieJian_Type> list)
        {
            if (list == null) return false;
            foreach (var kjj in list)
            {
                if (kjj.keyCode1 == KeyCode.None)
                    continue;

                if (kjj.keyCode2 == KeyCode.None && Input.GetKeyDown(kjj.keyCode1))
                    return true;

                if (Input.GetKey(kjj.keyCode1) && Input.GetKeyDown(kjj.keyCode2))
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
