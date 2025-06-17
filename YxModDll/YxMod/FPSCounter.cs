using UnityEngine;
using Multiplayer;
using HumanAPI;
using System;
using UnityEngine.UI;

namespace YxModDll.Mod
{
    public class FPSCounter : MonoBehaviour
    {
        private float elapsedTime;
        private int frameCount;
        private int fps;
        private float latency;

        public static bool showFPS;

        private string levelTitle = "加载中...";
        private Text LevelTextRef;

        void Update()
        {
            if (!showFPS) return;

            // 每帧尝试绑定一次 LevelText，如果还没找到
            if (LevelTextRef == null)
            {
                var infoBox = GameObject.FindObjectOfType<LevelInformationBox>();
                if (infoBox != null)
                {
                    LevelTextRef = infoBox.LevelText;
                }
            }

            // 读取标题
            if (LevelTextRef != null)
            {
                levelTitle = LevelTextRef.text;
            }

            elapsedTime += Time.deltaTime;
            frameCount++;

            if (elapsedTime >= 0.5f)
            {
                fps = Mathf.RoundToInt(frameCount / elapsedTime);
                elapsedTime = 0f;
                frameCount = 0;

                latency = NetGame.instance?.clientLatency?.latency ?? 0;
            }
        }

        private void OnGUI()
        {
            if (!showFPS) return;

            GUIStyle style = new GUIStyle
            {
                fontSize = 22,
                normal = { textColor = Color.white }
            };

            GUILayout.BeginArea(new Rect(10, 10, 500, 200));

            GUILayout.Label($"帧数: {fps} FPS", style);
            GUILayout.Label($"延迟: {latency:F0} ms", style);
            GUILayout.Label($"时间: {DateTime.Now:HH:mm:ss}", style);
            GUILayout.Label($"图名: {levelTitle}", style);

            if (Game.instance != null && Game.currentLevel != null && Game.currentLevel.checkpoints != null)
            {
                int total = Game.currentLevel.checkpoints.Length;
                int cur = Mathf.Clamp(Game.instance.currentCheckpointNumber, 0, total - 1);
                GUILayout.Label($"进度: {cur + 1}/{total}", style);
            }

            GUILayout.EndArea();
        }
    }
}
