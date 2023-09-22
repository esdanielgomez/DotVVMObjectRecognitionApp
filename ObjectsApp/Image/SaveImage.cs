using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace ObjectsApp.Image;

public class SaveImage : Controller
{
    readonly IWebHostEnvironment _hostingEnvironment;

    public SaveImage(IWebHostEnvironment hostingEnvironment)
    {
        _hostingEnvironment = hostingEnvironment;
    }

    [Route("api/[controller]/Save")]
    [HttpPost]
    public async Task<IActionResult> Save(IFormFile file)
    {
        var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "CameraPhotos");
        var filePath = Path.Combine(uploads, file.FileName);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return Ok("File uploaded successfully");
    }
}