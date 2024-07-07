using System;
using System.Collections.Generic;
using System.IO;
using Oxide.Core;
using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("PermsInfo", "TheProfessor", "0.5.1")]
    [Description("A plugin to show and backup permissions for groups and users, including group membership.")]

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
                SendReply(player, Lang("NotAuthorized", player.UserIDString));
                return;
            }

            ShowPermissions(true);
        }

        [ConsoleCommand("permsshow")]
        private void ShowPermissionsConsoleCommand(ConsoleSystem.Arg arg)
        {
            if (!arg.IsAdmin)
            {
                arg.ReplyWith(Lang("NotAuthorized", arg.Connection?.userid.ToString()));
                return;
            }

            ShowPermissions(true);
        }

        private void ShowPermissions(bool showInConsole)
        {
            Puts(Lang("ShowPermissions_Group"));

            var groups = permission.GetGroups();
            foreach (var group in groups)
            {
                Puts(string.Format(Lang("ShowPermissions_GroupItem"), group));
                var groupPermissions = permission.GetGroupPermissions(group);
                foreach (var perm in groupPermissions)
                {
                    Puts(string.Format(Lang("ShowPermissions_PermissionItem"), perm));
                }
                var groupMembers = permission.GetUsersInGroup(group);
                foreach (var member in groupMembers)
                {
                    string userName = covalence.Players.FindPlayerById(member)?.Name ?? member;
                    Puts(showInConsole ? $"    Member: {member} ({userName})" : $"    Member: {member}");
                }
            }

            Puts(Lang("ShowPermissions_User"));

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
                Puts(showInConsole ? string.Format(Lang("ShowPermissions_UserItem"), user, userName) : string.Format(Lang("ShowPermissions_UserItem"), user, user));
                var userPermissions = permission.GetUserPermissions(user);
                foreach (var perm in userPermissions)
                {
                    Puts(string.Format(Lang("ShowPermissions_UserPermissionItem"), perm));
                }
            }
        }

        [ChatCommand("permsbackup")]
        private void BackupPermissionsChatCommand(BasePlayer player, string command, string[] args)
        {
            if (!player.IsAdmin)
            {
                SendReply(player, Lang("NotAuthorized", player.UserIDString));
                return;
            }

            BackupPermissions(player);
        }

        [ConsoleCommand("permsbackup")]
        private void BackupPermissionsConsoleCommand(ConsoleSystem.Arg arg)
        {
            if (!arg.IsAdmin)
            {
                arg.ReplyWith(Lang("NotAuthorized", arg.Connection?.userid.ToString()));
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
                var groupMembers = permission.GetUsersInGroup(group);
                foreach (var member in groupMembers)
                {
                    // Extracting only the SteamID from the member string and using it in the command
                    // Assuming 'member' is in the format "steamID (username)" as shown in your examples.
                    var steamID = member.Split(' ')[0]; // This splits the string and takes the first part before any space
                    backupCommands.Add($"o.group add {steamID} {group}");
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
                Puts(cmd);  // Only outputting to console for visibility
            }

            var message = string.Format(Lang("BackupCompleted"), backupFilePath);
            if (player != null)
            {
                SendReply(player, message);
            }
            else
            {
                Puts(message);
            }
        }

        private string Lang(string key, string userId = null) => lang.GetMessage(key, this, userId);

        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["NotAuthorized"] = "You are not authorized to use this command.",
                ["ShowPermissions_Group"] = "Permissions for groups:",
                ["ShowPermissions_GroupItem"] = "Group: {0}",
                ["ShowPermissions_PermissionItem"] = "    {0}",
                ["ShowPermissions_User"] = "Permissions for users:",
                ["ShowPermissions_UserItem"] = "User: {0} ({1})",
                ["ShowPermissions_UserPermissionItem"] = "    {0}",
                ["BackupCompleted"] = "Backup completed. Commands written to {0}"
            }, this);
        }
    }
}
