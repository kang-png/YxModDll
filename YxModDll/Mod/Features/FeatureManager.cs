using HarmonyLib;
using HarmonyLib.Tools;
using HumanAPI;
using Microsoft.Win32;
using Multiplayer;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Windows.Interop;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static FileTools;

namespace YxModDll.Mod.Features
{
    public class FeatureManager : MonoBehaviour
    {
        public static bool flyCheat;

        public static bool kickCheat;

        public static bool pointCheat;

        public static bool debugCheat;

        public static bool enableHotkeys;

        public static bool xrayPlayers;

        public static bool objectsCheat;

        public static bool controlCheat;

        public static bool unconsciousCheat;

        public static bool achievementCheat;

        public static bool playersCheat;

        public static bool extraCheat;

        public static bool hotkeysCheat;

        public static bool guiOpened;

        public static bool objectsExtra;

        public static float scroll;

        public static float scrollPlayers;

        public static float scrollComponents;

        public static GameObject parent;

        public static GameObject selected;

        public static Human selectedPlayer;

        public static Human following;

        public static CSteamID previousLobbyID;

        public static float superJump;

        public static bool debugInfect;

        public static bool autoReach;

        public static bool liuhai;

        public static bool ignoreCollision;

        public static bool previewOnly;

        public static bool modifyHand;

        public static bool modifySpeed;

        public static bool minimap;

        public static bool modifyScale;

        public static bool noWorkshopReload;

        public static bool noDelay;

        public static bool loadingPreview;

        public static bool fixSkin;

        public static bool deleteFakeObjects;

        public static bool autoBhop;

        public static bool removeBugHuman;


        public static bool hookMode;

        public static bool followServer;

        public static bool newbieMode;

        public static bool strongConnect;

        public static bool quickPoint;

        public static bool advNewbieMode;

        public static bool advAirWall;

        public static int page;

        public static bool reach;

        public static Vector3 speed;

        public static float vspeed;

        public static Vector3 lastSpeed;

        public static int gameLevel;

        public static int lastCp;

        public static int lastHumanCount;

        public static string personaName;

        public static GameObject controlObject;

        public static float controlDistance;

        public static Vector3 controlStart;

        public static int netBodyCount;

        public static float initialTravel;

        public static float initialCarry;

        public static float initialClimb;

        public static float initialShip;

        public static float initialDrive;

        public static float initialDumpster;

        public static bool skinFixed;

        public static AudioSource bgm;

        public static SignalSoundPlayRandom randomBgm1;

        public static SignalScriptPlayaRandomSound1 randomBgm2;

        public static float bhopDir;

        public static int loadCheckpointState;

        public static bool autoClimb;

        public static int climbState;

        public static float? yawOverride;

        public static int jumpDir;

        public static bool aiMode;

        public static int pointState;

        public static string curSizeX;

        public static string curSizeY;

        public static string curSizeZ;

        public static string curLobbyMax;

        public static string curPersonaName;

        public static string curMass;

        public static string curInfectTime;

        public static string curSuperJump;

        public static string curSearch;

        public static string tasFile;

        public static string curHand;

        public static string curExtendedHand;

        public static string curSpeed;

        public static string curScale;

        public static string curGravity;

        public static bool bypassRichPresence;

        public static string levelName;

        public static NodeGraphViewer nodeGraphViewer;

        public static FreeRoamCam freeRoamCam;

        public static StackTrace stackTrace;

        public static Material glMaterial;

        public static Dictionary<Human, HumanAttribute> humans = new Dictionary<Human, HumanAttribute>();

        public static PerformanceCounter pc;

        public static Rect guiRect;

        public static Rect debugRect;

        public static Rect playersRect;

        public static Rect extraRect;

        public static Vector3 oldPos;

        public static GameObject cubePrimitive;

        public static GameObject cylinderPrimitive;

        public static GameObject spherePrimitive;

        public static GameObject markPrefab;

        public static List<GameObject> checkpointVisuals;

        public static List<GameObject> loadingZoneVisuals;

        public static List<GameObject> deathZoneVisuals;

        public static bool showCheckpoint;

        public static bool showLoadingZone;

        public static bool showDeathZone;

        public static bool showAirWall;

        public static bool showZoneVisuals;

        public static GameObject prefab;

        public static List<GameObject> instances = new List<GameObject>();

        public static List<GameObject> marks = new List<GameObject>();

        public static List<GameObject> airWallVisuals;

        public static Material fakeMaterial;

        public static GameObject mapObject;

        public static Camera mapCamera;

        public static RenderTexture mapTexture;

        public static float mapHeight;

        public static List<GameObject> objectsList = new List<GameObject>();

        public static string curText;

        public static Dictionary<Renderer, Material[]> fakeObjects = new Dictionary<Renderer, Material[]>();

        public static TriggerVolume[] triggerVolumes;

        public static GameObject[] triggerObjects;

        public static ColliderLabelTriggerVolume[] labelVolumes;

        public static ColliderLabel[] labelObjects;

        public static GameObject[] grabSensors;

        public static OtherCollisionSensor[] otherSensors;

        public static Material translucentMaterial;

        public static Coroutine loadCheckpointCoroutine;

        private Coroutine bhopCoroutine = null;

        public static readonly string[] exceptScenes = new string[2] { "DontDestroyOnLoad", "HideAndDontSave" };

        public static readonly Dictionary<Type, string> typeColors = new Dictionary<Type, string>
        {
            {
                typeof(short),
                "00FFFF"
            },
            {
                typeof(int),
                "00FFFF"
            },
            {
                typeof(long),
                "00FFFF"
            },
            {
                typeof(ushort),
                "00FFFF"
            },
            {
                typeof(uint),
                "00FFFF"
            },
            {
                typeof(ulong),
                "00FFFF"
            },
            {
                typeof(bool),
                "FFFF00"
            },
            {
                typeof(float),
                "FF00FF"
            },
            {
                typeof(double),
                "FF00FF"
            },
            {
                typeof(string),
                "00FF00"
            },
            {
                typeof(Vector3),
                "FF80FF"
            },
            {
                typeof(Vector2),
                "FF80FF"
            },
            {
                typeof(Quaternion),
                "FF80FF"
            },
            {
                typeof(Color),
                "008000"
            }
        };

        public const string version = "1.5.20.3";

        public const string guid = "com.plcc.hff.humanmod";

        public const int windowIdBase = 536705900;

        public static FeatureManager instance;

        public static Harmony harmony;

        public static int receiveCount;

        private bool cameraAdjusted;

        public static Camera MainCamera
        {
            get
            {
                if (FreeRoamCam.allowFreeRoam)
                {
                    if (freeRoamCam == null)
                    {
                        freeRoamCam = UnityEngine.Object.FindObjectOfType<FreeRoamCam>();
                    }
                    return Utils.cam.Invoke(freeRoamCam);
                }
                return Human.Localplayer.player.cameraController.gameCam;
            }
        }

        public void Start()
        {
            instance = this;
            xrayPlayers = false;
            removeBugHuman = PlayerPrefs.GetInt("removeBugHuman", 0) > 0;
            previewOnly = true;
            noWorkshopReload = false;
            noDelay = true;
            fixSkin = false;
            kickCheat = true;
            pointCheat = true;
            enableHotkeys = true;
            loadingPreview = true;
            pointState = 2;
            curHand = "0";
            curExtendedHand = "0.25";
            parent = (selected = null);
            previousLobbyID = new CSteamID((ulong)PlayerPrefs.GetInt("previousLobbyID"));
            superJump = 1f;
            guiRect = new Rect(10f, Screen.height - 330, 300f, 320f);
            int num = Screen.width * 85 / 128;
            int num2 = 285;
            debugRect = new Rect(num - num2, 90f, num2 * 2, 470f);
            playersRect = new Rect(Screen.width - 300, 200f, 300f, 290f);
            extraRect = new Rect(Screen.width - 300, 500f, 300f, 310f);
            checkpointVisuals = new List<GameObject>();
            loadingZoneVisuals = new List<GameObject>();
            deathZoneVisuals = new List<GameObject>();
            airWallVisuals = new List<GameObject>();
            cubePrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cylinderPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spherePrimitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            markPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            markPrefab.transform.localScale = 0.5f * Vector3.one;
            markPrefab.GetComponent<Renderer>().material.SetColor("_Color", new Color(1f, 0f, 1f, 1f));
            UnityEngine.Object.Destroy(markPrefab.GetComponent<Collider>());
            markPrefab.name = "Mark";
            UnityEngine.Object.DontDestroyOnLoad(markPrefab);
            markPrefab.SetActive(value: false);
            GameObject[] array = new GameObject[3] { cubePrimitive, cylinderPrimitive, spherePrimitive };
            foreach (GameObject gameObject in array)
            {
                gameObject.layer = LayerMask.NameToLayer("DefaultNoCam");
                gameObject.transform.localScale = Vector3.one;
                UnityEngine.Object.Destroy(gameObject.GetComponent<Collider>());
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                gameObject.SetActive(value: false);
                Renderer component = gameObject.GetComponent<Renderer>();
                SetRenderMode(component);
            }
            fakeMaterial = new Material(cubePrimitive.GetComponent<Renderer>().material);
            fakeMaterial.SetColor("_Color", new Color(1f, 1f, 0f, 0.33f));
            translucentMaterial = new Material(cubePrimitive.GetComponent<Renderer>().material);
            translucentMaterial.SetColor("_Color", new Color(0f, 0f, 1f, 0.33f));
            glMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
            stackTrace = new StackTrace();
            pc = new PerformanceCounter("Process", "Working Set - Private", Process.GetCurrentProcess().ProcessName);
            HarmonyFileLog.Enabled = false;
            harmony = Harmony.CreateAndPatchAll(typeof(FeatureManager), "com.plcc.hff.humanmod");
            // 确保外部补丁类也被注册（如黑名单入房拦截）
            Harmony.CreateAndPatchAll(typeof(YxModDll.Mod.BlacklistJoinPatch), "com.plcc.hff.humanmod");
        }

        public void Update()
        {
            if (Game.instance == null)
            {
                return;
            }
            if (MenuSystem.keyboardState == KeyboardState.None && enableHotkeys)
            {
                if (Input.GetKeyDown(KeyCode.H) && Human.Localplayer.hasGrabbed)
                {
                    foreach (GameObject grabbedObject in Utils.grabManager.Invoke(Human.Localplayer).grabbedObjects)
                    {
                        grabbedObject.SetActive(value: false);
                    }
                    Chat.TiShi(NetGame.instance.local, "已隐藏抓取物体");
                }
                if (Input.GetKeyDown(KeyCode.N) && Input.GetKey(KeyCode.LeftControl))
                {
                    autoClimb = !autoClimb;
                    if (autoClimb)
                    {
                        climbState = 0;
                        Vector3 vector = (Human.Localplayer.ragdoll.partLeftHand.transform.position - Human.Localplayer.ragdoll.partRightHand.transform.position).ZeroY();
                        float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
                        Human.Localplayer.controls.cameraYawAngle = (((Human.Localplayer.controls.cameraYawAngle - num) % 360f < 180f) ? (num + 90f) : (num - 90f));
                    }
                }
                //if (Input.GetKeyDown(KeyCode.R) && !Input.GetKey(KeyCode.LeftControl))
                if (HotKey.Is(HotKey.QuanJiZiShi) && !Input.GetKey(KeyCode.LeftControl))
                {
                    modifyHand = !modifyHand;
                    if (modifyHand)
                    {
                        Chat.TiShi(NetGame.instance.local, "已开启手部修改,按R键关闭");
                        curHand = "0.4";
                        curExtendedHand = "1.0";
                    }
                    else
                    {
                        //Chat.TiShi(NetGame.instance.local, "已关闭手部修改,按R键开启");
                        curHand = "0";
                        curExtendedHand = "1.0";
                    }
                }

                if (HotKey.Is(HotKey.AutoReach))
                {
                    autoReach = !autoReach; 
                    Chat.TiShi(NetGame.instance.local, $"按{HotKey.GetKeyName(HotKey.AutoReach)}键{(autoReach ? "开启" : "关闭")}自动伸手");
                }
                if (FreeRoamCam.allowFreeRoam && !cameraAdjusted)
                {
                    MainCamera.farClipPlane = UI_SheZhi.freeRoamCamDistance;
                    //UI_Main.ShowUI = false;
                    cameraAdjusted = true;
                    //UnityEngine.Debug.Log("[YxMod] FreeRoamCam 视距调整为 " + MainCamera.farClipPlane);
                }
                if (!FreeRoamCam.allowFreeRoam)
                {
                    cameraAdjusted = false;
                }
            }
            //if (gameLevel != Game.instance.currentLevelNumber)
            //{
            //    //RenderCheckpoints();
            //    //RenderLoadingZones();
            //    //RenderDeathZones();
            //    RenderZoneVisuals();
            //    RenderAirWalls();
            //    RenderFakeObjects();
            //    foreach (GameObject instance in instances)
            //    {
            //        UnityEngine.Object.Destroy(instance);
            //    }
            //    instances.Clear();
            //    lastCp = (from checkpoint in FindObjects<Checkpoint>()
            //              select checkpoint.number).MaxOr(0);
            //    netBodyCount = FindObjects<NetBody>().Count((NetBody netBody) => !exceptScenes.Contains(netBody.gameObject.scene.name));
            //    triggerVolumes = FindObjects<TriggerVolume>().ToArray();
            //    triggerObjects = triggerVolumes.SelectMany(GetTriggerObjects).Distinct().ToArray();
            //    labelVolumes = FindObjects<ColliderLabelTriggerVolume>().ToArray();
            //    labelObjects = FindObjects<ColliderLabel>().ToArray();
            //    grabSensors = (from c in ((IEnumerable<Component>)FindObjects<GrabSensor>()).Concat((IEnumerable<Component>)FindObjects<HumanAPI.Button>()).Concat(FindObjects<Lever>())
            //                   select c.gameObject).ToArray();
            //    otherSensors = FindObjects<OtherCollisionSensor>().ToArray();
            //    if (newbieMode)
            //    {
            //        CollectionExtensions.Do<LODGroup>(FindObjects<LODGroup>(), (Action<LODGroup>)UnityEngine.Object.Destroy);
            //    }
            //    gameLevel = Game.instance.currentLevelNumber;
            //}
            if (lastHumanCount != Human.all.Count)
            {
                IgnoreCollisionUpdate();
                lastHumanCount = Human.all.Count;
            }
            //if (NetGame.isServer || NetGame.isClient)
            //{
            //    previousLobbyID = ((NetTransportSteam)NetGame.instance.transport).lobbyID;
            //    PlayerPrefs.SetInt("previousLobbyID", (int)(ulong)previousLobbyID);
            //}
            //if (FreeRoamCam.allowFreeRoam && controlCheat)
            //{
            //    ControlObject();
            //}
            //if ((object)freeRoamCam == null)
            //{
            //    freeRoamCam = UnityEngine.Object.FindObjectOfType<FreeRoamCam>();
            //}
            //if ((object)nodeGraphViewer == null)
            //{
            //    nodeGraphViewer = ((Component)this).gameObject.AddComponent<NodeGraphViewer>();
            //}
            //foreach (Human item3 in Human.all)
            //{
            //    if (!item3.player.host.players.Contains(item3.player) && !item3.IsLocalPlayer && removeBugHuman)
            //    {
            //        Shell.Print("Removed Bug Human " + item3.player?.host.name);
            //        Human.all.Remove(item3);
            //        UnityEngine.Object.Destroy(item3.player);
            //    }
            //}
        }

