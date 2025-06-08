using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Multiplayer;
using Steamworks;
using UnityEngine;

namespace YxModDll.Mod
{
    // Token: 0x020008BA RID: 2234
    public class ColorfulSpeek : MonoBehaviour
    {
        // Token: 0x060051C2 RID: 20930 RVA: 0x0013ED38 File Offset: 0x0013CF38
        private void Top_Name_Change(string txt)
        {
            if (ColorfulSpeek.Func__(txt))
            {
                ColorfulSpeek.Top_Name = "";
                NetChat.Print("已恢复头顶名称，重新加入游戏生效");
                return;
            }
            string[] array = ColorfulSpeek.Func_00(txt, new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int num = 0;
            if (int.TryParse(array[0], out num))
            {
                num--;
                ColorfulSpeek.Top_Name = Human.all[num].player.host.name;
                NetChat.Print("已变更头顶名称，重新加入游戏生效");
                return;
            }
            ColorfulSpeek.Top_Name = txt;
            NetChat.Print("已变更头顶名称，重新加入游戏生效");
        }

        // Token: 0x060051C3 RID: 20931 RVA: 0x0013EDC0 File Offset: 0x0013CFC0
        public ColorfulSpeek()
        {
            this.hang_up_symbol = new string[] { "", "", "", "", "", "" };
            this.txtlength = new List<long>();
            this.渐变存数据 = new string[]
            {
                "//==================格式=============", "//", "//  名字 \t\t颜色代码1\t\t 颜色代码2\t颜色代码3....\t", "//", "//  格式要求：", "//\t1.书写格式参考上边的。", "//  \t2.用空格或tab隔开，不整齐也行，只要有空格或tab隔开就行。", "// \t3.颜色代码 少于等于五个是渐变，多于五个就是突变。", "// \t4.渐变代码参考网站 ：uigradients.com 。不小心弄炸了，删除那一行数据，保存文件然后重启游戏就行。", "//\t5.有没有#都无所谓，都能识别。",
                "//\t6.下边的是数据：", "//--------------------------------------------------------------------------------------------------------------------------", "天鹅绒太阳 \t#e1eec3 \t\t#f05053", "数字水 \t\t#74ebd5\t \t#ACB6E5", "榛 \t\t#77A1D3\t\t#79CBCA \t#E684AE", "可乐的渐变\t#FCE4EC\t\t#F8BBD0\t\t#F48FB1\t \t#F06292  #F48FB1  #F8BBD0  #FCE4EC  ", "决辞的七彩 \t#00EC00\t\t#3AE3DA\t\t#FF6133\t\t#ECFF33 #91B9EF #AEABF6 #CB9DFD"
            };
            this.gradientSeeds = new List<ColorfulSpeek.GradientSeed>();
            this.gradient_preview = ColorfulSpeek.Func_01(560, 170, TextureFormat.ARGB32, false);
            this.GradientList = new List<string>();
            this.Chat_note = 1;
            this.Chat_roomnamebar = 0;
        }

        // Token: 0x060051C4 RID: 20932 RVA: 0x000255C1 File Offset: 0x000237C1
        public string System_Color(uint netID)
        {
            checked
            {
                return HexConverter.ColorToHex(NetChat.instance.colors[(int)((IntPtr)((long)(unchecked((ulong)netID % (ulong)((long)NetChat.instance.colors.Length)))))]);
            }
        }

        // Token: 0x060051C5 RID: 20933 RVA: 0x0013EEF4 File Offset: 0x0013D0F4
        public string Out_Gradual_Name(string name, int gradual_num, int size_in)
        {
            if (gradual_num == 0)
            {
                return name;
            }
            string text = size_in.ToString();
            string[] list = ColorfulSpeek.NameSeed.list;
            string text2 = "";
            if (name.Length < list.Length - 2)
            {
                int num = list.Length / name.Length;
                int num2 = 0;
                int i = 0;
                while (i < list.Length)
                {
                    if (num2 >= name.Length)
                    {
                        break;
                    }
                    string text3 = string.Format("<color={0}>{1}</color>", list[i], name[num2].ToString());
                    text2 += text3;
                    i += num;
                    num2++;
                }
            }
            else
            {
                for (int j = 0; j < name.Length; j++)
                {
                    string text4 = string.Format("<color={0}>{1}</color>", list[j % list.Length], name[j].ToString());
                    text2 += text4;
                }
            }
            text2 = string.Format("<size={0}>{1}</size>", text, text2);
            if (ColorfulSpeek.Name_Bold)
            {
                text2 = "<b>" + text2 + "</b>";
            }
            if (ColorfulSpeek.Name_Italic)
            {
                text2 = "<i>" + text2 + "</i>";
            }
            return text2;
        }

        // Token: 0x060051C6 RID: 20934 RVA: 0x0013F00C File Offset: 0x0013D20C
        private void Name_to_Gradual(string txt)
        {
            if (ColorfulSpeek.Func__(txt))
            {
                return;
            }
            string[] array = ColorfulSpeek.Func_00(txt, new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int num = 0;
            if (!int.TryParse(array[0], out num) || num < 0 || num > 16 || array.Length != 2)
            {
                NetChat.Print("渐变字体范围在0-16");
                return;
            }
            int num2 = 0;
            if (int.TryParse(array[1], out num2) && num2 >= 0)
            {
                ColorfulSpeek.Chat_Name = this.Out_Gradual_Name(ColorfulSpeek.Chat_Name, num, num2);
                ColorfulSpeek.Func_02("Chat_Name", ColorfulSpeek.Chat_Name);
                NetChat.Print("名称已赋予渐变色");
                return;
            }
        }

        // Token: 0x060051C7 RID: 20935 RVA: 0x0013F09C File Offset: 0x0013D29C
        private void HouZhui()
        {
            ColorfulSpeek.Houzhui = !ColorfulSpeek.Houzhui;
            if (ColorfulSpeek.Houzhui)
            {
                ColorfulSpeek.Func_03("Houzhui", 1);
                NetChat.Print("已配置名字后缀");
                return;
            }
            ColorfulSpeek.Func_03("Houzhui", 0);
            NetChat.Print("名字已恢复为前缀");
        }

        // Token: 0x060051C8 RID: 20936 RVA: 0x0013F0E8 File Offset: 0x0013D2E8
        private void GradualchangeMode()
        {
            ColorfulSpeek.Gradual_Change = !ColorfulSpeek.Gradual_Change;
            if (!ColorfulSpeek.Gradual_Change)
            {
                ColorfulSpeek.Func_03("Gradual_Change", 0);
                NetChat.Print("发言字体颜色已恢复");
                return;
            }
            ColorfulSpeek.Func_03("Gradual_Change", 1);
            NetChat.Print("发言字体已变更为渐变色");
        }

        // Token: 0x060051C9 RID: 20937 RVA: 0x0013F134 File Offset: 0x0013D334
        public string GradualChange(string msg)
        {
            if (ColorfulSpeek.Gradual_Change)
            {
                if (ColorfulSpeek.Gradual_Random)
                {
                    int num = UnityEngine.Random.Range(0, this.gradientSeeds.Count - 1);
                    ColorfulSpeek.WordSeed = this.GetSeedByIndex(num, 0f, 1f);
                }
                string[] list = ColorfulSpeek.WordSeed.list;
                int size_Words = ColorfulSpeek.Size_Words;
                string text = "";
                global::System.Random random = ColorfulSpeek.Func_04();
                if (ColorfulSpeek.Func_05(msg) >= list.Length - 2)
                {
                    for (int i = 0; i < msg.Length; i++)
                    {
                        int num2 = (ColorfulSpeek.UseRandomSize ? ColorfulSpeek.Func_06(random, 30, 51) : size_Words);
                        string text2 = string.Format("<size={2}><color={0}>{1}</color></size>", list[i % list.Length], ColorfulSpeek.Func_07(msg, i).ToString(), num2);
                        text += text2;
                    }
                }
                else
                {
                    int num3 = list.Length / msg.Length;
                    int num4 = 0;
                    int num5 = 0;
                    while (num5 < list.Length && num4 < msg.Length)
                    {
                        int num6 = (ColorfulSpeek.UseRandomSize ? random.Next(30, 51) : size_Words);
                        string text3 = string.Format("<size={2}><color={0}>{1}</color></size>", list[num5], msg[num4].ToString(), num6);
                        text += text3;
                        num5 += num3;
                        num4++;
                    }
                }
                if (ColorfulSpeek.Words_Bold)
                {
                    text = "<b>" + text + "</b>";
                }
                if (ColorfulSpeek.Words_Italic)
                {
                    text = "<i>" + text + "</i>";
                }
                return text;
            }
            return msg;
        }

        // Token: 0x060051CA RID: 20938 RVA: 0x0013F2B8 File Offset: 0x0013D4B8
        public void Chat_Name_Change(string txt)
        {
            ColorfulSpeek.GameId_Change = false;
            if (ColorfulSpeek.Func__(txt))
            {
                ColorfulSpeek.hostID = NetGame.instance.players[0].host.hostId;
                ColorfulSpeek.Chat_Name = ColorfulSpeek.Func_09(ColorfulSpeek.Func_08());
                ColorfulSpeek.Func_02("Chat_Name", ColorfulSpeek.Chat_Name);
                NetChat.Print("已恢复自己的聊天名称");
                return;
            }
            string[] array = ColorfulSpeek.Func_00(txt, new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int num = 0;
            if (!int.TryParse(array[0], out num))
            {
                ColorfulSpeek.Chat_Name = txt;
                ColorfulSpeek.Func_02("Chat_Name", ColorfulSpeek.Chat_Name);
                NetChat.Print("已变更聊天名称");
                return;
            }
            num--;
            if (num < Human.all.Count && num >= 0)
            {
                ColorfulSpeek.hostID = (uint)num;
                ColorfulSpeek.GameId_Change = true;
                ColorfulSpeek.Chat_Name = Human.all[num].player.host.name;
                NetChat.Print("已变更聊天名称");
                return;
            }
        }

        // Token: 0x060051CB RID: 20939 RVA: 0x0013F3A8 File Offset: 0x0013D5A8
        private void Change_Size(string txt)
        {
            if (ColorfulSpeek.Func__(txt))
            {
                return;
            }
            if (!ColorfulSpeek.Gradual_Change)
            {
                NetChat.Print("未开启发言字体渐变，请输入 /渐变 来开启");
                return;
            }
            string[] array = ColorfulSpeek.Func_00(txt, new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int num = 0;
            if (int.TryParse(array[0], out num) && num >= 0 && array.Length <= 1)
            {
                ColorfulSpeek.Size_Words = num;
                ColorfulSpeek.Func_03("Color_Size", num);
                NetChat.Print(ColorfulSpeek.Func_10("字体大小变为{0}", num));
                return;
            }
        }

        // Token: 0x060051CC RID: 20940 RVA: 0x0013F424 File Offset: 0x0013D624
        private void Change_Gradual_Num(string txt)
        {
            if (ColorfulSpeek.Func__(txt))
            {
                return;
            }
            if (!ColorfulSpeek.Gradual_Change)
            {
                NetChat.Print("未开启发言字体渐变，请输入 /渐变 来开启");
                return;
            }
            string[] array = ColorfulSpeek.Func_00(txt, new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int num = 0;
            if (int.TryParse(array[0], out num) && num >= 0 && num <= 363 && array.Length <= 1)
            {
                ColorfulSpeek.Words_ColorType = num;
                ColorfulSpeek.Func_03("Gradual_Num", num);
                NetChat.Print(ColorfulSpeek.Func_10("渐变种类变为{0}", num));
                return;
            }
            NetChat.Print("渐变字体范围在0-16");
        }

        // Token: 0x060051CD RID: 20941 RVA: 0x0013F4B0 File Offset: 0x0013D6B0
        private void Awake()
        {
            ColorfulSpeek.ins = this;
            ColorfulSpeek.Base_Name = ColorfulSpeek.Func_11("Base_Name", ColorfulSpeek.Func_09(ColorfulSpeek.Func_08()));
            ColorfulSpeek.Size_Name = ColorfulSpeek.Func_12("Size_Name", 14);
            ColorfulSpeek.Name_ColorType = ColorfulSpeek.Func_12("Color_Type", 1);
            ColorfulSpeek.Chat_Name = ColorfulSpeek.Func_11("Chat_Name", ColorfulSpeek.Func_09(ColorfulSpeek.Func_08()));
            ColorfulSpeek.Gradual_Change = ColorfulSpeek.Func_12("Gradual_Change", 1) == 1;
            ColorfulSpeek.Houzhui = ColorfulSpeek.Func_12("Houzhui", 0) == 1;
            ColorfulSpeek.Size_Words = ColorfulSpeek.Func_12("Color_Size", 20);
            ColorfulSpeek.Words_ColorType = ColorfulSpeek.Func_12("Gradual_Num", 1);
            ColorfulSpeek.Name_Bold = ColorfulSpeek.Func_12("Name_Bold", 0) == 1;
            ColorfulSpeek.Name_Italic = ColorfulSpeek.Func_12("Name_Italic", 0) == 1;
            ColorfulSpeek.Words_Bold = ColorfulSpeek.Func_12("Name_Bold", 0) == 1;
            ColorfulSpeek.Words_Italic = ColorfulSpeek.Func_12("Name_Italic", 0) == 1;
            ColorfulSpeek.Gradual_Random = ColorfulSpeek.Func_12("Gradual_Random", 0) == 1;
            this.Command();
            this.Awake2();
        }

        // Token: 0x060051CE RID: 20942 RVA: 0x0013F5C8 File Offset: 0x0013D7C8
        public void Command()
        {
            NetChat.RegisterCommand(true, true, "name", new Action<string>(this.Chat_Name_Change), null);
            NetChat.RegisterCommand(true, true, "topname", new Action<string>(this.Top_Name_Change), null);
            NetChat.RegisterCommand(true, true, "后缀", new Action(this.HouZhui), null);
            NetChat.RegisterCommand(true, true, "渐变", new Action(this.GradualchangeMode), null);
            NetChat.RegisterCommand(true, true, "渐变种类", new Action<string>(this.Change_Gradual_Num), null);
            NetChat.RegisterCommand(true, true, "字体大小", new Action<string>(this.Change_Size), null);
            NetChat.RegisterCommand(true, true, "彩色名字", new Action<string>(this.Name_to_Gradual), null);
            NetChat.RegisterCommand(true, true, "gra", new Action<string>(this.Change_GradientList), null);
        }

        // Token: 0x060051CF RID: 20943 RVA: 0x0013F6A0 File Offset: 0x0013D8A0
        public static string colorshow(string msg)
        {
            if (ColorfulSpeek.Gradual_Change)
            {
                if (ColorfulSpeek.Gradual_Random)
                {
                    int num = UnityEngine.Random.Range(0, ColorfulSpeek.ins.gradientSeeds.Count - 1);
                    ColorfulSpeek.WordSeed = ColorfulSpeek.ins.GetSeedByIndex(num, 0f, 1f);
                }
                string[] list = ColorfulSpeek.WordSeed.list;
                int num2 = ColorfulSpeek.Size_Words;
                num2 = 30;
                string text = "";
                if (ColorfulSpeek.Func_05(msg) < list.Length - 2)
                {
                    int num3 = list.Length / ColorfulSpeek.Func_05(msg);
                    int num4 = 0;
                    int i = 0;
                    while (i < list.Length)
                    {
                        if (num4 >= ColorfulSpeek.Func_05(msg))
                        {
                            break;
                        }
                        string text2 = string.Format("<size={2}><color={0}>{1}</color></size>", list[i], ColorfulSpeek.Func_07(msg, num4).ToString(), num2);
                        text += text2;
                        i += num3;
                        num4++;
                    }
                }
                else
                {
                    for (int j = 0; j < msg.Length; j++)
                    {
                        string text3 = string.Format("<size={2}><color={0}>{1}</color></size>", list[j % list.Length], msg[j].ToString(), num2);
                        text += text3;
                    }
                }
                if (ColorfulSpeek.Words_Bold)
                {
                    text = "<b>" + text + "</b>";
                }
                if (ColorfulSpeek.Words_Italic)
                {
                    text = "<i>" + text + "</i>";
                }
                return text;
            }
            return msg;
        }

        // Token: 0x060051D0 RID: 20944 RVA: 0x0013F7FC File Offset: 0x0013D9FC
        public static string colorshows(string msg)
        {
            if (ColorfulSpeek.Gradual_Change)
            {
                string[] list = ColorfulSpeek.WordSeed.list;
                string text = "";
                int num = ColorfulSpeek.Size_Words;
                num = 20;
                if (ColorfulSpeek.Func_05(msg) < list.Length - 2)
                {
                    int num2 = list.Length / ColorfulSpeek.Func_05(msg);
                    int num3 = 0;
                    int i = 0;
                    while (i < list.Length)
                    {
                        if (num3 >= ColorfulSpeek.Func_05(msg))
                        {
                            break;
                        }
                        string text2 = string.Format("<size={2}><color={0}>{1}</color></size>", list[i], ColorfulSpeek.Func_07(msg, num3).ToString(), num);
                        text += text2;
                        i += num2;
                        num3++;
                    }
                }
                else
                {
                    for (int j = 0; j < msg.Length; j++)
                    {
                        string text3 = string.Format("<size={2}><color={0}>{1}</color></size>", list[j % list.Length], msg[j].ToString(), num);
                        text += text3;
                    }
                }
                if (ColorfulSpeek.Words_Bold)
                {
                    text = "<b>" + text + "</b>";
                }
                if (ColorfulSpeek.Words_Italic)
                {
                    text = "<i>" + text + "</i>";
                }
                return text;
            }
            return msg;
        }

        // Token: 0x060051D1 RID: 20945 RVA: 0x0013F91C File Offset: 0x0013DB1C
        public static string colorshowss(string msg)
        {
            if (!ColorfulSpeek.Gradual_Change)
            {
                return msg;
            }
            string[] list = ColorfulSpeek.WordSeed.list;
            int num = ColorfulSpeek.Size_Words;
            num = 12;
            string text = "";
            if (ColorfulSpeek.Func_05(msg) < list.Length - 2)
            {
                int num2 = list.Length / ColorfulSpeek.Func_05(msg);
                int num3 = 0;
                int i = 0;
                while (i < list.Length)
                {
                    if (num3 >= ColorfulSpeek.Func_05(msg))
                    {
                        break;
                    }
                    string text2 = string.Format("<size={2}><color={0}>{1}</color></size>", list[i], ColorfulSpeek.Func_07(msg, num3).ToString(), num);
                    text += text2;
                    i += num2;
                    num3++;
                }
            }
            else
            {
                for (int j = 0; j < msg.Length; j++)
                {
                    string text3 = string.Format("<size={2}><color={0}>{1}</color></size>", list[j % list.Length], msg[j].ToString(), num);
                    text += text3;
                }
            }
            if (ColorfulSpeek.Words_Bold)
            {
                text = "<b>" + text + "</b>";
            }
            if (ColorfulSpeek.Words_Italic)
            {
                text = "<i>" + text + "</i>";
            }
            return text;
        }

        // Token: 0x060051D3 RID: 20947 RVA: 0x0013FA38 File Offset: 0x0013DC38
        public static Color ColorSum(Color col1, Color col2)
        {
            return new Color(ColorfulSpeek.Func_13(col1.r + col2.r, 255f), ColorfulSpeek.Func_13(col1.g + col2.g, 255f), ColorfulSpeek.Func_13(col1.b + col2.b, 255f));
        }

        // Token: 0x060051D4 RID: 20948 RVA: 0x0013FA90 File Offset: 0x0013DC90
        public static Gradient GetGradient(Color start, Color end)
        {
            Gradient gradient = ColorfulSpeek.Func_14();
            GradientColorKey[] array = new GradientColorKey[2];
            array[0].color = start;
            array[0].time = 0f;
            array[1].color = end;
            array[1].time = 1f;
            GradientAlphaKey[] array2 = new GradientAlphaKey[2];
            array2[0].alpha = 1f;
            array2[0].time = 1f;
            array2[1].alpha = 0f;
            array2[1].time = 1f;
            ColorfulSpeek.Func_15(gradient, array, array2);
            return gradient;
        }

        // Token: 0x060051D5 RID: 20949 RVA: 0x0013FB38 File Offset: 0x0013DD38
        public static Color GradientColorbyStairs(Color startColor, Color endColor, float time)
        {
            float num = startColor[0] - endColor[0];
            float num2 = startColor[1] - endColor[1];
            float num3 = startColor[2] - endColor[2];
            float num4 = endColor[0] + (num * time + 0.5f);
            float num5 = endColor[1] + (num2 * time + 0.5f);
            float num6 = endColor[2] + (num3 * time + 0.5f);
            return new Color(num4, num5, num6);
        }

        // Token: 0x060051D6 RID: 20950 RVA: 0x0013FBBC File Offset: 0x0013DDBC
        public static Gradient GetGradient(Color start, Color middle, Color end)
        {
            Gradient gradient = ColorfulSpeek.Func_14();
            GradientColorKey[] array = new GradientColorKey[3];
            array[0].color = start;
            array[0].time = 0f;
            array[1].color = middle;
            array[1].time = 0.5f;
            array[2].color = end;
            array[2].time = 1f;
            GradientAlphaKey[] array2 = new GradientAlphaKey[3];
            array2[0].alpha = 1f;
            array2[0].time = 1f;
            array2[1].alpha = 0f;
            array2[1].time = 1f;
            array2[2].alpha = 0f;
            array2[2].time = 1f;
            ColorfulSpeek.Func_15(gradient, array, array2);
            return gradient;
        }

        // Token: 0x060051D7 RID: 20951 RVA: 0x0013FCA4 File Offset: 0x0013DEA4
        public void Change_GradientList(string txt)
        {
            if (ColorfulSpeek.Func__(txt))
            {
                ColorfulSpeek.Func_16("input empty string");
                return;
            }
            string[] array = ColorfulSpeek.Func_00(txt, new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length > 5 || array.Length < 2)
            {
                return;
            }
            Gradient gradient = ColorfulSpeek.Func_14();
            if (array.Length == 2 || array.Length == 4)
            {
                if (ColorfulSpeek.Func_17(array[0], "#"))
                {
                    array[0] = ColorfulSpeek.Func_18(array[0], "#", "");
                }
                if (ColorfulSpeek.Func_17(array[1], "#"))
                {
                    array[1] = ColorfulSpeek.Func_18(array[1], "#", "");
                }
                gradient = ColorfulSpeek.GetGradient(HexConverter.HexToColor(array[0]), HexConverter.HexToColor(array[1]));
            }
            if (array.Length == 3 || array.Length == 5)
            {
                if (ColorfulSpeek.Func_17(array[0], "#"))
                {
                    array[0] = ColorfulSpeek.Func_18(array[0], "#", "");
                }
                if (ColorfulSpeek.Func_17(array[1], "#"))
                {
                    array[1] = ColorfulSpeek.Func_18(array[1], "#", "");
                }
                if (ColorfulSpeek.Func_17(array[2], "#"))
                {
                    array[2] = ColorfulSpeek.Func_18(array[1], "#", "");
                }
                gradient = ColorfulSpeek.GetGradient(HexConverter.HexToColor(array[0]), HexConverter.HexToColor(array[1]), HexConverter.HexToColor(array[2]));
            }
            int num = ColorfulSpeek.Func_19(this.gradient_preview);
            for (int i = 0; i < num; i++)
            {
                float num2 = (float)(i + 1) / (float)num;
                for (int j = 0; j < ColorfulSpeek.Func_22(this.gradient_preview); j++)
                {
                    ColorfulSpeek.Func_21(this.gradient_preview, i, j, ColorfulSpeek.Func_20(gradient, num2));
                }
            }
            ColorfulSpeek.Func_23(this.gradient_preview, false);
            this.GradientList.Clear();
            if (array.Length != 2 && array.Length != 3)
            {
                if (array.Length == 4 || array.Length == 5)
                {
                    float num3 = 0f;
                    float num4 = 0f;
                    float.TryParse(array[array.Length - 2], out num3);
                    float.TryParse(array[array.Length - 1], out num4);
                    int num5 = 20;
                    for (int k = 0; k < num5; k++)
                    {
                        float num6 = num3 + (float)(k + 1) / (float)num5 * (num4 - num3);
                        Color color = ColorfulSpeek.Func_20(gradient, num6);
                        this.GradientList.Add(ColorfulSpeek.Func_24("#", HexConverter.ColorToHex(color)));
                    }
                }
                return;
            }
            int num7 = 20;
            for (int l = 0; l < num7; l++)
            {
                float num8 = (float)(l + 1) / (float)num7;
                Color color2 = ColorfulSpeek.Func_20(gradient, num8);
                this.GradientList.Add(ColorfulSpeek.Func_24("#", HexConverter.ColorToHex(color2)));
            }
        }

        // Token: 0x060051D8 RID: 20952 RVA: 0x0013FF60 File Offset: 0x0013E160
        private void GradientSeedsAwake()
        {
            this.gradientSeeds.Clear();
            ColorfulSpeek.GradientSeed gradientSeed = new ColorfulSpeek.GradientSeed
            {
                name = "灰色渐变",
                contains = new string[2]
            };
            gradientSeed.contains[0] = "#bdc3c7";
            gradientSeed.contains[1] = "#2c3e50";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "灰色渐变";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#bdc3c7";
            gradientSeed.contains[1] = "#2c3e50";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "小猪粉红";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ee9ca7";
            gradientSeed.contains[1] = "#ffdde1";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "酷蓝调";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#2193b0";
            gradientSeed.contains[1] = "#6dd5ed";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "月光小行星";
            gradientSeed.contains = new string[3];
            gradientSeed.contains[0] = "#0F2027";
            gradientSeed.contains[1] = "##203A43";
            gradientSeed.contains[2] = "#2C5364";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "傍晚的阳光";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##b92b27";
            gradientSeed.contains[1] = "##1565C0";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "凉爽的天空";
            gradientSeed.contains = new string[3];
            gradientSeed.contains[0] = "##2980B9";
            gradientSeed.contains[1] = "##6DD5FA";
            gradientSeed.contains[2] = "###FFFFFF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "Memariani";
            gradientSeed.contains = new string[3];
            gradientSeed.contains[0] = "###aa4b6b";
            gradientSeed.contains[1] = "###6b6b83";
            gradientSeed.contains[2] = "####3b8d99";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "Harvey";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "###1f4037";
            gradientSeed.contains[1] = "###99f2c8";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "碧蓝航线";
            gradientSeed.contains = new string[3];
            gradientSeed.contains[0] = "###7F7FD5";
            gradientSeed.contains[1] = "###86A8E7";
            gradientSeed.contains[2] = "####91EAE4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "女巫时间";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "###c31432";
            gradientSeed.contains[1] = "###240b36";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "大都市";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##659999";
            gradientSeed.contains[1] = "##f4791f";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "京友";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##dd3e54";
            gradientSeed.contains[1] = "##6be585";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "通过设计";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##009FFF";
            gradientSeed.contains[1] = "##ec2F4B";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "UltraVoilet";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##654ea3";
            gradientSeed.contains[1] = "##eaafc8";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "柑橘皮";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##FDC830";
            gradientSeed.contains[1] = "##F37335";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "Margo";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##FFEFBA";
            gradientSeed.contains[1] = "##FFFFFF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "魔法";
            gradientSeed.contains = new string[3];
            gradientSeed.contains[0] = "##59C173";
            gradientSeed.contains[1] = "#a17fe0";
            gradientSeed.contains[2] = "##5D26C1";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "易变";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##636363";
            gradientSeed.contains[1] = "##a2ab58";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "eXpresso";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##ad5389";
            gradientSeed.contains[1] = "##3c1053";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "纯色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##333333";
            gradientSeed.contains[1] = "##dd1818";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "月亮紫色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##4e54c8";
            gradientSeed.contains[1] = "##8f94fb";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "夕阳红";
            gradientSeed.contains = new string[3];
            gradientSeed.contains[0] = "##355C7D";
            gradientSeed.contains[1] = "##6C5B7B";
            gradientSeed.contains[2] = "##C06C84";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "婚礼那天的布鲁斯";
            gradientSeed.contains = new string[3];
            gradientSeed.contains[0] = "##40E0D0";
            gradientSeed.contains[1] = "##FF8C00";
            gradientSeed.contains[2] = "###C06C84";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "蓬雪果";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "###108dc7";
            gradientSeed.contains[1] = "###ef8e38";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "沙拉拉";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##D66D75";
            gradientSeed.contains[1] = "##E29587";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "甘露";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##43C6AC";
            gradientSeed.contains[1] = "##F8FFAE";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "Roseanna";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##FFAFBD";
            gradientSeed.contains[1] = "##ffc3a0";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "甜蜜的早晨";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##FF5F6D";
            gradientSeed.contains[1] = "##FFC371";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "Tranquil";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##EECDA3";
            gradientSeed.contains[1] = "##EF629F";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "Can You Feel The Love Tonight";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "##4568DC";
            gradientSeed.contains[1] = "##B06AB3";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "大头";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "###c94b4b";
            gradientSeed.contains[1] = "###4b134f";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#bdc3c7";
            gradientSeed.contains[1] = "#2c3e50";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ee9ca7";
            gradientSeed.contains[1] = "#ffdde1";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#2193b0";
            gradientSeed.contains[1] = "#6dd5ed";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#C6FFDD";
            gradientSeed.contains[1] = "#f7797d";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#12c2e9";
            gradientSeed.contains[1] = "#f7797d";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#b92b27";
            gradientSeed.contains[1] = "#1565C0";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#2980B9";
            gradientSeed.contains[1] = "#FFFFFF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FF0099";
            gradientSeed.contains[1] = "#493240";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#aa4b6b";
            gradientSeed.contains[1] = "#3b8d99";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#1f4037";
            gradientSeed.contains[1] = "#99f2c8";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#f953c6";
            gradientSeed.contains[1] = "#b91d73";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#7F7FD5";
            gradientSeed.contains[1] = "#91EAE4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#f12711";
            gradientSeed.contains[1] = "#f5af19";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#659999";
            gradientSeed.contains[1] = "#f4791f";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#8360c3";
            gradientSeed.contains[1] = "#2ebf91";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#654ea3";
            gradientSeed.contains[1] = "#eaafc8";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FF416C";
            gradientSeed.contains[1] = "#FF4B2B";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#a8ff78";
            gradientSeed.contains[1] = "#78ffd6";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FDC830";
            gradientSeed.contains[1] = "#F37335";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00B4DB";
            gradientSeed.contains[1] = "#0083B0";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFEFBA";
            gradientSeed.contains[1] = "#FFFFFF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#DA4453";
            gradientSeed.contains[1] = "#89216B";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#59C173";
            gradientSeed.contains[1] = "#a17fe0";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#4e54c8";
            gradientSeed.contains[1] = "#8f94fb";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#355C7D";
            gradientSeed.contains[1] = "#C06C84";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#bc4e9c";
            gradientSeed.contains[1] = "#f80759";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#40E0D0";
            gradientSeed.contains[1] = "#FF8C00";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FF8C00";
            gradientSeed.contains[1] = "#FF0080";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#11998e";
            gradientSeed.contains[1] = "#38ef7d";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FC5C7D";
            gradientSeed.contains[1] = "#6A82FB";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#fffbd5";
            gradientSeed.contains[1] = "#b20a2c";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00b09b";
            gradientSeed.contains[1] = "#96c93d";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#D3CCE3";
            gradientSeed.contains[1] = "#E9E4F0";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#CAC531";
            gradientSeed.contains[1] = "#F3F9A7";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00F260";
            gradientSeed.contains[1] = "#0575E6";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#fc4a1a";
            gradientSeed.contains[1] = "#f7b733";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#74ebd5";
            gradientSeed.contains[1] = "#ACB6E5";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ADA996";
            gradientSeed.contains[1] = "#EAEAEA";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#e1eec3";
            gradientSeed.contains[1] = "#f05053";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#22c1c3";
            gradientSeed.contains[1] = "#fdbb2d";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ff9966";
            gradientSeed.contains[1] = "#ff5e62";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#7F00FF";
            gradientSeed.contains[1] = "#E100FF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#C9D6FF";
            gradientSeed.contains[1] = "#E2E2E2";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#d9a7c7";
            gradientSeed.contains[1] = "#fffcdc";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#0cebeb";
            gradientSeed.contains[1] = "#29ffc6";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#642B73";
            gradientSeed.contains[1] = "#C6426E";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#1c92d2";
            gradientSeed.contains[1] = "#f2fcfe";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#36D1DC";
            gradientSeed.contains[1] = "#5B86E5";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#CB356B";
            gradientSeed.contains[1] = "#BD3F32";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#283c86";
            gradientSeed.contains[1] = "#45a247";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#EF3B36";
            gradientSeed.contains[1] = "#FFFFFF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#c0392b";
            gradientSeed.contains[1] = "#8e44ad";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#007991";
            gradientSeed.contains[1] = "#78ffd6";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#56CCF2";
            gradientSeed.contains[1] = "#2F80ED";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F2994A";
            gradientSeed.contains[1] = "#F2C94C";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#E44D26";
            gradientSeed.contains[1] = "#F16529";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#4AC29A";
            gradientSeed.contains[1] = "#BDFFF3";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#B2FEFA";
            gradientSeed.contains[1] = "#0ED2F7";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#30E8BF";
            gradientSeed.contains[1] = "#FF8235";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#D66D75";
            gradientSeed.contains[1] = "#E29587";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F7971E";
            gradientSeed.contains[1] = "#FFD200";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#34e89e";
            gradientSeed.contains[1] = "#0f3443";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#6190E8";
            gradientSeed.contains[1] = "#A7BFE8";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#0575E6";
            gradientSeed.contains[1] = "#021B79";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#4568DC";
            gradientSeed.contains[1] = "#B06AB3";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#43C6AC";
            gradientSeed.contains[1] = "#191654";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#093028";
            gradientSeed.contains[1] = "#237A57";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#43C6AC";
            gradientSeed.contains[1] = "#F8FFAE";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFAFBD";
            gradientSeed.contains[1] = "#ffc3a0";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#E8CBC0";
            gradientSeed.contains[1] = "#636FA4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#DCE35B";
            gradientSeed.contains[1] = "#45B649";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#c0c0aa";
            gradientSeed.contains[1] = "#1cefff";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#DBE6F6";
            gradientSeed.contains[1] = "#C5796D";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#3494E6";
            gradientSeed.contains[1] = "#EC6EAD";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#67B26F";
            gradientSeed.contains[1] = "#4ca2cd";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F3904F";
            gradientSeed.contains[1] = "#3B4371";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ee0979";
            gradientSeed.contains[1] = "#ff6a00";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#A770EF";
            gradientSeed.contains[1] = "#FDB99B";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#41295a";
            gradientSeed.contains[1] = "#2F0743";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#f4c4f3";
            gradientSeed.contains[1] = "#fc67fa";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00c3ff";
            gradientSeed.contains[1] = "#ffff1c";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ff7e5f";
            gradientSeed.contains[1] = "#feb47b";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ffffc00";
            gradientSeed.contains[1] = "#ffffff";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ff00cc";
            gradientSeed.contains[1] = "#333399";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#de6161";
            gradientSeed.contains[1] = "#2657eb";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ef32d9";
            gradientSeed.contains[1] = "#89fffd";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#3a6186";
            gradientSeed.contains[1] = "#89253e";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#4ECDC4";
            gradientSeed.contains[1] = "#556270";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#A1FFCE";
            gradientSeed.contains[1] = "#FAFFD1";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#BE93C5";
            gradientSeed.contains[1] = "#7BC6CC";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#bdc3c7";
            gradientSeed.contains[1] = "#2c3e50";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ffd89b";
            gradientSeed.contains[1] = "#19547b";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#808080";
            gradientSeed.contains[1] = "#3fada8";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#fceabb";
            gradientSeed.contains[1] = "#f8b500";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#f85032";
            gradientSeed.contains[1] = "#e73827";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#f79d00";
            gradientSeed.contains[1] = "#64f38c";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#cb2d3e";
            gradientSeed.contains[1] = "#ef473a";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#56ab2f";
            gradientSeed.contains[1] = "#a8e063";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#42275a";
            gradientSeed.contains[1] = "#734b6d";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F00000";
            gradientSeed.contains[1] = "#DC281E";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#2C3E50";
            gradientSeed.contains[1] = "#FD746C";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#2C3E50";
            gradientSeed.contains[1] = "#4CA1AF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#e96443";
            gradientSeed.contains[1] = "#904e95";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#0B486B";
            gradientSeed.contains[1] = "#F56217";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#3a7bd5";
            gradientSeed.contains[1] = "#3a6073";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00d2ff";
            gradientSeed.contains[1] = "#928DAB";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#2196f3";
            gradientSeed.contains[1] = "#f44336";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FF5F6D";
            gradientSeed.contains[1] = "#FFC371";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ff4b1f";
            gradientSeed.contains[1] = "#ff9068";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#16BFFD";
            gradientSeed.contains[1] = "#CB3066";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#EECDA3";
            gradientSeed.contains[1] = "#EF629F";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#a80077";
            gradientSeed.contains[1] = "#66ff00";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#f7ff00";
            gradientSeed.contains[1] = "#db36a4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ff4b1f";
            gradientSeed.contains[1] = "#1fddff";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#BA5370";
            gradientSeed.contains[1] = "#F4E2D8";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#E0EAFC";
            gradientSeed.contains[1] = "#CFDEF3";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#4CA1AF";
            gradientSeed.contains[1] = "#C4E0E5";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#4B79A1";
            gradientSeed.contains[1] = "#283E51";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#834d9b";
            gradientSeed.contains[1] = "#d04ed6";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#0099F7";
            gradientSeed.contains[1] = "#F11712";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#5A3F37";
            gradientSeed.contains[1] = "#2C7744";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#4DA0B0";
            gradientSeed.contains[1] = "#D39D38";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#5614B0";
            gradientSeed.contains[1] = "#DBD65C";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#114357";
            gradientSeed.contains[1] = "#F29492";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#fd746c";
            gradientSeed.contains[1] = "#ff9068";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#eacda3";
            gradientSeed.contains[1] = "#d6ae7b";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#6a3093";
            gradientSeed.contains[1] = "#a044ff";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#457fca";
            gradientSeed.contains[1] = "#5691c8";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#B24592";
            gradientSeed.contains[1] = "#F15F79";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#C02425";
            gradientSeed.contains[1] = "#F0CB35";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#403A3E";
            gradientSeed.contains[1] = "#BE5869";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#c2e59c";
            gradientSeed.contains[1] = "#64b3f4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFB75E";
            gradientSeed.contains[1] = "#ED8F03";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#76b852";
            gradientSeed.contains[1] = "#8DC26F";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#673AB7";
            gradientSeed.contains[1] = "#512DA8";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00C9FF";
            gradientSeed.contains[1] = "#92FE9D";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#f46b45";
            gradientSeed.contains[1] = "#eea849";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#005C97";
            gradientSeed.contains[1] = "#363795";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#e53935";
            gradientSeed.contains[1] = "#e35d5b";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#fc00ff";
            gradientSeed.contains[1] = "#00dbde";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#CCCCB2";
            gradientSeed.contains[1] = "#757519";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#304352";
            gradientSeed.contains[1] = "#d7d2cc";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ee9ca7";
            gradientSeed.contains[1] = "#ffdde1";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#525252";
            gradientSeed.contains[1] = "#3d72b4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#004FF9";
            gradientSeed.contains[1] = "#FFF94C";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F1F2B5";
            gradientSeed.contains[1] = "#135058";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#D1913C";
            gradientSeed.contains[1] = "#FFD194";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#7b4397";
            gradientSeed.contains[1] = "#dc2430";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#8e9eab";
            gradientSeed.contains[1] = "#eef2f3";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#136a8a";
            gradientSeed.contains[1] = "#267871";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00bf8f";
            gradientSeed.contains[1] = "#001510";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ff0084";
            gradientSeed.contains[1] = "#33001b";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#833ab4";
            gradientSeed.contains[1] = "#fcb045";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FEAC5E";
            gradientSeed.contains[1] = "#C779D0";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#C779D0";
            gradientSeed.contains[1] = "#fcb045";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ffb347";
            gradientSeed.contains[1] = "#ffcc33";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#43cea2";
            gradientSeed.contains[1] = "#185a9d";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFA17F";
            gradientSeed.contains[1] = "#00223E";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#948E99";
            gradientSeed.contains[1] = "#2E1437";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#D38312";
            gradientSeed.contains[1] = "#A83279";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#73C8A9";
            gradientSeed.contains[1] = "#373B44";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#abbaab";
            gradientSeed.contains[1] = "#ffffff";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#fdfc47";
            gradientSeed.contains[1] = "#24fe41";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#83a4d4";
            gradientSeed.contains[1] = "#b6fbff";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#52c234";
            gradientSeed.contains[1] = "#061700";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#fe8c00";
            gradientSeed.contains[1] = "#f83600";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00c6ff";
            gradientSeed.contains[1] = "#0072ff";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#70e1f5";
            gradientSeed.contains[1] = "#ffd194";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#556270";
            gradientSeed.contains[1] = "#ff6b6b";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#9d50bb";
            gradientSeed.contains[1] = "#6e48aa";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#b3ffab";
            gradientSeed.contains[1] = "#12fff7";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#aaffa9";
            gradientSeed.contains[1] = "#11ffbd";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#f0c27b";
            gradientSeed.contains[1] = "#4b1248";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ff4e50";
            gradientSeed.contains[1] = "#f9d423";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#fbd3e9";
            gradientSeed.contains[1] = "#bb377d";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#c9ffbf";
            gradientSeed.contains[1] = "#ffafbd";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#649173";
            gradientSeed.contains[1] = "#dbd5a4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#b993d6";
            gradientSeed.contains[1] = "#8ca6db";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00d2ff";
            gradientSeed.contains[1] = "#3a7bd5";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#d395gb";
            gradientSeed.contains[1] = "#bfe6ba";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#dad299";
            gradientSeed.contains[1] = "#b0dab9";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#f2709c";
            gradientSeed.contains[1] = "#ff9472";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#e6dada";
            gradientSeed.contains[1] = "#274046";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#5d4157";
            gradientSeed.contains[1] = "#a8caba";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ddd6f3";
            gradientSeed.contains[1] = "#faaca8";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#616161";
            gradientSeed.contains[1] = "#9bc5c3";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#50c9c3";
            gradientSeed.contains[1] = "#96deda";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#c21500";
            gradientSeed.contains[1] = "#ffc500";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#efefbb";
            gradientSeed.contains[1] = "#d4d3dd";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ffeeee";
            gradientSeed.contains[1] = "#ddefbb";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#73C8A9";
            gradientSeed.contains[1] = "#373B44";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#de6262";
            gradientSeed.contains[1] = "#ffb88c";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#d53369";
            gradientSeed.contains[1] = "#cbad6d";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#f857a6";
            gradientSeed.contains[1] = "#ff5858";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#fc354c";
            gradientSeed.contains[1] = "#0abfbc";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#e43a15";
            gradientSeed.contains[1] = "#e65245";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#c04848";
            gradientSeed.contains[1] = "#480048";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#5f2c82";
            gradientSeed.contains[1] = "#49a09d";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ec6f66";
            gradientSeed.contains[1] = "#f3a183";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#7474bf";
            gradientSeed.contains[1] = "#348ac7";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ece9e6";
            gradientSeed.contains[1] = "#ffffff";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "dae2f8";
            gradientSeed.contains[1] = "#d6a4a4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ed4264";
            gradientSeed.contains[1] = "#ffedbc";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#24c6dc";
            gradientSeed.contains[1] = "#514a9d";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#283048";
            gradientSeed.contains[1] = "#859398";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#3d7eaa";
            gradientSeed.contains[1] = "#ffe47a";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#1cd8d2";
            gradientSeed.contains[1] = "#93edc7";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#757f9a";
            gradientSeed.contains[1] = "#d7dde8";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#5c258d";
            gradientSeed.contains[1] = "#4389a2";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#134e5e";
            gradientSeed.contains[1] = "#71b280";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#2BC0E4";
            gradientSeed.contains[1] = "#EAECC6";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#085078";
            gradientSeed.contains[1] = "#85D8CE";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#4776E6";
            gradientSeed.contains[1] = "#8E54E9";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#614385";
            gradientSeed.contains[1] = "#516395";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#1F1C2C";
            gradientSeed.contains[1] = "#928DAB";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FF8008";
            gradientSeed.contains[1] = "#FFC837";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#1D976C";
            gradientSeed.contains[1] = "#93F9B9";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#EB3349";
            gradientSeed.contains[1] = "#F45C43";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#DD5E89";
            gradientSeed.contains[1] = "#F7BB97";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#4CB8C4";
            gradientSeed.contains[1] = "#3CD3AD";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#1FA2FF";
            gradientSeed.contains[1] = "#A6FFCB";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#1D2B64";
            gradientSeed.contains[1] = "#F8CDDA";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FF512F";
            gradientSeed.contains[1] = "#F09819";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#1A2980";
            gradientSeed.contains[1] = "#26D0CE";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#AA076B";
            gradientSeed.contains[1] = "#61045F";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FF512F";
            gradientSeed.contains[1] = "#DD2476";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F09819";
            gradientSeed.contains[1] = "#EDDE5D";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#403B4A";
            gradientSeed.contains[1] = "#E7E9BB";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#E55D87";
            gradientSeed.contains[1] = "#5FC3E4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#003973";
            gradientSeed.contains[1] = "#E5E5BE";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#CC95C0";
            gradientSeed.contains[1] = "#DBD4B4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#DBD4B4";
            gradientSeed.contains[1] = "#7AA1D2";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#3CA55C";
            gradientSeed.contains[1] = "#B5AC49";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#348F50";
            gradientSeed.contains[1] = "#56B4D3";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#DA22FF";
            gradientSeed.contains[1] = "#9733EE";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#02AAB0";
            gradientSeed.contains[1] = "#00CDAC";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#EDE574";
            gradientSeed.contains[1] = "#E1F5C4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#D31027";
            gradientSeed.contains[1] = "#EA384D";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#16A085";
            gradientSeed.contains[1] = "#F4D03F";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#603813";
            gradientSeed.contains[1] = "#b29f94";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#e52d27";
            gradientSeed.contains[1] = "#b31217";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ff6e7f";
            gradientSeed.contains[1] = "#bfe9ff";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#77A1D3";
            gradientSeed.contains[1] = "#79CBCA";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#79CBCA";
            gradientSeed.contains[1] = "#E684AE";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#2b5876";
            gradientSeed.contains[1] = "#4e4376";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#e65c00";
            gradientSeed.contains[1] = "#F9D423";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#2193b0";
            gradientSeed.contains[1] = "#6dd5ed";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#cc2b5e";
            gradientSeed.contains[1] = "#753a88";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ec008c";
            gradientSeed.contains[1] = "#fc6767";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#1488CC";
            gradientSeed.contains[1] = "#2B32B2";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00467F";
            gradientSeed.contains[1] = "#A5CC82";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#076585";
            gradientSeed.contains[1] = "#ffffff";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#BBD2C5";
            gradientSeed.contains[1] = "#536976";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#9796f0";
            gradientSeed.contains[1] = "#fbc7d4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#B79891";
            gradientSeed.contains[1] = "#94716B";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#BBD2C5";
            gradientSeed.contains[1] = "#536976";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#536976";
            gradientSeed.contains[1] = "#292E49";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#536976";
            gradientSeed.contains[1] = "#292E49";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#acb6e5";
            gradientSeed.contains[1] = "#86fde8";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFE000";
            gradientSeed.contains[1] = "#799F0C";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00416A";
            gradientSeed.contains[1] = "#E4E5E6";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ffe259";
            gradientSeed.contains[1] = "#ffa751";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#799F0C";
            gradientSeed.contains[1] = "#ACBB78";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#5433FF";
            gradientSeed.contains[1] = "#20BDFF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#20BDFF";
            gradientSeed.contains[1] = "#A5FECB";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#334d50";
            gradientSeed.contains[1] = "#cbcaa5";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00416A";
            gradientSeed.contains[1] = "#799F0C";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#799F0C";
            gradientSeed.contains[1] = "#FFE000";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F7F8F8";
            gradientSeed.contains[1] = "#ACBB78";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFE000";
            gradientSeed.contains[1] = "#799F0C";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FDEB71";
            gradientSeed.contains[1] = "#F8D800";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#ABDCFF";
            gradientSeed.contains[1] = "#0396FF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FEB692";
            gradientSeed.contains[1] = "#EA5455";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#CE9FFC";
            gradientSeed.contains[1] = "#7367F0";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#90F7EC";
            gradientSeed.contains[1] = "#32CCBC";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFF6B7";
            gradientSeed.contains[1] = "#F6416C";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#81FBB8";
            gradientSeed.contains[1] = "#28C76F";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#E2B0FF";
            gradientSeed.contains[1] = "#9F44D3";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F97794";
            gradientSeed.contains[1] = "#623AA2";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FCCF31";
            gradientSeed.contains[1] = "#F55555";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F761A1";
            gradientSeed.contains[1] = "#8C1BAB";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#43CBFF";
            gradientSeed.contains[1] = "#9708CC";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#5EFCE8";
            gradientSeed.contains[1] = "#736EFE";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FAD7A1";
            gradientSeed.contains[1] = "#E96D71";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFD26F";
            gradientSeed.contains[1] = "#3677FF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#A0FE65";
            gradientSeed.contains[1] = "#FA016D";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFDB01";
            gradientSeed.contains[1] = "#0E197D";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FEC163";
            gradientSeed.contains[1] = "#DE4313";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#92FFC0";
            gradientSeed.contains[1] = "#002661";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#EEAD92";
            gradientSeed.contains[1] = "#6018DC";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F6CEEC";
            gradientSeed.contains[1] = "#D939CD";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#52E5E7";
            gradientSeed.contains[1] = "#130CB7";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F1CA74";
            gradientSeed.contains[1] = "#A64DB6";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#E8D07A";
            gradientSeed.contains[1] = "#5312D6";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#EECE13";
            gradientSeed.contains[1] = "#B210FF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#79F1A4";
            gradientSeed.contains[1] = "#0E5CAD";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FDD819";
            gradientSeed.contains[1] = "#E80505";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFF3B0";
            gradientSeed.contains[1] = "#CA26FF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFF5C3";
            gradientSeed.contains[1] = "#9452A5";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F05F57";
            gradientSeed.contains[1] = "#360940";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#2AFADF";
            gradientSeed.contains[1] = "#4C83FF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFF886";
            gradientSeed.contains[1] = "#F072B6";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#97ABFF";
            gradientSeed.contains[1] = "#123597";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F5CBFF";
            gradientSeed.contains[1] = "#C346C2";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFF720";
            gradientSeed.contains[1] = "#3CD500";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FF6FD8";
            gradientSeed.contains[1] = "#3813C2";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#EE9AE5";
            gradientSeed.contains[1] = "#5961F9";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFD3A5";
            gradientSeed.contains[1] = "#FD6585";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#C2FFD8";
            gradientSeed.contains[1] = "#465EFB";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FD6585";
            gradientSeed.contains[1] = "#0D25B9";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FD6E6A";
            gradientSeed.contains[1] = "#FFC600";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#65FDF0";
            gradientSeed.contains[1] = "#1D6FA3";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#6B73FF";
            gradientSeed.contains[1] = "#000DFF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FF7AF5";
            gradientSeed.contains[1] = "#513162";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F0FF00";
            gradientSeed.contains[1] = "#58CFFB";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFE985";
            gradientSeed.contains[1] = "#FA742B";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFA6B7";
            gradientSeed.contains[1] = "#1E2AD2";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFAA85";
            gradientSeed.contains[1] = "#B3315F";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#72EDF2";
            gradientSeed.contains[1] = "#5151E5";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FF9D6C";
            gradientSeed.contains[1] = "#BB4E75";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#F6D242";
            gradientSeed.contains[1] = "#FF52E5";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#69FF97";
            gradientSeed.contains[1] = "#00E4FF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#3B2667";
            gradientSeed.contains[1] = "#BC78EC";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#70F570";
            gradientSeed.contains[1] = "#49C628";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#3C8CE7";
            gradientSeed.contains[1] = "#00EAFF";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FAB2FF";
            gradientSeed.contains[1] = "#1904E5";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#81FFEF";
            gradientSeed.contains[1] = "#F067B4";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFA8A8";
            gradientSeed.contains[1] = "#FCFF00";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FFCF71";
            gradientSeed.contains[1] = "#2376DD";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#FF96F9";
            gradientSeed.contains[1] = "#C32BAC";
            this.gradientSeeds.Add(gradientSeed);
            gradientSeed.name = "颜色";
            gradientSeed.contains = new string[2];
            gradientSeed.contains[0] = "#00416A";
            gradientSeed.contains[1] = "#E4E5E6";
            this.gradientSeeds.Add(gradientSeed);
        }

        // Token: 0x060051D9 RID: 20953 RVA: 0x00145934 File Offset: 0x00143B34
        private void GradientSeedsAwake2()
        {
            this.txtlength.Clear();
            string text = ColorfulSpeek.Func_24(ColorfulSpeek.Func_25(), "\\Human_Data\\Managed\\渐变\\");
            string text2 = text;
            if (!ColorfulSpeek.Func_26(text2) && !ColorfulSpeek.Func_27(text2))
            {
                ColorfulSpeek.Func_28(text2);
            }
            int num = 0;
            FileInfo[] array = ColorfulSpeek.Func_30(ColorfulSpeek.Func_29(text2));
            for (int i = 0; i < array.Length; i++)
            {
                text2 = ColorfulSpeek.Func_31(array[i]);
                if (ColorfulSpeek.Func_32(text2, ".txt"))
                {
                    num++;
                    this.txtlength.Add(ColorfulSpeek.Func_33(array[i]));
                    foreach (string text3 in ColorfulSpeek.Func_34(text2))
                    {
                        if (!ColorfulSpeek.Func__(text3) && !ColorfulSpeek.Func_35(text3, "//"))
                        {
                            string[] array3 = ColorfulSpeek.Func_00(ColorfulSpeek.Func_18(ColorfulSpeek.Func_18(text3, "#", ""), "\t", " "), new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            ColorfulSpeek.GradientSeed gradientSeed = default(ColorfulSpeek.GradientSeed);
                            if (array3.Length > 2 && array3.Length < 7)
                            {
                                gradientSeed.name = array3[0];
                                gradientSeed.contains = new string[array3.Length - 1];
                                for (int k = 0; k < gradientSeed.contains.Length; k++)
                                {
                                    gradientSeed.contains[k] = array3[k + 1];
                                }
                                gradientSeed.isOnlyList = false;
                                this.gradientSeeds.Add(gradientSeed);
                            }
                            else if (array3.Length >= 7)
                            {
                                gradientSeed = default(ColorfulSpeek.GradientSeed);
                                gradientSeed.name = array3[0];
                                gradientSeed.contains = new string[array3.Length - 1];
                                gradientSeed.list = new string[array3.Length - 1];
                                for (int l = 0; l < gradientSeed.contains.Length; l++)
                                {
                                    gradientSeed.contains[l] = array3[l + 1];
                                    gradientSeed.list[l] = ColorfulSpeek.Func_24("#", array3[l + 1]);
                                }
                                gradientSeed.isOnlyList = true;
                                this.gradientSeeds.Add(gradientSeed);
                            }
                        }
                    }
                }
            }
            if (num == 0)
            {
                ColorfulSpeek.Func_36(ColorfulSpeek.Func_24(text, "渐变存数据.txt"), this.渐变存数据);
                this.GradientSeedsAwake2();
            }
        }

        // Token: 0x060051DA RID: 20954 RVA: 0x00145B74 File Offset: 0x00143D74
        public static Gradient GetGradient(Color[] colors)
        {
            if (colors != null && colors.Length >= 2)
            {
                Gradient gradient = ColorfulSpeek.Func_14();
                GradientColorKey[] array = new GradientColorKey[colors.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].color = colors[i];
                    array[i].time = (float)i / (float)(array.Length - 1);
                }
                GradientAlphaKey[] array2 = new GradientAlphaKey[colors.Length];
                array2[0].alpha = 1f;
                array2[0].time = 1f;
                for (int j = 1; j < array2.Length; j++)
                {
                    array2[j].alpha = 0f;
                    array2[j].time = 1f;
                }
                ColorfulSpeek.Func_15(gradient, array, array2);
                return gradient;
            }
            return null;
        }

        // Token: 0x060051DB RID: 20955 RVA: 0x00145C44 File Offset: 0x00143E44
        private void Awake2()
        {
            this.GradientSeedsAwake();
            this.GradientSeedsAwake2();
            if (ColorfulSpeek.Name_ColorType > 0)
            {
                if (ColorfulSpeek.Name_ColorType >= this.gradientSeeds.Count)
                {
                    ColorfulSpeek.Name_ColorType = 1;
                }
                ColorfulSpeek.NameSeed = this.GetSeedByIndex(ColorfulSpeek.Name_ColorType, 0f, 1f);
            }
            if (ColorfulSpeek.Words_ColorType >= this.gradientSeeds.Count)
            {
                ColorfulSpeek.Words_ColorType = 1;
            }
            ColorfulSpeek.WordSeed = this.GetSeedByIndex(ColorfulSpeek.Words_ColorType, 0f, 1f);
        }

        // Token: 0x060051DC RID: 20956 RVA: 0x00145CCC File Offset: 0x00143ECC
        private void Update()
        {
            if (ColorfulSpeek.Func_37() % 120 != 0)
            {
                return;
            }
            string text = ColorfulSpeek.Func_24(ColorfulSpeek.Func_25(), "\\Human_Data\\Managed\\渐变\\");
            if (ColorfulSpeek.Func_26(text))
            {
                FileInfo[] array = ColorfulSpeek.Func_30(ColorfulSpeek.Func_29(text));
                for (int i = 0; i < array.Length; i++)
                {
                    if (ColorfulSpeek.Func_32(ColorfulSpeek.Func_31(array[i]), ".txt") && (i >= this.txtlength.Count || this.txtlength[i] != ColorfulSpeek.Func_33(array[i])))
                    {
                        this.Awake2();
                        return;
                    }
                }
                return;
            }
        }

        // Token: 0x060051DD RID: 20957 RVA: 0x00145D58 File Offset: 0x00143F58
        public void OutPut_to_txt(string txt, string path)
        {
            if (!ColorfulSpeek.Func_26(path) && !ColorfulSpeek.Func_27(path))
            {
                ColorfulSpeek.Func_28(path);
            }
            string[] array = null;
            try
            {
                array = ColorfulSpeek.Func_34(ColorfulSpeek.Func_24(path, "渐变输出.txt"));
            }
            catch
            {
                array = null;
            }
            List<string> list = new List<string>();
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    list.Add(array[i]);
                }
            }
            list.Add(ColorfulSpeek.Func_10("{0}\t渐变输出:", DateTime.Now));
            list.Add(txt);
            list.Add("\t");
            ColorfulSpeek.Func_36(ColorfulSpeek.Func_24(path, "渐变输出.txt"), list.ToArray());
        }

        // Token: 0x060051DE RID: 20958 RVA: 0x00145E08 File Offset: 0x00144008
        public ColorfulSpeek.GradientSeed SeedInit(int index, float begin = 0f, float end = 1f)
        {
            if (index >= this.gradientSeeds.Count)
            {
                ColorfulSpeek.Func_16("out of seeds range");
                return default(ColorfulSpeek.GradientSeed);
            }
            ColorfulSpeek.GradientSeed gradientSeed = this.gradientSeeds[index];
            if (!gradientSeed.isOnlyList)
            {
                if (gradientSeed.gradient == null && gradientSeed.contains.Length > 1 && gradientSeed.contains.Length < 6)
                {
                    for (int i = 0; i < gradientSeed.contains.Length; i++)
                    {
                        if (ColorfulSpeek.Func_17(gradientSeed.contains[i], "#"))
                        {
                            gradientSeed.contains[i] = ColorfulSpeek.Func_18(gradientSeed.contains[i], "#", "");
                        }
                    }
                    Color[] array = new Color[gradientSeed.contains.Length];
                    for (int j = 0; j < array.Length; j++)
                    {
                        array[j] = HexConverter.HexToColor(gradientSeed.contains[j]);
                    }
                    gradientSeed.gradient = ColorfulSpeek.GetGradient(array);
                }
                List<string> list = new List<string>();
                int num = 100;
                for (int k = 0; k < num; k++)
                {
                    float num2;
                    if (begin == end)
                    {
                        num2 = begin;
                    }
                    else
                    {
                        num2 = begin + (float)(k + 1) / (float)num * (end - begin);
                    }
                    Color color = ColorfulSpeek.Func_20(gradientSeed.gradient, num2);
                    list.Add(ColorfulSpeek.Func_24("#", HexConverter.ColorToHex(color)));
                }
                gradientSeed.list = list.ToArray();
                if (ColorfulSpeek.Func_38(gradientSeed.texture, null))
                {
                    gradientSeed.texture = ColorfulSpeek.Func_01(ColorfulSpeek.Func_19(ColorfulSpeek.EmptyTexture), ColorfulSpeek.Func_22(ColorfulSpeek.EmptyTexture), TextureFormat.ARGB32, false);
                    int num3 = ColorfulSpeek.Func_19(gradientSeed.texture);
                    for (int l = 0; l < num3; l++)
                    {
                        float num4 = (float)(l + 1) / (float)num3;
                        for (int m = 0; m < ColorfulSpeek.Func_22(gradientSeed.texture); m++)
                        {
                            Color color2 = ColorfulSpeek.Func_20(gradientSeed.gradient, num4);
                            color2 = color2.Brighten(0.3f);
                            ColorfulSpeek.Func_21(gradientSeed.texture, l, m, color2);
                        }
                    }
                    ColorfulSpeek.Func_23(gradientSeed.texture, false);
                }
                this.gradientSeeds[index] = gradientSeed;
                return gradientSeed;
            }
            if (ColorfulSpeek.Func_38(gradientSeed.texture, null))
            {
                gradientSeed.texture = ColorfulSpeek.Func_01(ColorfulSpeek.Func_19(ColorfulSpeek.EmptyTexture), ColorfulSpeek.Func_22(ColorfulSpeek.EmptyTexture), TextureFormat.ARGB32, false);
                int num5 = ColorfulSpeek.Func_19(gradientSeed.texture);
                int num6 = (int)((float)num5 / (float)gradientSeed.contains.Length + 0.5f);
                for (int n = 0; n < num5; n++)
                {
                    for (int num7 = 0; num7 < ColorfulSpeek.Func_22(gradientSeed.texture); num7++)
                    {
                        int num8 = (int)((float)(n + 1) / (float)num6);
                        if (num8 >= gradientSeed.contains.Length)
                        {
                            num8 = gradientSeed.contains.Length - 1;
                        }
                        Color color3 = HexConverter.HexToColor(gradientSeed.contains[num8]);
                        color3 = color3.Brighten(0.3f);
                        ColorfulSpeek.Func_21(gradientSeed.texture, n, num7, color3);
                    }
                }
                ColorfulSpeek.Func_23(gradientSeed.texture, false);
            }
            this.gradientSeeds[index] = gradientSeed;
            return gradientSeed;
        }

        // Token: 0x060051DF RID: 20959 RVA: 0x0014612C File Offset: 0x0014432C
        public void OutPut_to_PNG(Texture2D texture, string path)
        {
            if (!ColorfulSpeek.Func_38(texture, null))
            {
                if (!ColorfulSpeek.Func_26(path) && !ColorfulSpeek.Func_27(path))
                {
                    ColorfulSpeek.Func_28(path);
                }
                byte[] array = ColorfulSpeek.Func_39(texture);
                ColorfulSpeek.Func_40(ColorfulSpeek.Func_24(path, "1.png"), array);
                return;
            }
        }

        // Token: 0x060051E0 RID: 20960 RVA: 0x00146174 File Offset: 0x00144374
        public void OutPut_to_PNG(ColorfulSpeek.GradientSeed seed, string path)
        {
            Texture2D texture2D;
            if (!seed.isOnlyList)
            {
                texture2D = ColorfulSpeek.Func_01(ColorfulSpeek.Func_41(), ColorfulSpeek.Func_42(), TextureFormat.ARGB32, false);
                int num = ColorfulSpeek.Func_19(texture2D);
                for (int i = 0; i < num; i++)
                {
                    float num2 = (float)(i + 1) / (float)num;
                    for (int j = 0; j < ColorfulSpeek.Func_22(texture2D); j++)
                    {
                        Color color = ColorfulSpeek.Func_20(seed.gradient, num2);
                        ColorfulSpeek.Func_21(texture2D, i, j, color);
                    }
                }
                ColorfulSpeek.Func_23(texture2D, false);
            }
            else
            {
                texture2D = ColorfulSpeek.Func_01(ColorfulSpeek.Func_41(), ColorfulSpeek.Func_42(), TextureFormat.ARGB32, false);
                int num3 = ColorfulSpeek.Func_19(texture2D);
                int num4 = (int)((float)num3 / (float)seed.contains.Length + 0.5f);
                for (int k = 0; k < num3; k++)
                {
                    for (int l = 0; l < ColorfulSpeek.Func_22(texture2D); l++)
                    {
                        int num5 = (int)((float)(k + 1) / (float)num4);
                        if (num5 >= seed.contains.Length)
                        {
                            num5 = seed.contains.Length - 1;
                        }
                        Color color2 = HexConverter.HexToColor(seed.contains[num5]);
                        ColorfulSpeek.Func_21(texture2D, k, l, color2);
                    }
                }
                ColorfulSpeek.Func_23(texture2D, false);
            }
            if (!ColorfulSpeek.Func_26(path) && !ColorfulSpeek.Func_27(path))
            {
                ColorfulSpeek.Func_28(path);
            }
            byte[] array = ColorfulSpeek.Func_39(texture2D);
            ColorfulSpeek.Func_40(ColorfulSpeek.Func_43(path, seed.name, ".png"), array);
        }

        // Token: 0x060051E1 RID: 20961 RVA: 0x0002562C File Offset: 0x0002382C
        public ColorfulSpeek.GradientSeed GetSeedByIndex(int index, float begin = 0f, float end = 1f)
        {
            return this.SeedInit(index, begin, end);
        }

        // Token: 0x060051E2 RID: 20962 RVA: 0x00012D3F File Offset: 0x00010F3F
        public string HangUpSymbol(Human human)
        {
            return null;
        }

        // Token: 0x060051E3 RID: 20963 RVA: 0x001462CC File Offset: 0x001444CC
        public static string colorshowES(string msg)
        {
            if (!ColorfulSpeek.Gradual_Change)
            {
                return msg;
            }
            if (ColorfulSpeek.Gradual_Random)
            {
                int num = UnityEngine.Random.Range(0, ColorfulSpeek.ins.gradientSeeds.Count - 1);
                ColorfulSpeek.WordSeed = ColorfulSpeek.ins.GetSeedByIndex(num, 0f, 1f);
            }
            string[] list = ColorfulSpeek.WordSeed.list;
            int num2 = ColorfulSpeek.Size_Words;
            num2 = 20;
            string text = "";
            if (ColorfulSpeek.Func_05(msg) >= list.Length - 2)
            {
                for (int i = 0; i < msg.Length; i++)
                {
                    string text2 = string.Format("<size={2}><color={0}>{1}</color></size>", list[i % list.Length], ColorfulSpeek.Func_07(msg, i).ToString(), num2);
                    text += text2;
                }
            }
            else
            {
                int num3 = list.Length / msg.Length;
                int num4 = 0;
                int num5 = 0;
                while (num5 < list.Length && num4 < msg.Length)
                {
                    string text3 = string.Format("<size={2}><color={0}>{1}</color></size>", list[num5], msg[num4].ToString(), num2);
                    text += text3;
                    num5 += num3;
                    num4++;
                }
            }
            if (ColorfulSpeek.Words_Bold)
            {
                text = "<b>" + text + "</b>";
            }
            if (ColorfulSpeek.Words_Italic)
            {
                text = "<i>" + text + "</i>";
            }
            return text;
        }

        // Token: 0x060051E4 RID: 20964 RVA: 0x00146420 File Offset: 0x00144620
        public static string colorshowds(string msg)
        {
            if (ColorfulSpeek.Gradual_Change)
            {
                string[] list = ColorfulSpeek.WordSeed.list;
                string text = "";
                int num = ColorfulSpeek.Size_Words;
                num = 30;
                if (ColorfulSpeek.Func_05(msg) < list.Length - 2)
                {
                    int num2 = list.Length / ColorfulSpeek.Func_05(msg);
                    int num3 = 0;
                    int i = 0;
                    while (i < list.Length)
                    {
                        if (num3 >= ColorfulSpeek.Func_05(msg))
                        {
                            break;
                        }
                        string text2 = string.Format("<size={2}><color={0}>{1}</color></size>", list[i], ColorfulSpeek.Func_07(msg, num3).ToString(), num);
                        text += text2;
                        i += num2;
                        num3++;
                    }
                }
                else
                {
                    for (int j = 0; j < msg.Length; j++)
                    {
                        string text3 = string.Format("<size={2}><color={0}>{1}</color></size>", list[j % list.Length], msg[j].ToString(), num);
                        text += text3;
                    }
                }
                if (ColorfulSpeek.Words_Bold)
                {
                    text = "<b>" + text + "</b>";
                }
                if (ColorfulSpeek.Words_Italic)
                {
                    text = "<i>" + text + "</i>";
                }
                return text;
            }
            return msg;
        }

        // Token: 0x060051E5 RID: 20965 RVA: 0x00146540 File Offset: 0x00144740
        public void Chatrecord(NetHost client, string nick, string msg)
        {
            nick = ColorfulSpeek.Func_46(ColorfulSpeek.Func_45(ColorfulSpeek.Func_44("<(.|\n)+?>"), nick, ""), "[^a-zA-Z0-9\\u4e00-\\u9fa5\\s]", "");
            string text = string.Format("{0}", this.Chat_note + DateTime.Now.ToString(". dd日 HH:mm:ss "));
            if (!(this.Chat_roomname != NetGame.instance.server.name) && this.Chat_roomname != null)
            {
                this.Chat_note++;
                this.Chat_record = string.Concat(new string[] { this.Chat_record, text, nick, "  ", msg, "\n" });
                return;
            }
            this.Chat_note++;
            this.Chat_roomnamebar++;
            this.Chat_roomname = NetGame.instance.server.name;
            this.Chat_record = string.Concat(new string[] { this.Chat_record, "<size=20><color=#B57EE0>房间名：", this.Chat_roomname, "</color></size>\n", text, nick, "  ", msg, "\n" });
        }

        // Token: 0x060051E6 RID: 20966 RVA: 0x0000458A File Offset: 0x0000278A
        static bool Func__(string A_0)
        {
            return string.IsNullOrEmpty(A_0);
        }

        // Token: 0x060051E7 RID: 20967 RVA: 0x0000CC8C File Offset: 0x0000AE8C
        static string[] Func_00(string A_0, char[] A_1, StringSplitOptions A_2)
        {
            return A_0.Split(A_1, A_2);
        }

        // Token: 0x060051E8 RID: 20968 RVA: 0x00008041 File Offset: 0x00006241
        static Texture2D Func_01(int A_0, int A_1, TextureFormat A_2, bool A_3)
        {
            return new Texture2D(A_0, A_1, A_2, A_3);
        }

        // Token: 0x060051E9 RID: 20969 RVA: 0x00003591 File Offset: 0x00001791
        static void Func_02(string A_0, string A_1)
        {
            PlayerPrefs.SetString(A_0, A_1);
        }

        // Token: 0x060051EA RID: 20970 RVA: 0x0000CC83 File Offset: 0x0000AE83
        static void Func_03(string A_0, int A_1)
        {
            PlayerPrefs.SetInt(A_0, A_1);
        }

        // Token: 0x060051EB RID: 20971 RVA: 0x0000FA47 File Offset: 0x0000DC47
        static global::System.Random Func_04()
        {
            return new global::System.Random();
        }

        // Token: 0x060051EC RID: 20972 RVA: 0x000033D3 File Offset: 0x000015D3
        static int Func_05(string A_0)
        {
            return A_0.Length;
        }

        // Token: 0x060051ED RID: 20973 RVA: 0x000255B7 File Offset: 0x000237B7
        static int Func_06(global::System.Random A_0, int A_1, int A_2)
        {
            return A_0.Next(A_1, A_2);
        }

        // Token: 0x060051EE RID: 20974 RVA: 0x0000840F File Offset: 0x0000660F
        static char Func_07(string A_0, int A_1)
        {
            return A_0[A_1];
        }

        // Token: 0x060051EF RID: 20975 RVA: 0x000181B9 File Offset: 0x000163B9
        static CSteamID Func_08()
        {
            return SteamUser.GetSteamID();
        }

        // Token: 0x060051F0 RID: 20976 RVA: 0x00018FEF File Offset: 0x000171EF
        static string Func_09(CSteamID A_0)
        {
            return SteamFriends.GetFriendPersonaName(A_0);
        }

        // Token: 0x060051F1 RID: 20977 RVA: 0x000089A8 File Offset: 0x00006BA8
        static string Func_10(string A_0, object A_1)
        {
            return string.Format(A_0, A_1);
        }

        // Token: 0x060051F2 RID: 20978 RVA: 0x00014D81 File Offset: 0x00012F81
        static string Func_11(string A_0, string A_1)
        {
            return PlayerPrefs.GetString(A_0, A_1);
        }

        // Token: 0x060051F3 RID: 20979 RVA: 0x0000CC7A File Offset: 0x0000AE7A
        static int Func_12(string A_0, int A_1)
        {
            return PlayerPrefs.GetInt(A_0, A_1);
        }

        // Token: 0x060051F4 RID: 20980 RVA: 0x000098E1 File Offset: 0x00007AE1
        static float Func_13(float A_0, float A_1)
        {
            return Math.Min(A_0, A_1);
        }

        // Token: 0x060051F5 RID: 20981 RVA: 0x00025637 File Offset: 0x00023837
        static Gradient Func_14()
        {
            return new Gradient();
        }

        // Token: 0x060051F6 RID: 20982 RVA: 0x0002563E File Offset: 0x0002383E
        static void Func_15(Gradient A_0, GradientColorKey[] A_1, GradientAlphaKey[] A_2)
        {
            A_0.SetKeys(A_1, A_2);
        }

        // Token: 0x060051F7 RID: 20983 RVA: 0x00002672 File Offset: 0x00000872
        static void Func_16(object A_0)
        {
            MonoBehaviour.print(A_0);
        }

        // Token: 0x060051F8 RID: 20984 RVA: 0x00004F70 File Offset: 0x00003170
        static bool Func_17(string A_0, string A_1)
        {
            return A_0.Contains(A_1);
        }

        // Token: 0x060051F9 RID: 20985 RVA: 0x000083EB File Offset: 0x000065EB
        static string Func_18(string A_0, string A_1, string A_2)
        {
            return A_0.Replace(A_1, A_2);
        }

        // Token: 0x060051FA RID: 20986 RVA: 0x0000949C File Offset: 0x0000769C
        static int Func_19(Texture A_0)
        {
            return A_0.width;
        }

        // Token: 0x060051FB RID: 20987 RVA: 0x00025648 File Offset: 0x00023848
        static Color Func_20(Gradient A_0, float A_1)
        {
            return A_0.Evaluate(A_1);
        }

        // Token: 0x060051FC RID: 20988 RVA: 0x0000AB80 File Offset: 0x00008D80
        static void Func_21(Texture2D A_0, int A_1, int A_2, Color A_3)
        {
            A_0.SetPixel(A_1, A_2, A_3);
        }

        // Token: 0x060051FD RID: 20989 RVA: 0x000094BF File Offset: 0x000076BF
        static int Func_22(Texture A_0)
        {
            return A_0.height;
        }

        // Token: 0x060051FE RID: 20990 RVA: 0x0000F274 File Offset: 0x0000D474
        static void Func_23(Texture2D A_0, bool A_1)
        {
            A_0.Apply(A_1);
        }

        // Token: 0x060051FF RID: 20991 RVA: 0x0000249E File Offset: 0x0000069E
        static string Func_24(string A_0, string A_1)
        {
            return A_0 + A_1;
        }

        // Token: 0x06005200 RID: 20992 RVA: 0x00025651 File Offset: 0x00023851
        static string Func_25()
        {
            return Environment.CurrentDirectory;
        }

        // Token: 0x06005201 RID: 20993 RVA: 0x0001C0AC File Offset: 0x0001A2AC
        static bool Func_26(string A_0)
        {
            return Directory.Exists(A_0);
        }

        // Token: 0x06005202 RID: 20994 RVA: 0x000156F7 File Offset: 0x000138F7
        static bool Func_27(string A_0)
        {
            return File.Exists(A_0);
        }

        // Token: 0x06005203 RID: 20995 RVA: 0x0001C0B4 File Offset: 0x0001A2B4
        static DirectoryInfo Func_28(string A_0)
        {
            return Directory.CreateDirectory(A_0);
        }

        // Token: 0x06005204 RID: 20996 RVA: 0x000156DF File Offset: 0x000138DF
        static DirectoryInfo Func_29(string A_0)
        {
            return new DirectoryInfo(A_0);
        }

        // Token: 0x06005205 RID: 20997 RVA: 0x00025658 File Offset: 0x00023858
        static FileInfo[] Func_30(DirectoryInfo A_0)
        {
            return A_0.GetFiles();
        }

        // Token: 0x06005206 RID: 20998 RVA: 0x000156EF File Offset: 0x000138EF
        static string Func_31(FileSystemInfo A_0)
        {
            return A_0.FullName;
        }

        // Token: 0x06005207 RID: 20999 RVA: 0x0000C33D File Offset: 0x0000A53D
        static bool Func_32(string A_0, string A_1)
        {
            return A_0.EndsWith(A_1);
        }

        // Token: 0x06005208 RID: 21000 RVA: 0x00025660 File Offset: 0x00023860
        static long Func_33(FileInfo A_0)
        {
            return A_0.Length;
        }

        // Token: 0x06005209 RID: 21001 RVA: 0x000156FF File Offset: 0x000138FF
        static string[] Func_34(string A_0)
        {
            return File.ReadAllLines(A_0);
        }

        // Token: 0x0600520A RID: 21002 RVA: 0x0000CE75 File Offset: 0x0000B075
        static bool Func_35(string A_0, string A_1)
        {
            return A_0.StartsWith(A_1);
        }

        // Token: 0x0600520B RID: 21003 RVA: 0x00025668 File Offset: 0x00023868
        static void Func_36(string A_0, string[] A_1)
        {
            File.WriteAllLines(A_0, A_1);
        }

        // Token: 0x0600520C RID: 21004 RVA: 0x00008634 File Offset: 0x00006834
        static int Func_37()
        {
            return Time.frameCount;
        }

        // Token: 0x0600520D RID: 21005 RVA: 0x000024AF File Offset: 0x000006AF
        static bool Func_38(global::UnityEngine.Object A_0, global::UnityEngine.Object A_1)
        {
            return A_0 == A_1;
        }

        // Token: 0x0600520E RID: 21006 RVA: 0x0000DEB9 File Offset: 0x0000C0B9
        static byte[] Func_39(Texture2D A_0)
        {
            return A_0.EncodeToPNG();
        }

        // Token: 0x0600520F RID: 21007 RVA: 0x0000DEC1 File Offset: 0x0000C0C1
        static void Func_40(string A_0, byte[] A_1)
        {
            File.WriteAllBytes(A_0, A_1);
        }

        // Token: 0x06005210 RID: 21008 RVA: 0x000025A5 File Offset: 0x000007A5
        static int Func_41()
        {
            return Screen.width;
        }

        // Token: 0x06005211 RID: 21009 RVA: 0x000025AC File Offset: 0x000007AC
        static int Func_42()
        {
            return Screen.height;
        }

        // Token: 0x06005212 RID: 21010 RVA: 0x0000409D File Offset: 0x0000229D
        static string Func_43(string A_0, string A_1, string A_2)
        {
            return A_0 + A_1 + A_2;
        }

        // Token: 0x06005213 RID: 21011 RVA: 0x00025671 File Offset: 0x00023871
        static Regex Func_44(string A_0)
        {
            return new Regex(A_0);
        }

        // Token: 0x06005214 RID: 21012 RVA: 0x00025679 File Offset: 0x00023879
        static string Func_45(Regex A_0, string A_1, string A_2)
        {
            return A_0.Replace(A_1, A_2);
        }

        // Token: 0x06005215 RID: 21013 RVA: 0x00025683 File Offset: 0x00023883
        static string Func_46(string A_0, string A_1, string A_2)
        {
            return Regex.Replace(A_0, A_1, A_2);
        }

        // Token: 0x040031F2 RID: 12786
        public static string Base_Name;

        // Token: 0x040031F3 RID: 12787
        public static string Chat_Name;

        // Token: 0x040031F4 RID: 12788
        public static int color_length = 20;

        // Token: 0x040031F5 RID: 12789
        public static bool GameId_Change;

        // Token: 0x040031F6 RID: 12790
        public static bool Gradual_Change;

        // Token: 0x040031F7 RID: 12791
        public static uint hostID;

        // Token: 0x040031F8 RID: 12792
        public static bool Houzhui;

        // Token: 0x040031F9 RID: 12793
        public static ColorfulSpeek ins;

        // Token: 0x040031FA RID: 12794
        public static int Size_Name;

        // Token: 0x040031FB RID: 12795
        public static string Top_Name;

        // Token: 0x040031FC RID: 12796
        public static int Size_Words;

        // Token: 0x040031FD RID: 12797
        public static int Words_ColorType;

        // Token: 0x040031FE RID: 12798
        public static int Name_ColorType;

        // Token: 0x040031FF RID: 12799
        public List<string> GradientList;

        // Token: 0x04003200 RID: 12800
        public Texture2D gradient_preview;

        // Token: 0x04003201 RID: 12801
        public List<ColorfulSpeek.GradientSeed> gradientSeeds;

        // Token: 0x04003202 RID: 12802
        public static Texture2D EmptyTexture = ColorfulSpeek.Func_01(660, 210, TextureFormat.ARGB32, false);

        // Token: 0x04003203 RID: 12803
        public static ColorfulSpeek.GradientSeed NameSeed = default(ColorfulSpeek.GradientSeed);

        // Token: 0x04003204 RID: 12804
        public static ColorfulSpeek.GradientSeed WordSeed = default(ColorfulSpeek.GradientSeed);

        // Token: 0x04003205 RID: 12805
        public static bool Name_Italic;

        // Token: 0x04003206 RID: 12806
        public static bool Name_Bold;

        // Token: 0x04003207 RID: 12807
        public static bool Words_Italic;

        // Token: 0x04003208 RID: 12808
        public static bool Words_Bold;

        // Token: 0x04003209 RID: 12809
        public static bool Gradual_Random;

        // Token: 0x0400320A RID: 12810
        private string[] 渐变存数据;

        // Token: 0x0400320B RID: 12811
        private List<long> txtlength;

        // Token: 0x0400320C RID: 12812
        private string[] hang_up_symbol;

        // Token: 0x0400320D RID: 12813
        public int Chat_note;

        // Token: 0x0400320E RID: 12814
        public string Chat_roomname;

        // Token: 0x0400320F RID: 12815
        public string Chat_record;

        // Token: 0x04003210 RID: 12816
        public int Chat_roomnamebar;

        // Token: 0x04003211 RID: 12817
        public static bool UseRandomSize;

        // Token: 0x020008BB RID: 2235
        public struct GradientSeed
        {
            // Token: 0x04003212 RID: 12818
            public string name;

            // Token: 0x04003213 RID: 12819
            public string[] contains;

            // Token: 0x04003214 RID: 12820
            public Texture2D texture;

            // Token: 0x04003215 RID: 12821
            public string[] list;

            // Token: 0x04003216 RID: 12822
            public bool isOnlyList;

            // Token: 0x04003217 RID: 12823
            public Gradient gradient;
        }
    }
}
