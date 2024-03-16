using Azure.Storage.Blobs;
using HackWebApi.Blob;
using HackWebApi.Domain;
using HackWebApi.Messaging;
using Infrastructure.DB.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HackWebApi.Controllers;

[ApiController]
[Route("api/videos")]
public class HackWebApiController : Controller
{
    private readonly ItemMessaging _itemMessaging;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ReqRepository _reqRepository;

    public HackWebApiController(ItemMessaging itemMessaging, BlobServiceClient blobServiceClient, ReqRepository reqRepository)
    {
        _itemMessaging = itemMessaging;
        _blobServiceClient = blobServiceClient;
        _reqRepository = reqRepository;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadItemAsync([FromForm] VideoRequest videoRequest) //[FromBody] Item item)
    {
        if (videoRequest == null || videoRequest.VideoFile.Length <= 0)
        {
            return BadRequest("Arquivo inválido.");
        }

        try
        {
            // BLOB
            var containerName = "videos"; 
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var fileExtension = Path.GetExtension(videoRequest.VideoFile.FileName);
            var blobName = $"{videoRequest.NomeVideo}{fileExtension}";

            var blobClient = containerClient.GetBlobClient(blobName);
            using var stream = videoRequest.VideoFile.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            var blobUrl = blobClient.Uri.ToString();

            //FILA
            var message = new requestToMessage();
            message.Name = videoRequest.NomeVideo;
            await _itemMessaging.SendMessageAsync(message);

            //DB
            var dbObj = new requestToDB();
            dbObj.NomeUsuario = videoRequest.NomeUsuario;
            dbObj.NomeVideo = videoRequest.NomeVideo;
            dbObj.URL = blobUrl;

            await _reqRepository.Add(dbObj);
            await _reqRepository.Save();

            //RET
            return Ok(new { Url = blobUrl });
        }

        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItemByIdAsync(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var item = await _reqRepository.GetById(id);

        if (item == null)
            return NotFound("Item not found");

        return Ok(await _reqRepository.GetById(id));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItemsAsync([FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        var items = await _reqRepository.GetAll(skip, take);

        if (!items.Any())
            return NotFound("Items aren't found");

        return Ok(items);
    }
}
