using Multiplayer;
using System;
using System.Reflection;
using UnityEngine;
using YxModDll.Mod;
using YxModDll.Patches;
using static NameTag;

namespace YxModDll.Patches
{
    public class Patcher_NameTag : MonoBehaviour
    {
        private static FieldInfo _forceShow;

        private static FieldInfo _currentWaitTime;
        private static FieldInfo _player;
        private static FieldInfo _previousRenderState;
        private static FieldInfo _nameTagCallback;
        private static FieldInfo _rendererBG;
        private static FieldInfo _rendererText;




        private void Awake()
        {


            _forceShow = typeof(NameTag).GetField("forceShow", BindingFlags.NonPublic | BindingFlags.Instance);
            _currentWaitTime = typeof(NameTag).GetField("currentWaitTime", BindingFlags.NonPublic | BindingFlags.Instance);
            _player = typeof(NameTag).GetField("player", BindingFlags.NonPublic | BindingFlags.Instance);
            _previousRenderState = typeof(NameTag).GetField("previousRenderState", BindingFlags.NonPublic | BindingFlags.Static);
            _nameTagCallback = typeof(NameTag).GetField("nameTagCallback", BindingFlags.NonPublic | BindingFlags.Static);
            _rendererBG = typeof(NameTag).GetField("rendererBG", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            _rendererText = typeof(NameTag).GetField("rendererText", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);



            Patcher2.MethodPatch(typeof(NameTag), "Update", null, typeof(Patcher_NameTag), "NameTag_Update", new Type[] { typeof(NameTag) });

        }


        public static void NameTag_Update(NameTag instance)
        {
            var forceShow = (bool)_forceShow.GetValue(instance);
            var currentWaitTime = (float)_currentWaitTime.GetValue(instance);
            if (UI_SheZhi.xianshimingzi)/////修改
            {
                try
                {
                    if (ShouldDisplay(instance))
                    {
                        forceShow = true;
                        _forceShow.SetValue(instance, forceShow);
                        currentWaitTime = 5f;
                        _currentWaitTime.SetValue(instance, currentWaitTime);

                        EnableRenderers(instance, enable: true);
                    }
                }
                catch { }
                return;
            }
            if (currentWaitTime > 0f)
            {
                currentWaitTime -= Time.deltaTime;
                _currentWaitTime.SetValue(instance, currentWaitTime);
                if (currentWaitTime < 0f)
                {
                    EnableRenderers(instance, enable: false);
                }
            }
            else if (ShouldDisplay(instance) && (Options.controllerBindings.ViewPlayerNames.IsPressed || Options.keyboardBindings.ViewPlayerNames.IsPressed))
            {
                forceShow = true;
                _forceShow.SetValue(instance, forceShow);
                currentWaitTime = 5f;
                _currentWaitTime.SetValue(instance, currentWaitTime);
                EnableRenderers(instance, enable: true);
            }
            else
            {
                forceShow = false;
                _forceShow.SetValue(instance, forceShow);
            }
        }

        private static bool ShouldDisplay(NameTag instance)
        {
            var player = (NetPlayer)_player.GetValue(instance);

            return !player.isLocalPlayer && MenuSystem.instance.activeMenu == null;
        }
        private static void EnableRenderers(NameTag instance, bool enable)
        {
            var previousRenderState = (bool)_previousRenderState.GetValue(null);
            var nameTagCallback = (NameTagActivatedCallBack)_nameTagCallback.GetValue(null);
            var rendererBG = (Renderer)_rendererBG.GetValue(instance);
            var rendererText = (Renderer)_rendererText.GetValue(instance);


            if (previousRenderState != enable)
            {
                //nameTagCallback(enable);
                nameTagCallback?.Invoke(enable);
                previousRenderState = enable;
                _previousRenderState.SetValue(null, previousRenderState);
            }
            rendererBG.enabled = enable;
            //_rendererBG.SetValue(instance, rendererBG);
            rendererText.enabled = enable;
            //_rendererText.SetValue(instance, rendererText);
        }
    }
}

