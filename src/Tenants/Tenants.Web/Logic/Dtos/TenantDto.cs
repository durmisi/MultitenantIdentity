using System;

namespace Tenants.Web.Logic.Dtos
{
    public sealed class TenantDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Guid TenantGuid { get; set; }

        public bool IsActive { get; set; }
    }
    
}
