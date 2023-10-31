// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.IO.Hashing;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using GZSkinsX.Contracts.Appx;
using GZSkinsX.Contracts.Helpers;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.DevTools.Generators.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class ChecksumGeneratorPage : Page
{
    private readonly Lazy<XxHash3> _lazyXxHash3 = new(() => new());

    private readonly Lazy<XxHash64> _lazyXxHash64 = new(() => new());

    private HashComparisonResult _comparisonResult;

    private string? _inputFile;

    public ChecksumGeneratorPage()
    {
        InitializeComponent();

        HashAlgorithmSelector.SelectedIndex = 0;
        HashAlgorithmSelector.SelectionChanged += async (s, e) => await CheckSumAsync();

        OutputTextBox.TextChanged += (s, e) => UpdateComparisonResult();
        OutputComparisonTextBox.TextChanged += (s, e) => UpdateComparisonResult();
    }

    private void Grid_DragOver(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        DragEnterStateTrigger.IsActive = true;
    }

    private void Grid_DragLeave(object sender, DragEventArgs e)
    {
        DragEnterStateTrigger.IsActive = false;
    }

    private async void Grid_Drop(object sender, DragEventArgs e)
    {
        DragEnterStateTrigger.IsActive = false;

        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            var files = items.OfType<StorageFile>();
            if (files.Count() is 1)
            {
                _inputFile = files.Single().Path;
                await CheckSumAsync();
            }
        }
    }

    private async void OnBrowseFile(object sender, RoutedEventArgs e)
    {
        var filePicker = new FileOpenPicker();
        filePicker.FileTypeFilter.Add("*");

        WinRT.Interop.InitializeWithWindow.Initialize(
            filePicker, AppxContext.AppxWindow.MainWindowHandle);

        var file = await filePicker.PickSingleFileAsync();
        if (file is not null)
        {
            _inputFile = file.Path;
            await CheckSumAsync();
        }
    }

    private async void OnBrowseFileToComparison(object sender, RoutedEventArgs e)
    {
        var filePicker = new FileOpenPicker();
        filePicker.FileTypeFilter.Add("*");

        WinRT.Interop.InitializeWithWindow.Initialize(
            filePicker, AppxContext.AppxWindow.MainWindowHandle);

        var file = await filePicker.PickSingleFileAsync();
        if (file is not null)
        {
            OutputComparisonTextBox.Text = await CheckSumAsync(
                HashAlgorithmSelector.SelectedIndex, file.Path);
        }
    }

    private void OnCopyChecksum(object sender, RoutedEventArgs e)
    {
        var text = OutputTextBox.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        var dataPackage = new DataPackage
        {
            RequestedOperation = DataPackageOperation.Copy
        };

        dataPackage.SetText(text);
        Clipboard.SetContent(dataPackage);
    }

    private async Task CheckSumAsync()
    {
        var inputFile = _inputFile;
        if (File.Exists(inputFile) is false)
        {
            OutputTextBox.ClearValue(TextBox.TextProperty);
            return;
        }

        var selectedHashAlgorithm = HashAlgorithmSelector.SelectedIndex;
        if (selectedHashAlgorithm is not (>= 0 and <= 5))
        {
            OutputTextBox.ClearValue(TextBox.TextProperty);
            return;
        }

        var fileSize = new FileInfo(inputFile).Length;
        if (fileSize / (1024 * 1024) > 21)
        {
            InProgressStateTrigger.IsActive = true;
        }

        OutputTextBox.Text = await CheckSumAsync(selectedHashAlgorithm, inputFile);
        InProgressStateTrigger.IsActive = false;
    }

    private async Task<string?> CheckSumAsync(int hashAlgorithmKey, string inputFile) => hashAlgorithmKey switch
    {
        0 => await CheckSumAsync(MD5.Create(), inputFile),
        1 => await CheckSumAsync(SHA1.Create(), inputFile),
        2 => await CheckSumAsync(SHA256.Create(), inputFile),
        3 => await CheckSumAsync(SHA384.Create(), inputFile),
        4 => await CheckSumAsync(SHA512.Create(), inputFile),
        5 => await CheckSumAsync(_lazyXxHash3.Value, inputFile),
        6 => await CheckSumAsync(_lazyXxHash64.Value, inputFile),
        _ => null,
    };

    private static async Task<string?> CheckSumAsync(HashAlgorithm hashAlgorithm, string inputFile)
    {
        FileStream? inputStream = null;
        try
        {
            inputStream = new FileStream(inputFile, FileMode.Open,
                FileAccess.Read, FileShare.ReadWrite, 4096, true);

            var fileHash = await hashAlgorithm.ComputeHashAsync(inputStream);
            return BitConverter.ToString(fileHash).Replace("-", string.Empty).ToLowerInvariant();
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception excp)
        {
            await excp.ShowErrorDialogAsync();
            return excp.Message;
        }
        finally
        {
            inputStream?.Dispose();
            hashAlgorithm.Dispose();
        }
    }

    private static async Task<string?> CheckSumAsync(NonCryptographicHashAlgorithm hashAlgorithm, string inputFile)
    {
        FileStream? inputStream = null;
        try
        {
            inputStream = new FileStream(inputFile, FileMode.Open,
                FileAccess.Read, FileShare.ReadWrite, 4096, true);

            await hashAlgorithm.AppendAsync(inputStream);
            var fileHash = hashAlgorithm.GetHashAndReset();
            return BitConverter.ToString(fileHash).Replace("-", string.Empty).ToLowerInvariant();

        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception excp)
        {
            await excp.ShowErrorDialogAsync();
            return excp.Message;
        }
        finally
        {
            inputStream?.Dispose();
        }
    }

    private void UpdateComparisonResult()
    {
        HashComparisonResult EvaluateOutputComparisonResult()
        {
            var left = OutputTextBox.Text;
            var right = OutputComparisonTextBox.Text;

            if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
            {
                return HashComparisonResult.None;
            }
            else
            {
                return StringComparer.OrdinalIgnoreCase.Equals(left, right) ?
                    HashComparisonResult.Success : HashComparisonResult.Error;
            }
        }

        var newResult = EvaluateOutputComparisonResult();
        var previousResult = _comparisonResult;
        if (previousResult == newResult)
        {
            return;
        }

        _comparisonResult = newResult;
        if (newResult is HashComparisonResult.Error)
        {
            ComparisonResultInfoBar.Title = ResourceHelper.GetLocalized("GZSkinsX.DevTools.Generators.x/Resources/ChecksumGenerator_HashComparison_HashesMismatch");
            ComparisonResultInfoBar.Severity = InfoBarSeverity.Error;
            ComparisonResultInfoBar.IsOpen = true;
        }
        else if (newResult is HashComparisonResult.Success)
        {
            ComparisonResultInfoBar.Title = ResourceHelper.GetLocalized("GZSkinsX.DevTools.Generators.x/Resources/ChecksumGenerator_HashComparison_HashesMatch");
            ComparisonResultInfoBar.Severity = InfoBarSeverity.Success;
            ComparisonResultInfoBar.IsOpen = true;
        }
        else
        {
            ComparisonResultInfoBar.ClearValue(InfoBar.TitleProperty);
            ComparisonResultInfoBar.ClearValue(InfoBar.SeverityProperty);
            ComparisonResultInfoBar.ClearValue(InfoBar.IsOpenProperty);
        }
    }

    private enum HashComparisonResult
    {
        None,
        Error,
        Success
    }
}
