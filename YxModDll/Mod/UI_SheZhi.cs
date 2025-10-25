﻿using Multiplayer;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YxModDll.Mod.Features;
using YxModDll.Patches;


namespace YxModDll.Mod
{
    internal class UI_SheZhi : MonoBehaviour
    {
        private static Vector2 xiang_scrollPosition;
        private static Vector2 neirong_scrollPosition;


        private static int xianglist_kuan = 128;
        public static int shezhiID = 0;
        public static string shezhiIdName = "定点设置";


        private static string[] shezhiNames = { "定点设置", "开房设置", "聊天设置", "快捷键设置", "UI显示设置", "游戏设置", "YxMod设置", "黑名单设置" };
        private static string[] daxiaoNames = { "固定", "渐变", "跳跃", "随机" };
        private static string[] yanseNames = { "固定", "渐变", "跳跃", "随机" };

        public static bool huisu;
        public static bool guanxing;
        public static bool baoLiuDangQianSuDu;
        public static bool q;
        public static bool se;
        public static bool faq;
        public static float gaodu ;
        public static int geshu;
        public static string tishiStr;
        //public static int globalgeshu;
        //public static string globaltishiStr;


        public static bool huicheliaotian;
        public static bool pingbiyansedaima;
    


        public static bool MingZiSheZhi;
        public static string MingZiStr;
        public static bool MingZiZiDingYi;
        public static string MingZiDaiMa;

        public static bool MingZiCuTi;
        public static bool MingZiXieTi;


        public static int MingZiDaXiaoID;
        public static string MingZiDaXiaoStr;
        public static int MingZiDaXiao;
        public static int MingZiJianBianDaXiao1;
        public static int MingZiJianBianDaXiao2;
        public static int MingZiSuiJiDaXiao1;
        public static int MingZiSuiJiDaXiao2;
        public static int MingZiTiaoYueDaXiao1;
        public static int MingZiTiaoYueDaXiao2;

        public static int MingZiYanSeID;
        public static string MingZiYanSeStr;
        public static string MingZiYanSe;
        public static string MingZiJianBianYanSe1;
        public static string MingZiJianBianYanSe2;
        public static int MingZiSuiJiYanSeLiangDu;
        public static string MingZiTiaoYueYanSe1;
        public static string MingZiTiaoYueYanSe2;


        public static bool FaYanSheZhi;
        public static bool FaYanCuTi;
        public static bool FaYanXieTi;
        public static int FaYanDaXiaoID;
        public static string FaYanDaXiaoStr;

        public static int FaYanDaXiao;
        public static int FaYanJianBianDaXiao1;
        public static int FaYanJianBianDaXiao2;
        public static int FaYanSuiJiDaXiao1;
        public static int FaYanSuiJiDaXiao2;
        public static int FaYanTiaoYueDaXiao1;
        public static int FaYanTiaoYueDaXiao2;

        public static int FaYanYanSeID;
        public static string FaYanYanSeStr;
        public static string FaYanYanSe;
        public static string FaYanJianBianYanSe1;
        public static string FaYanJianBianYanSe2;
        public static int FaYanSuiJiYanSeLiangDu;
        public static string FaYanTiaoYueYanSe1;
        public static string FaYanTiaoYueYanSe2;

        public static int FaYanColorType;
        public static ColorfulSpeek.GradientSeed FaYanSeed = default(ColorfulSpeek.GradientSeed);
        public static int MingZiColorType;
        public static ColorfulSpeek.GradientSeed MingZiSeed = default(ColorfulSpeek.GradientSeed);

        public static int CaiDanColorType;

        ///////黑名单设置
        public static bool heimingdanEnabled = false;
        public static List<string> heimingdanList = new List<string>();
        public static string newHeimingdanSteamID = "";
        public static Vector2 heimingdanScrollPosition;

        ///////开放设置
        private static string[] guajidongzuoNames = { "跌落", "睡觉", "气球", "坐下", "挂件" };
        public static string fangming;
        public static string datingming;
        public static int zuidarenshu;
        public static bool yaoqing;
        public static bool jinzhijiaru;
        public static bool suodingguanqia;
        public static bool pingbizhafang;

        public static string pingbici;
        public static int pingbizishu;
        public static float fayanjiange;
        public static int pingbicishu;

        public static bool guajitixing;
        public static int guajishijian;
        public static int guajidongzuoID;

        public static int xujiarishu ;


        ////////UI显示设置
        public static bool xianshimingzi;
        //public static bool xiaoditu;
        public static bool guanqiajindu;
        //public static bool anjian;
        //public static bool fps;
        public static bool wanjia;
        public static bool sudu;
        public static bool touxianmingzi;
        public static bool touxianliaotian;
        public static int touxianliaotianshichang;
        public static bool touxiankongzhianniu;

        ////////游戏设置
        public static bool quchuqidonghuamian;
        public static bool quchujiazaishibai;
        public static bool guanbidatingxiazai;
        public static bool simifangjian;
        public static bool danyexianshi;
        //public static bool danyepaixu;
        public static bool haoyoufangjian;
        public static bool dangqianrenshupaixu;
        public static bool skinCheckEnabled;
        public static bool skinUseRGB24Format;
        public static bool SkipTextureCompression;
        public static bool splitScreenEnabled;

        public static float playerCamDistance = 300f;
        public static float freeRoamCamDistance = 300f;

        public static string mingZiPreviewStr = "";
        public static string faYanPreviewStr = "";
        public static float lastRandomUpdateTime = -10f;

