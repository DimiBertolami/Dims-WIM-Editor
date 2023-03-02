# M$ Boot ISO Editor

In case you have no idea what this is.. maybe some explanation would go a long way. So when I was a little kid the computers had less than a megabyte of ram, and the disk of my first computer (an xt086) with a 10 Mb harddisk. So I spent a fair amount of my childhood time reinstalling that computer from dos 6.22 to windows 3.11 and back.. Some games only worked in dos, and others were small enough to run alongside windows.. Now to reinstall that computer I needed to boot from a floppy disk (all you youngsters out there that didn't have to go through this, one such floppy's was 1.44Mb in size and took about 10 minutes to copy all its data.   
Now fast-forward to Windows 98.. Cdroms became popular but the computer simply didn't know how to boot from cd's. So a clever dude at Microsoft decided to create a raw copy of the floppy (sector by sector) and burn it on a cd. Much to his delight / surprize without too much hassle it would accept te floppy image on the cd.. all they basically needed to do was install a CDRom driver (mscdex) in dos so it could use it like a disk from inside dos. this was the common new way of presenting their users with ever increasing OS Size. but it became old quickly and needed a long overdue update to modern times. They had for each windows version a custom tailored boot loader back then. Few years later we arrive at windows 7 where they came closer and closer to the current situation. Windows 7 era was a giant leap forward technology wise. But online communities were also progressing all on their own. To be more precise the Bart PE community was a pretty large communityBart Lagerwij was a real clever guy from holland he got his hands on a tropical flavor of windows called windows CE for embedded devices. picture Microsoft GSM's with this version of windows. I had one of those and if you fancy your phone rebooting in the middle of a conversation then definately try them, it will be a lot of fun. Back to my topic this phone's OS... They used a virtual disk system that triggered an online revolution in this community.. we could create a file that would act as a totally legit disk to that operating system nowadays we all know vmware and hyperV but back then this was never seen before.. these virtual disk files had the .SDI extention. If you open any install CD/DVD from post windows 7 era you can still find these boot.sdi files inside.. The virtual disk was quickly integrated in this bart PE boot system.. how? ramdisks! you create a virtual disk inside a portion of your ram and the operating system would recognise it as an actual disk. why would they want to do such a silly thing? for one reason mainly because it was faster. alot faster. Normal disk response times are expressed in Milliseconds. memory (even the old  memory) responds in millionths of a second so that's x1000. 
We jump back a bit to the windows XP era.. when Installing windows XP I could use my cd copy on any type of computer, desktop, laptop, different brands they  all installed correctly. but installing from cd also took about 45 minutes and there was a company (norton) that had created a significantly faster method. ghost images. From 45 minutes to less than 20 but it was also an ancient trick called a sector based copy or raw copy from disk to file or the other way around. Only problem they had was that if they created an image of a dell desktop and tried to dump this image onto an acer laptop with different video  card it would simply blue screen. 
Microsoft actually solved this problem in Windows 7. They created another type of virtual disk that would act like the install CD and had the capability of detecting the hardware and assign correct drivers to it so that with Win7 you could create an image of your dell desktop and dump it on your acer and it would work. praise yourself lucky that you didn't have to learn this the hard way like i did.  
Now Windows 10 took all the lessons learned from the past 30 years and solve it for once and for allways. if you open a windows 10 iso you will find the bulk of the data on that disk to be in 2 files. A boot.wim and an install.wim (or .esd). The install.esd typically contains your versions of windows (home, pro, ultimate, enterprise and so on) and will do this in such a way that every needed file is exactly 1 time inside this image to reduce image size. Windows 7 also was the time when OEMs (Original Equipment Manufacturers, like dell, acer) started using secure boot because virusses back then were sometimes real nasty pieces of code. They could remove one or two files and your pc would'n boot anymore. so .EFI files. They are still the same good old bootloaders from way back with the single difference that you needed to sign your bootloader with a certificate at install time. So trusted. When you installed the computer. Signed bootloades and secure boot settings greatly reduced the impact of these virusses. I used the command line for as long as i can remember so as soon as when i realised i would have to do something more than once. I typically would try to do the same task from the command line. Simply because I could then create a script that did this for me but faster and less prone to user errors. 
One other thing I need to mention and then i promise to explain why this app and shed some light on the how.. I started my professional carreer with manual installs of windows. I implemented myself for the company i worked for back then a light touch automated install of windows. this means I would skip the windows installation cd and I booted the computer from bartPE. then I pointed the ntsetup.exe to an answer file on a fileserver. All the basic questions like where do you live, what time is it, what's your name, do you have querty or azery keyboard and so on. the standard cd install forced you to stay near the computer because it would ask these questions at random when it needed to know. I scripted my silent installation in such a way that all the manual actions and prompts were asked all the way in the beginning rather than during the installation. So for me this meant installing a computer took less than 10 Minutes and I created extra time to help other users, close more tickets, solve more problems. LinkedIn started it's uprise at that time and a company contacted me for a similar job. But they sold and supported a product called Altiris Deployment Server. Real expensive software but i learned how the pro's were doing it. To be honest it was not all that different from how I solved the same problem simply by scripting. Altiris needed to boot the computer to a 32bit environment that could run applications and also knew what to do with a network driver for that computer. I simply used bartPE. Altiris booted faster than my system but I was mostly happy to see that I arrived at similar conclusions than the big boys. Altiris was really popular and they were a higly ranked partner of microsoft so Microsoft decided to jump on the deployment server train again like altiris before it was too late. Windows 2000 server already had a decent install server for small companies of let's say max 100 computers. Ris. but alot of the microsoft clients were enterprise level companies and the RIS Server would not be so usefull in these larger environments. SCCM was born. System center Configuration manager. SCCM was invented before microsoft advanced from .WIM files to the compressed .ESD format so if you were so unlucky like me and sometimes only had access to the windows ISO download tool to get a copy of windows you would be out of luck until you figured out how to extract your .WIM from this .ESD file. Only after I had my share (and your share!) of troubles supporting this seemingly undocumented environment. I was able to extract the version of .WIM I needed from this .ESD. One of the reasons I was determined to learn how to develop my own applications was because I could then create a button that would execute all the necessary commands/tasks so I wouldn't have to each time explain how it's done but rather present my clients with an automated solution that removes some of the complexity of these systems.

This is where we finally arrive at why i created this particular app. Windows 7 enterprise deployment strategies heavily relied on another microsoft product called the WAIK (windows automated installation kit, basically a set of tools that when combined offered a greater flexibility than the standard iso/dvd solution. Waik was nice but Windows 10 forced this SDK To evolve into it's final state which is the ADK used today. Windows 10 no longer uses the WAIK But th eAssessment and Deployment kit. My luck finally changed because all the struggles I had in the past finally paid off because this ADK was basically a set of dos commands and scripts. 

so as an example you can try this app by installing a default ADK installation + the required PE Addon, then use it to create a customised windows PE Boot.WIM and/or iso.. on top of that you could install the ValidationOS optional components (these are .CAB files that implement missing features that you could need during your deployment stages like Powershell support, Mass storage driver support, other languages  etc.. They differ from the cabs offered by the ADK but work just fine also. You maximize the available scratchspace and replace the default pe background with your own nice background jpg.. you can also cleanup your mountpoints after completing these tasks, unmount your wim disk and export the image to increase boot performance. Finally push the button to build the iso and test with Qemu. In buttons in my app this would be something like this..
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



