# RustPermsInfo

## Maintenance status

**Archived — November 2026.** No active maintenance or current compatibility
support is planned. The source and commit history remain available under the
existing GPL-3.0 license.

This Oxide/uMod permissions plugin is retained for historical reference. Current
server/plugin compatibility and restore behavior have not been revalidated.

Review it in a disposable test environment before reuse. This archive is part of
its development history, not a recommendation for new production deployments.

RustPermsInfo is an Oxide/uMod plugin for Rust server admins. It prints current permissions and writes restore-friendly permission backup commands to files in the Oxide data directory.

The plugin is useful when you want a quick snapshot of group permissions, user permissions, and group membership before changing plugins, migrating servers, or troubleshooting access.

## What It Does

- Runs a permissions backup when the server initializes
- Adds admin-only commands for showing permissions
- Adds admin-only commands for creating a permission backup on demand
- Writes a timestamped backup file
- Updates `permsbackup.txt` as the latest backup

## Commands

### Chat Commands

```text
/permsshow
/permsbackup
```

Only admin players can use these commands.

### Console Commands

```text
permsshow
permsbackup
```

Only admin console contexts can use these commands.

## Backup Output

Backups are written under the Oxide data directory:

```text
oxide\data\permsbackup.txt
oxide\data\permsbackup-YYYYMMDD-HHMMSS.txt
```

The timestamped file preserves a point-in-time copy. `permsbackup.txt` is updated as the latest copy for convenience.

Backup lines are written as console commands, for example:

```text
o.grant group default backpacks.use
o.usergroup add 76561198000000000 default
o.grant user 76561198000000000 someplugin.use
```

## Installing

1. Copy `PermsInfo.cs` into your server's `oxide\plugins` folder.
2. Let Oxide compile/load the plugin.
3. Check the server console for the startup backup message.

## Restoring Permissions

1. Review the generated backup file before using it.
2. Copy the restore commands into the server console.
3. Restart or reload affected plugins if needed.
4. Run `permsshow` or `permsbackup` again to verify the expected state.

## Notes And Cautions

- Review generated commands before pasting them into a production console.
- The plugin captures the permission state that Oxide reports at runtime. If another plugin changes permissions after startup, run `permsbackup` again.
- Keep timestamped backups if you are about to make large permission changes.
