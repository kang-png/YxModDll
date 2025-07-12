using HumanAPI;
using Steamworks;
using System;
using System.Reflection;
using UnityEngine;
using YxModDll.Mod;

namespace YxModDll.Patches
{
    public class Patcher_LevelRepository : MonoBehaviour
    {
        private static Type targetType = typeof(LevelRepository);

        private static FieldInfo _onReadField;
        private static FieldInfo _UGCDetailsField;

        private void Awake()
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            _onReadField = targetType.GetField("onRead", flags);
            _UGCDetailsField = targetType.GetField("UGCDetails", flags);

            Patcher2.MethodPatch(
                targetType,
                "OnDownloadThumbnail",
                new Type[] { typeof(RemoteStorageDownloadUGCResult_t), typeof(bool) },
                typeof(Patcher_LevelRepository),
                "OnDownloadThumbnail_Patched",
                new Type[] { typeof(object), typeof(RemoteStorageDownloadUGCResult_t), typeof(bool) }
            );
        }

        public static void OnDownloadThumbnail_Patched(object __instance, RemoteStorageDownloadUGCResult_t param, bool bIOFailure)
        {
            Delegate onRead = (Delegate)_onReadField.GetValue(__instance);

            if (param.m_eResult != EResult.k_EResultOK)
            {
                if (!UI_SheZhi.quchujiazaishibai)
                {
                    if (onRead != null)
                        onRead.DynamicInvoke((object)null);

                    _onReadField.SetValue(__instance, null);
                    return;
                }
            }

            byte[] array = new byte[param.m_nSizeInBytes];

            var UGCDetails = _UGCDetailsField.GetValue(__instance);

            var m_nPublishedFileId = UGCDetails.GetType().GetField("m_nPublishedFileId", BindingFlags.Public | BindingFlags.Instance).GetValue(UGCDetails);
            var m_rgchTitle = UGCDetails.GetType().GetField("m_rgchTitle", BindingFlags.Public | BindingFlags.Instance).GetValue(UGCDetails);


            var m_hPreviewFileObj = UGCDetails.GetType().GetField("m_hPreviewFile", BindingFlags.Public | BindingFlags.Instance).GetValue(UGCDetails);

            var builtinLevelMetadata = new BuiltinLevelMetadata
            {
                folder = "none",
                workshopId = ((PublishedFileId_t)m_nPublishedFileId).m_PublishedFileId,
                itemType = WorkshopItemType.Level,
                title = (string)m_rgchTitle,
                cachedThumbnailBytes = array
            };

            if (onRead != null)
                onRead.DynamicInvoke(builtinLevelMetadata);

            _onReadField.SetValue(__instance, null);
        }
    }
}
