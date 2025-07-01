using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace BlazorKeyVaultDemo.Services
{
    public class KeyVaultService3
    {
        private readonly ILogger<KeyVaultService> _logger;
        private readonly SecretClient _secretClient;

        public KeyVaultService3(string keyVaultUrl, ILogger<KeyVaultService> logger)
        {
            _logger = logger;
            _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            _logger.LogInformation("KeyVaultService created for {KeyVaultUrl}", keyVaultUrl);
        }

        public async Task<string> GetSecretAsync(string secretName)
        {
            _logger.LogInformation("Fetching secret {SecretName} from Key Vault", secretName);
            KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName);
            _logger.LogInformation("Fetched secret successfully");
            return secret.Value;
        }
    }
}
