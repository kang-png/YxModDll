using System;
using HumanAPI;
using Steamworks;
using UnityEngine;
using YxModDll.Mod.Features;

public class LevelRepository2 
{
    private Action<WorkshopLevelMetadata> onRead;

    private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryCompletedCallResult;

    private CallResult<RemoteStorageDownloadUGCResult_t> OnUGCDownloadCallResult;

    private SteamUGCDetails_t UGCDetails;

    public static LevelRepository2 instance = new LevelRepository2();

    public void GetLevelNameAndThumbnail(ulong levelId, WorkshopItemSource levelType, Action<WorkshopLevelMetadata> onRead)
    {
        if (OnSteamUGCQueryCompletedCallResult != null)
        {
            OnSteamUGCQueryCompletedCallResult.Cancel();
            OnSteamUGCQueryCompletedCallResult = null;
        }

        if (OnUGCDownloadCallResult != null)
        {
            OnUGCDownloadCallResult.Cancel();
            OnUGCDownloadCallResult = null;
        }


        this.onRead = onRead;
        UGCQueryHandle_t handle = SteamUGC.CreateQueryUGCDetailsRequest(new PublishedFileId_t[1]
        {
            new PublishedFileId_t(levelId)
        }, 1u);
        OnSteamUGCQueryCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryCompleted);
        SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(handle);
        OnSteamUGCQueryCompletedCallResult.Set(hAPICall);
    }

    private void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t param, bool bIOFailure)
    {
        if (param.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log("OnSteamUGCQueryCompleted 失败");
            onRead(null);
            onRead = null;
            return;
        }

        bool queryUGCResult = SteamUGC.GetQueryUGCResult(param.m_handle, 0u, out UGCDetails);
        OnUGCDownloadCallResult = CallResult<RemoteStorageDownloadUGCResult_t>.Create(OnDownloadThumbnail);
        SteamAPICall_t hAPICall = SteamRemoteStorage.UGCDownload(UGCDetails.m_hPreviewFile, 0u);
        OnUGCDownloadCallResult.Set(hAPICall);
    }

    private void OnDownloadThumbnail(RemoteStorageDownloadUGCResult_t param, bool bIOFailure)
    {
        if (param.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log("OnDownloadThumbnail 失败");
            onRead(null);
            onRead = null;
            return;
        }

        byte[] array = new byte[param.m_nSizeInBytes];
        SteamRemoteStorage.UGCRead(UGCDetails.m_hPreviewFile, array, array.Length, 0u, EUGCReadAction.k_EUGCRead_Close);
        // 创建元数据对象
        WorkshopLevelMetadata metadata = new BuiltinLevelMetadata
        {
            folder = "ws:" + UGCDetails.m_nPublishedFileId.m_PublishedFileId + "/",//"none",
            workshopId = UGCDetails.m_nPublishedFileId.m_PublishedFileId,
            itemType = WorkshopItemType.Level,
            title = UGCDetails.m_rgchTitle,
            description = UGCDetails.m_rgchDescription,
            cachedThumbnailBytes = array
        };
        WorkshopRepository.instance.levelRepo.AddItem(WorkshopItemSource.LocalWorkshop, metadata);//加载到内存，会自动读取
        onRead(metadata);
        
        onRead = null;
    }

   
}