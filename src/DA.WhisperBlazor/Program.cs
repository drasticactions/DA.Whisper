// <copyright file="Program.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DA.WhisperBlazor;
using DA.Whisper;

var consoleLog = new ConsoleLog(false);
WhisperLogger.Instance.OnLog += (e) =>
{
    switch (e.Level)
    {
        case DA.Whisper.LogLevel.Error:
            consoleLog.LogError(e.Message);
            break;
        case DA.Whisper.LogLevel.Info:
            consoleLog.Log(e.Message);
            break;
        case DA.Whisper.LogLevel.Warning:
            consoleLog.LogWarning(e.Message);
            break;
        default:
            consoleLog.LogDebug(e.Message);
            break;
    }
};

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();