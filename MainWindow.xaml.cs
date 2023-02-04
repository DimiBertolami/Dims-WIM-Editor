//using CmdApp;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DimsISOTweaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string StandardArguments = Global.Args;
        int hWnd = 0;

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        public MainWindow()
        {

            InitializeComponent();
            if (Global.PID == 0)
            {
                Process x1 = new ReadStdOut().CreateProcess();
                MainScreen.Left = (((uint)System.Windows.SystemParameters.PrimaryScreenWidth) - MainScreen.Width - 23);
                MainScreen.Top = 35;
                x1.StartInfo.RedirectStandardInput = true;
                x1.StartInfo.UseShellExecute = false;
                x1.StartInfo.CreateNoWindow = false;
                x1.StandardInput.WriteLine("color 56 & cls");
                Global.PID = x1.Id;
                Global.ps = x1;
                Global.PositionX = ((int)System.Windows.SystemParameters.PrimaryScreenWidth) - ((int)MainScreen.ActualWidth) - 23;
                Global.PositionY = 35;
            }
        }

        void MountISO(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("powershell -command \"Mount-DiskImage -ImagePath '" + ISO.Text + "'\"");
            }
        }

        void DismountISO(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("powershell -command \"DisMount-DiskImage -ImagePath '" + ISO.Text + "'\"");
            }
        }

        void CopyWIM(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION & " +
                                      "pushd \"C:\\ADK\\Assessment and Deployment Kit\\Deployment Tools\" & " +
                                      "call DandISetEnv.bat");
                x.StandardInput.WriteLine("cd .. & cd Windows Preinstallation Environment & " +
                                          "copype %processor_architecture% " + MountPoint.Text);
            }
        }

        void cpWIM(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                Directory.CreateDirectory(MountPoint.Text + "\\Drivers");
                DriveInfo[] drives = DriveInfo.GetDrives();
                for (int i = 0; i < drives.Count(); i++)
                {
                    // boot.wim
                    if (File.Exists(drives[i].Name + "Sources\\boot.wim"))
                    {
                        usb.Text = drives[i].Name;
                        x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                                                                            "/unmount-wim " +
                                                                            "/mountdir:" + MountPoint.Text + "\\MOUNT " +
                                                                            "/commit" +
                                                                            ">nul|| echo skipping unmount...");
                        File.Copy(usb.Text + "Sources\\boot.wim",
                                 MountPoint.Text + "\\media\\sources\\boot.wim",
                                 true);
                        x.StandardInput.WriteLine("echo boot.wim copied from iso");
                        FileInfo file = new FileInfo(MountPoint.Text + "\\media\\sources\\boot.wim");
                        file.IsReadOnly = false;
                    }

                    // install.esd 
                    if (File.Exists(drives[i].Name + "Sources\\install.esd"))
                    {
                        usb.Text = drives[i].Name;
                        var dism1 = "C:\\pe__data\\DISM\\dism " +
                                             "/export-image " +
                                             "/SourceImageFile:" + usb.Text + "sources\\install.esd " +
                                             "/SourceIndex:" + Index.Text + " " +
                                             "/DestinationImageFile:" + MountPoint.Text + "\\media\\sources\\install.wim " +
                                             "/Compress:max " +
                                             "/CheckIntegrity";
                        x.StandardInput.WriteLine(dism1);
                    }
                }
            }
        }

        void addCabs(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("pushd C:\\pe__data\\cabs\\neutral");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /mount-wim " +
                                                                   "/wimfile:" + MountPoint.Text + "\\media\\sources\\boot.wim " +
                                                                   "/index:" + Index.Text + " " +
                                                                   "/MountDir:" + MountPoint.Text + "\\MOUNT" +
                                                                   ">nul|| echo skipping mount...");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Image:" + MountPoint.Text + "\\MOUNT " +
                                                                   "/Add-Package " +
                                                                   "/PackagePath:C:\\pe__data\\cabs\\neutral");
                x.StandardInput.WriteLine("cd .. & cd en-us");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Image:" + MountPoint.Text + "\\MOUNT " +
                                                                   "/Add-Package " +
                                                                   "/PackagePath:C:\\pe__data\\cabs\\en-us");
            }
        }

        void AddCabz(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("ECHO integrating the optional components from the ADK.");
                x.StandardInput.WriteLine("pushd C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /mount-wim " +
                                                       "/wimfile:" + MountPoint.Text + "\\media\\sources\\boot.wim " +
                                                       "/index:" + Index.Text + " " +
                                                       "/MountDir:" + MountPoint.Text + "\\MOUNT" +
                                                       ">nul|| echo skipping mount...");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Image:" +
                                MountPoint.Text + "\\mount " +
                                "/Add-Package " +
                                "/PackagePath:\"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\"");
                x.StandardInput.WriteLine("cd en-us");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                                "/Image:" + MountPoint.Text + "\\mount " +
                                "/Add-Package " +
                                "/PackagePath:\"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\en-us\"");
            }
        }

        void AddPEDrivers(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("md c:\\mount\\DRIVERS &" +
                                          "C:\\pe__data\\DISM\\dism " +
                                                            "/Online " +
                                                            "/Export-Driver " +
                                                            "/Destination:c:\\mount\\DRIVERS");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                                                            "/Image:" + MountPoint.Text + "\\mount " +
                                                            "/Add-driver " +
                                                            "/driver:c:\\mount\\drivers " +
                                                            "/recurse " +
                                                            "/forceunsigned");
            }
        }

        void adkMountWIM(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                                                "/mount-wim " +
                                                "/wimfile:" +
                                                    MountPoint.Text +
                                                    "\\media\\sources\\boot.wim " +
                                                "/index:" + Index.Text + " " +
                                                "/MountDir:" +
                                                    MountPoint.Text + "\\MOUNT");
            }
        }

        void ADKPESetup(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Directory.Exists("C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment"))
            {
                return;
            }
            else
            {
                if (Global.RedirectStandardInput == true)
                {
                    x.StandardInput.WriteLine("start /b /wait " +
                        "c:\\pe__data\\adkwinpesetup.exe " +
                            "/features + " +
                            "/installpath c:\\ADK");
                }
            }
        }

        void adksetup(object sender, RoutedEventArgs e)
        {
            WebClient webClient = new WebClient();
            Global.webClient = webClient;
            Process x = Global.ps;
            MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                if (Directory.Exists("c:\\ADK"))
                {
                    if (Directory.Exists("c:\\pe__data"))
                    {
                        if (File.Exists("c:\\pe__data\\ADKSETUP.EXE"))
                        {
                            if (File.Exists("c:\\pe__data\\ADKWINPE.EXE"))
                            {
                                if (File.Exists("c:\\pe__data\\VALIDATIONOS.ISO"))
                                {
                                    if (File.Exists("c:\\pe__data\\pstools.zip"))
                                    {
                                        if (File.Exists("c:\\pe__data\\winpe.jpg"))
                                        {
                                            x.StandardInput.WriteLine("echo environment ok");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // NoReadOnly(MountPoint.Text, "\\media\\sources\\boot.wim")
                    // 
                    // 


                    Directory.CreateDirectory("c:\\pe__data\\");
                    webClient.DownloadFile(
                        "https://go.microsoft.com/fwlink/?linkid=2196127",
                        @"c:\\pe__data\\adksetup.exe");
                    webClient.DownloadFile(
                        "https://go.microsoft.com/fwlink/?linkid=2196224",
                        @"c:\\pe__data\\adkwinpesetup.exe");
                    webClient.DownloadFile(
                        "https://aka.ms/DownloadValidationOS",
                        @"c:\\pe__data\\VALIDATIONOS.iso");
                    webClient.DownloadFile(
                        "https://download.sysinternals.com/files/PSTools.zip",
                        @"c:\\pe__data\\pstools.zip");
                    webClient.DownloadFile(
                        "https://github.com/DimiBertolami/Dims-WIM-Editor/blob/main/winpe.jpg",
                        @"c:\\pe__data\\winpe.jpg");
                    NoReadOnly(MountPoint.Text, "\\media\\sources\\boot.wim");
                    NoReadOnly(MountPoint.Text, "\\media\\sources\\INSTALL.WIM");
                    NoReadOnly("c:\\pe__data\\", "pstools.zip");
                    NoReadOnly("c:\\pe__data\\", "adksetup.exe");
                    NoReadOnly("c:\\pe__data\\", "validationOS.iso");
                    NoReadOnly("c:\\pe__data\\", "adkwinpesetup.exe");
                    x.StandardInput.WriteLine("ECHO validation OS iso downloaded");
                    x.StandardInput.WriteLine("echo sysinternals pstools downloaded");
                    x.StandardInput.WriteLine("echo custom PE background downloaded");
                    x.StandardInput.WriteLine("START /B /WAIT " +
                        "c:\\pe__data\\adksetup.exe " +
                                "/features optionid.deploymentTools " +
                                "/installpath c:\\ADK"); // +"/Q"
                    x.StandardInput.WriteLine("START /B /WAIT " +
                        "pe__data\\adkwinpesetup.exe " +
                                "/features + " +
                                "/installpath c:\\ADK"); // + "/Q"
                }
            }
        }

        void cleanResetBase(object sender, ContextMenuEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                                                    "/Image:" +
                                                        MountPoint.Text + "\\mount " +
                                                    "/cleanup-image " +
                                                    "/StartComponentCleanup " +
                                                    "/ResetBase");
            }
        }

        void CleanupIMG(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                                                    "/Image:" +
                                                        MountPoint.Text + "\\MOUNT " +
                                                    "/cleanup-image " +
                                                    "/StartComponentCleanup " +
                                                    "/ResetBase");
            }
        }

        void CleanUpMountPoints(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                    "/Cleanup-Mountpoints");
            }
        }

        void CreateBootableISO(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                if (File.Exists(MountPoint.Text +
                    "\\media\\sources\\boot.old"))
                {
                    File.Delete(MountPoint.Text +
                        "\\media\\sources\\boot.old");
                }
                if (File.Exists(MountPoint.Text +
                    "\\media\\sources\\boot2.wim"))
                {
                    File.Delete(MountPoint.Text +
                        "\\media\\sources\\boot2.wim");
                };
                x.StandardInput.WriteLine("pushd " +
                    "C:\\ADK\\Assessment and Deployment Kit\\" +
                    "windows Preinstallation Environment");
                x.StandardInput.WriteLine("echo Y" +
                    " | MakeWinPEMedia.cmd " +
                    "/ISO " + MountPoint.Text +
                    " " +
                    MountPoint.Text + "\\newPE.iso");
            }
        }

        void CreateBootableUsb(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                if (File.Exists(MountPoint.Text +
                    "\\media\\sources\\boot.old"))
                {
                    File.Delete(MountPoint.Text +
                        "\\media\\sources\\boot.old");
                };
                if (File.Exists(MountPoint.Text +
                    "\\media\\sources\\boot2.wim"))
                {
                    File.Delete(MountPoint.Text +
                        "\\media\\sources\\boot2.wim");
                };
                x.StandardInput.WriteLine("pushd" +
                    " \"C:\\ADK\\Assessment and Deployment Kit" +
                    "\\Windows Preinstallation Environment\"");
                x.StandardInput.WriteLine("pushd" +
                    " \"C:\\ADK\\Assessment and Deployment Kit" +
                    "\\Deployment Tools\"");
                x.StandardInput.WriteLine("call DandISetEnv.bat");
                x.StandardInput.WriteLine("popd");
                x.StandardInput.WriteLine("echo Y | MakeWinPEMedia.cmd /UFD " +
                    MountPoint.Text + " " + usb.Text);
            }
        }

        void ExportWIMAdk(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("echo boot.wim renamed");
                File.Move(MountPoint.Text + "\\media\\sources\\boot.wim",
                          MountPoint.Text + "\\media\\sources\\boot2.wim",
                          true);
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                        "/Export-Image " +
                        "/SourceImageFile:" + MountPoint.Text +
                                "\\media\\sources\\boot2.wim " +
                        "/SourceIndex:" + Index.Text + " " +
                        "/DestinationImageFile:" + MountPoint.Text +
                                "\\media\\sources\\boot.wim");
                x.StandardInput.WriteLine("echo removing backup boot.wim & " +
                    "del " + MountPoint.Text +
                        "\\media\\sources\\boot2.wim /Q /F");
            }
        }

        void getADKPackages(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                    "/Image:" + MountPoint.Text + "\\mount " +
                    "/get-Packages");
            }
        }

        void getBWIMInfo(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe " +
                                                    "/Get-WimInfo " +
                                                    "/WimFile:" + MountPoint.Text +
                                                            "\\media\\sources\\boot.wim");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe " +
                                                    "/Get-WimInfo " +
                                                    "/WimFile:" + MountPoint.Text +
                                                            "\\media\\sources\\boot.wim  " +
                                                    "/index:" + Index.Text);
                if (File.Exists(MountPoint.Text + "\\media\\sources\\install.wim"))
                {
                    x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe " +
                                                    "/Get-WimInfo " +
                                                    "/WimFile:" + MountPoint.Text +
                                                            "\\media\\sources\\install.wim");
                    x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe " +
                                                    "/Get-WimInfo " +
                                                    "/WimFile:" + MountPoint.Text +
                                                            "\\media\\sources\\install.wim  " +
                                                    "/index:" + Index.Text);
                }
                if (File.Exists(usb.Text + "\\sources\\boot.wim"))
                {
                    x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe " +
                                                    "/Get-WimInfo " +
                                                    "/WimFile:" + usb.Text + "\\sources\\boot.wim");
                    x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe " +
                                                    "/Get-WimInfo " +
                                                    "/WimFile:" + usb.Text + "\\sources\\boot.wim  " +
                                                    "/index:" + Index.Text);
                }
                if (File.Exists(usb.Text + "\\sources\\install.wim"))
                {
                    x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe " +
                                                    "/Get-WimInfo " +
                                                    "/WimFile:" + usb.Text + "\\sources\\install.wim");
                    x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe " +
                                                    "/Get-WimInfo " +
                                                    "/WimFile:" + usb.Text + "\\sources\\install.wim " +
                                                    "/index:" + Index.Text);
                }
            }
        }

        void getMountedWIMInfo(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                    "/Get-MountedWimInfo");
            }

        }

        void MntInstallWim(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                                                "/mount-wim " +
                                                "/WimFile:" + MountPoint.Text +
                                                        "\\media\\sources\\install.wim " +
                                                "/index:" + Index.Text + " " +
                                                "/MountDir:" +
                                                    MountPoint.Text + "\\MOUNT");
            }
        }

        void oFile(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\pe__data\\";
            dlg.ShowDialog();
            string fileName = dlg.FileName;
            ISO.Text = fileName;
        }

        void ReplacePEBackgroundImage(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("takeown /F " + MountPoint.Text +
                                            "\\mount\\Windows\\System32\\winpe.jpg");
                x.StandardInput.WriteLine("icacls " +
                    MountPoint.Text + "\\mount\\Windows\\System32\\winpe.jpg " +
                    "/grant administrators:F");
                x.StandardInput.WriteLine("copy " +
                    "c:\\pe__data\\winpe.jpg " +
                    MountPoint.Text + "\\mount\\Windows\\System32\\ /Y");
            }
        }

        void RunDMC(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine(CmdInput.Text);
            }
        }

        void SlipstreamKB(object sender, RoutedEventArgs e)
        {
            // this is how this works:
            // first deploy your default install.wim on a computer and let it perform windows update.
            // Just make sure you write down which KB-Nrs.msu's you install. then download the correct
            // MS Update file from https://www.catalog.update.microsoft.com/home.aspx . Place it in the
            // root of the mountpoint (c:\mount) and then slipstream them into your installation source.. 
            // piece of cake..

            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                                                "/Image:" +
                                                    MountPoint.Text + "\\mount " +
                                                "/Add-Package " +
                                                "/PackagePath=" + MountPoint.Text);
            }
        }

        void UnmountWIMAdk(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                            "/unmount-wim " +
                            "/mountdir:" +
                             MountPoint.Text +
                             "\\MOUNT " +
                             "/commit");
            }
        }

        void UnMountWIMDiscard(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                    "/unmount-wim " +
                    "/mountdir:" +
                            MountPoint.Text + "\\MOUNT " +
                    "/discard");
            }
        }

        void vOS(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                if (File.Exists("c:\\pe__data\\validationOS.wim") == false)
                {
                    x.StandardInput.WriteLine("powershell -command" +
                        " \"Mount-DiskImage " +
                        "-ImagePath c:\\pe__data\\ValidationOS.iso\"");
                    for (int i = 0; i < drives.Count(); i++)
                    {
                        if (File.Exists(drives[i].Name + "ValidationOS.wim"))
                        {
                            usb.Text = drives[i].Name;
                            x.StandardInput.WriteLine("copy " +
                                drives[i].Name + "ValidationOS.wim " +
                                "c:\\pe__data\\ValidationOS.wim /y");
                            x.StandardInput.WriteLine("xcopy /E /Z " +
                                drives[i].Name + "cabs " +
                                "c:\\pe__data\\cabs /y");
                            x.StandardInput.WriteLine("copy " +
                                drives[i].Name + "ValidationOS.wim " +
                                MountPoint.Text + "\\media\\sources\\boot.wim /y");
                        }
                    }
                    x.StandardInput.WriteLine("powershell -command" +
                        " \"DisMount-DiskImage " +
                        "-ImagePath c:\\pe__data\\ValidationOS.iso\"");
                }
            }
        }
        public static void NoReadOnly(string mpTxt, string ReadOnlyfile)
        {
            FileInfo file = new FileInfo(mpTxt + ReadOnlyfile);
            file.IsReadOnly = false;
        }

        void MaxScratch(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                x.StandardInput.WriteLine("@C:\\pe__data\\DISM\\dism /mount-wim " +
                                                       "/wimfile:" + MountPoint.Text + "\\media\\sources\\boot.wim " +
                                                       "/index:" + Index.Text + " " +
                                                       "/MountDir:" + MountPoint.Text + "\\MOUNT>nul " +
                                                       "|| echo skipping mount...");
                x.StandardInput.WriteLine("@C:\\pe__data\\DISM\\DISM.exe " +
                                                       "/Remount-Wim " +
                                                       "/MountDir:" + MountPoint.Text + "\\MOUNT>nul || echo skipping remount...");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe " +
                                                       "/Image:" + MountPoint.Text + "\\MOUNT " +
                                                       "/Set-ScratchSpace:512");
            }
        }

        void StdInChange(object sender, RoutedEventArgs e)
        {
            if (Global.PID == 0)
            {
                Process x1 = new ReadStdOut().CreateProcess();
            }
            else
            {
                Process x = Global.ps; MainScreen.Topmost = true;
                ProcessStartInfo psi = x.StartInfo;
                if (Global.RedirectStandardInput == true)
                {
                    x.Kill();
                    Process x1 = new ReadStdOut().CreateProcess(false);
                    Global.ps = x1;
                    StdInButton.Background = Brushes.Green;
                    Global.RedirectStandardInput = false;
                }
                else
                {
                    x.Kill();
                    Process x1 = new ReadStdOut().CreateProcess();
                    StdInButton.Background = Brushes.Red;
                    x1.StandardInput.WriteLine("color 56 & " +
                        "title the purple terminal is controlled " +
                        "by the gui .. The blue one by you & cls");
                    Global.ps = x1;
                    Global.RedirectStandardInput = true;
                }
            }
        }

        void ChgEcho(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (x.StartInfo.RedirectStandardInput == true)
            {
                if (Global.EchoStat == "echo off")
                {
                    x.StandardInput.WriteLine("echo on");
                    Global.EchoStat = "echo on";
                    EchoStat.Background = Brushes.Green;
                }
                else
                {
                    x.StandardInput.WriteLine("echo off");
                    Global.EchoStat = "echo off";
                    EchoStat.Background = Brushes.Red;
                }
            }
        }

        private void StdOutClick(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.WindowStyle == ProcessWindowStyle.Maximized)
            {
                Global.WindowStyle = ProcessWindowStyle.Minimized;
                hWnd = x.MainWindowHandle.ToInt32();
                Global.Handle = hWnd;
                Out.Background = Brushes.Red;
                ShowWindow(hWnd, SW_HIDE);
                //x.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                //x.StartInfo.CreateNoWindow = false;
                //x.Kill();
                //x.Start();
            }
            else
            {
                Global.WindowStyle = ProcessWindowStyle.Maximized;
                Out.Background = Brushes.Green;
                ShowWindow(hWnd, SW_SHOW);
                hWnd = 0;

                //x.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                //x.StartInfo.CreateNoWindow = false;
                //x.Kill();
                //x.Start();
            }
        }

        void OnTopAgain(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        void get_res(object sender, RoutedEventArgs e)
        {
            Global.PositionX = (int)MainScreen.Left;
            Global.PositionY = 35;
            MainScreen.Topmost = true;
            switch (Global.PositionX)
            {
                case 1297:
                    MainScreen.Left = 35;
                    Global.PositionX = 35;
                    MainScreen.Top =
                        (uint)System.Windows.SystemParameters.PrimaryScreenHeight -
                        (uint)MainScreen.ActualHeight - 35;
                    Global.PositionX =
                        (int)System.Windows.SystemParameters.PrimaryScreenHeight -
                        (int)MainScreen.ActualHeight - 35;
                    break;
                case 35:
                    MainScreen.Left =
                        ((uint)System.Windows.SystemParameters.PrimaryScreenWidth) -
                        (uint)MainScreen.ActualWidth - 23;
                    Global.PositionX =
                        ((int)System.Windows.SystemParameters.PrimaryScreenWidth) -
                        (int)MainScreen.ActualWidth - 23;
                    MainScreen.Top = 35;
                    Global.PositionY = 35;
                    break;
                default:
                    MainScreen.Left =
                        ((uint)System.Windows.SystemParameters.PrimaryScreenWidth) -
                        (uint)MainScreen.ActualWidth - 23;
                    Global.PositionX =
                        ((int)System.Windows.SystemParameters.PrimaryScreenWidth) -
                        (int)MainScreen.ActualWidth - 23;
                    MainScreen.Top = 35;
                    Global.PositionY = 35;
                    break;
            }
        }

        void CheckEnterKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Process x = Global.ps; MainScreen.Topmost = true;
                if (Global.RedirectStandardInput == true)
                {
                    x.StandardInput.WriteLine(CmdInput.Text);
                }
            }
        }

        void TestISO(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; 
            MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true)
            {
                if (File.Exists("C:\\Program Files\\qemu\\qemu-system-x86_64.exe")==false)
                { x.StandardInput.WriteLine("powershell -command \"winget install qemu\""); }
            }
            else
            {
                x.StandardInput.WriteLine("pushd C:\\Program Files\\qemu & " +
                                          "start qemu-system-x86_64.exe " +
                                                "-boot d " +
                                                "-cdrom \"" + ISO.Text + "\" " +
                                                "-m 8000");
            }
        }
    }
}