        public void FixedUpdate()
        {
            if (Game.instance == null || Human.Localplayer == null)
            {
                return;
            }
            Vector3 position = Human.Localplayer.transform.position;
            reach = (position - oldPos).y < 0f;
            if (autoReach && reach)
            {
                Human.Localplayer.controls.leftExtend = 1f;
                Human.Localplayer.controls.rightExtend = 1f;
            }
            lastSpeed = speed;
            if (NetGame.isClient)
            {
                if (position != oldPos)
                {
                    speed = (position - oldPos).ZeroY() / Time.fixedDeltaTime;
                    vspeed = (position - oldPos).y / Time.fixedDeltaTime;
                }
            }
            else
            {
                speed = Human.Localplayer.velocity.ZeroY();
                vspeed = Human.Localplayer.velocity.y;
            }
            oldPos = position;
        }

        public void OnGUI()
        {
            if (xrayPlayers)
            {
                XrayPlayers();
            }
        }

        private int CalcSize(float distance)
        {
            return (int)Mathf.Clamp(20f - (distance - 5f) / 5f, 14f, 20f);
        }

        public void XrayPlayers()
        {
            if (Human.Localplayer == null)
            {
                return;
            }
            GUIStyle style = new GUIStyle
            {
                alignment = TextAnchor.UpperLeft
            };
            foreach (Human item7 in Human.all)
            {
                if (!item7.IsLocalPlayer || FreeRoamCam.allowFreeRoam)
                {
                    float num = Vector3.Distance(Human.Localplayer.transform.position, item7.transform.position);
                    if (IsTheFieldOfView(item7.transform.position))
                    {
                        Vector3 vector = MainCamera.WorldToScreenPoint(item7.transform.position);
                        int num2 = CalcSize(num);
                        GUI.Label(new Rect(vector.x - 50f, (float)Screen.height - vector.y, 200f, 35f), string.Format("<color=#{0}>{1} {2:0.0}m</color>", item7.IsLocalPlayer ? "00FF00" : "FF0000", item7.player.host.name, num), style);
                    }
                }
            }
            if (showLoadingZone && loadingZoneVisuals != null)
            {
                foreach (GameObject loadingZoneVisual in loadingZoneVisuals)
                {
                    if (!(loadingZoneVisual == null))
                    {
                        Vector3 position = loadingZoneVisual.transform.position;
                        if (IsTheFieldOfView(position))
                        {
                            float num3 = Vector3.Distance(Human.Localplayer.transform.position, position);
                            Vector3 vector2 = MainCamera.WorldToScreenPoint(position);
                            GUI.Label(new Rect(vector2.x - 50f, (float)Screen.height - vector2.y, 200f, 35f), $"<color=#00FF00>Pass {num3:0.0}m</color>", style);
                        }
                    }
                }
            }
            if (showCheckpoint && checkpointVisuals != null)
            {
                foreach (GameObject checkpointVisual in checkpointVisuals)
                {
                    if (!(checkpointVisual == null))
                    {
                        Vector3 position2 = checkpointVisual.transform.position;
                        Checkpoint component = checkpointVisual.transform.parent.GetComponent<Checkpoint>();
                        int num4 = component?.number ?? checkpointVisual.transform.parent.GetComponent<SignalTriggerCheckpoint>().checkpointNum;
                        int num5 = component?.subObjective ?? 0;
                        if (IsTheFieldOfView(position2))
                        {
                            float num6 = Vector3.Distance(Human.Localplayer.transform.position, position2);
                            Vector3 vector3 = MainCamera.WorldToScreenPoint(position2);
                            GUI.Label(new Rect(vector3.x - 50f, (float)Screen.height - vector3.y, 200f, 35f), string.Format("<color=#FF8000>CP{0}{1} {2:0.0}m</color>", num4, (num5 > 0) ? ((object)num5) : "", num6), style);
                        }
                    }
                }
            }
            Vector3 a = (FreeRoamCam.allowFreeRoam ? freeRoamCam.transform.position : Human.Localplayer.transform.position);
            if (newbieMode)
            {
                Dictionary<Vector3, List<string>> dictionary = new Dictionary<Vector3, List<string>>();
                TriggerVolume[] array = triggerVolumes;
                foreach (TriggerVolume triggerVolume in array)
                {
                    if (triggerVolume == null)
                    {
                        continue;
                    }
                    Vector3 position3 = triggerVolume.transform.position;
                    if (!(Vector3.Distance(a, position3) > 100f))
                    {
                        IEnumerable<GameObject> source = triggerVolume.additionalColliders;
                        if (triggerVolume.colliderToCheckFor != null)
                        {
                            source = source.Prepend(triggerVolume.colliderToCheckFor.gameObject);
                            //source = new[] { triggerVolume.colliderToCheckFor.gameObject }.Concat(source);
                        }
                        string item = ((source.Count() > 0) ? string.Join("/", source.Select((GameObject gameObject2) => gameObject2?.name ?? "null")) : "玩家");
                        if (!dictionary.ContainsKey(position3))
                        {
                            dictionary[position3] = new List<string>();
                        }
                        dictionary[position3].Add(item);
                    }
                }
                ColliderLabelTriggerVolume[] array2 = labelVolumes;
                foreach (ColliderLabelTriggerVolume colliderLabelTriggerVolume in array2)
                {
                    if (colliderLabelTriggerVolume == null)
                    {
                        continue;
                    }
                    Vector3 position4 = colliderLabelTriggerVolume.transform.position;
                    if (!(Vector3.Distance(a, position4) > 100f))
                    {
                        string item2 = string.Join("/", colliderLabelTriggerVolume.labelsToCheckFor.Select((string text) => "#" + text));
                        if (!dictionary.ContainsKey(position4))
                        {
                            dictionary[position4] = new List<string>();
                        }
                        dictionary[position4].Add(item2);
                    }
                }
                foreach (KeyValuePair<Vector3, List<string>> item8 in dictionary)
                {
                    if (IsTheFieldOfView(item8.Key))
                    {
                        Vector3 vector4 = MainCamera.WorldToScreenPoint(item8.Key);
                        GUI.Label(new Rect(vector4.x - 50f, (float)Screen.height - vector4.y, 200f, 35f), "<color=#FF00FF>" + string.Join(", ", item8.Value) + "</color>", style);
                    }
                }
                (UnityEngine.Object[], string, Func<UnityEngine.Object, string>)[] array3 = new (UnityEngine.Object[], string, Func<UnityEngine.Object, string>)[4];
                UnityEngine.Object[] item3 = triggerObjects;
                array3[0] = (item3, "00FFFF", (UnityEngine.Object x) => x.name);
                item3 = labelObjects;
                array3[1] = (item3, "00FFFF", (UnityEngine.Object x) => "#" + ((ColliderLabel)x).Label);
                item3 = grabSensors;
                array3[2] = (item3, "FFFF00", (UnityEngine.Object x) => (x is HumanAPI.Button button) ? (x.name + ((button.input.value >= 0.5f) ? "√" : "")) : x.name);
                item3 = otherSensors;
                array3[3] = (item3, "8000FF", (UnityEngine.Object x) => x.name);
                (UnityEngine.Object[], string, Func<UnityEngine.Object, string>)[] array4 = array3;
                for (int num8 = 0; num8 < array4.Length; num8++)
                {
                    (UnityEngine.Object[], string, Func<UnityEngine.Object, string>) tuple = array4[num8];
                    UnityEngine.Object[] item4 = tuple.Item1;
                    string item5 = tuple.Item2;
                    Func<UnityEngine.Object, string> item6 = tuple.Item3;
                    UnityEngine.Object[] array5 = item4;
                    foreach (UnityEngine.Object obj in array5)
                    {
                        if (!(obj == null))
                        {
                            if (1 == 0)
                            {
                            }
                            Transform transform = ((obj is GameObject gameObject) ? gameObject.transform : ((!(obj is Component component2)) ? null : component2.transform));
                            if (1 == 0)
                            {
                            }
                            Transform transform2 = transform;
                            float num10 = Vector3.Distance(a, transform2.position);
                            if (!(num10 > 100f) && IsTheFieldOfView(transform2.position))
                            {
                                Vector3 vector5 = MainCamera.WorldToScreenPoint(transform2.position);
                                GUI.Label(new Rect(vector5.x - 50f, (float)Screen.height - vector5.y, 200f, 35f), $"<size={CalcSize(num10)}><color=#{item5}>{item6(obj)}</color></size>", style);
                            }
                        }
                    }
                }
            }
            for (int num11 = 0; num11 < marks.Count; num11++)
            {
                if (IsTheFieldOfView(marks[num11].transform.position))
                {
                    float num12 = Vector3.Distance(Human.Localplayer.transform.position, marks[num11].transform.position);
                    Vector3 vector6 = MainCamera.WorldToScreenPoint(marks[num11].transform.position);
                    GUI.Label(new Rect(vector6.x - 50f, (float)Screen.height - vector6.y, 200f, 35f), $"<color=#FF00FF>#{num11 + 1} {num12:0.0}m</color>", style);
                }
            }
        }


        public static bool IsTheFieldOfView(Vector3 pos)
        {
            Transform transform = MainCamera.transform;
            Vector2 vector = MainCamera.WorldToViewportPoint(pos);
            Vector3 normalized = (pos - transform.position).normalized;
            return Vector3.Dot(transform.forward, normalized) > 0f && vector.x > 0f && vector.x < 1f && vector.y > 0f && vector.y < 1f;
        }

        public static Vector3 GetBoundsCenter(Collider collider)
        {
            if (1 == 0)
            {
            }
            Vector3 result = ((collider is BoxCollider boxCollider) ? boxCollider.center : ((collider is SphereCollider sphereCollider) ? sphereCollider.center : ((!(collider is CapsuleCollider capsuleCollider)) ? collider.bounds.center : capsuleCollider.center)));
            if (1 == 0)
            {
            }
            return result;
        }

