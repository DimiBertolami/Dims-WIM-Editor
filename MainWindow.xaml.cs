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

        public void MountISO(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("powershell -command \"Mount-DiskImage -ImagePath " + ISO.Text + "\"");
        }

        public void DismountISO(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("powershell -command \"Dismount-DiskImage -ImagePath " + ISO.Text + "\"");
        }

        public void CopyWIM(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION & " +
                                      "pushd \"C:\\ADK\\Assessment and Deployment Kit\\Deployment Tools\" & " +
                                      "call DandISetEnv.bat");
            x.StandardInput.WriteLine("cd .. & cd Windows Preinstallation Environment & " +
                                      "copype %processor_architecture% " + MountPoint.Text);
        }

        private void cpWIM(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
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
            void getWIMInfo(object sender, RoutedEventArgs e)
            {
                Process x = Global.ps;
                Global.Args = "C:\\pe__data\\DISM\\dism.exe /Get-WimInfo /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim";
                x.StandardInput.WriteLine(Global.Args);
            }

            void MountWIM(object sender, RoutedEventArgs e)
            {
                Process x = Global.ps;
                Global.Args = "C:\\pe__data\\DISM\\dism /mount-wim /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim " +
                              "/index:" + Index.Text + " /MountDir:" + MountPoint.Text + "\\MOUNT\"";
                x.StandardInput.WriteLine(Global.Args);
            }


            void about(object sender, RoutedEventArgs e)
            {
                // update file from https://www.catalog.update.microsoft.com/home.aspx and slipstream it into
                // your installation source like i did here.. 
                Global.Args = "echo for /f \"usebackq\" %x in (`dir " + MountPoint.Text + "\\*.msu /b`) do wusa " +
                    MountPoint.Text + "\\%x /quiet /norestart";
                Process x = Global.ps;
                x.StandardInput.WriteLine(Global.Args);
                x.StandardInput.WriteLine("echo for /f \"usebackq tokens=*\" %x in (`dir " + MountPoint.Text +
                    "\\*.msu /b`) do C:\\pe__data\\DISM\\dism /Image:" + MountPoint.Text + "\\MOUNT /Add-Package /PackagePath=\"%x\"");
            }

            void UnMountWIM(object sender, RoutedEventArgs e)
            {
                Process x = Global.ps;
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /unmount-wim /mountdir:" + MountPoint.Text + "\\MOUNT /commit");
            }

            void CleanupWim(object sender, RoutedEventArgs e)
            {
                Process x = Global.ps;
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Cleanup-Wim");
            }


            void adkCopyPE(object sender, RoutedEventArgs e)
            {
                Process x = Global.ps;
                x.StandardInput.WriteLine("setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION & " +
                    "pushd \"C:\\ADK\\Assessment and Deployment Kit\\Deployment Tools\" & " +
                    "call DandISetEnv.bat");
                x.StandardInput.WriteLine("cd .. & " +
                    "cd Windows Preinstallation Environment & " +
                    "copype %processor_architecture% " + MountPoint.Text);
            }




            void getADKWIMNFO(object sender, RoutedEventArgs e)
            {
                Process x = Global.ps;
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                    "/GET-WIMINFO " +
                    "/WIMFILE:" + MountPoint.Text + "\\MEDIA\\SOURCES\\BOOT.WIM " +
                    "/INDEX:" + Index.Text);
            }

            void UnmountAdkIMG(object sender, RoutedEventArgs e)
            {
                Process x = Global.ps;
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /unmount-wim /MountDir:" + MountPoint.Text + "\\MOUNT /COMMIT");
            }


        }

        private void addCabs(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
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

        private void AddCabz(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
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

        private void AddPEDrivers(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Image:" + MountPoint.Text + "\\mount /Add-driver /driver:c:\\pe__data\\drivers /recurse /forceunsigned");
        }

        private void adkMountWIM(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /mount-wim /wimfile:" + MountPoint.Text + "\\media\\sources\\boot.wim" +
                " /index:" + Index.Text + " /MountDir:" + MountPoint.Text + "\\MOUNT");

        }

        private void ADKPESetup(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment")) { return; }
            Process x = Global.ps;
            x.StandardInput.WriteLine("pe__data\\adkwinpesetup.exe /features + /installpath c:\\ADK /Q");
            x.StandardInput.WriteLine("PE Addon for Windows Assessment and Deployment Kit Installed!");
        }

        private void adksetup(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
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

        private void cleanResetBase(object sender, ContextMenuEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Image:" + MountPoint.Text + "\\mount /cleanup-image /StartComponentCleanup /ResetBase");
        }

        private void CleanupIMG(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Image:" + MountPoint.Text + "\\MOUNT /cleanup-image /StartComponentCleanup /ResetBase /defer");
        }

        private void CleanUpMountPoints(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Cleanup-Mountpoints");
        }

        private void CreateBootableISO(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            if (File.Exists(MountPoint.Text + "\\media\\sources\\boot.old")) { File.Delete(MountPoint.Text + "\\media\\sources\\boot.old"); }
            if (File.Exists(MountPoint.Text + "\\media\\sources\\boot2.wim")) { File.Delete(MountPoint.Text + "\\media\\sources\\boot2.wim"); };
            x.StandardInput.WriteLine("pushd C:\\ADK\\Assessment and Deployment Kit\\windows Preinstallation Environment");
            x.StandardInput.WriteLine("echo Y | MakeWinPEMedia.cmd /ISO " + MountPoint.Text + " " + MountPoint.Text + "\\newPE.iso");
        }

        private void CreateBootableUsb(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            if (File.Exists(MountPoint.Text + "\\media\\sources\\boot.old")) { File.Delete(MountPoint.Text + "\\media\\sources\\boot.old"); };
            if (File.Exists(MountPoint.Text + "\\media\\sources\\boot2.wim")) { File.Delete(MountPoint.Text + "\\media\\sources\\boot2.wim"); };
            x.StandardInput.WriteLine("pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\"");
            x.StandardInput.WriteLine("pushd \"C:\\ADK\\Assessment and Deployment Kit\\Deployment Tools\"");
            x.StandardInput.WriteLine("call DandISetEnv.bat");
            x.StandardInput.WriteLine("popd");
            x.StandardInput.WriteLine("echo Y | MakeWinPEMedia.cmd /UFD " + MountPoint.Text + " " + usb.Text);
        }

        private void EchoStatus(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("echo off & cls");
        }

        private void ExportWIMAdk(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("echo boot.wim renamed");
            File.Move(MountPoint.Text + "\\media\\sources\\boot.wim",
                      MountPoint.Text + "\\media\\sources\\boot2.wim", true);
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Export-Image " +
                    "/SourceImageFile:" + MountPoint.Text + "\\media\\sources\\boot2.wim " +
                    "/SourceIndex:" + Index.Text + " " +
                    "/DestinationImageFile:" + MountPoint.Text + "\\media\\sources\\boot.wim");
            x.StandardInput.WriteLine("echo removing backup boot.wim & del " + MountPoint.Text + "\\media\\sources\\boot2.wim /Q /F");
        }

        private void getADKPackages(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Image:" + MountPoint.Text + "\\mount /get-Packages");
        }

        private void getBWIMInfo(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("echo sorry for the info spam!");
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe /Get-WimInfo /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim");
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe /Get-WimInfo /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim  /index:" + Index.Text);
            if(File.Exists(MountPoint.Text + "\\media\\sources\\install.wim"))
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe /Get-WimInfo /WimFile:" + MountPoint.Text + "\\media\\sources\\install.wim");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe /Get-WimInfo /WimFile:" + MountPoint.Text + "\\media\\sources\\install.wim  /index:" + Index.Text);
            }
            if (File.Exists(usb.Text + "\\sources\\boot.wim"))
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe /Get-WimInfo /WimFile:" + usb.Text + "\\sources\\boot.wim");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe /Get-WimInfo /WimFile:" + usb.Text + "\\sources\\boot.wim  /index:" + Index.Text);
            }
            if (File.Exists(usb.Text + "\\sources\\install.wim"))
            {
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe /Get-WimInfo /WimFile:" + usb.Text + "\\sources\\install.wim");
                x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism.exe /Get-WimInfo /WimFile:" + usb.Text + "\\sources\\install.wim  /index:" + Index.Text);
            }
        }

        private void getMountedWIMInfo(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /Get-MountedWimInfo");
        }

        private void getPackages(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /image:" + MountPoint.Text + "\\MOUNT /Get-Packages");
        }

        private void MntInstallWim(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                                            "/mount-wim " +
                                            "/WimFile:" + MountPoint.Text + "\\media\\sources\\install.wim " +
                                            "/index:" + Index.Text + " " +
                                            "/MountDir:" + MountPoint.Text + "\\MOUNT");
        }

        private void oFile(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\pe__data\\";
            dlg.ShowDialog();
            string fileName = dlg.FileName;
            ISO.Text = fileName;
        }

        private void ReplacePEBackgroundImage(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("takeown /F " + MountPoint.Text + "\\mount\\Windows\\System32\\winpe.jpg");
            x.StandardInput.WriteLine("icacls " + MountPoint.Text + "\\mount\\Windows\\System32\\winpe.jpg /grant administrators:F");
            x.StandardInput.WriteLine("copy c:\\pe__data\\winpe.jpg " + MountPoint.Text + "\\mount\\Windows\\System32\\ /Y");
        }

        private void RunDMC(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine(CmdInput.Text);
        }

        private void SlipstreamKB(object sender, RoutedEventArgs e)
        {
            // this is how this works:
            // first deploy your default install.wim on a computer and let it perform windows update.
            // Just make sure you write down which KB-Nrs.msu's you install. then download the correct
            // MS Update file from https://www.catalog.update.microsoft.com/home.aspx . Place it in the
            // root of the mountpoint (c:\mount) and then slipstream them into your installation source.. 
            // piece of cake..

            Process x = Global.ps;
            x.StandardInput.WriteLine("if you see a wild error appear, it might be because no install.wim is mounted");
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism " +
                                            "/Image:" + MountPoint.Text + "\\mount " +
                                            "/Add-Package " +
                                            "/PackagePath=" + MountPoint.Text);
        }

        private void ShowDosBox(object sender, RoutedEventArgs e)
        {
            if (Global.PID == 0)
            {
                Process x1 = new ReadStdOut().CreateProcess("", true);
                x1.StartInfo.RedirectStandardInput = true;
                x1.StandardInput.WriteLine("echo off & color 56");
                Global.PID = x1.Id;
                Global.ps = x1;
            }
            else
            {
                Process x1 = Global.ps;
            }
        }

        private void UnmountWIMAdk(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /unmount-wim /mountdir:" +
                         MountPoint.Text +
                         "\\MOUNT /commit");
        }

        private void UnMountWIMDiscard(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("C:\\pe__data\\DISM\\dism /unmount-wim /mountdir:" + MountPoint.Text + "\\MOUNT /discard");
        }

        private void vOS(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("copy c:\\pe__data\\ValidationOS.wim " + MountPoint.Text + "\\media\\sources\\boot.wim /y");
        }

        private void MaxScratch(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
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

        private void StdInChange(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            ProcessStartInfo psi = x.StartInfo;
            if(Global.RedirectStandardInput == true)
            {
                x.Kill();
                Process x1 = new ReadStdOut().CreateProcess("color 56 & pushd c:\\pe__data & echo off & cls", false);
                Global.ps = x1;
                StdInButton.Background = Brushes.Green;
                Global.RedirectStandardInput = false;
            }
            else
            {
                x.Kill();
                Process x1 = new ReadStdOut().CreateProcess("echo off & cls", true);
                StdInButton.Background = Brushes.Red;
                x1.StandardInput.WriteLine("color 56 & pushd c:\\pe__data & cls");
                Global.ps = x1;
                Global.RedirectStandardInput = true;
            }
        }

        private void ChgEcho(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            if (Global.EchoStat=="echo off")
            {
                x.StandardInput.WriteLine("echo on");
                Global.EchoStat = "echo on";
            } else
            {
                x.StandardInput.WriteLine("echo off");
                Global.EchoStat = "echo off";
            }
        }
    }
}

