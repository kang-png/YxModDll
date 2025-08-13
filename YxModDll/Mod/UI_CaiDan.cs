using HumanAPI;
using Multiplayer;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YxModDll.Mod.Features;
using static SteelSeriesHFF;



namespace YxModDll.Mod
{
    internal class UI_CaiDan
    {
        private static Vector2 scrollPosition;
        public static float gaodu;

        public static bool TouDeng;
        public static GameObject TouDengObject;
        public static bool QuanJuDeng;
        public static bool ZhenJiaTiChu;
        public static bool JieMiMoShi;
        public static bool ZhaoBuTongMoShi;
        public static bool JieSuoChengJiu;
        public static bool HuaBing;
        public static int chuansongzhiTop;
        public static int xuanfuyuTop;
        public static int qianshouTop;
        public static bool ChuFaQiXianShi;
        public static bool ZiDongShenShou;


        public static List<GameObject> 碰撞真渲染无 = new List<GameObject>();
        public static List<GameObject> 碰撞真渲染假 = new List<GameObject>();
        public static List<GameObject> 渲染真碰撞无 = new List<GameObject>();
        public static List<GameObject> 渲染真碰撞假 = new List<GameObject>();
        public static List<GameObject> 渲染真碰撞真触发真 = new List<GameObject>();
        public static List<GameObject> 渲染真碰撞真图层非碰撞图层 = new List<GameObject>();
        public static List<GameObject> 解密模式已修改 = new List<GameObject>();
        public static GameObject boxGameObject;
        public static GameObject sphereGameObject;

