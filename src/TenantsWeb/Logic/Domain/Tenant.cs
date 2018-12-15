using System;
using TenantsWeb.Logic.Base;

namespace TenantsWeb.Logic.Domain
{
    public class Tenant : Entity
    {
        private string _name;
        private bool _isActive;
        private Guid _tenantGuid;

        protected Tenant()
        {

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
    }
}
