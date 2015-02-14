###HOWTO: Install Windows 10 for Phones on Non-supported devices (and other hacks)

Instructions and original code posted [here](http://forum.xda-developers.com/windows-phone-8/development/howto-install-windows-10-phones-t3030105).

Research by [RustyGrom](http://forum.xda-developers.com/member.php?s=c31817caab1dc5d07fda3bf29ce38111&u=298227)

You can download the necessary binaries [here](bin/Release) if you don't want to build the project yourself.

1.	For the Lumia 1520 (and other phones?) reset your phone using the [Windows Phone Recovery Tool](http://www.microsoft.com/en-us/mobile/support/faq/?action=singleTopic&topic=FA142987). I recommend not logging in and setting everything up yet and just waiting til the end and doing a reset and letting it restore after you're on Windows 10. This may not be required for your device but most likely is due to the space issue cited by Microsoft. You can see your available space in the About menu of the Windows Insiders app. Before resetting mine was ~200mb and after it was ~600mb. You can try without resetting but may get an error after it tries to download and install the update. 

2.	Install the Windows Insider app on your phone (you will need to have a store account configured)

3.	Open the insiders app and go to the about options in the app bar and screenshot/make notes/email the settings to yourself. You may need this later.

4.	Download and extract the WPInsidersHacks app (attached to this post) to your computer

5.	Start the WPInsiderHacks app on pc, approve any firewall requests

6.	Connect to the same WiFi that your PC is on. Edit the settings for the WiFi connection to turn on the Proxy and set it to use the PC's IP address and enter 8877 for the port 

7.	Open Internet Explorer on your phone and navigate to http://[pc.ip.add.ress]:8877 where [pc.ip.add.ress] is the IP address of the PC running the WPInsidersHacks app

8.	Tap on the link at the bottom to the fiddler root certificate

9.	When prompted click open and then install to trust the cert and ok at the confirmation

10.	Run the Windows Insider app and tap get preview builds

11.	Tap on which custom action you'd like to perform. In my case I hit "Set to ATT Lumia 635" and click the arrow at the bottom

12.	Accept the agreement and click the check mark at the bottom. the app will close

13.	Now clear your proxy settings and run the insider app again

14.	Tap get preview builds and login with your Microsoft account

15.	Select the fast branch (if you're reading this of course you want the fast branch) and click the arrow at the bottom

16.	Go into the phone settings and check for updates. With any luck you should have Windows 10 there for you to install. On my AT&T 1520 it had to do 8.1 first but it kept going to 10 after 8.1 installed (had to check for updates again)

17.	You'll probably want to go into the settings and do another reset and then log into the phone and let it restore your previous data
