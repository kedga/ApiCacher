using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using System.Web;

namespace ApiCacher.Data;

public static class ConfigureEndpoints
{
	public static void ConfigureApi(this WebApplication app)
	{
		app.MapGet("/api-cacher/get/{inputUrl}", Get);

		static async Task<IResult> Get(string inputUrl, IHttpClientFactory httpClientFactory, 
			ApiCacheDbContext context)
		{
			var cachedContent = await context.CachedRequests
				.FirstOrDefaultAsync(x => x.Url == inputUrl);

			if (cachedContent is not null)
			{
				await Console.Out.WriteLineAsync();
				await Console.Out.WriteLineAsync($"{DateTime.Now:HH:mm:ss} - Fetching CACHED DATA (from {inputUrl})");
				await Console.Out.WriteLineAsync();

				var jsonObject = JsonSerializer.Deserialize<JsonDocument>(cachedContent.Content);

				return Results.Ok(jsonObject);
			}

			var decodedUrl = HttpUtility.UrlDecode(inputUrl);

			if (!decodedUrl.StartsWith("http://") && !decodedUrl.StartsWith("https://"))
			{
				decodedUrl = "http://" + decodedUrl;
			}

			if (!Uri.TryCreate(decodedUrl, UriKind.Absolute, out Uri? requestUri) 
				|| requestUri is null)
			{
				throw new ArgumentException("Invalid URL provided: " + decodedUrl);
			}

			var httpClient = httpClientFactory.CreateClient();

			try
			{
                await Console.Out.WriteLineAsync();
                await Console.Out.WriteLineAsync($"{DateTime.Now:HH:mm:ss} - Fetching data from the internet: {requestUri}");
				await Console.Out.WriteLineAsync();

				var response = await httpClient.GetAsync(requestUri);

				response.EnsureSuccessStatusCode();

				var responseBody = await response.Content.ReadAsStringAsync();

				context.CachedRequests.Add(new ApiCacheModel()
				{
					Url = inputUrl,
					Content = responseBody
				});

				await context.SaveChangesAsync();

				var jsonObject = JsonSerializer.Deserialize<JsonDocument>(responseBody);

				return Results.Ok(jsonObject);
			}

			catch (HttpRequestException ex)
			{
				await Console.Out.WriteLineAsync("ERROR getting data: " + ex.Message);

				return Results.Problem(ex.Message);
			}
		}
	}
}
