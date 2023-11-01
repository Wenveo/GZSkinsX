// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Buffers;
using System.Diagnostics;
using System.IO.Hashing;
using System.Text;
using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml.Controls;

using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GZSkinsX.DevTools.Generators.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
internal sealed partial class HashGeneratorPage : Page
{
    public HashGeneratorPage()
    {
        InitializeComponent();

        MatchCaseToggleSwitch.Toggled += (s, e) => GenerateHashes();
        OutputTypeSelector.SelectionChanged += (s, e) => GenerateHashes();
        InputTextBox.TextChanged += (s, e) => GenerateHashes();

        ClearInputTextButton.Click += (s, e) => InputTextBox.Text = string.Empty;
        PasteToInputTextBoxButton.Click += async (s, e) =>
        {
            var package = Clipboard.GetContent();
            if (package.Contains(StandardDataFormats.Text))
            {
                var text = await package.GetTextAsync();
                if (string.IsNullOrWhiteSpace(text) is false)
                {
                    InputTextBox.Text = text;
                }
            }
        };
    }

    private unsafe void GenerateHashes()
    {
        var inputText = InputTextBox.Text;
        if (string.IsNullOrWhiteSpace(inputText))
        {
            ClearResults();
            return;
        }

        var selectedOutputType = OutputTypeSelector.SelectedIndex;
        if (selectedOutputType is not 0 and not 1)
        {
            ClearResults();
            return;
        }

        if (MatchCaseToggleSwitch.IsOn)
        {
            fixed (char* ch = inputText)
            {
                GenerateHashesCore(ch, inputText.Length, Encoding.UTF8, Convert.ToBoolean(selectedOutputType));
            }
        }
        else
        {
            var length = inputText.Length;
            if (length < 256)
            {
                var ch = stackalloc char[length];
                length = inputText.AsSpan().ToLowerInvariant(new(ch, length));
                GenerateHashesCore(ch, length, Encoding.UTF8, Convert.ToBoolean(selectedOutputType));
            }
            else
            {
                var tempBuffer = ArrayPool<char>.Shared.Rent(length);
                fixed (char* ch = tempBuffer)
                {
                    length = inputText.AsSpan().ToLowerInvariant(new(ch, length));
                    GenerateHashesCore(ch, length, Encoding.UTF8, Convert.ToBoolean(selectedOutputType));
                }

                ArrayPool<char>.Shared.Return(tempBuffer);
            }
        }

        void GenerateHashesCore(char* ch, int length, Encoding encoding, bool isOutputHexNumbers)
        {
            var byteCount = encoding.GetByteCount(ch, length);
            if (byteCount < 512)
            {
                var bytes = stackalloc byte[byteCount];
                var bytesReceived = encoding.GetBytes(ch, length, bytes, byteCount);
                Debug.Assert(byteCount <= bytesReceived);

                GenerateResults(new(bytes, bytesReceived), isOutputHexNumbers);
            }
            else
            {
                var tempBuffer = ArrayPool<byte>.Shared.Rent(byteCount);
                fixed (byte* bytes = tempBuffer)
                {
                    var bytesReceived = encoding.GetBytes(ch, length, bytes, byteCount);
                    Debug.Assert(byteCount <= bytesReceived);

                    GenerateResults(new(bytes, bytesReceived), isOutputHexNumbers);
                }

                ArrayPool<byte>.Shared.Return(tempBuffer);
            }
        }

        void GenerateResults(ReadOnlySpan<byte> data, bool isOutputHexNumbers)
        {
            static uint CalcFNV1aHash(scoped ReadOnlySpan<byte> data)
            {
                var hashCode = 2166136261U;
                for (var i = 0; i < data.Length; i++)
                {
                    hashCode = (hashCode ^ data[i]) * 16777619;
                }

                return hashCode;
            }

            // Results
            var fnv1aHash = CalcFNV1aHash(data);
            var crc32Hash = Crc32.HashToUInt32(data);
            var xxh32Hash = XxHash32.HashToUInt32(data);
            var xxh64Hash = XxHash64.HashToUInt64(data);

            if (isOutputHexNumbers)
            {
                Crc32OutputTextBox.Text = crc32Hash.ToString("x");
                FNV1aOutputTextBox.Text = fnv1aHash.ToString("x");
                XxHash32OutputTextBox.Text = xxh32Hash.ToString("x");
                XxHash64OutputTextBox.Text = xxh64Hash.ToString("x");
            }
            else
            {
                Crc32OutputTextBox.Text = crc32Hash.ToString();
                FNV1aOutputTextBox.Text = fnv1aHash.ToString();
                XxHash32OutputTextBox.Text = xxh32Hash.ToString();
                XxHash64OutputTextBox.Text = xxh64Hash.ToString();
            }
        }
    }


    private void ClearResults()
    {
        Crc32OutputTextBox.ClearValue(TextBox.TextProperty);
        FNV1aOutputTextBox.ClearValue(TextBox.TextProperty);
        XxHash32OutputTextBox.ClearValue(TextBox.TextProperty);
        XxHash64OutputTextBox.ClearValue(TextBox.TextProperty);
    }

    public ICommand CopyResultCommand(int type)
    {
        void OnCopyResult()
        {
            var text = type switch
            {
                1 => Crc32OutputTextBox.Text,
                2 => FNV1aOutputTextBox.Text,
                3 => XxHash32OutputTextBox.Text,
                4 => XxHash64OutputTextBox.Text,
                _ => null
            };

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

        return new RelayCommand(OnCopyResult);
    }
}
