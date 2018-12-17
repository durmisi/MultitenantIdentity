using FluentNHibernate.Mapping;

namespace Tenants.Web.Logic.Domain
{
    public class TenantMap : ClassMap<Domain.Tenant>
    {
        public TenantMap()
        {
            Schema("dbo");
            Table("Tenants");

            Id(x => x.Id)
                .Column("TenantId");

            Map(x => x.Name)
                .Access.CamelCaseField(Prefix.Underscore)
                .Column("Name");

            Map(x => x.TenantGuid)
                .Access.CamelCaseField(Prefix.Underscore)
                .Column("TenantGuid");

            Map(x => x.IsActive)
                .Access.CamelCaseField(Prefix.Underscore)
                .Column("IsActive")
                ;

            HasMany<TenantService>(x => x.TenantServices)
                .KeyColumn("TenantId")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                ;
                            
        }
    }
}
