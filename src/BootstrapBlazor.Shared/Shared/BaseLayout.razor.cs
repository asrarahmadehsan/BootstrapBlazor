﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace BootstrapBlazor.Shared.Shared;

/// <summary>
/// 母版页基类
/// </summary>
public partial class BaseLayout : IAsyncDisposable
{
    private ElementReference MsLearnElement { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<BaseLayout>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IJSRuntime? JSRuntime { get; set; }

    [Inject]
    [NotNull]
    private IOptionsMonitor<WebsiteOptions>? WebsiteOption { get; set; }

    [NotNull]
    private string? DownloadText { get; set; }

    [NotNull]
    private string? HomeText { get; set; }

    [NotNull]
    private string? IntroductionText { get; set; }

    [NotNull]
    private string? ComponentsText { get; set; }

    [NotNull]
    private string? FlowText { get; set; }

    [NotNull]
    private string? InstallAppText { get; set; }

    [NotNull]
    private string? InstallText { get; set; }

    [NotNull]
    private string? CancelText { get; set; }

    [NotNull]
    private string? Title { get; set; }

    [NotNull]
    private JSModule? Module { get; set; }

    private static bool Installable = false;

    [NotNull]
    private static Action? OnInstallable { get; set; }

    private string DownloadUrl => $"{WebsiteOption.CurrentValue.BootstrapBlazorLink}/repository/archive/main.zip";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        DownloadText ??= Localizer[nameof(DownloadText)];
        HomeText ??= Localizer[nameof(HomeText)];
        IntroductionText ??= Localizer[nameof(IntroductionText)];
        ComponentsText ??= Localizer[nameof(ComponentsText)];
        FlowText ??= Localizer[nameof(FlowText)];
        InstallAppText ??= Localizer[nameof(InstallAppText)];
        InstallText ??= Localizer[nameof(InstallText)];
        CancelText ??= Localizer[nameof(CancelText)];
        Title ??= Localizer[nameof(Title)];
        OnInstallable = () => InvokeAsync(StateHasChanged);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="firstRender"></param>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Module = await JSRuntime.LoadModule($"./_content/BootstrapBlazor.Shared/modules/header.js", relative: false);
            await Module.InvokeVoidAsync("Header.init");
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    [JSInvokable]
    public static Task PWAInstallable()
    {
        Installable = true;
        OnInstallable.Invoke();
        return Task.CompletedTask;
    }

    private async Task InstallClicked()
    {
        Installable = false;
        await JSRuntime.InvokeVoidAsync("BlazorPWA.installPWA");
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (Module != null && disposing)
        {
            await Module.InvokeVoidAsync($"Header.dispose");
            await Module.DisposeAsync();
            Module = null;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}
