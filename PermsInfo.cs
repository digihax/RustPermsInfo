using System;
using System.Collections.Generic;
using System.IO;
using Oxide.Core;
using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("PermsInfo", "TheProfessor", "0.5.0")]
    [Description("A plugin to show and backup permissions for groups and users.")]

    public class PermsInfo : RustPlugin
    {
        void OnServerInitialized()
        {
            Puts("Running permsbackup on server initialization...");
            BackupPermissions();
        }

        [ChatCommand("permsshow")]
        private void ShowPermissionsChatCommand(BasePlayer player, string command, string[] args)
        {
            if (!player.IsAdmin)
            {
                SendReply(player, "You are not authorized to use this command.");
                return;
            }

            ShowPermissions();
        }

        [ConsoleCommand("permsshow")]
        private void ShowPermissionsConsoleCommand(ConsoleSystem.Arg arg)
        {
            if (!arg.IsAdmin)
            {
                arg.ReplyWith("You are not authorized to use this command.");
                return;
            }

            ShowPermissions();
        }

        private void ShowPermissions()
        {
            Puts("Permissions for groups:");

            var groups = permission.GetGroups();
            foreach (var group in groups)
            {
                Puts($"Group: {group}");
                var groupPermissions = permission.GetGroupPermissions(group);
                foreach (var perm in groupPermissions)
                {
                    Puts($"    {perm}");
                }
            }

            Puts("Permissions for users:");

            var users = new HashSet<string>();
            var permissions = permission.GetPermissions();
            foreach (var perm in permissions)
            {
                var permUsers = permission.GetPermissionUsers(perm);
                foreach (var user in permUsers)
                {
                    users.Add(user);
                }
            }

            foreach (var user in users)
            {
                string userName = covalence.Players.FindPlayerById(user)?.Name ?? user;
                Puts($"User: {user} ({userName})");
                var userPermissions = permission.GetUserPermissions(user);
                foreach (var perm in userPermissions)
                {
                    Puts($"    {perm}");
                }
            }
        }

        [ChatCommand("permsbackup")]
        private void BackupPermissionsChatCommand(BasePlayer player, string command, string[] args)
        {
            if (!player.IsAdmin)
            {
                SendReply(player, "You are not authorized to use this command.");
                return;
            }

            BackupPermissions(player);
        }

        [ConsoleCommand("permsbackup")]
        private void BackupPermissionsConsoleCommand(ConsoleSystem.Arg arg)
        {
            if (!arg.IsAdmin)
            {
                arg.ReplyWith("You are not authorized to use this command.");
                return;
            }

            BackupPermissions();
        }

        private void BackupPermissions(BasePlayer player = null)
        {
            var backupCommands = new List<string>();

            var groups = permission.GetGroups();
            foreach (var group in groups)
            {
                var groupPermissions = permission.GetGroupPermissions(group);
                foreach (var perm in groupPermissions)
                {
                    backupCommands.Add($"o.grant group {group} {perm}");
                }
            }

            var users = new HashSet<string>();
            var permissions = permission.GetPermissions();
            foreach (var perm in permissions)
            {
                var permUsers = permission.GetPermissionUsers(perm);
                foreach (var user in permUsers)
                {
                    users.Add(user);
                }
            }

            foreach (var user in users)
            {
                var userPermissions = permission.GetUserPermissions(user);
                foreach (var perm in userPermissions)
                {
                    backupCommands.Add($"o.grant user {user} {perm}");
                }
            }

            var backupFilePath = Path.Combine(Interface.Oxide.DataDirectory, "permsbackup.txt");
            File.WriteAllLines(backupFilePath, backupCommands);

            foreach (var cmd in backupCommands)
            {
                Puts(cmd);
            }

            if (player != null)
            {
                SendReply(player, $"Backup completed. Commands written to {backupFilePath}");
            }
            else
            {
                Puts($"Backup completed. Commands written to {backupFilePath}");
            }
        }
    }
}
