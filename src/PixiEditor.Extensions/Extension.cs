﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PixiEditor.Extensions.Metadata;

namespace PixiEditor.Extensions;

/// <summary>
///     This class is used to extend the functionality of the PixiEditor.
/// </summary>
public abstract class Extension
{
    public ExtensionServices Api { get; private set; }
    public ExtensionMetadata Metadata { get; private set; }
    public Assembly Assembly => GetType().Assembly;
    public virtual string Location => Assembly.Location;

    public void ProvideMetadata(ExtensionMetadata metadata)
    {
        if (Metadata != null)
        {
            return;
        }

        Metadata = metadata;
    }

    public void Load()
    {
        OnLoaded();
    }

    public void Initialize(ExtensionServices api)
    {
        Api = api;
        OnInitialized();
    }

    /// <summary>
    ///     Called right after the extension is loaded. Not all extensions are initialized at this point. PixiEditor API at this point is not available.
    ///     Use this method to load resources, patch language files, etc.
    /// </summary>
    protected virtual void OnLoaded()
    {
    }

    /// <summary>
    ///     Called after all extensions and PixiEditor is loaded. All extensions are initialized at this point.
    /// </summary>
    protected virtual void OnInitialized()
    {
    }
}
