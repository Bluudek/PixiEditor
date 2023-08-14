﻿using PixiEditor.ChangeableDocument.Actions;
using PixiEditor.ChangeableDocument.ChangeInfos;

namespace PixiEditor.AvaloniaUI.Models.DocumentPassthroughActions;
internal record class SetSelectedMember_PassthroughAction(Guid GuidValue) : IAction, IChangeInfo;
