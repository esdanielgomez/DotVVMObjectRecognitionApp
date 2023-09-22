using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using AIServices.Models;
using AIServices.APIs;
using DotVVM.Framework.Controls;
using DotVVM.Core.Storage;

namespace ObjectsApp.ViewModels;

public class DefaultViewModel : MasterPageViewModel
{
    public List<CustomVisionModel> Objects { get; set; }
    public UploadedFilesCollection Files { get; set; }
    public string ImageInput { get; set; }
    public string ImageOutput { get; set; }

    private readonly CustomVision _customVision;
    private readonly IUploadedFileStorage _storage;
    private readonly IWebHostEnvironment _hostingEnvironment;

    public DefaultViewModel(CustomVision customVision, IWebHostEnvironment hostingEnvironment, IUploadedFileStorage storage)
    {
        _customVision = customVision;
        _hostingEnvironment = hostingEnvironment;

        // use dependency injection to request IUploadedFileStorage
        _storage = storage;

        Files = new UploadedFilesCollection();
    }

    public async Task SaveImage()
    {
        var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "CameraPhotos");
        ImageInput = "image" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + ".jpg";
        var ImageFile = Path.Combine(uploads, ImageInput);

        await _storage.SaveAsAsync(Files.Files[0].FileId, ImageFile);
    }

    public async Task Detect()
    {
        var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "CameraPhotos");
        var ImageFile = Path.Combine(uploads, ImageInput);

        ImageOutput = ImageInput.Replace(".jpg", "_out.jpg");
        var imageOutFile = Path.Combine(uploads, ImageOutput);

        Objects = await _customVision.DetectObjectsAsync(ImageFile, imageOutFile);
    }

    public override async Task PreRender()
    {
        await base.PreRender();
    }
}