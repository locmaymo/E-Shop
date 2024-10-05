using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;

namespace Post.API.ImageHost {
public class FreeImageHost {
  public string Key { get; set; }
  public string Url { get; set; }

  public FreeImageHost(string key, string url) {
    Key = key;
    Url = url;
  }

  public async Task<string> UploadImageAsync(string base64) {
    using (var client = new HttpClient()) {
      var content = new MultipartFormDataContent();
      content.Add(new StringContent(base64), "source");
      content.Add(new StringContent(Key), "key");

      var response = await client.PostAsync(Url, content);
      response.EnsureSuccessStatusCode();

      var responseBody = await response.Content.ReadAsStringAsync();
      // to get url from response body access it in json format
      // responseBody.image.url then return it
      using (JsonDocument doc = JsonDocument.Parse(responseBody)) {
        JsonElement root = doc.RootElement;
        JsonElement imageUrlElement =
            root.GetProperty("image").GetProperty("url");
        return imageUrlElement.GetString();
      }
    }
  }
}
}
