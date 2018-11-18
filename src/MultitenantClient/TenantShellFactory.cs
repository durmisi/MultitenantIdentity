using Dotnettency;
using System;
using System.Threading.Tasks;

namespace MultitenantClient
{
    public class TenantShellFactory : ITenantShellFactory<Tenant>
    {
        public Task<TenantShell<Tenant>> Get(TenantDistinguisher distinguisher)
        {
            if (distinguisher.Uri.Port == 7000 || distinguisher.Uri.Port == 7001)
            {
                Guid tenantId = Guid.Parse("049c8cc4-3660-41c7-92f0-85430452be22");
                var tenant = new Tenant(tenantId, "Moogle");
                // Also adding any additional Uri's that should be mapped to this same tenant.
                var result = new TenantShell<Tenant>(tenant, new Uri("http://localhost:7000"),
                                                             new Uri("http://localhost:7001"));
                return Task.FromResult(result);
            }

            if (distinguisher.Uri.Port == 7002)
            {
                Guid tenantId = Guid.Parse("b17fcd22-0db1-47c0-9fef-1aa1cb09605e");
                var tenant = new Tenant(tenantId, "Gicrosoft");
                var result = new TenantShell<Tenant>(tenant);
                return Task.FromResult(result);
            }


            throw new NotImplementedException("Please make request on ports 7000 - 7003 to see various behaviour.");

        }
    }
}