        public static void CreatUI()//创建菜单功能区
        {
            gaodu = 0;
            //scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域
            scrollPosition = GUILayout.BeginScrollView(scrollPosition); //开始滚动视图区域
            //UI.CreatAnNiu("继续", false, JiXu);
            //gaodu += UI.buttonHeight;

            if (NetGame.isServer || NetGame.isLocal || (NetGame.isClient))
            {
                UI.CreatAnNiu("载点(Ctrl+F)", false, ZaiRuCunDangDian_CaiDan);
                gaodu += UI.buttonHeight;
            }

            if (NetGame.isServer || NetGame.isLocal || (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian))
            {
                UI.CreatAnNiu("重新开始", false, ChongXinKaiShi_CaiDan);
                gaodu += UI.buttonHeight;
                UI.CreatAnNiu("上一关(PgUp)", false, ShangYiGuan_CaiDan);
                gaodu += UI.buttonHeight;
                UI.CreatAnNiu("下一关(PgDn)", false, XiaYiGuan_CaiDan);
                gaodu += UI.buttonHeight;
                UI.CreatAnNiu("重置物品(F3)", false, ChongZhiWuPin_CaiDan);
                gaodu += UI.buttonHeight;
                UI.CreatAnNiu("重置动画", false, ResetAllAnimations_CaiDan);
                gaodu += UI.buttonHeight;
                UI.CreatAnNiu("召集(F2)", false, JiHe_CaiDan);
                gaodu += UI.buttonHeight;
            }

            if (NetGame.isServer || (NetGame.isClient && YxMod.YxModServer))
            {
                UI.CreatAnNiu("传送至(C)", false, ChuanSongZhi);
                gaodu += UI.buttonHeight;
                chuansongzhiTop = (int)gaodu;

                UI.CreatAnNiu("悬浮于(X)", false, XuanFuYu);
                gaodu += UI.buttonHeight;
                xuanfuyuTop = (int)gaodu;

                UI.CreatAnNiu("牵手(Z)", false, QianShou);
                gaodu += UI.buttonHeight;
                qianshouTop = (int)gaodu;
            }

            if (NetGame.isServer || (NetGame.isClient))
            {
                UI.CreatAnNiu("邀请好友", false, YaoQingHaoYou);
                gaodu += UI.buttonHeight;
            }
            //if (NetGame.isServer || (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian))
            //{
            //    UI.CreatAnNiu("换图", false);
            //gaodu += UI.buttonHeight;
            //}
            if (NetGame.isServer && App.state == AppSate.ServerPlayLevel)
            {
                UI.CreatAnNiu("返回大厅", false, FanHuiDaTing);
                gaodu += UI.buttonHeight;
            }
            else if ((NetGame.isServer && App.state == AppSate.ServerLobby) || NetGame.isClient)//在大厅
            {
                UI.CreatAnNiu("离开游戏", false, LiKai);
                gaodu += UI.buttonHeight;
            }

            GUILayout.Space(10);
            gaodu += 10;

            if (NetGame.isServer || NetGame.isLocal || (NetGame.isClient))
            {
                GUILayout.BeginHorizontal();
                UI.CreatAnNiu("分身+1", false, AddFenShen);
                UI.CreatAnNiu("分身-1", false, JianFenShen);
                GUILayout.EndHorizontal();
                gaodu += UI.buttonHeight;

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu("平滑+1", false, AddSmooth);
                UI.CreatAnNiu("平滑-1", false, JianSmooth);
                GUILayout.EndHorizontal();
                gaodu += UI.buttonHeight;

                UI.CreatAnNiu_AnXia("自动伸手", ref FeatureManager.autoReach, false, null, "下落状态自动伸手");
                gaodu += UI.buttonHeight;

                UI.CreatAnNiu_AnXia("自动爬墙", ref FeatureManager.autoClimb, false, FeatureManager.TryInitAutoClimb, "举起双手,贴着墙面站好（双手碰到墙），用快捷键 Ctrl+N 触发");
                gaodu += UI.buttonHeight;

                UI.CreatAnNiu_AnXia("自动海豚跳", ref FeatureManager.autoBhop, false, null, "速度大于3时，判断转向，自动转最佳角度");
                gaodu += UI.buttonHeight;

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("头灯", ref TouDeng, false, OnTouDeng_CaiDan);
                UI.CreatAnNiu_AnXia("全局灯", ref QuanJuDeng, false, OnQuanJuDeng_CaiDan);
                GUILayout.EndHorizontal();
                gaodu += UI.buttonHeight;

                UI.CreatAnNiu_AnXia("真假剔除", ref ZhenJiaTiChu, false, ZhenJiaTiChu_Mod, "不要和触发器显示一起开");
                gaodu += UI.buttonHeight;

                UI.CreatAnNiu_AnXia("触发器显示", ref FeatureManager.showAirWall, false, FeatureManager.instance.RenderAirWalls, "会显示检测区域、按钮、拉杆等交互物体。不建议一直开着，会占用较多内存，只有换图时才能释放。");
                gaodu += UI.buttonHeight;

                UI.CreatAnNiu_AnXia("存档点显示", ref FeatureManager.showZoneVisuals, false, FeatureManager.instance.RenderZoneVisuals, "包括检查点、过关点、死亡点");
                gaodu += UI.buttonHeight;

                UI.CreatAnNiu_AnXia("解密模式", ref JieMi_ZhaoBuTong.IsDecrypt, false, null, "自动显示5米内的密码");
                gaodu += UI.buttonHeight;

                UI.CreatAnNiu_AnXia("找不同模式", ref JieMi_ZhaoBuTong.FindDifferent, false, null, "自动显示8米内的不同处");
                gaodu += UI.buttonHeight;

                UI.CreatAnNiu_AnXia("解锁成就", ref JieSuoChengJiu, false, JieSuoChengJiu_CaiDan);
                gaodu += UI.buttonHeight;
            }

            if (NetGame.isServer || (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian))
            {
                UI.CreatAnNiu_AnXia("滑冰图", ref HuaBing, false, HuaBingTu);
                gaodu += UI.buttonHeight;
                //UI.CreatAnNiu_AnXia("蹦迪图", ref BengDi, false);
            }
            GUILayout.Space(10);
            gaodu += 10;

            UI.CreatAnNiu("设置>>", false, CaiDan_DingDianSheZhi);
            //gaodu += UI.buttonHeight;
            //UI.CreatAnNiu("开房设置>>", false, CaiDan_KaiFangSheZhi);
            //gaodu += UI.buttonHeight;
            //UI.CreatAnNiu("聊天设置>>", false, CaiDan_LiaoTianSheZhi);
            //gaodu += UI.buttonHeight;
            //UI.CreatAnNiu("UI显示设置>>", false, CaiDan_UISheZhi);
            //gaodu += UI.buttonHeight;
            //UI.CreatAnNiu("游戏设置>>", false, CaiDan_YouXiSheZhi);
            //gaodu += UI.buttonHeight;
            //UI.CreatAnNiu("YxMod设置>>", false, CaiDan_YxModSheZhi);
            gaodu += UI.buttonHeight;

            GUILayout.EndScrollView();
            //GUILayout.FlexibleSpace();
        }


