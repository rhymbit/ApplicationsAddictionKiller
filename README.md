# ApplicationsAddictionKiller ![KnifeLogo](https://images.emojiterra.com/google/android-kitkat/64px/1f52a.png)
This is a console application that can be used to help its users curb their addiction of certain apps on their devices.
If you're addicted to some video game or some social media application, this app would work wonders for you 😊. 

## Usage
* Clone this repository on your pc `git clone https://github.com/prateek332/ApplicationAddictionKiller`
* Publishing the application :-
  * If you're using Visual Studio then open the solution => `ApplicationsAddictionKiller\AppsAddictionKiller\AppsAddictionKiller.sln`
    and then publish the application. 
  * If you're using CLI (cmd, powershell, terminal), then go to folder `ApplicationsAddictionKiller\AppsAddictionKiller` and run command
    `dotnet publish`.
* After publishing go to `AppsAddictionKiller\AppsAddictionKiller\bin\Debug\net5.0\publish` folder where all the contents of 
  this folder are you ready to use application. You can use it from this location or copy its contents anywhere you like.
* While in the published app folder use the CLI and type 
  `./AppsAddictionKiller.exe app1 useTime1 cooldownTime1 app2 useTime2 cooldownTime2`, where:-
  * '**app1**' is the name of some application like _msedge_ , _gta5_ etc that runs on your device, '**useTime1**' is the
    amount of time in _minutes_ that this app would be allowed to run and '**cooldownTime1**' is the amount of time in _minutes_
    this app would not be allowed to run after its **useTime1** would've expired.
  * Same applies for the second '**app2**' and its '**useTime2**' and '**cooldownTime2**'
  * Read `Important`section before proceeding, click [here](https://github.com/prateek332/ApplicationsAddictionKiller#important) to visit the section.
  * **Here's a simple example :-**
    `/AppsAddictionKiller.exe msedge 20 10 gta5 30 20`, here '**Microsoft Edge**' would be allowed to run for 20 minutes at a time,
    after which it wouldn't be allowed to run for following 10 minutes. '**gta5**' would be allowed to run for 30 minutes, after which
    you can't play the game for another 20 minutes.
* This application can support monitoring of up to **10 applications and programs.**
* The best use of this application is to use this application from TaskSchedular of the operating system, so it can mointor your
  addictive applications all the time.
 
 ## Logs File Location
The application creates a logs file at location `AppData\Local\AppsAddictionKiller\logs.json`

## Important
* The application names given as command line arguments are the actual process's name, not the names of shortcuts with which
  you might have launched the application.
* Make sure to get the actual name of the process using the task manager or the actual location of the application on your device. Pass
  the correct process names in the CLI.
  
