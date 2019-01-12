using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dotnettency;
using Newtonsoft.Json;
using Tenants.Web.Client;
using Tenants.Web.Logic.Dtos;

namespace MultitenantClient
{
    public class TenantShellFactory : ITenantShellFactory<Tenant>
    {
        private readonly ITenantsClient _tenantsClient;
        private ApiResponse<List<TenantByServiceDto>> _tenants;

        public TenantShellFactory(ITenantsClient tenantsClient)
        {
            _tenantsClient = tenantsClient;
        }

        public Task<TenantShell<Tenant>> Get(TenantDistinguisher distinguisher)
        {
            var activeTenants = GetTenants();

            foreach (var tenant in activeTenants)
            {
                if (!isValidTenantHost(tenant.Hosts, distinguisher.Uri))
                    continue;

                var clientTenant = new Tenant(tenant.TenantGuid, tenant.TenantName)
                {
                     Configuration = JsonConvert.DeserializeObject<MultitenantClientConfiguration>(tenant.Configuration)
                };

                var result = new TenantShell<Tenant>(clientTenant,
                    tenant.Hosts
                        .Select(host => new TenantDistinguisher(new Uri(host)))
                        .ToArray()
                );

                return Task.FromResult(result);
            }

            throw new NotImplementedException("Please make request on ports 7000 - 7099 to see various behaviour.");
        }

        private bool isValidTenantHost(IEnumerable<string> hosts, Uri distinguisherUri)
        {
            foreach (var host in hosts)
            {
                var uri = new Uri(host);

                if (uri.Host != distinguisherUri.Host)
                {
                    continue;
                }

                if (uri.Port != distinguisherUri.Port)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private IEnumerable<TenantByServiceDto> GetTenants()
        {
            if (_tenants == null)
                _tenants = _tenantsClient.GetTenantsByService("MultiTenant Client").Result;

            return _tenants.Result;
        }
    }
}
