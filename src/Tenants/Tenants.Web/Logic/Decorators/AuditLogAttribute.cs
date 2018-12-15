using System;

namespace Tenants.Web.Logic.Decorators
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class AuditLogAttribute : Attribute
    {
        public AuditLogAttribute()
        {
        }
    }
}
