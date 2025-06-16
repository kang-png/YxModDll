using UnityEngine;
using Multiplayer;
using HumanAPI;
using System;
using YxModDll.Patches;

namespace YxModDll.Mod
{
    public class FPSCounter : MonoBehaviour
    {
        private float elapsedTime;
        private int frameCount;
        private int fps;
        private float latency;
        private ulong lastLevelId = 0;

        private WorkshopLevelMetadata currentWorkshopMetadata;
        private bool hasWorkshopMetadata = false;

        public static bool showFPS;

        void Update()
        {
            if (!showFPS) return;

            elapsedTime += Time.deltaTime;
            frameCount++;

            if (elapsedTime >= 0.5f)
            {
                fps = Mathf.RoundToInt(frameCount / elapsedTime);
                elapsedTime = 0f;
                frameCount = 0;

                latency = NetGame.instance?.clientLatency?.latency ?? 0;
            }

            // 核心改动：持续检测 currentLevel 是否变化且有效
            var net = NetGame.instance;
            ulong levelId = net?.currentLevel ?? 0;
            if (levelId != 0 && levelId != lastLevelId)
            {
                lastLevelId = levelId;
                hasWorkshopMetadata = false;
                currentWorkshopMetadata = null;

                var repo = WorkshopRepository.instance?.levelRepo;
                repo?.GetLevel(levelId, net.currentLevelType, (metadata) =>
                {
                    currentWorkshopMetadata = metadata;
                    hasWorkshopMetadata = metadata != null;
                });
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

            GUILayout.BeginArea(new Rect(10, 10, 500, 120));
            GUILayout.Label($"帧数: {fps} FPS", style);
            GUILayout.Label($"延迟: {latency:F0} ms", style);
            GUILayout.Label($"时间: {DateTime.Now:HH:mm:ss}", style);

            // 显示标题和进度
            if (hasWorkshopMetadata && Game.currentLevel?.checkpoints != null)
            {
                var cps = Game.currentLevel.checkpoints;
                int cur = Mathf.Clamp(Game.instance.currentCheckpointNumber, 0, cps.Length - 1);
                GUILayout.Label($"{currentWorkshopMetadata.title} ：{cur + 1}/{cps.Length}", style);
            }

            GUILayout.EndArea();
        }
    }
}
