using InControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static System.Collections.Specialized.BitVector32;

namespace YxModDll.Mod
{
    public class INI
    {
        //private static string yxmodPath = Path.Combine(Application.dataPath, "Managed", "YxMod");
        private static string yxmodPath = Path.GetDirectoryName(Application.dataPath);
        private static string configFilePath = yxmodPath + "\\YxMod.ini";
        private static string kuaijiejianFilePath = yxmodPath + "\\快捷键.ini";
        private const string kuaijiejianstr = @"## 以下是所有快捷键的KeyCode码 ## 
##————————————————————————————————————————————
## F1 ~ F12 / A ~ Z / Alpha0 ~ Alpha9(顶部数字键) / Keypad0 ~ Keypad9 (小键盘数字键)
## LeftShift/RightShift/LeftControl/RightControl/LeftAlt/RightAlt/LeftWindows/RightWindows
## Space/Enter/Backspace/Tab/Escape/CapsLock/Insert/Delete/Home/End/PageUp/PageDown/UpArrow/DownArrow/LeftArrow/RightArrow
## Mouse0（鼠标左键）/ Mouse1（鼠标右键）/ Mouse2（鼠标中键）/ Mouse3 ~ Mouse6（鼠标侧键）
##————————————————————————————————————————————

##以下是配置信息

";

        //private static Dictionary<string, Dictionary<string, string>> configData = new Dictionary<string, Dictionary<string, string>>();

        //public Config()
        //{
        //    ReadConfigFile();
        //}

        private static Dictionary<string, string> ReadConfigFile(string path)
        {
            Dictionary<string, string> configData = new Dictionary<string, string>();

            try
            {
                if (File.Exists(path))
                {
                    string[] lines = File.ReadAllLines(path);
                    foreach (string line in lines)
                    {
                        string trimmedLine = line.Trim();
                        //if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                        //{
                        //    // 解析节名
                        //    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                        //    if (!configData.ContainsKey(currentSection))
                        //    {
                        //        configData[currentSection] = new Dictionary<string, string>();
                        //    }
                        //}
                        if (!string.IsNullOrEmpty(trimmedLine) && !trimmedLine.StartsWith("#"))
                        {
                            //去掉注释

                            // 解析键值对
                            string[] parts = trimmedLine.Split('=');
                            if (parts.Length == 2)
                            {
                                string key = parts[0].Trim();
                                string value = parts[1].Trim();
                                configData[key] = value;
                            }
                        }
                    }

                }
                //else
                //{
                //    Debug.Log("配置文件不存在");
                //    //新建配置文件
                //}
            }
            catch (Exception e)
            {
                Debug.LogError("读取配置文件时出错: " + e.Message);
            }
            return configData;
        }
        public static void OpenIni()
        {
            try
            {
                Debug.Log("打开快捷键.ini文件"+ kuaijiejianFilePath);
                if (File.Exists(kuaijiejianFilePath))
                {
                    System.Diagnostics.Process.Start("notepad.exe", $"\"{kuaijiejianFilePath}\"");

                    //System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                    //{
                    //    FileName = "notepad.exe",
                    //    Arguments = $"\"{kuaijiejianFilePath}\"",
                    //    //UseShellExecute = true // 允许系统外壳处理
                    //    UseShellExecute = false,
                    //    CreateNoWindow = true
                    //});
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"打开快捷键.ini失败：{ex.Message}");
            }
        }

