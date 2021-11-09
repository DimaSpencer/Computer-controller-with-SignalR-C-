using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace VirusControllerLibrary
{
    public class AuthorizationRequest
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly Uri _authorizeUri;
        public AuthorizationRequest(Uri authorizeUri)
        {
            _authorizeUri = authorizeUri;
        }
        public async Task<string> AuthorizeWithTokenAsync(string username, string password)
        {
            var values = new Dictionary<string, string>
            {
                { "username", username },
                { "password", password }
            };

            var content = new FormUrlEncodedContent(values);

            var response = await _httpClient.PostAsync(_authorizeUri, content);
            string jsonString = await response.Content.ReadAsStringAsync();

            var authorizationData = JsonSerializer.Deserialize<AuthorizationData>(jsonString);
            return authorizationData.access_token;
        }
    }
}
