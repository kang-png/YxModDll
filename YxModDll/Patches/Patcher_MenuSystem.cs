using HumanAPI;
using InControl;
using Multiplayer;
using System.Collections;
using System.Reflection;
using UnityEngine;
using YxModDll.Mod;
using static UnityEngine.EventSystems.StandaloneInputModule;

namespace YxModDll.Patches

{
    public class Patcher_MenuSystem : MonoBehaviour
    {

        private static FieldInfo _mouseMode;
        private static FieldInfo _controllerLastInput;
        private static FieldInfo _gameMainObject;
        private static FieldInfo _useMenuInput;
        private static FieldInfo _focusOnMouseOver;
        private static FieldInfo _inputModule;


        public static bool showshubiao;

        private void Awake()
        {
            _mouseMode = typeof(MenuSystem).GetField("mouseMode", BindingFlags.NonPublic | BindingFlags.Instance);
            _controllerLastInput = typeof(MenuSystem).GetField("controllerLastInput", BindingFlags.NonPublic | BindingFlags.Instance);
            _gameMainObject = typeof(MenuSystem).GetField("gameMainObject", BindingFlags.NonPublic | BindingFlags.Instance);
            _useMenuInput = typeof(MenuSystem).GetField("useMenuInput", BindingFlags.NonPublic | BindingFlags.Instance);
            _focusOnMouseOver = typeof(MenuSystem).GetField("focusOnMouseOver", BindingFlags.NonPublic | BindingFlags.Instance);
            _inputModule = typeof(MenuSystem).GetField("inputModule", BindingFlags.NonPublic | BindingFlags.Instance);



            Patcher2.MethodPatch(typeof(MenuSystem), "BindCursor", new[] { typeof(bool) }, typeof(Patcher_MenuSystem), "BindCursor", new[] { typeof(MenuSystem), typeof(bool) });
        }

        //private void OnGUI()
        //{
        //    if (GUILayout.Button("开关"))
        //    {
        //        showshubiao = !showshubiao;
        //        Debug.Log(showshubiao);
        //    }
        //}


        public static void BindCursor(MenuSystem instance, bool force = true)
        {

            var mouseMode = (bool)_mouseMode.GetValue(instance);
            var controllerLastInput = (bool)_controllerLastInput.GetValue(instance);
            var gameMainObject = (Game)_gameMainObject.GetValue(instance);
            var useMenuInput = (bool)_useMenuInput.GetValue(instance);
            var focusOnMouseOver = (bool)_focusOnMouseOver.GetValue(instance);
            var inputModule = (InControlInputModule)_inputModule.GetValue(instance);


            bool flag = Options.controllerBindings.HasInput();
            if (flag)
            {
                mouseMode = false;
            }
            if (InputModuleActionAdapter.actions != null && InputModuleActionAdapter.actions.Move.Value != Vector2.zero)
            {
                mouseMode = false;
            }
            if (Options.keyboardBindings != null)
            {
                if (Options.keyboardBindings.Move.Value != Vector2.zero)
                {
                    mouseMode = false;
                }
                if (Options.keyboardBindings.Look.Value != Vector2.zero)
                {
                    mouseMode = true;
                }
            }
            if (Input.GetAxisRaw("mouse x") != 0f || Input.GetAxisRaw("mouse y") != 0f)
            {
                controllerLastInput = false;
            }
            if (flag)
            {
                controllerLastInput = true;
            }
            if ((gameMainObject.state == GameState.PlayingLevel || gameMainObject.state == GameState.LoadingLevel) && instance.state == MenuSystemState.Inactive)
            {
                mouseMode = useMenuInput && !controllerLastInput;
                //if (useMenuInput && mouseMode)
                if ((useMenuInput && mouseMode) || UI_Main.ShowShuBiao || NetChat.typing)
                {
                    if (force || instance.mouseBlocker.activeSelf)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                        instance.mouseBlocker.SetActive(value: false);
                    }
                    if (focusOnMouseOver != inputModule.focusOnMouseHover)
                    {
                        inputModule.focusOnMouseHover = focusOnMouseOver;
                    }
                }
                else
                {
                    if (force || !instance.mouseBlocker.activeSelf)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                        Cursor.SetCursor(Texture2D.blackTexture, Vector2.zero, CursorMode.ForceSoftware);
                        instance.mouseBlocker.SetActive(value: true);
                    }
                    inputModule.focusOnMouseHover = false;
                }
            }
            else
            {
                if (force || instance.mouseBlocker.activeSelf)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    instance.mouseBlocker.SetActive(value: false);
                }
                if (focusOnMouseOver != inputModule.focusOnMouseHover)
                {
                    inputModule.focusOnMouseHover = focusOnMouseOver;
                }
            }
        }



    }
}

