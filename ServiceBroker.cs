using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.SmartObjects.Services.ServiceSDK;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using SourceCode.Security.UserRoleManager.Management;
using System.Data;
using smartobjectClient = SourceCode.SmartObjects.Client;
using SourceCode.SmartObjects.Client.Filters;

namespace SourceCode.ServiceBroker.RolesManagement
{
    public class ServiceBroker : ServiceAssemblyBase
    {


        private string WFMServerConnectionString
        {
            get
            {
                return this.Service.ServiceConfiguration[Constants.Configs.K2ConnectionString].ToString();
            }
        }

        private bool AddDynamicRoles
        {
            get
            {
                return Convert.ToBoolean(this.Service.ServiceConfiguration[Constants.Configs.AddDynamicServiceObjects].ToString());
            }
        }


        public override string DescribeSchema()
        {
            try
            {
                this.Service.Name = "RoleManagerService";
                this.Service.MetaData.DisplayName = "Role Manager Service";
                this.Service.MetaData.Description = "Provices ServiceObjects to add/update roles and manage the role items in the roles.";

                #region Role Item Management
                ServiceObject roleItemManagement = new ServiceObject();
                roleItemManagement.Name = "RoleItemManagment";
                roleItemManagement.MetaData.DisplayName = "Role Item Management";
                roleItemManagement.MetaData.Description = "Manage role items on roles.";
                roleItemManagement.Active = true;

                roleItemManagement.Properties.Add(CreateProperty(Constants.Properties.RoleName, SoType.Text, "The name of the role to manage."));
                roleItemManagement.Properties.Add(CreateProperty(Constants.Properties.RoleItem, SoType.Text, "The FQN name of the role item."));
                roleItemManagement.Properties.Add(CreateProperty(Constants.Properties.RoleItemType, SoType.Text, "The type of role item (Group, User, SmartObject)."));
                roleItemManagement.Properties.Add(CreateProperty(Constants.Properties.RoleExtraData, SoType.Text, "Extradata for the role."));

                Method addRoleItem = new Method();
                addRoleItem.Name = Constants.Methods.AddRoleItem;
                addRoleItem.Type = MethodType.Create;
                addRoleItem.MetaData.DisplayName = "Add Role Item";
                addRoleItem.MetaData.Description = "Add a role item to the given role.";
                addRoleItem.InputProperties.Add(Constants.Properties.RoleName);
                addRoleItem.InputProperties.Add(Constants.Properties.RoleItem);
                addRoleItem.InputProperties.Add(Constants.Properties.RoleItemType);
                addRoleItem.InputProperties.Add(Constants.Properties.RoleExtraData);
                roleItemManagement.Methods.Add(addRoleItem);

                Method deleteRoleItem = new Method();
                deleteRoleItem.Name = Constants.Methods.DeleteRoleItem;
                deleteRoleItem.Type = MethodType.Delete;
                deleteRoleItem.MetaData.DisplayName = "Delete Role Item";
                deleteRoleItem.MetaData.Description = "Delete a role item to the given role.";
                deleteRoleItem.InputProperties.Add(Constants.Properties.RoleName);
                deleteRoleItem.InputProperties.Add(Constants.Properties.RoleItem);
                roleItemManagement.Methods.Add(deleteRoleItem);


                Method listRoleItems = new Method();
                listRoleItems.Name = Constants.Methods.ListRoleItems;
                listRoleItems.Type = MethodType.List;
                listRoleItems.MetaData.DisplayName = "List Role Items";
                listRoleItems.MetaData.Description = "List all role items for the given role.";
                listRoleItems.InputProperties.Add(Constants.Properties.RoleName);
                listRoleItems.ReturnProperties.Add(Constants.Properties.RoleItem);
                listRoleItems.ReturnProperties.Add(Constants.Properties.RoleItemType);
                listRoleItems.ReturnProperties.Add(Constants.Properties.RoleExtraData);
                roleItemManagement.Methods.Add(listRoleItems);

                #endregion Role Item Management


                #region Role Management
                ServiceObject roleManagement = new ServiceObject();
                roleManagement.Name = "RoleManagement";
                roleManagement.MetaData.DisplayName = "Role Management";
                roleManagement.MetaData.Description = "Add/update/delete/list roles.";
                roleManagement.Active = true;

                roleManagement.Properties.Add(CreateProperty(Constants.Properties.RoleName, SoType.Text, "Name of the role."));
                roleManagement.Properties.Add(CreateProperty(Constants.Properties.RoleDescription, SoType.Text, "The description of the role."));
                roleManagement.Properties.Add(CreateProperty(Constants.Properties.RoleGuid, SoType.Guid, "The guid of a role."));
                roleManagement.Properties.Add(CreateProperty(Constants.Properties.RoleDynamic, SoType.YesNo, "Is a role Dynamic?"));
                roleManagement.Properties.Add(CreateProperty(Constants.Properties.RoleExtraData, SoType.Text, "Extradata for the role."));
                roleManagement.Properties.Add(CreateProperty(Constants.Properties.IsRoleMember, SoType.YesNo, "Is a role member."));
                roleManagement.Properties.Add(CreateProperty(Constants.Properties.RoleItem, SoType.Text, "The FQN name of the role item."));
                roleManagement.Properties.Add(CreateProperty(Constants.Properties.RoleItemType, SoType.Text, "The type of role item (Group, User, SmartObject)."));

                Method listRoles = new Method();
                listRoles.Name = Constants.Methods.ListRoles;
                listRoles.Type = MethodType.List;
                listRoles.MetaData.DisplayName = "List Roles";
                listRoles.MetaData.Description = "List all roles in the system.";
                listRoles.ReturnProperties.Add(Constants.Properties.RoleName);
                listRoles.ReturnProperties.Add(Constants.Properties.RoleDescription);
                listRoles.ReturnProperties.Add(Constants.Properties.RoleGuid);
                listRoles.ReturnProperties.Add(Constants.Properties.RoleDynamic);
                listRoles.ReturnProperties.Add(Constants.Properties.RoleExtraData);
                roleManagement.Methods.Add(listRoles);

                Method isRoleMember = new Method();
                isRoleMember.Name = Constants.Methods.FindUserInRole;
                isRoleMember.Type = MethodType.Read;
                isRoleMember.MetaData.DisplayName = "Find user in role";
                isRoleMember.MetaData.Description = "Checks if the specified FQN of the user is a member of the specified role.";
                isRoleMember.InputProperties.Add(Constants.Properties.RoleItem);
                isRoleMember.InputProperties.Add(Constants.Properties.RoleName);
                isRoleMember.ReturnProperties.Add(Constants.Properties.IsRoleMember);
                roleManagement.Methods.Add(isRoleMember);

                Method addRole = new Method();
                addRole.Name = Constants.Methods.AddRole;
                addRole.Type = MethodType.Create;
                addRole.MetaData.DisplayName = "Add Role";
                addRole.MetaData.Description = "Add a new role to the system.";
                addRole.InputProperties.Add(Constants.Properties.RoleName);
                addRole.InputProperties.Add(Constants.Properties.RoleDescription);
                addRole.InputProperties.Add(Constants.Properties.RoleDynamic);
                addRole.InputProperties.Add(Constants.Properties.RoleItem);
                addRole.InputProperties.Add(Constants.Properties.RoleItemType);
                roleManagement.Methods.Add(addRole);

                Method deleteRole = new Method();
                deleteRole.Name = Constants.Methods.DeleteRole;
                deleteRole.Type = MethodType.Delete;
                deleteRole.MetaData.DisplayName = "Delete Role";
                deleteRole.MetaData.Description = "Delete a role from the system.";
                deleteRole.InputProperties.Add(Constants.Properties.RoleGuid);
                deleteRole.InputProperties.Add(Constants.Properties.RoleName);
                roleManagement.Methods.Add(deleteRole);

                #endregion Role Management

                this.Service.ServiceObjects.Add(roleManagement);
                this.Service.ServiceObjects.Add(roleItemManagement);
                ServicePackage.IsSuccessful = true;

                return base.DescribeSchema();
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendFormat("Exception: {0}", ex.Message);

                if (ex.InnerException != null)
                {
                    error.AppendFormat("InnerException: {0}", ex.InnerException.Message);
                }
                ServicePackage.ServiceMessages.Add(error.ToString(), MessageSeverity.Error);
                ServicePackage.IsSuccessful = false;
            }

            return base.DescribeSchema();
        }


