﻿using PixiEditor.DrawingApi.Core.Numerics;
using PixiEditor.DrawingApi.Core.Surface;
using PixiEditor.DrawingApi.Core.Surface.PaintImpl;

namespace PixiEditor.ChangeableDocument.Changes.Drawing;
internal class PasteImage_UpdateableChange : UpdateableChange
{
    private ShapeCorners corners;
    private readonly Guid memberGuid;
    private readonly bool ignoreClipsSymmetriesEtc;
    private readonly bool drawOnMask;
    private readonly Surface imageToPaste;
    private CommittedChunkStorage? savedChunks;
    private int frame;
    private static Paint RegularPaint { get; } = new Paint() { BlendMode = BlendMode.SrcOver };

    private bool hasEnqueudImage = false;

    [GenerateUpdateableChangeActions]
    public PasteImage_UpdateableChange(Surface image, ShapeCorners corners, Guid memberGuid, bool ignoreClipsSymmetriesEtc, bool isDrawingOnMask, int frame)
    {
        this.corners = corners;
        this.memberGuid = memberGuid;
        this.ignoreClipsSymmetriesEtc = ignoreClipsSymmetriesEtc;
        this.drawOnMask = isDrawingOnMask;
        this.imageToPaste = new Surface(image);
    }

    public override bool InitializeAndValidate(Document target)
    {
        return DrawingChangeHelper.IsValidForDrawing(target, memberGuid, drawOnMask);
    }

    [UpdateChangeMethod]
    public void Update(ShapeCorners corners)
    {
        this.corners = corners;
    }

    private AffectedArea DrawImage(Document target, ChunkyImage targetImage)
    {
        var prevAffArea = targetImage.FindAffectedArea();

        targetImage.CancelChanges();
        if (!ignoreClipsSymmetriesEtc)
            DrawingChangeHelper.ApplyClipsSymmetriesEtc(target, targetImage, memberGuid, drawOnMask);
        targetImage.EnqueueDrawImage(corners, imageToPaste, RegularPaint, false);
        hasEnqueudImage = true;

        var affArea = targetImage.FindAffectedArea();
        affArea.UnionWith(prevAffArea);
        return affArea;
    }

    public override OneOf<None, IChangeInfo, List<IChangeInfo>> Apply(Document target, bool firstApply, out bool ignoreInUndo)
    {
        ChunkyImage targetImage = DrawingChangeHelper.GetTargetImageOrThrow(target, memberGuid, drawOnMask, frame);
        var chunks = DrawImage(target, targetImage);
        savedChunks?.Dispose();
        savedChunks = new(targetImage, targetImage.FindAffectedArea().Chunks);
        targetImage.CommitChanges();
        hasEnqueudImage = false;
        ignoreInUndo = false;
        return DrawingChangeHelper.CreateAreaChangeInfo(memberGuid, chunks, drawOnMask);
    }

    public override OneOf<None, IChangeInfo, List<IChangeInfo>> ApplyTemporarily(Document target)
    {
        ChunkyImage targetImage = DrawingChangeHelper.GetTargetImageOrThrow(target, memberGuid, drawOnMask, frame);
        return DrawingChangeHelper.CreateAreaChangeInfo(memberGuid, DrawImage(target, targetImage), drawOnMask);
    }

    public override OneOf<None, IChangeInfo, List<IChangeInfo>> Revert(Document target)
    {
        var chunks = DrawingChangeHelper.ApplyStoredChunksDisposeAndSetToNull(target, memberGuid, drawOnMask, frame, ref savedChunks);
        return DrawingChangeHelper.CreateAreaChangeInfo(memberGuid, chunks, drawOnMask);
    }

    public override void Dispose()
    {
        if (hasEnqueudImage)
            throw new InvalidOperationException("Attempted to dispose the change while it's internally stored image is still used enqueued in some ChunkyImage. Most likely someone tried to dispose a change after ApplyTemporarily was called but before the subsequent call to Apply. Don't do that.");
        imageToPaste.Dispose();
        savedChunks?.Dispose();
    }
}
