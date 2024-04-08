using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace Secrets.Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        const string secretId = "dev/awsfundamentalscourse/apikey";
        var secretsClient = new AmazonSecretsManagerClient();
        
        var listSecretsRequest = new ListSecretVersionIdsRequest
        {
            SecretId = secretId,
            IncludeDeprecated = true
        };

        var versionResponse = await secretsClient.ListSecretVersionIdsAsync(listSecretsRequest);

        foreach (var version in versionResponse.Versions)
        {
            var stages = version.VersionStages
                .Aggregate("[", (current, stage) => current + (stage + ", "));
            stages += "]"; // closing bracket
            
            System.Console.WriteLine($"Version: {version.VersionId}; Created: {version.CreatedDate}; Stages: {stages}");

        }
        

        var getSecretRequest = new GetSecretValueRequest
        {
            SecretId = secretId,
            VersionId = "22746dd9-36d2-4367-8695-79e45f6a1b14"
            // VersionStage = "AWSPREVIOUS" // "AWSCURRENT" by default, these are called 'version stages'
            // any secrets that don't have a version stage are considered 'deprecated'
            // ALTERNATIVELY, YOU CAN FIND THE VERSION ID AND FETCH USING THAT
            
        };

        var getSecretResponse = await secretsClient.GetSecretValueAsync(getSecretRequest);

        System.Console.WriteLine(getSecretResponse.SecretString);

        var describeSecretRequest = new DescribeSecretRequest
        {
            SecretId = secretId
        };

        var descSecretResponse = await secretsClient.DescribeSecretAsync(describeSecretRequest);

        System.Console.WriteLine(descSecretResponse.VersionIdsToStages);
    }
}