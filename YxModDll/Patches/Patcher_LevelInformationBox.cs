using HumanAPI;
using Multiplayer;
using System.Collections;
using System.Reflection;
using UnityEngine;
using YxModDll.Mod;
using YxModDll.Patches;


namespace YxModDll.Patches
{
    public class Patcher_LevelInformationBox : MonoBehaviour
    {
        //LevelInformationBox instance;
        private static FieldInfo _prevDispInfoField;

        public static bool guanbidatingxiazai;

        private void Awake()
        {
            //instance = FindObjectOfType<LevelInformationBox>();
            _prevDispInfoField = typeof(LevelInformationBox).GetField("prevDispInfo", BindingFlags.NonPublic | BindingFlags.Instance);
            Patcher2.MethodPatch(typeof(LevelInformationBox), "GetNewLevel", new[] { typeof(ulong) }, typeof(Patcher_LevelInformationBox), "GetNewLevel", new[] { typeof(LevelInformationBox), typeof(ulong) });

        }

        public static IEnumerator GetNewLevel(LevelInformationBox instance, ulong levelID)
        {
            var prevDispInfo = (NetTransport.LobbyDisplayInfo)_prevDispInfoField.GetValue(instance);
            bool loaded = false;
            WorkshopLevelMetadata levelData = null;

            System.Action<WorkshopLevelMetadata> action = (WorkshopLevelMetadata l) =>
            {
                levelData = l;
                loaded = true;
                if (levelData != null && (prevDispInfo.FeaturesMask & 0x20000000u) != 0 && prevDispInfo.LevelID == levelID)
                {
                    instance.LevelText.text = levelData.title;
                    instance.LevelImage.texture = levelData.thumbnailTexture;
                    instance.LevelImage.enabled = instance.LevelImage.texture != null;
                }
            };

            if (UI_SheZhi.guanbidatingxiazai)
            {
                WorkshopRepository.instance.levelRepo.GetLevel(levelID, prevDispInfo.LevelType, action);
            }
            else
            {
                WorkshopRepository.instance.levelRepo.LoadLevel(levelID, action);
            }

            while (!loaded)
            {
                yield return null;
            }
        }

    }
}

