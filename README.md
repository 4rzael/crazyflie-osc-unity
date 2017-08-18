# crazyflie-osc-unity

A Unity client for my crazyflie-osc server.

## How to run it ?

An example scene is present. (actually, two, but the sphere curieuses may be more messy, even though more recent)

* Open it with Unity.

* Select the object OscManager and set your Osc server address / ports within the "drones" and "drones_meta" server. (also put this computer ip in the local server field)

* Select the object LpsManager and set your LPS nodes positions.

* Select the object DronesManager and set your drones radio URIs.

* Run the game (in editor mode)

* When the console prints "SYSTEM OK", the system is ready (or just wait until the drone seems to be at the correct position)

* Without quitting the game, go in scene view

* Click on the button "OSC - Start sync" in the drone inspector

* Move your drone in the unity editor

* Enjoy

## TODO

* Clean the code

* Add documentation

* Make it a Unity package
