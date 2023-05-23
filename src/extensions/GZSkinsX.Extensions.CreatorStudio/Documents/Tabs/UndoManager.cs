// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Collections.Generic;

using GZSkinsX.Api.CreatorStudio.Documents;
using GZSkinsX.Api.CreatorStudio.Documents.Tabs;

namespace GZSkinsX.Extensions.CreatorStudio.Documents.Tabs;

internal sealed class UndoManager : IUndoManager, IEquatable<UndoManager>
{
    private readonly IDocumentKey _key;

    private readonly Stack<IUndoCommand> _redoCommands;
    private readonly Stack<IUndoCommand> _undoCommands;

    public bool CanRedo => _undoCommands.Count > 0;

    public bool CanUndo => _redoCommands.Count > 0;

    public UndoManager(IDocumentKey key)
    {
        _key = key;

        _redoCommands = new();
        _undoCommands = new();
    }

    public void Add(IUndoCommand command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        command.Execute();
        Clear(_redoCommands);
        _undoCommands.Push(command);
    }

    public void Clear()
    {
        Clear(_redoCommands);
        Clear(_undoCommands);
    }

    private void Clear(Stack<IUndoCommand> stack)
    {
        if (stack.Count == 0)
        {
            return;
        }

        foreach (var item in stack)
        {
            if (item is not IDisposable disposable)
            {
                continue;
            }

            disposable.Dispose();
        }

        stack.Clear();
    }

    public void Redo()
    {
        if (_redoCommands.Count is 0)
        {
            return;
        }

        var redoCommand = _redoCommands.Pop();
        redoCommand.Execute();
        _undoCommands.Push(redoCommand);
    }

    public void Undo()
    {
        if (_undoCommands.Count is 0)
        {
            return;
        }

        var undoCommand = _undoCommands.Pop();
        undoCommand.Execute();
        _redoCommands.Push(undoCommand);
    }

    public bool Equals(UndoManager? other)
    {
        return other != null && other._key.Equals(_key);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as UndoManager);
    }

    public override int GetHashCode()
    {
        return _key.GetHashCode();
    }
}
