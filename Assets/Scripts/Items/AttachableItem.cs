using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class AttachableItem : CollectableItem
{
    public SkeletonDataAsset asset;

    private void Awake()
    {
        // TODO : Load string From GameManager
        asset = (SkeletonDataAsset)Resources.Load("Assets/Animations/Player/Spine/char1_SkeletonData.asset");
    }

    [Header("Spine")]
    [SpineSlot(dataField = "asset")] public string Slot;
    [SpineAttachment(dataField = "asset")] public string Attachment;
}

[System.Serializable]
public class ItemAttachment
{
    [SpineSlot] public string Slot;
    [SpineAttachment] public string Attachment;
}
