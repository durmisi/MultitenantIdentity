using System;
using Tenants.Web.Logic.Base;

namespace Tenants.Web.Logic.Domain
{
    public class TenantService : Entity
    {
        private Tenant _tenant;
        private AppService _appService;
        private readonly string _hostName;

        protected TenantService()
        {

        }

        public TenantService(Tenant tenant, AppService appService, string hostName)
        : this()
        {
            if (string.IsNullOrEmpty(hostName))
                throw new ArgumentException("Value cannot be null or empty.", nameof(hostName));

            _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
            _appService = appService ?? throw new ArgumentNullException(nameof(appService));

            _hostName = hostName;
        }

        public virtual Tenant Tenant
        {
            get => _tenant;
        }

        public virtual AppService AppService
        {
            get => _appService;
        }

        public virtual string HostName => _hostName;
    }
}