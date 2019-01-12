using System;
using Tenants.Web.Logic.Dtos;

namespace MultitenantIdentity
{
    public class Tenant
    {

        public Tenant(Guid tenantGuid, string name)
        {
            TenantGuid = tenantGuid;
            Name = name;
        }

        public string Name { get; set; }

        public Guid TenantGuid { get; set; }

      
        public IdentityServerConfiguration Configuration { get; set; }
    }
}
