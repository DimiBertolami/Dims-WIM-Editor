using System;
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
using CmdApp;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Reflection;

namespace DimsISOTweaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string StandardArguments = Global.Args;
        private object val;

        public int ID { get; set; } = 0;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void SpawnAShell(object sender, RoutedEventArgs e)
        {
            if (Global.PID == 0)
            {
                Global.Args = "@echo off";
                Process x = new ReadStdOut().CreateProcess(Global.Args, false, StandardArguments);
                x.StandardInput.WriteLine("title Dim's PE Shell & cls");
                x.StartInfo.RedirectStandardInput = false;
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("@echo off & title Dim's PE Shell & cls");
                x.StartInfo.RedirectStandardInput = false;
            }
        }


        public void MountISO(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("echo Mounting ISO...", false, StandardArguments);
                x.StandardInput.WriteLine("@echo off & powershell - command \"Mount-DiskImage -ImagePath " + ISO.Text + "\"");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps; 
                Global.Args = "powershell -command \"Mount-DiskImage -ImagePath " + ISO.Text + "\"";
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine(Global.Args);
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        public void DismountISO(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Global.Args = "@echo off & powershell -command \"Dismount-DiskImage -ImagePath " + ISO.Text + "\"";
                Process x = new ReadStdOut().CreateProcess("echo Dis-Mounting ISO...", false, StandardArguments);
                x.StandardInput.WriteLine(Global.Args);
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                Global.Args = "powershell -command \"Dismount-DiskImage -ImagePath " + ISO.Text + "\"";
                //x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine(Global.Args);
                //x.StartInfo.RedirectStandardInput = false;
            }
        }

        public void CopyWIM(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("echo creating local PE Environment...", false, StandardArguments);
                x.StandardInput.WriteLine("setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION & pushd \"C:\\ADK\\Assessment and Deployment Kit\\Deployment Tools\" & call DandISetEnv.bat");
                x.StandardInput.WriteLine("cd .. & cd Windows Preinstallation Environment & copype %processor_architecture% " + MountPoint.Text);
                x.StartInfo.RedirectStandardInput = false;
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                //x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION & pushd \"C:\\ADK\\Assessment and Deployment Kit\\Deployment Tools\" & call DandISetEnv.bat");
                x.StandardInput.WriteLine("cd .. & cd Windows Preinstallation Environment & copype %processor_architecture% " + MountPoint.Text);
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void cpWIM(object sender, RoutedEventArgs e)
        {
            Directory.CreateDirectory(MountPoint.Text + "\\MOUNTDIR");
            Directory.CreateDirectory(MountPoint.Text + "\\Drivers");
            Directory.CreateDirectory(MountPoint.Text + "\\BootWIM");
            DriveInfo[] drives = DriveInfo.GetDrives();
            for (int i = 0; i < drives.Count(); i++)
            {
                if (File.Exists(drives[i].Name + "Sources\\boot.wim"))
                {
                    usb.Text = drives[i].Name;
                    usb.FontSize = 13;
                    FileInfo fileInfo = new FileInfo(MountPoint.Text + "\\media\\sources\\boot.wim");
                    fileInfo.IsReadOnly = false;
                    File.Copy(usb.Text + "Sources\\boot.wim",
                             MountPoint.Text + "\\media\\sources\\boot.wim", true);
                    //File.Copy(usb.Text + "Sources\\boot.wim",
                    //         MountPoint.Text + "\\media\\sources\\boot.wim", true);
                    int PID = Global.PID;
                    if (PID == 0)
                    {
                        Process x = new ReadStdOut().CreateProcess("echo creating local PE Environment...", false, StandardArguments);
                        x.StandardInput.WriteLine("echo off & echo boot.wim copied from gandalfPE");
                        x.StartInfo.RedirectStandardInput = false;
                        Global.ps = x;
                    } else
                    {
                        Process x = Global.ps;
                        x.StartInfo.RedirectStandardInput = true;
                        x.StandardInput.WriteLine("echo boot.wim copied from gandalfPE");
                        x.StartInfo.RedirectStandardInput = false;
                    }
                }
            }

        }

        //}
        public void getWIMInfo(object sender, RoutedEventArgs e)
        {
            //new ReadStdOut().CreateProcess(" /c DISM.exe /Get-WimInfo /WimFile:" + MountPoint.Text + "\\BootWIM\\boot.wim", _contentLoaded, StandardArguments);
            int PID = Global.PID;
            if (PID == 0)
            {
                Global.Args = "DISM.exe /Get-WimInfo /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim";
                Process x = new ReadStdOut().CreateProcess("@echo off & cls", false, StandardArguments);
                x.StandardInput.WriteLine(Global.Args);
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                Global.Args = "DISM.exe /Get-WimInfo /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim";
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine(Global.Args);
                x.StartInfo.RedirectStandardInput = false;
            }


        }
        public void MountWIM(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Global.Args = "DISM /mount-wim /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim /index:" + Index.Text + " /MountDir:" + MountPoint.Text + "\\MOUNT\"";
                Process x = new ReadStdOut().CreateProcess("@echo off & echo Mounting WIM", false, StandardArguments);
                x.StandardInput.WriteLine(Global.Args);
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                Global.Args = "DISM /mount-wim /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim /index:" + Index.Text + " /MountDir:" + MountPoint.Text + "\\MOUNT\"" ;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine(Global.Args);
                x.StartInfo.RedirectStandardInput = false;
            }
        }
        public void AddPEDrivers(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Global.Args = "DISM /mount-wim /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim /index:" + Index.Text + " /MountDir:" + MountPoint.Text + "\\MOUNT\"";
                Process x = new ReadStdOut().CreateProcess("@echo off & echo Adding PE Drivers...", false, StandardArguments);
                x.StandardInput.WriteLine(Global.Args);
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                Global.Args = "DISM /mount-wim /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim /index:" + Index.Text + " /MountDir:" + MountPoint.Text + "\\MOUNT\"";
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine(Global.Args);
                x.StartInfo.RedirectStandardInput = false;
            }
        }
        void SlipstreamKB(object sender, RoutedEventArgs e)
        {
            // this is how this works:
            // first deploy your default install.wim on a computer and let it perform windows update.
            // Just make sure you write down which KB-Nrs.msu's you install. then download the correct
            // update file from https://www.catalog.update.microsoft.com/home.aspx and slipstream it into
            // your installation source like i did here.. 
            Global.Args = "for /f \"usebackq\" %x in (`dir " + MountPoint.Text + "\\*.msu /b`) do wusa " +
                MountPoint.Text + "\\%x /quiet /norestart";
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo slipstreaming updates...", false, StandardArguments);
                x.StandardInput.WriteLine(Global.Args);
                x.StandardInput.WriteLine("for /f \"usebackq\" %x in (`dir " + MountPoint.Text +
                    "\\*.msu /b`) do dism /Image:" + MountPoint.Text + "\\MOUNT /Add-Package /PackagePath=%x");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine(Global.Args);
                x.StandardInput.WriteLine("for /f \"usebackq\" %x in (`dir " + MountPoint.Text +
                    "\\*.msu /b`) do dism /Image:" + MountPoint.Text + "\\MOUNT /Add-Package /PackagePath=%x");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void UnMountWIM(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo Unmounting WIM... (committing changes)", false, StandardArguments);
                x.StandardInput.WriteLine("dism /unmount-wim /mountdir:" + MountPoint.Text + "\\MOUNT /commit");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("dism /unmount-wim /mountdir:" + MountPoint.Text + "\\MOUNT /commit");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void about(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("how to update windows 10 ISO/Wim (By Dimi Bertolami)\r\n\r\ntopics:\r\n- extra drivers\r\n- slipstream microsoft kb-updates\r\n- extra executables\r\n- how to write a PE-Network script\r\n\r\nfrom dosbox: \r\n\r\ncreate some temporary directories:\r\n(Here i'll download all the hotfixes)\r\nMD C:\\Mount\r\n\r\n:: extra drivers go into this directory. They will be installed recursively\r\nMD C:\\Mount\\Drivers\r\n\r\n:: boot.wim from the windows ISO goes here\r\nMD C:\\Mount\\BootWIM\r\n\r\n:: this is our Mount-Target Directory (in order to mount a wim file this folder has to be empty)\r\nmd C:\\MOUNT\\MOUNTDIR\r\n\r\nTo Mount an ISO with powershell: \r\npowershell -Command \"Mount-DiskImage -ImagePath C:\\Users\\Admin\\Desktop\\dewSystems\\ISO\\Gandalf10PE.ISO\"\r\n\r\ndetect drive letter where iso is mounted and copy wim to bootWIM Folder\r\nFOR /D %x in (A B C D E F G H I J K L M N O P Q R S T U V W X Y Z) do if EXIST %x:\\sources\\boot.wim (copy %x:\\sources\\boot.wim C:\\Mount\\BootWIM)\r\n\r\ncheck what image index to use\r\ndism /Get-WimInfo /WimFile:C:\\Mount\\BootWIM\\boot.wim\r\n\r\nmount wim file into mount-directory\r\ndism /mount-wim /wimfile:C:\\Mount\\BootWIM\\boot.wim /index:1 /MountDir:C:\\Mount\\MOUNTDIR\r\n\r\nrecursively add drivers to your PE (you must mount wim file first): \r\ndism /image:C:\\Mount\\MOUNTDIR /Add-Driver /Driver:D:\\Drivers /recurse\r\n\r\ndownload necessary updates manually\r\nslipstream downloaded windows updates into your solution\r\nDism /Image:C:\\Mount\\MOUNTDIR /Add-Package /PackagePath=kb4456655.msu /LogPath=C:\\mount\\dism.log\r\n\r\n");
        }

        private void addCabs(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo Adding optional components...", false, StandardArguments);
                // pushd C:\ESD\cabs\neutral 
                // dism / image:C:\MOUNT\MOUNTDIR / Add - Package / PackagePath:C:\ESD\cabs\neutral
                // dism /image:C:\MOUNT\MOUNTDIR /Add-Package /PackagePath:C:\ESD\cabs\en-us
                x.StandardInput.WriteLine("pushd C:\\ESD\\cabs\\neutral");
                x.StandardInput.WriteLine("dism /image:" + MountPoint.Text + "\\MOUNT /Add-Package /PackagePath:C:\\ESD\\cabs\\neutral");
                x.StandardInput.WriteLine("cd .. & cd en-us");
                x.StandardInput.WriteLine("dism /Image:" + MountPoint.Text + "\\MOUNT /Add-Package /PackagePath:C:\\ESD\\cabs\\en-us");
                x = Global.ps;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("pushd C:\\ESD\\cabs\\neutral");
                x.StandardInput.WriteLine("dism /Image:" + MountPoint.Text + "\\MOUNT /Add-Package /PackagePath:C:\\ESD\\cabs\\neutral");
                x.StandardInput.WriteLine("cd .. & cd en-us");
                x.StandardInput.WriteLine("dism /Image:" + MountPoint.Text + "\\MOUNT /Add-Package /PackagePath:C:\\ESD\\cabs\\en-us");
                //x.StartInfo.RedirectStandardInput = false;
            }
        }
        private void AddCabz(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo adding optional components...", false, StandardArguments);
                x.StandardInput.WriteLine("pushd C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs");
                x.StandardInput.WriteLine("dism /Image:" + MountPoint.Text + "\\mount /Add-Package /PackagePath:\"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs");
                x.StandardInput.WriteLine("cd en-us");
                x.StandardInput.WriteLine("dism /Image:" + MountPoint.Text + "\\mount /Add-Package /PackagePath:\"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\en-us");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                //x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("pushd C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs");
                x.StandardInput.WriteLine("dism /Image:" + MountPoint.Text + "\\mount /Add-Package /PackagePath:\"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs");
                x.StandardInput.WriteLine("cd en-us");
                x.StandardInput.WriteLine("dism /Image:" + MountPoint.Text + "\\mount /Add-Package /PackagePath:\"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\en-us");
                //x.StartInfo.RedirectStandardInput = false;
            }
        }


        private void UnMountWIMDiscard(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo Unmounting WIM... (discarding changes)", false, StandardArguments);
                x.StandardInput.WriteLine("dism /unmount-wim /mountdir:" + MountPoint.Text + "\\MOUNT /discard");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("dism /unmount-wim /mountdir:" + MountPoint.Text + "\\MOUNT /discard");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void CleanUpMountPoints(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo cleaning MountPoints...", false, StandardArguments);
                x.StandardInput.WriteLine("dism /Cleanup-Mountpoints");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("dism /Cleanup-Mountpoints");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void CleanupWim(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo cleaning WIM...", false, StandardArguments);
                x.StandardInput.WriteLine("dism /Cleanup-Wim");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("dism /Cleanup-Wim");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void getMountedWIMInfo(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo getting Mounted WIM details...", false, StandardArguments);
                x.StandardInput.WriteLine("dism /Get-MountedWimInfo");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("dism /Get-MountedWimInfo");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void getPackages(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo getting installed optional components...", false, StandardArguments);
                x.StandardInput.WriteLine("dism /image:" + MountPoint.Text + "\\MOUNT /Get-Packages");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("dism /image:" + MountPoint.Text + "\\MOUNT /Get-Packages");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void CleanupIMG(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo cleaning up image...", false, StandardArguments);
                x.StandardInput.WriteLine("Dism /Image:" + MountPoint.Text + "\\MOUNT /cleanup-image /StartComponentCleanup /ResetBase");
                x.StandardInput.WriteLine("Dism /Unmount-Image /MountDir:" + MountPoint.Text + "\\mount /Commit");
                //x.StandardInput.WriteLine("Dism /Export-Image /SourceImageFile:" + MountPoint.Text + "\\media\\sources\\install.wim /SourceIndex:1 /DestinationImageFile:" + MountPoint.Text + "\\BootWIM\\install_cleaned.wim");
                x.StandardInput.WriteLine("Dism /Export-Image /SourceImageFile:" + MountPoint.Text + "\\media\\sources\\boot.wim /SourceIndex:1 /DestinationImageFile:" + MountPoint.Text + "\\BootWIM\\boot_cleaned.wim");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("Dism /Image:" + MountPoint.Text + "\\MOUNT /cleanup-image /StartComponentCleanup /ResetBase");
                x.StandardInput.WriteLine("Dism /Unmount-Image /MountDir:" + MountPoint.Text + "\\mount /Commit");
                x.StandardInput.WriteLine("Dism /Export-Image /SourceImageFile:" + MountPoint.Text + "\\media\\sources\\boot.wim /SourceIndex:1 /DestinationImageFile:" + MountPoint.Text + "\\BootWIM\\boot_cleaned.wim");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void adksetup(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("c:\\ADK")) { return; }
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo Installing ADK...", false, StandardArguments);
                x.StandardInput.WriteLine("c:\\pe__data\\adksetup.exe /features optionid.deploymentTools /installpath c:\\ADK /Q");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("c:\\pe__data\\adksetup.exe /features optionid.deploymentTools /installpath c:\\ADK /Q");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void ADKPESetup(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment")) { return; }
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo Installing ADK PE Addon...", false, StandardArguments);
                x.StandardInput.WriteLine("c:\\pe__data\\adkwinpesetup.exe /features + /installpath c:\\ADK /Q");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("c:\\pe__data\\adkwinpesetup.exe /features + /installpath c:\\ADK /Q");
                x.StartInfo.RedirectStandardInput = false;
            }
        }


        private void adkMountWIM(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo Installing ADK Mounting WIM ...", false, StandardArguments);
                x.StandardInput.WriteLine("DISM /mount-wim /wimfile:" + MountPoint.Text + "\\media\\sources\\boot.wim" +
                                                " /index:" + Index.Text + " /MountDir:" + MountPoint.Text + "\\MOUNT");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("DISM /mount-wim /wimfile:" + MountPoint.Text + "\\media\\sources\\boot.wim" +
                                                " /index:" + Index.Text + " /MountDir:" + MountPoint.Text + "\\MOUNT");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void adkCopyPE(object sender, RoutedEventArgs e)
        {
            //copype amd64 C:\Mount
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo copying ADK environment...", false, StandardArguments);
                //x.StandardInput.WriteLine("pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\" & copype %processor_architecture% " + MountPoint.Text);                
                x.StandardInput.WriteLine("setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION & pushd \"C:\\ADK\\Assessment and Deployment Kit\\Deployment Tools\" & call DandISetEnv.bat");
                x.StandardInput.WriteLine("cd .. & cd Windows Preinstallation Environment & copype %processor_architecture% " + MountPoint.Text);
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION & pushd \"C:\\ADK\\Assessment and Deployment Kit\\Deployment Tools\" & call DandISetEnv.bat");
                x.StandardInput.WriteLine("cd .. & cd Windows Preinstallation Environment & copype %processor_architecture% " + MountPoint.Text);
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void vOS(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo copying Validation OS Wim File...", false, StandardArguments);
                x.StandardInput.WriteLine("copy c:\\PE__Data\\ValidationOS.wim " + MountPoint.Text + "\\media\\sources\\boot.wim /y");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("copy c:\\PE__Data\\ValidationOS.wim " + MountPoint.Text + "\\media\\sources\\boot.wim /y");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void getADKPackages(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo getting optional components...", false, StandardArguments);
                x.StandardInput.WriteLine("dism /Image:" + MountPoint.Text + "\\mount /get-Packages");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("dism /Image:" + MountPoint.Text + "\\mount  /get-Packages");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void cleanResetBase(object sender, ContextMenuEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo getting optional components...", false, StandardArguments);
                x.StandardInput.WriteLine("Dism /Image:c:\\PE%processor_architecture%\\mount /cleanup-image /StartComponentCleanup /ResetBase");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("Dism /Image:c:\\PE%processor_architecture%\\mount /cleanup-image /StartComponentCleanup /ResetBase");
                x.StartInfo.RedirectStandardInput = false;
            }            
        }

        private void UnmountWIMAdk(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo getting optional components...", false, StandardArguments);
                x.StandardInput.WriteLine("dism /unmount-wim /mountdir:" +
                             MountPoint.Text +
                             "\\MOUNT /commit");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("dism /unmount-wim /mountdir:" +
                             MountPoint.Text +
                             "\\MOUNT /commit");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void ExportWIMAdk(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo getting optional components...", false, StandardArguments);
                x.StandardInput.WriteLine("Dism /Export-Image /SourceImageFile:" + MountPoint.Text + "\\media\\sources\\boot.wim /SourceIndex:1 /DestinationImageFile:" + MountPoint.Text + "\\media\\sources\\boot2.wim");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("Dism /Export-Image /SourceImageFile:" + MountPoint.Text + "\\media\\sources\\boot.wim /SourceIndex:1 /DestinationImageFile:" + MountPoint.Text + "\\media\\sources\\boot2.wim");
                x.StartInfo.RedirectStandardInput = false;
            }
            File.Move(MountPoint.Text + "\\media\\sources\\boot.wim", MountPoint.Text + "\\media\\sources\\boot.old");
            File.Move(MountPoint.Text + "\\media\\sources\\boot2.wim", MountPoint.Text + "\\media\\sources\\boot.wim");
        }

        private void getADKWIMNFO(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo getting optional components...", false, StandardArguments);
                x.StandardInput.WriteLine("DISM /GET-WIMINFO /WIMFILE:" + MountPoint.Text + "\\MEDIA\\SOURCES\\BOOT.WIM /INDEX:1");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("DISM /GET-WIMINFO /WIMFILE:" + MountPoint.Text + "\\MEDIA\\SOURCES\\BOOT.WIM /INDEX:1");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void CreateBootableISO(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo getting optional components...", false, StandardArguments);
                x.StandardInput.WriteLine("pushd C:\\ADK\\Assessment and Deployment Kit\\windows Preinstallation Environment");
                x.StandardInput.WriteLine("echo Y | MakeWinPEMedia.cmd /ISO " + MountPoint.Text + " " + MountPoint.Text + "\\newPE.iso /y");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("pushd C:\\ADK\\Assessment and Deployment Kit\\windows Preinstallation Environment");
                x.StandardInput.WriteLine("echo Y | MakeWinPEMedia.cmd /ISO " + MountPoint.Text + " " + MountPoint.Text + "\\newPE.iso");
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void CreateBootableUsb(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo building bootable usb stick...", false, StandardArguments);
                x.StandardInput.WriteLine("pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\"");
                x.StandardInput.WriteLine("MakeWinPEMedia.cmd /UFD " + MountPoint.Text + " " + usb.Text);
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\"");
                x.StandardInput.WriteLine("MakeWinPEMedia.cmd /UFD " + MountPoint.Text + " " + usb.Text); 
                x.StartInfo.RedirectStandardInput = false;
            }
        }

        private void UnmountAdkIMG(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & echo unmounting ADK...", false, StandardArguments);
                x.StandardInput.WriteLine("dism /unmount-wim /MountDir:" + MountPoint.Text + "\\MOUNT /COMMIT");
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("dism /unmount-wim /MountDir:" + MountPoint.Text + "\\MOUNT /COMMIT");
                x.StartInfo.RedirectStandardInput = false;
            }
        }


        private void StdInChange(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (StdIn.IsChecked == true)
            {
                Global.RedirectStandardInput = true;

                if (PID == 0)
                {
                    Process x = new ReadStdOut().CreateProcess("@echo off & echo off & echo StdIn test", true, StandardArguments);
                    x.StartInfo.RedirectStandardInput = true;
                    Global.ps = x;
                }
                else
                {
                    Process x = Global.ps;
                    x.StandardInput.WriteLine("@echo redirect std input:" + Global.RedirectStandardInput);
                    x.StartInfo.RedirectStandardInput = true;
                }
            }
            else
            {
                Global.RedirectStandardInput = false;

                if (PID == 0)
                {
                    Process x = new ReadStdOut().CreateProcess("@echo off & echo off & echo Standard Input change: ", false, StandardArguments);
                    x = Global.ps;
                    x.StandardInput.WriteLine("@echo redirect std input:" + x.StartInfo.RedirectStandardInput);
                    x.StartInfo.RedirectStandardInput = false;
                }
                else
                {
                    Process x = Global.ps;
                    x.StandardInput.WriteLine("@echo redirect std input:" + x.StartInfo.RedirectStandardInput);
                    x.StartInfo.RedirectStandardInput = false;
                }

            }
        }

        private void redirectCMD(object sender, TouchEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & cls", false, StandardArguments);
                x.StandardInput.WriteLine(cmdInput.Text);
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StandardInput.WriteLine(cmdInput.Text);
            }

        }

        private void runDMC(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("@echo off & cls", false, StandardArguments);
                x.StandardInput.WriteLine(cmdInput.Text);
                Global.ps = x;
            }
            else
            {
                Process x = Global.ps;
                x.StandardInput.WriteLine(cmdInput.Text);
            }
        }
    }
}

