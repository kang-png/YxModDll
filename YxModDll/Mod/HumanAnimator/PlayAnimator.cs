using Multiplayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YxModDll.Patches;

namespace YxModDll.Mod.HumanAnimator
{
    internal class PlayAnimator : MonoBehaviour
    {
        
        public void Update()
        {
            if (NetGame.isServer)
            {
                foreach (Human human in Human.all)
                {
                    if (human.controls.unconscious)
                    {
                        if (human.GetExt().ntp)
                            continue;

                        if (!UI_GongNeng.Y_KaiGuan)
                        {
                            human.GetExt().numY = 0;
                            if (human.GetExt().bofangdonghua)
                            {
                                human.GetExt().bofangdonghua = false;
                            }
                        }
                        // 根据当前 Y 动作编号（numY）执行对应功能
                        switch (human.GetExt().numY)
                        {
                            case 10:
                                if (!human.GetExt().bofangdonghua) // 防止重复启动
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.TuoMaSi);
                                }
                                break;
                            case 11:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.PiLiWuDongJie);
                                }
                                break;
                            case 12:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.JiaoChaTiaoYue);
                                }
                                break;
                            case 13:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.YangWoQiZuo);
                                }
                                break;
                            case 14:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.FuWoCheng);
                                }
                                break;
                            case 15:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.XiHaWu);
                                }
                                break;
                            case 16:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.XiHaWu2);
                                }
                                break;
                            case 17:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.XiHaWu3);
                                }
                                break;
                            case 18:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.TouXuan);
                                }
                                break;
                            case 19:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.MuMaTi);
                                }
                                break;
                            case 20:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.KaiHeTiao);
                                }
                                break;
                            case 21:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.YaoBaiWu);
                                }
                                break;
                            case 22:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.SangBaWu);
                                }
                                break;
                            case 23:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.SangBaWu2);
                                }
                                break;
                            case 24:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.DianTunWu);
                                }
                                break;
                            case 25:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.QuanJi);
                                }
                                break;
                            case 26:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.QiMaWu);
                                }
                                break;
                            case 27:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.HeiYingTaoWuBu);
                                }
                                break;
                            case 28:
                                if (!human.GetExt().bofangdonghua)
                                {
                                    human.GetExt().bofangdonghua = true;
                                    PlayAnimationFromFile(human, DONGZUO_State.ManPao,1.5f);
                                }
                                break;
                        }
                    }
                    else // 松开 Y 键时
                    {
                        if (human.GetExt().bofangdonghua)
                        {
                            human.GetExt().bofangdonghua = false;
                        }
                    }
                }
            }



        }
        public void PlayAnimationFromFile(Human human, DONGZUO_State type, float speed = 1.0f, bool loop = true)
        {
            if (human == null)
            {
                Debug.LogError("Human 为空！");
                return;
            }


            // ✅ 动态添加组件
            SmoothAnimator animator = human.gameObject.GetComponent<SmoothAnimator>();
            if (animator == null)
            {
                animator = human.gameObject.AddComponent<SmoothAnimator>();

            }
            animator.human = human;
            animator.playbackSpeed = speed;
            animator.loop = loop;
            animator.state = type;

        }

    }
}
