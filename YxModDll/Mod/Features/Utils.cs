using HarmonyLib;
using HumanAPI;
using Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using YxModDll.Mod.Features;
using static HarmonyLib.AccessTools;

namespace YxModDll.Mod.Features;

public static class Utils
{
    public static FieldRef<FreeRoamCam, Camera> cam = FieldRef<FreeRoamCam, Camera>("cam");

    public static FieldRef<FreeRoamCam, FreeRoamCam.CameraKeyFrame[]> keyframes = FieldRef<FreeRoamCam, FreeRoamCam.CameraKeyFrame[]>("keyframes");

    public static FieldRef<Human, GrabManager> grabManager = FieldRef<Human, GrabManager>("grabManager");

    public static FieldRef<Human, GroundManager> groundManager = FieldRef<Human, GroundManager>("groundManager");

    public static FieldRef<Human, float> lastGroundAngle = FieldRef<Human, float>("lastGroundAngle");

    public static FieldRef<MenuSystem, bool> useMenuInput = (useMenuInput = FieldRef<MenuSystem, bool>("useMenuInput"));

    public static MethodInfo SetGameMode = Method<SteamRichPresence>("SetGameMode");

    public static MethodInfo SetLevelName = Method<SteamRichPresence>("SetLevelName");

    public static MethodInfo BeginReset = Method<SignalManager>("BeginReset");

    public static MethodInfo MakePlayers = Method<LevelInformationBox>("MakePlayers");

    public static MethodInfo MakeFlag = Method<LevelInformationBox>("MakeFlag");

    public static MethodInfo MakeLobbyTitle = Method<LevelInformationBox>("MakeLobbyTitle");

    public static MethodInfo OnLobbiesListed = Method<MultiplayerSelectLobbyMenu>("OnLobbiesListed");

    public static MethodInfo EnterMenu = Method<App>("EnterMenu");

    public static MethodInfo FinishCancelConnect = Method<App>("FinishCancelConnect");

    public static MethodInfo CheckStoredLevelAndLobbySubscriptions = Method<WorkshopRepository>("CheckStoredLevelAndLobbySubscriptions");

    public static MethodInfo ReadFolder = Method<WorkshopRepository>("ReadFolder");

    public static MethodInfo UpdateTitle = Method<LevelSelectMenu2>("UpdateTitle");

    public static MethodInfo DisableLevelContinue = Method<LevelSelectMenu2>("DisableLevelContinue");

    public static FieldRef<LevelSelectMenu2, bool> isMultiplayer = FieldRef<LevelSelectMenu2, bool>("isMultiplayer");

    public static MethodInfo InLevelSelectMenu = AccessTools.PropertySetter(typeof(LevelSelectMenu2), "InLevelSelectMenu");

    public static MethodInfo EnableShowLevelButtons = Method<LevelSelectMenu2>("EnableShowLevelButtons");

    public static MethodInfo BindLevelIfNeeded = Method<LevelSelectMenu2>("BindLevelIfNeeded");

    public static FieldRef<LevelSelectMenu2, WorkshopMenuItem> selectedMenuItem = FieldRef<LevelSelectMenu2, WorkshopMenuItem>("selectedMenuItem");

    public static FieldRef<LevelSelectMenu2, GameObject> previousSelectedItem = FieldRef<LevelSelectMenu2, GameObject>("previousSelectedItem");

    public static MethodInfo WaitAndSelect = Method<LevelSelectMenu2>("WaitAndSelect");

    public static FieldRef<ListView, int> orginalItemsInList = FieldRef<ListView, int>("orginalItemsInList");

    public static FieldRef<ListView, int> maxItemsOnScreen = FieldRef<ListView, int>("maxItemsOnScreen");

    public static FieldRef<ListView, bool> isHorizontal = FieldRef<ListView, bool>("isHorizontal");

    public static FieldRef<ListView, List<UnityEngine.UI.Button>> buttons = FieldRef<ListView, List<UnityEngine.UI.Button>>("buttons");

