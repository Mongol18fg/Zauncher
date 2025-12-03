// © 2025 -=W3IrD_M@@N=-. 
//This code is part of the Zauncher project and is distributed 
//by under the terms of the Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International license. Full text of the 
//license: https://creativecommons.org/licenses/by-nc-sa/4.0/
using Avalonia;
using System;

namespace Zauncher;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
