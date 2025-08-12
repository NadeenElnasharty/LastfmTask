using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ArtistController : ControllerBase
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly string _apiKey;

    public ArtistController(IHttpClientFactory httpFactory, IConfiguration config)
    {
        _httpFactory = httpFactory;
        _apiKey = config["LastFm:ApiKey"] ?? throw new ArgumentNullException("LastFm:ApiKey");
    }

    [HttpGet("{artistName}")]
    public async Task<IActionResult> Get(string artistName)
    {
        var client = _httpFactory.CreateClient("LastFm");
        var nameEscaped = Uri.EscapeDataString(artistName);

        var infoTask = client.GetStringAsync($"?method=artist.getinfo&artist={nameEscaped}&api_key={_apiKey}&format=json");
        var tagsTask = client.GetStringAsync($"?method=artist.gettoptags&artist={nameEscaped}&api_key={_apiKey}&format=json");
        var albumsTask = client.GetStringAsync($"?method=artist.gettopalbums&artist={nameEscaped}&api_key={_apiKey}&format=json&limit=12");

        await Task.WhenAll(infoTask, tagsTask, albumsTask);

        using var infoDoc = JsonDocument.Parse(infoTask.Result);
        using var tagsDoc = JsonDocument.Parse(tagsTask.Result);
        using var albumsDoc = JsonDocument.Parse(albumsTask.Result);

        // extract artist info
        var rootArtist = infoDoc.RootElement.GetProperty("artist");
        var name = rootArtist.GetProperty("name").GetString();
        string image = null;
        if (rootArtist.TryGetProperty("image", out var imageArray) && imageArray.ValueKind == JsonValueKind.Array)
        {
            for (int i = imageArray.GetArrayLength() - 1; i >= 0; i--)
            {
                var img = imageArray[i];
                if (img.TryGetProperty("#text", out var txt) && !string.IsNullOrEmpty(txt.GetString()))
                {
                    image = txt.GetString();
                    break;
                }
            }
        }
        string bio = "";
        if (rootArtist.TryGetProperty("bio", out var bioElem) && bioElem.TryGetProperty("summary", out var sum))
            bio = sum.GetString();

        // tags
        var tagsList = new List<string>();
        if (tagsDoc.RootElement.TryGetProperty("toptags", out var toptags) && toptags.TryGetProperty("tag", out var tagArr) && tagArr.ValueKind == JsonValueKind.Array)
        {
            foreach (var tag in tagArr.EnumerateArray())
            {
                if (tag.TryGetProperty("name", out var tname))
                    tagsList.Add(tname.GetString());
            }
        }

        // albums
        var albumsList = new List<object>();
        if (albumsDoc.RootElement.TryGetProperty("topalbums", out var topalbums) && topalbums.TryGetProperty("album", out var albumArr) && albumArr.ValueKind == JsonValueKind.Array)
        {
            foreach (var album in albumArr.EnumerateArray())
            {
                var albName = album.GetProperty("name").GetString();
                var albUrl = album.GetProperty("url").GetString();
                string albImg = null;
                if (album.TryGetProperty("image", out var albImgArr) && albImgArr.ValueKind == JsonValueKind.Array)
                {
                    for (int i = albImgArr.GetArrayLength() - 1; i >= 0; i--)
                    {
                        var img = albImgArr[i];
                        if (img.TryGetProperty("#text", out var t) && !string.IsNullOrEmpty(t.GetString()))
                        {
                            albImg = t.GetString();
                            break;
                        }
                    }
                }
                albumsList.Add(new { name = albName, url = albUrl, image = albImg });
            }
        }

        var result = new
        {
            name,
            image,
            bio,
            tags = tagsList,
            albums = albumsList
        };

        return Ok(result);
    }
}
