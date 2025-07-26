using Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YxModDll.Mod.Features;
using YxModDll.Patches;
namespace YxModDll.Mod
{
    public class WuTiGuaJian : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                SetWuTiGuaJian(Human.Localplayer);
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                QuXiaoWuTiGuaJian(Human.Localplayer);
            }

        }
        public void FixedUpdate()
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                foreach(Human human in Human.all)
                {
                    WuTiGuaJian_Fun(human);
                }
                
            }
        }


        public static void SetWuTiGuaJian(Human human)
        {
            if (NetGame.isServer || NetGame.isLocal)
            {
                

                Rigidbody wuti;
                GameObject grabObject = human.ragdoll.partLeftHand.sensor.grabObject;
                if (grabObject != null)
                {
                    wuti = grabObject.GetComponent<Rigidbody>();
                    // 检查抓取物体的父级是否有 Human 组件
                    Human componentInParent = grabObject.GetComponentInParent<Human>();
                    if (componentInParent != null)
                    {
                        //return;
                        //是human
                        wuti = componentInParent.GetComponent<Rigidbody>();
                    }
                    
                    
                    if (wuti == null)
                    {
                        Chat.TiShi(human.player.host, "物体无法设置为挂件，请更换物体");
                        return ;
                    }
                    QuXiaoWuTiGuaJian(human);
                    human.GetExt().ntp_wuti = wuti;

                    human.GetExt().wutiguajian = true;

                    wuti.isKinematic = true;

                    foreach (Rigidbody rigidbody in UnityEngine.Object.FindObjectsOfType(Type.GetTypeFromHandle(typeof(Rigidbody).TypeHandle)))
                    {
                        HuLue(human, wuti, rigidbody);
                    }
                }
                else
                {
                    Chat.TiShi(human.player.host, "左手没有抓住物体");
                }
            }
            else if(NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
            {
                Chat.SendYxModMsgClient(Chat.YxModMsgStr("wutiguajian"));
            }
        }
        private static void HuLue(Human human, Rigidbody wuti, Rigidbody targetRigidbody)
        {

            // 获取目标刚体的所有碰撞器
            Collider[] targetColliders = targetRigidbody.GetComponents<Collider>();

            foreach (Collider targetCollider in targetColliders)
            {
                Human componentInParent = targetCollider.GetComponentInParent<Human>();
                if (componentInParent != null)
                {
                    if (componentInParent == human)
                    {
                        IgnoreCollision.Ignore(componentInParent.transform, wuti.transform);
                        //break;
                    }
                }
                else
                {
                    IgnoreCollision.Ignore(targetCollider.transform, wuti.transform);

                }

            }
        }
        public static void QuXiaoWuTiGuaJian(Human human)
        {
            human.GetExt().wutiguajian = false;
            Rigidbody wuti = human.GetExt().ntp_wuti;

            if (wuti != null)
            {
                human.GetExt().ntp_wuti = null;
                QuXiaoXuanFu(wuti);
            }
        }
        private static void QuXiaoXuanFu(Rigidbody wuti)
        {
            wuti.isKinematic = false;
            foreach (Rigidbody rigidbody in UnityEngine.Object.FindObjectsOfType(Type.GetTypeFromHandle(typeof(Rigidbody).TypeHandle)))
            {
                bool isguajian = false;
                foreach (Human human2 in Human.all)
                {
                    if (human2.GetExt().ntp_wuti == rigidbody)
                    {
                        isguajian = true;
                        break;
                    }
                }
                if (!isguajian)
                {
                    IgnoreCollision.Unignore(rigidbody.transform, wuti.transform);
                }

            }

            //wuti.gameObject.GetComponent<NetBody>().Respawn();
        }

        private void WuTiGuaJian_Fun(Human human)
        {
            if (human.GetExt().ntp_wuti != null)
            {
                try
                {
                    //human.GetExt().ntp_wuti.isKinematic = true;
                    human.ReleaseGrab(human.GetExt().ntp_wuti.gameObject);

                    Vector3 position = human.transform.position;
                    position.y += 2.0f;
                    //human.ShuXing.ntp_wuti.MovePosition(position) ;
                    human.GetExt().ntp_wuti.position = position;


                    //// 定义旋转力矩（单位：牛顿·米）
                    //Vector3 torque = new Vector3(0f, 5f, 0f); // 绕 Y 轴施加力矩
                    //// 添加力矩
                    //human.ShuXing.ntp_wuti.AddTorque(torque, ForceMode.Force);



                    // 获取 human 的线速度
                    Vector3 velocity = human.rigidbodies[0].velocity;

                    // 计算速度的大小（标量）
                    float speed = velocity.magnitude;

                    // 将速度映射到旋转角度（例如：速度每增加 1，旋转角度增加 45 度）
                    float rotationSpeedPerSecond = Mathf.Clamp(speed * 100f, 30f, 2000f); // 最大限制为 360 度/秒

                    // 计算当前帧的旋转增量
                    float rotationThisFrame = rotationSpeedPerSecond * Time.deltaTime;

                    // 创建旋转增量（绕 Y 轴旋转）
                    Quaternion deltaRotation = Quaternion.Euler(0, 0, rotationThisFrame);

                    // 更新物体的旋转
                    human.GetExt().ntp_wuti.MoveRotation(human.GetExt().ntp_wuti.rotation * deltaRotation);




                    //if (human.controls.walkSpeed == 0f)
                    //{
                    //    // 每帧旋转一定角度
                    //    float rotationSpeed = 45f; // 每秒旋转的角度
                    //    Quaternion deltaRotation = Quaternion.Euler(0, 0, rotationSpeed * Time.fixedDeltaTime);

                    //    // 使用 MoveRotation 更新旋转
                    //    human.ShuXing.ntp_wuti.MoveRotation(human.ShuXing.ntp_wuti.rotation * deltaRotation);
                    //}
                    //else
                    //{
                    //    human.ShuXing.ntp_wuti.velocity = human.rigidbodies[0].velocity;
                    //    human.ShuXing.ntp_wuti.rotation = human.rigidbodies[0].rotation;
                    //}



                    //// 每帧旋转一定角度
                    //float rotationSpeed = 45f; // 每秒旋转的角度
                    //                           // 计算目标旋转角度（绕局部 Y 轴旋转）
                    //Quaternion deltaRotation = Quaternion.Euler( 0,  0,rotationSpeed * Time.fixedDeltaTime);

                    //// 使用 MoveRotation 更新旋转
                    //human.ShuXing.ntp_wuti.MoveRotation(human.ShuXing.ntp_wuti.rotation * deltaRotation);

                    //// 计算目标旋转角度
                    //Quaternion deltaRotation = Quaternion.Euler(Vector3.up * (rotationSpeed * Time.deltaTime));

                    //// 使用 MoveRotation 更新旋转
                    //human.ShuXing.ntp_wuti.MoveRotation(human.ShuXing.ntp_wuti.rotation * deltaRotation);
                }
                catch
                {
                    QuXiaoWuTiGuaJian(human);
                }
            }
            else
            {
                human.GetExt().wutiguajian = false;
            }
        }
    }
}
