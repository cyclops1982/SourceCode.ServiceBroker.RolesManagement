using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SourceCode.SmartObjects.Services.ServiceSDK;
using SourceCode.SmartObjects.Services.ServiceSDK.Types;
using SourceCode.SmartObjects.Services.ServiceSDK.Objects;

namespace SourceCode.ServiceBroker.RolesManagement
{
    public class ServiceBroker : ServiceAssemblyBase
    {
        public override string DescribeSchema()
        {
            try
            {
                this.Service.Name = "RoleManagerService";
                this.Service.MetaData.DisplayName = "Role Manager Service";
                this.Service.MetaData.Description = "Provices ServiceObjects to add/update roles and manage the roleitems in the roles.";

                ServiceObject serviceObject = new ServiceObject();
                serviceObject.Name = "RoleManagment";
                serviceObject.MetaData.DisplayName = "Role Management";
                serviceObject.MetaData.Description = "Manage roles (add/remove roleitems)";
                serviceObject.Active = true;

                serviceObject.Properties.Add(CreateProperty(Constants.Properties.RoleName, SoType.Text, "The name of the role."));
                serviceObject.Properties.Add(CreateProperty(Constants.Properties.RoleItem, SoType.Text, "The name of the role item."));

                Method addRoleItem = new Method();
                addRoleItem.Name = Constants.Methods.AddRoleItem;
                addRoleItem.Type = MethodType.Execute;
                addRoleItem.MetaData.DisplayName = "Add Role Item";
                addRoleItem.MetaData.Description = "Add a role item to the given role.";
                addRoleItem.InputProperties.Add(Constants.Properties.RoleName);
                addRoleItem.InputProperties.Add(Constants.Properties.RoleItem);
                serviceObject.Methods.Add(addRoleItem);

                Method deleteRoleItem = new Method();
                deleteRoleItem.Name = Constants.Methods.DeleteRoleItem;
                deleteRoleItem.Type = MethodType.Execute;
                deleteRoleItem.MetaData.DisplayName = "Delete Role Item";
                deleteRoleItem.MetaData.Description = "Delete a role item to the given role.";
                deleteRoleItem.InputProperties.Add(Constants.Properties.RoleName);
                deleteRoleItem.InputProperties.Add(Constants.Properties.RoleItem);
                serviceObject.Methods.Add(deleteRoleItem);


                Method listRoleItem = new Method();
                listRoleItem.Name = Constants.Methods.ListRoleItem;
                listRoleItem.Type = MethodType.List;
                listRoleItem.MetaData.DisplayName = "List Role Item";
                listRoleItem.MetaData.Description = "List all role items for the given role.";
                listRoleItem.InputProperties.Add(Constants.Properties.RoleName);
                serviceObject.Methods.Add(listRoleItem);

                
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
            ServiceObject serviceObject = this.Service.ServiceObjects[0];
            Method serviceMethod = serviceObject.Methods[0];

            switch (serviceMethod.Name)
            {
                case Constants.Methods.ListRoleItem:
                    ListRoleItems();
                    break;
            }
        }

        private void ListRoleItems()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// "Scumbag base class" - requires an override that does not need implementation.
        /// </summary>
        public override void Extend() {}



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
