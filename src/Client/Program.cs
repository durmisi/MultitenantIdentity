using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("================= Tenant 1 ==========================");
            var accessToken = GetAccessToken("http://localhost:5000", "client", "secret", "api1");
            var result = HttpGet(accessToken, "http://localhost:6000/identity");
            Console.WriteLine(JArray.Parse(result));

            Console.WriteLine("================= Tenant 1 - Requesting a token using the password grant ==========================");
            accessToken = RequestResourceOwnerPasswordAccessToken("http://localhost:5000", "ro.client", "secret", "alice", "password", "api1");
            result = HttpGet(accessToken, "http://localhost:6000/identity");
            Console.WriteLine(JArray.Parse(result));


            Console.WriteLine("================= Tenant 2 ==========================");
            var accessToken2 = GetAccessToken("http://localhost:5002", "client2", "secret", "api2");
            var result2 = HttpGet(accessToken2, "http://localhost:6002/identity");
            Console.WriteLine(JArray.Parse(result2));
            Console.WriteLine("================= Tenant 2 - Requesting a token using the password grant ==========================");
            accessToken = RequestResourceOwnerPasswordAccessToken("http://localhost:5002", "ro.client2", "secret", "bob", "password", "api2");
            result = HttpGet(accessToken, "http://localhost:6002/identity");
            Console.WriteLine(JArray.Parse(result));

            Console.ReadKey();
        }

        private static string HttpGet(string accessToken, string url)
        {
            // call api
            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            var response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            return content;
        }

        private static string GetAccessToken(string url, string client, string secret, string scope)
        {
            // discover endpoints from metadata
            var disco = DiscoveryClient.GetAsync(url).Result;
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                throw new Exception(disco.Error);
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, client, secret);
            var tokenResponse = tokenClient.RequestClientCredentialsAsync(scope).Result;

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                throw new Exception(tokenResponse.Error);
            }

            Console.WriteLine(tokenResponse.Json);
            return tokenResponse.AccessToken;
        }

        private static string RequestResourceOwnerPasswordAccessToken(string url, string client, string secret,
            string userName, string password,
            string scope)
        {
            // discover endpoints from metadata
            var disco = DiscoveryClient.GetAsync(url).Result;
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                throw new Exception(disco.Error);
            }

            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, client, secret);
            var tokenResponse =  tokenClient.RequestResourceOwnerPasswordAsync(userName, password, scope).Result;

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                throw new Exception(tokenResponse.Error);
            }

            Console.WriteLine(tokenResponse.Json);
            return tokenResponse.AccessToken;
        }
        
    }
}
