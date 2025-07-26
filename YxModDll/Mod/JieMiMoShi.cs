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

        //[CompilerGenerated]

        private GameObject gameObject_0;

        private List<GameObject> list_0 = new List<GameObject>();

        private List<GameObject> list_1 = new List<GameObject>();

        private List<GameObject> list_2 = new List<GameObject>();

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
        }


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
    }
}
