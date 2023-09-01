// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Linq;
using System.Windows.Input;

using GZSkinsX.Contracts.Appx;

using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace GZSkinsX.Contracts.Controls;

[TemplatePart(Name = PART_PrimaryButton, Type = typeof(Button))]
[TemplatePart(Name = PART_SecondaryButton, Type = typeof(Button))]
public class MultiStateToggleButton : ItemsControl
{
#pragma warning disable format
    internal const string NormalState           = "Normal";
    internal const string CheckedState          = "Checked";
    internal const string DisabledState         = "Disabled";
    internal const string FlyoutOpenState       = "FlyoutOpen";
    internal const string TouchPressedState     = "TouchPressed";

    internal const string CheckedFlyoutOpenState            = "CheckedFlyoutOpen"; 
    internal const string CheckedTouchPressedState          = "CheckedTouchPressed";
    internal const string CheckedPrimaryPressedState        = "CheckedPrimaryPressed";
    internal const string CheckedPrimaryPointerOverState    = "CheckedPrimaryPointerOver";
    internal const string CheckedSecondaryPressedState      = "CheckedSecondaryPressed";
    internal const string CheckedSecondaryPointerOverState  = "CheckedSecondaryPointerOver";
    internal const string PrimaryPressedState               = "PrimaryPressed";
    internal const string PrimaryPointerOverState           = "PrimaryPointerOver";
    internal const string SecondaryButtonSpanState          = "SecondaryButtonSpan";
    internal const string SecondaryButtonRightState         = "SecondaryButtonRight";
    internal const string SecondaryPressedState             = "SecondaryPressed";
    internal const string SecondaryPointerOverState         = "SecondaryPointerOver";

