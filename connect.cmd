:: create a share on a computer on the network and have the Pestartup.cmd map a connection to it.
:: create remoteuser with password remotePASS

net user remoteuser remotePASS /add
net localgroup administrators remoteuser /add
net use \\192.168.50.120\winsrc remotePASS /u:192.168.50.120\remoteUser

