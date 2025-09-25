using HumanAPI;
using Multiplayer;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YxModDll.Mod.Features;
using YxModDll.Patches;



namespace YxModDll.Mod
{
    internal class UI_WanJia
    {
        private static Vector2 wanjia_scrollPosition;
        private static Vector2 thing_scrollPosition;


        private static int wanjialist_kuan = 150;
        public static int humanID = 99;

        public static bool allkejiquanxian = UI_SheZhi.morenkejiquanxian;
        public static bool allliaotiankuangquanxian = true;
        public static bool alldingdian = true;
        public static bool allwujiasi = true;
        public static bool allwupengzhuang;
        public static bool allfeitian;
        public static bool allchaoren;
        public static bool allshanxian;
        public static bool alldongjie;
        public static bool allbengdi;
        public static bool allsanjitiao;
        public static bool alldiantun;
        public static bool allqiqiu;

        public static bool allqiqiuxifa;
        public static bool alldaoli;
        public static bool allzhuanquan;
        public static bool alltuoluo;

        public static bool allketouguai;
        public static bool alldiaosigui;
        public static bool allpangxie;
        public static bool allqianshui;

        public static bool alltuique;
        public static bool alltuiguai;
        public static bool allchaichu;
        public static bool allkongqipao;
        public static bool allbanshen;
        public static bool allchaojitiao;

        public static Human human;
        private static bool allshouhua;

        public static float bufasudu = 1.5f;

