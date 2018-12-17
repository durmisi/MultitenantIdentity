using System;

namespace Tenants.Web.Logic.Dtos
{
    public class RegisterTenantDto
    {
        public string Name { get; set; }
        public Guid TenantGuid { get; set; }
    }
}