    public static FieldRef<object> stateLock = AccessTools.StaticFieldRefAccess<object>(Field<NetGame>("stateLock"));

    public static MethodInfo SetMultiplayerLobbyID = Method<NetGame>("SetMultiplayerLobbyID");

    public static FieldRef<LevelInformationBox, NetTransport.LobbyDisplayInfo> prevDispInfo = FieldRef<LevelInformationBox, NetTransport.LobbyDisplayInfo>("prevDispInfo");

    public static MethodInfo GetAllocArrayFromChannelImpl = Method<Mesh>("GetAllocArrayFromChannelImpl");

    public static FieldRef<SignalSoundPlayRandom, AudioSource> target = FieldRef<SignalSoundPlayRandom, AudioSource>("target");

    public static FieldRef<SignalSoundPlayRandom, AudioClip[]> randomSounds = FieldRef<SignalSoundPlayRandom, AudioClip[]>("randomSounds");

    public static FieldRef<SignalScriptPlayaRandomSound1, AudioClip> currentAudioClip = FieldRef<SignalScriptPlayaRandomSound1, AudioClip>("currentAudioClip");

    public static FieldRef<NetHost, bool> mute = FieldRef<NetHost, bool>("mute");

    public static MethodInfo SetupBodies = Method<NetPlayer>("SetupBodies");

    public static MethodInfo ApplyJumpImpulses = Method<LegMuscles>("ApplyJumpImpulses");

    public static MethodInfo RemoveServerPlayer = Method<NetGame>("RemoveServerPlayer");

    public static MethodInfo AddServerPlayer = Method<NetGame>("AddServerPlayer");

    public static MethodInfo CalculateFarCam = Method<CameraController3>("CalculateFarCam");

    public static MethodInfo CalculateCloseCam = Method<CameraController3>("CalculateCloseCam");

    public static MethodInfo CalculateFirstPersonCam = Method<CameraController3>("CalculateFirstPersonCam");

    public static MethodInfo SmoothCamera = Method<CameraController3>("SmoothCamera");

    public static MethodInfo CompensateForWallsNearPlane = Method<CameraController3>("CompensateForWallsNearPlane");

    public static MethodInfo SpringArm = Method<CameraController3>("SpringArm");

    public static FieldRef<CameraController3, Ragdoll> ragdoll = FieldRef<CameraController3, Ragdoll>("ragdoll");

    public static StructFieldRef<UnityEngine.Random.State, int>[] states = (from i in Enumerable.Range(0, 4)
                                                                            select AccessTools.StructFieldRefAccess<UnityEngine.Random.State, int>($"s{i}")).ToArray();

    public static FieldInfo Field<T>(string name)
    {
        return AccessTools.DeclaredField(typeof(T), name);
    }

    public static MethodInfo Method<T>(string name)
    {
        return AccessTools.DeclaredMethod(typeof(T), name, (Type[])null, (Type[])null);
    }

    public static MethodInfo Method<T>(string name, Type[] parameters)
    {
        return AccessTools.DeclaredMethod(typeof(T), name, parameters, (Type[])null);
    }

    public static PropertyInfo Property<T>(string name)
    {
        return AccessTools.DeclaredProperty(typeof(T), name);
    }

    public static FieldRef<T, F> FieldRef<T, F>(string name)
    {
        return AccessTools.FieldRefAccess<T, F>(name);
    }

    public static T Assign<T, V>(this T obj, Func<T, V> func, out V variable)
    {
        variable = func(obj);
        return obj;
    }

    public static T SelectMax<T, C>(this IEnumerable<T> ts, Func<T, C> func) where C : IComparable
    {
        bool flag = true;
        C val = default(C);
        T result = default(T);
        foreach (T t in ts)
        {
            C val2 = func(t);
            if (flag || val2.CompareTo(val) == 1)
            {
                flag = false;
                val = val2;
                result = t;
            }
        }
        return result;
    }