        public static void CreatUI()//创建菜单功能区
        {
            float kehuquHeight = UI_Main.wanjiaUI_gao - UI_Windows.biaotiUiheight;
            float kehuquWeith = UI_Main.wanjiaUI_kuan - UI_Windows.biankuangsize * 2;
            Rect wanjialist = new Rect(0, 0, wanjialist_kuan, kehuquHeight);
            UI.CreatUiBox(wanjialist, UI_Windows.gongnengquTexture2);

            Rect thinglist = new Rect(wanjialist_kuan + UI_Windows.biankuangsize, 0, kehuquWeith - wanjialist_kuan - UI_Windows.biankuangsize, kehuquHeight);
            UI.CreatUiBox(thinglist, UI_Windows.gongnengquTexture2);

            GUILayout.BeginArea(wanjialist);
            wanjia_scrollPosition = GUILayout.BeginScrollView(wanjia_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域

            int i = UI.CreatHumanList(humanID);
            humanID = i;

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUILayout.BeginArea(thinglist);
            thing_scrollPosition = GUILayout.BeginScrollView(thing_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域

            if (humanID == 0)
            {
                if (NetGame.isServer)
                {
                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("客机权限", ref allkejiquanxian, false, KeJiQuanXian);
                    UI.CreatAnNiu_AnXia("聊天框权限", ref allliaotiankuangquanxian, false, AllLiaoTianKuangQuanXian);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("个人定点", ref alldingdian, false, GeRenDingDian);
                    UI.CreatAnNiu_AnXia("无假死", ref allwujiasi, false, WuJiaSi);
                    UI.CreatAnNiu_AnXia("无碰撞", ref allwupengzhuang, false, WuPengZhuang);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("飞天", ref allfeitian, false, FeiTian);
                    UI.CreatAnNiu_AnXia("超人", ref allchaoren, false, ChaoRen);
                    UI.CreatAnNiu_AnXia("闪现", ref allshanxian, false, ShanXian);
                    GUILayout.EndHorizontal();
                    
                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("冻结", ref alldongjie, false, DongJie);
                    UI.CreatAnNiu_AnXia("半身不遂", ref allbanshen, false, BanShen);
                    UI.CreatAnNiu_AnXia("超级跳", ref allchaojitiao, false, chaojitiao);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("手滑", ref allshouhua, false, ShouHua);
                    UI.CreatAnNiu_AnXia("蹦迪", ref allbengdi, false, BengDi);
                    UI.CreatAnNiu_AnXia("三级跳", ref allsanjitiao, false, SanJiTiao);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("电臀", ref alldiantun, false, DianTun);
                    UI.CreatAnNiu_AnXia("气球", ref allqiqiu, false, QiQiu);
                    UI.CreatAnNiu_AnXia("气球戏法", ref allqiqiuxifa, false, QiQiuXiFa);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("倒立", ref alldaoli, false, DaoLi);
                    UI.CreatAnNiu_AnXia("转圈圈", ref allzhuanquan, false, ZhuanQuan);
                    UI.CreatAnNiu_AnXia("陀螺", ref alltuoluo, false, TuoLuo);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("磕头怪", ref allketouguai, false, KeTouGuai);
                    UI.CreatAnNiu_AnXia("吊死鬼", ref alldiaosigui, false, DiaoSiGui);
                    UI.CreatAnNiu_AnXia("螃蟹", ref allpangxie, false, PangXie);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("潜水", ref allqianshui, false, QianShui);
                    UI.CreatAnNiu_AnXia("腿瘸", ref alltuique, false, TuiQue);
                    UI.CreatAnNiu_AnXia("腿拐", ref alltuiguai, false, TuiGuai);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("拆除", ref allchaichu, false, ChaiChu);
                    UI.CreatAnNiu_AnXia("空气炮", ref allkongqipao, false, KongQiPao);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                    UI.CreatFenGeXian();//分割线
                    GUILayout.Space(5);

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu("悬浮列队", false, () =>
                    {
                        foreach (Human human in Human.all)
                        {
                            YxMod.QuXiaoGuaJian(human);
                        }
                        //Chat.TiShi("所有玩家已在主机带领下列队完成。");
                        ArrangeAllHumansFacing(Human.Localplayer);
                    }, "所有玩家以主机为队长，左右排成一列。");
                    UI.CreatAnNiu("取消悬浮", false, () =>
                    {
                        foreach (Human human in Human.all)
                        {
                            YxMod.QuXiaoGuaJian(human);
                        }
                        //Chat.TiShi("已取消悬浮列队，玩家恢复自由状态。");
                    });
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                    UI.CreatFenGeXian();//分割线
                    GUILayout.Space(5);

                    GUILayout.Label(ColorfulSpeek.colorshows(UI.TranslateButtonText("所有人属性>>")));
                    float gravityY = Physics.gravity.y;
                    UI.CreatShuZhi("修改重力", ref gravityY, -50f, 0f, 0.5f, () => {
                        Physics.gravity = new Vector3(0f, gravityY, 0f);
                    }, yuan: -9.81f);

                    UI.CreatAnNiu_AnXia("修改速度", ref FeatureManager.modifySpeed, false, null, "点按钮切换开关");
                    if (FeatureManager.modifySpeed)
                    {
                        float tempSpeed = 1f;
                        float.TryParse(FeatureManager.curSpeed, out tempSpeed);

                        UI.CreatShuZhi("倍速", ref tempSpeed, 0f, 5f, 0.1f, () =>
                        {
                            FeatureManager.curSpeed = tempSpeed.ToString("0.00");
                        }, yuan: 1f);
                    }
                }

                GUILayout.Space(5);
                UI.CreatFenGeXian();//分割线
                GUILayout.Space(5);

                UI.CreatAnNiu("修复所有人皮肤", false, () => {
                    foreach (NetPlayer player in NetGame.instance.players)
                    {
                        if (!player.isLocalPlayer)
                        {
                            (FeatureManager.instance).StartCoroutine(FeatureManager.instance.OnReceiveSkinCoroutine(player));
                        }
                    }
                }, "重新加载所有人的皮肤，异步执行不会卡。慎点！会占大量内存可能蹦，优先用个人皮肤修复");
            }
            else
            {
                human = YxMod.ChaHumanId($"{humanID}");
                if (human == null)
                {
                    humanID = 0;
                    return;
                }

                

                GUILayout.BeginHorizontal();
                if ((NetGame.isServer && humanID != 1) || (NetGame.isClient && !KeJiZiJi())) //不是自己
                {
                    UI.CreatAnNiu("个人资料", false, GeRenZiLiao);
                    UI.CreatAnNiu("加好友", false, JiaHaoYou);
                }
                if ((NetGame.isServer && humanID != 1) || (NetGame.isClient && !KeJiZiJi() && humanID != 1)) //不是自己/房主/
                {
                    UI.CreatAnNiu("踢出房间", false, TiChuFangJian);
                }
                GUILayout.EndHorizontal();

                if ((NetGame.isServer) || (NetGame.isClient && YxMod.YxModServer && KeJiZiJi())) 
                {
                    if (NetGame.isServer)
                    {
                        UI.CreatShuZhi("步伐速度", ref human.GetExt().bufasudu, 1f, 3f, 0.1f, () =>
                        {
                            //human.GetExt().bufasudu = bufasudu;
                            if(humanID != 1 && human.GetExt().isClient)
                            {
                                Chat.SendYxModMsgServer(human.player.host, Chat.YxModMsgStr("bufasudu"), $"{human.GetExt().bufasudu}");
                            }
                            
                        }, yuan: 1.5f);
                    }
                    else if(NetGame.isClient)
                    {
                        UI.CreatShuZhi("步伐速度", ref bufasudu, 1f, 3f, 0.1f, () =>
                        {
                            Chat.SendYxModMsgClient(Chat.YxModMsgStr("bufasudu"), $"{bufasudu}");
                        }, yuan: 1.5f);
                    }

                }


                GUILayout.BeginHorizontal();
                if (NetGame.isServer || NetGame.isLocal || (NetGame.isClient && KeJiZiJi())) //是自己
                {
                    UI.CreatAnNiu_AnXia("物品挂件", ref human.GetExt().wutiguajian, false, WuTiGuaJian2,"左手抓住物体，按M键设置为挂件，点击按钮取消");
                    
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (NetGame.isServer)
                {
                    if (human.GetExt().isClient && humanID != 1)
                    {
                        UI.CreatAnNiu_AnXia("客机权限", ref human.GetExt().kejiquanxian, false, KeJiQuanXian);
                    }
                    UI.CreatAnNiu_AnXia("聊天框权限", ref human.GetExt().liaotiankuangquanxian, false);
                }
                GUILayout.EndHorizontal();

                if (NetGame.isServer || NetGame.isLocal || (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))) //自己
                {
                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("个人定点", ref human.GetExt().dingdian.kaiguan, false, GeRenDingDian);
                    UI.CreatAnNiu_AnXia("无假死", ref human.GetExt().wujiasi, false, WuJiaSi);
                    UI.CreatAnNiu_AnXia("无碰撞", ref human.GetExt().wupengzhuang, false, WuPengZhuang);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("飞天", ref human.GetExt().feitian, false, FeiTian);
                    UI.CreatAnNiu_AnXia("超人", ref human.GetExt().chaoren, false, ChaoRen);
                    UI.CreatAnNiu_AnXia("闪现", ref human.GetExt().shanxian, false, ShanXian);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu("传送至>>", false, ChuanSongZhi);
                    UI.CreatAnNiu("悬浮于>>", false, XuanFuYu);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu("牵手>>", false, QianShou);
                    UI.CreatAnNiu("被..背>>", false, Bei);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("冻结", ref human.GetExt().dongjie, false, DongJie);
                    UI.CreatAnNiu_AnXia("半身不遂", ref human.GetExt().banshen, false, BanShen);
                    UI.CreatAnNiu_AnXia("超级跳", ref human.GetExt().chaojitiao, false, chaojitiao);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("手滑", ref human.GetExt().shouhua, false, ShouHua);
                    UI.CreatAnNiu_AnXia("蹦迪", ref human.GetExt().bengdi, false, BengDi);
                    UI.CreatAnNiu_AnXia("三级跳", ref human.GetExt().sanjitiao, false, SanJiTiao);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("电臀", ref human.GetExt().diantun, false, DianTun);
                    UI.CreatAnNiu_AnXia("气球", ref human.GetExt().qiqiu, false, QiQiu);
                    UI.CreatAnNiu_AnXia("气球戏法", ref human.GetExt().qiqiuxifa, false, QiQiuXiFa);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("倒立", ref human.GetExt().daoli, false, DaoLi);
                    UI.CreatAnNiu_AnXia("转圈圈", ref human.GetExt().zhuanquan, false, ZhuanQuan);
                    UI.CreatAnNiu_AnXia("陀螺", ref human.GetExt().tuoluo, false, TuoLuo);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("磕头怪", ref human.GetExt().ketouguai, false, KeTouGuai);
                    UI.CreatAnNiu_AnXia("吊死鬼", ref human.GetExt().diaosigui, false, DiaoSiGui);
                    UI.CreatAnNiu_AnXia("螃蟹", ref human.GetExt().pangxie, false, PangXie);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("潜水", ref human.GetExt().qianshui, false, QianShui);
                    UI.CreatAnNiu_AnXia("腿瘸", ref human.GetExt().tuique, false, TuiQue);
                    UI.CreatAnNiu_AnXia("腿拐", ref human.GetExt().tuiguai, false, TuiGuai);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu_AnXia("拆除", ref human.GetExt().chaichu, false, ChaiChu);
                    UI.CreatAnNiu_AnXia("空气炮", ref human.GetExt().kongqipao, false, KongQiPao);
                    GUILayout.EndHorizontal();

                }


                if (NetGame.isServer || NetGame.isLocal)
                {
                    GUILayout.Space(5);
                    UI.CreatFenGeXian();//分割线
                    GUILayout.Space(5);

                    GUILayout.Label(ColorfulSpeek.colorshows(UI.TranslateButtonText("服务端属性>>")));

                    UI.CreatShuZhi("自定义Y", ref human.GetExt().numY, 0, 10, 1, null, yuan: 0);
                    float drag = human.rigidbodies[0].drag;
                    float mass = human.mass;
                    float maxLiftForce = human.motionControl2.hands.maxLiftForce;
                    float weight = human.weight;

                    UI.CreatShuZhi("体重", ref weight, 500f, 2000f, 10f, () => {
                        human.weight = weight;
                    }, yuan: 1020.24f);

                    GUILayout.BeginHorizontal();
                    UI.CreatShuZhi("阻力", ref drag, 0f, 5f, 0.1f, () => {
                        human.SetDrag(drag, true);
                    });
                    if (GUILayout.Button(ColorfulSpeek.colorshows(UI.TranslateButtonText("重置")), UI.styleButton()))
                    {
                        human.ResetDrag();
                        drag = human.rigidbodies[0].drag;
                    }
                    //GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    UI.CreatShuZhi("手部力量", ref maxLiftForce, 0f, 1000f, 50f, () => {
                        human.motionControl2.hands.maxLiftForce = maxLiftForce;
                    }, yuan: 500f);

                    UI.CreatShuZhi("跳跃间距", ref mass, 10f, 200f, 5f, () => {
                        human.mass = mass;
                    }, yuan: 104f);

                }
                if (NetGame.isServer || NetGame.isLocal || (NetGame.isClient && KeJiZiJi()))
                {
                    GUILayout.Space(5);
                    UI.CreatFenGeXian();//分割线
                    GUILayout.Space(5);
                    GUILayout.Label(ColorfulSpeek.colorshows(UI.TranslateButtonText("客户端属性>>")));

                    UI.CreatAnNiu_AnXia("修改手长", ref FeatureManager.modifyHand, false, null, "点按钮切换开关");
                    if (FeatureManager.modifyHand)
                    {
                        // 普通手长
                        float curHandVal = 0f;
                        float.TryParse(FeatureManager.curHand, out curHandVal);
                        UI.CreatShuZhi("普通手长", ref curHandVal, 0f, 100f, 0.1f, () =>
                        {
                            FeatureManager.curHand = curHandVal.ToString("0.00");
                        }, yuan: 0f);

                        // 伸手手长
                        float curExtendedHandVal = 1f;
                        float.TryParse(FeatureManager.curExtendedHand, out curExtendedHandVal);
                        UI.CreatShuZhi("伸手手长", ref curExtendedHandVal, 0f, 100f, 0.1f, () =>
                        {
                            FeatureManager.curExtendedHand = curExtendedHandVal.ToString("0.00");
                        }, yuan: 1f);
                    }

                }

                GUILayout.Space(5);
                UI.CreatFenGeXian();//分割线
                GUILayout.Space(5);

                GUILayout.Label(ColorfulSpeek.colorshows(UI.TranslateButtonText("皮肤管理>>")));
                UI.CreatAnNiu("修复皮肤", false, () => {
                    if (!human.player.isLocalPlayer)
                    {
                        (FeatureManager.instance).StartCoroutine(FeatureManager.instance.OnReceiveSkinCoroutine(human.player));
                    }
                }, "如果本地无皮肤数据，会向服务器请求一次（可能拿到旧皮肤）；已有本地数据时将直接应用。请勿重复点击。");
                if (NetGame.isServer || NetGame.isLocal || (NetGame.isClient && KeJiZiJi()))
                {
                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu("刷新皮肤", false, () =>
                    {
                        WorkshopRepository.instance.presetRepo.ResetPresets();
                    }, "清除皮肤缓存，重新加载 MainCharacter 和 CoopCharacter 皮肤");
                    UI.CreatAnNiu("切换皮肤", false, () =>
                    {
                        var netPlayer = Human.Localplayer?.player;
                        if (netPlayer != null)
                        {
                            // 读取当前皮肤索引（默认为0）
                            int curIndex = PlayerPrefs.GetInt("MainSkinIndex", 0);
                            // 切换到另一个（0 -> 1, 1 -> 0）
                            int newIndex = (curIndex + 1) % 2;

                            // 获取目标皮肤
                            var newSkin = WorkshopRepository.instance.presetRepo.GetPlayerSkin(newIndex);
                            if (newSkin != null)
                            {
                                // 切换皮肤
                                HuanFu(netPlayer, newSkin);
                                // 保存新的索引
                                PlayerPrefs.SetInt("MainSkinIndex", newIndex);
                                // 显示提示
                                Chat.TiShi(NetGame.instance.local, $"皮肤已切换为玩家{newIndex + 1}");
                            }
                            else
                            {
                                Chat.TiShi(NetGame.instance.local, "皮肤切换失败，目标皮肤不存在");
                            }
                        }
                        else
                        {
                            Chat.TiShi(NetGame.instance.local, "本地玩家未加载");
                        }
                    }, "换皮肤会发送给所有人，请勿频繁切换。防止炸房，五分钟只能换一次");
                    GUILayout.EndHorizontal();
                    //UI.CreatAnNiu("打开文件", false, () =>
                    //{
                    //    Process.Start("explorer.exe", $"\"{Application.persistentDataPath}\"");
                    //    Chat.TiShi(NetGame.instance.local, $"已打开文件夹：{Application.persistentDataPath}");
                    //});
                    //UI.CreatAnNiu("保存皮肤！慎点", false, () =>
                    //{
                    //    int slot = PlayerPrefs.GetInt("MainSkinIndex", 0);
                    //    WorkshopRepository.instance.presetRepo.SaveSkin(slot, human.player.skin);
                    //    string charName = slot == 0 ? "MainCharacter" : "CoopCharacter";
                    //    Chat.TiShi(NetGame.instance.local, $"皮肤已保存到 {charName}，下次启动游戏会自动应用");

                    //}, "注意：此操作会覆盖本地皮肤！工坊皮肤仅临时生效，想要永久生效请点击保存按钮。为了避免矛盾，不许保存别人皮肤。");
                    //GUILayout.Label(ColorfulSpeek.colorshows("订阅皮肤列表"), UI.SetLabelStyle_JuZhong());
                    for (int j = 0; j < WorkshopRepository.instance.presetRepo.Count; j++)
                    {
                        RagdollPresetMetadata preset = WorkshopRepository.instance.presetRepo[j];
                        if (preset == null) continue;

                        string title = preset.title;
                        if (title.Length > 12) title = title.Substring(0, 12) + "...";
                        string btnText = $"{j + 1}. {title}";

                        // 用CreatAnNiu创建按钮，点击调用HuanFu换皮肤
                        UI.CreatAnNiu_Left(btnText, false, () =>
                        {
                            human.player.skin = preset;
                            HuanFu(human?.player, preset);
                            Chat.TiShi(NetGame.instance.local, $"已切换到皮肤: {preset.title}");
                        }, preset.title);
                    }
                }

                if (NetGame.isServer || NetGame.isLocal || (NetGame.isClient && KeJiZiJi())) //是自己
                {

                    GUILayout.Space(5);
                    UI.CreatFenGeXian();//分割线
                    GUILayout.Space(5);

                    GUILayout.Label(ColorfulSpeek.colorshows(UI.TranslateButtonText("身体变形>>")));
                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu("大头萌", false, () =>
                    {
                        human.GetExt().scaleHead = 1.7f;
                        human.GetExt().scaleTorso = 1.0f;
                        human.GetExt().scaleLeftArm = 1.2f;
                        human.GetExt().scaleRightArm = 1.2f;
                        human.GetExt().scaleLeftLeg = 1.2f;
                        human.GetExt().scaleRightLeg = 1.2f;
                        human.GetExt().scaleBall = 1.2f;
                        ScaleSync(human, head: human.GetExt().scaleHead, torso: human.GetExt().scaleTorso, leftArm: human.GetExt().scaleLeftArm, rightArm: human.GetExt().scaleRightArm, leftLeg: human.GetExt().scaleLeftLeg, rightLeg: human.GetExt().scaleRightLeg, ball: human.GetExt().scaleBall);
                    });
                    UI.CreatAnNiu("脑袋尖尖", false, () =>
                    {
                        human.GetExt().scaleHead = 0.7f;
                        human.GetExt().scaleTorso = 1.1f;
                        human.GetExt().scaleLeftArm = 1.4f;
                        human.GetExt().scaleRightArm = 1.4f;
                        human.GetExt().scaleLeftLeg = 1.0f;
                        human.GetExt().scaleRightLeg = 1.0f;
                        human.GetExt().scaleBall = 1.0f;
                        ScaleSync(human, head: human.GetExt().scaleHead, torso: human.GetExt().scaleTorso, leftArm: human.GetExt().scaleLeftArm, rightArm: human.GetExt().scaleRightArm, leftLeg: human.GetExt().scaleLeftLeg, rightLeg: human.GetExt().scaleRightLeg, ball: human.GetExt().scaleBall);
                    });
                    UI.CreatAnNiu("小头高达", false, () =>
                    {
                        human.GetExt().scaleHead = 0.4f;
                        human.GetExt().scaleTorso = 1.0f;
                        human.GetExt().scaleLeftArm = 1.0f;
                        human.GetExt().scaleRightArm = 1.0f;
                        human.GetExt().scaleLeftLeg = 1.0f;
                        human.GetExt().scaleRightLeg = 1.0f;
                        human.GetExt().scaleBall = 1.0f; 
                        ScaleSync(human, head: human.GetExt().scaleHead, torso: human.GetExt().scaleTorso, leftArm: human.GetExt().scaleLeftArm, rightArm: human.GetExt().scaleRightArm, leftLeg: human.GetExt().scaleLeftLeg, rightLeg: human.GetExt().scaleRightLeg, ball: human.GetExt().scaleBall);
                    });
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    UI.CreatAnNiu("重置", false, () =>
                    {
                        human.GetExt().scaleHead = 1.0f;
                        human.GetExt().scaleTorso = 1.0f;
                        human.GetExt().scaleLeftArm = 1.0f;
                        human.GetExt().scaleRightArm = 1.0f;
                        human.GetExt().scaleLeftLeg = 1.0f;
                        human.GetExt().scaleRightLeg = 1.0f;
                        human.GetExt().scaleBall = 1.0f; 
                        ScaleSync(human, head: human.GetExt().scaleHead, torso: human.GetExt().scaleTorso, leftArm: human.GetExt().scaleLeftArm, rightArm: human.GetExt().scaleRightArm, leftLeg: human.GetExt().scaleLeftLeg, rightLeg: human.GetExt().scaleRightLeg, ball: human.GetExt().scaleBall);
                    });
                    GUILayout.EndHorizontal();


                    UI.CreatShuZhi("头部缩放", ref human.GetExt().scaleHead, 0.1f, 5f, 0.1f,
                        () => ScaleSync(human, head: human.GetExt().scaleHead), 1f);

                    UI.CreatShuZhi("躯干缩放", ref human.GetExt().scaleTorso, 0.1f, 5f, 0.1f,
                        () => ScaleSync(human, torso: human.GetExt().scaleTorso), 1f);

                    UI.CreatShuZhi("左手缩放", ref human.GetExt().scaleLeftArm, 0.1f, 5f, 0.1f,
                        () => ScaleSync(human, leftArm: human.GetExt().scaleLeftArm), 1f);

                    UI.CreatShuZhi("右手缩放", ref human.GetExt().scaleRightArm, 0.1f, 5f, 0.1f,
                        () => ScaleSync(human, rightArm: human.GetExt().scaleRightArm), 1f);

                    UI.CreatShuZhi("左腿缩放", ref human.GetExt().scaleLeftLeg, 0.1f, 5f, 0.1f,
                        () => ScaleSync(human, leftLeg: human.GetExt().scaleLeftLeg), 1f);

                    UI.CreatShuZhi("右腿缩放", ref human.GetExt().scaleRightLeg, 0.1f, 5f, 0.1f,
                        () => ScaleSync(human, rightLeg: human.GetExt().scaleRightLeg), 1f);

                    UI.CreatShuZhi("球缩放", ref human.GetExt().scaleBall, 0.1f, 5f, 0.1f,
                        () => ScaleSync(human, ball: human.GetExt().scaleBall), 1f);
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        private static bool KeJiZiJi()
        {
            return humanID == GetClientHumanID();//(int)NetGame.instance.local.hostId;
        }
        public static int GetClientHumanID()
        {
            int id = 0;
            if (NetGame.isClient)
            {
                List<NetHost> players = NetGame.instance.readyclients;//客户机名

                for (int j = 0; j < players.Count; j++)
                {
                    if (players[j].hostId == NetGame.instance.local.hostId)
                    {
                        id = j + 1 + 1;
                        break;
                    }
                }
            }
            return id;
        }
        private static void ChuanSongZhi()
        {
            UI_Main.ShowChuanSongUI = true;
            if (UI_Main.ShowChuanSongUI)
            {
                UI_Main.ShowXuanFuUI = UI_Main.ShowQianShouUI = UI_Main.ShowBeiUI = false;
                UI_Main.chuansongUiRect.xMin = UI_Main.wanjiaUiRect.xMin + 100;
                UI_Main.chuansongUiRect.yMin = UI_Main.wanjiaUiRect.yMin ;
            }

            UI_ChuanSong.yuan_humanID = humanID;
        
        }

        private static void XuanFuYu()//悬浮于
        {

            UI_Main.ShowXuanFuUI = true;
            if (UI_Main.ShowXuanFuUI)
            {
                UI_Main.ShowChuanSongUI = UI_Main.ShowQianShouUI = UI_Main.ShowBeiUI = false;
                UI_Main.xuanfuUiRect.xMin = UI_Main.wanjiaUiRect.xMin + 150;
                UI_Main.xuanfuUiRect.yMin = UI_Main.wanjiaUiRect.yMin;
            }
            UI_XuanFu.yuan_humanID = humanID;
        }
        private static void QianShou()//牵手
        {

            UI_Main.ShowQianShouUI = true;
            if (UI_Main.ShowQianShouUI)
            {
                UI_Main.ShowChuanSongUI = UI_Main.ShowXuanFuUI = UI_Main.ShowBeiUI = false;
                UI_Main.qianshouUiRect.xMin = UI_Main.wanjiaUiRect.xMin + 150;
                UI_Main.qianshouUiRect.yMin = UI_Main.wanjiaUiRect.yMin;
            }
            UI_QianShou.yuan_humanID = humanID;
        }
        private static void Bei()//背
        {

            UI_Main.ShowBeiUI = true;
            if (UI_Main.ShowBeiUI)
            {
                UI_Main.ShowQianShouUI = UI_Main.ShowChuanSongUI = UI_Main.ShowXuanFuUI = false;
                UI_Main.beiUiRect.xMin = UI_Main.wanjiaUiRect.xMin + 150;
                UI_Main.beiUiRect.yMin = UI_Main.wanjiaUiRect.yMin;
            }
            UI_Bei.yuan_humanID = humanID;
        }
        private static void GeRenZiLiao()
        {
            ulong ulSteamID = 0UL;
            ulong.TryParse(human.player.skinUserId, out ulSteamID);
            //YxMod.TryGetPlayerIPAddress(new CSteamID(ulSteamID));
            SteamFriends.ActivateGameOverlayToUser("steamid", new CSteamID(ulSteamID));
        }
        private static void JiaHaoYou()
        {
            ulong ulSteamID2 = 0UL;
            ulong.TryParse(human.player.skinUserId, out ulSteamID2);
            SteamFriends.ActivateGameOverlayToUser("friendadd", new CSteamID(ulSteamID2));
        }
        private static void TiChuFangJian()//有效果
        {
            TiChuFangJian(human);
        }
        public static void TiChuFangJian(Human human0)//有效果
        {
            if (NetGame.isServer)
            {
                NetGame.instance.Kick(human0.player.host);
                NetGame.instance.OnDisconnect(human0.player.host.connection, false);
            }
            else if (NetGame.isClient)
            {
                if (YxMod.YxModServer)//如果房主有文件 并且有权限
                {
                    if (YxMod.KeJiQuanXian)
                    {
                        Chat.SendYxModMsgClient(Chat.YxModMsgStr("kick"), $"{humanID-1}");
                    }
                }
                else
                {
                    //客机强踢人
                    //Debug.Log("开始尝试强踢");
                    ulong result3 = 0uL;
                    if (ulong.TryParse(human0.player.skinUserId, out result3))
                    {
                        NetHost host = new NetHost(new CSteamID(result3), string.Empty);
                        for (uint num3 = 0u; num3 < 12; num3++)
                        {
                            NetStream netStream = NetGame.BeginMessage(NetMsgId.RemoveHost);
                            try
                            {
                                netStream.WriteNetId(num3);
                                NetGame.instance.SendReliable(host, netStream);
                            }
                            finally
                            {
                                if (netStream != null)
                                {
                                    netStream = netStream.Release();
                                }
                            }
                        }
                        Chat.TiShi(NetGame.instance.local, $"尝试强踢玩家 {human0.player.host.name}");
                    }
                    else
                    {
                        //Debug.Log($"强踢失败! {human.player.host.name} skinUserId = {human.player.skinUserId}");
                    }
                }
            }
        }
        private static void WuTiGuaJian2()//物体挂件
        {
            if (NetGame.isServer)
            {
                if (human.GetExt().ntp_wuti == null)
                {
                    human.GetExt().wutiguajian = false;
                    return;
                }
                WuTiGuaJian.QuXiaoWuTiGuaJian(human);
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("quxiaowutiguajian"), $"{humanID - 1}");
            }
        }
        private static void KeJiQuanXian()//客机权限
        {
            if (NetGame.isServer)
            {
                if(humanID ==0)
                {
                    for(int i = 0; i < Human.all.Count;i++ )
                    {
                        if (Human.all[i].GetExt().isClient)
                        {
                            Human.all[i].GetExt().kejiquanxian = allkejiquanxian ;
                            Chat.SendYxModMsgServer(Human.all[i].player.host, Chat.YxModMsgStr("kejiquanxian"), human.GetExt().kejiquanxian ? "1" : "0");
                        }
                    }
                    Chat.TiShi($"所有客机 的 客机权限 已 {(allkejiquanxian ? "打开" : "关闭")}");
                }
                else
                {
                    Chat.SendYxModMsgServer(human.player.host, Chat.YxModMsgStr("kejiquanxian"), human.GetExt().kejiquanxian ? "1" : "0");

                }
            }
        }

        private static void AllLiaoTianKuangQuanXian()
        {
            if (NetGame.isServer)
            {
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().liaotiankuangquanxian = allliaotiankuangquanxian ;
                    }
                    Chat.TiShi($"所有玩家 的 聊天框权限 已 {(allliaotiankuangquanxian ? "打开" : "关闭")}");
                }
            }
        }


