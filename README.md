# Dims-WIM-Editor

so as an axample you can try creating a default ADK Pe, and install the ValidationOS optional components.. They work just fine also. Then you enlargen the scratchspace to its max and replace the default pd background.. you can also cleanup after indtallation, unmount and export the image to increase boot performance. Push the button to build the iso and test with Qemu. In buttons this would be something like this..
![image](https://user-images.githubusercontent.com/77209365/216770974-eddc731e-2717-4879-9d66-7aafd1ed8c22.png)


![image](https://user-images.githubusercontent.com/77209365/216769957-c36c4448-4d93-4a8f-85e0-babd6e5ed7ce.png)


And not to toot my own horn but this is how my cuwtom PE comes out from just 10 buttons..
![image](https://user-images.githubusercontent.com/77209365/216771611-8df41f69-7b12-4533-8693-6d7e4fb8210d.png)



![image](https://user-images.githubusercontent.com/77209365/216536611-120c3b45-de29-433e-86ff-2692a1a79787.png)


Here's a typical set of tasks/steps that you would need to do to create a bootable PE disk, this app is also designed to quickly upgrade this simple boot disk to become a custom windows installation disk.
![image](https://user-images.githubusercontent.com/77209365/213132445-6be93add-6bf7-4726-8916-22b2bb2dcc1b.png)


> This is the folder structure I used.. you do what you want offcourse..
![image](https://user-images.githubusercontent.com/77209365/208245447-a03cd392-b8e0-44aa-88bf-907faf1bac00.png)


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
  
> A More recent screenshot.. alot has changed today!
![image](https://user-images.githubusercontent.com/77209365/213092160-7a021c7c-d36d-44d1-a342-464efdba2de9.png)

![image](https://user-images.githubusercontent.com/77209365/210468600-f03c3f0c-3ec7-4757-b77b-ea43baa5bdfb.png)



  
  
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

Now download these MSU update files manually from https://www.catalog.update.microsoft.com/home.aspx <br />and then slipstream them into your mounted install.wim
since dism accepts a packagepath as a folder also this command is the simpler version of the one below.. i just leave it in there for reference..
```Dism /Image:C:\Mount\MOUNTDIR /Add-Package /PackagePath=C:\Mount```

```
for /f "usebackq" %x in (`dir *.msu /b`) do Dism /Image:C:\Mount\MOUNTDIR /Add-Package /PackagePath="%x"
```



