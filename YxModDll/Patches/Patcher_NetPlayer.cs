using HumanAPI;
using Multiplayer;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using YxModDll.Mod;

namespace YxModDll.Patches
{

    public class Patcher_NetPlayer : MonoBehaviour//NetPlayer// NetScope//MonoBehaviour
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
        private static MethodInfo _applyPresetMethod;

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
            _applyPresetMethod = typeof(RagdollCustomization).GetMethod("ApplyPreset", BindingFlags.Instance | BindingFlags.NonPublic);

            if (_applyPresetMethod == null)
            {
                Debug.LogError("[YxMod] Failed to locate RagdollCustomization.ApplyPreset via reflection");
            }

            //Patcher2.MethodPatch(typeof(NetPlayer), "ApplySkin", new[] { typeof(byte[]) }, typeof(Patcher_NetPlayer), "NetPlayer_ApplySkin", new[] { typeof(NetPlayer), typeof(byte[]) });
            Patcher2.MethodPatch(typeof(NetPlayer), "PreFixedUpdate", null, typeof(Patcher_NetPlayer), "NetPlayer_PreFixedUpdate", new Type[] { typeof(NetPlayer) });
            //Patcher2.MethodPatch(typeof(NetPlayer), "ApplyPreset", new[] { typeof(RagdollPresetMetadata), typeof(bool), typeof(bool) }, typeof(Patcher_NetPlayer), "NetPlayer_ApplyPreset", new[] { typeof(NetPlayer), typeof(RagdollPresetMetadata), typeof(bool), typeof(bool) });

            //Patcher2.MethodPatch(typeof(NetPlayer), "PreFixedUpdate", null, typeof(Patcher_NetPlayer), "NetPlayer_PreFixedUpdate", null);
            //// 创建补丁实例
            //var patchInstance = new Patcher_NetPlayer();
            //Patcher2.MethodPatch(typeof(NetPlayer), "PreFixedUpdate", null, patchInstance, "NetPlayer_PreFixedUpdate", new Type[] { typeof(NetPlayer) });

        }
        public static void NetPlayer_ApplySkin(NetPlayer __instance, byte[] bytes)
        {
            RagdollPresetMetadata preset = RagdollPresetMetadata.Deserialize(bytes);

            if (preset == null ||
                !IsValidPart(preset.main, "main") ||
                !IsValidPart(preset.head, "head") ||
                !IsValidPart(preset.upperBody, "upperBody") ||
                !IsValidPart(preset.lowerBody, "lowerBody"))
            {
                Debug.LogWarning($"[YxMod] ApplySkin skipped: invalid skin data");
                __instance.skin = PresetRepository.CreateDefaultSkin();
                return;
            }

            __instance.skin = preset;
            __instance.skin.SaveNetSkin(__instance.localCoopIndex, __instance.skinUserId);
        }


        private static bool IsValidPart(RagdollPresetPartMetadata part, string name)
        {
            if (part == null)
            {
                Debug.LogWarning($"[YxMod] Skin part missing: {name}");
                return false;
            }
            if (part.bytes == null || part.bytes.Length < 8)
            {
                Debug.LogWarning($"[YxMod] Texture bytes missing or too small in: {name}");
                return false;
            }
            // PNG文件头判断（可选）
            if (!(part.bytes[0] == 0x89 && part.bytes[1] == 0x50))
            {
                Debug.LogWarning($"[YxMod] Texture in {name} is not PNG format.");
                return false;
            }
            return true;
        }

        public static void NetPlayer_ApplyPreset(NetPlayer __instance, RagdollPresetMetadata preset, bool bake, bool useBaseTexture)
        {
            if (preset == null)
            {
                Debug.LogWarning("[YxMod] ApplyPreset skipped: preset is null");
                return;
            }

            bool IsValidPart(RagdollPresetPartMetadata part, string name)
            {
                if (part != null && part.bytes != null && part.bytes.Length >= 16)
                    return true;

                Debug.LogWarning($"[YxMod] Texture bytes missing or too small in: {name}");
                return false;
            }

            if (!IsValidPart(preset.main, "main") ||
                !IsValidPart(preset.head, "head") ||
                !IsValidPart(preset.upperBody, "upperBody") ||
                !IsValidPart(preset.lowerBody, "lowerBody"))
            {
                Debug.LogWarning("[YxMod] ApplyPreset skipped: invalid skin data");
                __instance.skin = PresetRepository.CreateDefaultSkin(); // ✅ 加上这一句
                return;
            }


            if (__instance.customization == null)
            {
                __instance.customization = __instance.human.ragdoll.gameObject.AddComponent<RagdollCustomization>();
            }

            if (Patcher_NetPlayer._applyPresetMethod != null)
            {
                Patcher_NetPlayer._applyPresetMethod.Invoke(__instance.customization, new object[] { preset, true, useBaseTexture });
            }
            else
            {
                Debug.LogError("[YxMod] ApplyPreset reflection method is null!");
                return;
            }

            __instance.customization.RebindColors(bake, compress: true);
            __instance.customization.ClearOutCachedClipVolumes();
        }


        public static void NetPlayer_PreFixedUpdate(NetPlayer instance)
        {
            //NetPlayer instance = this;
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

            //var baseMethod = typeof(NetPlayer).BaseType.GetMethod("PreFixedUpdate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //baseMethod.Invoke(instance, null);

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
}



