// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GZSkinsX.Appx.Command;

/// <summary>
/// 扩展方法定义。
/// </summary>
internal static class ExtensionMethods
{
    /// <summary>
    /// 更新命令栏中的所有分隔符可见性。
    /// </summary>
    /// <param name="collection">命令栏中的元素合集。</param>
    public static void UpdateAllSeparatorsVisibily(this IList<ICommandBarElement> collection)
    {
        bool HasVisibleElement(int index, int length)
        {
            do
            {
                if (collection[index] is UIElement { Visibility: Visibility.Visible })
                {
                    return true;
                }
            } while (++index < length);

            return false;
        }

        var previousIndex = -1;
        var separatorCount = 0;
        for (var i = 0; i < collection.Count; i++)
        {
            var element = collection[i];
            if (element is AppBarSeparator separator)
            {
                // 以分隔符所处位置为准向上查找可见元素

                // 如果先前记录了上一个分隔符的索引位置
                // 那么则判断间距是否大于 1(包括分隔符)
                // 其次从上次分隔符的位置+1查找可见元素

                var spacing = i - previousIndex;
                if (spacing > 1 && HasVisibleElement(previousIndex + 1, i))
                {
                    separator.Visibility = Visibility.Visible;
                }
                else
                {
                    separator.Visibility = Visibility.Collapsed;
                }

                previousIndex = i;
                separatorCount++;
            }
        }

        // 判断索引是否超出数组范围
        if (previousIndex < 0 || previousIndex > collection.Count)
        {
            return;
        }

        // 如果到了最后的一个分隔符
        // 它的上下部分都会存在元素

        // 但是上面的方法只会向上查找
        // 所以在此处需要再往下进行判断
        // 以决定最后的这一个分隔符是否可见

        if (separatorCount > 1 && collection[previousIndex] is AppBarSeparator lastSeparator)
        {
            var position = previousIndex + 1;
            if (collection.Count > position && HasVisibleElement(position, collection.Count))
            {
                lastSeparator.Visibility = Visibility.Visible;
            }
            else
            {
                lastSeparator.Visibility = Visibility.Collapsed;
            }
        }
    }
}
