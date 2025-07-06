using System.Text;
using System.Text.Json;

namespace BlazorWasmAppAzureAdLogin.Services
{
    public static class JwtHelper
    {
        public static Dictionary<string, object> DecodePayload(string token)
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
                throw new ArgumentException("Invalid JWT");

            var payload = parts[1];
            var jsonBytes = Convert.FromBase64String(PadBase64(payload));
            var json = Encoding.UTF8.GetString(jsonBytes);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;
        }

        private static string PadBase64(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: return base64 + "==";
                case 3: return base64 + "=";
                default: return base64;
            }
        }
    }

}
