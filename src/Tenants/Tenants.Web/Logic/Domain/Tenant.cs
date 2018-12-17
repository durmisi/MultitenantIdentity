using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Tenants.Web.Logic.Base;

namespace Tenants.Web.Logic.Domain
{
    public class Tenant : Entity
    {
        private string _name;
        private bool _isActive;
        private Guid _tenantGuid;
        private IList<TenantService> _tenantServices;
        protected Tenant()
        {
            _tenantServices = new List<TenantService>();
        }

        public Tenant(string name, Guid tenantGuid)
        : this()
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));

            if (tenantGuid == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(tenantGuid));
            }

            _isActive = false;
            _tenantGuid = tenantGuid;
        }

        public virtual string Name
        {
            get => _name;
        }

        public virtual Guid TenantGuid
        {
            get => _tenantGuid;
        }

        public virtual bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        public virtual IEnumerable<TenantService> TenantServices
        {
            get { return _tenantServices; }
        }

        public virtual void AddService(AppService service, string hostName)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            var tenantService = new TenantService(this, appService: service, hostName: hostName);

            if (TenantServices.Any(x => x.AppService.Name == service.Name && x.HostName == hostName))
            {
                throw new InvalidOperationException("App Service is already registered for this Tenant.");
            }

            _tenantServices.Add(tenantService);
        }

    }
}
