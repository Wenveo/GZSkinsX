// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System.Collections.Generic;
using System.Numerics;

using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Shapes;

namespace GZSkinsX.Uwp.Composition;

public static partial class CompositionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static InsetClip ClipToBounds(this UIElement element)
    {
        var visual = GetElementVisual(element);
        var insetClip = visual.Compositor.CreateInsetClip();
        visual.Clip = insetClip;
        return insetClip;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static UIElement EnableCompositionTranslation(this UIElement element)
    {
        return EnableCompositionTranslation(element, null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static UIElement EnableCompositionTranslation(this UIElement element, float x, float y, float z)
    {
        return EnableCompositionTranslation(element, new Vector3(x, y, z));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="defaultTranslation"></param>
    /// <returns></returns>
    public static UIElement EnableCompositionTranslation(this UIElement element, Vector3? defaultTranslation)
    {
        var visual = GetElementVisual(element);
        if (visual.Properties.TryGetVector3(CompositionFactory.TRANSLATION, out _) == CompositionGetValueStatus.NotFound)
        {
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            if (defaultTranslation.HasValue)
            {
                visual.Properties.InsertVector3(CompositionFactory.TRANSLATION, defaultTranslation.Value);
            }
            else
            {
                visual.Properties.InsertVector3(CompositionFactory.TRANSLATION, new Vector3());
            }
        }

        return element;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="enable"></param>
    /// <returns></returns>
    public static UIElement EnableTranslation(this UIElement element, bool enable)
    {
        ElementCompositionPreview.SetIsTranslationEnabled(element, enable);
        return element;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static CompositionBrush? GetAlphaMask(this UIElement element) => element switch
    {
        TextBlock t => t.GetAlphaMask(),
        Shape s => s.GetAlphaMask(),
        Image i => i.GetAlphaMask(),
        _ => null,
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="linkSize"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static Visual GetElementVisual(this UIElement element)
     => ElementCompositionPreview.GetElementVisual(element);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elements"></param>
    /// <returns></returns>
    public static IEnumerable<Visual> GetVisuals<T>(this IEnumerable<T> elements) where T : UIElement
    {
        foreach (var item in elements)
        {
            yield return GetElementVisual(item);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="scrollViewer"></param>
    /// <returns></returns>
    public static CompositionPropertySet GetScrollManipulationPropertySet(this ScrollViewer scrollViewer)
    {
        return ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static bool IsTranslationEnabled(this UIElement element)
    {
        return GetElementVisual(element).Properties.TryGetVector3(CompositionFactory.TRANSLATION, out _) != CompositionGetValueStatus.NotFound;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="element"></param>
    /// <param name="visual"></param>
    /// <returns></returns>
    public static T SetChildVisual<T>(this T element, Visual? visual) where T : UIElement
    {
        ElementCompositionPreview.SetElementChildVisual(element, visual);
        return element;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="animation"></param>
    public static void SetHideAnimation(this UIElement element, ICompositionAnimationBase? animation)
    {
        ElementCompositionPreview.SetImplicitHideAnimation(element, animation);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="animation"></param>
    public static void SetShowAnimation(this UIElement element, ICompositionAnimationBase? animation)
    {
        ElementCompositionPreview.SetImplicitShowAnimation(element, animation);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static UIElement SetTranslation(this UIElement element, Vector3 value)
    {
        element.GetElementVisual().Properties.InsertVector3("Translation", value);
        return element;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static bool SupportsAlphaMask(this UIElement element) => element switch
    {
        TextBlock or Shape or Image => true,
        _ => false,
    };
}
