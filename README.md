# Dims-WIM-Editor

quick dotnet WPF gui app
Windows ISO Editor can: 
  - silent download + install windows Assesment and deployment kit
  - silent download + install Win ADK PE Addon
  - Mount ISO
  - Copy WIM to temp directory
  - Mount Wim
  - integrate drivers
  - slipstream updates in iso
  
  ![image](https://user-images.githubusercontent.com/77209365/208239091-c3214384-60a2-4618-b205-19fa504aa0a8.png)
  
  
  
#Here's a little dosbox help: 
create some temp working directories:
(Here i'll download all the hotfixes)
```MD C:\Mount```

:: just drop all your extra drivers into this directory. They will be installed recursively
```MD C:\Mount\Drivers```

:: boot.wim from the windows ISO will go here
```MD C:\Mount\BootWIM```

:: this is our Mount-Target Directory (in order to mount a wim file this folder has to be empty)
```md C:\MOUNT\MOUNTDIR```

To Mount an ISO with powershell (This is still a dos command): 
```powershell -Command "Mount-DiskImage -ImagePath C:\Users\Admin\Desktop\dewSystems\ISO\Gandalf10PE.ISO"```

detect drive letter where iso is mounted and copy wim to working directory
```FOR /D %x in (A B C D E F G H I J K L M N O P Q R S T U V W X Y Z) do if EXIST %x:\sources\boot.wim (copy %x:\sources\boot.wim C:\Mount\BootWIM)```

check the image index number you would like to use
```dism /Get-WimInfo /WimFile:C:\Mount\BootWIM\boot.wim```

mount wim file
```dism /mount-wim /wimfile:C:\Mount\BootWIM\boot.wim /index:1 /MountDir:C:\Mount\MOUNTDIR```

recursively intergrate drivers to your PE (you must mount wim file first): 
```dism /image:C:\Mount\MOUNTDIR /Add-Driver /Driver:D:\Drivers /recurse```

For this little section you should download the necessary updates manually.. so how to do this?
First deploy your base WIM version to a testcomputer.
Once up and running, install windows updates
after updates are done run this cmd 
```"SystemInfo" ```

and write down which updates (hotfixes) were installed
Hotfix(s):                 17 Hotfix(s) Installed.
                           [01]: KB5020881
                           [02]: KB5017262
                           [03]: KB4562830
                           [04]: KB5003791
                           [05]: KB5011048
                           [06]: KB5012170
                           [07]: KB5021233
Now download these MSU Files manually from https://www.catalog.update.microsoft.com/home.aspx

slipstream windows updates into your mountedWIM
```for /f "usebackq" %x in (`cmd /c dir *.msu /b`) do echo %x```
```Dism /Image:C:\Mount\MOUNTDIR /Add-Package /PackagePath=kb4456655.msu /LogPath=C:\mount\dism.log```


