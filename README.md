# LogitechLCDFFXI
A FFXI applet for the Logitech GamePanel  
  
To us in it's current state:  
1. Copy the contents of the Windower4 to your Windower4 folder.  
2. Build and run the LCD applet through Visual Studio (I use 2013 Community)  
3. Log into FFXI.  
4. Type "//lua load LogitechGamePanel" into FFXI's chat.  
5. Press "Start Server" on the LCD Applet's window.  
6. Type "//lcd c" in FFXI's chat.  
7. Press the leftmost LCD button to open the first tab (player info) or the second button to open the second tab (location info).  
  
A work-in-progress.  
Final goal is to create an application that connects to a Windower4 LUA script to show information from FFXI on the Logitech GamePanel.  
  
Only monochrome support is currently planned, but colour LCD may come in the future.  
# Credits
Experience Code borrowed from barfiller  
Zone IDs from [Darkstar Project Wiki](https://wiki.dspt.info/index.php/Zone_IDs)  
LogitechEnginesWrapper.dll from Offical Logitech C++ SDK, converted to C# using LogitechLCD.cs  
