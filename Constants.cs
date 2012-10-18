using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceCode.ServiceBroker.RolesManagement
{
    public static class Constants
    {
        public static class Methods
        {
            public const string ListRoleItem = "ListRoleItems";
            public const string AddRoleItem = "AddRoleItems";
            public const string DeleteRoleItem = "DeleteRoleItems";
        }


        public static class Properties
        {
            public const string RoleName = "RoleName";
            public const string RoleItem = "RoleItem";
        }

        public static class Configs
        {
            public const string K2ConnectionString = "K2 Management ConnectionString";
            public const string AddDynamicServiceObjects = "Add Dynamic Service Objects";
        }

        public static class Defaults
        {
            public const string K2ConnectionString = "Integrated=True;IsPrimaryLogin=True;Authenticate=True;EncryptedPassword=False;Host=localhost;Port=5555";
            public const bool AddDynamicServiceObjects = false;
        }

    }
}
