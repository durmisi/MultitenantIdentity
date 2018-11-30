using System;
using System.Threading.Tasks;
using Dotnettency;

namespace MultitenantHybridClient
{
    public class TenantShellFactory : ITenantShellFactory<Tenant>
    {
        public Task<TenantShell<Tenant>> Get(TenantDistinguisher distinguisher)
        {
            if (distinguisher.Uri.Port == 8000 || distinguisher.Uri.Port == 8001)
            {
                Guid tenantId = Guid.Parse("049c8cc4-3660-41c7-92f0-85430452be22");
                var tenant = new Tenant(tenantId, "Moogle");
                // Also adding any additional Uri's that should be mapped to this same tenant.
                var result = new TenantShell<Tenant>(tenant, new Uri("http://localhost:8000"),
                                                             new Uri("http://localhost:8001"));
                return Task.FromResult(result);
            }

            if (distinguisher.Uri.Port == 8002)
            {
                Guid tenantId = Guid.Parse("b17fcd22-0db1-47c0-9fef-1aa1cb09605e");
                var tenant = new Tenant(tenantId, "Gicrosoft");
                var result = new TenantShell<Tenant>(tenant);
                return Task.FromResult(result);
            }


            throw new NotImplementedException("Please make request on ports 8000 - 8003 to see various behaviour.");

        }
    }
}