        public static bool IsInvisible(Collider collider, bool advanced)
        {
            if (advAirWall)
            {
                return true;
            }
            if (collider.gameObject.layer == 31)
            {
                return true;
            }
            string[] source = new string[4] { "Hidden/ProBuilder/HideVertices", "GUI/Text Shader", "Legacy Shaders/Transparent/Diffuse", "Sprites/Mask" };
            MeshRenderer component = collider.GetComponent<MeshRenderer>();
            string value = component?.material?.shader?.name;
            if (component == null || !component.enabled || source.Contains(value))
            {
                return true;
            }
            string[] source2 = new string[3] { "Plane", "Quad", "LargeStoneFloorLargeTiled2" };
            MeshFilter component2 = collider.GetComponent<MeshFilter>();
            if (component2?.mesh == null)
            {
                return true;
            }
            string text = (advanced ? component2.mesh.name.TrimInstance() : null);
            if ((collider is BoxCollider boxCollider && ((advanced && text != "Cube") || boxCollider.size != Vector3.one || boxCollider.center != Vector3.zero)) || (collider is SphereCollider sphereCollider && ((advanced && text != "Sphere") || sphereCollider.radius != 0.5f || sphereCollider.center != Vector3.zero)))
            {
                return true;
            }
            if (collider is MeshCollider { sharedMesh: var sharedMesh } && sharedMesh != null)
            {
                if (advanced)
                {
                    string text2 = sharedMesh.name.TrimInstance();
                    if ((!text.Contains("Combined Mesh (root: scene)") && text2 != text) || source2.Contains(text2))
                    {
                        return true;
                    }
                }
                else if (source2.Any(sharedMesh.name.Contains))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsOpenMesh(Mesh mesh)
        {
            Dictionary<Side, int> dictionary = new Dictionary<Side, int>();
            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                dictionary.AddOrCreate(new Side(vertices[triangles[i]], vertices[triangles[i + 1]]));
                dictionary.AddOrCreate(new Side(vertices[triangles[i]], vertices[triangles[i + 2]]));
                dictionary.AddOrCreate(new Side(vertices[triangles[i + 1]], vertices[triangles[i + 2]]));
            }
            return dictionary.Values.Any((int x) => x % 2 != 0);
        }

        public static IEnumerable<T> FindObjects<T>() where T : Component
        {
            return from x in Resources.FindObjectsOfTypeAll<T>()
                   where x.gameObject.scene.name != null
                   select x;
        }

        public static GameObject[] GetTriggerObjects(TriggerVolume volume)
        {
            GameObject[] array = volume.additionalColliders ?? Array.Empty<GameObject>();
            IEnumerable<GameObject> source;
            if (!(volume.colliderToCheckFor != null))
            {
                IEnumerable<GameObject> enumerable = array;
                source = enumerable;
            }
            else
            {
                source = CollectionExtensions.AddItem<GameObject>((IEnumerable<GameObject>)array, volume.colliderToCheckFor.gameObject);
            }
            return source.ToArray();
        }

        public void DebugMesh(MeshFilter filter)
        {
            Mesh mesh = filter.mesh;
            mesh.MarkDynamic();
            Color[] array = new Color[mesh.vertexCount];
            Type nestedType = typeof(Mesh).GetNestedType("InternalShaderChannel");
            Vector3[] array2 = (Vector3[])Utils.GetAllocArrayFromChannelImpl.Invoke(mesh, new object[3] { 1, 0, 3 });
            for (int i = 0; i < array2.Length; i++)
            {
                Color[] array3 = array;
                int num = i;
                float num2 = Vector3.Angle(array2[i], Vector3.up);
                if (1 == 0)
                {
                }
                Color color = ((num2 < 45f) ? Color.blue : ((!(num2 < 60f)) ? Color.green : Color.red));
                if (1 == 0)
                {
                }
                array3[num] = color;
            }
            mesh.colors = array;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.UploadMeshData(markNoLogerReadable: false);
        }

        public static void ComponentInfo(Component component)
        {
            HashSet<string> hashSet = new HashSet<string> { "transform", "gameObject", "tag", "name", "hideFlags", "useGUILayout", "enabled", "isActiveAndEnabled" };
            FieldInfo[] fields = component.GetType().GetFields();
            PropertyInfo[] properties = component.GetType().GetProperties();
            curText = $"--- {fields.Length + properties.Length - 5} ---\n";
            for (int i = 0; i < fields.Length + properties.Length - 5; i++)
            {
                if (i >= fields.Length && properties[i - fields.Length].GetIndexParameters().Length != 0)
                {
                    continue;
                }
                string text = ((i < fields.Length) ? fields[i].Name : properties[i - fields.Length].Name);
                Type type = ((i < fields.Length) ? fields[i].FieldType : properties[i - fields.Length].PropertyType);
                object obj = ((i < fields.Length) ? fields[i].GetValue(component) : properties[i - fields.Length].GetValue(component, null));
                if (hashSet.Contains(text))
                {
                    continue;
                }
                if (typeColors.TryGetValue(type, out var value))
                {
                    curText += $"<color=#{value}>{type.Name} {text}: {obj}</color>\n";
                }
                else if (obj is NodeInput nodeInput)
                {
                    curText = curText + "<color=#FF0000>NodeInput " + nodeInput.name + ": " + GetNodeInputName(nodeInput) + "</color>\n";
                }
                else if (obj is NodeOutput nodeOutput)
                {
                    NodeGraph componentInParent = nodeOutput.node.GetComponentInParent<NodeGraph>();
                    if ((object)componentInParent != null)
                    {
                        curText = curText + "<color=#00FF00>NodeOutput " + nodeOutput.name + ": " + GetNodeOutputs(componentInParent, nodeOutput) + "</color>\n";
                    }
                }
                else if (obj is IEnumerable<NodeInput> source)
                {
                    curText = curText + "<color=#FF8000>NodeInput[] " + text + ": {" + string.Join(", ", source.Select(GetNodeInputName)) + "}</color>\n";
                }
                else if (obj is IEnumerable<NodeOutput> source2)
                {
                    NodeGraph nodeGraph = selected.GetComponentInParent<NodeGraph>();
                    if ((object)nodeGraph != null)
                    {
                        curText = curText + "<color=#FF8000>NodeOutput[] " + text + ": {" + string.Join(", ", source2.Select((NodeOutput x) => "(" + GetNodeOutputs(nodeGraph, x) + ")")) + "}</color>\n";
                    }
                }
                else if (obj is IEnumerable<object> values)
                {
                    Type[] genericArguments = type.GetGenericArguments();
                    curText = curText + "<color=#FF8000>" + (type.IsArray ? type.GetElementType().Name : genericArguments[0]?.Name) + "[] " + text + ": {" + string.Join(", ", values) + "}</color>\n";
                }
                else
                {
                    curText += string.Format("{0} {1}: {2}\n", type.Name, text, obj ?? "null");
                }
            }
            if (component is Human human)
            {
                curText = curText + Array.IndexOf(human.rigidbodies, human.GetComponent<Rigidbody>()) + "\n";
            }
            else if (component is NodeGraph nodeGraph2)
            {
                NodeGraph nodeGraph3 = nodeGraph2.transform.parent?.GetComponentInParent<NodeGraph>();
                if ((object)nodeGraph3 != null)
                {
                    curText += "<color=#00C0FF>Inputs:\n";
                    foreach (NodeGraphInput input in nodeGraph2.inputs)
                    {
                        curText = curText + input.name + ": " + GetNodeInputName(input.input) + " -> " + GetNodeOutputs(nodeGraph2, input.inputSocket) + "\n";
                    }
                    curText += "Outputs:\n";
                    foreach (NodeGraphOutput output in nodeGraph2.outputs)
                    {
                        curText = curText + output.name + ": " + GetNodeInputName(output.outputSocket) + " -> " + GetNodeOutputs(nodeGraph3, output.output) + "\n";
                    }
                    curText += "</color>";
                }
            }
            else if (component is SignalUnityEvent { triggerEvent: not null } signalUnityEvent)
            {
                curText = curText + "<color=#00C0FF>" + ShowUnityEvent(signalUnityEvent.triggerEvent) + "</color>";
            }
            else if (component is UnityEngine.UI.Button { onClick: not null } button)
            {
                curText = curText + "<color=#00C0FF>" + ShowUnityEvent(button.onClick) + "</color>";
            }
            Shell.Print(curText);
        }

        public static void SetFreeRoamCam()
        {
            if (!FreeRoamCam.allowFreeRoam)
            {
                Camera gameCam = NetGame.instance.local.players[0].cameraController.gameCam;
                freeRoamCam.transform.rotation = gameCam.transform.rotation;
                FreeRoamCam.allowFreeRoam = true;
                Utils.cam.Invoke(freeRoamCam) = MenuCameraEffects.instance.OverrideCamera(freeRoamCam.transform, applyEffects: true);
            }
        }


        public void IgnoreCollisionUpdate()
        {
            for (int i = 0; i < Human.all.Count; i++)
            {
                for (int j = i + 1; j < Human.all.Count; j++)
                {
                    if (ignoreCollision)
                    {
                        IgnoreCollision.Ignore(Human.all[i].transform, Human.all[j].transform);
                    }
                    else
                    {
                        IgnoreCollision.Unignore(Human.all[i].transform, Human.all[j].transform);
                    }
                }
            }
        }

        public void RefreshObjects()
        {
            if (string.IsNullOrEmpty(curSearch))
            {
                objectsList.Clear();
                objectsList.AddRange(from x in parent.transform.Children()
                                     select x.gameObject);
            }
            else
            {
                objectsList = GetAllChildren(Game.currentLevel.gameObject, search: true);
            }
        }

        public void RenderCheckpoints()
        {
            CollectionExtensions.Do<GameObject>((IEnumerable<GameObject>)checkpointVisuals, (Action<GameObject>)UnityEngine.Object.Destroy);
            checkpointVisuals.Clear();
            if (!showCheckpoint)
            {
                return;
            }
            foreach (Component item in ((IEnumerable<Component>)FindObjects<Checkpoint>()).Concat((IEnumerable<Component>)FindObjects<SignalTriggerCheckpoint>()))
            {
                if (!(item is Checkpoint { number: 0 }))
                {
                    checkpointVisuals.AddRange(from collider in item.GetComponents<Collider>()
                                               select GenerateHitboxVisual(collider, new Color(1f, 0.66f, 0f, 0.33f)));
                }
            }
        }

        public void RenderLoadingZones()
        {
            CollectionExtensions.Do<GameObject>((IEnumerable<GameObject>)loadingZoneVisuals, (Action<GameObject>)UnityEngine.Object.Destroy);
            loadingZoneVisuals.Clear();
            if (!showLoadingZone)
            {
                return;
            }
            foreach (LevelPassTrigger item in FindObjects<LevelPassTrigger>())
            {
                loadingZoneVisuals.AddRange(from collider in item.GetComponents<Collider>()
                                            select GenerateHitboxVisual(collider, new Color(0f, 1f, 0f, 0.33f)));
            }
        }

        public void RenderDeathZones()
        {
            CollectionExtensions.Do<GameObject>((IEnumerable<GameObject>)deathZoneVisuals, (Action<GameObject>)UnityEngine.Object.Destroy);
            deathZoneVisuals.Clear();
            if (!showDeathZone)
            {
                return;
            }
            foreach (FallTrigger item in FindObjects<FallTrigger>())
            {
                deathZoneVisuals.AddRange(from collider in item.GetComponents<Collider>()
                                          select GenerateHitboxVisual(collider, new Color(1f, 0f, 0f, 0.33f)));
            }
        }

        //public void RenderAirWalls()
        //{
        //	CollectionExtensions.Do<GameObject>((IEnumerable<GameObject>)airWallVisuals, (Action<GameObject>)UnityEngine.Object.Destroy);
        //	airWallVisuals.Clear();
        //	if (showAirWall)
        //	{
        //		((MonoBehaviour)this).StartCoroutine(RenderAirWallsCoroutine());
        //	}
        //}
        private Coroutine autoRefreshCoroutine;
        private float refreshInterval = 10f;

        public void RenderAirWalls()
        {
            // 清除旧的可视化物体
            CollectionExtensions.Do<GameObject>((IEnumerable<GameObject>)airWallVisuals, (Action<GameObject>)UnityEngine.Object.Destroy);
            airWallVisuals.Clear();

            // 如果开启了空气墙显示
            if (showAirWall)
            {
                // 防止重复开启协程
                if (autoRefreshCoroutine != null)
                {
                    ((MonoBehaviour)this).StopCoroutine(autoRefreshCoroutine);
                }

                // 启动自动刷新
                autoRefreshCoroutine = ((MonoBehaviour)this).StartCoroutine(AutoRefreshAirWalls());
            }
            else
            {
                // 如果关闭了空气墙显示，也关闭刷新协程
                if (autoRefreshCoroutine != null)
                {
                    ((MonoBehaviour)this).StopCoroutine(autoRefreshCoroutine);
                    autoRefreshCoroutine = null;
                }
            }
        }
        private IEnumerator AutoRefreshAirWalls()
        {
            while (true)
            {
                // 清除旧的可视化物体
                CollectionExtensions.Do<GameObject>((IEnumerable<GameObject>)airWallVisuals, (Action<GameObject>)UnityEngine.Object.Destroy);
                airWallVisuals.Clear();
                yield return RenderAirWallsCoroutine();
                yield return new WaitForSeconds(refreshInterval);
            }
        }


        public void RenderZoneVisuals()
        {
            // Checkpoints
            CollectionExtensions.Do<GameObject>(checkpointVisuals, UnityEngine.Object.Destroy);
            checkpointVisuals.Clear();
            if (showZoneVisuals)
            {
                foreach (Component item in ((IEnumerable<Component>)FindObjects<Checkpoint>()).Concat((IEnumerable<Component>)FindObjects<SignalTriggerCheckpoint>()))
                {
                    if (!(item is Checkpoint { number: 0 }))
                    {
                        checkpointVisuals.AddRange(from collider in item.GetComponents<Collider>()
                                                   select GenerateHitboxVisual(collider, new Color(1f, 0.66f, 0f, 0.33f)));
                    }
                }
            }

            // LoadingZones
            CollectionExtensions.Do<GameObject>(loadingZoneVisuals, UnityEngine.Object.Destroy);
            loadingZoneVisuals.Clear();
            if (showZoneVisuals)
            {
                foreach (LevelPassTrigger item in FindObjects<LevelPassTrigger>())
                {
                    loadingZoneVisuals.AddRange(from collider in item.GetComponents<Collider>()
                                                select GenerateHitboxVisual(collider, new Color(0f, 1f, 0f, 0.33f)));
                }
            }

            // DeathZones
            CollectionExtensions.Do<GameObject>(deathZoneVisuals, UnityEngine.Object.Destroy);
            deathZoneVisuals.Clear();
            if (showZoneVisuals)
            {
                foreach (FallTrigger item in FindObjects<FallTrigger>())
                {
                    deathZoneVisuals.AddRange(from collider in item.GetComponents<Collider>()
                                              select GenerateHitboxVisual(collider, new Color(1f, 0f, 0f, 0.33f)));
                }
            }
        }

        public IEnumerator RenderAirWallsCoroutine()
        {
            int counter = 0;
            Type[] components = new Type[8]
            {
                typeof(Checkpoint),
                typeof(LevelPassTrigger),
                typeof(FallTrigger),
                typeof(ReverbZone),
                typeof(AmbienceZone),
                typeof(Sound2),
                typeof(MusicPlayer),
                typeof(OptimisationVolume)
            };
            Type[] winds = new Type[3]
            {
                typeof(WindEffector),
                typeof(Wind2),
                typeof(ForceArea)
            };
            Vector3 center = Human.all.Count > 0 ? Human.all[0].transform.position : Vector3.zero;
            foreach (Collider collider in FindObjects<Collider>())
            {
                if ((collider.transform.position - center).sqrMagnitude > 100f * 100f)
                    continue;
                //MonoBehaviour.print(collider.name);
                collider.GetComponent<Renderer>();
                _ = collider.GetComponent<MeshCollider>()?.sharedMesh;
                _ = collider.GetComponents<Collider>().Length;
                if (IsInvisible(collider, advanced: false) && !exceptScenes.Contains(collider.gameObject.scene.name) && components.All((Type c) => collider.GetComponent(c) == null))
                {
                    GameObject obj = GenerateHitboxVisual(color: winds.Any((Type x) => collider.GetComponent(x) != null) ? new Color(0f, 1f, 1f, 0.33f) : (collider.isTrigger ? new Color(1f, 0f, 1f, 0.33f) : ((collider.gameObject.activeInHierarchy && collider.enabled) ? new Color(0f, 0f, 1f, 0.33f) : new Color(1f, 1f, 1f, 0.33f))), collider: collider);
                    if (obj != null)
                    {
                        airWallVisuals.Add(obj);
                    }
                }
                counter++;
                if (counter % 1000 == 0)
                {
                    yield return null;
                }
            }
            foreach (EasterGrab easterGrab in FindObjects<EasterGrab>())
            {
                GameObject obj2 = UnityEngine.Object.Instantiate(spherePrimitive, easterGrab.transform);
                obj2.transform.localScale = Vector3.one * easterGrab.dist * 2f;
                Material[] materials = obj2.GetComponent<Renderer>().materials;
                foreach (Material material in materials)
                {
                    material.SetColor("_Color", new Color(0.5f, 0f, 1f, 0.33f));
                }
                obj2.name = "Visual";
                obj2.SetActive(value: true);
                airWallVisuals.Add(obj2);
                counter++;
                if (counter % 1000 == 0)
                {
                    yield return null;
                }
            }
            List<Transform> waypoints = new List<Transform>();
            foreach (FollowWaypoints followWaypoints in FindObjects<FollowWaypoints>())
            {
                Transform[] waypoints2 = followWaypoints.waypoints;
                foreach (Transform waypoint in waypoints2)
                {
                    if (!waypoints.Contains(waypoint))
                    {
                        waypoints.Add(waypoint);
                    }
                }
            }
            using List<Transform>.Enumerator enumerator4 = waypoints.GetEnumerator();
            while (enumerator4.MoveNext())
            {
                GameObject obj3 = UnityEngine.Object.Instantiate(parent: enumerator4.Current.transform, original: spherePrimitive);
                obj3.transform.localScale = Vector3.one * 0.5f;
                Material[] materials2 = obj3.GetComponent<Renderer>().materials;
                foreach (Material material2 in materials2)
                {
                    material2.SetColor("_Color", new Color(0.5f, 0f, 1f, 0.33f));
                }
                obj3.name = "Visual";
                obj3.SetActive(value: true);
                airWallVisuals.Add(obj3);
                counter++;
                if (counter % 1000 == 0)
                {
                    yield return null;
                }
            }
        }

        public void RenderFakeObjects()
        {
            foreach (Renderer item in fakeObjects.Keys.ToList())
            {
                if (!item)
                {
                    fakeObjects.Remove(item);
                }
            }
            if (fakeObjects.Count == 0)
            {
                foreach (Renderer item2 in FindObjects<Renderer>())
                {
                    Collider[] components = item2.GetComponents<Collider>();
                    if (components.Length > 1)
                    {
                        continue;
                    }
                    Collider collider = components.ElementAtOrDefault(0);
                    if ((collider == null || !collider.enabled || collider.isTrigger || (0xE7F1FE65u & (1 << collider.gameObject.layer)) == 0L || (collider is MeshCollider meshCollider && meshCollider.sharedMesh == null)) && !exceptScenes.Contains(item2.gameObject.scene.name) && item2.gameObject.name != "Visual")
                    {
                        fakeObjects[item2] = new Material[item2.materials.Length];
                        for (int i = 0; i < item2.materials.Length; i++)
                        {
                            fakeObjects[item2][i] = UnityEngine.Object.Instantiate(item2.materials[i]);
                        }
                    }
                }
            }
            foreach (KeyValuePair<Renderer, Material[]> fakeObject in fakeObjects)
            {
                for (int j = 0; j < fakeObject.Key.materials.Length; j++)
                {
                    Material material = fakeObject.Key.materials[j];
                    if (deleteFakeObjects)
                    {
                        material.CopyPropertiesFromMaterial(fakeMaterial);
                    }
                    else
                    {
                        material.CopyPropertiesFromMaterial(fakeObject.Value[j]);
                    }
                }
            }
        }

        public GameObject GenerateHitboxVisual(Collider collider, Color color)
        {
            GameObject gameObject = null;
            if ((bool)collider)
            {
                if (collider is BoxCollider boxCollider)
                {
                    gameObject = UnityEngine.Object.Instantiate(cubePrimitive, collider.transform);
                    gameObject.transform.localScale = boxCollider.size;
                    gameObject.transform.position = boxCollider.bounds.center;
                }
                else if (collider is CapsuleCollider capsuleCollider)
                {
                    float num = capsuleCollider.height / 2f - capsuleCollider.radius;
                    gameObject = UnityEngine.Object.Instantiate(cylinderPrimitive, collider.transform);
                    gameObject.transform.localScale = new Vector3(capsuleCollider.radius * 2f, num, capsuleCollider.radius * 2f);
                    gameObject.transform.position = capsuleCollider.bounds.center;
                    GameObject gameObject2 = UnityEngine.Object.Instantiate(spherePrimitive, gameObject.transform);
                    gameObject2.transform.localPosition = Vector3.up;
                    gameObject2.transform.localScale = new Vector3(1f, capsuleCollider.radius * 2f / num, 1f);
                    Material[] materials = gameObject2.GetComponent<Renderer>().materials;
                    foreach (Material material in materials)
                    {
                        material.SetColor("_Color", color);
                    }
                    gameObject2.name = "Visual";
                    gameObject2.SetActive(value: true);
                    GameObject gameObject3 = UnityEngine.Object.Instantiate(spherePrimitive, gameObject.transform);
                    gameObject3.transform.localPosition = Vector3.down;
                    gameObject3.transform.localScale = new Vector3(1f, capsuleCollider.radius * 2f / num, 1f);
                    Material[] materials2 = gameObject3.GetComponent<Renderer>().materials;
                    foreach (Material material2 in materials2)
                    {
                        material2.SetColor("_Color", color);
                    }
                    gameObject3.name = "Visual";
                    gameObject3.SetActive(value: true);
                    Transform transform = gameObject.transform;
                    int direction = capsuleCollider.direction;
                    if (1 == 0)
                    {
                    }
                    Vector3 localEulerAngles = direction switch
                    {
                        0 => new Vector3(0f, 0f, 90f),
                        2 => new Vector3(90f, 0f, 0f),
                        _ => Vector3.zero,
                    };
                    if (1 == 0)
                    {
                    }
                    transform.localEulerAngles = localEulerAngles;
                }
                else if (collider is SphereCollider sphereCollider)
                {
                    gameObject = UnityEngine.Object.Instantiate(spherePrimitive, collider.transform);
                    gameObject.transform.localScale = Vector3.one * sphereCollider.radius * 2f;
                    gameObject.transform.position = sphereCollider.bounds.center;
                }
                else if (collider is MeshCollider meshCollider)
                {
                    gameObject = UnityEngine.Object.Instantiate(cubePrimitive, collider.transform);
                    Mesh sharedMesh = meshCollider.sharedMesh;
                    if (sharedMesh == null)
                    {
                        return null;
                    }
                    gameObject.GetComponent<MeshFilter>().mesh = sharedMesh;
                    if (sharedMesh.name.Contains("Plane") || sharedMesh.name.Contains("Quad") || sharedMesh.name.Contains("LargeStoneFloorLargeTiled2"))
                    {
                        GameObject gameObject4 = UnityEngine.Object.Instantiate(gameObject, gameObject.transform);
                        gameObject4.transform.localScale = -gameObject4.transform.localScale;
                        Material[] materials3 = gameObject4.GetComponent<Renderer>().materials;
                        foreach (Material material3 in materials3)
                        {
                            material3.SetColor("_Color", color);
                        }
                        gameObject4.name = "Visual";
                        gameObject4.SetActive(value: true);
                    }
                }
            }
            if (gameObject == null)
            {
                gameObject = UnityEngine.Object.Instantiate(cubePrimitive, collider.transform);
            }
            Material[] materials4 = gameObject.GetComponent<Renderer>().materials;
            foreach (Material material4 in materials4)
            {
                material4.SetColor("_Color", color);
            }
            gameObject.name = "Visual";
            gameObject.SetActive(value: true);
            return gameObject;
        }

        public GameObject GenerateHitboxVisual(GameObject gameObject, Color color)
        {
            return GenerateHitboxVisual(gameObject.GetComponent<Collider>(), color);
        }

        public static List<GameObject> GetAllChildren(GameObject gameObject, bool search)
        {
            List<GameObject> list = new List<GameObject>();
            if (search)
            {
                string[] array = curSearch.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                bool flag = true;
                for (int i = 0; i < array.Length; i++)
                {
                    if (gameObject == Game.currentLevel.gameObject)
                    {
                        flag = false;
                        break;
                    }
                    if (array[i][0] == '@')
                    {
                        bool flag2 = false;
                        Component[] components = gameObject.GetComponents<Component>();
                        for (int j = 0; j < components.Length; j++)
                        {
                            if (components[j] != null && components[j].GetType().ToString().ToLower()
                                .Contains(array[i].Substring(1).ToLower()))
                            {
                                flag2 = true;
                                break;
                            }
                        }
                        if (!flag2)
                        {
                            flag = false;
                            break;
                        }
                    }
                    else if (array[i][0] == '#')
                    {
                        bool flag3 = false;
                        if (uint.TryParse(array[i].Substring(1), out var result) && gameObject.TryGetComponent<NetIdentity>(out var component) && component.sceneId == result)
                        {
                            flag3 = true;
                        }
                        if (!flag3)
                        {
                            flag = false;
                            break;
                        }
                    }
                    else if (!gameObject.name.ToLower().Contains(array[i].ToLower()))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    list.Add(gameObject);
                }
            }
            else
            {
                list.Add(gameObject);
            }
            if (gameObject.transform.childCount > 0)
            {
                list.AddRange(gameObject.transform.Children().SelectMany((Transform x) => GetAllChildren(x.gameObject, search)));
            }
            return list;
        }

        public static string GetNodeInputName(NodeInput nodeInput)
        {
            return (nodeInput.connectedNode != null) ? (nodeInput.connectedNode.name + "." + nodeInput.connectedNode.GetType().Name + "." + nodeInput.connectedSocket) : $"{nodeInput.value}";
        }

        public static List<string> GetNodeOutputs(Node node, NodeOutput nodeOutput)
        {
            IEnumerable<NodeInput> source;
            if (node is NodeGraph nodeGraph)
            {
                List<NodeInput> list = new List<NodeInput>();
                foreach (NodeInput item in nodeGraph.inputs.Select((NodeGraphInput input) => input.input))
                {
                    list.Add(item);
                }
                foreach (NodeInput item2 in nodeGraph.outputs.Select((NodeGraphOutput output) => output.outputSocket))
                {
                    list.Add(item2);
                }
                source = new _003C_003Ez__ReadOnlyList<NodeInput>(list);
            }
            else
            {
                source = node.ListNodeSockets().OfType<NodeInput>();
            }
            return (from input in source
                    where input.GetConnectedOutput() == nodeOutput
                    select node.name + "." + node.GetType().Name + "." + input.name).ToList();
        }

        public static string GetNodeOutputs(NodeGraph nodeGraph, NodeOutput nodeOutput)
        {
            return string.Join(", ", nodeGraph.GetComponentsInChildren<Node>().SelectMany((Node x) => GetNodeOutputs(x, nodeOutput)));
        }

        public static string ShowUnityEvent(UnityEventBase unityEventBase)
        {
            string text = "";
            foreach (object item in Traverse.Create((object)unityEventBase).Field("m_PersistentCalls").Field("m_Calls")
                .GetValue() as IList)
            {
                Traverse val = Traverse.Create(item);
                UnityEngine.Object value = val.Property<UnityEngine.Object>("target", (object[])null).Value;
                string value2 = val.Property<string>("methodName", (object[])null).Value;
                text = ((!(value is GameObject gameObject)) ? ((!(value is Component component)) ? (text + "null") : (text + component.name + "." + component.GetType().Name)) : (text + gameObject.name));
                if (string.IsNullOrEmpty(value2))
                {
                    text += ".No Function\n";
                    continue;
                }
                Traverse val2 = val.Property("arguments", (object[])null);
                PersistentListenerMode value3 = val.Property<PersistentListenerMode>("mode", (object[])null).Value;
                if (1 == 0)
                {
                }
                string text2 = value3 switch
                {
                    PersistentListenerMode.Void => "",
                    PersistentListenerMode.Object => val2.Property<UnityEngine.Object>("unityObjectArgument", (object[])null).Value?.name ?? "null",
                    PersistentListenerMode.Int => val2.Property<int>("intArgument", (object[])null).Value.ToString(),
                    PersistentListenerMode.Float => val2.Property<float>("floatArgument", (object[])null).Value.ToString(),
                    PersistentListenerMode.String => val2.Property<string>("stringArgument", (object[])null).Value.ToString(),
                    PersistentListenerMode.Bool => val2.Property<bool>("boolArgument", (object[])null).Value.ToString(),
                    _ => "???",
                };
                if (1 == 0)
                {
                }
                string text3 = text2;
                text = text + "." + value2 + "(" + text3 + ")\n";
            }
            return text;
        }

        public string ShowNodes(GameObject parent)
        {
            curText = "";
            Dictionary<NodeOutput, int> dictionary = new Dictionary<NodeOutput, int>();
            Node[] componentsInChildren = parent.GetComponentsInChildren<Node>();
            Node[] array = componentsInChildren;
            foreach (Node node in array)
            {
                foreach (NodeSocket item in node.ListNodeSockets())
                {
                    if (item is NodeInput nodeInput)
                    {
                        if (nodeInput.connectedNode != null)
                        {
                            NodeOutput connectedOutput = nodeInput.GetConnectedOutput();
                            if (connectedOutput != null && !dictionary.ContainsKey(connectedOutput))
                            {
                                dictionary[connectedOutput] = dictionary.Count;
                            }
                        }
                    }
                    else if (item is NodeOutput key && !dictionary.ContainsKey(key))
                    {
                        dictionary[key] = dictionary.Count;
                    }
                }
            }
            Node[] array2 = componentsInChildren;
            foreach (Node node2 in array2)
            {
                if (!(node2 is NodeGraph nodeGraph))
                {
                    continue;
                }
                foreach (NodeGraphInput input in nodeGraph.inputs)
                {
                    NodeOutput connectedOutput2 = input.input.GetConnectedOutput();
                    if (connectedOutput2 != null && dictionary.ContainsKey(connectedOutput2))
                    {
                        dictionary[input.inputSocket] = dictionary[connectedOutput2];
                    }
                }
                foreach (NodeGraphOutput output in nodeGraph.outputs)
                {
                    NodeOutput connectedOutput3 = output.outputSocket.GetConnectedOutput();
                    if (connectedOutput3 != null && dictionary.ContainsKey(output.output))
                    {
                        dictionary[connectedOutput3] = dictionary[output.output];
                    }
                }
            }
            Type[] excludes = new Type[3]
            {
                typeof(NodeSocket),
                typeof(List<NodeGraphInput>),
                typeof(List<NodeGraphOutput>)
            };
            Node[] array3 = componentsInChildren;
            foreach (Node node3 in array3)
            {
                List<int> list = new List<int>();
                List<int> list2 = new List<int>();
                foreach (NodeSocket item2 in node3.ListNodeSockets())
                {
                    if (item2 is NodeInput nodeInput2)
                    {
                        NodeOutput connectedOutput4 = nodeInput2.GetConnectedOutput();
                        if (connectedOutput4 != null && dictionary.ContainsKey(connectedOutput4))
                        {
                            list.Add(dictionary[connectedOutput4]);
                        }
                    }
                    else if (item2 is NodeOutput key2)
                    {
                        list2.Add(dictionary[key2]);
                    }
                }
                FieldInfo[] source = (from field in node3.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                                      where excludes.All((Type type) => !type.IsAssignableFrom(field.FieldType))
                                      select field).ToArray();
                curText = curText + ((node3 is NodeGraph) ? "*" : "") + "[" + string.Join("][", list) + "]" + node3.name + "." + node3.GetType().Name + "(" + string.Join(", ", source.Select((FieldInfo field) => field.GetValue(node3))) + ")[" + string.Join("][", list2) + "]\n";
            }
            return curText;
        }

        public void ControlObject()
        {
            Ray ray = Utils.cam.Invoke(freeRoamCam).ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, 5f) && hitInfo.collider != null && Input.GetMouseButtonDown(0))
            {
                controlObject = hitInfo.collider.gameObject;
                controlDistance = Vector3.Distance(controlObject.transform.position, ray.origin);
                controlStart = controlObject.transform.position;
            }
            if (controlObject != null)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    controlObject.transform.position = controlStart;
                    controlObject = null;
                    controlDistance = 0f;
                    controlStart = Vector3.zero;
                }
                if (Input.GetMouseButton(2))
                {
                    float axisRaw = Input.GetAxisRaw("Mouse Look Vertical");
                    if (axisRaw > 0f)
                    {
                        controlDistance += controlDistance * 2f * Time.deltaTime;
                    }
                    else if (axisRaw < 0f)
                    {
                        controlDistance -= controlDistance * 2f * Time.deltaTime;
                    }
                }
                controlObject.transform.position = ray.origin + ray.direction * controlDistance;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (controlObject.TryGetComponent<Rigidbody>(out var component))
                {
                    component.velocity = Vector3.zero;
                }
                controlObject = null;
                controlDistance = 0f;
                controlStart = Vector3.zero;
            }
        }

        public void Minimap()
        {
            if (!(Game.currentLevel != null))
            {
                return;
            }
            if (mapCamera == null)
            {
                GameObject gameObject = new GameObject("Minimap");
                gameObject.transform.parent = ((Component)this).transform;
                mapCamera = gameObject.AddComponent<Camera>();
                mapCamera.tag = "Untagged";
            }
            if (mapTexture == null)
            {
                mapTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
                if (!mapTexture.IsCreated())
                {
                    mapTexture.Create();
                }
            }
            if (mapCamera != null)
            {
                mapCamera.transform.position = Human.Localplayer.transform.position + Vector3.up * mapHeight;
                mapCamera.transform.rotation = Quaternion.Euler(90f, Human.Localplayer.controls.cameraYawAngle, Human.Localplayer.transform.rotation.z);
                if (Input.GetAxis("Mouse ScrollWheel") != 0f)
                {
                    mapHeight += Input.GetAxis("Mouse ScrollWheel") * 3f;
                }
                mapCamera.targetTexture = mapTexture;
                GUI.Box(new Rect(Screen.width - 400, 0f, 200f, 200f), mapTexture);
            }
        }

        public void FollowServer()
        {
            if (!NetGame.isClient)
            {
                return;
            }
            if (mapCamera == null)
            {
                GameObject gameObject = new GameObject("Minimap");
                gameObject.transform.parent = ((Component)this).transform;
                mapCamera = gameObject.AddComponent<Camera>();
                mapCamera.tag = "Untagged";
            }
            if (mapTexture == null)
            {
                mapTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
                if (!mapTexture.IsCreated())
                {
                    mapTexture.Create();
                }
            }
            if (mapCamera != null)
            {
                Transform transform = NetGame.instance.server.players[0].human.ragdoll.partHead.transform;
                mapCamera.transform.position = transform.position - 4f * transform.forward;
                mapCamera.transform.rotation = transform.rotation;
                mapCamera.targetTexture = mapTexture;
                GUI.Box(new Rect(Screen.width - 400, 0f, 200f, 200f), mapTexture);
            }
        }



        public IEnumerator LoadCheckpointClient()
        {
            for (int i = 0; i < 23; i++)
            {
                NetGame.instance.SendRequestRespawn();
                yield return new WaitForSeconds(1f);
            }
            NetGame.instance.SendRequestRespawn();
            loadCheckpointState = 1;
            yield return null;
            loadCheckpointState = 2;
            yield return null;
            loadCheckpointState = 0;
        }

        public IEnumerator TestBhop()
        {
            jumpDir = 1;
            for (yawOverride = -90f; yawOverride < 0f; yawOverride += 0.5f)
            {
                yield return new WaitForFixedUpdate();
            }
            for (int i = 0; i < 5; i++)
            {
                jumpDir = 1;
                for (yawOverride = 0f; yawOverride < 45f; yawOverride += 0.5f)
                {
                    yield return new WaitForFixedUpdate();
                }
                jumpDir = -1;
                for (yawOverride = 0f; yawOverride > -45f; yawOverride -= 0.5f)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
            yawOverride = null;
            jumpDir = 0;
        }

        public static NetHost GetClientNoNotice(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                return null;
            }
            string[] array = txt.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != 1)
            {
                return null;
            }
            if (int.TryParse(array[0], out var result))
            {
                if (result == 1)
                {
                    return NetGame.instance.server;
                }
                if (result - 2 < NetGame.instance.readyclients.Count && result > 1)
                {
                    return NetGame.instance.readyclients[result - 2];
                }
            }
            return null;
        }


        public static void SetRenderMode(Renderer component)
        {
            Material[] materials = component.materials;
            foreach (Material material in materials)
            {
                material.SetFloat("_Mode", 3f);
                material.SetInt("_SrcBlend", 1);
                material.SetInt("_DstBlend", 10);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
            }
            component.shadowCastingMode = ShadowCastingMode.Off;
            component.receiveShadows = false;
        }

        public static void SetRenderModeMaterial(Material material)
        {
            material.SetFloat("_Mode", 3f);
            material.SetInt("_SrcBlend", 1);
            material.SetInt("_DstBlend", 10);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
        }

        public static void SendChatMessage_(string name, string msg)
        {
            NetChat.OnReceive(NetGame.instance.local.hostId, "<size=20><color=#FF00FF>" + name + "</color></size>", msg);
            NetStream netStream = NetGame.BeginMessage(NetMsgId.Chat);
            try
            {
                netStream.WriteNetId(NetGame.instance.local.hostId);
                netStream.Write("<size=20><color=#FF00FF>" + name + "</color></size>");
                netStream.Write(msg);
                if (!NetGame.isServer)
                {
                    return;
                }
                foreach (NetHost readyclient in NetGame.instance.readyclients)
                {
                    NetGame.instance.SendReliable(readyclient, netStream);
                }
            }
            finally
            {
                if (netStream != null)
                {
                    netStream = netStream.Release();
                }
            }
        }

        public static void SendChatMessage_(NetHost netHost, string name, string msg)
        {
            if (netHost == NetGame.instance.local)
            {
                NetChat.Print("<size=20><color=#FF00FF>" + name + "</color></size>");
                return;
            }
            NetStream netStream = NetGame.BeginMessage(NetMsgId.Chat);
            try
            {
                netStream.WriteNetId(netHost.hostId);
                netStream.Write("<size=20><color=#FF00FF>" + name + "</color></size>");
                netStream.Write(msg);
                if (NetGame.isServer)
                {
                    NetGame.instance.SendReliable(netHost, netStream);
                }
            }
            finally
            {
                if (netStream != null)
                {
                    netStream = netStream.Release();
                }
            }
        }

        public static void SendReliable(NetHost client, byte[] data, int len)
        {
            int num = NetStream.CalculateSizeForTier(0);
            if (len <= num)
            {
                SteamNetworking.SendP2PPacket(new CSteamID(ulong.Parse(client.players[0].skinUserId)), data, (uint)len, EP2PSend.k_EP2PSendReliable);
                return;
            }
            int num2 = 0;
            int num3 = num - 4;
            int num4 = len / num3;
            if (num4 * num3 < len)
            {
                num4++;
            }
            byte[] array = NetStream.AllocateBuffer(0);
            for (int i = 0; i < num4; i++)
            {
                int num5 = num4 - i - 1;
                array[0] = 0;
                array[1] = (byte)(num5 / 256 / 256);
                array[2] = (byte)(num5 / 256 % 256);
                array[3] = (byte)(num5 % 256);
                if (num2 + num3 > len)
                {
                    num3 = len - num2;
                }
                Array.Copy(data, num2, array, 4, num3);
                if (!SteamNetworking.SendP2PPacket(new CSteamID(ulong.Parse(client.players[0].skinUserId)), array, (uint)(num3 + 4), EP2PSend.k_EP2PSendReliable))
                {
                    UnityEngine.Debug.LogErrorFormat("Failed to send packet {0} of {1}", i, num4);
                }
                num2 += num3;
            }
            NetStream.ReleaseBuffer(0, array);
        }


        //public static bool CanFly(Human human)
        //{
        //	return flyCheat || humans[human].fly || InfectMod.UsingSkill(human, SkillType.Fly);
        //}

        public void OnDestroy()
        {
            Harmony.UnpatchID("com.plcc.hff.humanmod");
            ((MonoBehaviour)this).StopAllCoroutines();
            deleteFakeObjects = false;
            RenderFakeObjects();
            ignoreCollision = false;
            IgnoreCollisionUpdate();
            UnityEngine.Object.Destroy(cubePrimitive);
            UnityEngine.Object.Destroy(spherePrimitive);
            UnityEngine.Object.Destroy(cylinderPrimitive);
            UnityEngine.Object.Destroy(markPrefab);
            UnityEngine.Object.Destroy(prefab);
            CollectionExtensions.Do<GameObject>(checkpointVisuals.Concat(loadingZoneVisuals).Concat(deathZoneVisuals).Concat(airWallVisuals)
                .Concat(instances)
                .Concat(marks), (Action<GameObject>)UnityEngine.Object.Destroy);
            UnityEngine.Object.Destroy(mapCamera?.gameObject);
        }




        public static bool SteamRichPresenceUpdate(SteamRichPresence __instance)
        {
            if (bypassRichPresence)
            {
                if (Game.instance == null || WorkshopRepository.instance == null)
                {
                    Utils.SetGameMode.Invoke(__instance, new object[1] { "#Local_level" });
                    Utils.SetLevelName.Invoke(__instance, new object[1] { levelName });
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(NetPlayer), "PreFixedUpdate")]
        [HarmonyPrefix]
        public static bool PreFixedUpdateReplace(NetPlayer __instance, ref object ___moveLock, ref float ___walkForward, ref float ___walkRight, ref float ___cameraPitch, ref float ___cameraYaw, ref float ___leftExtend, ref float ___rightExtend, ref bool ___jump, ref bool ___playDead, ref bool ___holding, ref bool ___shooting, ref int ___moveFrames)
        {
            //自由视角 /////修改
            if (__instance.isLocalPlayer && ((UI_Main.ShowShuBiao && UI_SheZhi.noKong_xianshishubiao) || FreeRoamCam.allowFreeRoam))
            {
                return false;
            }
            if (!autoReach && !modifyHand && !modifySpeed && loadCheckpointState == 0 && !autoClimb && !aiMode && !enableHotkeys)
            {
                return true;
            }
            object obj = ___moveLock;
            lock (obj)
            {
                if (__instance.isLocalPlayer && !__instance.human.disableInput)
                {
                    bool flag = true;
                    if (MenuSystem.instance.state == MenuSystemState.PauseMenu)
                    {
                        flag = false;
                    }
                    else
                    {
                        if ((App.state == AppSate.ServerLobby || App.state == AppSate.ClientLobby) && MenuSystem.instance.state != MenuSystemState.Inactive)
                        {
                            flag = false;
                        }
                        if (NetChat.typing && (NetGame.isClient || NetGame.isServer))
                        {
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        __instance.controls.ReadInput(out var walkForward, out var walkRight, out var cameraPitch, out var cameraYaw, out var leftExtend, out var rightExtend, out var jump, out var playDead, out var shooting);
                        if (modifyHand && float.TryParse(curHand, out var result) && float.TryParse(curExtendedHand, out var result2))
                        {
                            leftExtend = ((leftExtend > 0f) ? result2 : result);
                            rightExtend = ((rightExtend > 0f) ? result2 : result);
                        }
                        if (modifySpeed && float.TryParse(curSpeed, out var result3))
                        {
                            walkForward *= result3;
                            walkRight *= result3;
                        }
                        if (loadCheckpointState > 0)
                        {
                            walkForward = -1f;
                            walkRight = 0f;
                            if (loadCheckpointState == 2)
                            {
                                shooting = true;
                            }
                        }
                        if (autoClimb)
                        {
                            walkForward = 1f;
                            climbState++;
                            if (climbState <= 45)
                            {
                                cameraPitch = 80f;
                                leftExtend = (rightExtend = 1f);
                            }
                            else if (climbState == 46)
                            {
                                cameraPitch = -80f;
                                leftExtend = (rightExtend = 1f);
                            }
                            else if (climbState <= 49)
                            {
                                cameraPitch = -80f;
                                leftExtend = (rightExtend = 0f);
                            }
                            else
                            {
                                leftExtend = (rightExtend = 1f);
                                cameraPitch = ((climbState < 57) ? (-80f) : ((climbState >= 61) ? (-70f + 3.5f * (float)(climbState - 45 - 16)) : (-80f + 2.5f * (float)(climbState - 45 - 12))));
                                if (climbState == 80)
                                {
                                    climbState = 0;
                                }
                            }
                        }
                        else if (aiMode)
                        {
                            AI component = __instance.human.GetComponent<AI>();
                            if (component != null)
                            {
                                walkForward = component.walkForward;
                                walkRight = component.walkRight;
                                cameraPitch = component.cameraPitch;
                                jump = component.jump;
                                leftExtend = (component.reach ? 1 : 0);
                                rightExtend = (component.reach ? 1 : 0);
                            }
                        }
                        if (pointState < 2)
                        {
                            walkForward = -1f;
                            shooting = pointState == 1;
                            pointState++;
                        }
                        ___walkForward = walkForward;
                        ___walkRight = walkRight;
                        ___cameraPitch = cameraPitch;
                        ___cameraYaw = cameraYaw;
                        ___leftExtend = leftExtend;
                        ___rightExtend = rightExtend;
                        ___jump = jump;
                        ___playDead = playDead;
                        ___shooting = shooting;
                    }
                    else
                    {
                        ___walkForward = 0f;
                        ___walkRight = 0f;
                        ___jump = false;
                        ___shooting = false;
                    }
                    if (autoReach && reach)
                    {
                        ___leftExtend = 1f;
                        ___rightExtend = 1f;
                    }
                }
                if (NetGame.isClient)
                {
                    __instance.SendMove(___walkForward, ___walkRight, ___cameraPitch, ___cameraYaw, ___leftExtend, ___rightExtend, ___jump, ___playDead, ___shooting);
                }
                else
                {
                    ___holding = __instance.human.hasGrabbed;
                }
                __instance.controls.HandleInput(___walkForward, ___walkRight, ___cameraPitch, ___cameraYaw, ___leftExtend, ___rightExtend, ___jump, ___playDead, ___holding, ___shooting);
                ___moveFrames = 0;
            }
            return false;
        }



        public static void OnReceiveSkin2(NetPlayer player)
        {
            if (fixSkin)
            {
                ((MonoBehaviour)(object)instance).StartCoroutine(instance.OnReceiveSkinCoroutine(player));
            }
        }

        public static string GetFriendPersonaName(string __result, CSteamID steamIDFriend)
        {
            if (steamIDFriend == SteamUser.GetSteamID() && !string.IsNullOrEmpty(personaName))
            {
                return personaName;
            }
            return __result;
        }

        [HarmonyPatch(typeof(LegMuscles), "JumpAnimation")]
        [HarmonyPrefix]
        public static bool JumpAnimation(LegMuscles __instance, Vector3 torsoFeedback, ref Human ___human, ref float ___upImpulse, ref float ___forwardImpulse, ref int ___framesToApplyJumpImpulse)
        {
            HumanAttribute value;
            bool flag = humans.TryGetValue(___human, out value);
            if (superJump == 1f && (!flag || value.scale == 1f))
            {
                return true;
            }
            ___human.ragdoll.partHips.rigidbody.SafeAddForce(torsoFeedback);
            if (___human.jump)
            {
                float num = 0.75f;
                int num2 = 2;
                float num3 = Mathf.Sqrt(2f * num / Physics.gravity.magnitude);
                float f = Mathf.Clamp(Utils.groundManager.Invoke(___human).groudSpeed.y, 0f, 100f);
                f = Mathf.Pow(f, 1.2f);
                float num4 = (num3 + f / Physics.gravity.magnitude) * ___human.weight;
                float num5 = ___human.controls.unsmoothedWalkSpeed * ((float)num2 + f / 2f) * ___human.mass;
                Vector3 momentum = ___human.momentum;
                float num6 = Vector3.Dot(___human.controls.walkDirection.normalized, momentum);
                if (num6 < 0f)
                {
                    num6 = 0f;
                }
                float num7 = (num4 - momentum.y) * superJump;
                if (num7 < 0f)
                {
                    num7 = 0f;
                }
                float num8 = (num5 - num6) * superJump;
                if (num8 < 0f)
                {
                    num8 = 0f;
                }
                ___framesToApplyJumpImpulse = 1;
                if (___human.onGround || Time.time - ___human.GetComponent<Ball>().timeSinceLastNonzeroImpulse < 0.2f)
                {
                    num7 /= (float)___framesToApplyJumpImpulse;
                    num8 /= (float)___framesToApplyJumpImpulse;
                    ___upImpulse = num7;
                    ___forwardImpulse = num8;
                    Utils.ApplyJumpImpulses.Invoke(__instance, null);
                    ___framesToApplyJumpImpulse--;
                }
                ___human.skipLimiting = true;
                ___human.jump = false;
            }
            else
            {
                if (___framesToApplyJumpImpulse-- > 0)
                {
                    Utils.ApplyJumpImpulses.Invoke(__instance, null);
                }
                int num9 = 3;
                int num10 = 500;
                float num11 = ___human.controls.unsmoothedWalkSpeed * (float)num9 * ___human.mass;
                float num12 = Vector3.Dot(___human.controls.walkDirection.normalized, ___human.momentum);
                float num13 = Mathf.Clamp((num11 - num12) / Time.fixedDeltaTime, 0f, num10);
                if (flag)
                {
                    num13 *= Mathf.Pow(value.scale, 3f);
                }
                ___human.ragdoll.partChest.rigidbody.SafeAddForce(num13 * ___human.controls.walkDirection.normalized);
            }
            return false;
        }

        [HarmonyPatch(typeof(LegMuscles), "AddWalkForce")]
        [HarmonyPrefix]
        public static bool AddWalkForce(LegMuscles __instance, ref Human ___human, ref Ragdoll ___ragdoll)
        {
            if (!humans.TryGetValue(___human, out var value) || value.scale == 1f)
            {
                return true;
            }
            Vector3 vector = ___human.controls.walkDirection * 300f * Mathf.Pow(value.scale, 3f);
            ___ragdoll.partBall.rigidbody.SafeAddForce(vector);
            if (___human.onGround)
            {
                Utils.groundManager.Invoke(___human).DistributeForce(-vector, ___ragdoll.partBall.rigidbody.position);
            }
            else if (___human.hasGrabbed)
            {
                Utils.grabManager.Invoke(___human).DistributeForce(-vector * 0.5f);
            }
            return false;
        }

        [HarmonyPatch(typeof(FreeRoamCam), "Update")]
        [HarmonyPostfix]
        public static void FreeRoamCamUpdate(FreeRoamCam __instance)
        {
            if (FreeRoamCam.allowFreeRoam && enableHotkeys)
            {
                Utils.keyframes.Invoke(__instance)[0].pos = Vector3.zero;
                int num = ((Game.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? 1 : 10);
                int num2 = ((Game.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) ? 100 : num);
                int num3 = num2 - num;
                if (Game.GetKey(KeyCode.W))
                {
                    __instance.transform.position += __instance.transform.forward * Time.unscaledDeltaTime * num3;
                }
                if (Game.GetKey(KeyCode.S))
                {
                    __instance.transform.position -= __instance.transform.forward * Time.unscaledDeltaTime * num3;
                }
                if (Game.GetKey(KeyCode.A))
                {
                    __instance.transform.position -= __instance.transform.right * Time.unscaledDeltaTime * num3;
                }
                if (Game.GetKey(KeyCode.D))
                {
                    __instance.transform.position += __instance.transform.right * Time.unscaledDeltaTime * num3;
                }
                if (Game.GetKey(KeyCode.Q))
                {
                    __instance.transform.position += __instance.transform.up * Time.unscaledDeltaTime * num3;
                }
                if (Game.GetKey(KeyCode.Z))
                {
                    __instance.transform.position -= __instance.transform.up * Time.unscaledDeltaTime * num3;
                }
            }
        }

        [HarmonyPatch(typeof(HumanControls), "HandleInput")]
        [HarmonyPrefix]
        public static void HandleInput(HumanControls __instance, ref float cameraYaw, ref float walkForward, ref float walkRight, ref bool jump, ref Human ___humanScript)
        {
            Human human = ___humanScript;
            if (yawOverride.HasValue)
            {
                cameraYaw = yawOverride.Value;
                walkForward = 1f;
                walkRight = jumpDir;
                jump = true;
            }
            if (autoBhop && __instance.walkLocalDirection.x != 0f && human.velocity.ZeroY().magnitude > 3f)
            {
                float num = __instance.unsmoothedWalkSpeed * 3f * human.mass - 500f * Time.fixedDeltaTime;
                float num2 = num / human.momentum.magnitude;
                if (0f < num2 && num2 < Mathf.Sqrt(0.75f))
                {
                    cameraYaw = Quaternion.LookRotation(human.momentum.normalized).y + Mathf.Acos(num2) * 57.29578f * Mathf.Sign(__instance.walkLocalDirection.x);
                }
            }
        }

        [HarmonyPatch(typeof(HumanControls), "HandleInput")]
        [HarmonyPostfix]
        public static void HandleInput(HumanControls __instance, ref Human ___humanScript)
        {
            Human human = ___humanScript;
            if (human.IsLocalPlayer && liuhai)
            {
                Quaternion quaternion = Quaternion.Euler(0f, __instance.cameraYawAngle, 0f);
                Vector3 walkDirection = quaternion * __instance.walkLocalDirection;
                __instance.walkDirection = walkDirection;
            }
        }

    	[HarmonyPatch(typeof(LevelInformationBox), "UpdateDisplay", new Type[] { typeof(NetTransport.LobbyDisplayInfo) })]
    	[HarmonyTranspiler]
    	public static IEnumerable<CodeInstruction> UpdateDisplay(IEnumerable<CodeInstruction> instructions)
    	{
    		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
    		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
    		//IL_003e: Expected O, but got Unknown
    		return new CodeMatcher(instructions, (ILGenerator)null).MatchForward(false, (CodeMatch[])(object)new CodeMatch[1]
    		{
    			new CodeMatch((OpCode?)OpCodes.Call, (object)Utils.Method<LevelInformationBox>("GetNewLevel", new Type[1] { typeof(ulong) }), (string)null)
    		}).Advance(-2).SetAndAdvance(OpCodes.Ldarg_1, (object)null)
    			.RemoveInstruction()
    			.Set(OpCodes.Call, (object)Utils.Method<FeatureManager>("GetNewLevel"))
    			.InstructionEnumeration();
    	}
        public static IEnumerator GetNewLevel(LevelInformationBox instance, NetTransport.LobbyDisplayInfo dispInfo)
        {
            bool loaded = false;
            WorkshopLevelMetadata levelData;
            Action<WorkshopLevelMetadata> onRead = delegate (WorkshopLevelMetadata l)
            {
                levelData = l;
                loaded = true;
                NetTransport.LobbyDisplayInfo lobbyDisplayInfo = Utils.prevDispInfo.Invoke(instance);
                if (levelData != null && (lobbyDisplayInfo.FeaturesMask & 0x20000000) != 0 && lobbyDisplayInfo.LevelID == dispInfo.LevelID)
                {
                    instance.LevelText.text = levelData.title;
                    instance.LevelImage.texture = levelData.thumbnailTexture;
                    instance.LevelImage.enabled = instance.LevelImage.texture != null;
                }
            };
            //if (previewOnly && App.state == AppSate.Menu)
            if (UI_SheZhi.guanbidatingxiazai && App.state == AppSate.Menu)
            {
                //只获取图名和图片
                LevelRepository2.instance.GetLevelNameAndThumbnail(dispInfo.LevelID, dispInfo.LevelType, onRead);
                //WorkshopRepository.instance.levelRepo.GetLevel(dispInfo.LevelID, dispInfo.LevelType, onRead);
            }
            else
            {
                WorkshopRepository.instance.levelRepo.LoadLevel(dispInfo.LevelID, onRead);
            }
            while (!loaded)
            {
                yield return null;
            }
        }

        [HarmonyPatch(typeof(NetPlayer), "SpawnPlayer")]
        [HarmonyPrefix]
        public static bool SpawnPlayer(uint id, NetHost host, bool isLocal, string skinUserId, uint localCoopIndex, byte[] skinCRC, ref NetPlayer __result)
        {
            // 黑名单拦截（主机侧、非本地玩家）
            try
            {
                if (NetGame.isServer && !isLocal && HeiMingDan.Enabled)
                {
                    string playerName = host != null ? host.name : "玩家";
                    if (HeiMingDan.ShouldBlockPlayerJoin(skinUserId, playerName))
                    {
                        if (host != null) NetGame.instance.Kick(host);
                        __result = null;
                        return false;
                    }
                }
            }
            catch { }

            if (!modifyScale)
            {
                return true;
            }
            if (!float.TryParse(curScale, out var result))
            {
                return true;
            }
            NetPlayer component = UnityEngine.Object.Instantiate(Game.instance.playerPrefab).GetComponent<NetPlayer>();
            component.human.player = component;
            component.human.transform.localScale = Vector3.one * result;
            component.human.ragdoll = UnityEngine.Object.Instantiate(Game.instance.ragdollPrefab.gameObject, component.human.transform, worldPositionStays: false).GetComponent<Ragdoll>();
            component.human.ragdoll.BindBall(component.human.transform);
            Rigidbody[] componentsInChildren = component.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].mass *= Mathf.Pow(result, 3f);
            }
            component.human.Initialize();
            component.human.weight *= result;
            RagdollPresetMetadata ragdollPresetMetadata;
            if (isLocal)
            {
                ragdollPresetMetadata = NetPlayer.GetLocalSkin(localCoopIndex);
            }
            else
            {
                ragdollPresetMetadata = RagdollPresetMetadata.LoadNetSkin(localCoopIndex, skinUserId);
                if (ragdollPresetMetadata != null && !ragdollPresetMetadata.CheckCRC(skinCRC))
                {
                    ragdollPresetMetadata = null;
                }
                if (Options.parental == 1)
                {
                    ragdollPresetMetadata?.ClearCustomisation();
                }
            }
            component.netId = id;
            component.host = host;
            component.localCoopIndex = localCoopIndex;
            component.isLocalPlayer = isLocal;
            component.skin = ragdollPresetMetadata;
            component.skinUserId = skinUserId;
            component.skinCRC = skinCRC;
            if (isLocal && ragdollPresetMetadata != null && (App.state == AppSate.PlayLevel || App.state == AppSate.ClientPlayLevel || App.state == AppSate.ServerPlayLevel))
            {
                App.StartPlaytimeForItem(ragdollPresetMetadata.workshopId);
            }
            host.AddPlayer(component);
            if (ragdollPresetMetadata != null)
            {
                component.ApplyPreset(ragdollPresetMetadata, bake: true, !isLocal && Options.parental == 1);
            }
            else
            {
                component.ApplyPreset(PresetRepository.CreateDefaultSkin(), bake: false);
            }
            Utils.SetupBodies.Invoke(component, null);
            if (isLocal)
            {
                MenuCameraEffects.instance.AddHuman(component);
                Listener.instance.AddHuman(component.human);
                PlayerManager.instance.OnLocalPlayerAdded(component);
            }
            else
            {
                component.cameraController.gameObject.SetActive(value: false);
            }
            if (NetGame.netlog)
            {
                UnityEngine.Debug.LogFormat("Spawning {0}", component.netId);
            }
            __result = component;
            if (!humans.ContainsKey(component.human))
            {
                humans[component.human] = new HumanAttribute();
            }
            humans[component.human].scale = result;
            Physics.gravity = Vector3.down * 9.81f * result;
            return false;
        }

        //[HarmonyPatch(typeof(Resources), "UnloadUnusedAssets")]
        //[HarmonyPrefix]
        //public static void Patch_UnloadUnusedAssets()
        //{
        //    UnityEngine.Debug.Log("UnloadUnusedAssets 补丁触发");
        //    if (NetGame.instance?.local != null)
        //    {
        //        Chat.TiShi(NetGame.instance.local, "[YxMod] 正在卸载未使用的资源，请稍候...");
        //    }
        //}

        [HarmonyPatch(typeof(RagdollTexture), "BakeTexture")]
        [HarmonyPrefix]
        public static bool Prefix_BakeTexture(RagdollTexture __instance, ref RenderTexture rt, bool compress)
        {
            //UnityEngine.Debug.Log("BakeTexture 补丁触发");
            if (!UI_SheZhi.skinCheckEnabled || rt == null)
                return true;

            if (rt.width <= 1024 && rt.height <= 1024)
                return true;

            int newW = Mathf.Min(rt.width, 1024);
            int newH = Mathf.Min(rt.height, 1024);

            RenderTexture downscaled = RenderTexture.GetTemporary(newW, newH);
            Graphics.Blit(rt, downscaled);

            UnityEngine.Debug.Log($"[BakeTexture缩放] 原尺寸: {rt.width}x{rt.height} => 缩放为: {newW}x{newH}");
            //Chat.TiShi(NetGame.instance.local, $"[BakeTexture缩放] 将贴图缩放为 {newW}x{newH}");

            rt = downscaled;
            __instance.width = newW;
            __instance.height = newH;

            return true;
        }

        //[HarmonyPatch(typeof(RagdollTexture), "BakeTexture")]
        //[HarmonyPostfix]
        //public static void Postfix_BakeTexture(RenderTexture rt)
        //{
        //    //UnityEngine.Debug.Log("BakeTexture 后补丁触发");
        //    if (!UI_SheZhi.skinCheckEnabled || rt == null)
        //        return;

        //    // 这里判断尺寸，如果是缩放后的RT就释放
        //    if (rt.width <= 1024 && rt.height <= 1024)
        //    {
        //        RenderTexture.ReleaseTemporary(rt);
        //        //UnityEngine.Debug.Log("BakeTexture 缩放后RenderTexture已释放");
        //    }
        //}
        [HarmonyPatch(typeof(Texture2D), nameof(Texture2D.Compress))]
        [HarmonyPrefix]
        static bool Prefix_Compress(Texture2D __instance, bool highQuality)
        {
            if (!UI_SheZhi.SkipTextureCompression)
                return true;

            UnityEngine.Debug.Log($"[Patch] Skip Texture2D.Compress on {__instance.name}");
            return false;
        }
        [HarmonyPatch(typeof(Texture2D), MethodType.Constructor, new Type[] { typeof(int), typeof(int) })]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Texture2D_ctor_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //UnityEngine.Debug.Log("Texture2D.ctor 补丁触发");
            //UnityEngine.Debug.Log($"skinUseRGB24Format = {UI_SheZhi.skinUseRGB24Format}");
            if (PlayerPrefs.GetInt("skinUseRGB24Format", 1) <= 0) { return instructions; }

            var codes = new List<CodeInstruction>(instructions);
            bool replacedFormat = false;
            bool replacedMipmap = false;

            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];

                // RGBA32 = 4
                if (
                    (code.opcode == OpCodes.Ldc_I4 && (int)code.operand == 4) ||
                    code.opcode == OpCodes.Ldc_I4_4 ||
                    (code.opcode == OpCodes.Ldc_I4_S && (sbyte)code.operand == 4)
                )
                {
                    // 替换为 RGB24 = 3
                    codes[i] = new CodeInstruction(OpCodes.Ldc_I4_3);
                    UnityEngine.Debug.Log("[YxMod] Texture2D 构造函数中已将 RGBA32 替换为 RGB24");
                    replacedFormat = true;
                }
                // 替换 mipmap true (1) → false (0)
                // 通常紧跟在格式替换之后，确保对应正确的参数位置
                if (code.opcode == OpCodes.Ldc_I4_1)
                {
                    codes[i] = new CodeInstruction(OpCodes.Ldc_I4_0);
                    UnityEngine.Debug.Log("[YxMod] Texture2D 构造函数中已将 mipmap 设为 false");
                    replacedMipmap = true;
                }
            }

            if (!replacedFormat)
            {
                UnityEngine.Debug.LogWarning("[YxMod] ⚠ 未找到 RGBA32 指令，补丁可能失效");
            }
            if (!replacedMipmap)
            {
                UnityEngine.Debug.LogWarning("[YxMod] ⚠ 未找到 mipmap = true 指令，补丁可能失效");
            }

            return codes;
        }
        [HarmonyPatch(typeof(FileTools), nameof(FileTools.NativeTextureDecodeHeader))]
        [HarmonyPostfix]
        public static void Postfix_NativeTextureDecodeHeader(ref NativeTextureHeader header)
        {
            // 条件开关（可选）
            if (!UI_SheZhi.skinUseRGB24Format) return;

            if (header.format == TextureFormat.RGBA32)
            {
                header.format = TextureFormat.RGB24;
                UnityEngine.Debug.Log("[YxMod] 已将 NativeTextureHeader.format 从 RGBA32 替换为 RGB24");
                Chat.TiShi(NetGame.instance.local, "[YxMod] 已将 NativeTextureHeader.format 从 RGBA32 替换为 RGB24");
            }
        }
        [HarmonyPatch(typeof(FileTools), "TextureFromBytes")]
        [HarmonyPostfix]
        public static void Patch_TextureFromBytes(ref Texture2D __result, string name, byte[] bytes)
        {
            //UnityEngine.Debug.Log("TextureFromBytes 补丁触发");
            if (!UI_SheZhi.skinCheckEnabled || __result == null)
                return;

            int max = Mathf.Max(__result.width, __result.height);
            if (max <= 1024)
                return;

            float scale = 1024f / max;
            int newW = Mathf.RoundToInt(__result.width * scale);
            int newH = Mathf.RoundToInt(__result.height * scale);

            RenderTexture rt = RenderTexture.GetTemporary(newW, newH);
            RenderTexture.active = rt;
            Graphics.Blit(__result, rt);

            //Texture2D resized = new Texture2D(newW, newH, TextureFormat.RGBA32, false);
            Texture2D resized = new Texture2D(newW, newH);
            resized.ReadPixels(new Rect(0, 0, newW, newH), 0, 0);
            resized.Apply();

            RenderTexture.active = null;
            //RenderTexture.ReleaseTemporary(rt);
            UnityEngine.Object.Destroy(__result); // 销毁旧图

            resized.name = name;
            __result = resized;

            UnityEngine.Debug.Log($"[TextureFromBytes缩放] {name} => {newW}x{newH}");
            //Chat.TiShi(NetGame.instance.local, $"[TextureFromBytes缩放] {name} 缩放为 {newW}x{newH}");
        }


        private static readonly ConcurrentDictionary<string, DateTime> lastSkinReceiveTime = new();

        [HarmonyPatch(typeof(NetGame), "OnClientReceive")]
        [HarmonyPrefix]
        public static bool Prefix_OnClientReceive(object connection, NetStream stream)
        {
            //UnityEngine.Debug.Log("OnClientReceive 补丁触发");
            // 克隆 NetStream
            NetStream clone = NetStream.AllocStream(stream);
            NetMsgId netMsgId = clone.ReadMsgId();
            if (netMsgId == NetMsgId.RemoveHost)
            {
                uint hostId = clone.ReadNetId();
                NetHost netHost = NetGame.instance.FindAnyHost(hostId);

                if (netHost != null)
                {
                    string playerId = connection.ToString();
                    ulong ulSteamID = ulong.Parse(playerId);
                    CSteamID steamIDFriend = new CSteamID(ulSteamID);
                    string playername = SteamFriends.GetFriendPersonaName(steamIDFriend);
                    UnityEngine.Debug.Log($"[YxMod] connection:{playername} - 房主：{Human.all[0].player.host.name} - 被移除的是：{netHost.name}");

                    if (playerId != NetGame.instance.server.players[NetGame.instance.server.players.Count-1].skinUserId)
                    {
                        Chat.TiShi(NetGame.instance.local, $"玩家 {playername} 在强踢你！");
                        UnityEngine.Debug.Log($"玩家 {playername} 在强踢你！");
                        return false;
                    }

                }
            }
            clone.Release();
            return true;
        }
        [HarmonyPatch(typeof(NetGame), "OnServerReceive")]
        [HarmonyPrefix]
        public static bool Prefix_OnServerReceive(NetHost client, NetStream stream)
        {
            try
            {
                if (!NetGame.isServer || !HeiMingDan.Enabled || client == null || stream == null)
                {
                    return true;
                }
                // 只读取消息ID用于判定，不破坏原始流
                NetStream clone = NetStream.AllocStream(stream);
                NetMsgId netMsgId = clone.ReadMsgId();
                clone.Release();

                if (netMsgId == NetMsgId.AddPlayer)
                {
                    string steamID = client.connection != null ? client.connection.ToString() : null;
                    string playerName = client.name ?? "玩家";
                    if (!string.IsNullOrEmpty(steamID) && HeiMingDan.ShouldBlockPlayerJoin(steamID, playerName))
                    {
                        NetGame.instance.Kick(client);
                        return false;
                    }
                }
            }
            catch { }
            return true;
        }
        //[HarmonyPatch(typeof(App), "EnterLobbyAsync")]
        //[HarmonyPostfix]
        //static IEnumerator EnterLobbyAsyncPostfix(IEnumerator __result, App __instance)
        //{
        //    while (__result.MoveNext())
        //    {
        //        yield return __result.Current;
        //    }

        //    // 用反射访问私有字段 lobbyAssetbundle
        //    FieldInfo field = typeof(App).GetField("lobbyAssetbundle", BindingFlags.NonPublic | BindingFlags.Instance);
        //    AssetBundle bundle = (AssetBundle)field?.GetValue(__instance);

        //    if (bundle != null)
        //    {
        //        UnityEngine.Debug.Log("[YxMod] 正在使用Unload(true)卸载资源包...");
        //        Chat.TiShi(NetGame.instance.local, "[YxMod] 正在使用Unload(true)卸载资源包...");
        //        bundle.Unload(true);

        //        // 同时清空字段，防止后续误用
        //        field.SetValue(__instance, null);
        //    }

        //    // 卸载无引用资源
        //    yield return Resources.UnloadUnusedAssets();
        //    UnityEngine.Debug.Log("[YxMod] 内存清理完成。");
        //    Chat.TiShi(NetGame.instance.local, "[YxMod] 内存清理完成。");
        //}


        [HarmonyPatch(typeof(SafeForces), "SafeAddForce")]
        [HarmonyPrefix]
        public static void SafeAddForce(Rigidbody body, ref Vector3 force)
        {
            Human componentInParent = body.GetComponentInParent<Human>();
            if ((object)componentInParent != null && humans.TryGetValue(componentInParent, out var value) && value.scale != 1f)
            {
                force *= Mathf.Pow(humans[componentInParent].scale, 1f);
            }
        }

        [HarmonyPatch(typeof(CameraController3), "LateUpdate")]
        [HarmonyPrefix]
        public static bool LateUpdateReplace(CameraController3 __instance, ref Vector3 ___fixedupdateSmooth, ref float ___offsetSpeed)
        {
            if (!modifyScale)
            {
                return true;
            }
            if (!float.TryParse(curScale, out var result))
            {
                return true;
            }
            bool flag = !NetGame.isClient && !ReplayRecorder.isPlaying;
            object[] array = new object[7]
            {
                default(Vector3),
                0f,
                0f,
                0f,
                0f,
                0f,
                0f
            };
            switch (__instance.mode)
            {
                case CameraMode.Far:
                    Utils.CalculateFarCam.Invoke(__instance, array);
                    break;
                case CameraMode.Close:
                    Utils.CalculateCloseCam.Invoke(__instance, array);
                    break;
                case CameraMode.FirstPerson:
                    Utils.CalculateFirstPersonCam.Invoke(__instance, array);
                    break;
                default:
                    Utils.CalculateCloseCam.Invoke(__instance, array);
                    break;
            }
            Vector3 vector = (Vector3)array[0];
            float y = (float)array[1];
            float num = (float)array[2];
            float num2 = (float)array[3];
            float num3 = (float)array[4];
            float num4 = (float)array[5];
            float num5 = (float)array[6];
            if (CameraController3.fovAdjust != 0f)
            {
                num2 *= Mathf.Tan((float)Math.PI / 180f * num4 / 2f) / Mathf.Tan((float)Math.PI / 180f * (num4 + CameraController3.fovAdjust) / 2f);
                num4 += CameraController3.fovAdjust;
            }
            num2 /= MenuCameraEffects.instance.cameraZoom;
            num2 = Mathf.Max(num2, num3);
            if (MenuCameraEffects.instance.creditsAdjust > 0f)
            {
                num = Mathf.Lerp(num, 90f, MenuCameraEffects.instance.creditsAdjust * 0.7f);
                num2 += MenuCameraEffects.instance.creditsAdjust * 20f;
                num4 = Mathf.Lerp(num4, 40f, MenuCameraEffects.instance.creditsAdjust);
            }
            Quaternion quaternion = Quaternion.Euler(num, y, 0f);
            Vector3 vector2 = quaternion * Vector3.forward;
            Vector3 vector3 = ((!flag) ? ((Vector3)Utils.SmoothCamera.Invoke(__instance, new object[2]
            {
                __instance.human.ragdoll.partHead.transform.position,
                Time.unscaledDeltaTime
            })) : ___fixedupdateSmooth) + vector;
            num5 *= Mathf.Clamp(__instance.gameCam.transform.position.magnitude / 500f, 1f, 2f);
            __instance.gameCam.nearClipPlane = num5;
            __instance.gameCam.fieldOfView = num4;
            float num6 = ((!__instance.ignoreWalls) ? ((float)Utils.CompensateForWallsNearPlane.Invoke(__instance, new object[4]
            {
                vector3,
                quaternion,
                __instance.farRange * 1.2f,
                num3
            })) : 10000f);
            __instance.offset = (float)Utils.SpringArm.Invoke(__instance, new object[4]
            {
                __instance.offset,
                num2,
                num6,
                Time.unscaledDeltaTime
            });
            if (num6 < __instance.offset && !Physics.SphereCast(vector3 - vector2 * __instance.offset, num5 * 2f, vector2, out var _, __instance.offset - num6, __instance.wallLayers, QueryTriggerInteraction.Ignore))
            {
                __instance.offset = num6;
                ___offsetSpeed = 0f;
            }
            __instance.ApplyCamera(vector3, vector3 - vector2 * __instance.offset * result, quaternion, num4);
            return false;
        }

        [HarmonyPatch(typeof(LevelSelectMenu2), "Rebind")]
        [HarmonyPrefix]
        public static bool Rebind()
        {
            if (!noWorkshopReload || LevelSelectMenu2.displayMode != LevelSelectMenuMode.SubscribedWorkshop)
            {
                return true;
            }
            LevelSelectMenu2.instance.StartCoroutine(RebindCoroutine());
            return false;
        }

        public static IEnumerator RebindCoroutine()
        {
            bool flag = LevelSelectMenu2.displayMode == LevelSelectMenuMode.SubscribedWorkshop;
            Utils.UpdateTitle.Invoke(LevelSelectMenu2.instance, new object[1] { false });
            Utils.DisableLevelContinue.Invoke(LevelSelectMenu2.instance, Array.Empty<object>());
            bool flag2 = SteamUser.BLoggedOn();
            Utils.InLevelSelectMenu.Invoke(null, new object[1] { LevelSelectMenu2.displayMode != LevelSelectMenuMode.Campaign && LevelSelectMenu2.displayMode != LevelSelectMenuMode.LocalWorkshop });
            LevelSelectMenu2.instance.showCustomButton.SetActive(value: false);
            LevelSelectMenu2.instance.showSubscribedButton.SetActive(value: false);
            LevelSelectMenu2.instance.subscribedSubtitle.SetActive(value: false);
            LevelSelectMenu2.instance.customSubtitle.SetActive(value: false);
            LevelSelectMenu2.instance.ShowLocalLevelButton.SetActive(value: false);
            LevelSelectMenu2.instance.InvalidLevelInfoPanel.SetActive(value: false);
            LevelSelectMenu2.instance.noSubscribedPrompt.SetActive(value: false);
            LevelSelectMenu2.instance.offlinePanel.SetActive(value: false);
            LevelSelectMenu2.instance.noLocalPrompt.SetActive(value: false);
            List<WorkshopLevelMetadata> list = null;
            bool isMultiplayer = Utils.isMultiplayer.Invoke(LevelSelectMenu2.instance);
            bool isLobbyMode = LevelSelectMenu2.displayMode == LevelSelectMenuMode.BuiltInLobbies || LevelSelectMenu2.displayMode == LevelSelectMenuMode.WorkshopLobbies;
            switch (LevelSelectMenu2.displayMode)
            {
                case LevelSelectMenuMode.Campaign:
                    WorkshopRepository.instance.LoadBuiltinLevels(isMultiplayer && isLobbyMode);
                    list = WorkshopRepository.instance.levelRepo.BySource(isLobbyMode ? WorkshopItemSource.BuiltInLobbies : WorkshopItemSource.BuiltIn);
                    break;
                case LevelSelectMenuMode.EditorPicks:
                    WorkshopRepository.instance.LoadEditorPickLevels();
                    list = WorkshopRepository.instance.levelRepo.BySource(WorkshopItemSource.EditorPick);
                    break;
                case LevelSelectMenuMode.SubscribedWorkshop:
                    if (flag2)
                    {
                        ReloadSubscriptions(isLobby: false, onlyLevels: false);
                        list = WorkshopRepository.instance.levelRepo.BySource(WorkshopItemSource.Subscription);
                    }
                    break;
                case LevelSelectMenuMode.LocalWorkshop:
                    WorkshopRepository.instance.ReloadLocalLevels();
                    list = WorkshopRepository.instance.levelRepo.BySource(WorkshopItemSource.LocalWorkshop);
                    break;
                case LevelSelectMenuMode.BuiltInLobbies:
                    WorkshopRepository.instance.LoadBuiltinLevels(requestLobbies: true);
                    list = WorkshopRepository.instance.levelRepo.BySource(WorkshopItemSource.BuiltInLobbies);
                    break;
                case LevelSelectMenuMode.WorkshopLobbies:
                    if (flag2)
                    {
                        ReloadSubscriptions(isLobby: true, onlyLevels: false);
                        list = WorkshopRepository.instance.levelRepo.BySource(WorkshopItemSource.SubscriptionLobbies);
                    }
                    break;
            }
            if (list == null)
            {
                list = new List<WorkshopLevelMetadata>();
            }
            if (LevelSelectMenu2.displayMode != LevelSelectMenuMode.Campaign && (!isMultiplayer || isLobbyMode))
            {
                LevelSelectMenu2.instance.FindMoreButton.SetActive(value: false);
            }
            LevelSelectMenu2.instance.customFolder.text = PlayerPrefs.GetString("WorkshopRoot", Path.Combine(Application.dataPath, "Workshop"));
            if (list.Count == 0)
            {
                LevelSelectMenu2.instance.list.Bind(list);
                LevelSelectMenu2.instance.itemListImage.color = LevelSelectMenu2.instance.itemListError;
                if (!flag2)
                {
                    LevelSelectMenu2.instance.offlinePanel.SetActive(value: true);
                }
                else if (isLobbyMode)
                {
                    LevelSelectMenu2.instance.noSubscribedPrompt.SetActive(value: true);
                }
                else if (LevelSelectMenu2.displayMode == LevelSelectMenuMode.SubscribedWorkshop)
                {
                    LevelSelectMenu2.instance.noSubscribedPrompt.SetActive(value: true);
                }
                else
                {
                    LevelSelectMenu2.instance.noLocalPrompt.SetActive(value: true);
                }
                EventSystem.current.SetSelectedGameObject(LevelSelectMenu2.instance.BackButton.gameObject);
                LevelSelectMenu2.instance.levelImage.SetActive(value: false);
                LevelSelectMenu2.instance.LevelDescriptionPanel.SetActive(value: false);
            }
            else
            {
                LevelSelectMenu2.instance.itemListImage.color = LevelSelectMenu2.instance.itemListNormal;
                LevelSelectMenu2.instance.levelInfoPanel.SetActive(value: true);
                LevelSelectMenu2.instance.levelImage.SetActive(!flag);
                LevelSelectMenu2.instance.LevelDescriptionPanel.SetActive(flag);
                LevelSelectMenu2.instance.StartCoroutine(BindCoroutine(LevelSelectMenu2.instance.list, list));
                int i = 0;
                int checkpoint;
                while (i <= list.Count / 50)
                {
                    yield return null;
                    checkpoint = i++;
                }
                if (!string.IsNullOrEmpty(LevelSelectMenu2.selectedPath))
                {
                    int i2 = 0;
                    while (i2 < list.Count && !((list[i2].folder[0] == 'l') ? list[i2].folder.Equals(LevelSelectMenu2.selectedPath) : list[i2].folder.StartsWith(LevelSelectMenu2.selectedPath)))
                    {
                        checkpoint = i2++;
                    }
                }
                GameSave.GetLastSave(out var num, out checkpoint, out var _, out var _);
                WorkshopItemSource lastCheckpointLevelType = GameSave.GetLastCheckpointLevelType();
                if ((lastCheckpointLevelType == WorkshopItemSource.BuiltIn) ? (LevelSelectMenu2.displayMode == LevelSelectMenuMode.Campaign) : (lastCheckpointLevelType == WorkshopItemSource.EditorPick && LevelSelectMenu2.displayMode == LevelSelectMenuMode.EditorPicks))
                {
                    if (num < LevelSelectMenu2.instance.list.GetNumberItems - 1 && num != -1)
                    {
                        LevelSelectMenu2.instance.list.FocusItem(num);
                        if (list.Count > num && list[num] != null)
                        {
                            LevelSelectMenu2.instance.BindLevel(list[num]);
                        }
                    }
                    else
                    {
                        LevelSelectMenu2.instance.list.FocusItem(0);
                        if (list.Count > 0 && list[0] != null)
                        {
                            LevelSelectMenu2.instance.BindLevel(list[0]);
                        }
                    }
                }
                else
                {
                    LevelSelectMenu2.instance.list.FocusItem(0);
                    if (list.Count > 0 && list[0] != null)
                    {
                        LevelSelectMenu2.instance.BindLevel(list[0]);
                    }
                }
            }
            LevelSelectMenu2.instance.topPanel.Invalidate();
            Utils.EnableShowLevelButtons.Invoke(LevelSelectMenu2.instance, Array.Empty<object>());
            Utils.BindLevelIfNeeded.Invoke(LevelSelectMenu2.instance, new object[1] { Utils.selectedMenuItem.Invoke(LevelSelectMenu2.instance) });
            if (Utils.previousSelectedItem.Invoke(LevelSelectMenu2.instance) != null)
            {
                LevelSelectMenu2.instance.StartCoroutine((IEnumerator)Utils.WaitAndSelect.Invoke(LevelSelectMenu2.instance, Array.Empty<object>()));
            }
        }

        public static IEnumerator BindCoroutine(ListView listView, IList list)
        {
            listView.Clear();
            Utils.orginalItemsInList.Invoke(listView) = list.Count;
            if (list.Count == 0)
            {
                yield break;
            }
            int num = 0;
            int maxItemsOnScreen = Utils.maxItemsOnScreen.Invoke(listView);
            int num2;
            if (list.Count > maxItemsOnScreen)
            {
                num2 = maxItemsOnScreen + list.Count;
                num = (Utils.isHorizontal.Invoke(listView) ? Math.Max(list.Count - maxItemsOnScreen, 0) : 0);
                listView.isCarousel = true;
            }
            else
            {
                num2 = list.Count;
                listView.isCarousel = false;
            }
            int[] array = new int[num2];
            for (int i = 0; i < num2; i++)
            {
                array[i] = num;
                num++;
                if (num >= list.Count)
                {
                    num = 0;
                }
            }
            for (int j = 0; j < num2; j++)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(listView.itemTemplate.gameObject, listView.itemContainer.transform, worldPositionStays: false);
                Utils.buttons.Invoke(listView).Add(gameObject.GetComponent<UnityEngine.UI.Button>());
                ListViewItem component = gameObject.GetComponent<ListViewItem>();
                int num3 = array[j];
                component.Bind(j, list[num3]);
                gameObject.SetActive(value: true);
                if (j % 50 == 0)
                {
                    yield return null;
                }
            }
        }

        public static bool ReloadSubscriptions(bool isLobby, bool onlyLevels)
        {
            if (WorkshopRepository.instance.levelRepo.BySource(WorkshopItemSource.Subscription).Count > 0)
            {
                return false;
            }
            try
            {
                int numSubscribedItems = (int)SteamUGC.GetNumSubscribedItems();
                PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
                PublishedFileId_t[] array2 = new PublishedFileId_t[numSubscribedItems];
                SteamUGC.GetSubscribedItems(array, (uint)numSubscribedItems);
                int num = 0;
                PublishedFileId_t[] array3 = array;
                foreach (PublishedFileId_t publishedFileId_t in array3)
                {
                    uint itemState = SteamUGC.GetItemState(publishedFileId_t);
                    if ((itemState & 4) != 0)
                    {
                        array2[num] = publishedFileId_t;
                        num++;
                    }
                }
                Utils.CheckStoredLevelAndLobbySubscriptions.Invoke(WorkshopRepository.instance, new object[1] { array });
                for (int j = 0; j < num; j++)
                {
                    Utils.ReadFolder.Invoke(WorkshopRepository.instance, new object[2]
                    {
                        (!isLobby) ? WorkshopItemSource.Subscription : WorkshopItemSource.SubscriptionLobbies,
                        "ws:" + array2[j].ToString() + "/"
                    });
                }
            }
            catch (Exception exception)
            {
                UnityEngine.Debug.LogException(exception);
            }
            return false;
        }

        [HarmonyPatch(typeof(MultiplayerSelectLobbyMenu), "RebindList")]
        [HarmonyPrefix]
        public static bool RebindList(MultiplayerSelectLobbyMenu __instance)
        {
            if (!noDelay)
            {
                return true;
            }
            Dialogs.ShowListGamesProgress();
            __instance.StartCoroutine(DelayedOnlineScreen(__instance));
            return false;
        }

        public static IEnumerator DelayedOnlineScreen(MultiplayerSelectLobbyMenu __instance)
        {
            NetGame.instance.transport.ListLobbies(delegate (List<NetTransport.ILobbyEntry> i)
            {
                Utils.OnLobbiesListed.Invoke(__instance, new object[1] { i });
            });
            yield break;
        }

        [HarmonyPatch(typeof(App), "CancelConnect")]
        [HarmonyPrefix]
        public static bool CancelConnect()
        {
            if (!noDelay)
            {
                return true;
            }
            lock (App.stateLock)
            {
                if (App.state == AppSate.ClientJoin)
                {
                    NetGame.instance.LeaveGame();
                    Utils.EnterMenu.Invoke(App.instance, Array.Empty<object>());
                    MenuSystem.instance.HideMenus();
                    App.instance.StartCoroutine(DelayedCancelConnect());
                }
            }
            return false;
        }

        public static IEnumerator DelayedCancelConnect()
        {
            Utils.FinishCancelConnect.Invoke(App.instance, Array.Empty<object>());
            NetTransportSteam.RemoveOldPackets();
            yield break;
        }

        [HarmonyPatch(typeof(NetGame), "OnHelo")]
        [HarmonyPostfix]
        public static void OnHelo()
        {
            skinFixed = false;
        }

        [HarmonyPatch(typeof(App), "OnHostGameSuccess")]
        [HarmonyPostfix]
        public static void OnHostGameSuccess()
        {
            skinFixed = false;
        }

        [HarmonyPatch(typeof(App), "LevelLoadedClient")]
        [HarmonyPostfix]
        public static void LevelLoadedClient()
        {
            if (!fixSkin || skinFixed)
            {
                return;
            }
            foreach (NetPlayer player in NetGame.instance.players)
            {
                if (!player.isLocalPlayer)
                {
                    ((MonoBehaviour)(object)instance).StartCoroutine(instance.OnReceiveSkinCoroutine(player));
                }
            }
            skinFixed = true;
        }

        public IEnumerator OnReceiveSkinCoroutine(NetPlayer player)
        {
            yield return new WaitForSeconds(2f);
            if (player.skin == null)
            {
                player.skin = RagdollPresetMetadata.LoadNetSkin(player.localCoopIndex, player.skinUserId);
            }
            if (player.skin != null)
            {
                player.ApplyPreset(player.skin);
            }
            Shell.Print($"Applying {player.host.name} ({player.skin != null})");
        }

        [HarmonyPatch(typeof(Dialogs), "ShowLoadLevelProgress")]
        [HarmonyPrefix]
        public static bool ShowLoadLevelProgress()
        {
            if (!loadingPreview)
            {
                return true;
            }
            MenuCameraEffects.FadeFromBlack(0.02f);
            DialogOverlay.Show(0f, showProgress: true, Dialogs.T("TUTORIAL/LOADING"), null, null, null);
            return false;
        }

        [HarmonyPatch(typeof(NetGame), "OnHelo")]
        [HarmonyPrefix]
        public static bool OnHelo2(object serverConnection, NetStream stream, NetGame __instance)
        {
            if (!strongConnect)
            {
                return true;
            }
            uint num = stream.ReadNetId();
            string text = stream.ReadString();
            bool flag = stream.ReadBool();
            object obj = Utils.stateLock.Invoke();
            lock (obj)
            {
                NetGame.isClient = true;
                NetGame.isNetStarted = true;
                NetGame.isNetStarting = false;
            }
            __instance.local = new NetHost(null, __instance.transport.GetMyName())
            {
                isLocal = true,
                isReady = false
            };
            __instance.allclients.Add(__instance.local);
            __instance.server = new NetHost(serverConnection, NetMsgId.Helo.ToString())
            {
                isLocal = false,
                hostId = 0u,
                isReady = false
            };
            __instance.local.hostId = stream.ReadNetId();
            __instance.server.name = stream.ReadString();
            NetGame.lobbyLevel = stream.ReadString();
            string text2 = stream.ReadString();
            Utils.SetMultiplayerLobbyID.Invoke(__instance, new object[1] { text2 });
            __instance.transport.LobbyConnectedFixup();
            NetStream netStream = NetGame.BeginMessage(NetMsgId.Helo);
            try
            {
                netStream.WriteNetId(VersionDisplay.netCode);
                netStream.Write(VersionDisplay.fullVersion);
                netStream.Write(__instance.local.name);
                __instance.SendReliableToServer(netStream);
            }
            finally
            {
                if (netStream != null)
                {
                    netStream = netStream.Release();
                }
            }
            HumanAnalytics.instance.BeginMultiplayer(host: false);
            __instance.StopLocalGame();
            __instance.AddLocalPlayer();
            if (__instance.transport.IsRelayed(__instance.server))
            {
                App.instance.OnRelayConnection(__instance.server);
            }
            return false;
        }

        //[HarmonyPatch(typeof(NetSignal), "Process")]
        //[HarmonyTranspiler]
        //public static IEnumerable<CodeInstruction> ProcessPatch(IEnumerable<CodeInstruction> instructions)
        //{
        //    return Transpilers.MethodReplacer(instructions, (MethodBase)Utils.Method<UnityEngine.Debug>("LogError", new Type[1] { typeof(object) }), (MethodBase)Utils.Method<FeatureManager>("LogErrorReplace"));
        //}
        [HarmonyPatch(typeof(NetPlayer), "ReceiveMove")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ReceiveMovePatch(IEnumerable<CodeInstruction> instructions)
        {
            return Transpilers.MethodReplacer(
                instructions,
                AccessTools.Method(typeof(UnityEngine.Debug), "LogError", new[] { typeof(object) }),
                AccessTools.Method(typeof(FeatureManager), nameof(LogErrorReplace))
            );
        }
        static readonly FieldInfo inputDeviceChangeField = AccessTools.Field(typeof(MenuSystem), "InputDeviceChange");
        [HarmonyPatch(typeof(MenuSystem), "SetInputDevice")]
        [HarmonyPrefix]
        static bool Prefix_SetInputDevice(MenuSystem __instance, MenuSystem.eInputDeviceType device)
        {
            // 跳过空事件调用
            var callback = inputDeviceChangeField?.GetValue(__instance) as Action<MenuSystem.eInputDeviceType>;

            if (callback == null)
            {
                //UnityEngine.Debug.Log("[YxMod]Patch: InputDeviceChange is null, skipping SetInputDevice.");
                return false; // 跳过原方法，避免空指针
            }
            return true; 
        }

        public static void LogErrorReplace(string s)
        {
        }
        internal static void TryInitAutoClimb()
        {
            if (autoClimb)
            {
                climbState = 0;
                Vector3 vector = (Human.Localplayer.ragdoll.partLeftHand.transform.position - Human.Localplayer.ragdoll.partRightHand.transform.position).ZeroY();
                float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
                Human.Localplayer.controls.cameraYawAngle =
                    ((Human.Localplayer.controls.cameraYawAngle - num) % 360f < 180f)
                        ? (num + 90f)
                        : (num - 90f);
            }
        }
        [HarmonyPatch(typeof(MenuCameraEffects), nameof(MenuCameraEffects.SetupViewports))]
        [HarmonyPrefix]
        static bool Prefix_SetupViewports(MenuCameraEffects __instance)
        {
            if (!UI_SheZhi.splitScreenEnabled)
            {
                return false; 
            }
            // 分屏开时走原方法
            return true;
        }
        [HarmonyPatch(typeof(RagdollCustomization), "AllowPaint", MethodType.Getter)]
        [HarmonyPostfix]
        public static void AllowPaint(ref bool __result)
        {
            __result = true;
        }
    }
}

