using Azure.Storage.Blobs;

namespace HackWebApi.Blob;

public class BlobStorageService
{
    private readonly string _connectionString;

    public BlobStorageService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task UploadFileAsync(string containerName, string fileName, Stream fileStream)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await blobContainerClient.CreateIfNotExistsAsync();

        var blobClient = blobContainerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, true);
    }
}

