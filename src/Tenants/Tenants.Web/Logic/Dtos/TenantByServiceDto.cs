using System;
using System.Collections.Generic;

namespace Tenants.Web.Logic.Dtos
{
    public class TenantByServiceDto
    {
        public Guid TenantGuid { get; set; }

        public string TenantName { get; set; }

        public string Configuration { get; set; }    

        public List<string> Hosts { get; set; }
    }
}
