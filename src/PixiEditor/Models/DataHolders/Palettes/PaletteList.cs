﻿using PixiEditor.Helpers;

namespace PixiEditor.Models.DataHolders.Palettes;

internal sealed class PaletteList : NotifyableObject
{
    public bool FetchedCorrectly { get; set; } = false;
    public WpfObservableRangeCollection<Palette> Palettes { get; set; } = new WpfObservableRangeCollection<Palette>();
}
