﻿using PixiEditor.DrawingApi.Core.Numerics;

namespace PixiEditor.AvaloniaUI.Helpers;

public static class StructureHelpers
{
    public const int PreviewSize = 48;
    /// <summary>
    /// Calculates the size of a scaled-down preview for a given size of layer tight bounds.
    /// </summary>
    public static VecI CalculatePreviewSize(VecI tightBoundsSize)
    {
        double proportions = tightBoundsSize.Y / (double)tightBoundsSize.X;
        const int prSize = PreviewSize;
        return proportions > 1 ?
            new VecI(Math.Max((int)Math.Round(prSize / proportions), 1), prSize) :
            new VecI(prSize, Math.Max((int)Math.Round(prSize * proportions), 1));
    }
}
