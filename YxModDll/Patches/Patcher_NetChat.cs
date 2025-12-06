using HumanAPI;
using Multiplayer;
using Steamworks;
using System;
using System.Collections;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.ImageEffects;
using Voronoi2;
using YxModDll.Mod;
using YxModDll.Patches;
using static MenuCameraEffects;
using static UnityEngine.UI.Image;
using System.Collections.Generic;

namespace YxModDll.Patches
{
    public class Patcher_NetChat : MonoBehaviour
    {
        private static FieldInfo _phase;
        private static FieldInfo _speed;
        private static FieldInfo _contents;
        private static FieldInfo _dismissIn;

        private static FieldInfo _serverCommands;
        private static FieldInfo _clientCommands;

        //private static MethodInfo ShowMethod;
        private RectTransform chatPanelRect;
        private bool showChatLog;
        private Vector2 chatScroll;
        private int previousMessageCount;
        private bool previousVisible;

        public static bool huicheliaotian = true;
        private void Awake()
        {
            //ShowMethod = typeof(NetChat).GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Instance);
            _phase = typeof(NetChat).GetField("phase", BindingFlags.NonPublic | BindingFlags.Instance);
            _speed = typeof(NetChat).GetField("speed", BindingFlags.NonPublic | BindingFlags.Instance);
            _contents = typeof(NetChat).GetField("contents", BindingFlags.NonPublic | BindingFlags.Static);
            _dismissIn = typeof(NetChat).GetField("dismissIn", BindingFlags.NonPublic | BindingFlags.Instance);
            _serverCommands = typeof(NetChat).GetField("serverCommands", BindingFlags.Public | BindingFlags.Static);
            _clientCommands = typeof(NetChat).GetField("clientCommands", BindingFlags.Public | BindingFlags.Static);


            Patcher2.MethodPatch(typeof(NetChat), "Update", null, typeof(Patcher_NetChat), "NetChat_Update", null);
            chatPanelRect = NetChat.instance.receivedUI.GetComponent<RectTransform>();


        }
        public enum ChatTranslateMode
        {
            None,        // 번역 안 함
            TranslateToCN, // 자동 감지 → 중국어  
            TranslateToEN,
            TranslateToJP// 자동 감지 → 영어
        }
        public static ChatTranslateMode CurrentMode = ChatTranslateMode.None;
        private static string GetModeButtonLabel()
        {
            switch (CurrentMode)
            {
                case ChatTranslateMode.None:
                    return "No Translate";
                case ChatTranslateMode.TranslateToCN:
                    return "TranslateToCN";
                case ChatTranslateMode.TranslateToEN:
                    return "TranslateToEN";
                case ChatTranslateMode.TranslateToJP:
                    return "TranslateToJP";
                default:
                    return "Translate send";
            }
        }

