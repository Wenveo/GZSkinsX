// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

#nullable enable

using GZSkinsX.Api.Appx;
using GZSkinsX.Api.CreatorStudio.Documents;

using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace GZSkinsX.Extensions.CreatorStudio.Commands;

public sealed partial class NewFileView : UserControl
{
    private readonly ContentDialog _contentDialog;
    private readonly CreateTemplateView _createTemplateView;
    private readonly SelectTemplateView _selectTemplateView;

    public NewFileView(ContentDialog contentDialog)
    {
        _contentDialog = contentDialog;
        _createTemplateView = new();
        _selectTemplateView = new();

        InitializeComponent();

        _createTemplateView.OnCancel = OnCancel;
        _createTemplateView.OnBack = OnBack;
        _createTemplateView.OnOkay = OnOkay;

        _selectTemplateView.OnCancel = OnCancel;
        _selectTemplateView.OnNext = OnNext;

        contentPresenter.Content = _selectTemplateView;
    }

    private void OnCancel()
    {
        _contentDialog.Hide();
    }

    private void OnBack()
    {
        contentPresenter.Content = _selectTemplateView;
    }

    private void OnNext()
    {
        contentPresenter.Content = _createTemplateView;
    }

    private void OnOkay()
    {
        _contentDialog.Hide();

        var documentService = AppxContext.Resolve<IDocumentService>();
        var textDocument = documentService.CreateDocument(DocumentInfo.CreateEmpty(
            _createTemplateView.TemplateName.Text + ".txt",
            "2E464B27-C4BC-4819-A8C4-DCF622ED4863"));

        documentService.GetOrAdd(textDocument);
    }
}
