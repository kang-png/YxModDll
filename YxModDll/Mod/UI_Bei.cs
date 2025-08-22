using Multiplayer;
using UnityEngine;

namespace YxModDll.Mod
{
    internal class UI_Bei
    {
        private static Vector2 wuti_scrollPosition;
        public static int humanID = -1;
        public static int yuan_humanID = 1;
        public static int left = UI_Main.beiUI_left;
        public static int top = UI_Main.beiUI_top;

        public static void CreatUI()//创建菜单功能区
        {
            float kehuquHeight = UI_Main.beiUI_gao - UI_Windows.biaotiUiheight;
            float kehuquWeith = UI_Main.beiUI_kuan;// - UI_Windows.biankuangsize * 2;

            Rect humanlistRect = new Rect(0, 0, kehuquWeith, kehuquHeight);
            UI.CreatUiBox(humanlistRect, UI_Windows.gongnengquTexture);
            GUILayout.BeginArea(humanlistRect);
            UI.CreatAnNiu("取消被背", false, QuXiaoBeiBei);

            GUILayout.Space(10);

            wuti_scrollPosition = GUILayout.BeginScrollView(wuti_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域

            int i = UI.CreatHumanList(humanID,false);
            if (humanID != i)
            {
                humanID = i;
                Bei(yuan_humanID-1,humanID);
                humanID = -1;

            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public static void Bei(int yuanhuman,int inthuman)// human编号
        {
            if (NetGame.isServer)
            {
                if (!UI_GongNeng.guajianxitong_KaiGuan)
                {
                    UI_GongNeng.guajianxitong_KaiGuan = true;
                    UI_GongNeng.GuaJianXiTong();
                }
                c_BeiRen.BeiRen(Human.all[yuanhuman], Human.all[inthuman]);
            }
            else if (NetGame.isClient && YxMod.YxModServer)
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("beiren"),$"{yuanhuman}|{inthuman}");//
            }
            UI_Main.ShowBeiUI = false;
            UI_CaiDan.JiXu();
        }
        public static void QuXiaoBeiBei()//取消
        {
            if (NetGame.isServer)
            {
                c_BeiRen.QuXiaoBeiRen(Human.all[yuan_humanID - 1]);
            }
            else if (NetGame.isClient && YxMod.YxModServer)
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("qxbeiren"), $"{yuan_humanID-1}");//取消挂件
            }
            UI_Main.ShowBeiUI = false;
            UI_CaiDan.JiXu();
        }
    }
}
