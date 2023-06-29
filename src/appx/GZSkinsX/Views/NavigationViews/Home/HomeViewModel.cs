// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using System;
using System.Buffers;
using System.Threading.Tasks;

using CommunityToolkit.HighPerformance.Buffers;
using CommunityToolkit.Mvvm.ComponentModel;

using Windows.Storage.Streams;
using Windows.Web.Http;

namespace GZSkinsX.Views.NavigationViews.Home;

internal sealed partial class HomeViewModel : ObservableObject
{
    private const string Lol_news_link = "https://apps.game.qq.com/cmc/zmMcnTargetContentList?r0=jsonp&page=1&num=16&target=24&source=web_pc";

    private readonly HttpClient _httpClient;

    ~HomeViewModel()
    {
        _httpClient.Dispose();
    }

    public HomeViewModel()
    {
        _httpClient = new();
    }

    public async Task InitializeAsync()
    {
        var buffer = await _httpClient.GetBufferAsync(new Uri(Lol_news_link));
        using var reader = DataReader.FromBuffer(buffer);

        var bytes = ArrayPool<byte>.Shared.Rent((int)buffer.Length);
        try
        {
            reader.ReadBytes(bytes);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes);
        }
    }
}