        public override string GetConfigSection()
        {
            this.Service.ServiceConfiguration.Add(Constants.Configs.K2ConnectionString, true, Constants.Defaults.K2ConnectionString);
            this.Service.ServiceConfiguration.Add(Constants.Configs.AddDynamicServiceObjects, true, Constants.Defaults.AddDynamicServiceObjects);
            return base.GetConfigSection();
        }

        public override void Execute()
        {

            try
            {
                ServiceObject serviceObject = this.Service.ServiceObjects[0];
                Method serviceMethod = serviceObject.Methods[0];

                switch (serviceMethod.Name)
                {
                    case Constants.Methods.ListRoleItems:
                        ListRoleItems();
                        break;
                    case Constants.Methods.AddRoleItem:
                        AddRoleItem();
                        break;
                    case Constants.Methods.DeleteRoleItem:
                        DeleteRoleItem();
                        break;
                    case Constants.Methods.ListRoles:
                        ListRoles();
                        break;
                    case Constants.Methods.AddRole:
                        AddRole();
                        break;
                    case Constants.Methods.DeleteRole:
                        DeleteRole();
                        break;
                    case Constants.Methods.FindUserInRole:
                        FindUserInRole();
                        break;
                }
                ServicePackage.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                StringBuilder error = new StringBuilder();
                error.AppendFormat("Exception.Message: {0}", ex.Message);
                error.AppendFormat("Exception.StackTrace: {0}", ex.Message);

                Exception innerEx = ex;
                int i = 0;
                while (innerEx.InnerException != null)
                {
                    error.AppendFormat("{0} InnerException.Message: {1}", i, ex.InnerException.Message);
                    error.AppendFormat("{0} InnerException.StackTrace: {1}", i, ex.InnerException.StackTrace);
                    innerEx = innerEx.InnerException;
                    i++;
                }
                ServicePackage.ServiceMessages.Add(error.ToString(), MessageSeverity.Error);
                ServicePackage.IsSuccessful = false;
            }
        }

