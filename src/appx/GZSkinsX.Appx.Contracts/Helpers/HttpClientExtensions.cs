// Copyright 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "LICENSE.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GZSkinsX.Contracts.Helpers;

public static class HttpClientExtensions
{
    public static async Task DownloadAsync(this HttpClient client, Uri requestUri, Stream destination,
        IProgress<double>? progress = null, CancellationToken cancellationToken = default)
    {
        // Get the http headers first to examine the content length
        using var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        var contentLength = response.Content.Headers.ContentLength;

        using var download = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        // Ignore progress reporting when no progress reporter was passed or when the content length is unknown.
        if (progress is null || contentLength.HasValue is false)
        {
            await download.CopyToAsync(destination, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            // Convert absolute progress (bytes downloaded) into relative progress.
            var relativeProgress = new Progress<long>(totalBytes => progress.Report((double)totalBytes / contentLength.Value));

            // Use extension method to report progress while downloading.
            await download.CopyToAsync(destination, 16_384, relativeProgress, cancellationToken).ConfigureAwait(false);
        }

        // Flush the destination stream.
        await destination.FlushAsync(cancellationToken);
    }
}
