using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Xms.Api.Identity.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<string>> Get(string org,string clientId, string userName, string password)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict["client_id"] = clientId;
            dict["client_secret"] = "pwdsecret";
            dict["grant_type"] = "password";
            dict["username"] = userName;
            dict["password"] = password;
            
            using (HttpClient http = new HttpClient())
            {
                using (var content = new FormUrlEncodedContent(dict))
                {
                    var msg = await http.PostAsync(_configuration["IdentityServer:TokenUrl"]+ "?org="+ org, content);
                    if (!msg.IsSuccessStatusCode)
                    {
                        return StatusCode(Convert.ToInt32(msg.StatusCode));
                    }

                    string result = await msg.Content.ReadAsStringAsync();
                    return Content(result, "application/json");
                }
            }
        }
    }
}