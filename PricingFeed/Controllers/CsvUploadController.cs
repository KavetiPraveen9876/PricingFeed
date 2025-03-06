using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/upload")]
public class CsvUploadController : ControllerBase
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _config;

    public CsvUploadController(IAmazonS3 s3Client, IConfiguration config)
    {
        _s3Client = s3Client;
        _config = config;
    }

    [HttpPost("csv")]
    public async Task<IActionResult> UploadCsv([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("Invalid file");

        var bucketName = _config["AWS:BucketName"];
        var filePath = $"uploads/{file.FileName}";

        using (var newMemoryStream = new MemoryStream())
        {
            await file.CopyToAsync(newMemoryStream);
            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(newMemoryStream, bucketName, filePath);
        }

        return Ok(new { message = "File uploaded successfully", filePath });
    }
}