        private void OnGUI()
        {
            if (!NetChat.visible)
            {
                showChatLog = false;
                return;
            }
            Vector3[] array = new Vector3[4];
            chatPanelRect.GetWorldCorners(array);
            Vector3 vector = RectTransformUtility.WorldToScreenPoint(null, array[1]);
            float x = vector.x;
            float num = (float)Screen.height - vector.y;
            float width = chatPanelRect.rect.width;
            float height = chatPanelRect.rect.height;
            if (GUI.Button(new Rect(x + 5f, num - 30f, 120f, 25f), GetModeButtonLabel()))
            {
                // 클릭 시 모드 순환
                if (CurrentMode == ChatTranslateMode.None)
                    CurrentMode = ChatTranslateMode.TranslateToCN;
                else if (CurrentMode == ChatTranslateMode.TranslateToCN)
                    CurrentMode = ChatTranslateMode.TranslateToEN;
                else if (CurrentMode == ChatTranslateMode.TranslateToEN)
                    CurrentMode = ChatTranslateMode.TranslateToJP;
                else
                    CurrentMode = ChatTranslateMode.None;


                Debug.Log($"[YxMod] 번역 모드 변경됨: {CurrentMode}");
            }
            if (GUI.Button(new Rect(x + width - 80f, num - 30f, 75f, 25f), showChatLog ? "Hide" : "Show"))
            {
                showChatLog = !showChatLog;
                if (NetChat.instance.receivedUI != null)
                {
                    NetChat.instance.receivedUI.SetActive(!showChatLog);
                }
            }
            if (showChatLog)
            {
                GUIStyle guistyle = new GUIStyle(GUI.skin.box)
                {
                    alignment = TextAnchor.UpperLeft,
                    fontSize = 14,
                    normal =
                    {
                        textColor = Color.white
                    },
                    wordWrap = true
                };
                GUILayout.BeginArea(new Rect(x, num, width, height));
                GUILayout.BeginVertical("box", new GUILayoutOption[]
                {
                    GUILayout.Width(width),
                    GUILayout.Height(height)
                });
                int count = ChatLog.Messages.Count;
                chatScroll = GUILayout.BeginScrollView(chatScroll, new GUILayoutOption[] { GUILayout.ExpandHeight(true) });
                foreach (string text in ChatLog.Messages)
                {
                    GUILayout.Label(text, guistyle, Array.Empty<GUILayoutOption>());
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                GUILayout.EndArea();
                if (count != previousMessageCount)
                {
                    chatScroll.y = float.MaxValue;
                    previousMessageCount = count;
                }
            }
        }
        public static class ChatLog
        {

            public static void AddMessageToLog(string message)
            {
                if (ChatLog.chatMessages.Count >= 3000)
                {
                    ChatLog.chatMessages.RemoveAt(0);
                }
                ChatLog.chatMessages.Add(message);
            }


            public static IReadOnlyList<string> Messages
            {
                get
                {
                    return ChatLog.chatMessages;
                }
            }
            private static List<string> chatMessages = new List<string>();
        }
        public void NetChat_Update()
        {
            var phase = (float)_phase.GetValue(NetChat.instance);
            var speed = (float)_speed.GetValue(NetChat.instance);
            var contents = (string)_contents.GetValue(null);
            var dismissIn = (float)_dismissIn.GetValue(NetChat.instance);


            phase = Mathf.Clamp01(phase + speed * Time.deltaTime);
            _phase.SetValue(NetChat.instance, phase);

            if (GetComponent<CanvasGroup>().alpha != phase)
            {
                GetComponent<CanvasGroup>().alpha = phase;
                if (phase > 0f != NetChat.instance.receivedUI.activeSelf)
                {
                    NetChat.instance.receivedUI.SetActive(phase > 0f);
                }
            }
            NetChat.allowChat = MenuSystem.instance.activeMenu == null || MenuSystem.instance.activeMenu is MultiplayerLobbyMenu;
            if (!NetGame.isNetStarted && !string.IsNullOrEmpty(contents))
            {
                contents = string.Empty;
                _contents.SetValue(NetChat.instance, contents);

                NetChat.CropContents();
            }
            if (!NetGame.isNetStarted || !NetChat.allowChat)
            {
                if (NetChat.visible)
                {
                    Show(showMessages: false, showType: false);
                }
                return;
            }
            if (!NetChat.typing && NetChat.visible && dismissIn > 0f)
            {
                dismissIn -= Time.deltaTime;
                _dismissIn.SetValue(NetChat.instance, dismissIn);
                if (dismissIn <= 0f)
                {
                    Show(showMessages: false, showType: false);
                }
            }
            if (MenuSystem.keyboardState != 0 && MenuSystem.keyboardState != KeyboardState.NetChat)
            {
                return;
            }
            //修改
            if ((Input.GetKeyDown(KeyCode.T) || (Input.GetKeyDown(KeyCode.Return) && UI_SheZhi.huicheliaotian)) && !NetChat.typing)
            {
                if (Options.parental == 0)
                {
                    Show(showMessages: true, showType: true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return) && NetChat.typing)
            {
                string text = NetChat.instance.input.text.Trim();
                if (string.IsNullOrEmpty(text))
                {
                    NetChat.instance.input.text = string.Empty;
                    NetChat.instance.input.DeactivateInputField();
                    Show(showMessages: false, showType: false);
                    return;
                }
                if (text.StartsWith("/"))
                {
                    // 获取 CommandRegistry 类的 Execute 方法
                    MethodInfo executeMethod = typeof(CommandRegistry).GetMethod("Execute", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (NetGame.isServer || NetGame.isLocal)
                    {
                        //NetChat.serverCommands.Execute(text.Substring(1));
                        var serverCommands = _serverCommands.GetValue(null);
                        executeMethod.Invoke(serverCommands, new object[] { text.Substring(1) });
                    }
                    else
                    {
                        //NetChat.clientCommands.Execute(text.Substring(1));
                        var clientCommands = _clientCommands.GetValue(null);
                        executeMethod.Invoke(clientCommands, new object[] { text.Substring(1) });
                    }
                }
                else
                {
                    //NetChat.instance.Send(text);
                    Chat.Send(text);
                }
                NetChat.instance.input.text = string.Empty;
                Show(showMessages: true, showType: false);
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && NetChat.typing)
            {
                NetChat.instance.input.text = string.Empty;
                NetChat.instance.input.DeactivateInputField();
                Show(showMessages: false, showType: false);
            }
        }

        //private void Show(bool showMessages, bool showType)
        //{
        //    ShowMethod.Invoke(NetChat.instance, new object[] { showMessages, showType });
        //}


        private void Show(bool showMessages, bool showType)
        {
            var speed = (float)_speed.GetValue(NetChat.instance);


            if (showMessages || showType)
            {
                speed = 10f;

            }
            else
            {
                speed = -2f;
            }
            _speed.SetValue(NetChat.instance, speed);
            if (showType)
            {
                showMessages = true;
            }
            NetChat.visible = showMessages;
            NetChat.typing = showType;
            NetChat.instance.typeUI.SetActive(NetChat.typing);
            if (NetChat.typing)
            {
                NetChat.instance.input.ActivateInputField();
            }
            if (NetChat.typing)
            {
                MenuSystem.keyboardState = KeyboardState.NetChat;
                if (MenuSystem.instance != null)
                {
                    MenuSystem.instance.FocusOnMouseOver(enable: false);
                }
            }
            else
            {
                if (MenuSystem.instance != null)
                {
                    MenuSystem.instance.FocusOnMouseOver(enable: true);
                }
                StartCoroutine(UnblockMenu());
            }
        }
        private IEnumerator UnblockMenu()
        {
            yield return new WaitForSeconds(0.1f);
            MenuSystem.keyboardState = KeyboardState.None;
        }
    }
}

