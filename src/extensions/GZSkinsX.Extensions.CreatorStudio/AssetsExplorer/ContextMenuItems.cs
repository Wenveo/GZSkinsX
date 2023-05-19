// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Collections.Generic;
using System.Composition;

using GZSkinsX.Api.ContextMenu;
using GZSkinsX.Api.Controls;

using Windows.System;

namespace GZSkinsX.Extensions.CreatorStudio.AssetsExplorer;

internal static class MenuItemConstants
{
    public const string TREEVIEW_GUID = "B0783923-1256-4D56-8286-2F37BF108EE7";

    public const string GROUP_A = "0,32633D93-D8CC-4FE3-AE11-36CD2C167D74";
    public const string GORUP_B = "1,AF855D86-10C7-4DE1-8E7D-B72309D48AFE";
    public const string GORUP_C = "2,F159A31B-546F-40C8-B4FE-095DB990DC5C";
}

[Shared, ExportContextMenuItem]
[ContextMenuItemMetadata(
    OwnerGuid = MenuItemConstants.TREEVIEW_GUID,
    Guid = "391D2605-30A2-4CA4-B2CB-ECFE7C23837B",
    Group = MenuItemConstants.GROUP_A)]
internal sealed class OpenInNewTab : ContextMenuItemBase
{
    public OpenInNewTab()
    {
        Header = "Open In New Tab";
        Icon = new SegoeFluentIcon { Glyph = "\uE8A7" };
        ShortcutKey = new ShortcutKey(VirtualKey.O, VirtualKeyModifiers.Control);
    }
}

[Shared, ExportContextMenuItem]
[ContextMenuItemMetadata(
    OwnerGuid = MenuItemConstants.TREEVIEW_GUID,
    Guid = "A9FAA7AB-AFF5-4385-918E-764D704E8B18",
    Group = MenuItemConstants.GORUP_B)]
internal sealed class TestItemA : ContextMenuItemBase
{
    public TestItemA()
    {
        Header = "TestItemA";
        Icon = new SegoeFluentIcon { Glyph = "\uE7F4" };
        ShortcutKey = new ShortcutKey(VirtualKey.A, VirtualKeyModifiers.Control);
    }
}

[Shared, ExportContextMenuItem]
[ContextMenuItemMetadata(
    OwnerGuid = "A9FAA7AB-AFF5-4385-918E-764D704E8B18",
    Guid = "DC7498CD-FB77-4F5F-97C2-09CA468384BC",
    Group = MenuItemConstants.GORUP_B)]
internal sealed class TestItemB : ContextToggleMenuItemBase
{
    public TestItemB()
    {
        Header = "TestItemB";
        Icon = new SegoeFluentIcon { Glyph = "\uE80A" };
        ShortcutKey = new ShortcutKey(VirtualKey.B, VirtualKeyModifiers.Control);
    }
}

[Shared, ExportContextMenuItem]
[ContextMenuItemMetadata(
    OwnerGuid = "A9FAA7AB-AFF5-4385-918E-764D704E8B18",
    Guid = "BB3A3C48-5F1B-47F0-8838-9489182DA36A",
    Group = MenuItemConstants.GORUP_B)]
internal sealed class TestItemC : ContextRadioMenuItemBase
{
    private int _count;

    public TestItemC()
    {
        Header = "TestItemC";
        Icon = new SegoeFluentIcon { Glyph = "\uE811" };
        ShortcutKey = new ShortcutKey(VirtualKey.C, VirtualKeyModifiers.Control);
    }

    public override bool IsVisible(IContextMenuUIContext context)
    {
        return _count++ % 2 == 0;
    }
}

[Shared, ExportContextMenuItem]
[ContextMenuItemMetadata(
    OwnerGuid = MenuItemConstants.TREEVIEW_GUID,
    Guid = "B3A8FA22-06AE-4148-9F4F-77ED218B9DDB",
    Group = MenuItemConstants.GORUP_C)]
internal sealed class TestSubItems : ContextMenuItemBase, IContextMenuItemProvider
{
    private sealed class SubMenuItemBase : ContextMenuItemBase
    {
        public SubMenuItemBase(string header, string glpyh)
        {
            Header = header;
            Icon = new SegoeFluentIcon { Glyph = glpyh };
        }
    }

    public IEnumerable<CreatedContextMenuItem> CreateSubItems()
    {
        yield return new CreatedContextMenuItem(new(), new SubMenuItemBase("SubItemA", "\uE7C2"));
        yield return new CreatedContextMenuItem(new(), new SubMenuItemBase("SubItemB", "\uE7C1"));
        yield return new CreatedContextMenuItem();

        yield return new CreatedContextMenuItem(new()
        {
            Guid = "68E71E76-FB79-45E2-B093-9FE6D8D7BA7C"
        }, new SubMenuItemBase("SubItemC", "\uE7B8"));

        yield return new CreatedContextMenuItem(new(), new SubMenuItemBase("SubItemD", "\uE791"));
    }

    public TestSubItems()
    {
        Header = "MenuSubItem";
        Icon = new SegoeFluentIcon { Glyph = "\uE14C" };
    }
}

[Shared, ExportContextMenuItem]
[ContextMenuItemMetadata(
    OwnerGuid = "68E71E76-FB79-45E2-B093-9FE6D8D7BA7C",
    Guid = "94546174-7BB4-46C7-9214-2BE072356F47")]
internal sealed class TestSubItemSubItem : ContextRadioMenuItemBase
{
    public TestSubItemSubItem()
    {
        Header = "TestSubItem - SubItem";
        Icon = new SegoeFluentIcon { Glyph = "\uE14A" };
    }
}
