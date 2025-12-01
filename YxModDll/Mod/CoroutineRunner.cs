using UnityEngine;
using System.Collections;

namespace YxModDll.Mod
{
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        public static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("CoroutineRunner_YxMod");
                    Object.DontDestroyOnLoad(obj);
                    _instance = obj.AddComponent<CoroutineRunner>();
                }
                return _instance;
            }
        }

        public void RunCoroutine(IEnumerator routine)
        {
            StartCoroutine(routine);
        }
    }
}