# Dims-WIM-Editor

quick dotnet WPF gui app

> This is the folder structure I used.. you do what you want offcourse..
![image](https://user-images.githubusercontent.com/77209365/208245447-a03cd392-b8e0-44aa-88bf-907faf1bac00.png)

> in case you want to see only the dos commands I initially used:
https://github.com/DimiBertolami/Dims-WIM-Editor/blob/main/doscmds.txt

Windows ISO Editor can: 
  - silent download + install windows Assesment and deployment kit
  - silent download + install Win ADK PE Addon
  - Mount ISO
  - Copy WIM to temp directory
  - Mount Wim
  - integrate drivers
  - slipstream updates in iso
  - Cleanup Corrupted WIM Mountpoints
  - Cleanup WIM
  - Open up a shell prompt (*)
  
  #### TODO: When Shell is opened, output of buttons should be redirected to shell instead of messagebox!
  
> A More recent screenshot.. alot has changed today!
![image](https://user-images.githubusercontent.com/77209365/208691466-d27e5b84-a48a-4d25-b9c0-099f2ce2f0d4.png)

  
  
> Here's a little dosbox help: <br />

> create some temp working directories:<br />
> (Here i'll download all the hotfixes)<br />
```MD C:\Mount```

> just drop all your extra drivers into this directory. They will be installed recursively<br />
```MD C:\Mount\Drivers```

> boot.wim from the windows ISO will go here<br />
```MD C:\Mount\BootWIM```

> this is our Mount-Target Directory (in order to mount a wim file this folder has to be empty)<br />
```md C:\MOUNT\MOUNTDIR```

> To Mount an ISO with powershell (This is still a dos command):<br />
```powershell -Command "Mount-DiskImage -ImagePath C:\Users\Admin\Desktop\dewSystems\ISO\Gandalf10PE.ISO"```

> detect drive letter where iso is mounted and copy wim to working directory<br />
```FOR /D %x in (A B C D E F G H I J K L M N O P Q R S T U V W X Y Z) do if EXIST %x:\sources\boot.wim (copy %x:\sources\boot.wim C:\Mount\BootWIM)```

> check the image index number you would like to use<br />
```dism /Get-WimInfo /WimFile:C:\Mount\BootWIM\boot.wim```

> mount wim file<br />
```dism /mount-wim /wimfile:C:\Mount\BootWIM\boot.wim /index:1 /MountDir:C:\Mount\MOUNTDIR```

> recursively intergrate drivers to your PE (you must mount wim file first): <br />
```dism /image:C:\Mount\MOUNTDIR /Add-Driver /Driver:D:\Drivers /recurse```

> For this little section you should download the necessary updates manually.. so how to do this?<br />
> First deploy your base WIM version to a testcomputer.<br />
> Once up and running, install windows updates<br />
> after updates are done run this cmd <br />
```SystemInfo ```

> and write down which updates (hotfixes) were installed<br />
```Hotfix(s):                 17 Hotfix(s) Installed.
                           [01]: KB5020881
                           [02]: KB5017262
                           [03]: KB4562830
                           [04]: KB5003791
                           [05]: KB5011048
                           [06]: KB5012170
                           [07]: KB5021233
```

Now download these MSU update files manually from https://www.catalog.update.microsoft.com/home.aspx <br />and then slipstream them into your mounted boot.wim

```
for /f "usebackq" %x in (`dir *.msu /b`) do Dism /Image:C:\Mount\MOUNTDIR /Add-Package /PackagePath="%x"
```


