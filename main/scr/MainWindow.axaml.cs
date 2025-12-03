
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
if (TB_Nick.Text == null || TB_Nick.Text == "")
{
    TB_Status.Text = "Set a nickname first!";
}
else 
{
    string versionText = CMB_Ver.Text.ToString();
    string username = TB_Nick.Text.ToString();
    
    bool isOldVersion = versionText.StartsWith("Classic0") || 
                       versionText.StartsWith("Classic_") || 
                       versionText.StartsWith("Indev_") || 
                       versionText.StartsWith("Infdev_") ||
                       versionText.StartsWith("Alpha0");
    
    if (isOldVersion)
    {
        bool java8Exists = false;
        string java8Path = "";
        
        try
        {
            Process checkJava8 = new Process();
            checkJava8.StartInfo.FileName = "/bin/bash";
            checkJava8.StartInfo.Arguments = "-c \"ls -d /usr/lib/jvm/* 2>/dev/null | grep -E 'java-8|jdk-8|jre-8|1.8|temurin-8'\"";
            checkJava8.StartInfo.UseShellExecute = false;
            checkJava8.StartInfo.RedirectStandardOutput = true;
            checkJava8.StartInfo.CreateNoWindow = true;
            
            checkJava8.Start();
            string java8Result = checkJava8.StandardOutput.ReadToEnd().Trim();
            checkJava8.WaitForExit();
            
            if (!string.IsNullOrEmpty(java8Result))
            {
                string[] javaPaths = java8Result.Split('\n');
                java8Path = javaPaths[0].Trim();
                java8Exists = true;
                Console.WriteLine($"Found Java 8 at: {java8Path}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Java check error: {ex.Message}");
        }
        
        if (!java8Exists)
        {
            TB_Status.Text = "Java 8 required! Install Temurin 8:\nhttps://adoptium.net/temurin/releases/?version=8";
            return;
        }
        
        // Определяем правильный main class для каждой версии
        string mainClass;
        if (versionText.StartsWith("Classic0"))
        {
            mainClass = "com.mojang.minecraft.MinecraftApplet"; // Classic0
        }
        else if (versionText.StartsWith("Classic_") || 
                 versionText.StartsWith("Indev_") || 
                 versionText.StartsWith("Infdev_") ||
                 versionText.StartsWith("Alpha0"))
        {
            mainClass = "net.minecraft.client.MinecraftApplet"; // Classic, Indev, Infdev, Alpha0
        }
        else
        {
            mainClass = "net.minecraft.client.MinecraftApplet"; // по умолчанию
        }
        
        // Генерация HTML в папке Ver
        string verFolder = AppDomain.CurrentDomain.BaseDirectory + "Ver/";
        string htmlFile = Path.Combine(verFolder, $"{versionText}_{username}.html");
        string jarPath = Path.Combine(verFolder, versionText + ".jar");
        
        // Исправленный HTML с правильными параметрами
        string htmlContent = $@"<!DOCTYPE html>
<html>
<head>
    <title>Minecraft {versionText}</title>
</head>
<body>
<applet 
    code=""{mainClass}""
    archive=""{versionText}.jar""
    width=""854""
    height=""480"">
    <param name=""username"" value=""{username}"">
    <param name=""sessionid"" value=""12345"">
    <param name=""server"" value="""">
    <param name=""port"" value="""">
    <param name=""mppass"" value="""">
    <param name=""stand-alone"" value=""true"">
</applet>
</body>
</html>";
        
        File.WriteAllText(htmlFile, htmlContent);
        
        try
        {
            // Создаем файл политики безопасности
            string policyFile = Path.Combine(verFolder, "java.policy");
            string policyContent = @"grant {
    permission java.security.AllPermission;
};";
            
            File.WriteAllText(policyFile, policyContent);
            
            // Собираем команду для апплета
            string appletCommand;
            string memoryArgs = "";
            
            // Для старых версий памяти поменьше
            if (versionText.StartsWith("Classic0") || versionText.StartsWith("Classic_"))
            {
                memoryArgs = "-Xmx256M -Xms128M";
            }
            else
            {
                memoryArgs = "-Xmx512M -Xms256M";
            }
            
            // Параметры безопасности для отключения sandbox
            string securityArgs = $"-Djava.security.manager -Djava.security.policy==\"{policyFile}\" -Djava.awt.headless=false";
            
            // Путь к natives для старых версий
            string nativesPath = Path.Combine(verFolder, "natives");
            string libraryPath = $"-Djava.library.path=\"{nativesPath}\"";
            
            // Основной classpath с библиотеками
            string classpath = $"-cp \"{jarPath}:{verFolder}lwjgl.jar:{verFolder}lwjgl_util.jar:{verFolder}jinput.jar\"";
            
            if (!string.IsNullOrEmpty(java8Path))
            {
                // Вариант 1: Запуск через AppletViewer
                appletCommand = $"cd \"{verFolder}\" && \"{java8Path}/bin/java\" {memoryArgs} {securityArgs} {libraryPath} {classpath} sun.applet.AppletViewer \"{versionText}_{username}.html\"";
                
                // Вариант 2: Запуск как standalone приложение (если апплет не работает)
                // appletCommand = $"cd \"{verFolder}\" && \"{java8Path}/bin/java\" {memoryArgs} {securityArgs} {libraryPath} {classpath} {mainClass} \"{username}\"";
            }
            else
            {
                appletCommand = $"cd \"{verFolder}\" && java {memoryArgs} {securityArgs} {libraryPath} {classpath} sun.applet.AppletViewer \"{versionText}_{username}.html\"";
            }
            
            Console.WriteLine($"Applet command: {appletCommand}");
            
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "/bin/bash";
            psi.Arguments = $"-c \"{appletCommand}\"";
            psi.UseShellExecute = false;
            psi.CreateNoWindow = false;
            psi.WorkingDirectory = verFolder;
            
            Process.Start(psi);
            TB_Status.Text = $"Launched {versionText} as applet";
        }
        catch (Exception ex)
        {
            TB_Status.Text = $"Applet error: {ex.Message}";
        }
    }
    else
    {
        // Оригинальный запуск для Alpha и выше (без апплета)
        string verFolder = AppDomain.CurrentDomain.BaseDirectory + "Ver/";
        string javaPath = "java"; 
        string memoryArgs = "-Xmx1024M -Xms512M";
        string libraryPath = "-Djava.library.path=\"" + verFolder + "natives\"";
        string lwjglOpts = "-Dorg.lwjgl.opengl.Display.allowSoftwareOpenGL=true";
        string classpath = "-cp \"" + 
                            verFolder + CMB_Ver.Text + ".jar:" +
                            verFolder + "lwjgl.jar:" +
                            verFolder + "lwjgl_util.jar:" +
                            verFolder + "jinput.jar\"";
        
        string mainClass;
        
        if (versionText.StartsWith("Classic0"))
        {
            mainClass = "com.mojang.minecraft.MinecraftApplet";
        }
        else if (versionText.StartsWith("Alpha") || versionText.StartsWith("Beta"))
        {
            mainClass = "net.minecraft.client.Minecraft";
        }
        else
        {
            mainClass = "net.minecraft.client.Minecraft";
        }
        
        string arguments = memoryArgs + " " + 
                           libraryPath + " " + 
                           lwjglOpts + " " + 
                           classpath + " " + 
                           mainClass + " " + 
                           "\"" + username + "\"";

        Console.WriteLine("Command: " + javaPath + " " + arguments);
        
        try
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = javaPath;
            psi.Arguments = arguments;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = false;
            psi.WorkingDirectory = verFolder;
            
            Process.Start(psi);
            TB_Status.Text = $"Launched {versionText}";
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка запуска: " + ex.Message);
            TB_Status.Text = ex.Message;
        }
    }
}
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
            
            process.OutputDataReceived += (s, args) => 
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Console.WriteLine($"Output: {args.Data}");
                    TB_Status.Text = $"Output: {args.Data}";
            };
            
            process.ErrorDataReceived += (s, args) => 
            {
                if (!string.IsNullOrEmpty(args.Data))
                    Console.WriteLine($"Error: {args.Data}");
                    TB_Status.Text = $"Error: {args.Data}";
            };
            
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            
            TB_Status.Text = "Done!";
            Console.WriteLine($"Process exited with code: {process.ExitCode}");
            TB_Status.Text = $"Process exited with code: {process.ExitCode}";
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        TB_Status.Text = ex.Message;
    
    }
    }
}