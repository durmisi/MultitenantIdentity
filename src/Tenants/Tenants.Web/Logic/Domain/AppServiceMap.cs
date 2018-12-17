using FluentNHibernate.Mapping;

namespace Tenants.Web.Logic.Domain
{
    public class AppServiceMap : ClassMap<Domain.AppService>
    {
        public AppServiceMap()
        {
            Schema("dbo");
            Table("AppServices");

            Id(x => x.Id)
                .Column("AppServiceId")
                ;

            Map(x => x.Name)
                .Access.CamelCaseField(Prefix.Underscore)
                .Column("Name")
                ;

            Map(x => x.IsActive)
                .Access.CamelCaseField(Prefix.Underscore)
                .Column("IsActive")
                ;
        }
    }

    
}
