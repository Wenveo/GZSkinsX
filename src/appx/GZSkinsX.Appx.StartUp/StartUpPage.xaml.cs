// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Numerics;

using GZSkinsX.Api.Game;
using GZSkinsX.Api.Shell;

using Windows.ApplicationModel.Resources.Core;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GZSkinsX.Appx.StartUp;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class StartUpPage : Page
{
    /// <summary>
    /// 
    /// </summary>
    private IViewManagerService? _viewManagerService;

    /// <summary>
    /// 
    /// </summary>
    private IGameService? _gameService;

    /// <summary>
    /// 
    /// </summary>
    public StartUpPage()
    {
        InitializeComponent();
    }

    internal void InitializeContext(IViewManagerService viewManagerService,
                                    IGameService gameService,
                                    bool isInvalid)
    {
        _viewManagerService = viewManagerService;
        _gameService = gameService;

        var val = ResourceManager.Current.MainResourceMap.GetValue(isInvalid
            ? "GZSkinsX.Appx.StartUp/Resources/Appx_StartUp_Initialize_Invalid_Title"
            : "GZSkinsX.Appx.StartUp/Resources/Appx_StartUp_Initialize_Default_Title");

        Appx_StartUp_Initialize_Title.Text = val.ValueAsString;
    }
}

/// <summary>
/// 
/// </summary>
internal sealed class HoverXamlLight : XamlLight
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly string Id = typeof(HoverXamlLight).FullName;

    /// <summary>
    /// 
    /// </summary>
    private ExpressionAnimation? _lightPositionExpression;

    /// <summary>
    /// 
    /// </summary>
    private ScalarKeyFrameAnimation? _pointerEnteredAnimation;

    /// <summary>
    /// 
    /// </summary>
    private ScalarKeyFrameAnimation? _pointerExitedAnimation;

    /// <inheritdoc/>
    protected override void OnConnected(UIElement newElement)
    {
        var compositor = Window.Current.Compositor;

        // Create SpotLight and set its properties
        var pointLight = compositor.CreatePointLight();
        pointLight.Color = Colors.FloralWhite;
        pointLight.Intensity = 0f;

        // Associate CompositionLight with XamlLight
        CompositionLight = pointLight;

        _pointerEnteredAnimation = compositor.CreateScalarKeyFrameAnimation();
        _pointerEnteredAnimation.InsertKeyFrame(1, 5f);
        _pointerEnteredAnimation.Duration = TimeSpan.FromMilliseconds(800d);

        _pointerExitedAnimation = compositor.CreateScalarKeyFrameAnimation();
        _pointerExitedAnimation.InsertKeyFrame(1, 0);
        _pointerExitedAnimation.Duration = TimeSpan.FromMilliseconds(600d);

        // Define expression animation that relates light's offset to pointer position 
        var hoverPosition = ElementCompositionPreview.GetPointerPositionPropertySet(newElement);
        _lightPositionExpression = compositor.CreateExpressionAnimation("Vector3(hover.Position.X, hover.Position.Y, height)");
        _lightPositionExpression.SetReferenceParameter("hover", hoverPosition);
        _lightPositionExpression.SetScalarParameter("height", 32f);

        newElement.PointerEntered += OnPointerEntered;
        newElement.PointerExited += OnPointerExited;
        newElement.PointerMoved += OnPointerMoved;

        AddTargetElement(GetId(), newElement);
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        CompositionLight?.StartAnimation("Intensity", _pointerEnteredAnimation);
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        CompositionLight?.StartAnimation("Intensity", _pointerExitedAnimation);
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (CompositionLight is PointLight pointLight)
        {
            // touch input is still UI thread-bound as of the Creator's Update
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Touch)
            {
                var offset = e.GetCurrentPoint((UIElement)sender).Position.ToVector2();
                pointLight.Offset = new Vector3(offset.X, offset.Y, 32.0f);
            }
            else
            {
                // Get the pointer's current position from the property and bind the SpotLight's X-Y Offset
                pointLight.StartAnimation("Offset", _lightPositionExpression);
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnDisconnected(UIElement oldElement)
    {
        oldElement.PointerEntered -= OnPointerEntered;
        oldElement.PointerExited -= OnPointerExited;
        oldElement.PointerMoved -= OnPointerMoved;

        RemoveTargetElement(GetId(), oldElement);
        CompositionLight?.Dispose();

        _lightPositionExpression?.Dispose();
        _pointerEnteredAnimation?.Dispose();
        _pointerExitedAnimation?.Dispose();
    }

    /// <inheritdoc/>
    protected override string GetId()
    {
        return Id;
    }
}

/// <summary>
/// 
/// </summary>
internal sealed class AmbientXamlLight : XamlLight
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly string Id = typeof(AmbientXamlLight).FullName;

    /// <inheritdoc/>
    protected override void OnConnected(UIElement newElement)
    {
        var compositor = Window.Current.Compositor;

        // Create AmbientLight and set its properties
        var ambientLight = compositor.CreateAmbientLight();
        ambientLight.Color = Colors.White;

        // Associate CompositionLight with XamlLight
        CompositionLight = ambientLight;

        // Add UIElement to the Light's Targets
        AddTargetElement(GetId(), newElement);
    }

    /// <inheritdoc/>
    protected override void OnDisconnected(UIElement oldElement)
    {
        // Dispose Light when it is removed from the tree
        RemoveTargetElement(GetId(), oldElement);
        CompositionLight?.Dispose();
    }

    /// <inheritdoc/>
    protected override string GetId()
    {
        return Id;
    }
}
