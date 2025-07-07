using BizMate.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace BizMate.Application.Common.Extensions
{
    public class ImageBBUploader : IImageUploader
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ImageBBUploader(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["ImageBB:ApiKey"] ?? throw new ArgumentNullException("ImageBB API key is missing.");
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var base64Image = Convert.ToBase64String(ms.ToArray());

            var content = new MultipartFormDataContent
            {
                { new StringContent(base64Image), "image" }
            };

            var url = $"https://api.imgbb.com/1/upload?key={_apiKey}";
            var response = await _httpClient.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"ImageBB upload failed: {json}");

            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("data").GetProperty("url").GetString()!;
        }
    }
}