        private void FindUserInRole()
        {
            ServiceObject serviceObject = this.Service.ServiceObjects[0];
            serviceObject.Properties.InitResultTable();

            DataTable results = this.ServicePackage.ResultTable;
            DataRow row;
            bool isRoleMember = false;

            UserRoleManager urmServer = new UserRoleManager();
            using (urmServer.CreateConnection())
            {
                urmServer.Connection.Open(WFMServerConnectionString);
                Role role = urmServer.GetRole(serviceObject.Properties[Constants.Properties.RoleName].Value as string);
                if (role == null)
                {
                    throw new ApplicationException(Constants.ErrorText.RoleNotExist);
                }

                foreach (RoleItem roleItem in role.RoleItems)
                {
                    string roleItemName = roleItem.Name;

                    if (roleItem is UserItem)
                    {
                        // check if the specified username matches the current roleItem name
                        if (serviceObject.Properties[Constants.Properties.RoleItem].Value.ToString() == roleItem.Name)
                        {
                            // user exist in role
                            row = results.NewRow();
                            results.Rows.Add(FillResultRow(row, true));
                            isRoleMember = true;
                            break;
                        }
                    }
                    else
                    {
                        // It is a group item, use the smartobject method UMUser.Get_Group_Users to resolve all group users  

                        // Open a K2 Server connection
                        smartobjectClient.SmartObjectClientServer smoServer = new smartobjectClient.SmartObjectClientServer();
                        smoServer.CreateConnection();
                        smoServer.Connection.Open(WFMServerConnectionString);

                        // Get a handle to the ' UMUser' SmartObject
                        smartobjectClient.SmartObject umUser = smoServer.GetSmartObject("UMUser");

                        // Specify which method will be called
                        smartobjectClient.SmartListMethod getGroupUsers = umUser.ListMethods["Get_Group_Users"];
                        umUser.MethodToExecute = getGroupUsers.Name;

                        // Split FQN in SecurityLabel and groupname
                        string[] fqn = roleItem.Name.Split(':');

                        // Set the input properties
                        getGroupUsers.InputProperties["Labelname"].Value = fqn[0];
                        getGroupUsers.InputProperties["Group_name"].Value = fqn[1];

                        // Call the method
                        smartobjectClient.SmartObjectList smartObjectGroupUsers = smoServer.ExecuteList(umUser);

                        List<string> groupUsers = new List<string>();

                        foreach (smartobjectClient.SmartObject smo in smartObjectGroupUsers.SmartObjectsList)
                        {
                            groupUsers.Add(smo.Properties["FQN"].Value);
                        }

                        foreach (string user in groupUsers)
                        {
                            // check if the specified username matches the current roleItem name
                            if (serviceObject.Properties[Constants.Properties.RoleItem].Value.ToString() == user)
                            {
                                // user exist in role
                                row = results.NewRow();
                                results.Rows.Add(FillResultRow(row, true));
                                isRoleMember = true;
                                break;
                            }
                        }
//TODO: Close connection1!!
                    }
                }

                // the specified user is not found in the specified role
                if (!isRoleMember)
                {
                    row = results.NewRow();
                    results.Rows.Add(FillResultRow(row, false));
                }
            }
        }




