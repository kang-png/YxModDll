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

        public static void UI_ChuShiHua()
        {
            anniuTexture.SetPixel(0, 0, new Color32(130, 130, 130, 255));//按钮未按下背景色
            anniuTexture.Apply();

            anniuTexture2.SetPixel(0, 0, new Color32(0, 0xbe, 0x92, 255));//按钮按下
            anniuTexture2.Apply();

            anniuTexture3.SetPixel(0, 0, new Color32(0, 0x70, 0xd7, 255));//按钮按下
            anniuTexture3.Apply();

            LoadCacheFromFile(); // 加载翻译缓存
            //Debug.Log("buttonHeight : " + buttonHeight);
        }
        public static GUIStyle styleSelectionGrid()  ///按下松开效果
        {
            GUIStyle styleButton = new GUIStyle(GUI.skin.button);
            styleButton.normal.background = anniuTexture; // 没选中的
            styleButton.onNormal.background = anniuTexture2;//选中的
            styleButton.active.background = anniuTexture2; //鼠标按下的
            styleButton.hover.background = styleButton.onNormal.background;//鼠标移动的
            styleButton.onHover.background = styleButton.onNormal.background;//选中的  鼠标移动的
            styleButton.normal.textColor = new Color32(220, 220, 220, 255);// Color.white;
            styleButton.hover.textColor = Color.white;
            styleButton.alignment = TextAnchor.MiddleLeft;// MiddleCenter;
            styleButton.fontSize = 20;
            styleButton.richText = true; // ✅ 开启富文本支持（必须）
            return styleButton;
        }
        public static int CreatHumanList(int index,bool quanyuankongzhi=true)
        {
            try
            {
                //清空//
                //List<string> tempList = new List<string>(humanNames);
                //tempList.Clear();
                //humanNames = tempList.ToArray();
                humanNames = new string[0];
                //清空//
                if (quanyuankongzhi)
                {
                    string quanyuan = "全员控制";
                    AddHumanListButton(ColorfulSpeek.colorshows(quanyuan));
                }

                string fzname = $"1.{NetGame.instance.server.name}";
                //if (NetGame.isServer || (NetGame.isClient && YxMod.YxModServer))
                //{
                //    fzname = $"★{fzname}";
                //}

                AddHumanListButton(ColorfulSpeek.colorshows(fzname));
            
                //if(NetGame.isServer)
                //{
                //    ZiJiId = 1;
                //}
                List<NetHost> hosts = NetGame.instance.readyclients;//客户机名


                for (int j = 0; j < hosts.Count; j++)
                {
                    string name = $"{j+2}.{hosts[j].name}";
                    //if ((NetGame.isServer && players[j].players[0].human.isClient) || (NetGame.isClient && players[j].hostId == NetGame.instance.local.hostId))
                    //if ((NetGame.isServer && players[j].players[0].human.isClient) || (NetGame.isClient && players[j].players[0].human.isClient))
                    //{
                    //    name = $"☆{name}";
                    //}
                    AddHumanListButton(ColorfulSpeek.colorshows(name));
                    //if(NetGame.isClient && hosts[j].hostId == NetGame.instance.local.hostId)
                    //{
                    //    ZiJiId = j;
                    //}

                }

                index = GUILayout.SelectionGrid(index, humanNames, 1, styleSelectionGrid());
                //Debug.Log("当前选中：" + index + "   客户机总数：" + hosts.Count);
                index = (index > (quanyuankongzhi? hosts.Count + 1:hosts.Count)) ? 0 : index;
                return index;
            }
            catch
            {
                Debug.Log("选中出错了");
                return 0;
            }
        }

        private static void AddHumanListButton(string name)
        {
            // 创建一个新的字符串数组，长度比原来多一个
            List<string> tempList = new List<string>(humanNames);
            tempList.Add(name); // 向列表中添加新的按钮名称
            humanNames = tempList.ToArray(); // 将列表转换回数组

            // 更新SelectionGrid以反映变化
            UpdateSelectionGrid();
        }
        private static void UpdateSelectionGrid()
        {
            // 强制Unity重新绘制GUI，确保SelectionGrid更新
            GUI.changed = true;
            // 注意：在某些情况下，可能需要使用Layout.Repaint()来强制更新GUI
            // UnityEngine.Experimental.UIElements.Layout.Repaint();
        }
        private static GUIStyle styleButton_Tab(bool tab)  ///按下松开效果
        {
            GUIStyle styleButton = new GUIStyle(GUI.skin.button);
            styleButton.normal.background = !tab ? anniuTexture : anniuTexture2; // 按钮的背景纹理
            styleButton.active.background = anniuTexture2; 
            styleButton.hover.background = tab ? anniuTexture : anniuTexture2;
            styleButton.normal.textColor = new Color32(220, 220, 220, 255);// Color.white;
            styleButton.hover.textColor = Color.white;
            styleButton.alignment = TextAnchor.MiddleCenter;
            styleButton.fontSize = 20;
            return styleButton;
        }

        public static GUIStyle styleButton() /// 滑动效果
        {
            GUIStyle styleButton = new GUIStyle(GUI.skin.button);
            styleButton.normal.background = anniuTexture; // 按钮的背景纹理
            styleButton.active.background = anniuTexture2; // 假设按下时的纹理
            styleButton.hover.background = styleButton.active.background;
            styleButton.normal.textColor = Color.white;
            styleButton.hover.textColor = Color.black;
            styleButton.alignment = TextAnchor.MiddleCenter;
            styleButton.fontSize = 20;

            return styleButton;
        }
        public static GUIStyle styleButton_Left() /// 滑动效果,字体居左
        {
            GUIStyle styleButton = new GUIStyle(GUI.skin.button);
            styleButton.normal.background = anniuTexture; // 按钮的背景纹理
            styleButton.active.background = anniuTexture2; // 假设按下时的纹理
            styleButton.hover.background = styleButton.active.background;
            styleButton.normal.textColor = Color.white;
            styleButton.hover.textColor = Color.black;
            styleButton.alignment = TextAnchor.MiddleLeft;
            styleButton.fontSize = 20;

            return styleButton;
        }
        public static float GetButtonHeight(GUIStyle style, string sampleText = "按钮")
        {
            //GUIStyle style = styleButton();
            // 使用GUI.skin.button的字体大小作为参考，如果style中有自定义字体大小则应直接使用style.fontSize
            float lineHeight = style.fontSize + style.padding.vertical; // 基于字体大小和垂直内边距估算高度
                                                                        // 使用CalcSize进一步细化估算，考虑到文本的实际大小可能影响按钮高度
            Vector2 size = GUI.skin.button.CalcSize(new GUIContent(sampleText));
            return Mathf.Max(lineHeight, size.y); // 选取字体行高和文本内容高度中的较大者作为估算值
        }
        public static GUIStyle SetTxtStyle() /// 设置文本框的样式
        {
            GUIStyle styleTxt = new GUIStyle(GUI.skin.textField);
            //style.normal.background = MakeTex(500, 50, Color.white); // 创建一个纯白色的背景纹理
            styleTxt.normal.textColor = Color.white; // 设置字体颜色为黑色
            styleTxt.alignment = TextAnchor.MiddleLeft;
            styleTxt.fontSize = 20;
            return styleTxt;
        }
        public static GUIStyle SetLabelStyle_JuZhong()
        {
            // 创建或获取一个居中的GUIStyle
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter; // 设置文本居中对齐
            style.normal.textColor = Color.gray;
            style.fontSize = 20;
            return style;
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
                    GUILayout.Label(ColorfulSpeek.colorshows(name), GUILayout.Width(100));
                }

                string combinedKeys = "";

                if (keycodes != null && keycodes.Count > 0)
                {
                    List<string> keyStrings = new List<string>();
                    foreach (var keycode in keycodes)
                    {
                        if (keycode.keyCode1 != KeyCode.None)
                        {
                            string keystring = keycode.keyCode1.ToString();

                            if (keycode.keyCode2 != KeyCode.None)
                            {
                                keystring += $" + {keycode.keyCode2.ToString()}";
                            }

                            if (shuzi == 1)
                            {
                                keystring += " + Num";
                            }
                            else if (shuzi == 2)
                            {
                                keystring += " + Num + Num";
                            }

                            keyStrings.Add(keystring);
                        }
                    }
                    combinedKeys = string.Join(" | ", keyStrings);
                }
                else
                {
                    combinedKeys = "无快捷键";
                }

                GUILayout.Label(ColorfulSpeek.colorshows(combinedKeys));

                if (!string.IsNullOrEmpty(tishi))
                {
                    tishi = TranslateButtonText(tishi);
                    GUILayout.Label(ColorfulSpeek.colorshows(tishi));
                }

                GUILayout.FlexibleSpace();
            }
        }



        public static void CreatShuZhi(string name, ref float zhi, float min, float max, float add, Action callback = null, float? yuan = null)//创建加减数值的按钮
        {
            GUILayout.BeginHorizontal();
            ///$"<b><size=16>YxMod <i><color=grey>{BanBen}</color></i></size></b>"
            name = TranslateButtonText(name); // 确保按钮名称翻译
            GUILayout.Label(ColorfulSpeek.colorshows(name), GUILayout.Width(100));
            //GUILayout.Space(5);
            if (GUILayout.Button(ColorfulSpeek.colorshows("-"), UI.styleButton()))
            {
                zhi -= add;
                zhi = float.Parse(zhi.ToString("0.0"));
                if (zhi < min)
                {
                    zhi = min;
                }
                else
                {
                    callback?.Invoke(); // 如果callback不为null，则调用它
                }
            }

            // 使用 string 存储和显示 float 值
            string zhiStr = zhi.ToString("0.0");

            // 绘制可编辑文本框
            zhiStr = GUILayout.TextField(zhiStr, SetLabelStyle_JuZhong());

            // 尝试解析用户输入并更新 zhi
            if (float.TryParse(zhiStr, out float inputValue))
            {
                inputValue = Mathf.Clamp(inputValue, min, max);
                if (Math.Abs(zhi - inputValue) > 0.001f)
                {
                    zhi = float.Parse(inputValue.ToString("0.0")); // 保留 1 位小数
                    callback?.Invoke();
                }
            }


            if (GUILayout.Button(ColorfulSpeek.colorshows("+"), UI.styleButton()))
            {
                zhi += add;
                zhi = float.Parse(zhi.ToString("0.0"));
                if (zhi > max)
                {
                    zhi = max;
                }
                else
                {
                    callback?.Invoke(); // 如果callback不为null，则调用它
                }
            }

            GUILayout.FlexibleSpace();
            if (yuan.HasValue)
            {
                if (GUILayout.Button(ColorfulSpeek.colorshows("重置"), UI.styleButton()))
                {
                    zhi = float.Parse(yuan.Value.ToString("0.0"));
                    callback?.Invoke();
                }
            }

            //GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }
        public static void CreatShuZhi(string name, ref int zhi, int min, int max, int add, Action callback = null, int? yuan = null)//创建加减数值的按钮
        {
            GUILayout.BeginHorizontal();
            ///$"<b><size=16>YxMod <i><color=grey>{BanBen}</color></i></size></b>"
            name = TranslateButtonText(name); // 确保按钮名称翻译
            GUILayout.Label(ColorfulSpeek.colorshows(name), GUILayout.Width(100));
            //GUILayout.Space(5);
            if (GUILayout.Button(ColorfulSpeek.colorshows("-"), UI.styleButton()))
            {
                zhi -= add;
                //zhi = float.Parse(zhi.ToString("0.0"));
                if (zhi < min)
                {
                    zhi = min;
                }
                else
                {
                    callback?.Invoke(); // 如果callback不为null，则调用它
                }
            }

            // 显示可编辑输入框（整数）
            string inputStr = zhi.ToString();
            inputStr = GUILayout.TextField(inputStr, SetLabelStyle_JuZhong());

            if (int.TryParse(inputStr, out int parsedValue))
            {
                parsedValue = Mathf.Clamp(parsedValue, min, max);
                if (parsedValue != zhi)
                {
                    zhi = parsedValue;
                    callback?.Invoke();
                }
            }

            if (GUILayout.Button(ColorfulSpeek.colorshows("+"), UI.styleButton()))
            {
                zhi += add;
                //zhi = float.Parse(zhi.ToString("0.0"));
                if (zhi > max)
                {
                    zhi = max;
                }
                else
                {
                    callback?.Invoke(); // 如果callback不为null，则调用它
                }
            }

            GUILayout.FlexibleSpace();
            if (yuan.HasValue)
            {
                if (GUILayout.Button(ColorfulSpeek.colorshows("重置"), UI.styleButton()))
                {
                    zhi = yuan.Value;
                    callback?.Invoke();
                }
            }

            //GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

        }
        public static void CreatWenBenKuang(string name, ref string str, int maxChang, int KuanDuan,Action callback = null)//创建文本框
        {
            GUILayout.BeginHorizontal();
            ///$"<b><size=16>YxMod <i><color=grey>{BanBen}</color></i></size></b>"
            if (name != null)
            {
                name = TranslateButtonText(name); // 确保按钮名称翻译
                GUILayout.Label(ColorfulSpeek.colorshows(name));
            }
      
            string str1 = GUILayout.TextField(str, SetTxtStyle(), GUILayout.Width(KuanDuan));
            if(str != str1)
            {
                str = str1;
                if (str.Length > maxChang)
                {
                    //str = str.PadLeft(maxChang);
                    str = str.Substring(0, maxChang); // 超过长度则截取
                }
                callback?.Invoke(); // 如果callback不为null，则调用它

            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public static void CreatYanSeKuang(string name, string strYanSe, Action callback)//创建文本框
        {
            GUILayout.BeginHorizontal();
            ///$"<b><size=16>YxMod <i><color=grey>{BanBen}</color></i></size></b>"
            if (name != null)
            {
                name = TranslateButtonText(name); // 确保按钮名称翻译
                GUILayout.Label(ColorfulSpeek.colorshows(name));
            }

            UI.CreatAnNiu($"<color={strYanSe}>{strYanSe}</color>", false, callback);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void CreatFenGeXian()
        {
            // 初始化一个自定义的GUIStyle并设置边框宽度为0以去除边框
            GUIStyle LineStyle = new GUIStyle(GUI.skin.box);
            LineStyle.border = new RectOffset(0, 0, 0, 0); // 设置四个边的边框宽度都为0

            GUILayout.Box("", LineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(2)); // 高度设为2代表线条的粗细

        }
        public static void CreatAnNiu_Left(string name, bool chuizhijuzhong = true, Action callback = null, string tooltip = null)//一般按钮
        {
            if (chuizhijuzhong)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
            }

            GUIContent content = new GUIContent(ColorfulSpeek.colorshows(name));
            Rect buttonRect = GUILayoutUtility.GetRect(content, styleButton());

            if (GUI.Button(buttonRect, content, styleButton_Left()))
            {
                callback?.Invoke(); // 如果callback不为null，则调用它
            }

            // 设置提示文字
            if (!string.IsNullOrEmpty(tooltip) && buttonRect.Contains(Event.current.mousePosition))
            {
                currentTooltip = tooltip;
            }
            //int topBottomPadding = (GUI.skin.button.padding.top + GUI.skin.button.padding.bottom); // 这是按钮样式自带的上下内边距之和
            //float estimatedButtonHeight = GUI.skin.button.CalcSize(new GUIContent("Sample Text")).y + topBottomPadding;
            if (chuizhijuzhong)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }
            buttonHeight = GetButtonHeight(styleButton(), ColorfulSpeek.colorshows(name)) + 6;
            //Debug.Log(buttonHeight);
        }
        public static void CreatAnNiu(string name, bool chuizhijuzhong = true, Action callback = null, string tooltip = null)//一般按钮
        {
            if (chuizhijuzhong)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
            }

            name = TranslateButtonText(name); // 确保按钮名称翻译
            GUIContent content = new GUIContent(ColorfulSpeek.colorshows(name));
            Rect buttonRect = GUILayoutUtility.GetRect(content, styleButton());

            if (GUI.Button(buttonRect, content, styleButton()))
            {
                callback?.Invoke(); // 如果callback不为null，则调用它
            }

            // 设置提示文字
            if (!string.IsNullOrEmpty(tooltip) && buttonRect.Contains(Event.current.mousePosition))
            {
                tooltip = TranslateButtonText(tooltip); // 确保提示文字也翻译
                currentTooltip = tooltip;
            }
            //int topBottomPadding = (GUI.skin.button.padding.top + GUI.skin.button.padding.bottom); // 这是按钮样式自带的上下内边距之和
            //float estimatedButtonHeight = GUI.skin.button.CalcSize(new GUIContent("Sample Text")).y + topBottomPadding;
            if (chuizhijuzhong)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }
            buttonHeight = GetButtonHeight(styleButton(), ColorfulSpeek.colorshows(name))+6;
            //Debug.Log(buttonHeight);
        }

        public static void CreatAnNiu_AnXia(string name, ref bool tab, bool chuizhijuzhong = true, Action callback = null, string tooltip = null)//Tab按钮
        {
            if (chuizhijuzhong)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
            }
            //if (GUILayout.Button(ColorfulSpeek.colorshows(name), styleButton_Tab(tab)))
            //{
            //    tab = !tab;
            //    callback?.Invoke(); // 如果callback不为null，则调用它
            //}
            name = TranslateButtonText(name);
            Rect buttonRect = GUILayoutUtility.GetRect(new GUIContent(ColorfulSpeek.colorshows(name)), styleButton_Tab(tab));
            if (GUI.Button(buttonRect, ColorfulSpeek.colorshows(name), styleButton_Tab(tab)))
            {
                tab = !tab;
                callback?.Invoke();
            }
            if (!string.IsNullOrEmpty(tooltip) && buttonRect.Contains(Event.current.mousePosition))
            {
                tooltip = TranslateButtonText(tooltip); // 确保提示文字也翻译
                currentTooltip = tooltip;
            }
            if (chuizhijuzhong)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }
            buttonHeight = GetButtonHeight(styleButton(), ColorfulSpeek.colorshows(name)) +6;
        }


        public static void CreatUiBox(Rect rect, Texture2D texture)//创建界面Box
        {
            GUIStyle myGuiStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = texture },
            };
            GUI.Box(rect, GUIContent.none, myGuiStyle);//GUIContent.none      "<b><size=16>菜 单</size></b>"
        }
        public static void DrawTooltip()
        {
            if (!string.IsNullOrEmpty(currentTooltip))
            {
                // 获取屏幕坐标（左下为原点）
                Vector2 mousePos = Input.mousePosition;
                mousePos.y = Screen.height - mousePos.y; // 转为IMGUI左上原点

                GUIStyle tooltipStyle = new GUIStyle(GUI.skin.box);
                tooltipStyle.wordWrap = true;
                tooltipStyle.alignment = TextAnchor.UpperLeft;
                tooltipStyle.padding = new RectOffset(8, 8, 6, 6);

                float maxWidth = 300;
                string coloredText = ColorfulSpeek.colorshows(currentTooltip);
                Vector2 contentSize = tooltipStyle.CalcSize(new GUIContent(coloredText));
                float width = Mathf.Min(contentSize.x + 16, maxWidth);
                float height = tooltipStyle.CalcHeight(new GUIContent(coloredText), width);

                Rect rect = new Rect(mousePos.x + 15, mousePos.y + 15, width, height + 12);
                GUI.Box(rect, coloredText, tooltipStyle);
            }
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
            StartTranslationAsync(originalText, targetLang);

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
            ["个人定点"] = new()
            {
                ["English"] = "Personal Point",
                ["Chinese"] = "个人定点",
                ["French"] = "Point Personnel",
                ["Japanese"] = "個人ポイント",
            },
            ["无碰撞"] = new()
            {
                ["English"] = "No Collision",
                ["Chinese"] = "无碰撞",
                ["French"] = "Sans Collision",
                ["Japanese"] = "衝突なし",
            },
            ["无假死"] = new()
            {
                ["English"] = "No Ragdoll",
                ["Chinese"] = "无假死",
                ["French"] = "Pas de Ragdoll",
                ["Japanese"] = "ラグドール無効",
            },
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
