using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.SmartObjects.Services.ServiceSDK;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;
using SourceCode.Security.UserRoleManager.Management;
using System.Data;

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
                this.Service.MetaData.Description = "Provices ServiceObjects to add/update roles and manage the roleitems in the roles.";

                //TODO: Create 2 ServiceObjects - one for Role management (add/delete/update role information)
                //TODO: and one for Role Item managmeent (add/delete/update role information) - this will help split up the properties.
                ServiceObject serviceObject = new ServiceObject();
                serviceObject.Name = "RoleManagment";
                serviceObject.MetaData.DisplayName = "Role Management";
                serviceObject.MetaData.Description = "Manage roles (add/remove)";
                serviceObject.Active = true;

                serviceObject.Properties.Add(CreateProperty(Constants.Properties.RoleName, SoType.Text, "The name of the role."));
                serviceObject.Properties.Add(CreateProperty(Constants.Properties.RoleItem, SoType.Text, "The name of the role item."));
                serviceObject.Properties.Add(CreateProperty(Constants.Properties.RoleDescription, SoType.Text, "The description of the role."));
                serviceObject.Properties.Add(CreateProperty(Constants.Properties.RoleDynamic, SoType.YesNo, "Is a rule Dynamic?"));
                serviceObject.Properties.Add(CreateProperty(Constants.Properties.RoleGuid, SoType.Guid, "The guid of a role."));
                serviceObject.Properties.Add(CreateProperty(Constants.Properties.RoleExtraData, SoType.Text, "Extradata for the role."));
                serviceObject.Properties.Add(CreateProperty(Constants.Properties.RoleExclude, SoType.YesNo, "Excluded role item."));

                Method addRoleItem = new Method();
                addRoleItem.Name = Constants.Methods.AddRoleItem;
                addRoleItem.Type = MethodType.Execute;
                addRoleItem.MetaData.DisplayName = "Add Role Item";
                addRoleItem.MetaData.Description = "Add a role item to the given role.";
                addRoleItem.InputProperties.Add(Constants.Properties.RoleName);
                addRoleItem.InputProperties.Add(Constants.Properties.RoleItem);
                addRoleItem.InputProperties.Add(Constants.Properties.RoleExclude);
                addRoleItem.InputProperties.Add(Constants.Properties.RoleExtraData);
                serviceObject.Methods.Add(addRoleItem);

                Method deleteRoleItem = new Method();
                deleteRoleItem.Name = Constants.Methods.DeleteRoleItem;
                deleteRoleItem.Type = MethodType.Execute;
                deleteRoleItem.MetaData.DisplayName = "Delete Role Item";
                deleteRoleItem.MetaData.Description = "Delete a role item to the given role.";
                deleteRoleItem.InputProperties.Add(Constants.Properties.RoleName);
                deleteRoleItem.InputProperties.Add(Constants.Properties.RoleItem);
                serviceObject.Methods.Add(deleteRoleItem);


                Method listRoleItems = new Method();
                listRoleItems.Name = Constants.Methods.ListRoleItems;
                listRoleItems.Type = MethodType.List;
                listRoleItems.MetaData.DisplayName = "List Role Item";
                listRoleItems.MetaData.Description = "List all role items for the given role.";
                listRoleItems.InputProperties.Add(Constants.Properties.RoleName);
                listRoleItems.ReturnProperties.Add(Constants.Properties.RoleItem);
                listRoleItems.ReturnProperties.Add(Constants.Properties.RoleExclude);
                listRoleItems.ReturnProperties.Add(Constants.Properties.RoleExtraData);
                serviceObject.Methods.Add(listRoleItems);

                Method listRoles = new Method();
                listRoles.Name = Constants.Methods.ListRoles;
                listRoles.Type = MethodType.List;
                listRoles.MetaData.DisplayName = "List all roles";
                listRoles.MetaData.Description = "List all roles in the system.";
                listRoles.ReturnProperties.Add(Constants.Properties.RoleName);
                listRoles.ReturnProperties.Add(Constants.Properties.RoleDescription);
                listRoles.ReturnProperties.Add(Constants.Properties.RoleGuid);
                listRoles.ReturnProperties.Add(Constants.Properties.RoleDynamic);
                listRoles.ReturnProperties.Add(Constants.Properties.RoleExtraData);
                serviceObject.Methods.Add(listRoles);


                this.Service.ServiceObjects.Add(serviceObject);
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

        private void DeleteRoleItem()
        {
            ServiceObject serviceObject = this.Service.ServiceObjects[0];
            serviceObject.Properties.InitResultTable();

            //DataTable results = this.ServicePackage.ResultTable;

            UserRoleManager urmServer = new UserRoleManager();
            using (urmServer.CreateConnection())
            {
                urmServer.Connection.Open(WFMServerConnectionString);
                Role role = urmServer.GetRole(serviceObject.Properties[Constants.Properties.RoleName].Value as string);

                string roleItemName = serviceObject.Properties[Constants.Properties.RoleItem].Value as string;
                RoleItem remItem = null;
                foreach (RoleItem ri in role.Include)
                {
                    if (string.Compare(ri.Name, roleItemName, true) == 0)
                        remItem = ri;
                }

                if (remItem != null)
                    role.Include.Remove(remItem);
                else
                {
                    foreach (RoleItem ri in role.Exclude)
                    {
                        if (string.Compare(ri.Name, roleItemName, true) == 0)
                            remItem = ri;
                    }

                    if (remItem != null)
                        role.Include.Remove(remItem);
                }
                urmServer.UpdateRole(role);
            }
        }

        private void AddRoleItem()
        {
            throw new NotImplementedException();
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
                RoleItemCollection<Role, RoleItem> items = role.Include;
                foreach (RoleItem ri in items)
                {
                    DataRow row = results.NewRow();
                    row[Constants.Properties.RoleItem] = ri.Name;
                    row[Constants.Properties.RoleExtraData] = ri.ExtraData;
                    row[Constants.Properties.RoleExclude] = false;
                    results.Rows.Add(row);
                }

                items = role.Exclude;
                foreach (RoleItem ri in items)
                {
                    DataRow row = results.NewRow();
                    row[Constants.Properties.RoleItem] = ri.Name;
                    row[Constants.Properties.RoleExtraData] = ri.ExtraData;
                    row[Constants.Properties.RoleExclude] = true;
                    results.Rows.Add(row);
                }
            }
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