        public static void SetKuaiJieJian(string key, KuaiJieJian_Type value)
        {
            Dictionary<string, string> configData = ReadConfigFile(kuaijiejianFilePath);

            string strvalue = $"{value.keyCode1.ToString()}{(value.keyCode2 != KeyCode.None ? "+" + value.keyCode2.ToString() : "")}";

            if (!configData.ContainsKey(key))
            {
                configData.Add(key, strvalue);
            }
            else
            {
                configData[key] = strvalue;
            }
            SaveConfigFile(configData, kuaijiejianFilePath);
        }
        public static KuaiJieJian_Type GetKuaiJieJian(string key, KuaiJieJian_Type kuaijiejian = null)
        {
            Dictionary<string, string> configData = ReadConfigFile(kuaijiejianFilePath);
            if (kuaijiejian == null)
            {
                kuaijiejian = new KuaiJieJian_Type();
            }
            if (configData.ContainsKey(key))
            {
                string str = configData[key];

                if (!string.IsNullOrEmpty(str))
                {
                    string[] keys = str.Split('+');
                    try
                    {
                        
                        if(keys.Length == 1)
                        {
                            kuaijiejian.keyCode1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keys[0], ignoreCase: true);
                        }
                        else if (keys.Length == 2)
                        {
                            kuaijiejian.keyCode2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), keys[1], ignoreCase: true);
                        }

                    }
                    catch (ArgumentException ex)
                    {
                        Debug.LogError($"解析快捷键时出错: {ex.Message}");
                    }
                }
            }
            else
            {
                SetKuaiJieJian(key, kuaijiejian);
            }
            return kuaijiejian;
        }
        public static List<KuaiJieJian_Type> GetKuaiJieJianList(string key, List<KuaiJieJian_Type> defaultList = null)
        {
            Dictionary<string, string> configData = ReadConfigFile(kuaijiejianFilePath);
            var resultList = new List<KuaiJieJian_Type>();

            if (configData.TryGetValue(key, out string str) && !string.IsNullOrEmpty(str))
            {
                string[] orParts = str.Split('|');
                foreach (string part in orParts)
                {
                    string[] keys = part.Trim().Split('+');
                    try
                    {
                        var kjj = new KuaiJieJian_Type();
                        if (keys.Length == 1)
                        {
                            kjj.keyCode1 = (KeyCode)Enum.Parse(typeof(KeyCode), keys[0], true);
                        }
                        else if (keys.Length == 2)
                        {
                            kjj.keyCode1 = (KeyCode)Enum.Parse(typeof(KeyCode), keys[0], true);
                            kjj.keyCode2 = (KeyCode)Enum.Parse(typeof(KeyCode), keys[1], true);
                        }
                        resultList.Add(kjj);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"解析快捷键出错: {ex.Message}");
                    }
                }
            }
            else if (defaultList != null && defaultList.Count > 0)
            {
                resultList = defaultList;

                // 保存默认值回配置
                string saveStr = string.Join("|", defaultList.Select(k =>
                {
                    string s = k.keyCode1.ToString();
                    if (k.keyCode2 != KeyCode.None)
                        s += "+" + k.keyCode2.ToString();
                    return s;
                }));

                configData[key] = saveStr;
                SaveConfigFile(configData, kuaijiejianFilePath);
            }

            return resultList;
        }
        public static List<KuaiJieJian_Type> GetKuaiJieJianList(string key, KuaiJieJian_Type defaultSingle)
        {
            return GetKuaiJieJianList(key, new List<KuaiJieJian_Type> { defaultSingle });
        }



        public static bool GetBoolValue(string key, bool moren = false)
        {
            string str = GetValueInternal(key);
            bool boolstr = moren;
            if (str != null)
            {
                if (bool.TryParse(str, out bool result))
                {
                    boolstr = result;
                }
            }
            return boolstr;
        }
        public static float GetFolatValue(string key, float moren = 0f)
        {
            string str = GetValueInternal(key);
            float floatstr = moren;
            if (str != null)
            {
                if (float.TryParse(str, out float result))
                {
                    floatstr = result;
                }
            }
            return floatstr;
        }

        public static int GetIntValue(string key, int moren = 0)
        {
            string str = GetValueInternal(key);
            int intstr = moren;
            if (str != null)
            {
                if (int.TryParse(str, out int result))
                {
                    intstr = result;
                }
            }
            return intstr;
        }
        public static string GetStringValue(string key, string moren = "")
        {
            string str = GetValueInternal(key);
            string strstr = moren;
            if (str != null)
            {
                strstr = str;
            }
            return strstr;
        }

        private static string GetValueInternal(string key)
        {
            Dictionary<string, string> configData = ReadConfigFile(configFilePath);

            if (configData.ContainsKey(key))
            {
                return configData[key];
            }
            return null;
        }



        public static void SetValue(string key, string value)
        {
            SetValueInternal(key, value);
        }
        public static void SetValue(string key, int value)
        {
            SetValueInternal(key, value.ToString());
        }
        public static void SetValue(string key, float value)
        {
            SetValueInternal(key, value.ToString());
        }
        public static void SetValue(string key, bool value)
        {
            SetValueInternal(key, value.ToString());
        }

        private static void SetValueInternal(string key, string value)
        {
            Dictionary<string, string> configData = ReadConfigFile(configFilePath);

            if (!configData.ContainsKey(key))
            {
                configData.Add(key, value);
            }
            else
            {
                configData[key] = value;
            }
            SaveConfigFile(configData, configFilePath);
        }

        private static void SaveConfigFile(Dictionary<string, string> configData, string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.Write(kuaijiejianstr);
                    foreach (var keyValue in configData)
                    {
                        writer.WriteLine($"{keyValue.Key} = {keyValue.Value}");
                    }
                    writer.WriteLine();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("保存配置文件时出错: " + e.Message);
            }
        }
    }



}
