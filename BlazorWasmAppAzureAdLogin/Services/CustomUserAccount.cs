using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Text.Json.Serialization;

namespace BlazorWasmAppAzureAdLogin.Services
{
    public class CustomUserAccount : RemoteUserAccount
    {
        [JsonPropertyName("roles")]
        public List<string>? Roles { get; set; }

        //[JsonPropertyName("wids")]
        //public List<string>? Wids { get; set; }

        //[JsonPropertyName("oid")]
        //public string? Oid { get; set; }
    }
}
