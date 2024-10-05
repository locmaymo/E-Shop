using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Post.API.Data;
using System.IO;
using System.Threading.Tasks;
using Post.API.Models;
using Post.API.ImageHost;

[Route("api/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    private readonly MongoDBContext _context;
    private readonly FreeImageHost _imageHost;

    public ImagesController(IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("MongoSettings:ConnectionString").Value;
        var databaseName = configuration.GetSection("MongoSettings:DatabaseName").Value;
        _context = new MongoDBContext(connectionString, databaseName);

        var freeImageHostKey = configuration.GetSection("FreeImageHost:Key").Value;
        var freeImageHostUrl = configuration.GetSection("FreeImageHost:Url").Value;
        _imageHost = new FreeImageHost(freeImageHostKey, freeImageHostUrl);
    }

    [HttpGet]
    [Route("images")]
    public async Task<IActionResult> GetImages()
    {
        var images = await _context.Images.Find(_ => true).ToListAsync();
        return Ok(images);
    }

    [HttpGet]
    [Route("image/{id}")]
    public async Task<IActionResult> GetImage(string id)
    {
        var image = await _context.Images.Find(i => i.Id == id).FirstOrDefaultAsync();
        if (image == null)
            return NotFound();

        return Ok(image);
    }

    [HttpPost]
    [Route("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromForm] string name, [FromForm] string description)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();
            var base64String = Convert.ToBase64String(fileBytes);

            var uploadResult = await _imageHost.UploadImageAsync(base64String);

            // Parse the uploadResult to get the actual URL
            var imageUrl = ParseImageUrl(uploadResult);

            var image = new Image
            {
                Name = name,
                Url = imageUrl,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Images.InsertOneAsync(image);

            return Ok(new { message = "File uploaded successfully", image });
        }

    }

    [HttpPut]
    [Route("update/{id}")]
    public async Task<IActionResult> UpdateImage(string id, [FromBody] Image image)
    {
        var existingImage = await _context.Images.Find(i => i.Id == id).FirstOrDefaultAsync();
        if (existingImage == null)
            return NotFound();

        image.Id = id;
        await _context.Images.ReplaceOneAsync(i => i.Id == id, image);

        return Ok(new { message = "Image updated successfully" });
    }

    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IActionResult> DeleteImage(string id)
    {
        var image = await _context.Images.Find(i => i.Id == id).FirstOrDefaultAsync();
        if (image == null)
            return NotFound();

        await _context.Images.DeleteOneAsync(i => i.Id == id);

        return Ok(new { message = "Image deleted successfully" });
    }

    private string ParseImageUrl(string uploadResult)
    {
        // Implement this method to parse the actual image URL from the uploadResult
        // This is just a placeholder implementation
        return uploadResult;
    }
}