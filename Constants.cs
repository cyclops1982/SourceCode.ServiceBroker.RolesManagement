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
            /// 19-10-2012 PBL - do these constants reflect the real method names in the ServiceBroker class?
            ///                  because then we should get rid of the 's' at the single item methods AddRolItem / DeleteRoleItem 
            ///                  and add it for the ListRoleItems constant name?
            public const string ListRoleItems = "ListRoleItems";
            public const string ListRoles = "ListRoles";
            public const string AddRoleItem = "AddRoleItem";
            public const string DeleteRoleItem = "DeleteRoleItem";
        }


        public static class Properties
        {
            public const string RoleName = "RoleName";
            public const string RoleItem = "RoleItem";
            public const string RoleDescription = "Description";
            public const string RoleExtraData = "ExtraData";
            public const string RoleGuid = "Guid";
            public const string RoleDynamic = "IsDynamic";
            public const string RoleExclude = "Exclude";
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
