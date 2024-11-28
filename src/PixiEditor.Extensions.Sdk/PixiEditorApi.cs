﻿using PixiEditor.Extensions.CommonApi.Windowing;
using PixiEditor.Extensions.Sdk.Api;
using PixiEditor.Extensions.Sdk.Api.Logging;
using PixiEditor.Extensions.Sdk.Api.Palettes;
using PixiEditor.Extensions.Sdk.Api.UserPreferences;
using PixiEditor.Extensions.Sdk.Api.Window;

namespace PixiEditor.Extensions.Sdk;

public class PixiEditorApi
{
    public Logger Logger { get; }
    public WindowProvider WindowProvider { get; }
    public Preferences Preferences { get; }
    public PalettesProvider Palettes { get; }

    public PixiEditorApi()
    {
        Logger = new Logger();
        WindowProvider = new WindowProvider();
        Preferences = new Preferences();
        Palettes = new PalettesProvider();
    }
}
