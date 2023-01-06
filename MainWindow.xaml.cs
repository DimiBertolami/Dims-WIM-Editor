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
    /// 
    /// E:\sources>dism /Get-WimInfo /WimFile:install.esd
    /// 
    /// 
    /// dism /export-image 
    ///         /SourceImageFile:install.esd 
    ///         /SourceIndex:1 
    ///         /DestinationImageFile:c:\MOUNT\media\sources\install.wim
    ///         /Compress:max 
    ///         /CheckIntegrity
    ///         
    /// dism /export-image 
    ///         /SourceImageFile:install.esd 
    ///         /SourceIndex:2 
    ///         /DestinationImageFile:c:\MOUNT\media\sources\install.wim 
    ///         /Compress:max 
    ///         /CheckIntegrity
    ///         
    /// dism /get-wiminfo /wimfile:C:\mount\media\sources\install.wim /Index:1
    /// 
    /// DISM.exe /Mount-Wim /WimFile:C:\MOUNT\media\sources\install.wim /index:1 /MountDir:C:\MOUNT\mount
    /// 
    /// dism /Image:C:\MOUNT\MOUNT /Add-Package /PackagePath=C:\MOUNT
    /// deze werkt niet!
    /// dism /image:C:\MOUNT\mount /Add-Driver /Driver:c:\MOUNT\Drivers /recurse
    /// 
    /// Dism /Online /Export-Driver /Destination:c:\mount\drivers
    /// 
    /// 
    /// MakeWinPEMedia.cmd /ISO c:\MOUNT c:\MOUNT\dimiPE.iso
    /// 
    /// 
    /// cd \ADK\Assessment and Deployment Kit\Deployment Tools
    /// DandISetEnv.bat
    /// cd..
    /// cd "Windows Preinstallation Environment"
    /// MakeWinPEMedia.cmd /ISO c:\MOUNT c:\MOUNT\dimiPE.iso
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        private string StandardArguments = Global.Args;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void SpawnAShell(object sender, RoutedEventArgs e)
        {
            if (Global.PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("echo off", true);
                x.StandardInput.WriteLine("color 9e & cls");
                Global.ps = x;
            }
            else 
            {
                Process x = Global.ps;
                x.StandardInput.WriteLine("@echo off & color 9e & cls");
            }
        }


        public void MountISO(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps; 
                Global.Args = "powershell -command \"Mount-DiskImage -ImagePath " + ISO.Text + "\"";
                x.StandardInput.WriteLine(Global.Args);
        }

        public void DismountISO(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                Global.Args = "powershell -command \"Dismount-DiskImage -ImagePath " + ISO.Text + "\"";
                x.StandardInput.WriteLine(Global.Args);
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
            Directory.CreateDirectory(MountPoint.Text + "\\MOUNTDIR");
            Directory.CreateDirectory(MountPoint.Text + "\\Drivers");
            Directory.CreateDirectory(MountPoint.Text + "\\BootWIM");
            DriveInfo[] drives = DriveInfo.GetDrives();
            for (int i = 0; i < drives.Count(); i++)
            {
                if (File.Exists(drives[i].Name + "Sources\\boot.wim"))
                {
                    usb.Text = drives[i].Name;
                    File.Copy(usb.Text + "Sources\\boot.wim",
                             MountPoint.Text + "\\BOOTWIM\\boot.wim", true);
                    File.Copy(usb.Text + "Sources\\boot.wim",
                             MountPoint.Text + "\\media\\sources\\boot.wim", true);
                    FileInfo fileInfo = new FileInfo(MountPoint.Text + "\\media\\sources\\boot.wim");
                    fileInfo.IsReadOnly = false;
                        Process x = Global.ps;
                        x.StandardInput.WriteLine("echo boot.wim copied");
                }
            }

        }

        public void getWIMInfo(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                Global.Args = "DISM.exe /Get-WimInfo /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim";
                x.StandardInput.WriteLine(Global.Args);
        }

        public void MountWIM(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                Global.Args = "DISM /mount-wim /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim " +
                              "/index:" + Index.Text + " /MountDir:" + MountPoint.Text + "\\MOUNT\"" ;
                x.StandardInput.WriteLine(Global.Args);
        }

        public void AddPEDrivers(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                Global.Args = "DISM /mount-wim /WimFile:" + MountPoint.Text + "\\media\\sources\\boot.wim " +
                              "/index:" + Index.Text + " /MountDir:" + MountPoint.Text + "\\MOUNT\"";
                x.StandardInput.WriteLine(Global.Args);
        }

        void SlipstreamKB(object sender, RoutedEventArgs e)
        {
            // this is how this works:
            // first deploy your default install.wim on a computer and let it perform windows update.
            // Just make sure you write down which KB-Nrs.msu's you install. then download the correct
            // update file from https://www.catalog.update.microsoft.com/home.aspx and slipstream it into
            // your installation source like i did here.. 
            Global.Args = "echo for /f \"usebackq\" %x in (`dir " + MountPoint.Text + "\\*.msu /b`) do wusa " +
                MountPoint.Text + "\\%x /quiet /norestart";
                Process x = Global.ps;
                x.StandardInput.WriteLine(Global.Args);
                x.StandardInput.WriteLine("echo for /f \"usebackq tokens=*\" %x in (`dir " + MountPoint.Text +
                    "\\*.msu /b`) do dism /Image:" + MountPoint.Text + "\\MOUNT /Add-Package /PackagePath=\"%x\"");
        }

        private void UnMountWIM(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("dism /unmount-wim /mountdir:" + MountPoint.Text + "\\MOUNT /commit");
        }

        private void about(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("how to update windows 10 ISO/Wim (By Dimi Bertolami)\r\n\r\ntopics:\r" +
            //                "\n- extra drivers\r\n- slipstream microsoft kb-updates\r\n- extra exe" +
            //                "cutables\r\n- how to write a PE-Network script\r\n\r\nfrom dosbox: \r" +
            //                "\n\r\ncreate some temporary directories:\r\n(Here i'll download all t" +
            //                "he hotfixes)\r\nMD C:\\Mount\r\n\r\n:: extra drivers go into this dir" +
            //                "ectory. They will be installed recursively\r\nMD C:\\Mount\\Drivers\r" +
            //                "\n\r\n:: boot.wim from the windows ISO goes here\r\nMD C:\\Mount\\Boo" +
            //                "tWIM\r\n\r\n:: this is our Mount-Target Directory (in order to mount " +
            //                "a wim file this folder has to be empty)\r\nmd C:\\MOUNT\\MOUNTDIR\r\n" +
            //                "\r\nTo Mount an ISO with powershell: \r\npowershell -Command \"Mount-" +
            //                "DiskImage -ImagePath C:\\Users\\Admin\\Desktop\\dewSystems\\ISO\\Gand" +
            //                "alf10PE.ISO\"\r\n\r\ndetect drive letter where iso is mounted and cop" +
            //                "y wim to bootWIM Folder\r\nFOR /D %x in (A B C D E F G H I J K L M N " +
            //                "O P Q R S T U V W X Y Z) do if EXIST %x:\\sources\\boot.wim (copy %x:" +
            //                "\\sources\\boot.wim C:\\Mount\\BootWIM)\r\n\r\ncheck what image index" +
            //                " to use\r\ndism /Get-WimInfo /WimFile:C:\\Mount\\BootWIM\\boot.wim\r\" +
            //                "n\r\nmount wim file into mount-directory\r\ndism /mount-wim /wimfile:" +
            //                "C:\\Mount\\BootWIM\\boot.wim /index:1 /MountDir:C:\\Mount\\MOUNTDIR\r" +
            //                "\n\r\nrecursively add drivers to your PE (you must mount wim file fir" +
            //                "st): \r\ndism /image:C:\\Mount\\MOUNTDIR /Add-Driver /Driver:D:\\Driv" +
            //                "ers /recurse\r\n\r\ndownload necessary updates manually\r\nslipstream" +
            //                " downloaded windows updates into your solution\r\nDism /Image:C:\\Mou" +
            //                "nt\\MOUNTDIR /Add-Package /PackagePath=kb4456655.msu /LogPath=C:\\mou" +
            //                "nt\\dism.log\r\n\r\n");
        }

        private void addCabs(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("pushd C:\\ESD\\cabs\\neutral");
                x.StandardInput.WriteLine("dism /Image:" + MountPoint.Text + "\\MOUNT /Add-Package /PackagePath:C:\\ESD\\cabs\\neutral");
                x.StandardInput.WriteLine("cd .. & cd en-us");
                x.StandardInput.WriteLine("dism /Image:" + MountPoint.Text + "\\MOUNT /Add-Package /PackagePath:C:\\ESD\\cabs\\en-us");
        }

        private void AddCabz(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("pushd C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs");
                x.StandardInput.WriteLine("dism /Image:" + 
                                MountPoint.Text + "\\mount " +
                                "/Add-Package " +
                                "/PackagePath:\"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs");
                x.StandardInput.WriteLine("cd en-us");
                x.StandardInput.WriteLine("dism " +
                                "/Image:" + MountPoint.Text + "\\mount " +
                                "/Add-Package " +
                                "/PackagePath:\"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\en-us");
        }


        private void UnMountWIMDiscard(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("dism /unmount-wim /mountdir:" + MountPoint.Text + "\\MOUNT /discard");
        }

        private void CleanUpMountPoints(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("dism /Cleanup-Mountpoints");
        }

        private void CleanupWim(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("dism /Cleanup-Wim");
        }

        private void getMountedWIMInfo(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("dism /Get-MountedWimInfo");
        }

        private void getPackages(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("dism /image:" + MountPoint.Text + "\\MOUNT /Get-Packages");
        }

        private void CleanupIMG(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("Dism /Image:" + MountPoint.Text + "\\MOUNT /cleanup-image /StartComponentCleanup /ResetBase");
            x.StandardInput.WriteLine("Dism /Unmount-Image /MountDir:" + MountPoint.Text + "\\mount /Commit");
            x.StandardInput.WriteLine("Dism /Export-Image " +
                "/SourceImageFile:" + MountPoint.Text + "\\media\\sources\\boot.wim " +
                "/SourceIndex:" + Index.Text + " " +
                "/DestinationImageFile:" + MountPoint.Text + "\\media\\sources\\boot_cleaned.wim");
            x.StandardInput.WriteLine("move " + MountPoint.Text + "\\media\\sources\\boot.wim " + 
                MountPoint.Text + "\\media\\sources\\boot.old");
            x.StandardInput.WriteLine("move " + 
                MountPoint.Text + "\\media\\sources\\boot_cleaned.wim " + 
                MountPoint.Text + "\\media\\sources\\boot.wim");

            if (File.Exists(MountPoint.Text + "\\media\\sources\\install.wim")) 
            {
                x.StandardInput.WriteLine("Dism /Export-Image /SourceImageFile:" + 
                    MountPoint.Text + "\\media\\sources\\install.wim /SourceIndex:" + Index.Text + 
                    " /DestinationImageFile:" + MountPoint.Text + "\\BootWIM\\install_cleaned.wim");
            }
        }

        private void adksetup(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("c:\\ADK")) { return; }
                Process x = Global.ps;
                x.StandardInput.WriteLine("Installers\\adksetup.exe /features optionid.deploymentTools /installpath c:\\ADK /Q");
                x.StandardInput.WriteLine("Windows Assessment and Deployment Kit Installed!");
        }

        private void ADKPESetup(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment")) { return; }
                Process x = Global.ps;
                x.StandardInput.WriteLine("Installers\\adkwinpesetup.exe /features + /installpath c:\\ADK /Q");
                x.StandardInput.WriteLine("PE Addon for Windows Assessment and Deployment Kit Installed!");
        }


        private void adkMountWIM(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("DISM /mount-wim /wimfile:" + MountPoint.Text + "\\media\\sources\\boot.wim" +
                    " /index:" + Index.Text + " /MountDir:" + MountPoint.Text + "\\MOUNT");
        }

        private void adkCopyPE(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine("setlocal ENABLEEXTENSIONS ENABLEDELAYEDEXPANSION & " +
                "pushd \"C:\\ADK\\Assessment and Deployment Kit\\Deployment Tools\" & " +
                "call DandISetEnv.bat");
            x.StandardInput.WriteLine("cd .. & " +
                "cd Windows Preinstallation Environment & " +
                "copype %processor_architecture% " + MountPoint.Text);
        }

        private void vOS(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("copy c:\\Installers\\ValidationOS.wim " + MountPoint.Text + "\\media\\sources\\boot.wim /y");
        }

        private void getADKPackages(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("dism /Image:" + MountPoint.Text + "\\mount /get-Packages");
        }

        private void cleanResetBase(object sender, ContextMenuEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("Dism /Image:c:\\PE%processor_architecture%\\mount /cleanup-image /StartComponentCleanup /ResetBase");
        }

        private void UnmountWIMAdk(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("dism /unmount-wim /mountdir:" +
                             MountPoint.Text +
                             "\\MOUNT /commit");
        }

        private void ExportWIMAdk(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("Dism /Export-Image " +
                    "/SourceImageFile:" + MountPoint.Text + "\\media\\sources\\boot.wim " +
                    "/SourceIndex:" + Index.Text + " " +
                    "/DestinationImageFile:" + MountPoint.Text + "\\media\\sources\\boot2.wim");
                File.Move(MountPoint.Text + "\\media\\sources\\boot.wim", 
                    MountPoint.Text + "\\media\\sources\\boot.old");
                File.Move(MountPoint.Text + "\\media\\sources\\boot2.wim", 
                    MountPoint.Text + "\\media\\sources\\boot.wim");
        }

        private void getADKWIMNFO(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("DISM " +
                    "/GET-WIMINFO " +
                    "/WIMFILE:" + MountPoint.Text + "\\MEDIA\\SOURCES\\BOOT.WIM " +
                    "/INDEX:" + Index.Text);
        }

        private void CreateBootableISO(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("pushd C:\\ADK\\Assessment and Deployment Kit\\windows Preinstallation Environment");
                x.StandardInput.WriteLine("echo Y | MakeWinPEMedia.cmd /ISO " + MountPoint.Text + " " + MountPoint.Text + "\\newPE.iso");
        }

        private void CreateBootableUsb(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\"");
                x.StandardInput.WriteLine("MakeWinPEMedia.cmd /UFD " + MountPoint.Text + " " + usb.Text); 
        }

        private void UnmountAdkIMG(object sender, RoutedEventArgs e)
        {
                Process x = Global.ps;
                x.StandardInput.WriteLine("dism /unmount-wim /MountDir:" + MountPoint.Text + "\\MOUNT /COMMIT");
        }


        private void StdInChange(object sender, RoutedEventArgs e)
        {
            if (StdIn.IsChecked == true)
            {
                if (Global.PID == 0)
                {
                    Process x1 = new ReadStdOut().CreateProcess("echo off & color 9e & cls", true);
                    Global.ps = x1;
                }
                else
                {
                    Process x1 = Global.ps;
                    x1.StandardInput.WriteLine("echo.");
                }
            }
            else
            {
                if(Global.PID == 0)
                {
                    Process x2 = new ReadStdOut().CreateProcess("echo off & echo has Standard Input changed?", false);
                }
                else
                {
                    Process x2 = Global.ps;
                }
            }
        }

        private void RunDMC(object sender, RoutedEventArgs e)
        {
            Process x = Global.ps;
            x.StandardInput.WriteLine(CmdInput.Text + ">nul");
        }
    }
}

