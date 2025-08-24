using Multiplayer;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using YxModDll.Patches;


namespace YxModDll.Mod
{
    public class c_BeiRen : MonoBehaviour
    {
        //public static Human bei_human;
        private void FixedUpdate()
        {
            foreach (Human human in Human.all)
            {

                if (!UI_GongNeng.guajianxitong_KaiGuan)
                {
                    if (human.GetExt().bei_human != null)
                    {
                        QuXiaoBeiRen(human);
                    }
                }
                else
                {
                    if (human.GetExt().bei_human != null)
                    {
                        human.transform.localPosition = new Vector3(0, -0.3f, -0.3f);
                        //human.transform.localRotation = Quaternion.Euler(0, 0, 0);

                    }
                }
                //QingChuWuXiaoHuman(human);

            }
        }
        private void QingChuWuXiaoHuman(Human human)
        {
            if (!GetHumans().Contains(human))
            {
                // 关键修改：如果当前角色是背负者，先取消对被背者的背负关系
                if (human.GetExt().bei_human != null)
                {
                    // 取消背负，解除父子关系
                    QuXiaoBeiRen(human);
                }
                if (human.transform.parent != null)
                {
                    human.transform.SetParent(null);
                }
                foreach (var rb in human.rigidbodies)
                {
                    rb.isKinematic = false;
                    rb.detectCollisions = true;
                    rb.useGravity = true;
                }

                Debug.Log("销毁无效human");
                Destroy(human.gameObject);
                Human.all.Remove(human);
            }
        }
        private void Update()
        {
            if(NetGame.isServer || NetGame.isClient)
            {
                int num1 = HotKey.Is2(HotKey.Bei).num1;
                int num2 = HotKey.Is2(HotKey.Bei).num2;
                if (num1 > -1 && num2 > -1)
                {
                    UI_Bei.Bei(num1, num2);
                }
            }
        }
        //// 确保QuXiaoBeiRen方法正确解除父子关系
        //public static void QuXiaoBeiRen(Human human)
        //{
        //    if (human == null) return;

        //    Human carried = human.GetExt().bei_human;
        //    if (carried != null)
        //    {
        //        // 解除被背者的父子关系（核心：避免被连带销毁）
        //        carried.transform.SetParent(null);
        //        // 恢复被背者的物理状态
        //        foreach (var rb in carried.rigidbodies)
        //        {
        //            rb.detectCollisions = true;
        //            rb.isKinematic = false;
        //            rb.useGravity = true;
        //        }
        //        // 清除背负关系记录
        //        carried.GetExt().bei_human = null;
        //    }
        //    // 清除当前角色的背负记录
        //    human.GetExt().bei_human = null;
        //}
        public static void QuXiaoBeiRen(Human human)
        {
            if(human==null)
            { return; }
            human.GetExt().bei_human = null;
            if (human.transform.parent != null)
            {
                human.transform.SetParent(null);
            }
            foreach (var rb in human.rigidbodies)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
                rb.useGravity = true;
            }

        }
        public static void BeiRen(Human human1, Human human2) //human1 背在 human2 身上
        {

            if (!UI_GongNeng.guajianxitong_KaiGuan)
            {
                Chat.TiShi(NetGame.instance.local, "挂件系统已关闭");
                return;
            }
            if (human1 == null || human2 == null || human1 == human2)
            {
                return;
            }
            if(human2.GetExt().bei_human == human1)
            {
                QuXiaoBeiRen(human2);
            }

            if (human1.GetExt(). bei_human == null)
            {
                YxMod.QuXiaoGuaJian(human1);//先取消挂件
                if(human2.GetExt().ntp_human = human1)
                {
                    YxMod.QuXiaoGuaJian(human2);
                }

                human1.GetExt().bei_human = human2;

                
                for (int i = 0; i < human1.rigidbodies.Length; i++)
                {
                    human1.rigidbodies[i].detectCollisions = false;
                    human1.rigidbodies[i].isKinematic = true;
                    human1.rigidbodies[i].useGravity = false;
                    human1.rigidbodies[i].velocity = Vector3.zero;
                    human1.rigidbodies[i].angularVelocity = Vector3.zero;

                    //human2.rigidbodies[i].transform.rotation = Quaternion.Euler(0, 0, 0);

                    human1.rigidbodies[i].transform.position = human2.rigidbodies[i].transform.position;
                    human1.rigidbodies[i].transform.rotation = human2.rigidbodies[i].transform.rotation;

                }

                human1.transform.SetParent(human2.ragdoll.partChest.transform);
                human1.transform.localPosition = new Vector3(0, -0.3f, -0.3f);

                BeCarriedPose(human1);

                return;
            }

            QuXiaoBeiRen(human1);

        }
        private static void BeCarriedPose(Human human)
        {
            // 1. 腿部姿势：两腿叉开
            // 左腿向外旋转
            human.ragdoll.partLeftThigh.transform.localRotation = Quaternion.Euler(30, 0, 50);
            human.ragdoll.partLeftLeg.transform.localRotation = Quaternion.Euler(-0, 0, 0);

            // 右腿向外旋转（与左腿对称）
            human.ragdoll.partRightThigh.transform.localRotation = Quaternion.Euler(30, 0, -50);
            human.ragdoll.partRightLeg.transform.localRotation = Quaternion.Euler(-0, 0, 0);

            // 2. 手臂姿势：弯曲并搂住脖子
            // 左手臂抬起并弯曲
            human.ragdoll.partLeftArm.transform.localRotation = Quaternion.Euler(10, 90, 110);// (0, 90, 110)   x：    y:内外    z:
            human.ragdoll.partLeftForearm.transform.localRotation = Quaternion.Euler(60, 0, 0);
            //human.ragdoll.partLeftHand.transform.localRotation = Quaternion.Euler(0, 0, 30); // 手自然弯曲

            // 右手臂抬起并弯曲（与左臂对称）
            human.ragdoll.partRightArm.transform.localRotation = Quaternion.Euler(-10, -90, -110);
            human.ragdoll.partRightForearm.transform.localRotation = Quaternion.Euler(60, 0, 0);
            //human.ragdoll.partRightHand.transform.localRotation = Quaternion.Euler(0, 0, -30);

            // 3. 头部微调：稍微后仰
            human.ragdoll.partHead.transform.localRotation = Quaternion.Euler(-30, 0, 0);


        }
        private static List<Human> GetHumans()
        {
            List<Human> list = new List<Human>();
            foreach (NetPlayer player in NetGame.instance.server.players)
            {
                list.Add(player.human);
            }
            foreach (NetHost host in NetGame.instance.readyclients)
            {
                foreach (NetPlayer player in host.players)
                {
                    list.Add(player.human);
                }
            }

            return list;
        }

    }


}
