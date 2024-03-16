namespace HackWebApi.Domain;

public class VideoRequest
{
    public string NomeUsuario { get; set; }
    public string NomeVideo { get; set; }
    public IFormFile VideoFile { get; set; }
}

public record requestToMessage
{
    public string Id { get; set; }
    public string? Name { get; set; }
}

public record requestToDB
{
    public int Id { get; set; }
    public string NomeUsuario { get; set; }
    public string NomeVideo { get; set; }
    public string? URL { get; set; }
}

// ID
// URL VIDEO STORAGE ACCOUNT
