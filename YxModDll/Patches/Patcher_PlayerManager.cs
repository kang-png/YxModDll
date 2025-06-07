using HumanAPI;
using InControl;
using Multiplayer;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;
using Voronoi2;
using YxModDll.Patches;
using static MenuCameraEffects;
using static Multiplayer.NetTransport;
using static UnityEngine.UI.Image;


namespace YxModDll.Patches
{
    public class Patcher_PlayerManager : MonoBehaviour
    {
        private static FieldInfo _activeDevices;

        //private List<InputDevice> activeDevices = new List<InputDevice>();

        public static bool huicheliaotian = true;
        private void Awake()
        {
            _activeDevices = typeof(PlayerManager).GetField("activeDevices", BindingFlags.NonPublic | BindingFlags.Instance);
            Patcher2.MethodPatch(typeof(PlayerManager), "OnLocalPlayerAdded", new[] { typeof(NetPlayer) }, typeof(Patcher_PlayerManager), "OnLocalPlayerAdded", new[] { typeof(PlayerManager), typeof(NetPlayer) });

        }

        public static void OnLocalPlayerAdded(PlayerManager instance, NetPlayer player)
        {
            var activeDevices = (List<InputDevice>)_activeDevices.GetValue(instance);

            instance.ApplyControls();
            if (NetGame.instance.local.players.Count > 100 && activeDevices.Count < 200)//修改
            {
                NetGame.instance.RemoveLocalPlayer(player);
            }
        }
    }
}

