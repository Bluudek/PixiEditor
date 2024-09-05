﻿using PixiEditor.DrawingApi.Core.ColorsImpl;
using PixiEditor.DrawingApi.Core.Numerics;
using PixiEditor.Numerics;

namespace PixiEditor.ChangeableDocument.Changes.Drawing;
internal class DrawEllipse_UpdateableChange : UpdateableChange
{
    private readonly Guid memberGuid;
    private RectI location;
    private double rotation;
    private readonly Color strokeColor;
    private readonly Color fillColor;
    private readonly int strokeWidth;
    private readonly bool drawOnMask;
    private int frame;

    private CommittedChunkStorage? storedChunks;

    [GenerateUpdateableChangeActions]
    public DrawEllipse_UpdateableChange(Guid memberGuid, RectI location, double rotationRad, Color strokeColor, Color fillColor, int strokeWidth, bool drawOnMask, int frame)
    {
        this.memberGuid = memberGuid;
        this.location = location;
        this.rotation = rotationRad;
        this.strokeColor = strokeColor;
        this.fillColor = fillColor;
        this.strokeWidth = strokeWidth;
        this.drawOnMask = drawOnMask;
        this.frame = frame;
    }

    [UpdateChangeMethod]
    public void Update(RectI location, double rotationRad)
    {
        this.location = location;
        rotation = rotationRad;
    }

    public override bool InitializeAndValidate(Document target)
    {
        return DrawingChangeHelper.IsValidForDrawing(target, memberGuid, drawOnMask);
    }

    private AffectedArea UpdateEllipse(Document target, ChunkyImage targetImage)
    {
        var oldAffectedChunks = targetImage.FindAffectedArea();

        targetImage.CancelChanges();

        if (!location.IsZeroOrNegativeArea)
        {
            DrawingChangeHelper.ApplyClipsSymmetriesEtc(target, targetImage, memberGuid, drawOnMask);
            targetImage.EnqueueDrawEllipse(location, strokeColor, fillColor, strokeWidth, rotation);
        }

        var affectedArea = targetImage.FindAffectedArea();
        affectedArea.UnionWith(oldAffectedChunks);

        return affectedArea;
    }

    public override OneOf<None, IChangeInfo, List<IChangeInfo>> Apply(Document target, bool firstApply, out bool ignoreInUndo)
    {
        if (location.IsZeroOrNegativeArea)
        {
            ignoreInUndo = true;
            return new None();
        }

        var image = DrawingChangeHelper.GetTargetImageOrThrow(target, memberGuid, drawOnMask, frame);
        var area = UpdateEllipse(target, image);
        storedChunks = new CommittedChunkStorage(image, image.FindAffectedArea().Chunks);
        image.CommitChanges();
        ignoreInUndo = false;
        return DrawingChangeHelper.CreateAreaChangeInfo(memberGuid, area, drawOnMask);
    }

    public override OneOf<None, IChangeInfo, List<IChangeInfo>> ApplyTemporarily(Document target)
    {
        var image = DrawingChangeHelper.GetTargetImageOrThrow(target, memberGuid, drawOnMask, frame);
        var area = UpdateEllipse(target, image);
        return DrawingChangeHelper.CreateAreaChangeInfo(memberGuid, area, drawOnMask);
    }

    public override OneOf<None, IChangeInfo, List<IChangeInfo>> Revert(Document target)
    {
        var affArea = DrawingChangeHelper.ApplyStoredChunksDisposeAndSetToNull(target, memberGuid, drawOnMask, frame, ref storedChunks);
        var changes = DrawingChangeHelper.CreateAreaChangeInfo(memberGuid, affArea, drawOnMask);
        return changes;
    }

    public override void Dispose()
    {
        storedChunks?.Dispose();
    }
}
