namespace Tenants.Web.Logic.Dtos
{
    public class AddServiceDto
    {
        public long TenantId { get; set; }

        public long AppServiceId { get; set; }

        public string HostName { get; set; }
    }
}