        void Update()
        {
            if (Time.time - lastRandomUpdateTime >= 1f)
            {
                lastRandomUpdateTime = Time.time;
                mingZiPreviewStr = Chat.SetDaXiaoYanSe(MingZiStr, MingZiDaXiaoID, MingZiYanSeID, true);
                faYanPreviewStr = Chat.SetDaXiaoYanSe("佩玉鸣鸾罢歌舞，妈妈带我吃红薯", FaYanDaXiaoID, FaYanYanSeID, false);
            }
        }
        public static void CreatUI()//创建菜单功能区
        {
            float kehuquHeight = UI_Main.shezhiUI_gao - UI_Windows.biaotiUiheight;
            float kehuquWeith = UI_Main.shezhiUI_kuan - UI_Windows.biankuangsize * 2;

            Rect xianglist = new Rect(0, 0, xianglist_kuan, kehuquHeight);
            UI.CreatUiBox(xianglist, UI_Windows.gongnengquTexture2);
            GUILayout.BeginArea(xianglist);
            xiang_scrollPosition = GUILayout.BeginScrollView(xiang_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域

            // 添加：对 shezhiNames 做颜色处理
            string[] colorfulNames = new string[shezhiNames.Length];
            for (int j = 0; j < shezhiNames.Length; j++)
            {
                string translated = UI.TranslateButtonText(shezhiNames[j]);
                colorfulNames[j] = ColorfulSpeek.colorshows(translated);
            }

            int i = GUILayout.SelectionGrid(shezhiID, colorfulNames, 1, UI.styleSelectionGrid());
            //if (shezhiID != i)
            //{
            shezhiID = i;
                shezhiIdName = shezhiNames[shezhiID];
                //Debug.Log(shezhiID + " " + shezhiIdName);
            //}

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            Rect thinglist = new Rect(xianglist_kuan + UI_Windows.biankuangsize, 0, kehuquWeith - xianglist_kuan - UI_Windows.biankuangsize, kehuquHeight);
            UI.CreatUiBox(thinglist, UI_Windows.gongnengquTexture2);
            GUILayout.BeginArea(thinglist);
            neirong_scrollPosition = GUILayout.BeginScrollView(neirong_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域
            //GUILayout.BeginHorizontal();
            //GUILayout.EndHorizontal();
            switch (shezhiIdName)
            {
                case "定点设置":
                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("Q定点", ref q, false, Q);
                    UI.CreatAnNiu_AnXia("SE定点", ref se, false, SE);
                    UI.CreatAnNiu_AnXia("发Q定点", ref faq, false, () => {
                        PlayerPrefs.SetInt("faqdingdian", faq ? 1 : 0);
                    }, "网络不佳时若常误触读点，建议开启此功能，通过发送 Q 指令实现定点。");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("回溯", ref huisu, false, HuiSu, "保留定点时的姿势");
                    UI.CreatAnNiu_AnXia("惯性", ref guanxing, false, GuanXing, "保留定点时的速度");
                    UI.CreatAnNiu_AnXia("保速", ref baoLiuDangQianSuDu, false, BaoSu, "保留读点时的速度");
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    UI.CreatFenGeXian();//分割线
                    GUILayout.Space(5);
                    UI.CreatShuZhi("定点高度", ref gaodu, 0.0f, 2.0f, 0.1f,GaoDu);
                    UI.CreatShuZhi("存点数量", ref geshu, 1, 10, 1,GeShu);
                    UI.CreatWenBenKuang("定点提示", ref tishiStr, 100, 210,TiShiStr);
                    GUILayout.Label(tishiStr, UI.SetLabelStyle_JuZhong());
                    //GUILayout.Space(5);
                    //UI.CreatFenGeXian();//分割线
                    //GUILayout.Space(5);
                    //GUILayout.Label(ColorfulSpeek.colorshows("他人设置：作为主机时其他玩家的设置"), UI.SetLabelStyle_JuZhong());
                    //UI.CreatShuZhi("存点数量", ref globalgeshu, 1, 10, 1, GlobalGeShu);
                    //UI.CreatWenBenKuang("定点提示", ref globaltishiStr, 100, 210, GlobalTiShiStr);
                    //GUILayout.Label(globaltishiStr, UI.SetLabelStyle_JuZhong());
                    GUILayout.Space(5);
                    UI.CreatFenGeXian();//分割线
                    GUILayout.Space(5);
                    GUILayout.Label("定点提示支持富文本格式", UI.SetLabelStyle_JuZhong());
                    GUILayout.Label("作为主机时，其他玩家将自动同步你的当前设置", UI.SetLabelStyle_JuZhong());
                    GUILayout.Label($"<i>仅房主或YxMod的房间有效</i>", UI.SetLabelStyle_JuZhong(), GUILayout.ExpandWidth(true));
                    break;
                case "聊天设置":
                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("回车聊天", ref huicheliaotian, false,HuiCheLiaoTian);
                    UI.CreatAnNiu_AnXia("屏蔽颜色代码", ref pingbiyansedaima, false,PingBiYanSeDaiMa);
                    //UI.CreatAnNiu_AnXia("屏蔽炸房", ref pingbizhafang, false);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    UI.CreatFenGeXian();//分割线
                    GUILayout.Space(5);
                    // 给 daxiaoNames 加颜色// 大小
                    string[] daxiaoNamesColored = new string[daxiaoNames.Length];
                    for (int i1 = 0; i1 < daxiaoNames.Length; i1++)
                    {
                        string translated = UI.TranslateButtonText(daxiaoNames[i1]); // 先翻译
                        daxiaoNamesColored[i1] = ColorfulSpeek.colorshows(translated); // 再上色
                    }

                    // 颜色
                    string[] yanseNamesColored = new string[yanseNames.Length];
                    for (int i2 = 0; i2 < yanseNames.Length; i2++)
                    {
                        string translated = UI.TranslateButtonText(yanseNames[i2]); // 先翻译
                        yanseNamesColored[i2] = ColorfulSpeek.colorshows(translated); // 再上色
                    }


                    UI.CreatAnNiu_AnXia("名字设置>>", ref MingZiSheZhi, false, SetMingZiSheZhi,"点按钮切换开关");
                    if (MingZiSheZhi)
                    {
                        UI.CreatAnNiu_AnXia("自定义代码", ref MingZiZiDingYi, false, SetMingZiZiDingYi);
                        if (MingZiZiDingYi)
                        {
                            UI.CreatWenBenKuang(null, ref MingZiDaiMa, 1000, 297, SetMingZiDaiMa);
                            GUILayout.Label(MingZiDaiMa, UI.SetLabelStyle_JuZhong());
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            UI.CreatWenBenKuang("修改名字", ref MingZiStr, 100, 140, SetMingZiStr);

                            UI.CreatAnNiu("还原", false, HuanYuanMingZi);
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();

                            UI.CreatAnNiu_AnXia("粗体", ref MingZiCuTi, false, SetMingZiCuTi);
                            UI.CreatAnNiu_AnXia("斜体", ref MingZiXieTi, false, SetMingZiXieTi);
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label(ColorfulSpeek.colorshows(UI.TranslateButtonText("大小设置")));

                            int daxiaoid = GUILayout.SelectionGrid(MingZiDaXiaoID, daxiaoNamesColored , 4, UI.styleSelectionGrid());
                            if (MingZiDaXiaoID != daxiaoid)
                            {
                                MingZiDaXiaoID = daxiaoid;
                                MingZiDaXiaoStr = daxiaoNames[MingZiDaXiaoID];
                                PlayerPrefs.SetInt("MingZiDaXiaoID", MingZiDaXiaoID);
                                //Debug.Log(MingZiDaXiaoID + " " + MingZiDaXiaoStr);
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            switch (MingZiDaXiaoStr)
                            {
                                case "固定":

                                    UI.CreatShuZhi("字号", ref MingZiDaXiao, 2, 30, 1, SetMingZiDaXiao);

                                    break;
                                case "渐变":
                                    UI.CreatShuZhi("开始", ref MingZiJianBianDaXiao1, 2, 30, 1, SetMingZiJianBianDaXiao1);
                                    GUILayout.FlexibleSpace();
                                    UI.CreatShuZhi("结束", ref MingZiJianBianDaXiao2, 2, 30, 1, SetMingZiJianBianDaXiao2);
                                    break;
                                case "随机":
                                    UI.CreatShuZhi("最小", ref MingZiSuiJiDaXiao1, 2, 30, 1, SetMingZiSuiJiDaXiao1);
                                    GUILayout.FlexibleSpace();
                                    UI.CreatShuZhi("最大", ref MingZiSuiJiDaXiao2, 2, 30, 1, SetMingZiSuiJiDaXiao2);
                                    break;
                                case "跳跃":
                                    UI.CreatShuZhi("字号1", ref MingZiTiaoYueDaXiao1, 2, 30, 1, SetMingZiTiaoYueDaXiao1);
                                    GUILayout.FlexibleSpace();
                                    UI.CreatShuZhi("字号2", ref MingZiTiaoYueDaXiao2, 2, 30, 1, SetMingZiTiaoYueDaXiao2);
                                    break;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(ColorfulSpeek.colorshows(UI.TranslateButtonText("颜色设置")));
                            int yanseid = GUILayout.SelectionGrid(MingZiYanSeID, yanseNamesColored, 4, UI.styleSelectionGrid());
                            if (MingZiYanSeID != yanseid)
                            {
                                MingZiYanSeID = yanseid;
                                MingZiYanSeStr = yanseNames[MingZiYanSeID];
                                PlayerPrefs.SetInt("MingZiYanSeID", MingZiYanSeID);
                                //Debug.Log(MingZiYanSeID + " " + MingZiYanSeStr);
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            switch (MingZiYanSeStr)
                            {
                                case "固定":
                                    UI.CreatYanSeKuang("色值", MingZiYanSe, SetMingZiYanSe);
                                    break;
                                case "渐变":
                                    //UI.CreatYanSeKuang("开始", MingZiJianBianYanSe1, SetMingZiJianBianYanSe1);
                                    //UI.CreatYanSeKuang("结束", MingZiJianBianYanSe2, SetMingZiJianBianYanSe2);
                                    UI.CreatShuZhi("名字渐变", ref MingZiColorType, 1, 363, 1, SetMingZiColorType);
                                    break;
                                case "随机":
                                    UI.CreatShuZhi("最低亮度", ref MingZiSuiJiYanSeLiangDu, 0, 95, 5, SetMingZiSuiJiYanSeLiangDu);
                                    break;
                                case "跳跃":
                                    UI.CreatYanSeKuang("色1", MingZiTiaoYueYanSe1, SetMingZiTiaoYueYanSe1);
                                    UI.CreatYanSeKuang("色2", MingZiTiaoYueYanSe2, SetMingZiTiaoYueYanSe2);
                                    break;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Label(mingZiPreviewStr, UI.SetLabelStyle_JuZhong());
                            //GUILayout.Label(Chat.SetDaXiaoYanSe(MingZiStr, MingZiDaXiaoID, MingZiYanSeID, true), UI.SetLabelStyle_JuZhong());
                        }
                    }
                    GUILayout.Space(5);
                    UI.CreatFenGeXian();//分割线
                    GUILayout.Space(5);
                    UI.CreatAnNiu_AnXia("发言设置>>", ref FaYanSheZhi, false, SetFaYanSheZhi, "点按钮切换开关");
                    if (FaYanSheZhi)
                    {
                        GUILayout.BeginHorizontal();
                        UI.CreatAnNiu_AnXia("粗体", ref FaYanCuTi, false, SetFaYanCuTi);
                        UI.CreatAnNiu_AnXia("斜体", ref FaYanXieTi, false, SetFaYanXieTi);
                        GUILayout.EndHorizontal();



                        GUILayout.BeginHorizontal();
                        GUILayout.Label(ColorfulSpeek.colorshows(UI.TranslateButtonText("大小设置")));

                        int daxiaoid = GUILayout.SelectionGrid(FaYanDaXiaoID, daxiaoNamesColored , 4, UI.styleSelectionGrid());
                        if (FaYanDaXiaoID != daxiaoid)
                        {
                            FaYanDaXiaoID = daxiaoid;
                            FaYanDaXiaoStr = daxiaoNames[FaYanDaXiaoID];
                            PlayerPrefs.SetInt("FaYanDaXiaoID", FaYanDaXiaoID);
                            //Debug.Log(FaYanDaXiaoID + " " + FaYanDaXiaoStr);
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        switch (FaYanDaXiaoStr)
                        {
                            case "固定":

                                UI.CreatShuZhi("字号", ref FaYanDaXiao, 2, 30, 1, SetFaYanDaXiao);
                                break;
                            case "渐变":
                                UI.CreatShuZhi("开始", ref FaYanJianBianDaXiao1, 2, 30, 1, SetFaYanJianBianDaXiao1);
                                GUILayout.FlexibleSpace();
                                UI.CreatShuZhi("结束", ref FaYanJianBianDaXiao2, 2, 30, 1, SetFaYanJianBianDaXiao2);
                                break;
                            case "随机":
                                UI.CreatShuZhi("最小", ref FaYanSuiJiDaXiao1, 2, 30, 1, SetFaYanSuiJiDaXiao1);
                                GUILayout.FlexibleSpace();
                                UI.CreatShuZhi("最大", ref FaYanSuiJiDaXiao2, 2, 30, 1, SetFaYanSuiJiDaXiao2);
                                break;
                            case "跳跃":
                                UI.CreatShuZhi("字号1", ref FaYanTiaoYueDaXiao1, 2, 30, 1, SetFaYanTiaoYueDaXiao1);
                                GUILayout.FlexibleSpace();
                                UI.CreatShuZhi("字号2", ref FaYanTiaoYueDaXiao2, 2, 30, 1, SetFaYanTiaoYueDaXiao2);
                                break;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(ColorfulSpeek.colorshows(UI.TranslateButtonText("颜色设置")));

                        int yanseid = GUILayout.SelectionGrid(FaYanYanSeID, yanseNamesColored, 4, UI.styleSelectionGrid());
                        if (FaYanYanSeID != yanseid)
                        {
                            FaYanYanSeID = yanseid;
                            FaYanYanSeStr = yanseNames[FaYanYanSeID];
                            PlayerPrefs.SetInt("FaYanYanSeID", FaYanYanSeID);
                            //Debug.Log(FaYanYanSeID + " " + FaYanYanSeStr);
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        switch (FaYanYanSeStr)
                        {
                            case "固定":
                                UI.CreatYanSeKuang("色值", FaYanYanSe, SetFaYanYanSe);
                                break;
                            case "渐变":
                                //UI.CreatYanSeKuang("开始", FaYanJianBianYanSe1, SetFaYanJianBianYanSe1);
                                //UI.CreatYanSeKuang("结束", FaYanJianBianYanSe2, SetFaYanJianBianYanSe2);
                                UI.CreatShuZhi("发言渐变", ref FaYanColorType, 1, 363, 1, SetFaYanColorType);
                                break;
                            case "随机":
                                UI.CreatShuZhi("最低亮度", ref FaYanSuiJiYanSeLiangDu, 0, 95, 5, SetFaYanSuiJiYanSeLiangDu);
                                break;
                            case "跳跃":
                                UI.CreatYanSeKuang("色1", FaYanTiaoYueYanSe1, SetFaYanTiaoYueYanSe1);
                                UI.CreatYanSeKuang("色2", FaYanTiaoYueYanSe2, SetFaYanTiaoYueYanSe2);
                                break;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Label(faYanPreviewStr, UI.SetLabelStyle_JuZhong());
                        //GUILayout.Label(Chat.SetDaXiaoYanSe("佩玉鸣鸾罢歌舞，妈妈带我吃红薯", FaYanDaXiaoID, FaYanYanSeID, false), UI.SetLabelStyle_JuZhong());
                    }
                    GUILayout.Space(5);
                    UI.CreatFenGeXian();//分割线
                    GUILayout.Space(5);
                    GUILayout.Label("提示：使用 /say [内容] 可发送无颜色消息，避免富文本干扰，如：/say dingdian 4", UI.SetLabelStyle_JuZhong());
                    break;
                case "开房设置":
                    //房名设置//最大人数//仅限邀请//锁定关卡//挂机提醒//房间外显最大人数和当前人数修改
                    //屏蔽炸房//防踢
                    UI.CreatWenBenKuang("房间名称", ref fangming, 100, 197,XiuGaiFangMing);
                    UI.CreatWenBenKuang("大厅名称", ref datingming, 1000, 197, XiuGaiDaTingMing);
                    GUILayout.Space(5);
                    UI.CreatFenGeXian();
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    UI.CreatShuZhi("玩家上限", ref zuidarenshu, 0, 99, 1,SetWanJiaShangXian);
                    UI.CreatAnNiu_AnXia("仅限邀请", ref yaoqing, false, SetJinXianYaoQing);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    UI.CreatShuZhi("虚假人数", ref xujiarishu, 0, 100,1,SetXuJiaRenShu);
                    UI.CreatAnNiu_AnXia("锁定关卡", ref suodingguanqia, false, SetSuoDingGuanQia);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("游戏中禁止加入", ref jinzhijiaru, false, SetJinZhiJiaRu);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                    UI.CreatFenGeXian();
                    GUILayout.Space(5);
                    UI.CreatAnNiu_AnXia("发言限制", ref pingbizhafang, false,SetPingBiZhaFang);
                    if(pingbizhafang)
                    {
                        UI.CreatWenBenKuang("屏蔽固定词", ref pingbici, 100, 180,SetPingBiCi);
                        UI.CreatShuZhi("发言字数限制(个)", ref pingbizishu, 0, 1000, 5,SetPingBiZiShu);
                        UI.CreatShuZhi("发言间隔限制(秒)", ref fayanjiange, 0.0f, 5.0f, 0.1f,SetFaYanJianGe);
                        UI.CreatShuZhi("发言重复次数限制(条)", ref pingbicishu, 0, 10, 1,SetPingBiCiShu);

                    }
                    GUILayout.Space(5);
                    UI.CreatFenGeXian();
                    GUILayout.Space(5);
                    UI.CreatAnNiu_AnXia("挂机提醒", ref guajitixing, false, SetGuaJiTiXing);
                    if (guajitixing)
                    {
                        UI.CreatShuZhi("无动作进入挂机(分钟)", ref guajishijian, 1, 10, 1,SetGuaJiShiJian);
                        GUILayout.BeginHorizontal();
                        // 彩色处理后的文本，去掉原来的 <size=16>
                        string coloredLabel = ColorfulSpeek.colorshows("动作");
                        GUILayout.Label(coloredLabel);

                        // guajidongzuoNames 数组里每个元素也要做颜色处理，先生成新数组
                        string[] colorfulDongzuoNames = new string[guajidongzuoNames.Length];
                        for (int i1 = 0; i1 < guajidongzuoNames.Length; i1++)
                        {
                            string translated = UI.TranslateButtonText(guajidongzuoNames[i1]); // 先翻译
                            colorfulDongzuoNames[i1] = ColorfulSpeek.colorshows(translated); // 再上色
                        }

                        int dongzuoid = GUILayout.SelectionGrid(guajidongzuoID, colorfulDongzuoNames, 5, UI.styleSelectionGrid());

                        if (guajidongzuoID != dongzuoid)
                        {
                            guajidongzuoID = dongzuoid;
                            PlayerPrefs.SetInt("guajidongzuoID", guajidongzuoID);

                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(5);
                    UI.CreatFenGeXian();
                    GUILayout.Space(5);
                    GUILayout.Label($"<i>仅房主有效</i>", UI.SetLabelStyle_JuZhong(), GUILayout.ExpandWidth(true));
                    GUILayout.Space(5);
                    break;
                case "快捷键设置":
                    HotKey.Draw();
                    break;
                case "UI显示设置":
                    UI.CreatAnNiu_AnXia("一直显示名字", ref xianshimingzi, false,XianShiMingZi);
                    //UI.CreatAnNiu_AnXia("头显聊天内容", ref touxianliaotian, false);
                    //if (touxianliaotian)
                    //{
                    //    UI.CreatShuZhi("显示时长(秒)", ref touxianliaotianshichang, 5, 100, 5);
                    //}
                    //UI.CreatAnNiu_AnXia("头显控制按钮", ref touxiankongzhianniu, false);
                    UI.CreatAnNiu_AnXia("显示FPS", ref FPSCounter.showFPS , false,XianShiFPS);

                    //UI.CreatAnNiu_AnXia("显示关卡进度", ref guanqiajindu, false);
                    UI.CreatAnNiu_AnXia("显示小地图", ref MiniMap.instance.SetMiniMap, false,XianShiXiaoDiTu);
                    UI.CreatAnNiu_AnXia("显示按键信息", ref KeyDisplayUI.showKeys, false,XianShiAnJian);

                    //UI.CreatAnNiu_AnXia("显示玩家信息", ref wanjia, false);
                    //UI.CreatAnNiu_AnXia("显示玩家速度", ref sudu, false);
                    UI.CreatAnNiu_AnXia("菜单颜色>>", ref ColorfulSpeek.Gradual_Change, false, SetCaiDanYanShe);
                    if (ColorfulSpeek.Gradual_Change)
                    {
                        UI.CreatShuZhi("菜单渐变", ref CaiDanColorType, 0, 363, 1, SetCaiDanColorType);
                    }
                    break;
                case "游戏设置":
                    //去除启动画面
                    //去除工坊文件加载失败
                    //关闭大厅下载
                    //显示私密房间
                    //所有房间单页显示
                    //房间排序:YxMod房间-好友房间-公开房间-私密房间   按照房间当前人数排序/总人数排序
                    UI.CreatAnNiu_AnXia("去除游戏启动画面", ref quchuqidonghuamian, false,QiDongHuaMian);
                    UI.CreatAnNiu_AnXia("去除没订阅地图文件加载失败", ref quchujiazaishibai, false,JiaZaiShiBai);
                    UI.CreatAnNiu_AnXia("关闭大厅下载", ref guanbidatingxiazai, false,GuanBiDaTingXiaZai);
                    UI.CreatAnNiu_AnXia("房间列表按照当前玩家数量降序显示", ref dangqianrenshupaixu, false,FangJianPaiXu);
                    UI.CreatAnNiu_AnXia("显示私密房间(用 * 标识)", ref simifangjian, false,SiMiFangJian);
                    UI.CreatAnNiu_AnXia("房间列表单页显示", ref danyexianshi, false,DanYeXianShi);
                    if (danyexianshi)
                    {
                        UI.CreatAnNiu_AnXia("好友房间(用 @ 标识)", ref haoyoufangjian, false,HaoYouFangJian);
                        //UI.CreatAnNiu_AnXia("按照“YxMod - 好友 - 公开 - 私密”排序", ref danyepaixu, false);
                    }
                    UI.CreatAnNiu_AnXia("降低贴图分辨率", ref skinCheckEnabled, false, SaveSkinCheckSetting,
                        "将 2048×2048 的皮肤压缩为 1024×1024，减少约 75% 的内存占用，有助于降低内存溢出风险");

                    UI.CreatAnNiu_AnXia("修改贴图格式", ref skinUseRGB24Format, false, () =>
                    {
                        PlayerPrefs.SetInt("skinUseRGB24Format", skinUseRGB24Format ? 1 : 0);
                    },"将贴图格式更改为 RGB24（不支持透明像素），可减少约 25% 的内存占用。可与分辨率压缩叠加，总体节省约 81.25%。重启生效");
                    UI.CreatAnNiu_AnXia("跳过贴图压缩", ref SkipTextureCompression, false, () =>
                    {
                        PlayerPrefs.SetInt("SkipTextureCompression", SkipTextureCompression ? 1 : 0);
                    }, "跳过贴图压缩，减少 CPU 和内存占用，降低崩溃风险。虽增加 GPU 负担，但更稳定。关闭恢复默认。");

                    UI.CreatAnNiu_AnXia("清除Bug玩家", ref FeatureManager.removeBugHuman, false, () =>
                    {
                        PlayerPrefs.SetInt("removeBugHuman", FeatureManager.removeBugHuman ? 1 : 0);
                    },"移除异常玩家对象。通常因为网络问题没有正确加入房间，导致主机信息和玩家信息对不上。比如头顶ID不一致，同时接收两个房间的消息。");
                    UI.CreatAnNiu_AnXia("分身时分屏显示", ref splitScreenEnabled, false, () =>
                    {
                        PlayerPrefs.SetInt("splitScreenEnabled", splitScreenEnabled ? 1 : 0);
                    }, "控制分身是否分屏显示");
                    UI.CreatAnNiu_AnXia("显示目标距离", ref FeatureManager.xrayPlayers, false, null, "显示玩家距离、存档点距离");
                    UI.CreatShuZhi("玩家视距", ref playerCamDistance, 100f, 20000f, 100f, () =>
                    {
                        FeatureManager.MainCamera.farClipPlane = UI_SheZhi.playerCamDistance;
                    }, yuan: 300f);
                    UI.CreatShuZhi("F8视距", ref freeRoamCamDistance, 100f, 20000f, 100f, () =>
                    {
                        PlayerPrefs.SetFloat("freeRoamCamDistance", freeRoamCamDistance);
                    }, yuan: 300f);
                    GUILayout.Space(5);
                    UI.CreatFenGeXian();
                    GUILayout.Space(5);
                    GUILayout.Label("提示：人类为32位程序，最多只能使用2GB内存，内存满后必然崩溃。建议地图大小不要超过300MB。每位玩家的皮肤大约包含2-4张2048分辨率的图片，加载后会占用约44-88MB内存。", UI.SetLabelStyle_JuZhong());
                    break;
                case "YxMod设置":
                    //呼出YxMod界面时人物不动
                    //显示鼠标方式:按一下Alt      长按Alt
                    //鼠标显示时人物不动
                    //YxMod界面透明度

                    //新进入的YxMod玩家默认打开客机权限
                    //没有客机权限的YxMod玩家可以控制自己
                    //禁止客机玩家控制我(同时你也无法控制其他YxMod玩家)
                    //房主不受客机玩家控制(自己开房有效)
                    //

                    UI.CreatAnNiu_AnXia("按Shift键显示鼠标", ref shift_xianshishubiao, false, Shift_XianShiShuBiao);
                    UI.CreatAnNiu_AnXia("显示鼠标时人物不可控", ref noKong_xianshishubiao, false, NoKong_XianShiShuBiao);
                    UI.CreatAnNiu_AnXia("房主时默认打开客机权限", ref morenkejiquanxian, false, MoRenKeJiQuanXian, "开启后，所有人默认拥有房主权限。为避免混乱，建议关闭");
                    UI.CreatAnNiu_AnXia("房主时不受客机玩家控制", ref fangzhububeikong, false, FangZhuBuBeiKong);
                    UI.CreatAnNiu_AnXia("客机时禁止其他客机控制我", ref jinzhibeikong, false, JinZhiBeiKong, "开启后，其他客机无法控制你，连自己也不行。如需控制自己，请关闭此选项");
                
                    break;
                case "黑名单设置":
                    // 标题
                    GUILayout.Label(ColorfulSpeek.colorshows(UI.TranslateButtonText("黑名单设置")), UI.SetLabelStyle_JuZhong());

                    // 防御性初始化
                    if (heimingdanList == null) heimingdanList = new List<string>();

                    // 启用/禁用
                    UI.CreatAnNiu_AnXia("启用黑名单", ref heimingdanEnabled, false, () => {
                        HeiMingDan.Enabled = heimingdanEnabled;
                        HeiMingDan.SaveHeiMingDan();
                    });

                    GUILayout.Space(5);
                    UI.CreatFenGeXian();
                    GUILayout.Space(5);

                    // 添加条目（改为纵向排列，文字与按钮换行）
                    GUILayout.Label(UI.TranslateButtonText("添加SteamID"), UI.SetLabelStyle_JuZhong());
                    UI.CreatWenBenKuang(null, ref newHeimingdanSteamID, 100, 280, null);
                    GUILayout.Space(3);
                    UI.CreatAnNiu("添加", false, () => {
                        if (HeiMingDan.AddPlayer(newHeimingdanSteamID))
                        {
                            heimingdanList = new List<string>(HeiMingDan.SteamIDs);
                            newHeimingdanSteamID = string.Empty;
                        }
                    });
                    GUILayout.Space(3);
                    UI.CreatAnNiu("查看个人资料", false, () => {
                        HeiMingDan.OpenSteamProfile(newHeimingdanSteamID);
                    });

                    GUILayout.Space(5);
                    UI.CreatFenGeXian();
                    GUILayout.Space(5);

                    // 列表（提高可视高度）
                    using (UI.ScrollView(ref heimingdanScrollPosition, GUILayout.Height(160)))
                    {
                        foreach (var sid in heimingdanList.ToArray())
                        {
                            // 每条目分两行：先显示ID，再显示按钮（换行）
                            GUILayout.Label(sid, UI.SetLabelStyle_JuZhong());
                            GUILayout.BeginHorizontal();
                            UI.CreatAnNiu("查看", false, () => HeiMingDan.OpenSteamProfile(sid));
                            GUILayout.Space(6);
                            UI.CreatAnNiu("移除", false, () => {
                                if (HeiMingDan.RemovePlayer(sid))
                                {
                                    heimingdanList = new List<string>(HeiMingDan.SteamIDs);
                                }
                            });
                            GUILayout.EndHorizontal();
                            GUILayout.Space(4);
                        }
                    }

                    GUILayout.Space(5);
                    UI.CreatFenGeXian();
                    GUILayout.Space(5);

                    // 帮助（缩小字号且自动换行）
                    GUILayout.Label(HeiMingDan.GetSteamIDHelpText(), UI.SetLabelStyle_JuZhong());
                    break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public static void Shift_XianShiShuBiao()
        {
            PlayerPrefs.SetInt("shift_xianshishubiao", shift_xianshishubiao ? 1 : 0);
        }
        //public static void NoKong_HuChu()
        //{
        //    PlayerPrefs.SetInt("noKong_huchu", noKong_huchu ? 1 : 0);
        //}
        public static void NoKong_XianShiShuBiao()
        {
            PlayerPrefs.SetInt("noKong_xianshishubiao", noKong_xianshishubiao ? 1 : 0);
        }
        public static void MoRenKeJiQuanXian()
        {
            PlayerPrefs.SetInt("morenkejiquanxian", morenkejiquanxian ? 1 : 0);
        }
        //public static void KongZhiZiji()
        //{
        //    PlayerPrefs.SetInt("kongzhiziji", kongzhiziji ? 1 : 0);
        //}
        public static void JinZhiBeiKong()
        {
            if(NetGame.isClient)
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("jinzhibeikong"), jinzhibeikong ? "1" : "0");
            }
        
            PlayerPrefs.SetInt("jinzhibeikong", jinzhibeikong ? 1 : 0);
        }
        public static void FangZhuBuBeiKong()
        {
            PlayerPrefs.SetInt("fangzhububeikong", fangzhububeikong ? 1 : 0);
        }



        public static void ChuShiHua()
        {
            huisu = PlayerPrefs.GetInt("huisudingdian", 0) > 0;
            guanxing = PlayerPrefs.GetInt("guanxingdingdian", 0) > 0;
            baoLiuDangQianSuDu = PlayerPrefs.GetInt("baosudingdian", 0) > 0;
            q = PlayerPrefs.GetInt("qdingdian", 1) > 0;
            se = PlayerPrefs.GetInt("sedingdian", 1) > 0;
            faq = PlayerPrefs.GetInt("faqdingdian", 0) > 0;
            gaodu = PlayerPrefs.GetFloat("dingdiangaodu", 0.2f);
            geshu = PlayerPrefs.GetInt("dingdiangeshu", 1);
            tishiStr = PlayerPrefs.GetString("dingdiantishi", "已存点");
            //globalgeshu = PlayerPrefs.GetInt("globaldingdiangeshu", 2);
            //globaltishiStr = PlayerPrefs.GetString("globaldingdiantishi", "已存点");

            huicheliaotian = PlayerPrefs.GetInt("huicheliaotian", 1) > 0;
            pingbiyansedaima = PlayerPrefs.GetInt("pingbiyansedaima", 0) > 0;

            MingZiSheZhi = PlayerPrefs.GetInt("MingZiSheZhi", 1) > 0;
            MingZiStr = PlayerPrefs.GetString("MingZiStr", SteamFriends.GetPersonaName());     /////读取  名字
            MingZiZiDingYi = PlayerPrefs.GetInt("MingZiZiDingYi", 0) > 0;
            MingZiCuTi = PlayerPrefs.GetInt("MingZiCuTi", 1) > 0;
            MingZiXieTi = PlayerPrefs.GetInt("MingZiXieTi", 0) > 0;
            MingZiDaiMa = PlayerPrefs.GetString("MingZiDaiMa", "<size=16><color=#FF0000>Y</color></size>" +
                                                               "<size=18><color=#00FF00>X</color></size>" +
                                                               "<size=20><color=#0000FF>M</color></size>" +
                                                               "<size=22><color=#FF0000>O</color></size>" +
                                                               "<size=24><color=#00FF00>D</color></size>" +
                                                               "<size=26><color=#0000FF>°</color></size>");
       
            MingZiDaXiao = PlayerPrefs.GetInt("MingZiDaXiao", 14);
            MingZiDaXiaoID = PlayerPrefs.GetInt("MingZiDaXiaoID", 2);
            MingZiDaXiaoStr= daxiaoNames[MingZiDaXiaoID];
            MingZiJianBianDaXiao1 = PlayerPrefs.GetInt("MingZiJianBianDaXiao1", 13);
            MingZiJianBianDaXiao2 = PlayerPrefs.GetInt("MingZiJianBianDaXiao2", 20);
            MingZiSuiJiDaXiao1 = PlayerPrefs.GetInt("MingZiSuiJiDaXiao1", 10);
            MingZiSuiJiDaXiao2 = PlayerPrefs.GetInt("MingZiSuiJiDaXiao2", 25);
            MingZiTiaoYueDaXiao1 = PlayerPrefs.GetInt("MingZiTiaoYueDaXiao1", 15);
            MingZiTiaoYueDaXiao2 = PlayerPrefs.GetInt("MingZiTiaoYueDaXiao2", 19);

            MingZiYanSe = PlayerPrefs.GetString("MingZiYanSe", "#FF00FF");
            MingZiYanSeID = PlayerPrefs.GetInt("MingZiYanSeID", 1);
            MingZiYanSeStr = yanseNames[MingZiYanSeID];
            MingZiJianBianYanSe1 = PlayerPrefs.GetString("MingZiJianBianYanSe1","#FF00FF");
            MingZiJianBianYanSe2 = PlayerPrefs.GetString("MingZiJianBianYanSe2", "#9F2EFF");
            MingZiSuiJiYanSeLiangDu = PlayerPrefs.GetInt("MingZiSuiJiYanSeLiangDu", 70);
            MingZiTiaoYueYanSe1 = PlayerPrefs.GetString("MingZiTiaoYueYanSe1", "#FF0000");
            MingZiTiaoYueYanSe2 = PlayerPrefs.GetString("MingZiTiaoYueYanSe2", "#0000FF");


            MingZiColorType = PlayerPrefs.GetInt("MingZiColorType", 43);
            FaYanColorType = PlayerPrefs.GetInt("FaYanColorType", 43);
            CaiDanColorType = PlayerPrefs.GetInt("Gradual_Num", 43);
            FaYanSeed = ColorfulSpeek.ins.GetSeedByIndex(FaYanColorType, 0f, 1f);
            MingZiSeed = ColorfulSpeek.ins.GetSeedByIndex(MingZiColorType, 0f, 1f);
            ColorfulSpeek.WordSeed = ColorfulSpeek.ins.GetSeedByIndex(CaiDanColorType, 0f, 1f);


            FaYanSheZhi = PlayerPrefs.GetInt("FaYanSheZhi", 1) > 0;
            FaYanCuTi = PlayerPrefs.GetInt("FaYanCuTi", 0) > 0;
            FaYanXieTi = PlayerPrefs.GetInt("FaYanXieTi", 1) > 0;

            FaYanDaXiao = PlayerPrefs.GetInt("FaYanDaXiao", 14);
            FaYanDaXiaoID = PlayerPrefs.GetInt("FaYanDaXiaoID", 3);
            FaYanDaXiaoStr = daxiaoNames[FaYanDaXiaoID];
            FaYanJianBianDaXiao1 = PlayerPrefs.GetInt("FaYanJianBianDaXiao1", 13);
            FaYanJianBianDaXiao2 = PlayerPrefs.GetInt("FaYanJianBianDaXiao2", 20);
            FaYanSuiJiDaXiao1 = PlayerPrefs.GetInt("FaYanSuiJiDaXiao1", 10);
            FaYanSuiJiDaXiao2 = PlayerPrefs.GetInt("FaYanSuiJiDaXiao2", 25);
            FaYanTiaoYueDaXiao1 = PlayerPrefs.GetInt("FaYanTiaoYueDaXiao1", 15);
            FaYanTiaoYueDaXiao2 = PlayerPrefs.GetInt("FaYanTiaoYueDaXiao2", 19);

            FaYanYanSe = PlayerPrefs.GetString("FaYanYanSe", "#FF00FF");
            FaYanYanSeID = PlayerPrefs.GetInt("FaYanYanSeID", 3);
            FaYanYanSeStr = yanseNames[FaYanYanSeID];
            FaYanJianBianYanSe1 = PlayerPrefs.GetString("FaYanJianBianYanSe1", "#FF00FF");
            FaYanJianBianYanSe2 = PlayerPrefs.GetString("FaYanJianBianYanSe2", "#9F2EFF");
            FaYanSuiJiYanSeLiangDu = PlayerPrefs.GetInt("FaYanSuiJiYanSeLiangDu", 70);
            FaYanTiaoYueYanSe1 = PlayerPrefs.GetString("FaYanTiaoYueYanSe1", "#FF0000");
            FaYanTiaoYueYanSe2 = PlayerPrefs.GetString("FaYanTiaoYueYanSe2", "#0000FF");

            //开放设置
            fangming = PlayerPrefs.GetString("fangming", SteamFriends.GetPersonaName());
            datingming = PlayerPrefs.GetString("datingming", SteamFriends.GetPersonaName());
            zuidarenshu = PlayerPrefs.GetInt("zuidarenshu", 10);
            SetMaxPlayers(zuidarenshu);
            xujiarishu = PlayerPrefs.GetInt("xujiarishu", 20);
            yaoqing = PlayerPrefs.GetInt("yaoqing", 0) > 0;
            suodingguanqia = PlayerPrefs.GetInt("suodingguanqia", 0) > 0;
            jinzhijiaru = PlayerPrefs.GetInt("jinzhijiaru", 0) > 0;
            pingbizhafang = PlayerPrefs.GetInt("pingbizhafang", 0) > 0;
            pingbici = PlayerPrefs.GetString("pingbici", "大佬炸房|闲人速退|■");
            pingbizishu = PlayerPrefs.GetInt("pingbizishu", 150);
            pingbicishu = PlayerPrefs.GetInt("pingbicishu", 5);
            fayanjiange = PlayerPrefs.GetFloat("fayanjiange", 0.5f);

            guajitixing = PlayerPrefs.GetInt("guajitixing", 1) > 0;
            guajishijian = PlayerPrefs.GetInt("guajishijian", 5);
            guajidongzuoID = PlayerPrefs.GetInt("guajidongzuoID", 1);

            // 黑名单（从文件加载到UI状态）
            heimingdanEnabled = HeiMingDan.Enabled;
            heimingdanList = new List<string>(HeiMingDan.SteamIDs);

            //游戏设置
            quchuqidonghuamian = PlayerPrefs.GetInt("quchuqidonghuamian", 1) > 0;
            quchujiazaishibai = PlayerPrefs.GetInt("quchujiazaishibai", 1) > 0;
            guanbidatingxiazai = PlayerPrefs.GetInt("guanbidatingxiazai", 1) > 0;
            simifangjian = PlayerPrefs.GetInt("simifangjian", 1) > 0;
            dangqianrenshupaixu = PlayerPrefs.GetInt("dangqianrenshupaixu", 1) > 0;
            danyexianshi = PlayerPrefs.GetInt("danyexianshi", 1) > 0;
            haoyoufangjian = PlayerPrefs.GetInt("haoyoufangjian", 1) > 0;
            skinCheckEnabled = PlayerPrefs.GetInt("skinCheckEnabled", 1) > 0;
            skinUseRGB24Format = PlayerPrefs.GetInt("skinUseRGB24Format", 1) > 0;
            SkipTextureCompression = PlayerPrefs.GetInt("SkipTextureCompression", 1) > 0;
            splitScreenEnabled = PlayerPrefs.GetInt("splitScreenEnabled", 1) > 0;
            freeRoamCamDistance = PlayerPrefs.GetFloat("freeRoamCamDistance", 300f);

            //显示设置
            KeyDisplayUI.showKeys = PlayerPrefs.GetInt("showKeys", 0) > 0;
            FPSCounter. showFPS = PlayerPrefs.GetInt("showFPS", 1) > 0;
            xianshimingzi = PlayerPrefs.GetInt("xianshimingzi", 1) > 0;
            MiniMap.instance.SetMiniMap = PlayerPrefs.GetInt("SetMiniMap", 0) > 0;//显示小地图

            //yxmod设置
            shift_xianshishubiao = PlayerPrefs.GetInt("shift_xianshishubiao", 1) > 0;
            //noKong_huchu = PlayerPrefs.GetInt("noKong_huchu", 1) > 0;
            noKong_xianshishubiao = PlayerPrefs.GetInt("noKong_xianshishubiao", 1) > 0;
            morenkejiquanxian = PlayerPrefs.GetInt("morenkejiquanxian", 1) > 0;
            //kongzhiziji = PlayerPrefs.GetInt("kongzhiziji", 1) > 0;
            jinzhibeikong = PlayerPrefs.GetInt("jinzhibeikong", 0) > 0;
            fangzhububeikong = PlayerPrefs.GetInt("fangzhububeikong", 0) > 0;




            //Debug.Log("ChuShiHua");
        }
        public static bool shift_xianshishubiao=true;
        //public static bool noKong_huchu = true;
        public static bool noKong_xianshishubiao = true;
        public static bool morenkejiquanxian = true;
        //public static bool kongzhiziji = true;
        public static bool jinzhibeikong;
        public static bool fangzhububeikong;

        public static void KeJiChuShiHua() ///在进入房间时,初始化
        {
            if (NetGame.isClient && YxMod.YxModServer)
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("huisudingdian"), huisu ? "1" : "0");
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("guanxingdingdian"), guanxing ? "1" : "0");
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("baosudingdian"), baoLiuDangQianSuDu ? "1" : "0");
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("qdingdian"), q ? "1" : "0");
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("sedingdian"), se ? "1" : "0");
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("dingdiangaodu"), $"{gaodu}");
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("dingdiangeshu"), $"{geshu}");
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("dingdiantishi"), tishiStr);
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("jinzhibeikong"), jinzhibeikong?"1":"0");
                //Debug.Log("KeJiChuShiHua");
            }
        }

        public static void XianShiXiaoDiTu()
        {
            PlayerPrefs.SetInt("SetMiniMap", MiniMap.instance.SetMiniMap ? 1 : 0);
        
        }


        public static void XianShiMingZi()
        {
            //if (xianshimingzi)
            //{
            //    AlwaysViewPlayerName.SetAlways();
            //}
            //else
            //{
            //    AlwaysViewPlayerName.RemoveAlways();
            //}

            PlayerPrefs.SetInt("xianshimingzi", xianshimingzi ? 1 : 0);
        }
        public static void XianShiFPS()
        {
            PlayerPrefs.SetInt("showFPS", FPSCounter.showFPS ? 1 : 0);
        }

        public static void XianShiAnJian()
        {
            //string msg = (KeyDisplayUI.showKeys ? "显示按键信息已开启 快捷键为Shift+G" : "显示按键信息已关闭");
            //Chat.TiShi(NetGame.instance.local, msg);
            PlayerPrefs.SetInt("showKeys", KeyDisplayUI.showKeys ? 1 : 0);
        }

        public static void QiDongHuaMian()
        {
            PlayerPrefs.SetInt("quchuqidonghuamian", quchuqidonghuamian ? 1 : 0);
        }
        public static void JiaZaiShiBai()
        {
            PlayerPrefs.SetInt("quchujiazaishibai", quchujiazaishibai ? 1 : 0);
        }
        public static void GuanBiDaTingXiaZai()
        {
            PlayerPrefs.SetInt("guanbidatingxiazai", guanbidatingxiazai ? 1 : 0);
        }
        public static void FangJianPaiXu()
        {
            PlayerPrefs.SetInt("dangqianrenshupaixu", dangqianrenshupaixu ? 1 : 0);
        }
        public static void SiMiFangJian()
        {
            PlayerPrefs.SetInt("simifangjian", simifangjian ? 1 : 0);
        }
        public static void DanYeXianShi()
        {
            PlayerPrefs.SetInt("danyexianshi", danyexianshi ? 1 : 0);
        }
        public static void HaoYouFangJian()
        {
            PlayerPrefs.SetInt("haoyoufangjian", haoyoufangjian ? 1 : 0);
        }

        public static void SaveSkinCheckSetting()
        {
            PlayerPrefs.SetInt("skinCheckEnabled", skinCheckEnabled ? 1 : 0);
        }

        public static void Skip_Start()
        {
            if (App.state == AppSate.Startup)
            {
                try
                {
                    StartupExperienceController.instance.SkipStartupExperience(null);
                }
                catch { }
            }
        }

        public static void SetPingBiZhaFang()
        {
            PlayerPrefs.SetInt("pingbizhafang", pingbizhafang ? 1 : 0);
            if (NetGame.isServer)
            {
                Chat.TiShi($"屏蔽炸房已 {(pingbizhafang ? "打开" : "关闭")}", TiShiMsgId.XiTongTiShi);
            }
            else if (NetGame.isClient)
            {
                Chat.TiShi(NetGame.instance.local, $"屏蔽炸房已 {(pingbizhafang ? "打开" : "关闭")}，仅房主有效");
            }
        }

        public static void SetGuaJiTiXing()
        {
            PlayerPrefs.SetInt("guajitixing", guajitixing ? 1 : 0);
            if (NetGame.isServer)
            {
                Chat.TiShi($"挂机提醒已 {(guajitixing ? "打开" : "关闭")}", TiShiMsgId.XiTongTiShi);
            }
            else if (NetGame.isClient)
            {
                Chat.TiShi(NetGame.instance.local, $"挂机提醒已 {(guajitixing ? "打开" : "关闭")}，仅房主有效");
            }
        }

        public static void SetGuaJiShiJian()
        {
            PlayerPrefs.SetInt("guajishijian", guajishijian );
        }


        public static void SetPingBiCi()
        {
            PlayerPrefs.SetString("pingbici", pingbici);
        }
        public static void SetFaYanJianGe()
        {
            PlayerPrefs.SetFloat("fayanjiange", fayanjiange);
        }
        public static void SetPingBiCiShu()
        {
            PlayerPrefs.SetInt("pingbicishu", pingbicishu);
        }
        public static void SetPingBiZiShu()
        {
            PlayerPrefs.SetInt("pingbizishu", pingbizishu);
        }

        public static void XiuGaiFangMing()
        {
            NetTransportSteam component = NetGame.instance.GetComponent<NetTransportSteam>();
            SteamMatchmaking.SetLobbyData(component.lobbyID, "name", $"★{UI_SheZhi.fangming}");///修改房名
            PlayerPrefs.SetString("fangming", fangming);
        }

        public static void XiuGaiDaTingMing()
        {
            NetTransportSteam component = NetGame.instance.GetComponent<NetTransportSteam>();

            if (component != null)
            {
                // 修改 Lobby 数据中的 "ll" 字段（你自定义的大厅名键）
                SteamMatchmaking.SetLobbyData(component.lobbyID, "ll", $"{UI_SheZhi.datingming}");
                PlayerPrefs.SetString("datingming", datingming);

                // 提示玩家
                NetChat.Print("大厅名称已修改为：" + $"{UI_SheZhi.datingming}");
            }
        }
        public static void SetJinZhiJiaRu()///参加正在进行的游戏
        {
            PlayerPrefs.SetInt("jinzhijiaru", jinzhijiaru ? 1 : 0);
            new MultiplayerLobbySettingsMenu().JoinInProgressChanged(jinzhijiaru ? 0 : 1);
            if (NetGame.isServer)
            {
                Chat.TiShi($"游戏中禁止加入已 {(jinzhijiaru ? "打开" : "关闭")}", TiShiMsgId.XiTongTiShi);
            }
            else if (NetGame.isClient)
            {
                Chat.TiShi(NetGame.instance.local, $"游戏中禁止加入已 {(jinzhijiaru ? "打开" : "关闭")}，仅房主有效");
            }
        }
        public static void SetJinXianYaoQing()
        {
            PlayerPrefs.SetInt("yaoqing", yaoqing ? 1 : 0);
            new MultiplayerLobbySettingsMenu().InviteOnlyChanged(yaoqing?1:0);
            if (NetGame.isServer)
            {
                Chat.TiShi($"仅限邀请已 {(yaoqing?"打开":"关闭")}", TiShiMsgId.XiTongTiShi);
            }
            else if (NetGame.isClient)
            {
                Chat.TiShi(NetGame.instance.local, $"仅限邀请已 {(yaoqing ? "打开" : "关闭")}，仅房主有效");
            }
        }
        public static void SetSuoDingGuanQia()
        {
            PlayerPrefs.SetInt("suodingguanqia", suodingguanqia ? 1 : 0);
            new MultiplayerLobbySettingsMenu().LevelProgressChanged(suodingguanqia ? 1 : 0);
            if (NetGame.isServer)
            {
                Chat.TiShi($"锁定关卡已 {(suodingguanqia ? "打开" : "关闭")}", TiShiMsgId.XiTongTiShi);
            }
            else if (NetGame.isClient)
            {
                Chat.TiShi(NetGame.instance.local, $"锁定关卡已 {(suodingguanqia ? "打开" : "关闭")}，仅房主有效");
            }
        }
        public static void SetWanJiaShangXian()
        {
            SetMaxPlayers(zuidarenshu);
            PlayerPrefs.SetInt("zuidarenshu", zuidarenshu);
        
            if (NetGame.isServer)
            {
                Chat.TiShi($"玩家上限修改为：{zuidarenshu}", TiShiMsgId.XiTongTiShi);
            }
            else if (NetGame.isClient)
            {
                Chat.TiShi(NetGame.instance.local, $"玩家上限修改为：{zuidarenshu}，仅房主有效");
            }
        }
        private static void SetXuJiaRenShu()
        {
            NetTransportSteam component = NetGame.instance.GetComponent<NetTransportSteam>();
            SteamMatchmaking.SetLobbyData(component.lobbyID, "mp", (UI_SheZhi.xujiarishu + Options.lobbyMaxPlayers).ToString());///虚假最大人数
            //Debug.Log(Human.all.Count);
            SteamMatchmaking.SetLobbyData(component.lobbyID, "cp", (UI_SheZhi.xujiarishu + Human.all.Count-1).ToString());
            PlayerPrefs.SetInt("xujiarishu", xujiarishu);

        }
        private static void SetMaxPlayers(int num)
        {
            new MultiplayerLobbySettingsMenu().MaxPlayersChanged(num - 2);
            new MultiplayerLobbySettingsMenu().JoinInProgressChanged(num - 3);
        }


        public static void GuaJiTiXing_Fun(Human human)
        {
            if (!NetGame.isServer && !NetGame.isClient )
            {
                return;
            }
            if(human==null)
            {
                return;
            }
            if(!guajitixing)
            {
                if(human.GetExt().YiGuaJi)
                {
                    human.GetExt().YiGuaJi = false;
                    //human.GetExt().lastCaoZuoTime = Time.time;
                    YxMod.enThrowing(human, true);
                    //human.GetExt().ntp = false;
                    human.GetExt().guajiNtpHuman = null;
                    Rigidbody[] rigidbodies2 = human.rigidbodies;
                    for (int j = 0; j < rigidbodies2.Length; j++)
                    {
                        rigidbodies2[j].useGravity = true;
                        rigidbodies2[j].drag = 0f;
                    }
                }

                return;
            }
            if (YouCaoZuo(human) || human.GetExt().cameraPitchAngle != human.controls.cameraPitchAngle || human.GetExt().cameraYawAngle != human.controls.cameraYawAngle)
            {
                if (human.GetExt().YiGuaJi)
                {
                    human.GetExt().YiGuaJi = false;
                    Chat.TiShi($"玩家 {human.player.host.name} 回到了游戏");

                    YxMod.enThrowing(human, true);
                    human.GetExt().guajiNtpHuman = null;
                    Rigidbody[] rigidbodies2 = human.rigidbodies;
                    for (int j = 0; j < rigidbodies2.Length; j++)
                    {
                        rigidbodies2[j].useGravity = true;
                        rigidbodies2[j].drag = 0f;
                    }

                    //if (App.state != AppSate.ServerLoadLobby)
                    //{
                    //    Game.instance.RespawnAllPlayers(human.player.host);
                    //}
                }
                human.GetExt().lastCaoZuoTime = Time.time;
            }
            else
            {
                if (!human.GetExt().YiGuaJi)
                {
                    if (Time.time - human.GetExt().lastCaoZuoTime >= UI_SheZhi.guajishijian * 60) ///UI_SheZhi.guajishijian
                    {
                        Chat.TiShi($"玩家 {human.player.host.name} {UI_SheZhi.guajishijian}分钟无操作,已挂机");
                        human.GetExt().YiGuaJi = true;
                    }
                }
                else
                {
                    //ZuoXia(Human.all[0]);
                }
            }
            human.GetExt().cameraPitchAngle = human.controls.cameraPitchAngle;
            human.GetExt().cameraYawAngle = human.controls.cameraYawAngle;


            //"跌落", "睡觉", "气球", "坐下", "挂件"

            if (human.GetExt().YiGuaJi)
            {
                if (guajidongzuoID == 0)
                {
                    YxMod.enThrowing(human, false);
                }
                else if (guajidongzuoID == 4)//挂件
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        if (Human.all[i].GetExt().guajiNtpHuman != human && Human.all[i] != human)
                        {
                            human.GetExt().guajiNtpHuman = Human.all[i];
                        }
                    }
                    if (human.GetExt().guajiNtpHuman != null)
                    {
                        //human.GetExt().ntp = true;
                        YxMod.enThrowing(human, false);
                        for (int i = 0; i < human.rigidbodies.Length; i++)
                        {
                            human.rigidbodies[i].rotation = human.GetExt().guajiNtpHuman.rigidbodies[i].rotation;
                            Vector3 position = human.GetExt().guajiNtpHuman.rigidbodies[i].position;
                            position.y += 2f;
                            human.rigidbodies[i].position = position;
                            human.rigidbodies[i].velocity = human.GetExt().guajiNtpHuman.rigidbodies[i].velocity;
                        }
                    }
                    else
                    {
                        //human.GetExt().ntp = false;
                        YxMod.enThrowing(human, true);
                    }

                }
                else
                {
                    if (guajidongzuoID == 1)//睡觉
                    {
                        human.state = HumanState.FreeFall;
                    }
                    else if (guajidongzuoID == 2)//气球
                    {
                        human.state = HumanState.FreeFall;
                    }
                    else if (guajidongzuoID == 3)
                    {
                        //human.state = HumanState.FreeFall;
                        YxMod.ZuoXia(human);
                    }
                    YxMod.enThrowing(human, flag: true);
                }
            }
            //else
            //{
            //    YxMod.enThrowing(human, flag: true);
            //    //human.GetExt().ntp = false;
            //    human.GetExt().guajiNtpHuman = null;
            //}
            Rigidbody[] rigidbodies = human.rigidbodies;
            for (int j = 0; j < rigidbodies.Length; j++)
            {
                if (human.GetExt().YiGuaJi)
                {
                    if (guajidongzuoID == 0 || guajidongzuoID == 1 || guajidongzuoID == 4)
                    {
                        rigidbodies[j].useGravity = true;
                        rigidbodies[j].drag = 0f;
                    }

                    else if (guajidongzuoID == 2 || guajidongzuoID == 3)
                    {
                        rigidbodies[j].useGravity = false;
                        rigidbodies[j].drag = 0f;
                    }
                }
                //else
                //{
                //    rigidbodies[j].useGravity = true;
                //    rigidbodies[j].drag = 0f;
                //}
            }
        }

