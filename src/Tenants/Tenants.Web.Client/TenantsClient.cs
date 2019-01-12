using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tenants.Web.Logic.Dtos;

namespace Tenants.Web.Client
{
    public class ApiResponse<T>
    {
        public T Result { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime TimeGenerated { get; set; }
    }

    public interface ITenantsClient
    {
        Task<ApiResponse<List<TenantDto>>> GetTenants();
        Task<ApiResponse<List<TenantByServiceDto>>> GetTenantsByService(string identityServer);
    }

    public class TenantsClient : ITenantsClient
    {
        private readonly HttpClient _httpClient;


        public TenantsClient(HttpClient httpClient) => this._httpClient = httpClient;

        public async Task<ApiResponse<List<TenantDto>>> GetTenants()
        {
            var response = await this._httpClient.GetAsync("/api/tenants");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<ApiResponse<List<TenantDto>>>();
        }

        public async Task<ApiResponse<List<TenantByServiceDto>>> GetTenantsByService(string appServiceName)
        {
            var response = await this._httpClient.GetAsync($"/api/tenants/{appServiceName}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<ApiResponse<List<TenantByServiceDto>>>();
        }
    }
}
