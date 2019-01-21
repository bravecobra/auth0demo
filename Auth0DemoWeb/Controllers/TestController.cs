using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Auth0DemoWeb.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        private readonly IConfiguration _configuration;
        //private static string accessToken;
        //private static string refreshToken;
        private static HttpClient Client = new HttpClient();

        public TestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> GetAllValues(string apb="")
        {
            await SetupAuthorizationHeader();
            var response  = await Client.GetAsync(GetEndpointUri(apb) + "/api/values");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            return View(nameof(Index));
        }

        private string GetEndpointUri(string apb)
        {
            string uri;
            switch (apb)
            {
                case "111222":
                {
                    uri = _configuration["Backend:pharmacy111222"];
                    break;
                }
                case "999999":
                {
                    uri = _configuration["Backend:pharmacy999999"];
                    break;
                }
                default:
                {
                    uri = _configuration["Backend:Uri"];
                    break;
                }
            }

            return uri;
        }

        public async Task<IActionResult> GetById(string apb = "")
        {
            await SetupAuthorizationHeader();
            var response = await Client.GetAsync(GetEndpointUri(apb) + "/api/values/2");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            return View(nameof(Index));
        }

        public async Task<IActionResult> Create(string apb = "")
        {
            await SetupAuthorizationHeader();
            
            var response = await Client.PostAsync(
                GetEndpointUri(apb) + "/api/values", 
                 new StringContent(JsonConvert.SerializeObject(GetSamplePayload()), 
                 Encoding.UTF8, 
                 "application/json")
            );

            response.EnsureSuccessStatusCode();

            return View(nameof(Index));
        }

        public async Task<IActionResult> Update(string apb = "")
        {
            await SetupAuthorizationHeader();
            var response = await Client.PutAsync(
                GetEndpointUri(apb) + "/api/values",
                new StringContent(JsonConvert.SerializeObject(GetSamplePayload()), 
                Encoding.UTF8, 
                "application/json"));

            response.EnsureSuccessStatusCode();

            return View(nameof(Index));
        }

        public async Task<IActionResult> Delete(string apb = "")
        {
            await SetupAuthorizationHeader();
            var response = await Client.DeleteAsync(GetEndpointUri(apb) + "/api/values/1");
            response.EnsureSuccessStatusCode();

            return View(nameof(Index));
        }

        private async Task SetupAuthorizationHeader()
        {
            //used only to prove that we can acquire the refresh token
            //if(string.IsNullOrEmpty(refreshToken))
            //{
            //    refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            //}

            //if(string.IsNullOrEmpty(accessToken))
            //{
            //    accessToken = await HttpContext.GetTokenAsync("access_token");
            //}
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            //if (Client.DefaultRequestHeaders.Authorization == null)
            //{
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //}
        }

        private Payload GetSamplePayload()
        {
            return new Payload
            {
                id = DateTime.Now.Millisecond,
                Value = $"value-{DateTime.Now.Ticks}"
            };
        }
    }
}
