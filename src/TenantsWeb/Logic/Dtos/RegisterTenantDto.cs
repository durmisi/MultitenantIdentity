using System;

namespace TenantsWeb.Logic.Dtos
{
    public class RegisterTenantDto
    {
        public string Name { get; set; }
        public Guid TenantGuid { get; internal set; }
    }
}
