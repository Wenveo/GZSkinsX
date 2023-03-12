// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Linq;

using Windows.UI.Xaml;

namespace GZSkinsX.Uwp.UI.Toolkit;

// Adapted from https://github.com/rudyhuyn/XamlPlus
public static partial class StyleExtensions
{
    // Used to distinct normal ResourceDictionary and the one we add.
    private sealed class StyleExtensionResourceDictionary : ResourceDictionary
    {
    }

    public static ResourceDictionary GetResources(Style obj)
    => (ResourceDictionary)obj.GetValue(ResourcesProperty);

    public static void SetResources(Style obj, ResourceDictionary value)
    => obj.SetValue(ResourcesProperty, value);

    public static readonly DependencyProperty ResourcesProperty =
        DependencyProperty.RegisterAttached("Resources", typeof(ResourceDictionary),
            typeof(StyleExtensions), new PropertyMetadata(null, ResourcesChanged));

    private static void ResourcesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not FrameworkElement frameworkElement)
        {
            return;
        }

        var mergedDictionaries = frameworkElement.Resources?.MergedDictionaries;
        if (mergedDictionaries == null)
        {
            return;
        }

        var existingResourceDictionary =
            mergedDictionaries.FirstOrDefault(c => c is StyleExtensionResourceDictionary);
        if (existingResourceDictionary != null)
        {
            // Remove the existing resource dictionary
            mergedDictionaries.Remove(existingResourceDictionary);
        }

        if (e.NewValue is ResourceDictionary resource)
        {
            var clonedResources = new StyleExtensionResourceDictionary();
            clonedResources.CopyFrom(resource);
            mergedDictionaries.Add(clonedResources);
        }

        if (frameworkElement.IsLoaded)
        {
            // Only force if the style was applied after the control was loaded
            ForceControlToReloadThemeResources(frameworkElement);
        }
    }

    private static void ForceControlToReloadThemeResources(FrameworkElement frameworkElement)
    {
        // To force the refresh of all resource references.
        // Note: Doesn't work when in high-contrast.
        var currentRequestedTheme = frameworkElement.RequestedTheme;
        frameworkElement.RequestedTheme = currentRequestedTheme == ElementTheme.Dark
            ? ElementTheme.Light
            : ElementTheme.Dark;
        frameworkElement.RequestedTheme = currentRequestedTheme;
    }
}
