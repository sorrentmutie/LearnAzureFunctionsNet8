using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LearnFunctions
{
    public class BlobFunction
    {
        private readonly ILogger<BlobFunction> _logger;

        public BlobFunction(ILogger<BlobFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(BlobFunction))]
        [BlobOutput("myoutputcontainer/{name}", Connection = "MyStorageAccountConnectionString")]
        public async Task<string> Run(
            [BlobTrigger("mycontainer/{name}", Connection = "MyStorageAccountConnectionString")] byte[] stream, string name)
        {
            var blobStreamReader = new StreamReader(new MemoryStream(stream, false));
            var x = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {x}");
            return $"My Blob Output {x}";
        }
    }
}
