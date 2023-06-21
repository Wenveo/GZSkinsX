// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Numerics;

using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Shapes;

namespace GZSkinsX.Api.Composition;

public static partial class CompositionExtensions
{
    public static InsetClip ClipToBounds(this UIElement element)
    {
        var visual = GetElementVisual(element);
        var insetClip = visual.Compositor.CreateInsetClip();
        visual.Clip = insetClip;
        return insetClip;
    }

    public static CompositionBrush? GetAlphaMask(this UIElement element) => element switch
    {
        TextBlock t => t.GetAlphaMask(),
        Shape s => s.GetAlphaMask(),
        Image i => i.GetAlphaMask(),
        _ => null,
    };

    public static ContainerVisual GetContainerVisual(this UIElement element)
    {
        if (ElementCompositionPreview.GetElementChildVisual(element) is ContainerVisual container)
        {
            return container;
        }

        // Create a new container visual, link it's size to the element's and then set
        // the container as the child visual of the element.
        var elementVisual = GetElementVisual(element);
        container = elementVisual.Compositor.CreateContainerVisual();
        container.LinkSize(elementVisual);
        element.SetChildVisual(container);
        return container;
    }

    public static Visual GetElementVisual(this UIElement element)
     => ElementCompositionPreview.GetElementVisual(element);

    public static IEnumerable<Visual> GetVisuals<T>(this IEnumerable<T> elements) where T : UIElement
    {
        foreach (var item in elements)
        {
            yield return GetElementVisual(item);
        }
    }

    public static CompositionPropertySet GetScrollManipulationPropertySet(this ScrollViewer scrollViewer)
    {
        return ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
    }

    public static bool IsTranslationEnabled(this UIElement element)
    {
        return GetElementVisual(element).Properties.TryGetVector3(CompositionFactory.TRANSLATION, out _) != CompositionGetValueStatus.NotFound;
    }

    public static T SetChildVisual<T>(this T element, Visual? visual) where T : UIElement
    {
        ElementCompositionPreview.SetElementChildVisual(element, visual);
        return element;
    }

    public static void SetHideAnimation<T>(this T element, ICompositionAnimationBase? animation) where T : UIElement
    {
        ElementCompositionPreview.SetImplicitHideAnimation(element, animation);
    }

    public static void SetShowAnimation<T>(this T element, ICompositionAnimationBase? animation) where T : UIElement
    {
        ElementCompositionPreview.SetImplicitShowAnimation(element, animation);
    }

    public static UIElement SetTranslation<T>(this T element, Vector3 value) where T : UIElement
    {
        element.GetElementVisual().Properties.InsertVector3("Translation", value);
        return element;
    }

    public static bool SupportsAlphaMask(this UIElement element) => element switch
    {
        TextBlock or Shape or Image => true,
        _ => false,
    };
}