        private static bool YouCaoZuo(Human human)
        {
            bool caozuo = false;
            if (human.controls.rightGrab || human.controls.leftGrab || human.controls.walkLocalDirection.x != 0f || human.controls.walkLocalDirection.z != 0f || human.controls.jump || human.controls.shootingFirework)
            {
                caozuo = true;
            }
            return caozuo;
        }



        public static void HuiSu()
        {
            if(NetGame.isServer)
            {
                Human.all[0].GetExt().dingdian.huisu = huisu;

            }
            else if (NetGame.isClient)
            {
                if (YxMod.YxModServer)
                {
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("huisudingdian"), huisu ? "1" : "0");
                }
            }
            PlayerPrefs.SetInt("huisudingdian", huisu ? 1 : 0);
        
            //保存设置
        }
        public static void GuanXing()
        {
            // 互斥：如果开了惯性，就关掉保速
            if (guanxing)
            {
                baoLiuDangQianSuDu = false;
            }

            if (NetGame.isServer || NetGame.isLocal)
            {
                var ext = Human.all[0].GetExt().dingdian;
                ext.guanxing = guanxing;
                ext.baoLiuDangQianSuDu = baoLiuDangQianSuDu;
            }
            else if (NetGame.isClient)
            {
                if (YxMod.YxModServer)
                {
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("guanxingdingdian"), guanxing ? "1" : "0");
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("baosudingdian"), baoLiuDangQianSuDu ? "1" : "0");
                }
            }

            PlayerPrefs.SetInt("guanxingdingdian", guanxing ? 1 : 0);
            PlayerPrefs.SetInt("baosudingdian", baoLiuDangQianSuDu ? 1 : 0);
        }
        public static void BaoSu()
        {
            // 互斥：如果开了保速，就关掉惯性
            if (baoLiuDangQianSuDu)
            {
                guanxing = false;
            }

            if (NetGame.isServer || NetGame.isLocal)
            {
                var ext = Human.all[0].GetExt().dingdian;
                ext.baoLiuDangQianSuDu = baoLiuDangQianSuDu;
                ext.guanxing = guanxing;
            }
            else if (NetGame.isClient)
            {
                if (YxMod.YxModServer)
                {
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("baosudingdian"), baoLiuDangQianSuDu ? "1" : "0");
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("guanxingdingdian"), guanxing ? "1" : "0");
                }
            }

            PlayerPrefs.SetInt("baosudingdian", baoLiuDangQianSuDu ? 1 : 0);
            PlayerPrefs.SetInt("guanxingdingdian", guanxing ? 1 : 0);
        }


        public static void Q()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                Human.all[0].GetExt().dingdian.q = q;
            }
            else if (NetGame.isClient)
            {
                if (YxMod.YxModServer)
                {
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("qdingdian"), q ? "1" : "0");
                }
            }
            PlayerPrefs.SetInt("qdingdian", q ? 1 : 0);
        }
        public static void SE()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                Human.all[0].GetExt().dingdian.se = se;
            }
            else if (NetGame.isClient)
            {
                if (YxMod.YxModServer)
                {
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("sedingdian"), se ? "1" : "0");
                }
            }
            PlayerPrefs.SetInt("sedingdian", se ? 1 : 0);
        }
        public static void GaoDu()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                Human.all[0].GetExt().dingdian.gaodu = gaodu;
            }
            else if (NetGame.isClient)
            {
                if (YxMod.YxModServer)
                {
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("dingdiangaodu"), $"{gaodu}");
                }
            }
            PlayerPrefs.SetFloat("dingdiangaodu", gaodu);
        }
        public static void GeShu()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                Human.all[0].GetExt().dingdian.geshu = geshu;
            }
            else if (NetGame.isClient)
            {
                if (YxMod.YxModServer)
                {
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("dingdiangeshu"), $"{geshu}");
                }
            }
            PlayerPrefs.SetInt("dingdiangeshu", geshu);
        }
        public static void TiShiStr()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                Human.all[0].GetExt().dingdian.tishiStr = tishiStr;
            }
            else if (NetGame.isClient)
            {
                if (YxMod.YxModServer)
                {
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("dingdiantishi"), tishiStr);
                }
            }
            PlayerPrefs.SetString("dingdiantishi", tishiStr);
        }
        //public static void GlobalGeShu()
        //{
        //    if (NetGame.isClient && YxMod.YxModServer)
        //    {
        //        Chat.SendYxModMsgClient(Chat.YxModMsgStr("globaldingdiangeshu"), globalgeshu.ToString());
        //    }
        //    PlayerPrefs.SetInt("globaldingdiangeshu", globalgeshu);
        //}
        //public static void GlobalTiShiStr()
        //{
        //    if (NetGame.isClient && YxMod.YxModServer)
        //    {
        //        Chat.SendYxModMsgClient(Chat.YxModMsgStr("globaldingdiantishi"), globaltishiStr);
        //    }
        //    PlayerPrefs.SetString("globaldingdiantishi", globaltishiStr);
        //}
        public static void HuiCheLiaoTian()
        {
            PlayerPrefs.SetInt("huicheliaotian", huicheliaotian ? 1 : 0);
        }
        public static void PingBiYanSeDaiMa()
        {
            PlayerPrefs.SetInt("pingbiyansedaima", pingbiyansedaima ? 1 : 0);
        }

        public static void SetMingZiSheZhi()
        {
            PlayerPrefs.SetInt("MingZiSheZhi", MingZiSheZhi ? 1 : 0);
        }
        public static void SetMingZiStr()
        {
            PlayerPrefs.SetString("MingZiStr", MingZiStr);
        }
        public static void HuanYuanMingZi()
        {
            MingZiStr = SteamFriends.GetPersonaName(); //"愤怒的小小鸟";
            SetMingZiStr();
        }
        public static void SetMingZiZiDingYi()
        {
            PlayerPrefs.SetInt("MingZiZiDingYi", MingZiZiDingYi ? 1 : 0);
        }
        public static void SetMingZiCuTi()
        {
            PlayerPrefs.SetInt("MingZiCuTi", MingZiCuTi ? 1 : 0);
        }
        public static void SetMingZiXieTi()
        {
            PlayerPrefs.SetInt("MingZiXieTi", MingZiXieTi ? 1 : 0);
        }

        public static void SetMingZiDaiMa()
        {
            PlayerPrefs.SetString("MingZiDaiMa", MingZiDaiMa);
        }

        public static void SetMingZiDaXiao()
        {
            PlayerPrefs.SetInt("MingZiDaXiao", MingZiDaXiao);
        }
        public static void SetMingZiJianBianDaXiao1()
        {
            PlayerPrefs.SetInt("MingZiJianBianDaXiao1", MingZiJianBianDaXiao1);
        }
        public static void SetMingZiJianBianDaXiao2()
        {
            PlayerPrefs.SetInt("MingZiJianBianDaXiao2", MingZiJianBianDaXiao2);
        }
        public static void SetMingZiSuiJiDaXiao1()
        {
            PlayerPrefs.SetInt("MingZiSuiJiDaXiao1", MingZiSuiJiDaXiao1);
        }
        public static void SetMingZiSuiJiDaXiao2()
        {
            PlayerPrefs.SetInt("MingZiSuiJiDaXiao2", MingZiSuiJiDaXiao2);
        }
        public static void SetMingZiTiaoYueDaXiao1()
        {
            PlayerPrefs.SetInt("MingZiTiaoYueDaXiao1", MingZiTiaoYueDaXiao1);
        }
        public static void SetMingZiTiaoYueDaXiao2()
        {
            PlayerPrefs.SetInt("MingZiTiaoYueDaXiao2", MingZiTiaoYueDaXiao2);
        }


        public static void SetFaYanSheZhi()
        {
            PlayerPrefs.SetInt("FaYanSheZhi", FaYanSheZhi ? 1 : 0);
        }

        public static void SetFaYanCuTi()
        {
            PlayerPrefs.SetInt("FaYanCuTi", FaYanCuTi ? 1 : 0);
        }
        public static void SetFaYanXieTi()
        {
            PlayerPrefs.SetInt("FaYanXieTi", FaYanXieTi ? 1 : 0);
        }


        public static void SetFaYanDaXiao()
        {
            PlayerPrefs.SetInt("FaYanDaXiao", FaYanDaXiao);
        }
        public static void SetFaYanJianBianDaXiao1()
        {
            PlayerPrefs.SetInt("FaYanJianBianDaXiao1", FaYanJianBianDaXiao1);
        }
        public static void SetFaYanJianBianDaXiao2()
        {
            PlayerPrefs.SetInt("FaYanJianBianDaXiao2", FaYanJianBianDaXiao2);
        }
        public static void SetFaYanSuiJiDaXiao1()
        {
            PlayerPrefs.SetInt("FaYanSuiJiDaXiao1", FaYanSuiJiDaXiao1);
        }
        public static void SetFaYanSuiJiDaXiao2()
        {
            PlayerPrefs.SetInt("FaYanSuiJiDaXiao2", FaYanSuiJiDaXiao2);
        }

        public static void SetFaYanTiaoYueDaXiao1()
        {
            PlayerPrefs.SetInt("FaYanTiaoYueDaXiao1", FaYanTiaoYueDaXiao1);
        }
        public static void SetFaYanTiaoYueDaXiao2()
        {
            PlayerPrefs.SetInt("FaYanTiaoYueDaXiao2", FaYanTiaoYueDaXiao2);
        }


        public static void SetMingZiYanSe()
        {

            YanSe.GetYanSeWindow(ref MingZiYanSe);
            PlayerPrefs.SetString("MingZiYanSe", MingZiYanSe);
        }
        public static void SetMingZiJianBianYanSe1() 
        { 
            YanSe.GetYanSeWindow(ref MingZiJianBianYanSe1);
            PlayerPrefs.SetString("MingZiJianBianYanSe1", MingZiJianBianYanSe1);
        }
        public static void SetMingZiJianBianYanSe2() 
        { 
            YanSe.GetYanSeWindow(ref MingZiJianBianYanSe2);
            PlayerPrefs.SetString("MingZiJianBianYanSe2", MingZiJianBianYanSe2);
        }
        public static void SetMingZiSuiJiYanSeLiangDu()
        {
            PlayerPrefs.SetInt("MingZiSuiJiYanSeLiangDu", MingZiSuiJiYanSeLiangDu);
        }
        public static void SetMingZiTiaoYueYanSe1() 
        { 
            YanSe.GetYanSeWindow(ref MingZiTiaoYueYanSe1);
            PlayerPrefs.SetString("MingZiTiaoYueYanSe1", MingZiTiaoYueYanSe1);
        }
        public static void SetMingZiTiaoYueYanSe2() 
        { 
            YanSe.GetYanSeWindow(ref MingZiTiaoYueYanSe2);
            PlayerPrefs.SetString("MingZiTiaoYueYanSe2", MingZiTiaoYueYanSe2);
        }

        public static void SetFaYanYanSe() 
        { 
            YanSe.GetYanSeWindow(ref FaYanYanSe);
            PlayerPrefs.SetString("FaYanYanSe", FaYanYanSe);
        }
        public static void SetFaYanJianBianYanSe1() 
        { 
            YanSe.GetYanSeWindow(ref FaYanJianBianYanSe1);
            PlayerPrefs.SetString("FaYanJianBianYanSe1", FaYanJianBianYanSe1);
        }
        public static void SetFaYanJianBianYanSe2() 
        { 
            YanSe.GetYanSeWindow(ref FaYanJianBianYanSe2);
            PlayerPrefs.SetString("FaYanJianBianYanSe2", FaYanJianBianYanSe2);
        }
        public static void SetFaYanColorType()
        {
            FaYanSeed = ColorfulSpeek.ins.GetSeedByIndex(FaYanColorType, 0f, 1f);
            PlayerPrefs.SetInt("FaYanColorType", FaYanColorType);
        }
        public static void SetMingZiColorType()
        {
            MingZiSeed = ColorfulSpeek.ins.GetSeedByIndex(MingZiColorType, 0f, 1f);
            PlayerPrefs.SetInt("MingZiColorType", MingZiColorType);
        }
        public static void SetCaiDanYanShe()
        {
            PlayerPrefs.SetInt("Gradual_Change", ColorfulSpeek.Gradual_Change ? 1 : 0);
            UI.nameCache.Clear();
            UI.heightCache.Clear();
            UI.contentCache.Clear();
        }

        public static void SetCaiDanColorType()
        {
            ColorfulSpeek.ins.Change_Gradual_Num(CaiDanColorType.ToString());
            UI.nameCache.Clear();
            UI.heightCache.Clear();
            UI.contentCache.Clear();
        }
        public static void SetFaYanSuiJiYanSeLiangDu()
        {
            PlayerPrefs.SetInt("FaYanSuiJiYanSeLiangDu", FaYanSuiJiYanSeLiangDu);
        }
        public static void SetFaYanTiaoYueYanSe1() 
        { 
            YanSe.GetYanSeWindow(ref FaYanTiaoYueYanSe1);
            PlayerPrefs.SetString("FaYanTiaoYueYanSe1", FaYanTiaoYueYanSe1);
        }
        public static void SetFaYanTiaoYueYanSe2() 
        { 
            YanSe.GetYanSeWindow(ref FaYanTiaoYueYanSe2);
            PlayerPrefs.SetString("FaYanTiaoYueYanSe2", FaYanTiaoYueYanSe2);
        }

    }
}