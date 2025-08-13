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

            //LoadCacheFromFile(); // 加载翻译缓存
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
                    string quanyuan = UI.TranslateButtonText("全员控制");
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
                // 添加本地分身
                for (int k = 1; k < NetGame.instance.local.players.Count; k++)
                {
                    string cloneName = $"{hosts.Count + k + 1}.{NetGame.instance.local.players[k].human.name}分身";
                    AddHumanListButton(ColorfulSpeek.colorshows(cloneName));
                }


                index = GUILayout.SelectionGrid(index, humanNames, 1, styleSelectionGrid());
                //Debug.Log("当前选中：" + index + "   客户机总数：" + hosts.Count);
                //index = (index > (quanyuankongzhi? hosts.Count + 1:hosts.Count)) ? 0 : index;
                int totalCount = hosts.Count + NetGame.instance.local.players.Count; // 1是主机
                if (quanyuankongzhi)
                {
                    totalCount += 1; // 多加一个"全员控制"
                }

                index = (index >= totalCount) ? 0 : index;

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
            //GUILayout.Label(ColorfulSpeek.colorshows(name), GUILayout.Width(100));
            GUILayout.Label(ColorfulSpeek.colorshows(name));
            GUILayout.FlexibleSpace();
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
            GUILayout.FlexibleSpace();
            if (yuan.HasValue)
            {
                if (GUILayout.Button(ColorfulSpeek.colorshows(UI.TranslateButtonText("重置")), UI.styleButton()))
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
            //GUILayout.Label(ColorfulSpeek.colorshows(name), GUILayout.Width(100));
            GUILayout.Label(ColorfulSpeek.colorshows(name));
            GUILayout.FlexibleSpace();
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
            GUILayout.FlexibleSpace();
            if (yuan.HasValue)
            {
                if (GUILayout.Button(ColorfulSpeek.colorshows(UI.TranslateButtonText("重置")), UI.styleButton()))
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
            ["自定义Y"] = new() { ["English"] = "Custom Y" },
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
