// Copyright 2022 - 2023 GZSkins, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Composition;

using GZSkinsX.Contracts.Extension;

using Microsoft.UI.Xaml;

namespace GZSkinsX.Extension;

[Shared]
[Export(typeof(IExtensionService))]
internal sealed class ExtensionServiceImpl : IExtensionService
{
    private readonly Lazy<IAutoLoaded, AutoLoadedMetadataAttribute>[] _mefAutoLoaded;
    private readonly Lazy<IExtension, ExtensionMetadataAttribute>[] _extensions;

    public IEnumerable<IExtension> Extensions => _extensions.Select(a => a.Value);

    [ImportingConstructor]
    public ExtensionServiceImpl(
        [ImportMany] IEnumerable<Lazy<IAutoLoaded, AutoLoadedMetadataAttribute>> mefAutoLoaded,
        [ImportMany] IEnumerable<Lazy<IExtension, ExtensionMetadataAttribute>> extensions)
    {
        _mefAutoLoaded = mefAutoLoaded.OrderBy(a => a.Metadata.Order).ToArray();
        _extensions = extensions.OrderBy(a => a.Metadata.Order).ToArray();
    }

    public IEnumerable<ResourceDictionary> GetMergedResourceDictionaries()
    {
        foreach (var extension in _extensions)
        {
            var value = extension.Value;
            foreach (var rsrc in value.MergedResourceDictionaries)
            {
                var asm = value.GetType().Assembly.GetName();
                var uri = new Uri($"ms-appx://{asm.Name}/{rsrc}", UriKind.Absolute);
                yield return new ResourceDictionary { Source = uri };
            }
        }
    }

    public void LoadAutoLoaded(AutoLoadedType loadType)
    {
        foreach (var extension in _mefAutoLoaded.Where(a => a.Metadata.LoadType == loadType))
        {
            var value = extension.Value;
        }
    }

    public void NotifyExtensions(ExtensionEvent eventType)
    {
        foreach (var extension in _extensions)
        {
            extension.Value.OnEvent(eventType);
        }
    }

    public override int GetHashCode()
    => HashCode.Combine(_mefAutoLoaded, _extensions);
}
