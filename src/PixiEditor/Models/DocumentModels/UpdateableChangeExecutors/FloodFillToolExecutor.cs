﻿using PixiEditor.ChangeableDocument.Actions.Undo;
using PixiEditor.DrawingApi.Core.ColorsImpl;
using PixiEditor.DrawingApi.Core.Numerics;
using PixiEditor.Extensions.Palettes;
using PixiEditor.Models.Enums;
using PixiEditor.Numerics;
using PixiEditor.ViewModels.SubViewModels.Document;
using PixiEditor.ViewModels.SubViewModels.Tools.Tools;

namespace PixiEditor.Models.DocumentModels.UpdateableChangeExecutors;
#nullable enable
internal class FloodFillToolExecutor : UpdateableChangeExecutor
{
    private bool considerAllLayers;
    private bool drawOnMask;
    private Guid memberGuid;
    private Color color;

    public override ExecutionState Start()
    {
        var fillTool = ViewModelMain.Current?.ToolsSubViewModel.GetTool<FloodFillToolViewModel>();
        ColorsViewModel? colorsVM = ViewModelMain.Current?.ColorsSubViewModel;
        var member = document!.SelectedStructureMember;

        if (fillTool is null || member is null || colorsVM is null)
            return ExecutionState.Error;
        drawOnMask = member is LayerViewModel layer ? layer.ShouldDrawOnMask : true;
        if (drawOnMask && !member.HasMaskBindable)
            return ExecutionState.Error;
        if (!drawOnMask && member is not LayerViewModel)
            return ExecutionState.Error;

        colorsVM.AddSwatch(new PaletteColor(color.R, color.G, color.B));
        memberGuid = member.GuidValue;
        considerAllLayers = fillTool.ConsiderAllLayers;
        color = colorsVM.PrimaryColor;
        var pos = controller!.LastPixelPosition;

        internals!.ActionAccumulator.AddActions(new FloodFill_Action(memberGuid, pos, color, considerAllLayers, drawOnMask));

        return ExecutionState.Success;
    }

    public override void OnPixelPositionChange(VecI pos)
    {
        internals!.ActionAccumulator.AddActions(new FloodFill_Action(memberGuid, pos, color, considerAllLayers, drawOnMask));
    }

    public override void OnLeftMouseButtonUp()
    {
        internals!.ActionAccumulator.AddActions(new ChangeBoundary_Action());
        onEnded!(this);
    }

    public override void ForceStop()
    {
        internals!.ActionAccumulator.AddActions(new ChangeBoundary_Action());
    }
}