        private void DeleteRoleItem()
        {
            ServiceObject serviceObject = this.Service.ServiceObjects[0];
            serviceObject.Properties.InitResultTable();

            UserRoleManager urmServer = new UserRoleManager();
            using (urmServer.CreateConnection())
            {
                urmServer.Connection.Open(WFMServerConnectionString);
                Role role = urmServer.GetRole(serviceObject.Properties[Constants.Properties.RoleName].Value as string);
                if (role == null)
                {
                    throw new ApplicationException(Constants.ErrorText.RoleNotExist);
                }

                string roleItemName = serviceObject.Properties[Constants.Properties.RoleItem].Value as string;
                RoleItem remItem = null;

                foreach (RoleItem ri in role.RoleItems)
                {
                    if (string.Compare(ri.Name, roleItemName, true) == 0)
                    {
                        remItem = ri;
                    }
                }
                if (remItem != null)
                {
                    role.RoleItems.Remove(remItem);
                }
                urmServer.UpdateRole(role);
            }
        }

        private void AddRoleItem()
        {
            ServiceObject serviceObject = this.Service.ServiceObjects[0];
            serviceObject.Properties.InitResultTable();

            UserRoleManager urmServer = new UserRoleManager();
            using (urmServer.CreateConnection())
            {
                urmServer.Connection.Open(WFMServerConnectionString);
                Role role = urmServer.GetRole(serviceObject.Properties[Constants.Properties.RoleName].Value as string);
                if (role == null)
                {
                    throw new ApplicationException(Constants.ErrorText.RoleNotExist);
                }
                string roleItemName = serviceObject.Properties[Constants.Properties.RoleItem].Value as string;
                string roleItemType = serviceObject.Properties[Constants.Properties.RoleItemType].Value as string;
                RoleItem ri;
                switch (roleItemType.ToUpper())
                {
                    case "GROUP":
                        ri = new GroupItem(roleItemName);
                        break;
                    case "USER":
                        ri = new UserItem(roleItemName);
                        break;
                    default:
                        throw new ApplicationException(string.Format(Constants.ErrorText.RoleTypeNotSupported, roleItemType));
                    //break;
                }
                role.RoleItems.Add(ri);

                urmServer.UpdateRole(role);

            }
        }

        private void ListRoleItems()
        {
            ServiceObject serviceObject = this.Service.ServiceObjects[0];
            serviceObject.Properties.InitResultTable();

            DataTable results = this.ServicePackage.ResultTable;

            UserRoleManager urmServer = new UserRoleManager();
            using (urmServer.CreateConnection())
            {
                urmServer.Connection.Open(WFMServerConnectionString);
                Role role = urmServer.GetRole(serviceObject.Properties[Constants.Properties.RoleName].Value as string);
                if (role == null)
                {
                    throw new ApplicationException(Constants.ErrorText.RoleNotExist);
                }
                foreach (RoleItem ri in role.RoleItems)
                {
                    DataRow row = results.NewRow();
                    results.Rows.Add(FillRoleItemRow(row, ri));
                }
            }
        }



        private static DataRow FillRoleItemRow(DataRow row, RoleItem ri)
        {
            row[Constants.Properties.RoleItem] = ri.Name;
            row[Constants.Properties.RoleExtraData] = ri.ExtraData;
            if (ri is GroupItem)
            {
                row[Constants.Properties.RoleItemType] = Constants.RoleItemType.Group;
            }
            else if (ri is UserItem)
            {
                row[Constants.Properties.RoleItemType] = Constants.RoleItemType.User;
            }
            else if (ri is SmartObjectItem)
            {
                row[Constants.Properties.RoleItemType] = Constants.RoleItemType.SmartObject;
            }
            else
            {
                row[Constants.Properties.RoleItemType] = Constants.RoleItemType.Unknown;
            }
            return row;
        }

