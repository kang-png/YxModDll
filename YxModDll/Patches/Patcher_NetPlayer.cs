using Multiplayer;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using YxModDll.Mod;

namespace YxModDll.Patches
{

    public class Patcher_NetPlayer : NetPlayer// NetScope//MonoBehaviour
    {
        private static FieldInfo _moveLock;

        private static FieldInfo _walkForward;
        private static FieldInfo _walkRight;
        private static FieldInfo _cameraPitch;
        private static FieldInfo _cameraYaw;
        private static FieldInfo _leftExtend;
        private static FieldInfo _rightExtend;
        private static FieldInfo _jump;
        private static FieldInfo _playDead;
        private static FieldInfo _shooting;
        private static FieldInfo _holding;
        private static FieldInfo _moveFrames;

        public static bool renwubudong;

        private void Awake()
        {

            _moveLock = typeof(NetPlayer).GetField("moveLock", BindingFlags.NonPublic | BindingFlags.Instance);
            _walkForward = typeof(NetPlayer).GetField("walkForward", BindingFlags.NonPublic | BindingFlags.Instance);
            _walkRight = typeof(NetPlayer).GetField("walkRight", BindingFlags.NonPublic | BindingFlags.Instance);
            _cameraPitch = typeof(NetPlayer).GetField("cameraPitch", BindingFlags.NonPublic | BindingFlags.Instance);
            _cameraYaw = typeof(NetPlayer).GetField("cameraYaw", BindingFlags.NonPublic | BindingFlags.Instance);
            _leftExtend = typeof(NetPlayer).GetField("leftExtend", BindingFlags.NonPublic | BindingFlags.Instance);
            _rightExtend = typeof(NetPlayer).GetField("rightExtend", BindingFlags.NonPublic | BindingFlags.Instance);
            _jump = typeof(NetPlayer).GetField("jump", BindingFlags.NonPublic | BindingFlags.Instance);
            _playDead = typeof(NetPlayer).GetField("playDead", BindingFlags.NonPublic | BindingFlags.Instance);
            _shooting = typeof(NetPlayer).GetField("shooting", BindingFlags.NonPublic | BindingFlags.Instance);
            _holding = typeof(NetPlayer).GetField("holding", BindingFlags.NonPublic | BindingFlags.Instance);
            _moveFrames = typeof(NetPlayer).GetField("moveFrames", BindingFlags.NonPublic | BindingFlags.Instance);


            //Patcher2.MethodPatch(typeof(NetPlayer), "PreFixedUpdate", null, typeof(Patcher_NetPlayer), "NetPlayer_PreFixedUpdate", new Type[] { typeof(NetPlayer) });
            //Patcher2.MethodPatch(typeof(NetPlayer), "PreFixedUpdate", null, typeof(Patcher_NetPlayer), "NetPlayer_PreFixedUpdate", null);
            //// 创建补丁实例
            //var patchInstance = new Patcher_NetPlayer();
            //Patcher2.MethodPatch(typeof(NetPlayer), "PreFixedUpdate", null, patchInstance, "NetPlayer_PreFixedUpdate", new Type[] { typeof(NetPlayer) });

        }


        public void NetPlayer_PreFixedUpdate()
        {
            NetPlayer instance = this;

            var moveLock = (object)_moveLock.GetValue(instance);
            var walkForward = (float)_walkForward.GetValue(instance);
            var walkRight = (float)_walkRight.GetValue(instance);
            var cameraPitch = (float)_cameraPitch.GetValue(instance);
            var cameraYaw = (float)_cameraYaw.GetValue(instance);
            var leftExtend = (float)_leftExtend.GetValue(instance);
            var rightExtend = (float)_rightExtend.GetValue(instance);

            var jump = (bool)_jump.GetValue(instance);
            var playDead = (bool)_playDead.GetValue(instance);
            var shooting = (bool)_shooting.GetValue(instance);

            var holding = (bool)_holding.GetValue(instance);

            var moveFrames = (int)_moveFrames.GetValue(instance);



            //自由视角 /////修改
            if (instance.isLocalPlayer && ((UI_Main.ShowShuBiao && UI_SheZhi.noKong_xianshishubiao) || FreeRoamCam.allowFreeRoam))
            {
                return;
            }

            lock (moveLock)
            {
                if (instance.isLocalPlayer && !instance.human.disableInput)
                {
                    bool flag = true;
                    if (MenuSystem.instance.state == MenuSystemState.PauseMenu)
                    {
                        flag = false;
                    }
                    else
                    {
                        if ((App.state == AppSate.ServerLobby || App.state == AppSate.ClientLobby) && MenuSystem.instance.state != 0)
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
                        instance.controls.ReadInput(out walkForward, out walkRight, out cameraPitch, out cameraYaw, out leftExtend, out rightExtend, out jump, out playDead, out shooting);
                        _walkForward.SetValue(instance, walkForward);
                        _walkRight.SetValue(instance, walkRight);
                        _cameraPitch.SetValue(instance, cameraPitch);
                        _cameraYaw.SetValue(instance, cameraYaw);
                        _leftExtend.SetValue(instance, leftExtend);
                        _rightExtend.SetValue(instance, rightExtend);
                        _jump.SetValue(instance, jump);
                        _playDead.SetValue(instance, playDead);
                        _shooting.SetValue(instance, shooting);
                    }
                    else
                    {
                        walkForward = 0f;
                        _walkForward.SetValue(instance, walkForward);
                        walkRight = 0f;
                        _walkRight.SetValue(instance, walkRight);
                        jump = false;
                        _jump.SetValue(instance, jump);
                        shooting = false;
                        _shooting.SetValue(instance, shooting);
                    }
                }
                if (NetGame.isClient)
                {
                    instance.SendMove(walkForward, walkRight, cameraPitch, cameraYaw, leftExtend, rightExtend, jump, playDead, shooting);
                }
                else
                {

                    holding = instance.human.hasGrabbed;
                    _holding.SetValue(instance, holding);
                }
                instance.controls.HandleInput(walkForward, walkRight, cameraPitch, cameraYaw, leftExtend, rightExtend, jump, playDead, holding, shooting);
                moveFrames = 0;
                _moveFrames.SetValue(instance, moveFrames);
            }
        }
    }
    // ✅ 放在 namespace 中，保持 public static
    //public static class NetPlayerExtHelper
    //{
    //    private static readonly ConditionalWeakTable<NetPlayer, NetPlayerReflectionAccessor> _ext
    //        = new ConditionalWeakTable<NetPlayer, NetPlayerReflectionAccessor>();

    //    public static NetPlayerReflectionAccessor GetAccessor(this NetPlayer player)
    //        => _ext.GetOrCreateValue(player);
    //}

    //public class NetPlayerReflectionAccessor
    //{
    //    private readonly NetPlayer player;

    //    private static readonly FieldInfo playDeadField =
    //        typeof(NetPlayer).GetField("playDead", BindingFlags.Instance | BindingFlags.NonPublic);

    //    public NetPlayerReflectionAccessor(NetPlayer player)
    //    {
    //        this.player = player ?? throw new ArgumentNullException(nameof(player));
    //    }

    //    public bool playDead
    //    {
    //        get => (bool)playDeadField.GetValue(player);
    //        set => playDeadField.SetValue(player, value);
    //    }
    //}
}



