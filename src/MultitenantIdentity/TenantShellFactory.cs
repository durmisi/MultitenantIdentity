using Dotnettency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenants.Web.Client;
using Tenants.Web.Logic.Dtos;

namespace MultitenantIdentity
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

            foreach (var activeTenant in activeTenants)
            {
                if(!isValidTenantHost(activeTenant.Hosts, distinguisher.Uri))
                    continue;

                var tenant = new Tenant(activeTenant.TenantGuid, activeTenant.TenantName);
                var result = new TenantShell<Tenant>(tenant, 
                    activeTenant.Hosts
                        .Select(host => new TenantDistinguisher(new Uri(host)))
                        .ToArray()
                );

                return Task.FromResult(result);
            }

            throw new NotImplementedException("Please make request on ports 5000 - 5099 to see various behaviour.");

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
                _tenants = _tenantsClient.GetTenantsByService("Identity Server").Result;

            return _tenants.Result;
        }
    }
}
