using System;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static System.IO.FileInfo;
using System.IO.Enumeration;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.IO.Packaging;
//using CmdApp;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.IO.Compression;
using Microsoft.Win32;
using System.Runtime;
using WinISOEditor.Entities;

namespace DimsISOTweaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string StandardArguments = Global.Args;


        public MainWindow()
        {
            InitializeComponent();
        }

        void MountISO(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                x.StandardInput.WriteLine("powershell -command \"Mount-DiskImage -ImagePath " + ISO.Text + "\"");
            }
        }

        void DismountISO(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                x.StandardInput.WriteLine("powershell -command \"Dismount-DiskImage -ImagePath " + ISO.Text + "\"");
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
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /mount-wim /wimfile:" + MountPoint.Text + "\\media\\sources\\boot.wim" +
                    " /index:" + Index.Text + " /MountDir:" + MountPoint.Text + "\\MOUNT");
            }
        }

        void ADKPESetup(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment")) { return; }
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                x.StandardInput.WriteLine("pe__data\\adkwinpesetup.exe /features + /installpath c:\\ADK /Q");
                x.StandardInput.WriteLine("PE Addon for Windows Assessment and Deployment Kit Installed!");
            }
        }

        void adksetup(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                WebClient webClient = Global.webClient;
                if (File.Exists("c:\\pe__data\\adksetup.exe") == false)
                {
                    MessageBox.Show("c:\\pe__data\\adksetup.exe not found");
                    webClient.DownloadFile("https://go.microsoft.com/fwlink/?linkid=2196127", @"c:\\pe__data\\adksetup.exe");
                }
                if (File.Exists("c:\\pe__data\\adwinpeksetup.exe") == false)
                {
                    webClient.DownloadFile("https://go.microsoft.com/fwlink/?linkid=2196224", @"c:\\pe__data\\adwinpeksetup.exe");
                }
                if (File.Exists("c:\\pe__data\\VALIDATIONOS.iso") == false)
                {
                    webClient.DownloadFile("https://aka.ms/DownloadValidationOS", @"c:\\pe__data\\VALIDATIONOS.iso");
                }
                if (File.Exists("c:\\pe__data\\VALIDATIONOSARM64.iso") == false)
                {
                    webClient.DownloadFile("https://aka.ms/DownloadValidationOS_arm64", @"c:\\pe__data\\VALIDATIONOSARM64.iso");
                }
                if (File.Exists("c:\\pe__data\\winpe.jpg") == false)
                {
                    webClient.DownloadFile("https://github.com/DimiBertolami/Dims-WIM-Editor/blob/main/winpe.jpg", @"c:\\pe__data\\winpe.jpg");
                }
                // 
                if (File.Exists("c:\\pe__data\\pstools.zip") == false)
                {
                    webClient.DownloadFile("https://download.sysinternals.com/files/PSTools.zip", @"c:\\pe__data\\pstools.zip");
                }
                if (Directory.Exists("c:\\ADK"))
                {
                    MessageBox.Show("c:\\adk exists! I will simply show the commands I would have used otherwise...");
                    x.StandardInput.WriteLine("echo Windows Assessment and Deployment Kit Installation started!");
                    x.StandardInput.WriteLine("echo c:\\pe__data\\adksetup.exe /features optionid.deploymentTools /installpath c:\\ADK /Q");
                    x.StandardInput.WriteLine("echo Windows Assessment and Deployment Kit Installed!");
                    return;
                }
                else
                {
                    x.StandardInput.WriteLine("echo Windows Assessment and Deployment Kit Installation started!");
                    x.StandardInput.WriteLine("c:\\pe__data\\adksetup.exe /features optionid.deploymentTools /installpath c:\\ADK /Q");
                    x.StandardInput.WriteLine("echo Windows Assessment and Deployment Kit Installed!");
                }
            }
        }

        void cleanResetBase(object sender, ContextMenuEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Image:" + MountPoint.Text + "\\mount /cleanup-image /StartComponentCleanup /ResetBase");
            }
        }

        void CleanupIMG(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Image:" + MountPoint.Text + "\\MOUNT /cleanup-image /StartComponentCleanup /ResetBase /defer");
            }
        }

        void CleanUpMountPoints(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Cleanup-Mountpoints");
            }
        }

        void CreateBootableISO(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                if (File.Exists(MountPoint.Text + "\\media\\sources\\boot.old")) { File.Delete(MountPoint.Text + "\\media\\sources\\boot.old"); }
                if (File.Exists(MountPoint.Text + "\\media\\sources\\boot2.wim")) { File.Delete(MountPoint.Text + "\\media\\sources\\boot2.wim"); };
                x.StandardInput.WriteLine("pushd C:\\ADK\\Assessment and Deployment Kit\\windows Preinstallation Environment");
                x.StandardInput.WriteLine("echo Y | MakeWinPEMedia.cmd /ISO " + MountPoint.Text + " " + MountPoint.Text + "\\newPE.iso");
            }
        }

        void CreateBootableUsb(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                if (File.Exists(MountPoint.Text + "\\media\\sources\\boot.old")) { File.Delete(MountPoint.Text + "\\media\\sources\\boot.old"); };
                if (File.Exists(MountPoint.Text + "\\media\\sources\\boot2.wim")) { File.Delete(MountPoint.Text + "\\media\\sources\\boot2.wim"); };
                x.StandardInput.WriteLine("pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\"");
                x.StandardInput.WriteLine("pushd \"C:\\ADK\\Assessment and Deployment Kit\\Deployment Tools\"");
                x.StandardInput.WriteLine("call DandISetEnv.bat");
                x.StandardInput.WriteLine("popd");
                x.StandardInput.WriteLine("echo Y | MakeWinPEMedia.cmd /UFD " + MountPoint.Text + " " + usb.Text);
            }
        }

        void EchoStatus(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                x.StandardInput.WriteLine("echo off & cls");
            }
        }

        void ExportWIMAdk(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                x.StandardInput.WriteLine("echo boot.wim renamed");
                File.Move(MountPoint.Text + "\\media\\sources\\boot.wim",
                          MountPoint.Text + "\\media\\sources\\boot2.wim", true);
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Export-Image " +
                        "/SourceImageFile:" + MountPoint.Text + "\\media\\sources\\boot2.wim " +
                        "/SourceIndex:" + Index.Text + " " +
                        "/DestinationImageFile:" + MountPoint.Text + "\\media\\sources\\boot.wim");
                x.StandardInput.WriteLine("echo removing backup boot.wim & del " + MountPoint.Text + "\\media\\sources\\boot2.wim /Q /F");
            }
        }

        void getADKPackages(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Image:" + MountPoint.Text + "\\mount /get-Packages");
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
                                                    "/WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe " +
                                                    "/Get-WimInfo " +
                                                    "/WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim  " +
                                                    "/index:" + Index.Text);
                if (File.Exists(MountPoint.Text + "\\media\\sources\\install.wim"))
                {
                    x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe " +
                                                    "/Get-WimInfo " +
                                                    "/WimFile:" + MountPoint.Text + "\\media\\sources\\install.wim");
                    x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe " +
                                                    "/Get-WimInfo " +
                                                    "/WimFile:" + MountPoint.Text + "\\media\\sources\\install.wim  " +
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
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Get-MountedWimInfo");
            }

        }

        void MntInstallWim(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                                                "/mount-wim " +
                                                "/WimFile:" + MountPoint.Text + "\\media\\sources\\install.wim " +
                                                "/index:" + Index.Text + " " +
                                                "/MountDir:" + MountPoint.Text + "\\MOUNT");
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
                x.StandardInput.WriteLine("takeown /F " + MountPoint.Text + "\\mount\\Windows\\System32\\winpe.jpg");
                x.StandardInput.WriteLine("icacls " + MountPoint.Text + "\\mount\\Windows\\System32\\winpe.jpg /grant administrators:F");
                x.StandardInput.WriteLine("copy c:\\pe__data\\winpe.jpg " + MountPoint.Text + "\\mount\\Windows\\System32\\ /Y");
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
                x.StandardInput.WriteLine("if you see a wild error appear, it might be because no install.wim is mounted");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                                                "/Image:" + MountPoint.Text + "\\mount " +
                                                "/Add-Package " +
                                                "/PackagePath=" + MountPoint.Text);
            }
        }

        void UnmountWIMAdk(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /unmount-wim /mountdir:" +
                             MountPoint.Text +
                             "\\MOUNT /commit");
            }
        }

        void UnMountWIMDiscard(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /unmount-wim /mountdir:" + MountPoint.Text + "\\MOUNT /discard");
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
                    x.StandardInput.WriteLine("powershell -command \"Mount-DiskImage -ImagePath c:\\pe__data\\ValidationOS.iso\"");
                    for (int i = 0; i < drives.Count(); i++)
                    {
                        if (File.Exists(drives[i].Name + "ValidationOS.wim"))
                        {
                            usb.Text = drives[i].Name;
                            x.StandardInput.WriteLine("copy " + drives[i].Name + "ValidationOS.wim c:\\pe__data\\ValidationOS.wim /y");
                            x.StandardInput.WriteLine("xcopy /E /Z " + drives[i].Name + "cabs c:\\pe__data\\cabs /y");
                            x.StandardInput.WriteLine("echo validationOS cabs copied from iso");
                            x.StandardInput.WriteLine("copy " + drives[i].Name + "ValidationOS.wim " + MountPoint.Text + "\\media\\sources\\boot.wim /y");
                            x.StandardInput.WriteLine("echo validationOS.wim copied from iso");
                        }
                    }
                    x.StandardInput.WriteLine("powershell -command \"DisMount-DiskImage -ImagePath c:\\pe__data\\ValidationOS.iso\"");
                }
                else
                {
                    x.StandardInput.WriteLine("copy " + usb.Text + "ValidationOS.wim " + MountPoint.Text + "\\media\\sources\\boot.wim /y");
                }
                FileInfo file = new FileInfo(MountPoint.Text + "\\media\\sources\\boot.wim");
                file.IsReadOnly = false;
            }
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

        void ShowDosBox(object sender, RoutedEventArgs e)
        {
            if (Global.PID == 0)
            {
                Process x1 = new ReadStdOut().CreateProcess("", true);
                MainScreen.Left = (((uint)System.Windows.SystemParameters.PrimaryScreenWidth) - MainScreen.Width -23);
                MainScreen.Top = 35;
                x1.StartInfo.RedirectStandardInput = true;
                x1.StartInfo.UseShellExecute = false;
                x1.StartInfo.CreateNoWindow = Global.CreateNoWindow;
                x1.StandardInput.WriteLine("color 56 & cls");
                Global.PID = x1.Id;
                Global.ps = x1;
                Global.PositionX = ((int)System.Windows.SystemParameters.PrimaryScreenWidth) - (int)MainScreen.ActualWidth - 23;
                Global.PositionY = 35;
            }
            else
            {
                Process x1 = Global.ps;
            }
        }

        void StdInChange(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            ProcessStartInfo psi = x.StartInfo;
            if (Global.RedirectStandardInput == true)
            {
                x.Kill();
                Process x1 = new ReadStdOut().CreateProcess("", false);
                Global.ps = x1;
                StdInButton.Background = Brushes.Green;
                Global.RedirectStandardInput = false;
            }
            else
            {
                x.Kill();
                Process x1 = new ReadStdOut().CreateProcess("", true);
                StdInButton.Background = Brushes.Red;
                x1.StandardInput.WriteLine("color 56");
                Global.ps = x1;
                Global.RedirectStandardInput = true;
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

        void WindowStyleChg(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.WindowStyle== ProcessWindowStyle.Maximized) 
            {
                Global.WindowStyle = ProcessWindowStyle.Hidden;
                x.Kill();
                CMDWindowStyle.Background = Brushes.Red;
                x.StartInfo.WindowStyle = Global.WindowStyle;
                x.StartInfo.CreateNoWindow = true;
                x.Start();

            }
            else
            {
                Global.WindowStyle = ProcessWindowStyle.Maximized;
                x.Kill();
                CMDWindowStyle.Background = Brushes.Green;
                x.StartInfo.WindowStyle = Global.WindowStyle;
                x.StartInfo.CreateNoWindow = false;
                x.Start();
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
                    MainScreen.Top = (uint)System.Windows.SystemParameters.PrimaryScreenHeight - (uint)MainScreen.ActualHeight -35;
                    Global.PositionX = (int)System.Windows.SystemParameters.PrimaryScreenHeight - (int)MainScreen.ActualHeight -35;
                    break;
                case 35:
                    MainScreen.Left = ((uint)System.Windows.SystemParameters.PrimaryScreenWidth) - (uint)MainScreen.ActualWidth - 23;
                    Global.PositionX = ((int)System.Windows.SystemParameters.PrimaryScreenWidth) - (int)MainScreen.ActualWidth - 23;
                    MainScreen.Top = 35;
                    Global.PositionY = 35;
                    break;
                default:
                    MainScreen.Left = ((uint)System.Windows.SystemParameters.PrimaryScreenWidth) - (uint)MainScreen.ActualWidth - 23;
                    Global.PositionX = ((int)System.Windows.SystemParameters.PrimaryScreenWidth) - (int)MainScreen.ActualWidth - 23;
                    MainScreen.Top = 35;
                    Global.PositionY = 35;
                    break;
            }
        }

        void CheckEnterKey(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
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
            Process x = Global.ps; MainScreen.Topmost = true;
            if (Global.RedirectStandardInput == true) 
            {
                if (!File.Exists("C:\\Program Files\\qemu\\qemu-system-x86_64.exe"))
                { x.StandardInput.WriteLine("powershell -command \"winget install qemu\""); }
                x.StandardInput.WriteLine("pushd C:\\Program Files\\qemu & " +
                                          "start /b qemu-system-x86_64.exe -boot d -cdrom \"" + ISO.Text + "\" -m 12000");
            } else
            {
            }
        }
    }
}

