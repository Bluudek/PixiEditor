﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PixiEditor.Extensions.CommonApi.Palettes;
using PixiEditor.Models.DataHolders;
using BackendColor = PixiEditor.DrawingApi.Core.ColorsImpl.Color;

namespace PixiEditor.Views.UserControls.Palettes;

/// <summary>
/// Interaction logic for CompactPaletteViewer.xaml
/// </summary>
internal partial class CompactPaletteViewer : UserControl
{
    public static readonly DependencyProperty ColorsProperty = DependencyProperty.Register(nameof(Colors), typeof(WpfObservableRangeCollection<PaletteColor>), typeof(CompactPaletteViewer));

    public WpfObservableRangeCollection<PaletteColor> Colors
    {
        get { return (WpfObservableRangeCollection<PaletteColor>)GetValue(ColorsProperty); }
        set { SetValue(ColorsProperty, value); }
    }

    public ICommand SelectColorCommand
    {
        get { return (ICommand)GetValue(SelectColorCommandProperty); }
        set { SetValue(SelectColorCommandProperty, value); }
    }


    public static readonly DependencyProperty SelectColorCommandProperty =
        DependencyProperty.Register(nameof(SelectColorCommand), typeof(ICommand), typeof(CompactPaletteViewer));

    public CompactPaletteViewer()
    {
        InitializeComponent();
    }

    private void RemoveColorMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        MenuItem menuItem = (MenuItem)sender;
        PaletteColor color = (PaletteColor)menuItem.CommandParameter;
        if (Colors.Contains(color))
        {
            Colors.Remove(color);
        }
    }
}
