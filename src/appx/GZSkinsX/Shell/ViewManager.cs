// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GZSkinsX.Api.Shell;

using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Shell;


internal sealed class ViewManager : IViewManager
{
    private readonly Frame _frame;

    public bool CanGoBack => _frame.CanGoBack;

    public bool CanGoForward => _frame.CanGoForward;

    public ViewManager()
    {
        _frame = new Frame();
    }

    public void GoBack()
    {
        if (CanGoBack)
        {
            _frame.GoBack();
        }
    }

    public void GoForward()
    {
        throw new NotImplementedException();
    }

    public void NavigateTo(Guid elemGuid)
    {
        throw new NotImplementedException();
    }
}
