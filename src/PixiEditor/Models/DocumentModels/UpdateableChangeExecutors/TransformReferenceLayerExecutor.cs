﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChunkyImageLib.DataHolders;
using PixiEditor.DrawingApi.Core.Numerics;
using PixiEditor.Models.Enums;
using PixiEditor.Numerics;

namespace PixiEditor.Models.DocumentModels.UpdateableChangeExecutors;
internal class TransformReferenceLayerExecutor : UpdateableChangeExecutor
{
    public override ExecutionState Start()
    {
        if (document!.ReferenceLayerViewModel.ReferenceBitmap is null)
            return ExecutionState.Error;

        ShapeCorners corners = document.ReferenceLayerViewModel.ReferenceShapeBindable;
        document.TransformViewModel.ShowTransform(DocumentTransformMode.Scale_Rotate_Shear_NoPerspective, true, corners, true);
        document.ReferenceLayerViewModel.IsTransforming = true;
        internals!.ActionAccumulator.AddActions(new TransformReferenceLayer_Action(corners));
        return ExecutionState.Success;
    }

    public override void OnTransformMoved(ShapeCorners corners)
    {
        internals!.ActionAccumulator.AddActions(new TransformReferenceLayer_Action(corners));
    }

    public override void OnSelectedObjectNudged(VecI distance) => document!.TransformViewModel.Nudge(distance);

    public override void OnMidChangeUndo() => document!.TransformViewModel.Undo();

    public override void OnMidChangeRedo() => document!.TransformViewModel.Redo();

    public override void OnTransformApplied()
    {
        internals!.ActionAccumulator.AddFinishedActions(new EndTransformReferenceLayer_Action());
        document!.TransformViewModel.HideTransform();
        document.ReferenceLayerViewModel.IsTransforming = false;
        onEnded!.Invoke(this);
    }

    public override void ForceStop()
    {
        internals!.ActionAccumulator.AddFinishedActions(new EndTransformReferenceLayer_Action());
        document!.TransformViewModel.HideTransform();
        document.ReferenceLayerViewModel.IsTransforming = false;
    }
}