        private static DataRow FillResultRow(DataRow row, bool isMember)
        {
            row[Constants.Properties.IsRoleMember] = isMember;
            return row;
        }

        private void ListRoles()
        {
            ServiceObject serviceObject = this.Service.ServiceObjects[0];
            serviceObject.Properties.InitResultTable();
            DataTable results = this.ServicePackage.ResultTable;

            UserRoleManager urmServer = new UserRoleManager();
            using (urmServer.CreateConnection())
            {
                urmServer.Connection.Open(WFMServerConnectionString);
                Role[] roles = urmServer.GetRoles();
                foreach (Role r in roles)
                {
                    DataRow row = results.NewRow();
                    row[Constants.Properties.RoleName] = r.Name;
                    row[Constants.Properties.RoleDescription] = r.Description;
                    row[Constants.Properties.RoleGuid] = r.Guid;
                    row[Constants.Properties.RoleDynamic] = r.IsDynamic;
                    row[Constants.Properties.RoleExtraData] = r.ExtraData;
                    results.Rows.Add(row);
                }
            }
        }

        private void AddRole()
        {
            ServiceObject serviceObject = this.Service.ServiceObjects[0];
            serviceObject.Properties.InitResultTable();

            Role role = new Role();
            UserRoleManager urmServer = new UserRoleManager();

            using (urmServer.CreateConnection())
            {
                urmServer.Connection.Open(WFMServerConnectionString);

                string roleName = serviceObject.Properties[Constants.Properties.RoleName].Value as string;
                string roleDescription = serviceObject.Properties[Constants.Properties.RoleDescription].Value as string;
                bool roleIsDynamic = Convert.ToBoolean(serviceObject.Properties[Constants.Properties.RoleDynamic].Value as string);

                role.Name = roleName;
                role.Description = roleDescription;
                role.IsDynamic = roleIsDynamic;

                // At least one roleItem has to be created with the new group
                string roleItemName = serviceObject.Properties[Constants.Properties.RoleItem].Value as string;
                string roleItemType = serviceObject.Properties[Constants.Properties.RoleItemType].Value as string;
                RoleItem ri;
                switch (roleItemType)
                {
                    case Constants.RoleItemType.Group:
                        ri = new GroupItem(roleItemName);
                        break;
                    case Constants.RoleItemType.User:
                        ri = new UserItem(roleItemName);
                        break;
                    default:
                        throw new ApplicationException(string.Format(Constants.ErrorText.RoleTypeNotSupported, roleItemType));
                    //break;
                }
                role.RoleItems.Add(ri);
                urmServer.CreateRole(role);
                urmServer.Connection.Close();
            }
        }

        private void DeleteRole()
        {
            ServiceObject serviceObject = this.Service.ServiceObjects[0];
            serviceObject.Properties.InitResultTable();

            UserRoleManager urmServer = new UserRoleManager();

            using (urmServer.CreateConnection())
            {
                urmServer.Connection.Open(WFMServerConnectionString);

                string roleName = serviceObject.Properties[Constants.Properties.RoleName].Value as string;
                Guid roleGUID = new Guid(serviceObject.Properties[Constants.Properties.RoleGuid].Value as string);

                Role role = urmServer.GetRole(roleName);
                if (role == null)
                {
                    throw new ApplicationException(Constants.ErrorText.RoleNotExist);
                }
                else
                {
                    urmServer.DeleteRole(roleGUID, roleName);
                    urmServer.Connection.Close();
                }
            }
        }

        /// <summary>
        /// "Scumbag base class" - requires an override that does not need implementation.
        /// </summary>
        public override void Extend() { }


        /// <summary>
        /// Creates an instance of a Service Object property
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="type">Service Object Type</param>
        /// <param name="description">A nice description.</param>
        /// <returns>The property you wanted to create.</returns>
        private Property CreateProperty(string name, SoType type, string description)
        {
            Property property = new Property();
            property.Name = name;
            property.SoType = type;
            property.MetaData.DisplayName = name;
            property.MetaData.Description = description;
            return property;
        }

    }
}
