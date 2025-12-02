
// © 2025 -=W3IrD_M@@N=-. 
//This code is part of the Zauncher project and is distributed 
//by under the terms of the Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International license. Full text of the 
//license: https://creativecommons.org/licenses/by-nc-sa/4.0/
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Diagnostics;
using System.IO;
namespace Zauncher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

private void BTN_Game_Click(object sender, RoutedEventArgs e)
    {
    // For Debug 
    
    
    // Main Java Command For Start
    string javaPath = "java"; 
    string memoryArgs = "-Xmx1024M -Xms512M";
    string libraryPath = "-Djava.library.path=\"" + AppDomain.CurrentDomain.BaseDirectory + "Ver/natives\"";
    string lwjglOpts = "-Dorg.lwjgl.opengl.Display.allowSoftwareOpenGL=true";
    string classpath = "-cp \"" + 
                        AppDomain.CurrentDomain.BaseDirectory + "Ver/" + CMB_Ver.Text + ".jar:" +
                        AppDomain.CurrentDomain.BaseDirectory + "Ver/lwjgl.jar:" +
                        AppDomain.CurrentDomain.BaseDirectory + "Ver/lwjgl_util.jar:" +
                        AppDomain.CurrentDomain.BaseDirectory + "Ver/jinput.jar\"";
    
    string versionText = CMB_Ver.Text.ToString();
    string mainClass;
    
    if (versionText.StartsWith("Classic0"))
    {
        mainClass = "com.mojang.minecraft.MinecraftApplet";  // For Classic0...
    }
    else if (versionText.Equals("Classic") || 
             versionText.StartsWith("Indev") || 
             versionText.StartsWith("Infdev"))
    {
        mainClass = "net.minecraft.client.MinecraftApplet";  // For Classic, Indev, Infdev
    }
    else if (versionText.StartsWith("Alpha") || 
             versionText.StartsWith("Beta"))
    {
        mainClass = "net.minecraft.client.Minecraft";  // For Alpha & Beta
    }
    else
    {
        // Default for others versions
        mainClass = "net.minecraft.client.Minecraft";
    }
    
    string username = TB_Nick.Text.ToString();
    string arguments = memoryArgs + " " + 
                       libraryPath + " " + 
                       lwjglOpts + " " + 
                       classpath + " " + 
                       mainClass + " " + 
                       "\"" + username + "\"";

Console.WriteLine(javaPath, arguments);
    Process.Start(javaPath, arguments);
    }
private void BTN_Fix_Click(object sender , RoutedEventArgs e)
    {
        try
    {
        string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fix.sh");
        string bashCommand = $"/bin/bash \"{scriptPath}\"";
        
        Console.WriteLine($"Executing: {bashCommand}");
        
        var processInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"\"{scriptPath}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
            CreateNoWindow = true
        };
        
        using (Process process = new Process())
        {
            process.StartInfo = processInfo;
            
            // Обработка вывода
            process.OutputDataReceived += (s, args) => 
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Console.WriteLine($"Output: {args.Data}");
            };
            
            process.ErrorDataReceived += (s, args) => 
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Console.WriteLine($"Error: {args.Data}");
            };
            
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            
            Console.WriteLine($"Process exited with code: {process.ExitCode}");
        }
    }
    catch (Exception ex)
    {
        // Можно также показать сообщение пользователю
        // MessageBox.Show($"Error: {ex.Message}");
    }
    }
}