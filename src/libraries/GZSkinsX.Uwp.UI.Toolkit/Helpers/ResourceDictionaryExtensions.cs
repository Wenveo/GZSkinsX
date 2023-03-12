// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI.Xaml;

namespace GZSkinsX.Uwp.UI.Toolkit;

// Adapted from https://github.com/rudyhuyn/XamlPlus
internal static class ResourceDictionaryExtensions
{
    /// <summary>
    /// Copies  the <see cref="ResourceDictionary"/> provided as a parameter into the calling dictionary, includes overwriting the source location, theme dictionaries, and merged dictionaries.
    /// </summary>
    /// <param name="destination">ResourceDictionary to copy values to.</param>
    /// <param name="source">ResourceDictionary to copy values from.</param>
    internal static void CopyFrom(this ResourceDictionary destination, ResourceDictionary source)
    {
        if (source.Source != null)
        {
            destination.Source = source.Source;
        }
        else
        {
            // Clone theme dictionaries
            if (source.ThemeDictionaries != null)
            {
                foreach (var theme in source.ThemeDictionaries)
                {
                    if (theme.Value is ResourceDictionary themedResource)
                    {
                        var themeDictionary = new ResourceDictionary();
                        themeDictionary.CopyFrom(themedResource);
                        destination.ThemeDictionaries[theme.Key] = themeDictionary;
                    }
                    else
                    {
                        destination.ThemeDictionaries[theme.Key] = theme.Value;
                    }
                }
            }

            // Clone merged dictionaries
            if (source.MergedDictionaries != null)
            {
                foreach (var mergedResource in source.MergedDictionaries)
                {
                    var themeDictionary = new ResourceDictionary();
                    themeDictionary.CopyFrom(mergedResource);
                    destination.MergedDictionaries.Add(themeDictionary);
                }
            }

            // Clone all contents
            foreach (var item in source)
            {
                destination[item.Key] = item.Value;
            }
        }
    }
}
