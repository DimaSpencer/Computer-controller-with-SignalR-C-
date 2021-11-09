using Newtonsoft.Json;
using System;

namespace VirusControllerLibrary
{
    [Serializable]
    public class AuthorizationData
    {
        public string access_token { get; set; }
    }
}