        private static void CaiDan_DingDianSheZhi()
        {
            UI_SheZhi.shezhiID = 0;
            UI_Main.ShowSheZhiUI=true;
        }
        private static void CaiDan_KaiFangSheZhi()
        {
            UI_SheZhi.shezhiID = 1;
            UI_Main.ShowSheZhiUI = true;
        }
        private static void CaiDan_LiaoTianSheZhi()
        {
            UI_SheZhi.shezhiID = 2;
            UI_Main.ShowSheZhiUI = true;
        }
        private static void CaiDan_UISheZhi()
        {
            UI_SheZhi.shezhiID = 3;
            UI_Main.ShowSheZhiUI = true;
        }
        private static void CaiDan_YouXiSheZhi()
        {
            UI_SheZhi.shezhiID = 4;
            UI_Main.ShowSheZhiUI = true;
        }
        private static void CaiDan_YxModSheZhi()
        {
            UI_SheZhi.shezhiID = 5;
            UI_Main.ShowSheZhiUI = true;
        }

        public static void Update()
        {
            if(!NetGame.isServer && !NetGame.isClient)
            {
                return;
            }
            if(TouDeng && TouDengObject != null)
            {
                TouDengObject.transform.position = GameObject.Find("Listener").transform.position + new Vector3(0f, 1f, 0f);
            }
        
        }
        public static void JiXu()
        {
            UI_Main.XianShiCaiDan(false);
        }
        public static void ZaiRuCunDangDian_CaiDan()
        {
            JiXu();
            ZaiRuCunDangDian();
        }
        public static void ZaiRuCunDangDian()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                Game.instance.Respawn(NetGame.instance.local.players[0].human, Vector3.zero);
                //Game.instance.RestartCheckpoint();//全部
            }
            else if (NetGame.isClient)
            {
                NetGame.instance.SendRequestRespawn();
                Game.instance.Resume();
            }
        }
        private static void ChongXinKaiShi_CaiDan()
        {
            JiXu();
            ChongXinKaiShi();
        }
        public static void ChongXinKaiShi()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                Game.instance.RestartLevel();
                Game.instance.Resume();
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 重新开始了游戏");
            }
            else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("cxks"));//重新开始
            
            }
        
        }
        private static void ShangYiGuan_CaiDan()
        {
            JiXu();
            ShangYiGuan();
        }
        public static void ShangYiGuan()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                YxMod.ShangYiGuan();
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 使游戏进入到第 {Game.instance.currentCheckpointNumber + 1} 关");
            }
            else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("syg"));//上一关
            }
        }

        private static void XiaYiGuan_CaiDan()
        {
            JiXu();
            XiaYiGuan();
        }
        public static void XiaYiGuan()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                YxMod.XiaYiGuan();
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 使游戏进入到第 {Game.instance.currentCheckpointNumber + 1} 关");
            }
            else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("xyg"));//下一关
            }
        
        }
        private static void ChongZhiWuPin_CaiDan()
        {
            JiXu();
            ChongZhiWuPin();
        }
        public static void ChongZhiWuPin()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                YxMod.ChongZhiWuPin();
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 重置了所有物品");
            }
            else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("czwp"));
            
            }
        
        }
        public static void ResetAllAnimations_CaiDan()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                ResetAllAnimations();
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 重置了所有动画");
            }
            else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("czdh"));
            
            }
        }
        public static void ResetAllAnimations()
        {
            int countAnimator = 0;
            int countAnimation = 0;

            // 处理 Animator
            Animator[] animators = GameObject.FindObjectsOfType<Animator>();
            foreach (var animator in animators)
            {
                if (!animator.enabled)
                    continue;

                animator.Rebind();
                countAnimator++;
            }

            // 处理 Animation
            Animation[] animations = GameObject.FindObjectsOfType<Animation>();
            foreach (var animation in animations)
            {
                if (!animation.enabled)
                    continue;

                animation.Rewind();
                countAnimation++;
            }

            //Debug.Log($"已重置 {countAnimator} 个 Animator，{countAnimation} 个 Animation 组件");
            Chat.TiShi(NetGame.instance.local, $"已重置 {countAnimator} 个 Animator，{countAnimation} 个 Animation 组件");
        }
        private static void JiHe_CaiDan()
        {
            JiXu();
            JiHe();
        }
        public static void JiHe()
        {
            if (NetGame.isServer)
            {
                Human human = NetGame.instance.local.players[0].human;
                YxMod.JiHe(human);
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 召集了所有玩家");
            }
            else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("jihe"));//集合
            
            }
        
        }
        private static void ChuanSongZhi()
        {
            UI_Main.ShowChuanSongUI =true;
            if (UI_Main.ShowChuanSongUI)
            {
                UI_Main.ShowXuanFuUI = UI_Main.ShowQianShouUI = false;

                UI_Main.chuansongUiRect.yMin = UI_Main.caidanUiRect.yMin + chuansongzhiTop + 35;

                if (NetGame.isServer)
                {
                    UI_ChuanSong.yuan_humanID = 1;
                }
                else if (NetGame.isClient)
                {
                    UI_ChuanSong.yuan_humanID = UI_WanJia.GetClientHumanID();
                }

            }
        }
        private static void XuanFuYu()
        {
            UI_Main.ShowXuanFuUI=true;
            if (UI_Main.ShowXuanFuUI)
            {
                UI_Main.ShowChuanSongUI = UI_Main.ShowQianShouUI = false;
                UI_Main.xuanfuUiRect.yMin = UI_Main.caidanUiRect.yMin + xuanfuyuTop + 35;
                if (NetGame.isServer)
                {
                    UI_XuanFu.yuan_humanID = 1;
                }
                else if (NetGame.isClient)
                {
                    UI_XuanFu.yuan_humanID = UI_WanJia.GetClientHumanID();
                }
            }
        }
        private static void QianShou()
        {
            UI_Main.ShowQianShouUI = true;
            if (UI_Main.ShowQianShouUI)
            {
                UI_Main.ShowChuanSongUI = UI_Main.ShowXuanFuUI = false;
                UI_Main.qianshouUiRect.yMin = UI_Main.caidanUiRect.yMin + qianshouTop + 35;
                if (NetGame.isServer)
                {
                    UI_QianShou.yuan_humanID = 1;
                }
                else if (NetGame.isClient)
                {
                    UI_QianShou.yuan_humanID = UI_WanJia.GetClientHumanID();
                }
            }
        }
        private static void YaoQingHaoYou()
        {
            JiXu();
            NetGame.instance.transport.SendInvite();
        }
        private static void LiKai()
        {
            //App.instance.TuiChu(false);
            JiXu();
            LiKaiYouXi();
        }
        public static void LiKaiYouXi()
        {
            if (App.state == AppSate.ClientLoadLevel || App.state == AppSate.ClientLoadLobby || App.state == AppSate.ServerLoadLevel || App.state == AppSate.ServerLoadLobby)
            {
                NetGame.instance.LeaveGame();
                Game.instance.singleRun = false;
                SteamUGC.StopPlaytimeTrackingForAllItems();
                Game.instance.UnloadLevel();
                //EnterMenu();
                MenuSystem.instance.ShowMainMenu<MainMenu>(true);
                return;
            }
            if (App.state == AppSate.PlayLevel || App.state == AppSate.ClientPlayLevel || App.state == AppSate.ServerPlayLevel || App.state == AppSate.ClientWaitServerLoad)
            {
                App.instance.PauseLeave(instantLeave: true);
            }
            if (App.isClient)
            {
                App.instance.LeaveLobby();
            }
            else if (App.isServer)
            {
                App.instance.StopServer();
            }

            //            NetGame.instance.LeaveGame();
            //            EnterMenu();
            //            MenuSystem.instance.ShowMainMenu();
        }
        private static void FanHuiDaTing()
        {
            //App.instance.TuiChu(true);
            JiXu();
            LiKaiYouXi();
        }
        private static void AddFenShen()
        {
            YxMod.Add_Local_Player("1");
        }
        private static void JianFenShen()
        {
            YxMod.Remove_Local_Player("1");
        }
        private static void AddSmooth()
        {
            OnCameraSmooth(((Options.cameraSmoothing / 20) + 1.0).ToString());
        }
        private static void JianSmooth()
        {
            OnCameraSmooth(((Options.cameraSmoothing / 20) - 1.0).ToString());
        }
        private static void OnCameraSmooth(string param)
        {
            if (!string.IsNullOrEmpty(param))
            {
                param = param.ToLowerInvariant();
                if (float.TryParse(param, out var result))
                {
                    Options.cameraSmoothing = (int)(result * 20f);
                }
                else if ("off".Equals(param))
                {
                    Options.cameraSmoothing = 0;
                }
                else if ("on".Equals(param))
                {
                    Options.cameraSmoothing = 20;
                }
                Chat.TiShi(NetGame.instance.local , "smooth变更为" + Options.cameraSmoothing / 20);
            }
        }
        private static void OnTouDeng_CaiDan()
        {
            OnTouDeng(TouDeng);
        }

        private static void OnTouDeng(bool DaKai = true)///头顶灯光源
        {
            TouDengObject = GameObject.Find("TouDeng");
            if (DaKai)
            {
                if(TouDengObject == null)
                {
                    TouDengObject = new GameObject("TouDeng");
                    TouDengObject.AddComponent<Light>();
                    TouDengObject.GetComponent<Light>().range = 20f;
                    TouDengObject.GetComponent<Light>().intensity = 1f;
                }
                //TouDengObject.transform.position = GameObject.Find("Listener").transform.position + new Vector3(0f, 1f, 0f);
                return;
            }
            //gameObject.transform.position = new Vector3(0f, 0f, 0f);
            UnityEngine.Object.Destroy(TouDengObject);
        }
        private static void OnQuanJuDeng_CaiDan()
        {
            OnQuanJuDeng(QuanJuDeng);
        }
        private static void OnQuanJuDeng(bool DaKai = true)///全局灯光源
        {
            GameObject gameObject = GameObject.Find("QuanJuDeng");
            if (DaKai)
            {
                if (gameObject == null)
                {
                    gameObject = new GameObject("QuanJuDeng");
                    gameObject.AddComponent<Light>();
                    gameObject.GetComponent<Light>().type = LightType.Directional;
                    gameObject.transform.rotation = Quaternion.Euler(60f, 30f, 0f);
                }
                return;
            }
            UnityEngine.Object.Destroy(gameObject);
        }
        public static void ZhenJiaTiChu_Mod()///真假剔除
        {
            if (ZhenJiaTiChu)
            {
                if (碰撞真渲染假.Count <= 0 && 碰撞真渲染无.Count <= 0 && 渲染真碰撞假.Count <= 0 && 渲染真碰撞无.Count <= 0 && 渲染真碰撞真触发真.Count <= 0 && 渲染真碰撞真图层非碰撞图层.Count <= 0)
                {
                    //ZhenJiaTiChu=true;
                    MeshRenderer[] componentsInChildren = (UnityEngine.Object.FindObjectOfType<BuiltinLevel>()).gameObject.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer meshRenderer in componentsInChildren)
                    {
                        if (meshRenderer.enabled && meshRenderer.gameObject.GetComponent<Collider>() == null)
                        {
                            meshRenderer.enabled = false;
                            渲染真碰撞无.Add(meshRenderer.gameObject);
                        }
                        else if (meshRenderer.enabled && !meshRenderer.gameObject.GetComponent<Collider>().enabled)
                        {
                            meshRenderer.enabled = false;
                            渲染真碰撞假.Add(meshRenderer.gameObject);
                        }
                        else if (meshRenderer.enabled && meshRenderer.gameObject.GetComponent<Collider>().enabled && meshRenderer.gameObject.GetComponent<Collider>().isTrigger)
                        {
                            meshRenderer.enabled = false;
                            渲染真碰撞真触发真.Add(meshRenderer.gameObject);
                        }
                        else if (meshRenderer.enabled && meshRenderer.gameObject.GetComponent<Collider>().enabled && !meshRenderer.gameObject.GetComponent<Collider>().isTrigger && !GameObjectLayerTest(meshRenderer.gameObject))
                        {
                            meshRenderer.enabled = false;
                            渲染真碰撞真触发真.Add(meshRenderer.gameObject);
                        }
                    }
                    Collider[] componentsInChildren2 = UnityEngine.Object.FindObjectOfType<BuiltinLevel>().gameObject.GetComponentsInChildren<Collider>(includeInactive: true);
                    foreach (Collider collider in componentsInChildren2)
                    {
                        if (!collider.enabled || collider.isTrigger || !GameObjectLayerTest(collider.gameObject))
                        {
                            continue;
                        }
                        if (collider.gameObject.GetComponent<MeshRenderer>() != null)
                        {
                            if (!collider.GetComponent<MeshRenderer>().enabled)
                            {
                                collider.GetComponent<MeshRenderer>().enabled = true;
                                碰撞真渲染假.Add(collider.gameObject);
                            }
                            continue;
                        }
                        if (collider.gameObject.GetComponent<MeshRenderer>() == null)
                        {
                            collider.gameObject.AddComponent<MeshRenderer>();
                        }
                        if (collider.gameObject.GetComponent<MeshFilter>() == null)
                        {
                            collider.gameObject.AddComponent<MeshFilter>();
                        }
                        if (collider as MeshCollider)
                        {
                            collider.gameObject.GetComponent<MeshFilter>().sharedMesh = ((MeshCollider)collider).sharedMesh;
                        }
                        if (collider as BoxCollider)
                        {
                            if (boxGameObject == null)
                            {
                                boxGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            }
                            collider.gameObject.GetComponent<MeshFilter>().sharedMesh = boxGameObject.GetComponent<MeshFilter>().sharedMesh;
                        }
                        if (collider as SphereCollider)
                        {
                            if (sphereGameObject == null)
                            {
                                sphereGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            }
                            collider.gameObject.GetComponent<MeshFilter>().sharedMesh = sphereGameObject.GetComponent<MeshFilter>().sharedMesh;
                        }
                        碰撞真渲染无.Add(collider.gameObject);
                    }
                    //Debug.Log(string.Concat(new object[12]
                    //{
                    //"碰撞真渲染假=", 碰撞真渲染假.Count, " 碰撞真渲染无=", 碰撞真渲染无.Count, " 渲染真碰撞假=", 渲染真碰撞假.Count, " 渲染真碰撞无=", 渲染真碰撞无.Count, " 渲染真碰撞真触发真=", 渲染真碰撞真触发真.Count,
                    //" 渲染真碰撞真图层非碰撞图层=", 渲染真碰撞真图层非碰撞图层.Count
                    //}));
                }

                Chat.TiShi(NetGame.instance.local, "真假剔除已打开");
                return;
            }
            //ZhenJiaTiChu = false;
            Chat.TiShi(NetGame.instance.local, "真假剔除已关闭");
            foreach (GameObject item in 碰撞真渲染假)
            {
                try
                {
                    item.GetComponent<MeshRenderer>().enabled = false;
                }
                catch
                {
                    break;
                }
            }
            碰撞真渲染假.Clear();
            foreach (GameObject item2 in 碰撞真渲染无)
            {
                try
                {
                    item2.GetComponent<MeshRenderer>().enabled = false;
                }
                catch
                {
                    break;
                }
            }
            碰撞真渲染无.Clear();
            foreach (GameObject item3 in 渲染真碰撞假)
            {
                try
                {
                    item3.GetComponent<MeshRenderer>().enabled = true;
                }
                catch
                {
                    break;
                }
            }
            渲染真碰撞假.Clear();
            foreach (GameObject item4 in 渲染真碰撞无)
            {
                try
                {
                    item4.GetComponent<MeshRenderer>().enabled = true;
                }
                catch
                {
                    break;
                }
            }
            渲染真碰撞无.Clear();
            foreach (GameObject item5 in 渲染真碰撞真触发真)
            {
                try
                {
                    item5.GetComponent<MeshRenderer>().enabled = true;
                }
                catch
                {
                    break;
                }
            }
            渲染真碰撞真触发真.Clear();
            foreach (GameObject item6 in 渲染真碰撞真图层非碰撞图层)
            {
                try
                {
                    item6.GetComponent<MeshRenderer>().enabled = true;
                }
                catch
                {
                    break;
                }
            }
            渲染真碰撞真图层非碰撞图层.Clear();
            //Debug.Log(string.Concat(new object[12]
            //{
            //    "碰撞真渲染假=", 碰撞真渲染假.Count, " 碰撞真渲染无=", 碰撞真渲染无.Count, " 渲染真碰撞假=", 渲染真碰撞假.Count, " 渲染真碰撞无=", 渲染真碰撞无.Count, " 渲染真碰撞真触发真=", 渲染真碰撞真触发真.Count,
            //    " 渲染真碰撞真图层非碰撞图层=", 渲染真碰撞真图层非碰撞图层.Count
            //}));
        }
        public static void JieMiMoShi_Mod()
        {
            if (JieMiMoShi)
            {
                if (解密模式已修改.Count == 0)
                {
                    // 找到场景里的 Level 对象
                    GameObject levelObject = GameObject.Find("Level");
                    if (levelObject == null)
                    {
                        Chat.TiShi(NetGame.instance.local, "未找到 Level 对象");
                        return;
                    }

                    // 获取所有 TriggerVolume 组件
                    TriggerVolume[] allTriggers = levelObject.GetComponentsInChildren<TriggerVolume>();
                    if (allTriggers == null || allTriggers.Length == 0)
                    {
                        Chat.TiShi(NetGame.instance.local, "未找到任何触发器");
                        return;
                    }

                    // 获取玩家当前坐标
                    var player = NetGame.instance.local?.players?[0]?.human;
                    if (player == null)
                    {
                        Chat.TiShi(NetGame.instance.local, "未找到玩家");
                        return;
                    }

                    Vector3 playerPosition = player.transform.position;

                    foreach (TriggerVolume trigger in allTriggers)
                    {
                        float distance = Vector3.Distance(playerPosition, trigger.transform.position);
                        if (Camera.main != null)
                        {
                            Vector3 toTarget = (trigger.transform.position - Camera.main.transform.position).normalized;
                            Vector3 forward = Camera.main.transform.forward;
                            Vector3 viewportPos = Camera.main.WorldToViewportPoint(trigger.transform.position);

                            bool inFront = Vector3.Dot(forward, toTarget) > 0f;
                            bool inViewport = viewportPos.x >= 0f && viewportPos.x <= 1f && viewportPos.y >= 0f && viewportPos.y <= 1f;

                            if (inFront && inViewport && distance < 150f)
                            {
                                string color = (trigger.output != null && trigger.output.value != 0f) ? "#00EC00" : "#FF0000";
                                Vector3 screenPos = Camera.main.WorldToScreenPoint(trigger.transform.position);
                                GUI.Label(
                                    new Rect(screenPos.x - 50f, Screen.height - screenPos.y, 200f, 35f),
                                    $"<size=10><color={color}><{trigger.colliderToCheckFor?.name ?? "Trigger"}> 距离:{Convert.ToInt32(Math.Round(distance, 2))}m</color></size>"
                                );
                            }
                        }
                    }

                    Chat.TiShi(NetGame.instance.local, $"解密模式开启，处理了 {解密模式已修改.Count} 个触发器");
                }
                else
                {
                    Chat.TiShi(NetGame.instance.local, "解密模式已重复开启，无需再次处理");
                }
            }
            else
            {
                // 关闭逻辑：恢复所有之前修改过的
                foreach (GameObject go in 解密模式已修改)
                {
                    try
                    {
                        if (go != null)
                            go.SetActive(true);
                    }
                    catch { }
                }
                解密模式已修改.Clear();
                Chat.TiShi(NetGame.instance.local, "解密模式已关闭，已还原所有修改");
            }
        }
        public static bool GameObjectLayerTest(GameObject gameObject)
        {
            if (gameObject.layer != 1 && gameObject.layer != 1 && gameObject.layer != 4 && gameObject.layer != 8 && gameObject.layer != 11 && gameObject.layer != 12 && gameObject.layer != 15 && gameObject.layer != 17)
            {
                return gameObject.layer != 19;
            }
            return false;
        }
        private static void JieSuoChengJiu_CaiDan()
        {
            if(JieSuoChengJiu)
            {
                unlock();
            }
            //else
            //{
            //    StatsAndAchievements.ResetAchievements();
            //    Chat.TiShi(NetGame.instance.local, "成就已重置");
            //}
        }
        public static void unlock()//解锁成就
        {
            IEnumerator enumerator = Enum.GetValues(Type.GetTypeFromHandle(typeof(Achievement).TypeHandle)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    StatsAndAchievements.UnlockAchievement((Achievement)enumerator.Current);
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
        private static void HuaBingTu()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                HuaBingTu_Fun(HuaBing);
                if(HuaBing)
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} 把地图变成了滑冰蹦迪地图");
                }
                else
                {
                    Chat.TiShi($"玩家 {NetGame.instance.local.name} 把地图恢复了原状");
                }
            }
            else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("huabing"));//集合
            }
        }

        public static void HuaBingTu_Fun(bool DaKai=true)
        {
            GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject gameObject in array)
            {
                if (!(Vector3.Distance(gameObject.transform.position, NetGame.instance.local.players[0].human.transform.position) < 1000f) || !(gameObject.GetComponentInParent<Human>() == null))
                {
                    continue;
                }
                Collider component = gameObject.GetComponent<Collider>();
                try
                {
                    if (component != null)
                    {
                        if (!DaKai)
                        {
                            component.material.bounceCombine = PhysicMaterialCombine.Minimum;
                            component.material.bounciness = 0f;
                            component.material.frictionCombine = PhysicMaterialCombine.Average;
                            component.material.staticFriction = 0.5f;
                            component.material.dynamicFriction = 0.5f;
                            component.sharedMaterial.staticFriction = 0.5f;
                            component.sharedMaterial.dynamicFriction = 0.5f;
                        }
                        else
                        {
                            component.material.bounceCombine = PhysicMaterialCombine.Maximum;
                            component.material.bounciness = 1f;
                            component.material.frictionCombine = PhysicMaterialCombine.Minimum;
                            component.material.staticFriction = 0f;
                            component.material.dynamicFriction = 0f;
                            component.sharedMaterial.staticFriction = 0f;
                            component.sharedMaterial.dynamicFriction = 0f;
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
}
