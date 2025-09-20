using I2.Loc;
using Multiplayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;




namespace YxModDll.Mod
{
    internal class UI
    {
        public static Texture2D anniuTexture = new Texture2D(1, 1);
        public static Texture2D anniuTexture2 = new Texture2D(1, 1);//选中按下时
        public static Texture2D anniuTexture3 = new Texture2D(1, 1);//移动至按钮时

        private static string[] humanNames = new string[0]; // 初始为空的按钮名称数组
        public static int ZiJiId;
        public static float buttonHeight;
        public static string currentTooltip = null;
        public static Vector2 currentTooltipPos;
        private static readonly GUIContent _tempContent = new GUIContent();

        private static GUIStyle _styleSelectionGrid;
        private static GUIStyle _styleButtonTabTrue;
        private static GUIStyle _styleButtonTabFalse;
        private static GUIStyle _styleButton;
        private static GUIStyle _styleButtonLeft;
        private static GUIStyle _styleTxt;
        private static GUIStyle _styleLabelCenter;
        private static bool _stylesInited = false;
        public static void EnsureStyles()
        {
            if (_stylesInited) return;

            _styleSelectionGrid = CreateStyleSelectionGrid();
            _styleButtonTabTrue = CreateStyleButtonTab(true);
            _styleButtonTabFalse = CreateStyleButtonTab(false);
            _styleButton = CreateStyleButton();
            _styleButtonLeft = CreateStyleButtonLeft();
            _styleTxt = CreateTxtStyle();
            _styleLabelCenter = CreateLabelStyleCenter();

            _stylesInited = true;
        }
        public static void UI_ChuShiHua()
        {
            anniuTexture.SetPixel(0, 0, new Color32(130, 130, 130, 255));//按钮未按下背景色
            anniuTexture.Apply();

            anniuTexture2.SetPixel(0, 0, new Color32(0, 0xbe, 0x92, 255));//按钮按下
            anniuTexture2.Apply();

            anniuTexture3.SetPixel(0, 0, new Color32(0, 0x70, 0xd7, 255));//按钮按下
            anniuTexture3.Apply();

            //LoadCacheFromFile(); // 加载翻译缓存
            //Debug.Log("buttonHeight : " + buttonHeight);
        }
        private static GUIStyle CreateStyleSelectionGrid()
        {
            var styleButton = new GUIStyle(GUI.skin.button)
            {
                normal = { background = anniuTexture, textColor = new Color32(220, 220, 220, 255) },
                onNormal = { background = anniuTexture2 },
                active = { background = anniuTexture2 },
                hover = { background = anniuTexture2, textColor = Color.white },
                onHover = { background = anniuTexture2 },
                alignment = TextAnchor.MiddleLeft,
                fontSize = 20,
                richText = true
            };
            return styleButton;
        }

        private static GUIStyle CreateStyleButtonTab(bool tab)
        {
            var styleButton = new GUIStyle(GUI.skin.button)
            {
                normal = { background = tab ? anniuTexture2 : anniuTexture, textColor = new Color32(220, 220, 220, 255) },
                active = { background = anniuTexture2 },
                hover = { background = tab ? anniuTexture : anniuTexture2, textColor = Color.white },
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20
            };
            return styleButton;
        }

        private static GUIStyle CreateStyleButton()
        {
            var styleButton = new GUIStyle(GUI.skin.button)
            {
                normal = { background = anniuTexture, textColor = Color.white },
                active = { background = anniuTexture2 },
                hover = { background = anniuTexture2, textColor = Color.black },
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20
            };
            return styleButton;
        }
        private static GUIStyle CreateStyleButtonLeft()
        {
            var styleButton = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 20
            };
            styleButton.normal.background = anniuTexture;
            styleButton.active.background = anniuTexture2;
            styleButton.hover.background = anniuTexture2;
            styleButton.normal.textColor = Color.white;
            styleButton.hover.textColor = Color.black;
            return styleButton;
        }

