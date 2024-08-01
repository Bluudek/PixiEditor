﻿using Avalonia.Input;
using PixiEditor.Models.Handlers;

namespace PixiEditor.Models.Commands.Commands;

internal partial class Command
{
    internal class ToolCommand : Command
    {
        public Type ToolType { get; init; }

        public Key TransientKey { get; init; }

        public override object GetParameter() => ToolType;

        public ToolCommand(IToolsHandler handler) : base(handler.SetTool, CommandController.Current.CanExecuteEvaluators["PixiEditor.HasDocument"]) { }
    }
}
