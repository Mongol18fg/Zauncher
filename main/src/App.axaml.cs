// © 2025 -=W3IrD_M@@N=-. 
//This code is part of the Zauncher project and is distributed 
//by under the terms of the Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International license. Full text of the 
//license: https://creativecommons.org/licenses/by-nc-sa/4.0/
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Zauncher;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}