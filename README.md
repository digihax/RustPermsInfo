# RustPermsInfo

This mod written by Digihax aka The Professor, admin of AZ Casual, a small and friendly non-kos rust server, up and running for 5+ years

This is a mod for Rust that shows perms from the console, and backs up on each startup to a file for easy restore.

This mod adds two console commands:

permsshow - Shows the current permissions
permsbackup - creates a backup file in oxide\data to allow easy recreation of permissions based on the mods you have installed.

Copy/paste the .txt file output into a console to recreate the permissions.

Sample permsbackup.txt:
o.grant group default backpacks.use
o.grant group default backpacks.gui
o.grant group default quicksmelt.use
o.grant group default minicopterlock.usekeylock
o.grant group default minicopterlock.usecodelock
o.grant group DiscordUsers spawnheli.minicopter.spawn
o.grant group DiscordUsers spawnheli.minicopter.fetch
o.grant group DiscordUsers spawnheli.minicopter.despawn
o.grant group DiscordUsers skinbox.use

