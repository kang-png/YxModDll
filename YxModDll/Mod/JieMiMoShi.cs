using HumanAPI;
using Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace YxModDll.Mod
{
    public class JieMi_ZhaoBuTong : MonoBehaviour
    {

        [CompilerGenerated]
        private Action action_0;

        private GameObject gameObject_0;

        private List<GameObject> list_0 = new List<GameObject>();

        private List<GameObject> list_1 = new List<GameObject>();

        private List<GameObject> list_2 = new List<GameObject>();

        private float float_1;

        private bool bool_6;

        public static bool IsDecrypt;

        public static bool FindDifferent;

        public static JieMi_ZhaoBuTong instance;

        private void Awake()
        {
            JieMi_ZhaoBuTong.instance = this;
        }


        public static bool IsInView(Vector3 worldPos)
        {
            Transform transform = Camera.main.transform;
            Vector2 vector = Camera.main.WorldToViewportPoint(worldPos);
            Vector3 normalized = (worldPos - transform.position).normalized;
            return Vector3.Dot(transform.forward, normalized) > 0f && vector.x >= 0f && vector.x <= 1f && vector.y >= 0f && vector.y <= 1f;
        }

        private void OnGUI()
        {
            if ((NetGame.isServer || NetGame.isClient) && gameObject_0 != null && (this.list_0.Count != 0 || this.list_1.Count != 0 || this.list_2.Count != 0))
            {
                Human human = NetGame.instance.local.players[NetGame.instance.local.players.Count - 1].human;
                if (FindDifferent)
                {
                    float num = 8f;
                    for (int i = 0; i < this.list_0.Count; i++)
                    {
                        NodeGraph component = list_0[i].transform.GetComponent<NodeGraph>();
                        if (component != null && component.outputs.Count != 0)
                        {
                            float num2 = Vector3.Distance(human.transform.position, component.transform.position);
                            if (IsInView(component.transform.position) && Math.Round((double)num2, 2) < (double)num)
                            {
                                if (component != null && component.outputs.Count != 0 && component.outputs[0].output.value != 0f)
                                {
                                    Vector3 vector = Camera.main.WorldToScreenPoint(component.transform.position);

                                    GUI.Label(new Rect(vector.x - 50f, (float)Screen.height - vector.y, 200f, 35f), string.Concat(new object[]
                                    {
                                        "<size=20><Color=#00EC00>",
                                        "<不同点>",
                                        "</Color></size>"
                                    }));
                                }
                                else if (component != null && component.outputs.Count != 0 && component.outputs[0].output.value == 0f)
                                {
                                    Vector3 vector2 = Camera.main.WorldToScreenPoint(component.transform.position);
                                    GUI.Label(new Rect(vector2.x - 50f, (float)Screen.height - vector2.y, 200f, 35f), string.Concat(new object[]
                                    {
                                        "<size=20><Color=#FF0000>",
                                        "<不同点>",
                                        "</Color></size>"
                                    }));
                                }
                            }
                        }
                    }
                }
                if (IsDecrypt)
                {
                    for (int j = 0; j < this.list_1.Count; j++)
                    {
                        float num3 = 5f;
                        TriggerVolume component2 = this.list_1[j].transform.GetComponent<TriggerVolume>();
                        if (component2 != null)
                        {
                            float num4 = Vector3.Distance(human.transform.position, component2.transform.position);
                            if (IsInView(component2.transform.position) && Math.Round((double)num4, 2) < (double)num3)
                            {
                                if (component2.colliderToCheckFor != null && component2.output != null && component2.output.value != 0f)
                                {
                                    Vector3 vector3 = Camera.main.WorldToScreenPoint(component2.transform.position);
                                    GUI.Label(new Rect(vector3.x - 50f, (float)Screen.height - vector3.y, 200f, 35f), string.Concat(new object[]
                                    {
                                        "<size=20><Color=#00EC00>",
                                        "<" + component2.colliderToCheckFor.name + ">",
                                        "</Color></size>"
                                    }));
                                }
                                else if (component2.colliderToCheckFor != null)
                                {
                                    Vector3 vector4 = Camera.main.WorldToScreenPoint(component2.transform.position);
                                    GUI.Label(new Rect(vector4.x - 50f, (float)Screen.height - vector4.y, 200f, 35f), string.Concat(new object[]
                                    {
                                        "<size=20><Color=#FF0000>",
                                        "<" + component2.colliderToCheckFor.name + ">",
                                        "</Color></size>"
                                    }));
                                }
                            }
                        }
                    }
                    for (int k = 0; k < this.list_2.Count; k++)
                    {
                        TriggerVolume component3 = this.list_2[k].transform.GetComponent<TriggerVolume>();
                        if (component3 != null && component3.colliderToCheckFor != null)
                        {
                            float num5 = Vector3.Distance(human.transform.position, component3.colliderToCheckFor.transform.position);
                            if (IsInView(component3.colliderToCheckFor.transform.position) && Math.Round((double)num5, 2) < 150.0)
                            {
                                if (component3 != null && component3.colliderToCheckFor != null && component3.output != null && component3.output.value != 0f)
                                {
                                    Vector3 vector5 = Camera.main.WorldToScreenPoint(component3.colliderToCheckFor.transform.position);
                                    GUI.Label(new Rect(vector5.x - 50f, (float)Screen.height - vector5.y, 200f, 35f), string.Concat(new object[]
                                    {
                                        "<size=20><Color=#00EC00>",
                                        component3.colliderToCheckFor.transform.name,
                                        "</Color></size>"
                                    }));
                                }
                                else if (component3 != null && component3.colliderToCheckFor != null)
                                {
                                    Vector3 vector6 = Camera.main.WorldToScreenPoint(component3.colliderToCheckFor.transform.position);
                                    GUI.Label(new Rect(vector6.x - 50f, (float)Screen.height - vector6.y, 200f, 35f), string.Concat(new object[]
                                    {
                                        "<size=20><Color=#FF0000>",
                                        component3.colliderToCheckFor.transform.name,
                                        "</Color></size>"
                                    }));
                                }
                            }
                        }
                    }
                }
            }
        }


        private void FixedUpdate()
        {
            if (!NetGame.isServer && !NetGame.isClient)
            {
                FindDifferent = false;
                IsDecrypt = false;
            }
            this.UpdateObj2();
            if (Input.GetKey(KeyCode.M) && (NetGame.isServer || NetGame.isClient) && IsDecrypt)
            {
                this.float_1 += Time.fixedDeltaTime;
                if (this.float_1 > 2f && !this.bool_6)
                {
                    Human human = NetGame.instance.local.players[NetGame.instance.local.players.Count - 1].human;
                    this.bool_6 = true;
                    this.list_2.Clear();
                    for (int i = 0; i < this.list_1.Count; i++)
                    {
                        TriggerVolume component = this.list_1[i].transform.GetComponent<TriggerVolume>();
                        if (component != null)
                        {
                            float num = Vector3.Distance(human.transform.position, component.transform.position);
                            if (IsInView(component.transform.position) && Math.Round((double)num, 2) < 5.0 && component.colliderToCheckFor != null)
                            {
                                this.list_2.Add(component.transform.gameObject);
                            }
                        }
                    }
                    NetChat.Print("已获取视野内" + this.list_2.Count.ToString() + "个必须配件");
                    return;
                }
            }
            else
            {
                this.float_1 = 0f;
                this.bool_6 = false;
            }
        }


        //private void UpdateObj()
        //{
        //    if (!NetGame.isClient && !NetGame.isServer)
        //    {
        //        this.gameObject_0 = null;
        //        return;
        //    }
        //    if (this.gameObject_0 == null)
        //    {
        //        this.gameObject_0 = GameObject.Find("Level");
        //    }
        //    if (this.gameObject_0 != null && this.bool_4)
        //    {
        //        this.list_0 = new List<GameObject>();
        //        Transform[] componentsInChildren = this.gameObject_0.transform.GetComponentsInChildren<Transform>();
        //        for (int i = 0; i < componentsInChildren.Length; i++)
        //        {
        //            GameObject gameObject = componentsInChildren[i].transform.gameObject;
        //            Rigidbody componentInChildren = gameObject.transform.GetComponentInChildren<Rigidbody>();
        //            Collider componentInChildren2 = gameObject.transform.GetComponentInChildren<Collider>();
        //            NodeGraph componentInChildren3 = gameObject.transform.GetComponentInChildren<NodeGraph>();
        //            bool flag = true;
        //            if (this.bool_0 && componentInChildren == null)
        //            {
        //                flag = false;
        //            }
        //            if (this.bool_1 && componentInChildren2 == null)
        //            {
        //                flag = false;
        //            }
        //            if (this.bool_2 && componentInChildren != null && componentInChildren.isKinematic)
        //            {
        //                flag = false;
        //            }
        //            if (this.bool_5 && componentInChildren3 == null)
        //            {
        //                flag = false;
        //            }
        //            if ((this.string_0 != "" && flag && gameObject.name.Contains(this.string_0, StringComparison.OrdinalIgnoreCase) && flag && Vector3.Distance(NetGame.instance.local.players[0].human.transform.position, gameObject.transform.position) < 8f) || (this.string_0 == "" && flag && Vector3.Distance(NetGame.instance.local.players[0].human.transform.position, gameObject.transform.position) < 8f))
        //            {
        //                this.list_0.Add(gameObject.transform.gameObject);
        //            }
        //        }
        //        if (this.bool_3)
        //        {
        //            int count = this.list_0.Count;
        //            for (int j = 0; j < count; j++)
        //            {
        //                Transform[] componentsInChildren2 = this.list_0[j].transform.GetComponentsInChildren<Transform>();
        //                for (int k = 0; k < componentsInChildren2.Length; k++)
        //                {
        //                    if (componentsInChildren2[k].transform != this.list_0[j].transform && Vector3.Distance(NetGame.instance.local.players[0].human.transform.position, componentsInChildren2[k].transform.position) < 8f)
        //                    {
        //                        this.list_0.Add(componentsInChildren2[k].transform.gameObject);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}


        //private void Render()
        //{
        //    if ((NetGame.isServer || NetGame.isClient) && (this.string_0 != "" || this.bool_5))
        //    {
        //        if (this.string_0 != null || this.bool_5)
        //        {
        //            this.gameObject_1 = null;
        //        }
        //        if (this.list_0.Count != 0)
        //        {
        //            Class70.StartDrawing();
        //            for (int i = 0; i < this.list_0.Count; i++)
        //            {
        //                if (DrawPlayer.IsInView(this.list_0[i].transform.position))
        //                {
        //                    this.list_0[i].gameObject.Draw();
        //                }
        //            }
        //            Class70.EndDrawing();
        //            return;
        //        }
        //    }
        //    if ((NetGame.isServer || NetGame.isClient) && this.gameObject_1 != null && this.bool_5 && this.string_0 == "" && DrawPlayer.IsInView(this.gameObject_1.transform.position))
        //    {
        //        Class70.StartDrawing();
        //        this.gameObject_1.gameObject.Draw();
        //        Class70.EndDrawing();
        //    }
        //}


        private void UpdateObj2()
        {
            if (!NetGame.isClient && !NetGame.isServer)
            {
                this.gameObject_0 = null;
                if (!FindDifferent)
                {
                    this.list_0.Clear();
                }
                if (!IsDecrypt)
                {
                    this.list_2.Clear();
                    this.list_1.Clear();
                }
                return;
            }
            if (this.gameObject_0 == null)
            {
                this.gameObject_0 = GameObject.Find("Level");
                this.list_0.Clear();
                this.list_1.Clear();
                this.list_2.Clear();
            }
            if (this.gameObject_0 != null)
            {
                if (FindDifferent)
                {
                    this.list_0.Clear();
                    NodeGraph[] componentsInChildren = this.gameObject_0.transform.GetComponentsInChildren<NodeGraph>();
                    for (int i = 0; i < componentsInChildren.Length; i++)
                    {
                        if (componentsInChildren[i].outputs.Count != 0 && Vector3.Distance(NetGame.instance.local.players[0].human.transform.position, componentsInChildren[i].transform.position) < 8f)
                        {
                            this.list_0.Add(componentsInChildren[i].transform.gameObject);
                        }
                    }
                }
                if (IsDecrypt)
                {
                    this.list_1.Clear();
                    TriggerVolume[] componentsInChildren2 = this.gameObject_0.transform.GetComponentsInChildren<TriggerVolume>();
                    for (int j = 0; j < componentsInChildren2.Length; j++)
                    {
                        if (Vector3.Distance(NetGame.instance.local.players[0].human.transform.position, componentsInChildren2[j].transform.position) < 8f)
                        {
                            this.list_1.Add(componentsInChildren2[j].transform.gameObject);
                        }
                    }
                }
            }
        }


        //private void GUI_()
        //{
        //    if (this.bool_5 && this.gameObject_0 != null && this.list_0.Count != 0)
        //    {
        //        for (int i = 0; i < this.list_0.Count; i++)
        //        {
        //            Component human = NetGame.instance.local.players[NetGame.instance.local.players.Count - 1].human;
        //            NodeGraph component = this.list_0[i].transform.GetComponent<NodeGraph>();
        //            if (component != null && component.outputs.Count == 1)
        //            {
        //                float num = Vector3.Distance(human.transform.position, component.transform.position);
        //                if (Class70.IsInView(component.transform.position) && Math.Round((double)num, 2) < 8.0)
        //                {
        //                    if (component != null && component.outputs.Count != 0 && component.outputs[0].output.value != 0f)
        //                    {
        //                        Vector3 vector = Camera.main.WorldToScreenPoint(component.transform.position);
        //                        GUI.Label(new Rect(vector.x - 50f, (float)Screen.height - vector.y, 200f, 35f), string.Concat(new object[]
        //                        {
        //                            "<size=10><Color=#00EC00>",
        //                            component.transform.name,
        //                            " 距离:",
        //                            Convert.ToInt32(Math.Round((double)num, 2)),
        //                            "m</Color></size>"
        //                        }));
        //                    }
        //                    else if (component != null && component.outputs.Count != 0 && component.outputs[0].output.value == 0f)
        //                    {
        //                        Vector3 vector2 = Camera.main.WorldToScreenPoint(component.transform.position);
        //                        GUI.Label(new Rect(vector2.x - 50f, (float)Screen.height - vector2.y, 200f, 35f), string.Concat(new object[]
        //                        {
        //                            "<size=10><Color=#FF0000>",
        //                            component.transform.name,
        //                            " 距离:",
        //                            Convert.ToInt32(Math.Round((double)num, 2)),
        //                            "m</Color></size>"
        //                        }));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (UI.Home && this.gameObject_0 != null)
        //    {
        //        GUI.Box(new Rect(10f, (float)Screen.height - 320f, 270f, 320f), "");
        //        GUILayout.BeginArea(new Rect(20f, (float)Screen.height - 310f, 250f, 300f));
        //        this.bool_4 = GUILayout.Toggle(this.bool_4, "组件搜索" + ((!this.bool_4) ? " 关闭" : (" 开启 组件数:" + this.list_0.Count.ToString())), Array.Empty<GUILayoutOption>());
        //        if (this.bool_4)
        //        {
        //            string a = this.string_0;
        //            this.string_0 = GUILayout.TextField(this.string_0, Array.Empty<GUILayoutOption>());
        //            this.bool_0 = GUILayout.Toggle(this.bool_0, "刚体", Array.Empty<GUILayoutOption>());
        //            this.bool_1 = GUILayout.Toggle(this.bool_1, "碰撞器", Array.Empty<GUILayoutOption>());
        //            this.bool_5 = GUILayout.Toggle(this.bool_5, "搜索疑似解密", Array.Empty<GUILayoutOption>());
        //            if (a != this.string_0 || this.string_0 == "" || (!NetGame.isServer && !NetGame.isClient))
        //            {
        //                this.bool_3 = false;
        //            }
        //            if (this.string_0 != "" && (NetGame.isServer || NetGame.isClient))
        //            {
        //                this.bool_3 = GUILayout.Toggle(this.bool_3, "扫描子组件", Array.Empty<GUILayoutOption>());
        //            }
        //            if (this.bool_0)
        //            {
        //                this.bool_2 = GUILayout.Toggle(this.bool_2, "非运动", Array.Empty<GUILayoutOption>());
        //            }
        //            else
        //            {
        //                this.bool_2 = false;
        //            }
        //            this.vector2_0 = GUILayout.BeginScrollView(this.vector2_0, Array.Empty<GUILayoutOption>());
        //            for (int j = 0; j < this.list_0.Count; j++)
        //            {
        //                NodeGraph component2 = this.list_0[j].transform.GetComponent<NodeGraph>();
        //                if (component2 != null && component2.outputs.Count != 0)
        //                {
        //                    NetChat.Print(component2.transform.name + "抓到" + component2.outputs[0].output.value);
        //                }
        //                if (component2 != null && component2.outputs.Count == 0)
        //                {
        //                    NetChat.Print(component2.transform.name + "抓不到" + component2.outputs.Count.ToString());
        //                }
        //                if (GUILayout.Button(this.list_0[j].transform.name + ((component2 != null) ? "带Graph" : "空"), new GUILayoutOption[]
        //                {
        //                    GUILayout.Width(240f)
        //                }) && this.string_0 == "")
        //                {
        //                    this.gameObject_1 = this.list_0[j];
        //                }
        //            }
        //            GUILayout.EndScrollView();
        //        }
        //        else
        //        {
        //            this.gameObject_1 = null;
        //            this.list_0.Clear();
        //        }
        //        GUILayout.EndArea();
        //    }
        //}





    }
}
