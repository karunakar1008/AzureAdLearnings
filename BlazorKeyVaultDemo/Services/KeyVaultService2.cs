using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace BlazorKeyVaultDemo.Services
{
    public class KeyVaultService2
    {
        private readonly SecretClient _secretClient;
        private readonly IConfiguration configuration;
        private string keyVaultUrl;
        public KeyVaultService2(IConfiguration configuration)
        {
            this.configuration = configuration;
            keyVaultUrl = $"https://{this.configuration["KeyVaultName"]}.vault.azure.net/";
            _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

        }
        public async Task<string> GetSecretAsync(string secretName)
        {
            KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);
            return secret.Value;
        }
    }
}
