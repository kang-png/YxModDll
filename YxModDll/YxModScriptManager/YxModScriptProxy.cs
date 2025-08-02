using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace YxModScriptManager
{
    public class YxModScriptProxy : MarshalByRefObject
    {
        private GameObject _gameObject;// = new GameObject("YxGameObject");

        private static Dictionary<string, Assembly> _assemblyCache = new Dictionary<string, Assembly>();
        public YxModScriptProxy()
        {
            _gameObject = new GameObject("YxGame测试Object");
            _gameObject.hideFlags = HideFlags.HideAndDontSave;
            UnityEngine.Object.DontDestroyOnLoad(_gameObject);

        }
        // 在 YxModScriptProxy 类中添加
        public override object InitializeLifetimeService()
        {
            // 取消租约，使对象永久有效（跨域调用场景必备）
            return null;
        }
        public Assembly LoadAssembly(string dllPath)
        {
            if (_assemblyCache.TryGetValue(dllPath, out Assembly cachedAssembly))
            {
                return cachedAssembly;
            }
            //Assembly assembly = Assembly.LoadFrom(dll);
            byte[] assemblyData = File.ReadAllBytes(dllPath);
            Assembly assembly = Assembly.Load(assemblyData);
            _assemblyCache[dllPath] = assembly;

            assemblyData = null; // 释放字节数组
            return assembly;
        }
        public void LoadScript(string dllpath)
        {
            if (File.Exists(dllpath))
            {
                try
                {
                    //byte[] assemblyData = File.ReadAllBytes(dllpath);
                    //Assembly assembly = Assembly.Load(assemblyData);
                    //assemblyData = null; // 立即释放字节数组


                    //Assembly assembly = Assembly.LoadFrom(dll);


                    Assembly assembly = LoadAssembly(dllpath);

                    //foreach (Type type in assembly.GetTypes())
                    //{
                    //    //Log(type.Name);
                    //}

                    Type _type = assembly.GetType("YxMod测试Point");
                    if (_type != null)
                    {
                        MonoBehaviour _instance = _gameObject.AddComponent(_type) as MonoBehaviour;
                        _type = null;
                        Log($"重新加载：{dllpath}");
                    }
                    else
                    {
                        Log($"类型 YxModPoint 未找到于 {dllpath}");
                    }
                }
                catch (Exception e)
                {
                    Log($"加载脚本失败：{e.Message}");
                }

            }
            else
            {
                Log($"DLL 文件未找到：{dllpath}");
            }
        }
        public void UnloadScript()
        {

            if (_gameObject != null)
            {

                //UnityEngine.GameObject.Destroy(_instance);
                //_instance = null;
                UnityEngine.GameObject.Destroy(_gameObject);
                // 清理所有组件
                //foreach (var component in _gameObject.GetComponents<Component>())
                //{
                //    if (component != null)
                //    {
                //        UnityEngine.GameObject.Destroy(component);
                //    }
                //}
                _gameObject = null;
                Log("脚本已卸载。");
            }
            if (_assemblyCache != null)
            {
                _assemblyCache.Clear(); // 清空字典以释放引用
                _assemblyCache = null;
            }
        }
        public void Log(string message)
        {
            Console.WriteLine($"[ScriptManager]{message}");
        }

    }


}
