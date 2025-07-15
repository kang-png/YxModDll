using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UnityEngine;

namespace YxModDll.Mod.Utils
{
    internal class YxImageHelper
    {
        public static Texture2D LoadAndResizeTexture(string name, byte[] bytes, int maxSize = 1024)
        {
            using (var ms = new MemoryStream(bytes))
            using (var bitmap = new Bitmap(ms))
            {
                int w = bitmap.Width;
                int h = bitmap.Height;
                int max = Mathf.Max(w, h);

                if (max > maxSize)
                {
                    float scale = (float)maxSize / max;
                    w = Mathf.RoundToInt(w * scale);
                    h = Mathf.RoundToInt(h * scale);
                }

                using (var resized = new Bitmap(bitmap, new Size(w, h)))
                {
                    var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
                    var data = resized.LockBits(
                        new Rectangle(0, 0, w, h),
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format32bppArgb
                    );

                    byte[] raw = new byte[w * h * 4];
                    System.Runtime.InteropServices.Marshal.Copy(data.Scan0, raw, 0, raw.Length);
                    resized.UnlockBits(data);

                    // 注意：ARGB 转 RGBA（交换 R 和 B 分量）
                    for (int i = 0; i < raw.Length; i += 4)
                    {
                        byte b = raw[i];
                        raw[i] = raw[i + 2];     // R
                        raw[i + 2] = b;          // B
                    }

                    tex.LoadRawTextureData(raw);
                    tex.Apply();
                    tex.name = name;
                    return tex;
                }
            }
        }
    }
}
