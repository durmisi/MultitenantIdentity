using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Tenants.Web.Logic.Base;
using Tenants.Web.Logic.Dtos;
using Tenants.Web.Logic.Utils;

namespace Tenants.Web.Logic.Services
{
    public sealed class GetTenantsByServiceQuery : IQuery<List<TenantByServiceDto>>
    {
        public string AppServiceName;

        public GetTenantsByServiceQuery(string serviceName)
        {
            AppServiceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
        }

        internal sealed class GetTenantsByServiceQueryHandler : IQueryHandler<GetTenantsByServiceQuery, List<TenantByServiceDto>>
        {
            private readonly QueriesConnectionString _connectionString;

            public GetTenantsByServiceQueryHandler(QueriesConnectionString connectionString)
            {
                _connectionString = connectionString;
            }

            public List<TenantByServiceDto> Handle(GetTenantsByServiceQuery query)
            {
                string sql = @"
                              SELECT 
                                    T.TenantGuid,
                                    T.[Name] as TenantName, 
                                    TSH.HostName
                              FROM Tenants T
                              inner join TenantServices TS on T.TenantId = TS.TenantId
                              inner join AppServices ASrv on ASrv.AppServiceId = TS.AppServiceId
							  inner join TenantServiceHosts TSH on TS.Id = TSH.[TenantServiceId]
                                  where T.IsActive = 1 and ASrv.[Name] = @AppServiceName
                              order by TenantGuid
                ";

                using (SqlConnection connection = new SqlConnection(_connectionString.Value))
                {
                    var tenantByServiceDtoIntls = connection
                        .Query<TenantByServiceDtoIntl>(sql, new { AppServiceName = query.AppServiceName })
                        .ToList();

                    var tenants = new List<TenantByServiceDto>();

                    foreach (var grouping in tenantByServiceDtoIntls.GroupBy(x => x.TenantGuid))
                    {
                        var tenant = tenantByServiceDtoIntls.First(x => x.TenantGuid == grouping.Key);
                        var hosts = tenantByServiceDtoIntls.Where(x => x.TenantGuid == grouping.Key)
                            .Select(x => x.HostName).ToList();

                        tenants.Add(new TenantByServiceDto()
                        {
                            TenantGuid = grouping.Key,
                            TenantName = tenant.TenantName,
                            Hosts = hosts
                        });
                    }

                    return tenants;
                }
            }

            private class TenantByServiceDtoIntl
            {
                public Guid TenantGuid { get; set; }

                public string TenantName { get; set; }

                public string HostName { get; set; }

            }
        }
    }
}
