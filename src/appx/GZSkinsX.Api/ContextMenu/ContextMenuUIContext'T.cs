// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Api.ContextMenu;

public class ContextMenuUIContext<T1, T2> : ContextMenuUIContext, IContextMenuUIContext<T1, T2>
{
    public new T1 UIObject => ((T1)base.UIObject)!;

    public new T2 Parameter => ((T2)base.Parameter)!;

    T1 IContextMenuUIContext<T1, T2>.UIObject => ((T1)base.UIObject)!;

    T2 IContextMenuUIContext<T1, T2>.Parameter => ((T2)base.Parameter)!;

    public ContextMenuUIContext(T1 uiObject, T2 parameter)
        : base(uiObject!, parameter!) { }
}
