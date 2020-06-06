# PTZPadController Application for controlling cameras
## Objectives & Requirements
The objectives of this project are:
  - To develop software that will allow the Streaming operator to safely control the cameras using a joystick.
  - Tell the speaker which camera is OnAir
## Success Measurement Criteria
As a minimum, the program must be able to control the position and zoom of the cameras. For other settings, Gain, Brightness, White Balance, etc., the operator will go through the camera's web interface. The program should also not be connected to OBS or StreamDeck.
Important limitations
This program will be developed for the Windows OS, and only for the Datavideo PTC-140 cameras and the ATEM Mini Pro video mixer. However we will be careful to make technological choices that do not close the door to the Mac world.

## Requirements
Design a desktop application that allows the cameras to be controlled using a joystick from the EEA Streaming computer. The application should also be able to turn the camera's LED to green when it is in Preview of the ATEM Mini and to red when it is in Program.
The application will be used live, so it must be extremely stable and manage automatic disconnection and reconnection with the different peripherals without having to reboot the application.
