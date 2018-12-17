using System;
using Tenants.Web.Logic.Base;

namespace Tenants.Web.Logic.Domain
{
    public class AppService :Entity
    {
        private string _name;
        private bool _isActive;

        protected AppService()
        {

        }

        public AppService(string name)
        : this()
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            _name = name;
            _isActive = true;
        }


        public virtual string Name
        {
            get => _name;
        }

        public virtual void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            _name = name;
        }
        
        public virtual bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }
    }
}
