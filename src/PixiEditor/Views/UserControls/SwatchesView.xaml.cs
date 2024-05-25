﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PixiEditor.DrawingApi.Core.ColorsImpl;
using PixiEditor.Extensions.CommonApi.Palettes;

namespace PixiEditor.Views.UserControls;

internal partial class SwatchesView : UserControl
{
    public static readonly DependencyProperty SwatchesProperty =
        DependencyProperty.Register(nameof(Swatches), typeof(ObservableCollection<PaletteColor>), typeof(SwatchesView));

    public ObservableCollection<PaletteColor> Swatches
    {
        get => (ObservableCollection<PaletteColor>)GetValue(SwatchesProperty);
        set => SetValue(SwatchesProperty, value);
    }

    public static readonly DependencyProperty MouseDownCommandProperty =
        DependencyProperty.Register(nameof(MouseDownCommand), typeof(ICommand), typeof(SwatchesView));

    public ICommand MouseDownCommand
    {
        get => (ICommand)GetValue(MouseDownCommandProperty);
        set => SetValue(MouseDownCommandProperty, value);
    }

    public static readonly DependencyProperty SelectSwatchCommandProperty =
        DependencyProperty.Register(nameof(SelectSwatchCommand), typeof(ICommand), typeof(SwatchesView));

    public ICommand SelectSwatchCommand
    {
        get => (ICommand)GetValue(SelectSwatchCommandProperty);
        set => SetValue(SelectSwatchCommandProperty, value);
    }

    public SwatchesView()
    {
        InitializeComponent();
    }
}
