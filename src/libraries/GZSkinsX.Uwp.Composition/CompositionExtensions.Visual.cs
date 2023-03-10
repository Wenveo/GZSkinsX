// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Numerics;

using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace GZSkinsX.Uwp.Composition;

public static partial class CompositionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    private static string LINKED_SIZE_EXPRESSION { get; } = $"{nameof(Visual)}.{nameof(Visual.Size)}";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceVisual"></param>
    /// <returns></returns>
    public static ExpressionAnimation CreateLinkedSizeExpression(Visual sourceVisual)
    {
        return sourceVisual.CreateExpressionAnimation(nameof(Visual.Size))
                           .SetParameter(nameof(Visual), sourceVisual)
                           .SetExpression(LINKED_SIZE_EXPRESSION);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="targetVisual"></param>
    /// <param name="sourceVisual"></param>
    /// <returns></returns>
    public static T LinkSize<T>(this T targetVisual, Visual sourceVisual) where T : Visual
    {
        targetVisual.StartAnimation(CreateLinkedSizeExpression(sourceVisual));
        return targetVisual;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="targetVisual"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    public static T LinkSize<T>(this T targetVisual, FrameworkElement element) where T : Visual
    {
        targetVisual.StartAnimation(CreateLinkedSizeExpression(element.GetElementVisual()));
        return targetVisual;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="targetVisual"></param>
    /// <param name="sourceVisual"></param>
    /// <returns></returns>
    public static T LinkShapeSize<T>(this T targetVisual, Visual sourceVisual) where T : CompositionGeometry
    {
        targetVisual.StartAnimation(CreateLinkedSizeExpression(sourceVisual));
        return targetVisual;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visual"></param>
    /// <returns></returns>
    public static Vector3 GetTranslation(this Visual visual)
    {
        visual.Properties.TryGetVector3(CompositionFactory.TRANSLATION, out var translation);
        return translation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visual"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Visual SetCenterPoint(this Visual visual, float x, float y, float z)
    {
        return SetCenterPoint(visual, new Vector3(x, y, z));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visual"></param>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Visual SetCenterPoint(this Visual visual, Vector3 vector)
    {
        visual.CenterPoint = vector;
        return visual;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visual"></param>
    /// <returns></returns>
    public static Visual SetCenterPoint(this Visual visual)
    {
        return SetCenterPoint(visual, new Vector3(visual.Size / 2f, 0f));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visual"></param>
    /// <param name="axis"></param>
    /// <returns></returns>
    public static Visual SetRotationAxis(this Visual visual, Vector3 axis)
    {
        visual.RotationAxis = axis;
        return visual;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visual"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Visual SetRotationAxis(this Visual visual, float x, float y, float z)
    {
        visual.RotationAxis = new Vector3(x, y, z);
        return visual;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visual"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Visual SetTranslation(this Visual visual, float x, float y, float z)
    {
        return SetTranslation(visual, new Vector3(x, y, z));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visual"></param>
    /// <param name="translation"></param>
    /// <returns></returns>
    public static Visual SetTranslation(this Visual visual, Vector3 translation)
    {
        visual.Properties.InsertVector3(CompositionFactory.TRANSLATION, translation);
        return visual;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="visual"></param>
    /// <returns></returns>
    public static Visual WithStandardTranslation(this Visual visual)
    {
        return CompositionFactory.EnableStandardTranslation(visual);
    }
}
