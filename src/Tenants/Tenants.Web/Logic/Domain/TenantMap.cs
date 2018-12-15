using FluentNHibernate.Mapping;

namespace Tenants.Web.Logic.Domain
{
    public class CourseMap : ClassMap<Domain.Tenant>
    {
        public CourseMap()
        {
            Id(x => x.Id);

            Map(x => x.Name)
                .Access.CamelCaseField();

            Map(x => x.TenantGuid)
                .Access.CamelCaseField();

            Map(x => x.IsActive)
                .Access.CamelCaseField()
                ;
        }
    }
}
