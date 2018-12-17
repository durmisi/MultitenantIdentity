using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Tenants.Web.Logic.Base;
using Tenants.Web.Logic.Dtos;
using Tenants.Web.Logic.Utils;

namespace Tenants.Web.Logic.Services
{
    public sealed class GetListQuery : IQuery<List<TenantDto>>
    {
      
        internal sealed class GetListQueryHandler : IQueryHandler<GetListQuery, List<TenantDto>>
        {
            private readonly QueriesConnectionString _connectionString;

            public GetListQueryHandler(QueriesConnectionString connectionString)
            {
                _connectionString = connectionString;
            }

            public List<TenantDto> Handle(GetListQuery query)
            {
                string sql = @"
                 
                            SELECT  [TenantId]
                                  ,[TenantGuid]
                                  ,[Name]
                                  ,[IsActive]
                              FROM [TenantsDb].[dbo].[Tenants]
                              order by [Name]
                ";

                using (SqlConnection connection = new SqlConnection(_connectionString.Value))
                {
                    var tenants = connection
                        .Query<TenantDto>(sql)
                        .ToList();

                    return tenants;
                }
            }
        }
    }
}
