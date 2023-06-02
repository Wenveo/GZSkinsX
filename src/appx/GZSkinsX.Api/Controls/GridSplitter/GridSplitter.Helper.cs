// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GZSkinsX.SDK.Controls;

/// <summary>
/// Represents the control that redistributes space between columns or rows of a Grid control.
/// </summary>
public partial class GridSplitter
{
    private static bool IsStarColumn(ColumnDefinition definition)
    {
        return ((GridLength)definition.GetValue(ColumnDefinition.WidthProperty)).IsStar;
    }

    private static bool IsStarRow(RowDefinition definition)
    {
        return ((GridLength)definition.GetValue(RowDefinition.HeightProperty)).IsStar;
    }

    private bool SetColumnWidth(ColumnDefinition columnDefinition, double horizontalChange, GridUnitType unitType)
    {
        var newWidth = columnDefinition.ActualWidth + horizontalChange;

        var minWidth = columnDefinition.MinWidth;
        if (!double.IsNaN(minWidth) && newWidth < minWidth)
        {
            newWidth = minWidth;
        }

        var maxWidth = columnDefinition.MaxWidth;
        if (!double.IsNaN(maxWidth) && newWidth > maxWidth)
        {
            newWidth = maxWidth;
        }

        if (newWidth > ActualWidth)
        {
            columnDefinition.Width = new GridLength(newWidth, unitType);
            return true;
        }

        return false;
    }

    private bool IsValidColumnWidth(ColumnDefinition columnDefinition, double horizontalChange)
    {
        var newWidth = columnDefinition.ActualWidth + horizontalChange;

        var minWidth = columnDefinition.MinWidth;
        if (!double.IsNaN(minWidth) && newWidth < minWidth)
        {
            return false;
        }

        var maxWidth = columnDefinition.MaxWidth;
        if (!double.IsNaN(maxWidth) && newWidth > maxWidth)
        {
            return false;
        }

        if (newWidth <= ActualWidth)
        {
            return false;
        }

        return true;
    }

    private bool SetRowHeight(RowDefinition rowDefinition, double verticalChange, GridUnitType unitType)
    {
        var newHeight = rowDefinition.ActualHeight + verticalChange;

        var minHeight = rowDefinition.MinHeight;
        if (!double.IsNaN(minHeight) && newHeight < minHeight)
        {
            newHeight = minHeight;
        }

        var maxWidth = rowDefinition.MaxHeight;
        if (!double.IsNaN(maxWidth) && newHeight > maxWidth)
        {
            newHeight = maxWidth;
        }

        if (newHeight > ActualHeight)
        {
            rowDefinition.Height = new GridLength(newHeight, unitType);
            return true;
        }

        return false;
    }

    private bool IsValidRowHeight(RowDefinition rowDefinition, double verticalChange)
    {
        var newHeight = rowDefinition.ActualHeight + verticalChange;

        var minHeight = rowDefinition.MinHeight;
        if (!double.IsNaN(minHeight) && newHeight < minHeight)
        {
            return false;
        }

        var maxHeight = rowDefinition.MaxHeight;
        if (!double.IsNaN(maxHeight) && newHeight > maxHeight)
        {
            return false;
        }

        if (newHeight <= ActualHeight)
        {
            return false;
        }

        return true;
    }

    // Return the targeted Column based on the resize behavior
    private int GetTargetedColumn()
    {
        var currentIndex = Grid.GetColumn(TargetControl);
        return GetTargetIndex(currentIndex);
    }

    // Return the sibling Row based on the resize behavior
    private int GetTargetedRow()
    {
        var currentIndex = Grid.GetRow(TargetControl);
        return GetTargetIndex(currentIndex);
    }

    // Return the sibling Column based on the resize behavior
    private int GetSiblingColumn()
    {
        var currentIndex = Grid.GetColumn(TargetControl);
        return GetSiblingIndex(currentIndex);
    }

    // Return the sibling Row based on the resize behavior
    private int GetSiblingRow()
    {
        var currentIndex = Grid.GetRow(TargetControl);
        return GetSiblingIndex(currentIndex);
    }

    // Gets index based on resize behavior for first targeted row/column
    private int GetTargetIndex(int currentIndex)
    {
        return _resizeBehavior switch
        {
            GridResizeBehavior.CurrentAndNext => currentIndex,
            GridResizeBehavior.PreviousAndNext => currentIndex - 1,
            GridResizeBehavior.PreviousAndCurrent => currentIndex - 1,
            _ => -1,
        };
    }

    // Gets index based on resize behavior for second targeted row/column
    private int GetSiblingIndex(int currentIndex)
    {
        return _resizeBehavior switch
        {
            GridResizeBehavior.CurrentAndNext => currentIndex + 1,
            GridResizeBehavior.PreviousAndNext => currentIndex + 1,
            GridResizeBehavior.PreviousAndCurrent => currentIndex,
            _ => -1,
        };
    }

    // Checks the control alignment and Width/Height to detect the control resize direction columns/rows
    private GridResizeDirection GetResizeDirection()
    {
        var direction = ResizeDirection;

        if (direction == GridResizeDirection.Auto)
        {
            // When HorizontalAlignment is Left, Right or Center, resize Columns
            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                direction = GridResizeDirection.Columns;
            }

            // When VerticalAlignment is Top, Bottom or Center, resize Rows
            else if (VerticalAlignment != VerticalAlignment.Stretch)
            {
                direction = GridResizeDirection.Rows;
            }

            // Check Width vs Height
            else
            {
                direction = ActualWidth <= ActualHeight ? GridResizeDirection.Columns : GridResizeDirection.Rows;
            }
        }

        return direction;
    }

    // Get the resize behavior (Which columns/rows should be resized) based on alignment and Direction
    private GridResizeBehavior GetResizeBehavior()
    {
        var resizeBehavior = ResizeBehavior;

        if (resizeBehavior == GridResizeBehavior.BasedOnAlignment)
        {
            if (_resizeDirection == GridResizeDirection.Columns)
            {
                resizeBehavior = HorizontalAlignment switch
                {
                    HorizontalAlignment.Left => GridResizeBehavior.PreviousAndCurrent,
                    HorizontalAlignment.Right => GridResizeBehavior.CurrentAndNext,
                    _ => GridResizeBehavior.PreviousAndNext,
                };
            }

            // resize direction is vertical
            else
            {
                resizeBehavior = VerticalAlignment switch
                {
                    VerticalAlignment.Top => GridResizeBehavior.PreviousAndCurrent,
                    VerticalAlignment.Bottom => GridResizeBehavior.CurrentAndNext,
                    _ => GridResizeBehavior.PreviousAndNext,
                };
            }
        }

        return resizeBehavior;
    }
}