        private static void GeRenDingDian()//个人定点
        {

            if (NetGame.isServer)
            {
                if(!UI_GongNeng.dingdian_KaiGuan)
                {

                    UI_GongNeng.dingdian_KaiGuan = true;
                    UI_GongNeng.DingDian();
                }

                if(humanID==0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().dingdian.kaiguan = alldingdian ;
                    }
                    Chat.TiShi($"所有玩家 的 个人定点 已 {(alldingdian ? "打开" : "关闭")}");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().dingdian.kaiguan ? "打开" : "关闭")}了 {human.player.host.name} 的个人定点");
                }
            
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("gerendingdian"), $"{humanID-1}");
            }
        }
        private static void WuJiaSi()//无假死
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.wujiasi_KaiGuan)
                {
                    UI_GongNeng.wujiasi_KaiGuan = true;
                    UI_GongNeng.WuJiaSi();
                }

                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().wujiasi = allwujiasi;
                    }
                    Chat.TiShi($"所有玩家 的 无假死 已 {(allwujiasi ? "打开" : "关闭")}");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().wujiasi ? "打开" : "关闭")}了 {human.player.host.name} 的无假死");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("wujiasi"), $"{humanID - 1}");
            }
        }
        private static void WuPengZhuang()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.wupengzhuang_KaiGuan)
                {
                    UI_GongNeng.wupengzhuang_KaiGuan = true;
                    UI_GongNeng.WuPengZhuang();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().wupengzhuang = allwupengzhuang;
                    }
                    Chat.TiShi($"所有玩家 的 无碰撞 已 {(allwupengzhuang ? "打开" : "关闭")}");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().wupengzhuang ? "打开" : "关闭")}了 {human.player.host.name} 的无碰撞");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("wupengzhuang"), $"{humanID - 1}");
            }
        }
        public static void FeiTian()//飞天
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.feitianxitong_KaiGuan)
                {
                    UI_GongNeng.feitianxitong_KaiGuan = true;
                    UI_GongNeng.FeiTianXiTong();
                }

                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().feitian = allfeitian;
                        YxMod.SetFeiTian(Human.all[i]);
                        if (Human.all[i].GetExt().feitian)
                        {
                            Chat.TiShi(Human.all[i].player.host, "普通情况下是正常飞天。按住左键，W，空格，保持两秒，可进入超人状态。");
                        }
                    }
                    Chat.TiShi($"所有玩家 都 {(allfeitian ? "学会了" : "忘记了")} 飞天");
                }
                else
                {
                    YxMod.SetFeiTian(human);
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().feitian ? "赋予" : "取消了")} {human.player.host.name} 飞天能力");
                    if (human.GetExt().feitian)
                    {
                        Chat.TiShi(human.player.host, "普通情况下是正常飞天。按住左键，W，空格，保持两秒，可进入超人状态。");
                    }
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("feitian"), $"{humanID - 1}");
            }
        }
        private static void ChaoRen()//超人
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.feitianxitong_KaiGuan)
                {
                    UI_GongNeng.feitianxitong_KaiGuan = true;
                    UI_GongNeng.FeiTianXiTong();
                }

                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().chaoren = allchaoren;
                    }
                    Chat.TiShi($"所有玩家 都 {(allchaoren ? "变成了 超人" : "恢复为人类")}");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().chaoren ? "赋予" : "取消了")} {human.player.host.name} 超人能力");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("chaoren"), $"{humanID - 1}");
            }
        }
        private static void ShanXian()//闪现
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.shanxianxitong_KaiGuan)
                {
                    UI_GongNeng.shanxianxitong_KaiGuan = true;
                    UI_GongNeng.ShanXianXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().shanxian = allshanxian;
                    }
                    Chat.TiShi($"所有玩家 都 {(allshanxian ? "学会了" : "忘记了")} 闪现");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().shanxian ? "赋予" : "取消了")} {human.player.host.name} 闪现能力");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("shanxian"), $"{humanID - 1}");
            }
        }

        private static void DongJie()//冻结
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }

                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                    
                        Human.all[i].GetExt().dongjie = alldongjie;
                        YxMod.DongJie(Human.all[i]);
                    }
                    Chat.TiShi($"所有玩家 都被 {(alldongjie ? "冻结" : "解冻")} 了");
                }
                else
                {
                    YxMod.DongJie(human);
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} 把 {human.player.host.name} {(human.GetExt().dongjie ? "冻结了" : "解冻了")}");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("dongjie"), $"{humanID - 1}");
            }
        }
        private static void BanShen()//半身不遂
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }

                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().banshen = allbanshen;
                        YxMod.BanShen(Human.all[i]);
                    }
                    Chat.TiShi($"所有玩家 都 {(allbanshen ? "学会" : "忘记")} 了 半身不遂");
                }
                else
                {
                    YxMod.BanShen(human);
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().banshen ? "赋予了" : "取消了")} {human.player.host.name} 半身不遂");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("banshen"), $"{humanID - 1}");
            }
        }

        private static void BengDi()//蹦迪
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {

                        Human.all[i].GetExt().bengdi = allbengdi;
                        YxMod.BengDi(Human.all[i]);
                    }
                    Chat.TiShi($"所有玩家 都 {(allbengdi ? "学会了" : "忘记了")} 蹦迪");
                }
                else
                {
                    YxMod.BengDi(human);
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().bengdi ? "赋予" : "取消了")} {human.player.host.name} 蹦迪能力");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("bengdi"), $"{humanID - 1}");
            }
        }
        private static void SanJiTiao()//三级跳
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().sanjitiao = allsanjitiao;
                    }
                    Chat.TiShi($"所有玩家 都 {(allsanjitiao ? "学会了" : "忘记了")} 三级跳");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().sanjitiao ? "赋予" : "取消了")} {human.player.host.name} 三级跳能力");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("sanjitiao"), $"{humanID - 1}");
            }
        }
        private static void DianTun()//电臀
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }

                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().diantun = alldiantun;
                        YxMod.DianTun(Human.all[i]);
                    }
                    Chat.TiShi($"所有玩家 都 {(alldiantun ? "学会了" : "忘记了")} 电臀");
                }
                else
                {
                    YxMod.DianTun(human);
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().diantun ? "赋予" : "取消了")} {human.player.host.name} 电臀能力");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("diantun"), $"{humanID - 1}");
            }
        }
        private static void QiQiu()//气球
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {

                        Human.all[i].GetExt().qiqiu = allqiqiu;
                        YxMod.QiQiu(Human.all[i]);
                    }
                    Chat.TiShi($"所有玩家 都 {(allqiqiu ? "变成了 气球" : "恢复了")}");
                }
                else
                {
                    YxMod.QiQiu(human);
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} 把 {human.player.host.name} {(human.GetExt().qiqiu ? "变成了气球" : "恢复正常")}");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("qiqiu"), $"{humanID - 1}");
            }
        }
        private static void QiQiuXiFa()//气球戏法
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().qiqiuxifa = allqiqiuxifa;
                    }
                    Chat.TiShi($"所有玩家 都 {(allqiqiuxifa ? "学会了" : "忘记了")} 左键抓住物品可以起飞的气球戏法");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().qiqiuxifa ? "赋予" : "取消了")} {human.player.host.name} 左键抓住物品可以起飞的气球戏法能力");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("qiqiuxifa"), $"{humanID - 1}");
            }
        }
        private static void DaoLi()//倒立
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().daoli = alldaoli;
                        YxMod.DaoLi(Human.all[i]);
                    }
                    Chat.TiShi($"所有玩家 都 {(alldaoli ? "学会了" : "忘记了")} 双手抓地使用倒立");
                }
                else
                {
                    YxMod.DaoLi(human);
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().daoli ? "赋予" : "取消了")} {human.player.host.name} 双手抓地使用倒立能力");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("daoli"), $"{humanID - 1}");
            }
        }
        private static void ZhuanQuan()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().zhuanquan = allzhuanquan;
                    }
                    Chat.TiShi($"所有玩家 都 {(allzhuanquan ? "学会了" : "忘记了")} 按住空格可以转圈圈");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().zhuanquan ? "赋予" : "取消了")} {human.player.host.name} 按住空格可以转圈圈能力");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("zhuanquan"), $"{humanID - 1}");
            }
        }
        private static void TuoLuo()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().tuoluo = alltuoluo;
                    }
                    Chat.TiShi($"所有玩家 都 {(alltuoluo ? "变成了 小陀螺" : "恢复了")} ");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} 把 {human.player.host.name} {(human.GetExt().tuoluo ? "变成了小陀螺" : "恢复正常")}");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("tuoluo"), $"{humanID - 1}");
            }
        }
        private static void KeTouGuai()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().ketouguai = allketouguai;
                    }
                    Chat.TiShi($"所有玩家 都 {(allketouguai ? "变成了 磕头怪" : "恢复了")}");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} 把 {human.player.host.name} {(human.GetExt().ketouguai ? "变成了磕头怪" : "恢复正常")}");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("ketouguai"), $"{humanID - 1}");
            }
        }
        private static void DiaoSiGui()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().diaosigui = alldiaosigui;
                        YxMod.DiaoSiGui(Human.all[i]);
                    }
                    Chat.TiShi($"所有玩家 都 {(alldiaosigui ? "变成了 吊死鬼" : "恢复了")}");
                }
                else
                {
                    YxMod.DiaoSiGui(human);
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} 把 {human.player.host.name} {(human.GetExt().diaosigui ? "变成了吊死鬼" : "恢复正常")}");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("diaosigui"), $"{humanID - 1}");
            }
        }
        private static void PangXie()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().pangxie = allpangxie;
                    }
                    Chat.TiShi($"所有玩家 都 {(allpangxie ? "变成了 大闸蟹" : "恢复了")}");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} 把 {human.player.host.name} {(human.GetExt().pangxie ? "变成了大闸蟹" : "恢复正常")}");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("pangxie"), $"{humanID - 1}");
            }
        }
        private static void ShouHua()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }

                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().shouhua = allshouhua;
                    }
                    Chat.TiShi($"所有玩家 都 {(allshouhua ? "手滑了！东西拿不稳" : "恢复了正常抓握")}");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} 让 {human.player.host.name} {(human.GetExt().shouhua ? "手滑啦！" : "重新稳住了")}");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("shouhua"), $"{humanID - 1}");
            }
        }

        private static void QianShui()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().qianshui = allqianshui;
                    }
                    Chat.TiShi($"所有玩家 都 {(allqianshui ? "学会了" : "忘记了")} 潜水");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().qianshui ? "赋予" : "取消了")} {human.player.host.name} 潜水能力");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("qianshui"), $"{humanID - 1}");
            }
        }
        private static void TuiQue()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }

                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().tuique = alltuique;
                        YxMod.TuiQue(Human.all[i]);
                    }
                    Chat.TiShi($"所有玩家 都 {(alltuique ? "瘸了一条腿" : "被治好了")}");
                }
                else
                {
                    YxMod.TuiQue(human);
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().tuique ? "打瘸了" : "治好了")} {human.player.host.name} 一条腿");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("tuique"), $"{humanID - 1}");
            }
        }
        private static void TuiGuai()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().tuiguai = alltuiguai;
                        YxMod.TuiGuai(Human.all[i]);
                    }
                    Chat.TiShi($"所有玩家 都 {(alltuiguai ? "获得了" : "丢掉了")} 拐杖");
                }
                else
                {
                    YxMod.TuiGuai(human);
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().tuiguai ? "送给" : "收回了")} {human.player.host.name} 一支拐杖");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("tuiguai"), $"{humanID - 1}");
            }
        }
        private static void ChaiChu()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().chaichu = allchaichu;
                        if (Human.all[i].GetExt().chaichu)
                        {
                            Chat.TiShi(Human.all[i].player.host, "开启拆除,左手抓住目标，即可拆卸。");
                        }
                    }
                    Chat.TiShi($"所有玩家 都 {(allchaichu ? "学会了" : "忘记了")} 拆除");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().chaichu ? "赋予" : "取消了")} {human.player.host.name} 拆除能力");
                    if (human.GetExt().chaichu)
                    {
                        Chat.TiShi(human.player.host, "开启拆除,左手抓住目标，即可拆卸。");
                    }
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("chaichu"), $"{humanID - 1}");
            }
        }
        private static void KongQiPao()//空气炮
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0)// 全员控制
                {
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().kongqipao = allkongqipao;
                        if (Human.all[i].GetExt().kongqipao)
                        {
                            Chat.TiShi(Human.all[i].player.host, "长按 鼠标左键 向前方打出空气炮，被击中的物体会被击飞。");
                        }
                    }
                    Chat.TiShi($"所有玩家 都 {(allkongqipao ? "获得了" : "丢掉了")} 空气炮");
                }
                else// 单个玩家控制
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().kongqipao ? "赋予" : "取消了")} {human.player.host.name} 空气炮能力");
                    if (human.GetExt().kongqipao)
                    {
                        Chat.TiShi(human.player.host, "长按 鼠标左键 向前方打出空气炮，被击中的物体会被击飞。");
                    }
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("kongqipao"), $"{humanID - 1}");
            }
        }

        private static void chaojitiao()//超级跳
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                if (!UI_GongNeng.yulexitong_KaiGuan)
                {
                    UI_GongNeng.yulexitong_KaiGuan = true;
                    UI_GongNeng.YuLeXiTong();
                }
                if (humanID == 0) // 全员控制
                {
                    YxMod.SuperJumpEnabled = allchaojitiao; // 同步全局状态
                    for (int i = 0; i < Human.all.Count; i++)
                    {
                        Human.all[i].GetExt().chaojitiao = allchaojitiao;
                        YxMod.chaojitiao(Human.all[i]); // 为每个玩家应用超级跳效果
                    }
                    Chat.TiShi($"所有玩家 都 {(allchaojitiao ? "学会了" : "忘记了")} 超级跳");
                }
                else // 单个玩家控制
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.GetExt().chaojitiao ? "赋予" : "取消了")} {human.player.host.name} 超级跳能力");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("chaojitiao"), $"{humanID - 1}");
            }
        }
        public static void HuanFu(NetPlayer netPlayer, RagdollPresetMetadata skin)
        {
            byte[] array;
            if (skin != null)
            {
                array = skin.GetSerialized();
            }
            else
            {
                Chat.TiShi(NetGame.instance.local, "原皮肤未加载");
                return;
                //if (((NetPlayer)netPlayer_1).host.inform.list_0.Count == 0)
                //{
                //    //smethod_47(("玩家[{0}]原皮肤未加载", (object)((NetPlayer)netPlayer_1).host.name));
                //    return;
                //}
                //array = ((NetPlayer)netPlayer_1).host.inform.list_0[0];
            }
            netPlayer.ApplyPreset(RagdollPresetMetadata.Deserialize(array), bake: false);
            NetStream netStream = NetGame.BeginMessage(NetMsgId.SendSkin);
            try
            {
                netStream.WriteNetId(netPlayer.localCoopIndex);
                netStream.Write(netPlayer.skinUserId);
                netStream.WriteArray(array, 32);
                if (NetGame.isClient)
                {
                    NetGame.instance.SendReliableToServer(netStream);
                }
                for (int i = 0; i < NetGame.instance.readyclients.Count; i++)
                {
                    NetHost netHost = NetGame.instance.readyclients[i];
                    if (netHost != null && netHost != NetGame.instance.local)
                    {
                        //ssr4 = true;
                        NetGame.instance.SendReliable(netHost, netStream);
                        //if (!ssr4)
                        //{
                        //    //("玩家[{0}]可见失败", (object)netHost.name));
                        //}
                        //else
                        //{
                        //    smethod_47("皮肤改变成功");
                        //}
                    }
                }
            }
            catch
            {
                Chat.TiShi(NetGame.instance.local, "皮肤改变失败");
            }
            finally
            {
                if (netStream != null)
                {
                    netStream = netStream.Release();
                }
            }
        }
        public static void ArrangeAllHumansFacing(Human leader)
        {
            List<Human> allHumans = Human.all;
            if (allHumans == null || leader == null) return;

            List<Human> followers = new List<Human>(allHumans);
            followers.Remove(leader); // 去掉队长

            float spacing = 1f;  // 间距
            float offsetY = -2f; // 悬浮高度

            // 使用队长摄像机方向作为参考
            float cameraYaw = leader.controls.cameraYawAngle;
            Vector3 right = Quaternion.Euler(0f, cameraYaw, 0f) * Vector3.right;

            Human prevLeft = leader; // 上一个挂左边的人
            Human prevRight = leader; // 上一个挂右边的人

            for (int i = 0; i < followers.Count; i++)
            {
                Human follower = followers[i];

                // 奇数（i=0,2,4...）挂左边，偶数（i=1,3,5...）挂右边
                bool isLeft = (i % 2 == 0);

                Vector3 offset = right * (isLeft ? -spacing : spacing);
                offset.y = offsetY;

                // 通过 SetGuaJian 来挂载，传入参数注意顺序
                YxMod.SetGuaJian(isLeft ? prevLeft : prevRight, follower);

                // 设置偏移量，挂载后更新
                follower.GetExt().ntp_Offset = offset;

                // 更新各侧的上一人
                if (isLeft)
                    prevLeft = follower;
                else
                    prevRight = follower;
            }
        }
        private void SuoFang(Human human)
        {
            //if (suofang)
            {
                if (human.ragdoll.partLeftHand.sensor.grabBody != null)
                {

                    if (Input.GetAxis("Mouse ScrollWheel") < 0f)
                    {

                        Transform transform = human.ragdoll.partLeftHand.sensor.grabBody.transform;
                        transform.localScale = transform.localScale / 1.2f;
                        return;
                    }
                    if (Input.GetAxis("Mouse ScrollWheel") > 0f)
                    {
                        Transform transform2 = human.ragdoll.partLeftHand.sensor.grabBody.transform;
                        transform2.localScale = transform2.localScale * 1.2f;

                    }
                }
            }
        }
        public static void SetHumanScaleByPart(
            Human human,
            float? head = null,
            float? torso = null,
            float? leftArm = null,
            float? rightArm = null,
            float? leftLeg = null,
            float? rightLeg = null,
            float? ball = null)
        {
            if (human == null) return;

            // 头
            if (head.HasValue)
                human.ragdoll.partHead.transform.localScale = Vector3.one * head.Value;

            // 躯干
            if (torso.HasValue)
            {
                human.ragdoll.partChest.transform.localScale = Vector3.one * torso.Value;
                human.ragdoll.partWaist.transform.localScale = Vector3.one * torso.Value;
                human.ragdoll.partHips.transform.localScale = Vector3.one * torso.Value;
            }

            // 左手
            if (leftArm.HasValue)
            {
                human.ragdoll.partLeftArm.transform.localScale = Vector3.one * leftArm.Value;
                human.ragdoll.partLeftForearm.transform.localScale = Vector3.one * leftArm.Value;
                human.ragdoll.partLeftHand.transform.localScale = Vector3.one * leftArm.Value;
            }

            // 右手
            if (rightArm.HasValue)
            {
                human.ragdoll.partRightArm.transform.localScale = Vector3.one * rightArm.Value;
                human.ragdoll.partRightForearm.transform.localScale = Vector3.one * rightArm.Value;
                human.ragdoll.partRightHand.transform.localScale = Vector3.one * rightArm.Value;
            }

            // 左腿
            if (leftLeg.HasValue)
            {
                human.ragdoll.partLeftThigh.transform.localScale = Vector3.one * leftLeg.Value;
                human.ragdoll.partLeftLeg.transform.localScale = Vector3.one * leftLeg.Value;
                human.ragdoll.partLeftFoot.transform.localScale = Vector3.one * leftLeg.Value;
            }

            // 右腿
            if (rightLeg.HasValue)
            {
                human.ragdoll.partRightThigh.transform.localScale = Vector3.one * rightLeg.Value;
                human.ragdoll.partRightLeg.transform.localScale = Vector3.one * rightLeg.Value;
                human.ragdoll.partRightFoot.transform.localScale = Vector3.one * rightLeg.Value;
            }

            // 球
            if (ball.HasValue)
                human.ragdoll.partBall.transform.localScale = Vector3.one * ball.Value;
        }
        private static void ScaleSync(Human human,
    float? head = null, float? torso = null, float? leftArm = null, float? rightArm = null,
    float? leftLeg = null, float? rightLeg = null, float? ball = null)
        {
            // 先本地应用一次（所有情况都需要）
            UI_WanJia.SetHumanScaleByPart(human,
                head ?? human.GetExt().scaleHead,
                torso ?? human.GetExt().scaleTorso,
                leftArm ?? human.GetExt().scaleLeftArm,
                rightArm ?? human.GetExt().scaleRightArm,
                leftLeg ?? human.GetExt().scaleLeftLeg,
                rightLeg ?? human.GetExt().scaleRightLeg,
                ball ?? human.GetExt().scaleBall);

            // 主机 → 广播
            if (NetGame.isServer)
            {
                string msg = $"{human.GetExt().scaleHead}|{human.GetExt().scaleTorso}|{human.GetExt().scaleLeftArm}|{human.GetExt().scaleRightArm}|{human.GetExt().scaleLeftLeg}|{human.GetExt().scaleRightLeg}|{human.GetExt().scaleBall}";
                foreach (var host in NetGame.instance.readyclients)
                {
                    if (host.players.Count > 0)
                    {
                        Human h = host.players[0].human;
                        if (h != null && h.GetExt().isClient) // 只发给装了 Mod 的
                        {
                            Chat.SendYxModMsgServer(host, Chat.YxModMsgStr("scale"),
                                $"{human.player.host.hostId};{msg}");
                        }
                    }
                }
            }
            // 客机 → 请求主机转发
            else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
            {
                string msg = $"{human.player.host.hostId};{human.GetExt().scaleHead}|{human.GetExt().scaleTorso}|{human.GetExt().scaleLeftArm}|{human.GetExt().scaleRightArm}|{human.GetExt().scaleLeftLeg}|{human.GetExt().scaleRightLeg}|{human.GetExt().scaleBall}";
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("scale"), msg);
            }
        }

        public static void DebugHumanRigidbodySync(Human human)
        {
            if (human == null || human.ragdoll == null)
            {
                UnityEngine.Debug.Log("Human 或 ragdoll 为 null");
                return;
            }

            void PrintPart(string name, HumanSegment segment)
            {
                if (segment == null)
                {
                    UnityEngine.Debug.Log($"{name}: segment is null");
                    return;
                }

                Rigidbody rb = segment.rigidbody;
                NetBody netBody = rb.GetComponent<NetBody>();
                string sync = netBody != null ? netBody.syncLocalScale.ToString() : "NetBody missing";

                UnityEngine.Debug.Log($"{name}: Rigidbody = {(rb != null ? "存在" : "null")}, syncLocalScale = {sync}");
            }

            PrintPart("Head", human.ragdoll.partHead);
            PrintPart("Chest", human.ragdoll.partChest);
            PrintPart("Waist", human.ragdoll.partWaist);
            PrintPart("Hips", human.ragdoll.partHips);

            PrintPart("LeftArm", human.ragdoll.partLeftArm);
            PrintPart("LeftForearm", human.ragdoll.partLeftForearm);
            PrintPart("LeftHand", human.ragdoll.partLeftHand);

            PrintPart("RightArm", human.ragdoll.partRightArm);
            PrintPart("RightForearm", human.ragdoll.partRightForearm);
            PrintPart("RightHand", human.ragdoll.partRightHand);

            PrintPart("LeftThigh", human.ragdoll.partLeftThigh);
            PrintPart("LeftLeg", human.ragdoll.partLeftLeg);
            PrintPart("LeftFoot", human.ragdoll.partLeftFoot);

            PrintPart("RightThigh", human.ragdoll.partRightThigh);
            PrintPart("RightLeg", human.ragdoll.partRightLeg);
            PrintPart("RightFoot", human.ragdoll.partRightFoot);

            PrintPart("Ball", human.ragdoll.partBall);
        }

    }
}