        private static GUIStyle CreateTxtStyle()
        {
            var styleTxt = new GUIStyle(GUI.skin.textField)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 20
            };
            styleTxt.normal.textColor = Color.white;
            return styleTxt;
        }

        private static GUIStyle CreateLabelStyleCenter()
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20
            };
            style.normal.textColor = Color.gray;
            return style;
        }

        // 缓存上一次生成的玩家名字，用于比较是否变化
        private static List<string> lastHumanNames = new List<string>();
        public static int CreatHumanList(int index, bool quanyuankongzhi = true)
        {
            try
            {
                List<string> tempNames = new List<string>();

                // 全员控制按钮
                if (quanyuankongzhi)
                {
                    string quanyuan = UI.TranslateButtonText("全员控制");
                    tempNames.Add(ColorfulSpeek.colorshows(quanyuan));
                }

                // 房主按钮
                string fzname = $"1.{NetGame.instance.server.name}";
                tempNames.Add(ColorfulSpeek.colorshows(fzname));

                // 客户机玩家按钮
                List<NetHost> hosts = NetGame.instance.readyclients;
                for (int j = 0; j < hosts.Count; j++)
                {
                    string name = $"{j + 2}.{hosts[j].name}";
                    tempNames.Add(ColorfulSpeek.colorshows(name));
                }

                // 本地玩家分身按钮
                for (int k = 1; k < NetGame.instance.local.players.Count; k++)
                {
                    string cloneName = $"{hosts.Count + k + 1}.{NetGame.instance.local.players[k].human.name}分身";
                    tempNames.Add(ColorfulSpeek.colorshows(cloneName));
                }

                // 检查是否与上一次相同
                bool listChanged = lastHumanNames.Count != tempNames.Count;
                if (!listChanged)
                {
                    for (int i = 0; i < tempNames.Count; i++)
                    {
                        if (lastHumanNames[i] != tempNames[i])
                        {
                            listChanged = true;
                            break;
                        }
                    }
                }

                // 仅在变化时更新数组并触发 GUI 刷新
                if (listChanged)
                {
                    humanNames = tempNames.ToArray();
                    lastHumanNames = new List<string>(tempNames); // 缓存最新列表
                    GUI.changed = true; // 只刷新一次
                }

                // 绘制 SelectionGrid
                index = GUILayout.SelectionGrid(index, humanNames, 1, styleSelectionGrid());

                // 防止 index 超出范围
                int totalCount = humanNames.Length;
                index = (index >= totalCount) ? 0 : index;

                return index;
            }
            catch (Exception e)
            {
                Debug.LogError("CreatHumanList 出错: " + e);
                return 0;
            }
        }

        public static GUIStyle styleSelectionGrid() => _styleSelectionGrid;
        public static GUIStyle styleButton_Tab(bool tab) => tab ? _styleButtonTabTrue : _styleButtonTabFalse;
        public static GUIStyle styleButton() => _styleButton;

        public static GUIStyle styleButton_Left() => _styleButtonLeft;
        public static GUIStyle SetTxtStyle() => _styleTxt;
        public static GUIStyle SetLabelStyle_JuZhong() => _styleLabelCenter;

        // 缓存高度
        internal static readonly Dictionary<string, float> heightCache = new();

        public static float GetButtonHeight(GUIStyle style, string sampleText = "按钮")
        {
            if (heightCache.TryGetValue(sampleText, out float h))
            {
                return h;
            }

            float lineHeight = style.fontSize + style.padding.vertical;

            _tempContent.text = sampleText;
            Vector2 size = style.CalcSize(_tempContent);

            h = Mathf.Max(lineHeight, size.y);
            heightCache[sampleText] = h;

            return h;
        }


        public static LayoutDisposable Horizontal(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
            return new LayoutDisposable(GUILayout.EndHorizontal);
        }

        public static LayoutDisposable Vertical(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(options);
            return new LayoutDisposable(GUILayout.EndVertical);
        }

        public static LayoutDisposable ScrollView(ref Vector2 scrollPos, params GUILayoutOption[] options)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, options);
            return new LayoutDisposable(GUILayout.EndScrollView);
        }

        /// <summary>
        /// 布局 disposable 类，利用using语句自动调用End方法
        /// </summary>
        public struct LayoutDisposable : IDisposable
        {
            private Action _onDispose;

            public LayoutDisposable(Action onDispose)
            {
                _onDispose = onDispose;
            }

            // using块结束时自动调用
            public void Dispose()
            {
                _onDispose?.Invoke();
            }
        }
        public static void CreateHotKey(string name, ref List<KuaiJieJian_Type> keycodes, int shuzi = 0, string tishi = "")
        {
            using (Horizontal())
            {
                if (!string.IsNullOrEmpty(name))
                {
                    name = TranslateButtonText(name);
                    GUILayout.Label(GetColoredName(name), GUILayout.Width(100));
                }

                string combinedKeys;
                if (keycodes != null && keycodes.Count > 0)
                {
                    List<string> keyStrings = new List<string>();
                    foreach (var keycode in keycodes)
                    {
                        if (keycode.keyCode1 != KeyCode.None)
                        {
                            string keystring = keycode.keyCode1.ToString();
                            if (keycode.keyCode2 != KeyCode.None)
                                keystring += $" + {keycode.keyCode2}";

                            if (shuzi == 1) keystring += " + Num";
                            else if (shuzi == 2) keystring += " + Num + Num";

                            keyStrings.Add(keystring);
                        }
                    }
                    combinedKeys = string.Join(" | ", keyStrings);
                }
                else
                {
                    combinedKeys = "无快捷键";
                }

                GUILayout.Label(GetColoredName(combinedKeys));

                if (!string.IsNullOrEmpty(tishi))
                {
                    tishi = TranslateButtonText(tishi);
                    GUILayout.Label(GetColoredName(tishi));
                }

                GUILayout.FlexibleSpace();
            }
        }

        // 缓存按钮显示文字，避免每帧生成
        internal static Dictionary<string, string> nameCache = new Dictionary<string, string>();

        private static string GetColoredName(string name)
        {
            if (!nameCache.TryGetValue(name, out string colored))
            {
                colored = ColorfulSpeek.colorshows(name);
                nameCache[name] = colored;
            }
            return colored;
        }
        internal static readonly Dictionary<string, GUIContent> contentCache = new();

        private static GUIContent GetContent(string text)
        {
            if (!contentCache.TryGetValue(text, out var content))
            {
                content = new GUIContent(text);
                contentCache[text] = content;
            }
            return content;
        }

        // 浮点版
        public static void CreatShuZhi(string name, ref float zhi, float min, float max, float add, Action callback = null, float? yuan = null)
        {
            GUILayout.BeginHorizontal();

            name = TranslateButtonText(name);
            GUILayout.Label(GetColoredName(name));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(GetColoredName("-"), UI.styleButton()))
            {
                float oldValue = zhi;
                zhi = Mathf.Max(min, Mathf.Round((zhi - add) * 10f) / 10f);
                if (Math.Abs(zhi - oldValue) > 0.001f) callback?.Invoke();
            }

            // 缓存显示字符串，减少每帧生成
            string displayStr = zhi.ToString("0.0");
            displayStr = GUILayout.TextField(displayStr, SetLabelStyle_JuZhong());
            if (float.TryParse(displayStr, out float inputValue))
            {
                inputValue = Mathf.Clamp(inputValue, min, max);
                float roundedValue = Mathf.Round(inputValue * 10f) / 10f;
                if (Math.Abs(zhi - roundedValue) > 0.001f)
                {
                    zhi = roundedValue;
                    callback?.Invoke();
                }
            }

            if (GUILayout.Button(GetColoredName("+"), UI.styleButton()))
            {
                float oldValue = zhi;
                zhi = Mathf.Min(max, Mathf.Round((zhi + add) * 10f) / 10f);
                if (Math.Abs(zhi - oldValue) > 0.001f) callback?.Invoke();
            }

            GUILayout.FlexibleSpace();
            if (yuan.HasValue && GUILayout.Button(GetColoredName("重置"), UI.styleButton()))
            {
                float oldValue = zhi;
                zhi = Mathf.Round(yuan.Value * 10f) / 10f;
                if (Math.Abs(zhi - oldValue) > 0.001f) callback?.Invoke();
            }

            GUILayout.EndHorizontal();
        }

        // 整数版
        public static void CreatShuZhi(string name, ref int zhi, int min, int max, int add, Action callback = null, int? yuan = null)
        {
            GUILayout.BeginHorizontal();

            name = TranslateButtonText(name);
            GUILayout.Label(GetColoredName(name));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(GetColoredName("-"), UI.styleButton()))
            {
                int oldValue = zhi;
                zhi = Mathf.Max(min, zhi - add);
                if (zhi != oldValue) callback?.Invoke();
            }

            string displayStr = zhi.ToString();
            displayStr = GUILayout.TextField(displayStr, SetLabelStyle_JuZhong());
            if (int.TryParse(displayStr, out int inputValue))
            {
                inputValue = Mathf.Clamp(inputValue, min, max);
                if (inputValue != zhi)
                {
                    zhi = inputValue;
                    callback?.Invoke();
                }
            }

            if (GUILayout.Button(GetColoredName("+"), UI.styleButton()))
            {
                int oldValue = zhi;
                zhi = Mathf.Min(max, zhi + add);
                if (zhi != oldValue) callback?.Invoke();
            }

            GUILayout.FlexibleSpace();
            if (yuan.HasValue && GUILayout.Button(GetColoredName("重置"), UI.styleButton()))
            {
                int oldValue = zhi;
                zhi = yuan.Value;
                if (zhi != oldValue) callback?.Invoke();
            }

            GUILayout.EndHorizontal();
        }

        // 文本框
        public static void CreatWenBenKuang(string name, ref string str, int maxChang, int KuanDuan, Action callback = null)
        {
            GUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(name))
            {
                name = TranslateButtonText(name);
                GUILayout.Label(GetColoredName(name));
            }

            string newStr = GUILayout.TextField(str, SetTxtStyle(), GUILayout.Width(KuanDuan));
            if (newStr != str)
            {
                if (newStr.Length > maxChang)
                    newStr = newStr.Substring(0, maxChang);

                str = newStr;
                callback?.Invoke();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        // 颜色按钮框
        public static void CreatYanSeKuang(string name, string strYanSe, Action callback)
        {
            GUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(name))
            {
                name = TranslateButtonText(name);
                GUILayout.Label(GetColoredName(name));
            }

            // 缓存按钮文本，避免每帧重复生成
            string btnText = $"<color={strYanSe}>{strYanSe}</color>";
            UI.CreatAnNiu(btnText, false, callback);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        // 分割线（线条样式可缓存）
        private static GUIStyle lineStyle;
        public static void CreatFenGeXian()
        {
            if (lineStyle == null)
            {
                lineStyle = new GUIStyle(GUI.skin.box);
                lineStyle.border = new RectOffset(0, 0, 0, 0);
            }

            GUILayout.Box("", lineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(2));
        }

        // 左侧按钮
        public static void CreatAnNiu_Left(string name, bool chuizhijuzhong = true, Action callback = null, string tooltip = null)
        {
            if (chuizhijuzhong)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
            }

            string coloredName = GetColoredName(name);
            GUIContent content = GetContent(coloredName);

            Rect buttonRect = GUILayoutUtility.GetRect(content, styleButton()); // 布局计算用 styleButton()

            if (GUI.Button(buttonRect, content, styleButton_Left()))
            {
                callback?.Invoke();
            }

            if (!string.IsNullOrEmpty(tooltip) && buttonRect.Contains(Event.current.mousePosition))
            {
                currentTooltip = tooltip;
            }

            if (chuizhijuzhong)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }

            buttonHeight = GetButtonHeight(styleButton(), coloredName) + 6;
        }

        // 一般按钮
        public static void CreatAnNiu(string name, bool chuizhijuzhong = true, Action callback = null, string tooltip = null)
        {
            if (chuizhijuzhong)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
            }

            name = TranslateButtonText(name);
            string coloredName = GetColoredName(name); // 缓存彩色字符串
            GUIContent content = GetContent(coloredName);

            Rect buttonRect = GUILayoutUtility.GetRect(content, styleButton());

            if (GUI.Button(buttonRect, content, styleButton()))
            {
                callback?.Invoke();
            }

            if (!string.IsNullOrEmpty(tooltip) && buttonRect.Contains(Event.current.mousePosition))
            {
                currentTooltip = TranslateButtonText(tooltip);
            }

            if (chuizhijuzhong)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }

            buttonHeight = GetButtonHeight(styleButton(), coloredName) + 6;
        }

        // Tab按钮
        public static void CreatAnNiu_AnXia(string name, ref bool tab, bool chuizhijuzhong = true, Action callback = null, string tooltip = null)
        {
            if (chuizhijuzhong)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
            }

            name = TranslateButtonText(name);
            string coloredName = GetColoredName(name); // 缓存彩色字符串
            GUIContent content = GetContent(coloredName);

            Rect buttonRect = GUILayoutUtility.GetRect(content, styleButton_Tab(tab));

            if (GUI.Button(buttonRect, content, styleButton_Tab(tab)))
            {
                tab = !tab;
                callback?.Invoke();
            }

            if (!string.IsNullOrEmpty(tooltip) && buttonRect.Contains(Event.current.mousePosition))
            {
                currentTooltip = TranslateButtonText(tooltip);
            }

            if (chuizhijuzhong)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }

            buttonHeight = GetButtonHeight(styleButton(), coloredName) + 6;
        }

        // 缓存样式
        private static GUIStyle tooltipStyle;
        private static GUIStyle uiBoxStyle;

        public static void CreatUiBox(Rect rect, Texture2D texture)
        {
            if (uiBoxStyle == null || uiBoxStyle.normal.background != texture)
            {
                uiBoxStyle = new GUIStyle(GUI.skin.box)
                {
                    normal = { background = texture }
                };
            }
            GUI.Box(rect, GUIContent.none, uiBoxStyle);
        }

        public static void DrawTooltip()
        {
            if (string.IsNullOrEmpty(currentTooltip)) return;

            if (tooltipStyle == null)
            {
                tooltipStyle = new GUIStyle(GUI.skin.box)
                {
                    wordWrap = true,
                    alignment = TextAnchor.UpperLeft,
                    padding = new RectOffset(8, 8, 6, 6)
                };
            }

            // 缓存彩色文本
            string coloredText = GetColoredName(currentTooltip);

            // 获取鼠标位置
            Vector2 mousePos = Input.mousePosition;
            mousePos.y = Screen.height - mousePos.y;

            float maxWidth = 300;
            Vector2 contentSize = tooltipStyle.CalcSize(new GUIContent(coloredText));
            float width = Mathf.Min(contentSize.x + 16, maxWidth);
            float height = tooltipStyle.CalcHeight(new GUIContent(coloredText), width);

            Rect rect = new Rect(mousePos.x + 15, mousePos.y + 15, width, height + 12);
            GUI.Box(rect, coloredText, tooltipStyle);
        }

        // 正在翻译的记录（防止重复请求）
        private static HashSet<Tuple<string, string>> pendingTranslations = new();

        // 调用方只用这个：返回已知翻译，没翻译就先返回原文，后台自动补全
        public static string TranslateButtonText(string originalText)
        {
            string targetLang = LocalizationManager.CurrentLanguage;
            //UnityEngine.Debug.Log($"当前语言: {targetLang}");
            if (targetLang == "Chinese Simplified")
                return originalText;
            // 查缓存
            if (buttonTranslations.TryGetValue(originalText, out var langDict))
            {
                if (langDict.TryGetValue(targetLang, out var result))
                {
                    return result;
                }
            }

            // 没翻译，触发后台翻译（不等待）
            //StartTranslationAsync(originalText, targetLang);

            // 返回原文（下次再回来会看到翻译结果）
            return originalText;
        }
        private static string LanguageNameToCode(string name)
        {
            return name switch
            {
                "English" => "en",
                "French" => "fra",
                "Spanish" => "spa",
                "German" => "de",
                "Russian" => "ru",
                "Italian" => "it",
                "Chinese" => "zh",
                "Chinese Simplified" => "zh",
                "ChineseTraditional" => "cht",
                "Japanese" => "jp",
                "Korean" => "kor",
                "Brazilian" => "pt", // 可选写 pt-BR，但通常 pt 足够
                "Portuguese" => "pt",
                "Turkish" => "tr",
                "Thai" => "th",
                "Indonesian" => "id",
                "Polish" => "pl",
                "Ukrainian" => "uk",
                "Arabic" => "ara",
                "Lithuanian" => "lt",
                _ => "en" // fallback
            };
        }

        // 异步翻译，并更新缓存（不重复请求）
        private static DateTime lastTranslateTime = DateTime.MinValue;

        private static async void StartTranslationAsync(string originalText, string targetLang)
        {
            var key = Tuple.Create(originalText, targetLang);
            if (pendingTranslations.Contains(key)) return;

            pendingTranslations.Add(key);

            // ✅ 频率限制：与上次翻译间隔 < 600ms，则等待
            TimeSpan delay = TimeSpan.FromMilliseconds(600);
            var timeSinceLast = DateTime.UtcNow - lastTranslateTime;
            if (timeSinceLast < delay)
                await Task.Delay(delay - timeSinceLast);

            lastTranslateTime = DateTime.UtcNow;

            // ✅ 语言名称 -> 语言代码
            string langCode = LanguageNameToCode(targetLang);

            string translated = await TranslateViaApi(originalText, langCode);
            pendingTranslations.Remove(key);

            if (!string.IsNullOrEmpty(translated))
            {
                if (!buttonTranslations.ContainsKey(originalText))
                    buttonTranslations[originalText] = new Dictionary<string, string>();

                buttonTranslations[originalText][targetLang] = translated;
                SaveCacheToFile(); // ✅ 保存到本地
            }
        }


        private static async Task<string> TranslateViaApi(string text, string toLang)
        {
            try
            {
                WWWForm form = new WWWForm();
                form.AddField("text", text);
                form.AddField("from", "auto");
                form.AddField("to", toLang);

                using (UnityWebRequest www = UnityWebRequest.Post("https://tools.mgtv100.com/external/v1/baidu_translate", form))
                {
                    var request = www.SendWebRequest();
                    while (!request.isDone)
                        await Task.Yield();

                    if (www.isNetworkError || www.isHttpError)
                    {
                        Debug.LogWarning($"翻译失败: {www.error}");
                        return null;
                    }

                    string json = www.downloadHandler.text;
                    Debug.Log($"翻译返回: {json}");

                    // ✅ 手动提取 "trans_result":["xxx"]
                    string key = "\"trans_result\":[\"";
                    int start = json.IndexOf(key);
                    if (start >= 0)
                    {
                        start += key.Length;
                        int end = json.IndexOf("\"", start);
                        if (end > start)
                        {
                            string translated = json.Substring(start, end - start);
                            return translated;
                        }
                    }

                    Debug.LogWarning("翻译失败: 无法从 JSON 中提取结果");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"翻译失败: {e.Message}");
            }

            return null;
        }


        static Dictionary<string, Dictionary<string, string>> buttonTranslations = new()
        {
            ["上一关(PgUp)"] = new() { ["English"] = "Prev(PgUp)" },
            ["下一关(PgDn)"] = new() { ["English"] = "Next(PgDn)" },
            ["传送至(C)"] = new() { ["English"] = "TP To(C)" },
            ["分身+1"] = new() { ["English"] = "Clone+1" },
            ["分身-1"] = new() { ["English"] = "Clone-1" },
            ["召集(F2)"] = new() { ["English"] = "Call(F2)" },
            ["平滑+1"] = new() { ["English"] = "Smooth+1" },
            ["平滑-1"] = new() { ["English"] = "Smooth-1" },
            ["悬浮于(X)"] = new() { ["English"] = "Float(X)" },
            ["牵手(Z)"] = new() { ["English"] = "Hold(Z)" },
            ["离开游戏"] = new() { ["English"] = "Exit" },
            ["设置>>"] = new() { ["English"] = "Settings" },
            ["载点(Ctrl+F)"] = new() { ["English"] = "LoadPt(Ctrl+F)" },
            ["返回大厅"] = new() { ["English"] = "Lobby" },
            ["邀请好友"] = new() { ["English"] = "Invite" },
            ["重新开始"] = new() { ["English"] = "Restart" },
            ["重置动画"] = new() { ["English"] = "ResetAnim" },
            ["重置物品(F3)"] = new() { ["English"] = "ResetItems(F3)" },
            ["全局灯"] = new() { ["English"] = "GlobalLight" },
            ["头灯"] = new() { ["English"] = "Headlight" },
            ["存档点显示"] = new() { ["English"] = "SaveVis" },
            ["找不同模式"] = new() { ["English"] = "DiffMode" },
            ["滑冰图"] = new() { ["English"] = "SkateMap" },
            ["真假剔除"] = new() { ["English"] = "FakeOff" },
            ["自动伸手"] = new() { ["English"] = "AutoReach" },
            ["自动海豚跳"] = new() { ["English"] = "AutoBhop" },
            ["自动爬墙"] = new() { ["English"] = "AutoClimb" },
            ["解密模式"] = new() { ["English"] = "Decrypt" },
            ["解锁成就"] = new() { ["English"] = "Unlock" },
            ["触发器显示"] = new() { ["English"] = "Triggers" },
            ["个人资料"] = new() { ["English"] = "Profile" },
            ["传送至>>"] = new() { ["English"] = "TP>>" },
            ["保存皮肤！慎点"] = new() { ["English"] = "SaveSkin!" },
            ["修复所有人皮肤"] = new() { ["English"] = "FixAllSkin" },
            ["修复皮肤"] = new() { ["English"] = "FixSkin" },
            ["切换皮肤"] = new() { ["English"] = "SwapSkin" },
            ["刷新皮肤"] = new() { ["English"] = "ReloadSkin" },
            ["加好友"] = new() { ["English"] = "AddFriend" },
            ["悬浮于>>"] = new() { ["English"] = "Float>>" },
            ["牵手>>"] = new() { ["English"] = "Hold>>" },
            ["踢出房间"] = new() { ["English"] = "Kick" },
            ["三级跳"] = new() { ["English"] = "TripleJump" },
            ["个人定点"] = new() { ["English"] = "MyPoint" },
            ["修改手长"] = new() { ["English"] = "HandLen" },
            ["修改速度"] = new() { ["English"] = "Speed" },
            ["倒立"] = new() { ["English"] = "Handstand" },
            ["冻结"] = new() { ["English"] = "Freeze" },
            ["半身不遂"] = new() { ["English"] = "HalfParalysis" },
            ["吊死鬼"] = new() { ["English"] = "HangGhost" },
            ["客机权限"] = new() { ["English"] = "GuestPerm" },
            ["手滑"] = new() { ["English"] = "Slippery" },
            ["拆除"] = new() { ["English"] = "Dismantle" },
            ["无碰撞"] = new() { ["English"] = "NoCollision" },
            ["气球"] = new() { ["English"] = "Balloon" },
            ["气球戏法"] = new() { ["English"] = "BalloonFX" },
            ["潜水"] = new() { ["English"] = "Dive" },
            ["物品挂件"] = new() { ["English"] = "ItemSlot" },
            ["电臀"] = new() { ["English"] = "ElectroButt" },
            ["磕头怪"] = new() { ["English"] = "HeadBanger" },
            ["空气炮"] = new() { ["English"] = "AirCannon" },
            ["聊天框权限"] = new() { ["English"] = "ChatPerm" },
            ["腿拐"] = new() { ["English"] = "LegTwist" },
            ["腿瘸"] = new() { ["English"] = "LegLimp" },
            ["螃蟹"] = new() { ["English"] = "Crab" },
            ["超人"] = new() { ["English"] = "Superman" },
            ["超级跳"] = new() { ["English"] = "SuperJump" },
            ["蹦迪"] = new() { ["English"] = "Dance" },
            ["转圈圈"] = new() { ["English"] = "Spin" },
            ["闪现"] = new() { ["English"] = "Blink" },
            ["陀螺"] = new() { ["English"] = "Gyro" },
            ["飞天"] = new() { ["English"] = "Fly" },
            ["降低贴图分辨率"] = new() { ["English"] = "LowTextureRes" },
            ["跳过贴图压缩"] = new() { ["English"] = "SkipTextureCompression" },
            ["传送系统"] = new() { ["English"] = "Teleport" },
            ["击飞系统"] = new() { ["English"] = "KnockUp" },
            ["娱乐系统"] = new() { ["English"] = "FunMode" },
            ["定点系统"] = new() { ["English"] = "Waypoint" },
            ["挂机提醒"] = new() { ["English"] = "AFKAlert" },
            ["无假死"] = new() { ["English"] = "NoFakeDeath" },
            ["无碰撞系统"] = new() { ["English"] = "NoCollision" },
            ["漂浮代码"] = new() { ["English"] = "FloatCode" },
            ["爬墙代码"] = new() { ["English"] = "ClimbCode" },
            ["物体挂件"] = new() { ["English"] = "ItemDeco" },
            ["玩家挂件"] = new() { ["English"] = "PlyDeco" },
            ["牵手系统"] = new() { ["English"] = "HandHold" },
            ["闪现系统"] = new() { ["English"] = "Blink" },
            ["飞天系统"] = new() { ["English"] = "Fly" },
            ["功能开关(K)"] = new() { ["English"] = "Switch(K)" },
            ["换图(H)"] = new() { ["English"] = "Map(H)" },
            ["物体控制(I)"] = new() { ["English"] = "Obj(I)" },
            ["玩家控制(P)"] = new() { ["English"] = "Plyr(P)" },
            ["菜单(Tab)"] = new() { ["English"] = "Menu(Tab)" },
            ["设置(O)"] = new() { ["English"] = "Options(O)" },
            ["Q定点"] = new() { ["English"] = "Qpoint" },
            ["SE定点"] = new() { ["English"] = "SEpoint" },
            ["一直显示名字"] = new() { ["English"] = "AlwaysShowName" },
            ["仅限邀请"] = new() { ["English"] = "InviteOnly" },
            ["保速"] = new() { ["English"] = "KeepSpeed" },
            ["修改贴图格式"] = new() { ["English"] = "ChangeTextureFormat" },
            ["关闭大厅下载"] = new() { ["English"] = "DisableLobbyDL" },
            ["去除没订阅地图文件加载失败"] = new() { ["English"] = "SkipMissingMapError" },
            ["去除游戏启动画面"] = new() { ["English"] = "SkipIntro" },
            ["发Q定点"] = new() { ["English"] = "SendQpoint" },
            ["发言设置>>"] = new() { ["English"] = "SpeechSettings>>" },
            ["发言限制"] = new() { ["English"] = "SpeechLimit" },
            ["名字设置>>"] = new() { ["English"] = "NameSettings>>" },
            ["回溯"] = new() { ["English"] = "Rewind" },
            ["回车聊天"] = new() { ["English"] = "EnterToChat" },
            ["好友房间(用 @ 标识)"] = new() { ["English"] = "FriendRoom(@)" },
            ["客机时禁止其他客机控制我"] = new() { ["English"] = "BlockGuestControl" },
            ["屏蔽颜色代码"] = new() { ["English"] = "BlockColorCodes" },
            ["惯性"] = new() { ["English"] = "Inertia" },
            ["房主时不受客机玩家控制"] = new() { ["English"] = "HostBlocksGuestCtrl" },
            ["房主时默认打开客机权限"] = new() { ["English"] = "HostGuestRightsOn" },
            ["房间列表单页显示"] = new() { ["English"] = "SinglePageRoomList" },
            ["房间列表按照当前玩家数量降序显示"] = new() { ["English"] = "SortRoomsByPlayers" },
            ["按Shift键显示鼠标"] = new() { ["English"] = "ShowMouse(Shift)" },
            ["斜体"] = new() { ["English"] = "Italic" },
            ["显示FPS"] = new() { ["English"] = "ShowFPS" },
            ["显示小地图"] = new() { ["English"] = "ShowMiniMap" },
            ["显示按键信息"] = new() { ["English"] = "ShowKeys" },
            ["显示目标距离"] = new() { ["English"] = "ShowTargetDist" },
            ["显示私密房间(用 * 标识)"] = new() { ["English"] = "ShowPrivateRoom(*)" },
            ["显示鼠标时人物不可控"] = new() { ["English"] = "LockCharWMouse" },
            ["清除Bug玩家"] = new() { ["English"] = "ClearBugPlayer" },
            ["游戏中禁止加入"] = new() { ["English"] = "NoJoinInGame" },
            ["粗体"] = new() { ["English"] = "Bold" },
            ["自定义代码"] = new() { ["English"] = "CustomCode" },
            ["菜单颜色>>"] = new() { ["English"] = "MenuColors>>" },
            ["锁定关卡"] = new() { ["English"] = "LockLevel" },
            ["锁定关卡"] = new() { ["English"] = "LockLevel" },
            ["修改名字"] = new() { ["English"] = "EditName" },
            ["大厅名称"] = new() { ["English"] = "LobbyName" },
            ["定点提示"] = new() { ["English"] = "WaypointTip" },
            ["屏蔽固定词"] = new() { ["English"] = "BlockWords" },
            ["房间名称"] = new() { ["English"] = "RoomName" },
            ["F8视距"] = new() { ["English"] = "F8ViewDist" },
            ["发言字数限制(个)"] = new() { ["English"] = "ChatCharLimit" },
            ["发言渐变"] = new() { ["English"] = "ChatGradient" },
            ["发言重复次数限制(条)"] = new() { ["English"] = "ChatRepeatLimit" },
            ["发言间隔限制(秒)"] = new() { ["English"] = "ChatInterval" },
            ["名字渐变"] = new() { ["English"] = "NameGradient" },
            ["字号"] = new() { ["English"] = "FontSize" },
            ["字号1"] = new() { ["English"] = "FontSize1" },
            ["字号2"] = new() { ["English"] = "FontSize2" },
            ["存点数量"] = new() { ["English"] = "SaveCount" },
            ["定点高度"] = new() { ["English"] = "PointHeight" },
            ["开始"] = new() { ["English"] = "Start" },
            ["无动作进入挂机(分钟)"] = new() { ["English"] = "IdleAFKMin" },
            ["最低亮度"] = new() { ["English"] = "MinBrightness" },
            ["最大"] = new() { ["English"] = "Max" },
            ["最小"] = new() { ["English"] = "Min" },
            ["玩家上限"] = new() { ["English"] = "PlayerLimit" },
            ["玩家视距"] = new() { ["English"] = "PlayerViewDist" },
            ["结束"] = new() { ["English"] = "End" },
            ["菜单渐变"] = new() { ["English"] = "MenuGradient" },
            ["虚假人数"] = new() { ["English"] = "FakePlayer" },
            ["体重"] = new() { ["English"] = "Weight" },
            ["修改重力"] = new() { ["English"] = "ChangeGravity" },
            ["倍速"] = new() { ["English"] = "SpeedMult" },
            ["手部力量"] = new() { ["English"] = "HandPow" },
            ["普通手长"] = new() { ["English"] = "HandLen" },
            ["伸手手长"] = new() { ["English"] = "HandExt" },
            ["跳跃间距"] = new() { ["English"] = "JumpDist" },
            ["阻力"] = new() { ["English"] = "Drag" },
            ["修改快捷键"] = new() { ["English"] = "EditHotkey" },
            ["重新读取快捷键"] = new() { ["English"] = "ReloadHotkey" },
            ["取消牵手"] = new() { ["English"] = "CancelHold" },
            ["还原"] = new() { ["English"] = "Restore" },
            ["取消悬浮"] = new() { ["English"] = "CancelFloat" },
            ["跌落"] = new() { ["English"] = "Fall" },
            ["睡觉"] = new() { ["English"] = "Sleep" },
            ["气球"] = new() { ["English"] = "Balloon" },
            ["坐下"] = new() { ["English"] = "Sit" },
            ["挂件"] = new() { ["English"] = "Pendant" },
            ["定点设置"] = new() { ["English"] = "WaypointSet" },
            ["开房设置"] = new() { ["English"] = "RoomSet" },
            ["聊天设置"] = new() { ["English"] = "ChatSet" },
            ["快捷键设置"] = new() { ["English"] = "HotkeySet" },
            ["UI显示设置"] = new() { ["English"] = "UISet" },
            ["游戏设置"] = new() { ["English"] = "GameSet" },
            ["YxMod设置"] = new() { ["English"] = "YxModSet" },
            ["固定"] = new() { ["English"] = "Fix" },
            ["渐变"] = new() { ["English"] = "Grad" },
            ["跳跃"] = new() { ["English"] = "Jump" },
            ["随机"] = new() { ["English"] = "Rand" },
            ["皮肤管理>>"] = new() { ["English"] = "Skins>>" },
            ["客户端属性>>"] = new() { ["English"] = "Client>>" },
            ["所有人属性>>"] = new() { ["English"] = "All>>" },
            ["服务端属性>>"] = new() { ["English"] = "Server>>" },
            ["颜色设置"] = new() { ["English"] = "Color" },
            ["大小设置"] = new() { ["English"] = "Size" },
            ["重置"] = new() { ["English"] = "Reset" },
            ["全员控制"] = new() { ["English"] = "AllPlayers" },
            ["分身时分屏显示"] = new() { ["English"] = "splitScreenEnabled" },
            ["自定义Y"] = new() { ["English"] = "CustomY" },
            ["悬浮列队"] = new() { ["English"] = "HoverLine" },
        };
        private static readonly string translationCachePath = Path.Combine(Application.persistentDataPath, "button_translations.bin");

        private static void SaveCacheToFile()
        {
            try
            {
                using (FileStream fs = new FileStream(translationCachePath, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, buttonTranslations);
                }
                Debug.Log("翻译缓存二进制保存成功");
            }
            catch (Exception e)
            {
                Debug.LogWarning("保存失败: " + e.Message);
            }
        }

        private static void LoadCacheFromFile()
        {
            try
            {
                if (!File.Exists(translationCachePath))
                    return;

                using (FileStream fs = new FileStream(translationCachePath, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    buttonTranslations = (Dictionary<string, Dictionary<string, string>>)bf.Deserialize(fs);
                }
                Debug.Log("翻译缓存二进制加载成功");
            }
            catch (Exception e)
            {
                Debug.LogWarning("加载失败: " + e.Message);
                buttonTranslations = new Dictionary<string, Dictionary<string, string>>();
            }
        }
    }
}