    public static T SelectMin<T, C>(this IEnumerable<T> ts, Func<T, C> func) where C : IComparable
    {
        bool flag = true;
        C val = default(C);
        T result = default(T);
        foreach (T t in ts)
        {
            C val2 = func(t);
            if (flag || val2.CompareTo(val) == -1)
            {
                flag = false;
                val = val2;
                result = t;
            }
        }
        return result;
    }

    public static T MaxOr<T>(this IEnumerable<T> ts, T or)
    {
        return (ts.Count() > 0) ? ts.Max() : or;
    }

    public static T MinOr<T>(this IEnumerable<T> ts, T or)
    {
        return (ts.Count() > 0) ? ts.Min() : or;
    }

    public static IEnumerator<T> GetEnumerator<T>(Type type, object instance)
    {
        return (IEnumerator<T>)type.GetMethod("GetEnumerator").Invoke(instance, new object[0]);
    }

    public static Delegate Partial<T>(T del, params object[] arg) where T : Delegate
    {
        return (Func<object[], object>)delegate (object[] args)
        {
            T val = del;
            object[] array = arg;
            int num = 0;
            object[] array2 = new object[array.Length + args.Length];
            object[] array3 = array;
            foreach (object obj in array3)
            {
                array2[num] = obj;
                num++;
            }
            foreach (object obj in args)
            {
                array2[num] = obj;
                num++;
            }
            return val.DynamicInvoke(array2);
        };
    }

    public static bool TryGetComponent<T>(this GameObject gameObject, out T component)
    {
        component = gameObject.GetComponent<T>();
        return component != null;
    }

    public static bool TryGetComponent<T>(this Transform transform, out T component)
    {
        component = transform.GetComponent<T>();
        return component != null;
    }

    public static bool ValueIs<T>(this FieldInfo fieldInfo, object obj, out T value)
    {
        value = default(T);
        if (fieldInfo.FieldType == typeof(T))
        {
            value = (T)fieldInfo.GetValue(obj);
            return true;
        }
        return false;
    }

    public static IEnumerator ScheduleCoroutine(int frames, Action action)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return new WaitForFixedUpdate();
        }
        action();
    }

    public static void Schedule(this MonoBehaviour behaviour, int frames, Action action)
    {
        behaviour.StartCoroutine(ScheduleCoroutine(frames, action));
    }

    public static void AddOrCreate<TKey>(this Dictionary<TKey, int> dict, TKey key)
    {
        if (!dict.ContainsKey(key))
        {
            dict[key] = 0;
        }
        dict[key]++;
    }

    public static int NewHashCode(this Vector3 vector)
    {
        int hashCode = vector.x.GetHashCode();
        int hashCode2 = vector.y.GetHashCode();
        int hashCode3 = vector.z.GetHashCode();
        return hashCode * 11451 + hashCode2 * 191981 + hashCode3;
    }

    public static string ColoredVector3(Vector3 vector)
    {
        return $"<color=#000000>(<color=#FF0000>{vector.x:0.0}</color>, <color=#00FF00>{vector.y:0.0}</color>, <color=#0000FF>{vector.z:0.0}</color>)</color>";
    }

    public static string TrimInstance(this string s)
    {
        Match match = Regex.Match(s, " \\(\\d+\\)");
        if (match.Success)
        {
            s = s.Substring(0, match.Index);
        }
        bool flag;
        do
        {
            flag = false;
            if (s.EndsWith(" (Instance)"))
            {
                flag = true;
                s = s.Substring(0, s.Length - 11);
            }
            if (s.EndsWith(" Instance"))
            {
                flag = true;
                s = s.Substring(0, s.Length - 9);
            }
        }
        while (flag);
        return s;
    }

    public static ChildrenEnumerable Children(this Transform transform)
    {
        return new ChildrenEnumerable(transform);
    }

    public static void InitFields()
    {
    }
}
