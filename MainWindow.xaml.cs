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

namespace DimsISOTweaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string StandardArguments = "color 9e & title WIM-Editor";
        private object val;

        public int ID { get; set; } = 0;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void SpawnAShell(object sender, RoutedEventArgs e)
        // it's Spawn - A - Shell .. not Spawn as Hell !
        {
            //int PID = Global.PID;
            if (Global.PID == 0)
            {
                Process x = new ReadStdOut().CreateProcess("echo off & @echo if you listen to a unix Shell, you can hear the C! & @echo on", false, StandardArguments);
                Global.ps = x;
            }
            else
            {
                //Process x = Process.GetProcessById(Global.PID);
                Process x = Process.GetProcessById(Global.PID);
                x.StartInfo.RedirectStandardInput = true;
                x.StandardInput.WriteLine("If you listen to a unix shell, you can hear the C!");
                x.StartInfo.RedirectStandardInput = false;
            }
        }


        public void MountISO(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Global.ps = new ReadStdOut().CreateProcess("powershell -command \"Mount-DiskImage -ImagePath " + ISO.Text + "\"", _contentLoaded, StandardArguments);
            }
            else
            {
                var ps = Global.ps;
                var Psi = ps.StartInfo;

                ps.StartInfo.RedirectStandardInput = true;
                ps.StandardInput.WriteLine("powershell -command \"Mount-DiskImage -ImagePath " + ISO.Text + "\"");
                ps.WaitForExit();
                ps.StartInfo.RedirectStandardInput = false;

                //Psi.RedirectStandardInput.ToString();
                //Psi.RedirectStandardInput = true;
                //MessageBox.Show("bool : " + Psi.RedirectStandardInput);

                //MessageBox.Show("redirect STD In:" + x.StartInfo.RedirectStandardInput);
                //x.StartInfo.RedirectStandardInput = true;
                //x.StandardInput.WriteLine("powershell -command \"Mount-DiskImage -ImagePath " + ISO.Text + "\"");
                //x.StartInfo.RedirectStandardInput = false;
                //Process.GetProcessById(PID).StandardInput.WriteLine("powershell -command \"Mount-DiskImage -ImagePath " + ISO.Text + "\"");
            }
        }

        public void DismountISO(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Process ps = new ReadStdOut().CreateProcess("/c powershell.exe -Command \"Dismount-DiskImage -ImagePath " + ISO.Text + "\"", _contentLoaded, StandardArguments);
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
            if (Directory.Exists(MountPoint.Text)) 
            {
                FileInfo fileInfo = new FileInfo(MountPoint.Text + "\\BootWIM\\boot.wim");
                if (fileInfo.IsReadOnly)
                {
                    FileAttributes attr = File.GetAttributes(MountPoint.Text + "\\BootWIM\\boot.wim");
                    string sFile = Convert.ToString(attr);
                    MessageBox.Show(sFile);
                    {
                        FileInfo myfile = new FileInfo(MountPoint.Text + "\\BootWIM\\boot.wim");
                        myfile.IsReadOnly = false;
                        MessageBox.Show("ReadOnly Attribute Removed", "Success");
                    }
                }
                Directory.Delete(MountPoint.Text, true); 
            }
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
                    FileInfo fileInfo = new FileInfo(MountPoint.Text + "\\BootWIM\\boot.wim");
                    if (fileInfo.IsReadOnly) { fileInfo.IsReadOnly = false; };
                }
            }
        }
        public void getWIMInfo(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c DISM.exe /Get-WimInfo /WimFile:" + MountPoint.Text + "\\BootWIM\\boot.wim", _contentLoaded, StandardArguments);
        }
        public void MountWIM(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c DISM /mount-wim /wimfile:" + MountPoint.Text +
                "\\BootWIM\\boot.wim /index:" + Index.Text +
                " /MountDir:" + MountPoint.Text +
                "\\MOUNTDIR", _contentLoaded, StandardArguments);
        }
        public void AddPEDrivers(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /image:" + MountPoint.Text +
                "\\MOUNTDIR /Add-Driver /Driver:" + MountPoint.Text +
                "\\Drivers /recurse", _contentLoaded, StandardArguments);
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
                MountPoint.Text + "\\%x /quiet /norestart", _contentLoaded, StandardArguments);
            new ReadStdOut().CreateProcess(" /c echo for /f \"usebackq\" %x in (`dir " +
            MountPoint.Text +
            "\\*.msu /b`) do dism /Image:" +
            MountPoint.Text +
            "\\MOUNTDIR /Add-Package /PackagePath=%x", _contentLoaded, StandardArguments);
        }

        private void UnMountWIM(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /unmount-wim /mountdir:" +
             MountPoint.Text +
             "\\MOUNTDIR /commit", _contentLoaded, StandardArguments);
        }

        private void about(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("how to update windows 10 ISO/Wim (By Dimi Bertolami)\r\n\r\ntopics:\r\n- extra drivers\r\n- slipstream microsoft kb-updates\r\n- extra executables\r\n- how to write a PE-Network script\r\n\r\nfrom dosbox: \r\n\r\ncreate some temporary directories:\r\n(Here i'll download all the hotfixes)\r\nMD C:\\Mount\r\n\r\n:: extra drivers go into this directory. They will be installed recursively\r\nMD C:\\Mount\\Drivers\r\n\r\n:: boot.wim from the windows ISO goes here\r\nMD C:\\Mount\\BootWIM\r\n\r\n:: this is our Mount-Target Directory (in order to mount a wim file this folder has to be empty)\r\nmd C:\\MOUNT\\MOUNTDIR\r\n\r\nTo Mount an ISO with powershell: \r\npowershell -Command \"Mount-DiskImage -ImagePath C:\\Users\\Admin\\Desktop\\dewSystems\\ISO\\Gandalf10PE.ISO\"\r\n\r\ndetect drive letter where iso is mounted and copy wim to bootWIM Folder\r\nFOR /D %x in (A B C D E F G H I J K L M N O P Q R S T U V W X Y Z) do if EXIST %x:\\sources\\boot.wim (copy %x:\\sources\\boot.wim C:\\Mount\\BootWIM)\r\n\r\ncheck what image index to use\r\ndism /Get-WimInfo /WimFile:C:\\Mount\\BootWIM\\boot.wim\r\n\r\nmount wim file into mount-directory\r\ndism /mount-wim /wimfile:C:\\Mount\\BootWIM\\boot.wim /index:1 /MountDir:C:\\Mount\\MOUNTDIR\r\n\r\nrecursively add drivers to your PE (you must mount wim file first): \r\ndism /image:C:\\Mount\\MOUNTDIR /Add-Driver /Driver:D:\\Drivers /recurse\r\n\r\ndownload necessary updates manually\r\nslipstream downloaded windows updates into your solution\r\nDism /Image:C:\\Mount\\MOUNTDIR /Add-Package /PackagePath=kb4456655.msu /LogPath=C:\\mount\\dism.log\r\n\r\n");
        }

        private void addCabs(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess("/c pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\" & dism /Image:" +
                MountPoint.Text +
                "\\MOUNTDIR /Add-Package /PackagePath:\"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\", _contentLoaded, StandardArguments);
            new ReadStdOut().CreateProcess("/c pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\en-us\" & dism /Image:" +
                MountPoint.Text +
                "\\MOUNTDIR /Add-Package /PackagePath:\"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\\amd64\\WinPE_OCs\\en-us\\", _contentLoaded, StandardArguments);
        }

        private void UnMountWIMDiscard(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /unmount-wim /mountdir:" +
                MountPoint.Text +
                "\\MOUNTDIR /discard", _contentLoaded, StandardArguments);
        }

        private void CleanUpMountPoints(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /Cleanup-Mountpoints", _contentLoaded, StandardArguments);
        }

        private void CleanupWim(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /Cleanup-Wim", _contentLoaded, StandardArguments);
        }

        private void getMountedWIMInfo(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /Get-MountedWimInfo", _contentLoaded, StandardArguments);
        }

        private void getPackages(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /image:" + MountPoint.Text + "\\MOUNTDIR /Get-Packages", _contentLoaded, StandardArguments);

        }

        private void CleanupIMG(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c Dism /Image:" + MountPoint.Text + "\\MOUNTDIR /cleanup-image /StartComponentCleanup /ResetBase", _contentLoaded, StandardArguments);
            new ReadStdOut().CreateProcess(" /c Dism /Unmount-Image /MountDir:C:\\test\\offline /Commit", _contentLoaded, StandardArguments);
            new ReadStdOut().CreateProcess(" /c Dism /Export-Image /SourceImageFile:C:\\Images\\install.wim /SourceIndex:1 /DestinationImageFile:C:\\Images\\install_cleaned.wim", _contentLoaded, StandardArguments);
        }

        private void adksetup(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("c:\\ADK")) { return; }
            new ReadStdOut().CreateProcess(" /c Installers\\adksetup.exe /features optionid.deploymentTools /installpath c:\\ADK /Q", _contentLoaded, StandardArguments);
        }

        private void ADKPESetup(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists("C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment")) { return; }
            new ReadStdOut().CreateProcess(" /c Installers\\adkwinpesetup.exe /features + /installpath c:\\ADK /Q", _contentLoaded, StandardArguments);
        }


        private void adkMountWIM(object sender, RoutedEventArgs e)
        {
            //dism /mount-wim /wimfile:c:\MOUNT\dimpe\media\sources\boot.wim /index:1 /MountDir:c:\MOUNT\dimpe\mount
            new ReadStdOut().CreateProcess(" /c DISM /mount-wim /wimfile:" + MountPoint.Text + "\\media\\sources\\boot.wim" +
                " /index:" + Index.Text +
                " /MountDir:" + MountPoint.Text +
                "\\MOUNTDIR", _contentLoaded, StandardArguments);
        }

        private void adkCopyPE(object sender, RoutedEventArgs e)
        {
            //copype amd64 C:\Mount
            new ReadStdOut().CreateProcess(" /c pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\" & copype %processor_architecture% " + MountPoint.Text, _contentLoaded, StandardArguments);
        }

        private void vOS(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c copy c:\\adk\\ValidationOS.wim C:\\PE%processor_architecture%\\media\\sources\\boot.wim /y", _contentLoaded, StandardArguments);
        }

        private void AddCabz(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c for /f \"usebackq\" %x in (`dir d:\\cabs\\*.cab /s /b`) do dism /Image:c:\\PE%processor_architecture%\\mount /Add-Package/PackagePath:\"%x\"", _contentLoaded, StandardArguments);

        }

        private void getADKPackages(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /Image:c:\\PE%processor_architecture%\\mount /get-Packages", _contentLoaded, StandardArguments);
        }

        private void cleanResetBase(object sender, ContextMenuEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c Dism /Image:c:\\PE%processor_architecture%\\mount /cleanup-image /StartComponentCleanup /ResetBase", _contentLoaded, StandardArguments);
        }

        private void UnmountWIMAdk(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c dism /unmount-wim /mountdir:" +
                             MountPoint.Text +
                             "\\MOUNTDIR /commit", _contentLoaded, StandardArguments);
        }

        private void ExportWIMAdk(object sender, RoutedEventArgs e)
        {
            new ReadStdOut().CreateProcess(" /c Dism /Export-Image /SourceImageFile:" + MountPoint.Text + "\\media\\sources\\boot.wim /SourceIndex:1 /DestinationImageFile:" + MountPoint.Text + "\\media\\sources\\boot2.wim", _contentLoaded, StandardArguments);
            File.Delete(MountPoint.Text + "\\media\\sources\\boot.wim");
            File.Move(MountPoint.Text + "\\media\\sources\\boot2.wim", MountPoint.Text + "\\media\\sources\\boot.wim");
        }

        private void getADKWIMNFO(object sender, RoutedEventArgs e)
        {
            Process ps = new ReadStdOut().CreateProcess("DISM /GET-WIMINFO /WIMFILE:" + MountPoint.Text + "\\MEDIA\\SOURCES\\BOOT.WIM /INDEX:1", _contentLoaded, StandardArguments);
        }

        private void CreateIso(object sender, RoutedEventArgs e)
        {
            Process ps = new ReadStdOut().CreateProcess(" /k pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\" & MakeWinPEMedia.cmd /ISO C:\\AMDimPE c:\\users\\admin\\Desktop\\AMDPE64.iso", _contentLoaded, StandardArguments);
            //if(PID > 0) { }
        }

        private void CreateBootableUsb(object sender, RoutedEventArgs e)
        {
            int PID = Global.PID;
            if (PID == 0)
            {
                Global.ps = new ReadStdOut().CreateProcess(" pushd \"C:\\ADK\\Assessment and Deployment Kit\\Windows Preinstallation Environment\" & MakeWinPEMedia.cmd /UFD C:\\AMDimPE " + usb.Text, true);
            }
            else 
            {
                var x = new ReadStdOut();
                x.CreateCommandOnPid(PID, " /k color 7c & title Wim-Editor & echo off & pushd c:\\AMDimPe\\", true);
            }
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

