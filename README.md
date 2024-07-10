# SCP-TTT

Not finished! (Proudly written entirely within 2 nights between 12am and 3am, obviously bad code)

If you want to contribute, please do because I do not have the time to finish this plugin! Feel free to also fork it! (Small credit towards me would be appreciated because I'm an egotistical maniac!)

An Exiled 9 plugin that aims to port TTT to SCP Sl

Feel free to check out another gamemode, [Desert Bus](https://github.com/morgana-x/ScpSL-DesertBus), as well!

## Current issues:
+ Players occasionally crash (Either due to the odd 50 items on surface or due to spamming hints every second, or the 30 npcs when testing, Or northwood are bad programmers, (but more likely I'm the bad programer)
+ Health State (Healthy, Injured, Etc) isn't updated properlly due to the config dictionary being unordered, need to make local copy of dictionary and sort by keys before looping through them

## Todo:
+ Corpse info system
+ Class loadouts for detective
+ clean the bad code up!
+ Better weapon spawning system, (Right now just spawns stuff in specific places on surface)
+ ~~Switch "Map" to Light containment after above is done~~ Make a map system where it chooses a random zone for the map?
+ Disable Light Containment Decontamination?

## Features:
+ Basic Round logic (Round Prep) (Round) (Checks for Innocent, Traitor wins and stalemate) (Round End / Win screen) (Restart)
+ Hud System (Show role, time left) (Works with spectators)
+ Basic Role Allocation ( (Mostly) Faithfully recreated the original Gmod function for this)
