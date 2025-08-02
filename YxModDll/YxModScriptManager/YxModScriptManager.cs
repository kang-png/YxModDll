
using System;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace YxModScriptManager
{
    public class YxModScriptManager : MonoBehaviour
    {
        private static string yxmodPath = Path.Combine(Path.GetDirectoryName(Application.dataPath),"YxMod测试");
        //private static string yxmodPath = Path.Combine(Application.dataPath, "Managed", "YxMod");
        //public static string dllpath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "YxMod\\YxMod.dll");
        private AppDomain _appDomain;
        public YxModScriptProxy _proxy;


        private void Awake()
        {
            Debug.Log("ScriptManager加载成功！");
        }
        private void Start()
        {
            LoadYxMod();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                LoadYxMod();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                UnLoadYxMod();
            }
        }
        private void LoadYxMod()
        {
            UnLoadYxMod();
            //AppDomainSetup setup = new AppDomainSetup();
            //setup.PrivateBinPath = yxmodPath;
            //_appDomain = _appDomain ?? AppDomain.CreateDomain("ScriptDomain", null, setup);

            _appDomain = _appDomain ?? AppDomain.CreateDomain("YxModScriptDomain");

            //创建代理对象
            _proxy = _proxy ?? (YxModScriptProxy)_appDomain.CreateInstanceAndUnwrap(
                Assembly.GetExecutingAssembly().FullName,
                typeof(YxModScriptProxy).FullName);

            _proxy.LoadScript($"{yxmodPath}\\YxMod测试.dll");

        }
        //private void OnDestroy()
        //{
        //    //////Destroy(shezhi);

        //    Debug .Log("OnDestroy");
        //    ////YxMod.Log("YxMod加载成功222！");

        //}
        private void UnLoadYxMod()
        {
            try
            {
                if (_proxy != null)
                {
                    _proxy.UnloadScript();
                    _proxy = null;
                    _appDomain = null;
                    //AppDomain.Unload(_appDomain);
                }
            }
            catch (AppDomainUnloadedException)
            {
                Debug.LogWarning("AppDomain was already unloaded.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to unload AppDomain: {e.Message}");
            }
            //if (_appDomain != null)
            //{
            //    //GameObject YxGameObject = GameObject.Find("YxGameObject");
            //    //if (YxGameObject != null)
            //    //{
            //    //    _proxy.UnloadScript();
            //    //    //_proxy = null;
            //    //    //Debug.Log("2222");
            //    //}
            //    AppDomain.Unload(_appDomain);
            //    //_appDomain = null;
            //    Debug.Log("3333");
            //}
            //if (_proxy != null)
            //{

            //    _proxy = null;
            //    //    Debug.Log("4444");
            //}
        }

    }
}
