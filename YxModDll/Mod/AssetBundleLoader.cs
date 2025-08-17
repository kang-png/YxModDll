using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace YxModDll.Mod
{
    public class AssetBundleLoader : MonoBehaviour
    {
        private static string dongzuoPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "dongzuo");

        //private static string tuomasiPath = dongzuoPath + "\\tuomasi";
        //private static string piliwudongjiePath = dongzuoPath + "\\piliwudongjie";
        


        private static readonly List<AssetBundle> AssetBundles = new List<AssetBundle>();

        private static readonly Dictionary<string, AssetBundle> Assets = new Dictionary<string, AssetBundle>();

        private void Start()
        {
            Debug.Log("加载包");

            // 获取所有枚举成员名称并转为小写
            string[] allStateNames = Array.ConvertAll(
                Enum.GetNames(typeof(DONGZUO_State)),
                name => name.ToLower()
            );

            // 遍历输出（示例）
            foreach (string name in allStateNames)
            {
                //Console.WriteLine(name);
                string path = dongzuoPath + $"\\{name}";
                StartCoroutine(LoadAssetBundle(path));
            }

            

            //StartCoroutine(LoadAssetBundleFromBase64(tuomasiBase64));
        }

        public static IEnumerator LoadAssetBundleFromBase64(string base64Data)
        {
            byte[] rawData;
            try
            {
                rawData = Convert.FromBase64String(base64Data);
            }
            catch (FormatException ex)
            {
                Debug.LogError("❌ Base64 格式错误: " + ex.Message);
                yield break;
            }

            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(rawData);
            yield return request;

            AssetBundle ab = request.assetBundle;
            if (ab == null)
            {
                Debug.LogError("❌ 无法从 Base64 创建 AssetBundle，数据可能损坏或格式不匹配");
                yield break;
            }

            AssetBundleManifest manifest = null;
            try
            {
                manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
            catch (Exception)
            {
                // 没有 Manifest 也很正常（比如单个动画包）
            }

            if (manifest != null)
            {
                // 有依赖，递归加载
                string dummyPath = "base64://embedded"; // 虚拟路径用于依赖拼接
                foreach (string bundleName in manifest.GetAllAssetBundles())
                {
                    foreach (string dep in manifest.GetAllDependencies(bundleName))
                    {
                        // 这里假设依赖也在 Base64 中或外部提供（简化处理）
                        // 您可根据需要扩展：比如也内嵌多个 Base64 包
                        Debug.Log($"依赖项（未处理）: {dep}");
                    }
                    yield return LoadAssetBundleFromBase64(base64Data); // 如果是自包含包
                }
            }

            // ✅ 添加到全局管理
            AssetBundles.Add(ab);
            Debug.Log($"✅ Base64 AssetBundle 加载成功: {ab.name}");

            foreach (string assetPath in ab.GetAllAssetNames())
            {
                if (!string.IsNullOrEmpty(assetPath) && assetPath.ToLower() != "assetbundlemanifest")
                {
                    string key = assetPath.ToLower();
                    if (!Assets.ContainsKey(key))
                    {
                        Assets[key] = ab;
                    }
                    Debug.Log("包含 asset: " + assetPath);
                }
            }

            manifest = null;
        }

        // 下载文件的协程
        private static IEnumerator DownloadFile(string url, string savePath)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // 发送请求
                yield return webRequest.SendWebRequest();

                // 处理结果（区分Unity不同版本的错误判断方式）
                bool isError = false;
#if UNITY_2020_1_OR_NEWER
                isError = webRequest.result != UnityWebRequest.Result.Success;
#else
                isError = webRequest.isNetworkError || webRequest.isHttpError;
#endif

                if (isError)
                {
                    Debug.LogError($"❌ 下载失败：{webRequest.error}（URL：{url}）");
                    yield break;
                }

                // 保存文件到本地
                try
                {
                    File.WriteAllBytes(savePath, webRequest.downloadHandler.data);
                    Debug.Log($"📥 文件下载成功（大小：{webRequest.downloadHandler.data.Length}字节）");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"❌ 保存文件失败：{ex.Message}（路径：{savePath}）");
                }
            }
        }
        // 文件名与下载URL的映射关系
        private static readonly Dictionary<string, string> _fileDownloadUrls = new Dictionary<string, string>
        {
            { "tuomasi", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755442240825/tuomasi.txt" },
            { "piliwudongjie", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755442240352/piliwudongjie.txt" },
            { "yaobaiwu", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445676749/yaobaiwu.txt" },
            { "yangwoqizuo", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445676245/yangwoqizuo.txt" },
            { "xihawu3", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445675820/xihawu3.txt" },
            { "xihawu2", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445675412/xihawu2.txt" },
            { "xihawu", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445675006/xihawu.txt" },
            { "touxuan", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445674581/touxuan.txt" },
            { "sangbawu2", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445674154/sangbawu2.txt" },
            { "sangbawu", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445673674/sangbawu.txt" },
            { "quanji", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445673211/quanji.txt" },
            { "qimawu", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445672782/qimawu.txt" },
            { "mumati", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445672349/mumati.txt" },
            { "kaihetiao", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445671921/kaihetiao.txt" },
            { "jiaochatiaoyue", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445671483/jiaochatiaoyue.txt" },
            { "heiyingtaowubu", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445671040/heiyingtaowubu.txt" },
            { "fuwocheng", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445670636/fuwocheng.txt" },
            { "diantunwu", "https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755445670212/diantunwu.txt" }

        };
        public static IEnumerator LoadAssetBundle(string path)
        {
            if (!File.Exists(path))
            {
                //Debug.LogError($"❌ 包文件不存在：{path}");
                ////https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755442240825/tuomasi.txt
                ////resources/upload/95fa5d1312ce/1755442240352/piliwudongjie.txt
                //yield break;

                Debug.LogWarning($"⚠️ 本地文件不存在：{path}，尝试从网络下载...");

                // 解析文件名（从路径中提取）
                string fileName = Path.GetFileName(path);
                if (!_fileDownloadUrls.TryGetValue(fileName, out string downloadUrl))
                {
                    Debug.LogError($"❌ 未找到 {fileName} 对应的下载URL，请检查配置");
                    yield break;
                }
                //string downloadUrl = $"https://suye.bce.baidu.com/resources/upload/95fa5d1312ce/1755442240825/{fileName}.txt";
                // 先确保目录存在（避免保存时出错）
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Debug.Log($"📂 创建目录：{directory}");
                }

                // 执行下载
                yield return DownloadFile(downloadUrl, path);

                // 下载完成后再次检查文件是否存在
                if (!File.Exists(path))
                {
                    Debug.LogError($"❌ 下载失败，文件仍不存在：{path}");
                    yield break;
                }
                Debug.Log($"✅ 下载完成，已保存到：{path}");



            }
            AssetBundleCreateRequest request = null;
            try
            {
                request = AssetBundle.LoadFromFileAsync(path);
            }
            catch
            {
            }
            if (request == null)
            {
                yield break;
            }
            yield return request;
            AssetBundle ab = request.assetBundle;
            if (ab)
            {
                AssetBundleManifest manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (manifest)
                {
                    foreach (string item in manifest.GetAllAssetBundles())
                    {
                        foreach (string str in manifest.GetAllDependencies(item))
                        {
                            yield return AssetBundleLoader.LoadAssetBundle(Path.GetDirectoryName(path) + "/" + str);
                        }
                        string[] array2 = null;
                        yield return AssetBundleLoader.LoadAssetBundle(Path.GetDirectoryName(path) + "/" + item);
                        //item = null;
                    }
                    string[] array = null;
                }
                AssetBundleLoader.AssetBundles.Add(ab);
                Debug.Log("包含资源: " + ab.name);
                foreach (string text in ab.GetAllAssetNames())
                {
                    if (!string.IsNullOrEmpty(ab.name) && !string.IsNullOrEmpty(text) && !(text.ToLower() == "assetbundlemanifest"))
                    {
                        Debug.Log("包含asset: " + text);
                        AssetBundleLoader.Assets[text] = ab;
                    }
                }
                manifest = null;
            }
            yield break;
        }

        private static AssetBundle GetAssetBundle(string asset, out string properlyName)
        {
            string text = (asset != null) ? asset.ToLower() : null;
            properlyName = text;
            if (!string.IsNullOrEmpty(text) && AssetBundleLoader.Assets.ContainsKey(text))
            {
                return AssetBundleLoader.Assets[text];
            }
            foreach (string text2 in AssetBundleLoader.Assets.Keys)
            {
                if (text2.Contains(text))
                {
                    properlyName = text2;
                    return AssetBundleLoader.Assets[text2];
                }
            }
            return null;
        }

        public static T[] LoadAllAssets<T>() where T : UnityEngine.Object
        {
            List<T> list = new List<T>();
            foreach (AssetBundle assetBundle in AssetBundleLoader.AssetBundles)
            {
                list.AddRange(assetBundle.LoadAllAssets<T>());
            }
            return list.ToArray();
        }

        public static T LoadAsset<T>(string asset) where T : UnityEngine.Object
        {
            string name;
            AssetBundle assetBundle = AssetBundleLoader.GetAssetBundle(asset, out name);
            if (assetBundle == null)
            {
                return default(T);
            }
            return assetBundle.LoadAsset<T>(name);
        }

        public static AssetBundleRequest LoadAssetAsync<T>(string asset) where T : UnityEngine.Object
        {
            string name;
            AssetBundle assetBundle = AssetBundleLoader.GetAssetBundle(asset, out name);
            if (assetBundle == null)
            {
                return null;
            }
            return assetBundle.LoadAssetAsync<T>(name);
        }

        public static T[] LoadAssetWithSubAssets<T>(string asset) where T : UnityEngine.Object
        {
            string name;
            AssetBundle assetBundle = AssetBundleLoader.GetAssetBundle(asset, out name);
            if (assetBundle == null)
            {
                return new T[0];
            }
            return assetBundle.LoadAssetWithSubAssets<T>(name);
        }

        public static AssetBundleRequest LoadAssetWithSubAssetsAsync<T>(string asset) where T : UnityEngine.Object
        {
            string name;
            AssetBundle assetBundle = AssetBundleLoader.GetAssetBundle(asset, out name);
            if (assetBundle == null)
            {
                return null;
            }
            return assetBundle.LoadAssetWithSubAssetsAsync<T>(name);
        }


    }
}
