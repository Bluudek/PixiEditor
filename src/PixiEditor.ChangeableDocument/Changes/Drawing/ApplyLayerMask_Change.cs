﻿using PixiEditor.ChangeableDocument.ChangeInfos.Properties;
using PixiEditor.DrawingApi.Core.Numerics;

namespace PixiEditor.ChangeableDocument.Changes.Drawing;
internal class ApplyLayerMask_Change : Change
{
    private readonly Guid layerGuid;
    private CommittedChunkStorage? savedMask;
    private CommittedChunkStorage? savedLayer;

    [GenerateMakeChangeAction]
    public ApplyLayerMask_Change(Guid layerGuid)
    {
        this.layerGuid = layerGuid;
    }

    public override bool InitializeAndValidate(Document target)
    {
        //TODO: Check if support for different Layer types is needed here.
        if (!target.TryFindMember<RasterLayer>(layerGuid, out var layer) || layer.Mask is null)
            return false;

        savedLayer = new CommittedChunkStorage(layer.LayerImage, layer.LayerImage.FindCommittedChunks());
        savedMask = new CommittedChunkStorage(layer.Mask, layer.Mask.FindCommittedChunks());
        return true;
    }

    public override OneOf<None, IChangeInfo, List<IChangeInfo>> Apply(Document target, bool firstApply, out bool ignoreInUndo)
    {
        var layer = target.FindMemberOrThrow<RasterLayer>(layerGuid);
        if (layer.Mask is null)
            throw new InvalidOperationException("Cannot apply layer mask, no mask");

        ChunkyImage newLayerImage = new ChunkyImage(target.Size);
        newLayerImage.AddRasterClip(layer.Mask);
        newLayerImage.EnqueueDrawChunkyImage(VecI.Zero, layer.LayerImage);
        newLayerImage.CommitChanges();

        var affectedChunks = layer.LayerImage.FindAllChunks();
        // use a temp value to ensure that LayerImage always stays in a valid state
        var toDispose = layer.LayerImage;
        layer.LayerImage = newLayerImage;
        toDispose.Dispose();

        var toDisposeMask = layer.Mask;
        layer.Mask = null;
        toDisposeMask.Dispose();

        ignoreInUndo = false;
        return new List<IChangeInfo>
        {
            new StructureMemberMask_ChangeInfo(layerGuid, false),
            new LayerImageArea_ChangeInfo(layerGuid, new AffectedArea(affectedChunks))
        };
    }

    public override OneOf<None, IChangeInfo, List<IChangeInfo>> Revert(Document target)
    {
        var layer = target.FindMemberOrThrow<RasterLayer>(layerGuid);
        if (layer.Mask is not null)
            throw new InvalidOperationException("Cannot restore layer mask, it already has one");
        if (savedLayer is null || savedMask is null)
            throw new InvalidOperationException("Cannot restore layer mask, no saved data");

        ChunkyImage newMask = new ChunkyImage(target.Size);
        savedMask.ApplyChunksToImage(newMask);
        var affectedChunksMask = newMask.FindAffectedArea();
        newMask.CommitChanges();
        layer.Mask = newMask;

        savedLayer.ApplyChunksToImage(layer.LayerImage);
        var affectedChunksLayer = layer.LayerImage.FindAffectedArea();
        layer.LayerImage.CommitChanges();

        return new List<IChangeInfo>
        {
            new StructureMemberMask_ChangeInfo(layerGuid, true),
            new LayerImageArea_ChangeInfo(layerGuid, affectedChunksLayer),
            new MaskArea_ChangeInfo(layerGuid, affectedChunksMask)
        };
    }

    public override void Dispose()
    {
        savedLayer?.Dispose();
        savedMask?.Dispose();
    }
}
