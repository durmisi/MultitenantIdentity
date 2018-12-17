using FluentNHibernate.Mapping;

namespace Tenants.Web.Logic.Domain
{
    public class TenantServiceMap : ClassMap<Domain.TenantService>
    {
        public TenantServiceMap()
        {

            Schema("dbo");
            Table("TenantServices");

            Id(x => x.Id)
                .Column("Id")
                ;

            References(x => x.AppService)
                .Column("AppServiceId")
                .Access.CamelCaseField(Prefix.Underscore)
                ;

            References(x => x.Tenant)
                .Column("TenantId")
                .Access.CamelCaseField(Prefix.Underscore)
                ;

            Map(x => x.HostName)
                .Access.CamelCaseField(Prefix.Underscore)
                .Column("HostName")
                ;

        }
    }
}
