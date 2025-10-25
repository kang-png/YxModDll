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
                fontSize = 16,
                wordWrap = true
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
                    name = TranslateButtonText(name)+"：";
                    GUILayout.Label(GetColoredName(name));
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
            if (yuan.HasValue && GUILayout.Button(GetColoredName(TranslateButtonText("重置")), UI.styleButton()))
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
            ["上一关(PgUp)"] = new() { ["English"] = "Prev(PgUp)", ["Korean"] = "이전(PgUp)" },
            ["下一关(PgDn)"] = new() { ["English"] = "Next(PgDn)", ["Korean"] = "다음(PgDn)" },
            ["传送至(C)"] = new() { ["English"] = "TP To(C)", ["Korean"] = "순간이동(C)" },
            ["分身+1"] = new() { ["English"] = "Clone+1", ["Korean"] = "분신+1" },
            ["分身-1"] = new() { ["English"] = "Clone-1", ["Korean"] = "분신-1" },
            ["召集(F2)"] = new() { ["English"] = "Call(F2)", ["Korean"] = "소집(F2)" },
            ["平滑+1"] = new() { ["English"] = "Smooth+1", ["Korean"] = "평활+1" },
            ["平滑-1"] = new() { ["English"] = "Smooth-1", ["Korean"] = "평활-1" },
            ["悬浮于(X)"] = new() { ["English"] = "Float(X)", ["Korean"] = "공중부양(X)" },
            ["牵手(Z)"] = new() { ["English"] = "Hold(Z)", ["Korean"] = "손잡기(Z)" },
            ["离开游戏"] = new() { ["English"] = "Exit", ["Korean"] = "나가기" },
            ["设置>>"] = new() { ["English"] = "Settings", ["Korean"] = "설정>>" },
            ["载点(Ctrl+F)"] = new() { ["English"] = "LoadPt(Ctrl+F)", ["Korean"] = "로드점(Ctrl+F)" },
            ["返回大厅"] = new() { ["English"] = "Lobby", ["Korean"] = "로비" },
            ["邀请好友"] = new() { ["English"] = "Invite", ["Korean"] = "친구초대" },
            ["重新开始"] = new() { ["English"] = "Restart", ["Korean"] = "재시작" },
            ["重置动画"] = new() { ["English"] = "ResetAnim", ["Korean"] = "애니리셋" },
            ["重置物品(F3)"] = new() { ["English"] = "ResetItems(F3)", ["Korean"] = "아이템리셋(F3)" },
            ["全局灯"] = new() { ["English"] = "GlobalLight", ["Korean"] = "전체조명" },
            ["头灯"] = new() { ["English"] = "Headlight", ["Korean"] = "헤드등" },
            ["存档点显示"] = new() { ["English"] = "SaveVis", ["Korean"] = "세이브표시" },
            ["找不同模式"] = new() { ["English"] = "DiffMode", ["Korean"] = "차이찾기" },
            ["滑冰图"] = new() { ["English"] = "SkateMap", ["Korean"] = "스케이트맵" },
            ["真假剔除"] = new() { ["English"] = "FakeOff", ["Korean"] = "가짜제거" },
            ["自动伸手"] = new() { ["English"] = "AutoReach", ["Korean"] = "자동손뻗기" },
            ["自动海豚跳"] = new() { ["English"] = "AutoBhop", ["Korean"] = "자동버니합" },
            ["自动爬墙"] = new() { ["English"] = "AutoClimb", ["Korean"] = "자동벽타기" },
            ["解密模式"] = new() { ["English"] = "Decrypt", ["Korean"] = "암호해제" },
            ["解锁成就"] = new() { ["English"] = "Unlock", ["Korean"] = "도전과제해제" },
            ["触发器显示"] = new() { ["English"] = "Triggers", ["Korean"] = "트리거표시" },
            ["个人资料"] = new() { ["English"] = "Profile", ["Korean"] = "프로필" },
            ["传送至>>"] = new() { ["English"] = "TP>>", ["Korean"] = "순간이동>>" },
            ["保存皮肤！慎点"] = new() { ["English"] = "SaveSkin!", ["Korean"] = "스킨저장!주의" },
            ["修复所有人皮肤"] = new() { ["English"] = "FixAllSkin", ["Korean"] = "전체스킨복구" },
            ["修复皮肤"] = new() { ["English"] = "FixSkin", ["Korean"] = "스킨복구" },
            ["切换皮肤"] = new() { ["English"] = "SwapSkin", ["Korean"] = "스킨변경" },
            ["刷新皮肤"] = new() { ["English"] = "ReloadSkin", ["Korean"] = "스킨재로드" },
            ["加好友"] = new() { ["English"] = "AddFriend", ["Korean"] = "친구추가" },
            ["悬浮于>>"] = new() { ["English"] = "Float>>", ["Korean"] = "공중부양>>" },
            ["牵手>>"] = new() { ["English"] = "Hold>>", ["Korean"] = "손잡기>>" },
            ["踢出房间"] = new() { ["English"] = "Kick", ["Korean"] = "강퇴" },
            ["三级跳"] = new() { ["English"] = "TripleJump", ["Korean"] = "삼단점프" },
            ["个人定点"] = new() { ["English"] = "MyPoint", ["Korean"] = "개인지점" },
            ["修改手长"] = new() { ["English"] = "HandLen", ["Korean"] = "팔길이" },
            ["修改速度"] = new() { ["English"] = "Speed", ["Korean"] = "속도" },
            ["倒立"] = new() { ["English"] = "Handstand", ["Korean"] = "손서기" },
            ["冻结"] = new() { ["English"] = "Freeze", ["Korean"] = "프리즈" },
            ["半身不遂"] = new() { ["English"] = "HalfParalysis", ["Korean"] = "반신마비" },
            ["吊死鬼"] = new() { ["English"] = "HangGhost", ["Korean"] = "교수형" },
            ["客机权限"] = new() { ["English"] = "GuestPerm", ["Korean"] = "게스트권한" },
            ["手滑"] = new() { ["English"] = "Slippery", ["Korean"] = "미끄러움" },
            ["拆除"] = new() { ["English"] = "Dismantle", ["Korean"] = "해체" },
            ["无碰撞"] = new() { ["English"] = "NoCollision", ["Korean"] = "충돌무시" },
            ["气球"] = new() { ["English"] = "Balloon", ["Korean"] = "풍선" },
            ["气球戏法"] = new() { ["English"] = "BalloonFX", ["Korean"] = "풍선효과" },
            ["潜水"] = new() { ["English"] = "Dive", ["Korean"] = "다이빙" },
            ["物品挂件"] = new() { ["English"] = "ItemSlot", ["Korean"] = "아이템장식" },
            ["电臀"] = new() { ["English"] = "ElectroButt", ["Korean"] = "전기엉덩이" },
            ["磕头怪"] = new() { ["English"] = "HeadBanger", ["Korean"] = "머리박기" },
            ["空气炮"] = new() { ["English"] = "AirCannon", ["Korean"] = "에어캐논" },
            ["聊天框权限"] = new() { ["English"] = "ChatPerm", ["Korean"] = "채팅권한" },
            ["腿拐"] = new() { ["English"] = "LegTwist", ["Korean"] = "다리꼬기" },
            ["腿瘸"] = new() { ["English"] = "LegLimp", ["Korean"] = "절름발이" },
            ["螃蟹"] = new() { ["English"] = "Crab", ["Korean"] = "게걸음" },
            ["超人"] = new() { ["English"] = "Superman", ["Korean"] = "슈퍼맨" },
            ["超级跳"] = new() { ["English"] = "SuperJump", ["Korean"] = "슈퍼점프" },
            ["蹦迪"] = new() { ["English"] = "Dance", ["Korean"] = "댄스" },
            ["转圈圈"] = new() { ["English"] = "Spin", ["Korean"] = "회전" },
            ["闪现"] = new() { ["English"] = "Blink", ["Korean"] = "점멸" },
            ["陀螺"] = new() { ["English"] = "Gyro", ["Korean"] = "자이로" },
            ["飞天"] = new() { ["English"] = "Fly", ["Korean"] = "비행" },
            ["降低贴图分辨率"] = new() { ["English"] = "LowTextureRes", ["Korean"] = "텍스쳐해상도↓" },
            ["跳过贴图压缩"] = new() { ["English"] = "SkipTextureCompression", ["Korean"] = "텍스쳐압축생략" },
            ["传送系统"] = new() { ["English"] = "Teleport", ["Korean"] = "순간이동시스템" },
            ["击飞系统"] = new() { ["English"] = "KnockUp", ["Korean"] = "녹업시스템" },
            ["娱乐系统"] = new() { ["English"] = "FunMode", ["Korean"] = "재미시스템" },
            ["定点系统"] = new() { ["English"] = "Waypoint", ["Korean"] = "웨이포인트" },
            ["挂机提醒"] = new() { ["English"] = "AFKAlert", ["Korean"] = "자리비움알림" },
            ["无假死"] = new() { ["English"] = "NoFakeDeath", ["Korean"] = "가짜사망무시" },
            ["无碰撞系统"] = new() { ["English"] = "NoCollision", ["Korean"] = "충돌무시시스템" },
            ["漂浮代码"] = new() { ["English"] = "FloatCode", ["Korean"] = "부유코드" },
            ["爬墙代码"] = new() { ["English"] = "ClimbCode", ["Korean"] = "벽타기코드" },
            ["物体挂件"] = new() { ["English"] = "ItemDeco", ["Korean"] = "오브젝트장식" },
            ["玩家挂件"] = new() { ["English"] = "PlyDeco", ["Korean"] = "플레이어장식" },
            ["牵手系统"] = new() { ["English"] = "HandHold", ["Korean"] = "손잡기시스템" },
            ["闪现系统"] = new() { ["English"] = "Blink", ["Korean"] = "점멸시스템" },
            ["飞天系统"] = new() { ["English"] = "Fly", ["Korean"] = "비행시스템" },
            ["功能开关(K)"] = new() { ["English"] = "Switch(K)", ["Korean"] = "기능(K)" },
            ["换图(H)"] = new() { ["English"] = "Map(H)", ["Korean"] = "맵(H)" },
            ["物体控制(I)"] = new() { ["English"] = "Obj(I)", ["Korean"] = "물체(I)" },
            ["玩家控制(P)"] = new() { ["English"] = "Plyr(P)", ["Korean"] = "플레이어(P)" },
            ["菜单(Tab)"] = new() { ["English"] = "Menu(Tab)", ["Korean"] = "메뉴(Tab)" },
            ["设置(O)"] = new() { ["English"] = "Options(O)", ["Korean"] = "옵션(O)" },
            ["Q定点"] = new() { ["English"] = "Qpoint", ["Korean"] = "Q지점" },
            ["SE定点"] = new() { ["English"] = "SEpoint", ["Korean"] = "SE지점" },
            ["一直显示名字"] = new() { ["English"] = "AlwaysShowName", ["Korean"] = "이름항상표시" },
            ["仅限邀请"] = new() { ["English"] = "InviteOnly", ["Korean"] = "초대전용" },
            ["保速"] = new() { ["English"] = "KeepSpeed", ["Korean"] = "속도유지" },
            ["修改贴图格式"] = new() { ["English"] = "ChangeTextureFormat", ["Korean"] = "텍스쳐형식변경" },
            ["关闭大厅下载"] = new() { ["English"] = "DisableLobbyDL", ["Korean"] = "로비다운로드끄기" },
            ["去除没订阅地图文件加载失败"] = new() { ["English"] = "SkipMissingMapError", ["Korean"] = "맵로드에러생략" },
            ["去除游戏启动画面"] = new() { ["English"] = "SkipIntro", ["Korean"] = "인트로생략" },
            ["发Q定点"] = new() { ["English"] = "SendQpoint", ["Korean"] = "Q지점전송" },
            ["发言设置>>"] = new() { ["English"] = "SpeechSettings>>", ["Korean"] = "채팅설정>>" },
            ["发言限制"] = new() { ["English"] = "SpeechLimit", ["Korean"] = "채팅제한" },
            ["名字设置>>"] = new() { ["English"] = "NameSettings>>", ["Korean"] = "이름설정>>" },
            ["回溯"] = new() { ["English"] = "Rewind", ["Korean"] = "되감기" },
            ["回车聊天"] = new() { ["English"] = "EnterToChat", ["Korean"] = "엔터채팅" },
            ["好友房间(用 @ 标识)"] = new() { ["English"] = "FriendRoom(@)", ["Korean"] = "친구방(@)" },
            ["客机时禁止其他客机控制我"] = new() { ["English"] = "BlockGuestControl", ["Korean"] = "게스트제어차단" },
            ["屏蔽颜色代码"] = new() { ["English"] = "BlockColorCodes", ["Korean"] = "색상코드차단" },
            ["惯性"] = new() { ["English"] = "Inertia", ["Korean"] = "관성" },
            ["房主时不受客机玩家控制"] = new() { ["English"] = "HostBlocksGuestCtrl", ["Korean"] = "호스트제어차단" },
            ["房主时默认打开客机权限"] = new() { ["English"] = "HostGuestRightsOn", ["Korean"] = "게스트권한기본on" },
            ["房间列表单页显示"] = new() { ["English"] = "SinglePageRoomList", ["Korean"] = "방목록단일페이지" },
            ["房间列表按照当前玩家数量降序显示"] = new() { ["English"] = "SortRoomsByPlayers", ["Korean"] = "방목록인원순정렬" },
            ["按Shift键显示鼠标"] = new() { ["English"] = "ShowMouse(Shift)", ["Korean"] = "마우스표시(Shift)" },
            ["斜体"] = new() { ["English"] = "Italic", ["Korean"] = "기울임" },
            ["显示FPS"] = new() { ["English"] = "ShowFPS", ["Korean"] = "FPS표시" },
            ["显示小地图"] = new() { ["English"] = "ShowMiniMap", ["Korean"] = "미니맵표시" },
            ["显示按键信息"] = new() { ["English"] = "ShowKeys", ["Korean"] = "키표시" },
            ["显示目标距离"] = new() { ["English"] = "ShowTargetDist", ["Korean"] = "목표거리표시" },
            ["显示私密房间(用 * 标识)"] = new() { ["English"] = "ShowPrivateRoom(*)", ["Korean"] = "비밀방표시(*)" },
            ["显示鼠标时人物不可控"] = new() { ["English"] = "LockCharWMouse", ["Korean"] = "마우스시캐릭잠금" },
            ["清除Bug玩家"] = new() { ["English"] = "ClearBugPlayer", ["Korean"] = "버그플레이어제거" },
            ["游戏中禁止加入"] = new() { ["English"] = "NoJoinInGame", ["Korean"] = "게임중참여금지" },
            ["粗体"] = new() { ["English"] = "Bold", ["Korean"] = "굵게" },
            ["自定义代码"] = new() { ["English"] = "CustomCode", ["Korean"] = "커스텀코드" },
            ["菜单颜色>>"] = new() { ["English"] = "MenuColors>>", ["Korean"] = "메뉴색상>>" },
            ["锁定关卡"] = new() { ["English"] = "LockLevel", ["Korean"] = "레벨잠금" },

            ["修改名字"] = new() { ["English"] = "EditName", ["Korean"] = "이름변경" },
            ["大厅名称"] = new() { ["English"] = "LobbyName", ["Korean"] = "로비이름" },
            ["定点提示"] = new() { ["English"] = "WaypointTip", ["Korean"] = "지점팁" },
            ["屏蔽固定词"] = new() { ["English"] = "BlockWords", ["Korean"] = "단어차단" },
            ["房间名称"] = new() { ["English"] = "RoomName", ["Korean"] = "방이름" },
            ["F8视距"] = new() { ["English"] = "F8ViewDist", ["Korean"] = "F8시야거리" },
            ["发言字数限制(个)"] = new() { ["English"] = "ChatCharLimit", ["Korean"] = "채팅글자수제한" },
            ["发言渐变"] = new() { ["English"] = "ChatGradient", ["Korean"] = "채팅색" },
            ["发言重复次数限制(条)"] = new() { ["English"] = "ChatRepeatLimit", ["Korean"] = "채팅반복제한" },
            ["发言间隔限制(秒)"] = new() { ["English"] = "ChatInterval", ["Korean"] = "채팅간격제한" },
            ["名字渐变"] = new() { ["English"] = "NameGradient", ["Korean"] = "이름색" },
            ["字号"] = new() { ["English"] = "FontSize", ["Korean"] = "글꼴크기" },
            ["字号1"] = new() { ["English"] = "FontSize1", ["Korean"] = "글꼴크기1" },
            ["字号2"] = new() { ["English"] = "FontSize2", ["Korean"] = "글꼴크기2" },
            ["存点数量"] = new() { ["English"] = "SaveCount", ["Korean"] = "세이브개수" },
            ["定点高度"] = new() { ["English"] = "PointHeight", ["Korean"] = "지점높이" },
            ["开始"] = new() { ["English"] = "Start", ["Korean"] = "시작" },
            ["无动作进入挂机(分钟)"] = new() { ["English"] = "IdleAFKMin", ["Korean"] = "자리비움분" },
            ["最低亮度"] = new() { ["English"] = "MinBrightness", ["Korean"] = "최소밝기" },
            ["最大"] = new() { ["English"] = "Max", ["Korean"] = "최대" },
            ["最小"] = new() { ["English"] = "Min", ["Korean"] = "최소" },
            ["玩家上限"] = new() { ["English"] = "PlayerLimit", ["Korean"] = "플레이어제한" },
            ["玩家视距"] = new() { ["English"] = "PlayerViewDist", ["Korean"] = "플레이어시야" },
            ["结束"] = new() { ["English"] = "End", ["Korean"] = "종료" },
            ["菜单渐变"] = new() { ["English"] = "MenuGradient", ["Korean"] = "메뉴그라데이션" },
            ["虚假人数"] = new() { ["English"] = "FakePlayer", ["Korean"] = "가짜인원" },
            ["体重"] = new() { ["English"] = "Weight", ["Korean"] = "체중" },
            ["修改重力"] = new() { ["English"] = "ChangeGravity", ["Korean"] = "중력변경" },
            ["倍速"] = new() { ["English"] = "SpeedMult", ["Korean"] = "속도배율" },
            ["手部力量"] = new() { ["English"] = "HandPow", ["Korean"] = "손힘" },
            ["普通手长"] = new() { ["English"] = "HandLen", ["Korean"] = "일반팔길이" },
            ["伸手手长"] = new() { ["English"] = "HandExt", ["Korean"] = "뻗은팔길이" },
            ["跳跃间距"] = new() { ["English"] = "JumpDist", ["Korean"] = "점프간격" },
            ["阻力"] = new() { ["English"] = "Drag", ["Korean"] = "저항" },
            ["修改快捷键"] = new() { ["English"] = "EditHotkey", ["Korean"] = "단축키변경" },
            ["重新读取快捷键"] = new() { ["English"] = "ReloadHotkey", ["Korean"] = "단축키재로드" },
            ["取消牵手"] = new() { ["English"] = "CancelHold", ["Korean"] = "손잡기취소" },
            ["还原"] = new() { ["English"] = "Restore", ["Korean"] = "복원" },
            ["取消悬浮"] = new() { ["English"] = "CancelFloat", ["Korean"] = "공중부양취소" },
            ["跌落"] = new() { ["English"] = "Fall", ["Korean"] = "낙하" },
            ["睡觉"] = new() { ["English"] = "Sleep", ["Korean"] = "수면" },

            ["坐下"] = new() { ["English"] = "Sit", ["Korean"] = "앉기" },
            ["挂件"] = new() { ["English"] = "Pendant", ["Korean"] = "펜던트" },
            ["定点设置"] = new() { ["English"] = "WaypointSet", ["Korean"] = "지점설정" },
            ["开房设置"] = new() { ["English"] = "RoomSet", ["Korean"] = "방설정" },
            ["聊天设置"] = new() { ["English"] = "ChatSet", ["Korean"] = "채팅설정" },
            ["快捷键设置"] = new() { ["English"] = "HotkeySet", ["Korean"] = "단축키설정" },
            ["UI显示设置"] = new() { ["English"] = "UISet", ["Korean"] = "UI설정" },
            ["游戏设置"] = new() { ["English"] = "GameSet", ["Korean"] = "게임설정" },
            ["YxMod设置"] = new() { ["English"] = "YxModSet", ["Korean"] = "YxMod설정" },
            ["固定"] = new() { ["English"] = "Fix", ["Korean"] = "고정" },
            ["渐变"] = new() { ["English"] = "Grad", ["Korean"] = "그라데" },
            ["跳跃"] = new() { ["English"] = "Jump", ["Korean"] = "점프" },
            ["随机"] = new() { ["English"] = "Rand", ["Korean"] = "무작위" },
            ["皮肤管理>>"] = new() { ["English"] = "Skins>>", ["Korean"] = "스킨관리>>" },
            ["客户端属性>>"] = new() { ["English"] = "Client>>", ["Korean"] = "클라이언트>>" },
            ["所有人属性>>"] = new() { ["English"] = "All>>", ["Korean"] = "모든사용자>>" },
            ["服务端属性>>"] = new() { ["English"] = "Server>>", ["Korean"] = "서버>>" },
            ["颜色设置"] = new() { ["English"] = "Color", ["Korean"] = "색상" },
            ["大小设置"] = new() { ["English"] = "Size", ["Korean"] = "크기" },
            ["重置"] = new() { ["English"] = "Reset", ["Korean"] = "리셋" },
            ["全员控制"] = new() { ["English"] = "AllPlayers", ["Korean"] = "전원제어" },
            ["分身时分屏显示"] = new() { ["English"] = "splitScreenEnabled", ["Korean"] = "분신시분할화면" },
            ["自定义Y"] = new() { ["English"] = "CustomY", ["Korean"] = "커스텀Y" },
            ["悬浮列队"] = new() { ["English"] = "HoverLine", ["Korean"] = "공중부양대열" },
            ["身体变形>>"] = new() { ["English"] = "Body Deformation>>", ["Korean"] = "신체 변형>>" },
            ["大头萌"] = new() { ["English"] = "Big Head", ["Korean"] = "큰머리" },
            ["脑袋尖尖"] = new() { ["English"] = "Pointy", ["Korean"] = "뾰족" },
            ["小头高达"] = new() { ["English"] = "Gundam", ["Korean"] = "건담" },
            ["头部缩放"] = new() { ["English"] = "Head", ["Korean"] = "머리" },
            ["躯干缩放"] = new() { ["English"] = "Torso", ["Korean"] = "몸통" },
            ["左手缩放"] = new() { ["English"] = "L Arm", ["Korean"] = "왼팔" },
            ["右手缩放"] = new() { ["English"] = "R Arm", ["Korean"] = "오른팔" },
            ["左腿缩放"] = new() { ["English"] = "L Leg", ["Korean"] = "왼다리" },
            ["右腿缩放"] = new() { ["English"] = "R Leg", ["Korean"] = "오른다리" },
            ["球缩放"] = new() { ["English"] = "Ball", ["Korean"] = "공" },
            // 添加快捷键翻译
            ["存点"] = new() { ["English"] = "Save Point", ["Korean"] = "저장점" },
            ["载入自己"] = new() { ["English"] = "Load Self", ["Korean"] = "자기로드" },
            ["召集玩家"] = new() { ["English"] = "Call Players", ["Korean"] = "플레이어소집" },
            ["重置物品"] = new() { ["English"] = "Reset Items", ["Korean"] = "아이템리셋" },
            ["上一关"] = new() { ["English"] = "Prev Level", ["Korean"] = "이전관" },
            ["下一关"] = new() { ["English"] = "Next Level", ["Korean"] = "다음관" },
            ["创建房间"] = new() { ["English"] = "Create Room", ["Korean"] = "방생성" },
            ["全员飞天"] = new() { ["English"] = "All Fly", ["Korean"] = "전원비행" },
            ["全员超人"] = new() { ["English"] = "All Superman", ["Korean"] = "전원슈퍼맨" },
            ["全员闪现"] = new() { ["English"] = "All Blink", ["Korean"] = "전원점멸" },
            ["拳击姿势"] = new() { ["English"] = "Boxing Pose", ["Korean"] = "복싱포즈" },
            ["控制分身"] = new() { ["English"] = "Control Clone", ["Korean"] = "분신제어" },
            ["切换分身"] = new() { ["English"] = "Switch Clone", ["Korean"] = "분신전환" },
            ["指定飞"] = new() { ["English"] = "Target Fly", ["Korean"] = "지정비행" },
            ["指定飞天"] = new() { ["English"] = "Target Fly", ["Korean"] = "지정비행" },
            ["指定超人"] = new() { ["English"] = "Target Superman", ["Korean"] = "지정슈퍼맨" },
            ["指定闪现"] = new() { ["English"] = "Target Blink", ["Korean"] = "지정점멸" },
            ["传送至"] = new() { ["English"] = "TP To", ["Korean"] = "순간이동" },
            ["悬浮于"] = new() { ["English"] = "Float On", ["Korean"] = "공중부양" },
            ["牵手"] = new() { ["English"] = "Hold Hand", ["Korean"] = "손잡기" },
            ["背人"] = new() { ["English"] = "Carry", ["Korean"] = "업기" },
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
