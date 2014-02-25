using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceCode.ServiceBroker.RolesManagement
{
    public static class Constants
    {
        public static class ErrorText
        {
            public const string RoleNotExist = "Invalid Role Name - Role not found.";
            public const string RoleTypeNotSupported = "Could not determine role item type. '{0}' is unknown or not supported.";
        }
        public static class Methods
        {
            public const string ListRoleItems = "ListRoleItems";
            public const string ListRoles = "ListRoles";
            public const string AddRole = "AddRole";
            public const string DeleteRole = "DeleteRole";
            public const string AddRoleItem = "AddRoleItem";
            public const string DeleteRoleItem = "DeleteRoleItem";
            public const string FindUserInRole = "FindUserInRole";
        }

        public static class RoleItemType
        {
            public const string Group = "Group";
            public const string User = "User";
            public const string SmartObject = "SmartObject";
            public const string Unknown = "Unknown";
        }

        public static class Properties
        {
            public const string RoleName = "RoleName";
            public const string RoleDescription = "Description";
            public const string RoleGuid = "Guid";
            public const string RoleDynamic = "IsDynamic";
            public const string RoleExtraData = "ExtraData";
            public const string RoleItem = "RoleItem";
            public const string RoleItemType = "RoleItemType";
            public const string RoleExclude = "Exclude";
            public const string IsRoleMember = "IsRoleMember";
        }

        public static class Configs
        {
            public const string K2ConnectionString = "K2 Management ConnectionString"; //TODO: Change to 'User Role Management Server' ?
            public const string AddDynamicServiceObjects = "Add Dynamic Service Objects";
        }

        public static class Defaults
        {
            public const string K2ConnectionString = "Integrated=True;IsPrimaryLogin=True;Authenticate=True;EncryptedPassword=False;Host=localhost;Port=5555";
            public const bool AddDynamicServiceObjects = false;
        }

    }
}