    internal const string PART_PrimaryButton            = "PrimaryButton";
    internal const string PART_SecondaryButton          = "SecondaryButton";
#pragma warning restore format

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(ICommand),
            typeof(MultiStateToggleButton), new PropertyMetadata(null));

    public static readonly DependencyProperty CommandParameterProperty =
        DependencyProperty.Register(nameof(CommandParameter), typeof(object),
            typeof(MultiStateToggleButton), new PropertyMetadata(null));

    public static readonly DependencyProperty FlyoutProperty =
        DependencyProperty.Register(nameof(Flyout), typeof(FlyoutBase),
            typeof(MultiStateToggleButton), new PropertyMetadata(null, OnFlyoutChangedCallback));

    public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register(nameof(State), typeof(string),
            typeof(MultiStateToggleButton), new PropertyMetadata(string.Empty, OnStateChangedCallback));

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public FlyoutBase Flyout
    {
        get => (FlyoutBase)GetValue(FlyoutProperty);
        set => SetValue(FlyoutProperty, value);
    }

    public string State
    {
        get => (string)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    internal bool IsFlyoutOpen
    {
        get => _isFlyoutOpen;
    }

    public event TypedEventHandler<MultiStateToggleButton, EventArgs>? Click;

    public MultiStateToggleButton()
    {
        DefaultStyleKey = typeof(MultiStateToggleButton);

        KeyUp += OnMultiStateToggleButtonKeyUp;
        KeyDown += OnMultiStateToggleButtonKeyDown;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new MultiStateToggleButtonAutomationPeer(this);
    }

#pragma warning disable format
    private Button?             _primaryButton;
    private Button?             _secondaryButton;
    private PointerDeviceType   _lastPointerDeviceType;
    private long?               _flyoutPlacementCallbackToken;
    private long?               _primaryButtonIsPressedCallbackToken;
    private long?               _primaryButtonIsPointerOverCallbackToken;
    private long?               _secondaryButtonIsPressedCallbackToken;
    private long?               _secondaryButtonIsPointerOverCallbackToken;
    private bool                _isKeyDown;
    private bool                _isFlyoutOpen;
    private bool                _hasLoaded;

    protected override void OnApplyTemplate()
    {
        UnregisterEvents();
        base.OnApplyTemplate();

        _primaryButton   = GetTemplateChild(PART_PrimaryButton)   as Button;
        _secondaryButton = GetTemplateChild(PART_SecondaryButton) as Button;

        if (_primaryButton is not null)
        {
            _primaryButton.Click += OnClickPrimary;

            _primaryButtonIsPressedCallbackToken =
                _primaryButton.RegisterPropertyChangedCallback(ButtonBase.IsPressedProperty, OnVisualPropertyChanged);

            _primaryButtonIsPointerOverCallbackToken =
                _primaryButton.RegisterPropertyChangedCallback(ButtonBase.IsPointerOverProperty, OnVisualPropertyChanged);

            // Register for pointer events so we can keep track of the last used pointer type
            _primaryButton.PointerEntered       += OnPointerEvent;
            _primaryButton.PointerExited        += OnPointerEvent;
            _primaryButton.PointerPressed       += OnPointerEvent;
            _primaryButton.PointerReleased      += OnPointerEvent;
            _primaryButton.PointerCanceled      += OnPointerEvent;
            _primaryButton.PointerCaptureLost   += OnPointerEvent;
        }

        if (_secondaryButton is not null)
        {
            // Do localization for the secondary button
            var secondaryName = AppxContext.MRTCoreService
                .AllResourceMaps["Microsoft.UI.Xaml.2.7"]
                .GetString("Microsoft.UI.Xaml/Resources/SplitButtonSecondaryButtonName");

            AutomationProperties.SetName(_secondaryButton, secondaryName);

            _secondaryButton.Click += OnClickSecondary;

            _secondaryButtonIsPressedCallbackToken =
                _secondaryButton.RegisterPropertyChangedCallback(ButtonBase.IsPressedProperty, OnVisualPropertyChanged);

            _secondaryButtonIsPointerOverCallbackToken =
                _secondaryButton.RegisterPropertyChangedCallback(ButtonBase.IsPointerOverProperty, OnVisualPropertyChanged);

            // Register for pointer events so we can keep track of the last used pointer type
            _secondaryButton.PointerEntered     += OnPointerEvent;
            _secondaryButton.PointerExited      += OnPointerEvent;
            _secondaryButton.PointerPressed     += OnPointerEvent;
            _secondaryButton.PointerReleased    += OnPointerEvent;
            _secondaryButton.PointerCanceled    += OnPointerEvent;
            _secondaryButton.PointerCaptureLost += OnPointerEvent;
        }

        _hasLoaded = true;
        Items.VectorChanged += OnItemsChanged;

        UpdateContentAndVisualStates();
    }
#pragma warning restore format

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new MultiStateToggleButtonState();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is MultiStateToggleButtonState;
    }

    private void OnClickPrimary(object sender, RoutedEventArgs e)
    {
        Click?.Invoke(this, EventArgs.Empty);

        if (FrameworkElementAutomationPeer.FromElement(this) is { } peer)
        {
            peer.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
        }
    }

    private void OnClickSecondary(object sender, RoutedEventArgs e)
    {
        OpenFlyout();
    }

    private void OnItemsChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
    {
        UpdateContentAndVisualStates();
    }

    private void OnPointerEvent(object sender, PointerRoutedEventArgs e)
    {
        var pointerDeviceType = e.Pointer.PointerDeviceType;
        if (pointerDeviceType != _lastPointerDeviceType)
        {
            _lastPointerDeviceType = pointerDeviceType;
            UpdateVisualStates();
        }
    }

    private void OnVisualPropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        UpdateVisualStates();
    }

    private void OnMultiStateToggleButtonKeyDown(object sender, KeyRoutedEventArgs e)
    {
        var key = e.Key;
        if (key is VirtualKey.Space or VirtualKey.Enter or VirtualKey.GamepadA)
        {
            _isKeyDown = true;
            UpdateVisualStates();
        }
    }

    private void OnMultiStateToggleButtonKeyUp(object sender, KeyRoutedEventArgs e)
    {
        var key = e.Key;
        if (key is VirtualKey.Space or VirtualKey.Enter or VirtualKey.GamepadA)
        {
            _isKeyDown = false;
            UpdateVisualStates();

            if (IsEnabled)
            {
                OnClickPrimary(null!, null!);
                ExecuteCommand();
                e.Handled = true;
            }
        }
        else if (key is VirtualKey.Down)
        {
            var menuState = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Menu);
            var menuKeyDown = (menuState & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;

            if (IsEnabled && menuKeyDown)
            {
                // Open the menu on alt-down
                OpenFlyout();
                e.Handled = true;
            }
        }
        else if (key is VirtualKey.F4 && IsEnabled)
        {
            // Open the menu on F4
            OpenFlyout();
            e.Handled = true;
        }
    }

    private static void OnFlyoutChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as MultiStateToggleButton)?.OnFlyoutChanged(
            e.OldValue as FlyoutBase, e.NewValue as FlyoutBase);
    }

    private void OnFlyoutChanged(FlyoutBase? oldValue, FlyoutBase? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.Opened -= OnFlyoutOpened;
            oldValue.Closed -= OnFlyoutClosed;

            if (_flyoutPlacementCallbackToken.HasValue)
            {
                oldValue.UnregisterPropertyChangedCallback(FlyoutBase.PlacementProperty, _flyoutPlacementCallbackToken.Value);
                _flyoutPlacementCallbackToken = null;
            }
        }

        if (newValue is not null)
        {
            newValue.Opened += OnFlyoutOpened;
            newValue.Closed += OnFlyoutClosed;

            _flyoutPlacementCallbackToken = newValue.RegisterPropertyChangedCallback(FlyoutBase.PlacementProperty, OnFlyoutPlacementChanged);
        }

        UpdateVisualStates();
    }

    private void OnFlyoutOpened(object sender, object e)
    {
        _isFlyoutOpen = true;
        UpdateVisualStates();

        if (FrameworkElementAutomationPeer.FromElement(this) is { } peer)
        {
            peer.RaisePropertyChangedEvent(
                ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                ExpandCollapseState.Collapsed, ExpandCollapseState.Expanded);
        }
    }

    private void OnFlyoutClosed(object sender, object e)
    {
        _isFlyoutOpen = false;
        UpdateVisualStates();

        if (FrameworkElementAutomationPeer.FromElement(this) is { } peer)
        {
            peer.RaisePropertyChangedEvent(
                ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                ExpandCollapseState.Expanded, ExpandCollapseState.Collapsed);
        }
    }

    private void OnFlyoutPlacementChanged(DependencyObject sender, DependencyProperty dp)
    {
        UpdateVisualStates();
    }

    private static void OnStateChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as MultiStateToggleButton)?.OnStateChanged(
            e.OldValue as string, e.NewValue as string);
    }

    private void OnStateChanged(string? oldValue, string? newValue)
    {
        var oldState = GetState(oldValue);
        if (oldState is not null)
        {
            oldState.Parent = null;
        }

        UpdateContentAndVisualStates();
    }

    private void UpdateContent()
    {
        if (_hasLoaded is false)
        {
            return;
        }

        if (_primaryButton is not null)
        {
            var currentState = GetState(State);
            if (currentState is not null)
            {
                currentState.Parent ??= this;
                _primaryButton.Content = currentState.Content;
                AutomationProperties.SetName(this, currentState.StateName);
            }
            else
            {
                _primaryButton.Content = null;
                AutomationProperties.SetName(this, null);
            }
        }
    }

    private void UpdateVisualStates(bool useTransitions = true)
    {
        if (_lastPointerDeviceType is PointerDeviceType.Touch || _isKeyDown)
        {
            VisualStateManager.GoToState(this, SecondaryButtonSpanState, useTransitions);
        }
        else
        {
            VisualStateManager.GoToState(this, SecondaryButtonRightState, useTransitions);
        }

        var primaryButton = _primaryButton;
        var secondaryButton = _secondaryButton;

        if (IsEnabled is false)
        {
            VisualStateManager.GoToState(this, DisabledState, useTransitions);
        }
        else if (primaryButton is not null && secondaryButton is not null)
        {
            if (_isFlyoutOpen)
            {
                if (InternalIsChecked())
                {
                    VisualStateManager.GoToState(this, CheckedFlyoutOpenState, useTransitions);
                }
                else
                {
                    VisualStateManager.GoToState(this, FlyoutOpenState, useTransitions);
                }
            }
            else if (InternalIsChecked())
            {
                if (_lastPointerDeviceType is PointerDeviceType.Touch || _isKeyDown)
                {
                    if (primaryButton.IsPressed || secondaryButton.IsPressed || _isKeyDown)
                    {
                        VisualStateManager.GoToState(this, CheckedTouchPressedState, useTransitions);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, CheckedState, useTransitions);
                    }
                }
                else if (primaryButton.IsPressed)
                {
                    VisualStateManager.GoToState(this, CheckedPrimaryPressedState, useTransitions);
                }
                else if (primaryButton.IsPointerOver)
                {
                    VisualStateManager.GoToState(this, CheckedPrimaryPointerOverState, useTransitions);
                }
                else if (secondaryButton.IsPointerOver)
                {
                    VisualStateManager.GoToState(this, CheckedSecondaryPressedState, useTransitions);
                }
                else if (secondaryButton.IsPointerOver)
                {
                    VisualStateManager.GoToState(this, CheckedSecondaryPointerOverState, useTransitions);
                }
                else
                {
                    VisualStateManager.GoToState(this, CheckedState, useTransitions);
                }
            }
            else
            {
                if (_lastPointerDeviceType is PointerDeviceType.Touch || _isKeyDown)
                {
                    if (primaryButton.IsPressed || secondaryButton.IsPressed || _isKeyDown)
                    {
                        VisualStateManager.GoToState(this, TouchPressedState, useTransitions);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, NormalState, useTransitions);
                    }
                }
                else if (primaryButton.IsPressed)
                {
                    VisualStateManager.GoToState(this, PrimaryPressedState, useTransitions);
                }
                else if (primaryButton.IsPointerOver)
                {
                    VisualStateManager.GoToState(this, PrimaryPointerOverState, useTransitions);
                }
                else if (secondaryButton.IsPointerOver)
                {
                    VisualStateManager.GoToState(this, SecondaryPressedState, useTransitions);
                }
                else if (secondaryButton.IsPointerOver)
                {
                    VisualStateManager.GoToState(this, SecondaryPointerOverState, useTransitions);
                }
                else
                {
                    VisualStateManager.GoToState(this, NormalState, useTransitions);
                }
            }
        }
    }

    internal void UpdateContentAndVisualStates(bool useTransitions = true)
    {
        UpdateContent();
        UpdateVisualStates(useTransitions);
    }

    internal MultiStateToggleButtonState? GetState(string? name)
    {
        if (name is null || name == string.Empty)
        {
            return null;
        }

        foreach (var item in Items.OfType<MultiStateToggleButtonState>())
        {
            if (StringComparer.Ordinal.Equals(item.StateName, name))
            {
                return item;
            }
        }

        return null;
    }

    internal bool InternalIsChecked()
    {
        var currentState = GetState(State);
        if (currentState is not null)
        {
            return currentState.IsChecked;
        }

        return false;
    }

    internal void OpenFlyout()
    {
        Flyout?.ShowAt(this, new());
    }

    internal void CloseFlyout()
    {
        Flyout?.Hide();
    }

    internal void ExecuteCommand()
    {
        var command = Command;
        if (command is not null)
        {
            var commandParameter = CommandParameter;
            if (command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }
    }

    internal void Invoke()
    {
        if (FrameworkElementAutomationPeer.FromElement(_primaryButton) is { } peer &&
            peer.GetPattern(PatternInterface.Invoke) is IInvokeProvider invokeProvider)
        {
            invokeProvider.Invoke();
            return;
        }

        // If we don't have a primary button that provides an invoke provider, we'll fall back to calling OnClickPrimary manually.
        OnClickPrimary(null!, null!);
    }

#pragma warning disable format
    private void UnregisterEvents()
    {
        var primaryButton = _primaryButton;
        if (primaryButton is not null)
        {
            primaryButton.Click -= OnClickPrimary;

            if (_primaryButtonIsPressedCallbackToken.HasValue)
            {
                primaryButton.UnregisterPropertyChangedCallback(
                    ButtonBase.IsPressedProperty, _primaryButtonIsPressedCallbackToken.Value);

                _primaryButtonIsPressedCallbackToken = null;
            }

            if (_primaryButtonIsPointerOverCallbackToken.HasValue)
            {
                primaryButton.UnregisterPropertyChangedCallback(
                    ButtonBase.IsPointerOverProperty, _primaryButtonIsPointerOverCallbackToken.Value);

                _primaryButtonIsPointerOverCallbackToken = null;
            }

            primaryButton.PointerEntered        -= OnPointerEvent;
            primaryButton.PointerExited         -= OnPointerEvent;
            primaryButton.PointerPressed        -= OnPointerEvent;
            primaryButton.PointerReleased       -= OnPointerEvent;
            primaryButton.PointerCanceled       -= OnPointerEvent;
            primaryButton.PointerCaptureLost    -= OnPointerEvent;
        }

        var secondaryButton = _secondaryButton;
        if (secondaryButton is not null)
        {
            secondaryButton.Click -= OnClickSecondary;

            if (_secondaryButtonIsPressedCallbackToken.HasValue)
            {
                secondaryButton.UnregisterPropertyChangedCallback(
                    ButtonBase.IsPressedProperty, _secondaryButtonIsPressedCallbackToken.Value);

                _secondaryButtonIsPressedCallbackToken = null;
            }

            if (_secondaryButtonIsPointerOverCallbackToken.HasValue)
            {
                secondaryButton.UnregisterPropertyChangedCallback(
                    ButtonBase.IsPointerOverProperty, _secondaryButtonIsPointerOverCallbackToken.Value);

                _secondaryButtonIsPointerOverCallbackToken = null;
            }

            secondaryButton.PointerEntered      -= OnPointerEvent;
            secondaryButton.PointerExited       -= OnPointerEvent;
            secondaryButton.PointerPressed      -= OnPointerEvent;
            secondaryButton.PointerReleased     -= OnPointerEvent;
            secondaryButton.PointerCanceled     -= OnPointerEvent;
            secondaryButton.PointerCaptureLost  -= OnPointerEvent;
        }

        Items.VectorChanged -= OnItemsChanged;
    }
#pragma warning restore format
}

