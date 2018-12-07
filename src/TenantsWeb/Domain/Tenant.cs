using System;

namespace TenantsWeb.Domain
{
    public class Tenant
    {
        public Tenant(Guid tenantGuid, string name)
        {
            TenantGuid = tenantGuid;
            Name = name;
            Id = Guid.NewGuid();
        }

        public Guid TenantGuid { get; set; }
        public string Name { get; set; }
        public Guid Id { get; internal set; }

        public bool IsActive { get; set; }
    }
}
