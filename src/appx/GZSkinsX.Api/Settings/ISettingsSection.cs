// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

namespace GZSkinsX.Api.Settings;

public interface ISettingsSection
{
    string Name { get; }

    object Attribute(string key);

    TValue? Attribute<TValue>(string key) where TValue : class;

    void Attribute(string key, object value);

    bool Delete(string key);

    void DeleteSection(string name);

    ISettingsSection GetOrCreateSection(string name);
}
