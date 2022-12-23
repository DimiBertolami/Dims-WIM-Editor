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

namespace DimsISOTweaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string StandardArguments = "cmd /k color 9e & title WIM-Editor & echo off";
        private object val;

        public int ID { get; set; } = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        public void MountISO(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                PID = new ReadStdOut().CreateProcess(" /c powershell -command \"Mount-DiskImage -ImagePath " + ISO.Text + "\"");
            }
            else
            {
                Process x = Process.GetProcessById(PID);
                x.StandardInput.WriteLine(" /c powershell -command \"Mount-DiskImage -ImagePath " + ISO.Text + "\"");
            }
        }

        public void DismountISO(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                PID = new ReadStdOut().CreateProcess("/c powershell.exe -Command \"Dismount-DiskImage -ImagePath " + ISO.Text + "\"");
            }
            else
            {
                Process x = Process.GetProcessById(PID);
                x.StandardInput.WriteLine($" /c powershell -command \"Dismount-DiskImage -ImagePath {ISO.Text}\"");
            }
        }

        public void CopyWIM(object sender, RoutedEventArgs e)
        {
            //the boolean means recursive delete
            if (Directory.Exists(MountPoint.Text)) { Directory.Delete(MountPoint.Text, true); }
            Directory.CreateDirectory(MountPoint.Text);
            Directory.CreateDirectory(MountPoint.Text + "\\Drivers");
            Directory.CreateDirectory(MountPoint.Text + "\\MOUNTDIR");
            Directory.CreateDirectory(MountPoint.Text + "\\BootWIM");
            DriveInfo[] drives = DriveInfo.GetDrives();
            for (int i = 0; i < drives.Count(); i++)
            {
                if (File.Exists(drives[i].Name + "Sources\\boot.wim"))
                {
                    //var FileHandle = File.OpenHandle(MountPoint.Text + "\\BootWIM\\boot.wim");
                    File.Copy(drives[i].Name + "Sources\\boot.wim",
                            MountPoint.Text + "\\BootWIM\\boot.wim", true);
                    FileInfo fileInfo = new FileInfo(MountPoint.Text + "\\BootWIM\boot.wim");
                    if (fileInfo.IsReadOnly) { fileInfo.IsReadOnly = false; };
                }
            }
        }
        public void getWIMInfo(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c DISM.exe /Get-WimInfo /WimFile:" + MountPoint.Text + "\\BootWIM\\boot.wim");
        }
        public void MountWIM(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c DISM /mount-wim /wimfile:" + MountPoint.Text +
                "\\BootWIM\\boot.wim /index:" + Index.Text +
                " /MountDir:" + MountPoint.Text +
                "\\MOUNTDIR");
        }
        public void AddPEDrivers(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /image:" + MountPoint.Text +
                "\\MOUNTDIR /Add-Driver /Driver:" + MountPoint.Text +
                "\\Drivers /recurse");
        }
        void SlipstreamKB(object sender, RoutedEventArgs e)
        {
            // this is how this works:
            // first deploy your default install.wim on a computer and let it perform windows update.
            // Just make sure you write down which KB-Nrs.msu's you install. then download the correct
            // update file from https://www.catalog.update.microsoft.com/home.aspx and slipstream it into
            // your installation source like i did here.. 
            new ReadStdOut().CreateProcess(" /c for /f \"usebackq\" %x in (`dir " +
                MountPoint.Text + "\\*.msu /b`) do wusa " +
                MountPoint.Text + "\\%x /quiet /norestart");
            new ReadStdOut().CreateProcess(" /c echo for /f \"usebackq\" %x in (`dir " +
            MountPoint.Text +
            "\\*.msu /b`) do dism /Image:" +
            MountPoint.Text +
            "\\MOUNTDIR /Add-Package /PackagePath=%x");
        }

        private void UnMountWIM(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /unmount-wim /mountdir:" +
             MountPoint.Text +
             "\\MOUNTDIR /commit");
        }

        private void about(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("how to update windows 10 ISO/Wim (By Dimi Bertolami)\r\n\r\ntopics:\r\n- extra drivers\r\n- slipstream microsoft kb-updates\r\n- extra executables\r\n- how to write a PE-Network script\r\n\r\nfrom dosbox: \r\n\r\ncreate some temporary directories:\r\n(Here i'll download all the hotfixes)\r\nMD C:\\Mount\r\n\r\n:: extra drivers go into this directory. They will be installed recursively\r\nMD C:\\Mount\\Drivers\r\n\r\n:: boot.wim from the windows ISO goes here\r\nMD C:\\Mount\\BootWIM\r\n\r\n:: this is our Mount-Target Directory (in order to mount a wim file this folder has to be empty)\r\nmd C:\\MOUNT\\MOUNTDIR\r\n\r\nTo Mount an ISO with powershell: \r\npowershell -Command \"Mount-DiskImage -ImagePath C:\\Users\\Admin\\Desktop\\dewSystems\\ISO\\Gandalf10PE.ISO\"\r\n\r\ndetect drive letter where iso is mounted and copy wim to bootWIM Folder\r\nFOR /D %x in (A B C D E F G H I J K L M N O P Q R S T U V W X Y Z) do if EXIST %x:\\sources\\boot.wim (copy %x:\\sources\\boot.wim C:\\Mount\\BootWIM)\r\n\r\ncheck what image index to use\r\ndism /Get-WimInfo /WimFile:C:\\Mount\\BootWIM\\boot.wim\r\n\r\nmount wim file into mount-directory\r\ndism /mount-wim /wimfile:C:\\Mount\\BootWIM\\boot.wim /index:1 /MountDir:C:\\Mount\\MOUNTDIR\r\n\r\nrecursively add drivers to your PE (you must mount wim file first): \r\ndism /image:C:\\Mount\\MOUNTDIR /Add-Driver /Driver:D:\\Drivers /recurse\r\n\r\ndownload necessary updates manually\r\nslipstream downloaded windows updates into your solution\r\nDism /Image:C:\\Mount\\MOUNTDIR /Add-Package /PackagePath=kb4456655.msu /LogPath=C:\\mount\\dism.log\r\n\r\n");
        }

        private void addCabs(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess("/c pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\" & dism /Image:" +
                MountPoint.Text +
                "\\MOUNTDIR /Add-Package /PackagePath:\"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\");
            new ReadStdOut().CreateProcess("/c pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\en-us\" & dism /Image:" +
                MountPoint.Text +
                "\\MOUNTDIR /Add-Package /PackagePath:\"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\en-us\\");
        }

        private void UnMountWIMDiscard(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /unmount-wim /mountdir:" +
                MountPoint.Text +
                "\\MOUNTDIR /discard");
        }

        private void CleanUpMountPoints(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /Cleanup-Mountpoints");
        }

        private void SpawnAShell(object sender, RoutedEventArgs e)
        // it's Spawn - A - Shell .. not Spawn as Hell !
        {
            int PID = Global.PID;
            //new PS().Create("echo hello world!");
            var cmdStartInfo = new ProcessStartInfo() { };
            cmdStartInfo.RedirectStandardInput = true;
            cmdStartInfo.CreateNoWindow = false;
            cmdStartInfo.UseShellExecute = false;
            cmdStartInfo.FileName = "cmd.exe";
            cmdStartInfo.Arguments = " /k color 9e";
            cmdStartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            cmdStartInfo.WorkingDirectory = MountPoint.Text;
            var cmdProcess = new Process();
            cmdProcess.Start();

            PID = cmdProcess.Id;
            cmdProcess.StandardInput.WriteLine("echo dit is een test!");
        }

        private void CleanupWim(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /Cleanup-Wim");
        }

        private void getMountedWIMInfo(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /Get-MountedWimInfo");
        }

        private void getPackages(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /image:" + MountPoint.Text + "\\MOUNTDIR /Get-Packages");

        }

        private void CleanupIMG(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c Dism /Image:" + MountPoint.Text + "\\MOUNTDIR /cleanup-image /StartComponentCleanup /ResetBase");
            new ReadStdOut().CreateProcess(" /c Dism /Unmount-Image /MountDir:C:\\test\\offline /Commit");
            new ReadStdOut().CreateProcess(" /c Dism /Export-Image /SourceImageFile:C:\\Images\\install.wim /SourceIndex:1 /DestinationImageFile:C:\\Images\\install_cleaned.wim");
        }

        private void adksetup(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("c:\\ADK")) { return; }
            new ReadStdOut().CreateProcess(" /c Installers\\adksetup.exe /features optionid.deploymentTools /installpath c:\\ADK /Q");
        }

        private void ADKPESetup(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment")) { return; }
            new ReadStdOut().CreateProcess(" /c Installers\\adkwinpesetup.exe /features + /installpath c:\\ADK /Q");
        }


        private void adkMountWIM(object sender, RoutedEventArgs e)
        {
            //dism /mount-wim /wimfile:c:\MOUNT\dimpe\media\sources\boot.wim /index:1 /MountDir:c:\MOUNT\dimpe\mount
            new ReadStdOut().CreateProcess(" /c DISM /mount-wim /wimfile:" + MountPoint.Text + "\\pe%processor_architecture%\\media\\sources\\boot.wim" +
                " /index:" + Index.Text +
                " /MountDir:" + MountPoint.Text +
                "\\MOUNTDIR");
        }

        private void adkCopyPE(object sender, RoutedEventArgs e)
        {
            //copype amd64 C:\WinPE_amd64
            new ReadStdOut().CreateProcess(" /c copype %processor_architecture% C:\\PE%processor_architecture%");
        }

        private void vOS(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c copy c:\\adk\\ValidationOS.wim C:\\PE%processor_architecture%\\media\\sources\\boot.wim /y");
        }

        private void AddCabz(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c for /f \"usebackq\" %x in (`dir d:\\cabs\\*.cab /s /b`) do dism /Image:c:\\PE%processor_architecture%\\mount /Add-Package/PackagePath:\"%x\"");

        }

        private void getADKPackages(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /Image:c:\\PE%processor_architecture%\\mount /get-Packages");
        }

        private void cleanResetBase(object sender, ContextMenuEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c Dism /Image:c:\\PE%processor_architecture%\\mount /cleanup-image /StartComponentCleanup /ResetBase");
        }

        private void UnmountWIMAdk(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /unmount-wim /mountdir:" +
                             MountPoint.Text +
                             "\\MOUNTDIR /commit");
        }

        private void ExportWIMAdk(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c Dism /Export-Image /SourceImageFile:C:\\PE%processor_architecture%\\media\\sources\\boot.wim /SourceIndex:1 /DestinationImageFile:C:\\PE%processor_architecture%\\media\\sources\\boot2.wim");
            File.Delete("C:\\PE%processor_architecture%\\media\\sources\\boot.wim");
            File.Copy("C:\\PE%processor_architecture%\\media\\sources\\boot2.wim", "C:\\PE%processor_architecture%\\media\\sources\\boot.wim");
        }

        private void getADKWIMNFO(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess("DISM /GET-WIMINFO /WIMFILE:C:\\PE%processor_architecture%\\MEDIA\\SOURCES\\BOOT.WIM /INDEX:1");
        }

        private void CreateIso(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /k pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\" & MakeWinPEMedia.cmd /ISO C:\\AMDimPE c:\\users\\admin\\Desktop\\AMDPE64.iso");
        }

        private void CreateBootableUsb(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess("install dotnet setup");
        }

        private void UnmountAdkIMG(object sender, RoutedEventArgs e)
        {
            //
        }

        private void CreateBootableISO(object sender, RoutedEventArgs e)
        {
            //
        }
    }
}

