using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using NetCoreAI.Project3_RapidApi.ViewModels;
using Newtonsoft.Json;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var apiKey = config["RapidApi:Key"];

List<ApiSeriesViewModel> apiSeriesViewModels = new List<ApiSeriesViewModel>();

var client = new HttpClient();
var request = new HttpRequestMessage
{
    Method = HttpMethod.Get,
    RequestUri = new Uri("https://imdb-top-100-movies.p.rapidapi.com/series/"),
    Headers =
    {
        { "x-rapidapi-key", apiKey },
        { "x-rapidapi-host", "imdb-top-100-movies.p.rapidapi.com" },
    },
};

using (var response = await client.SendAsync(request))
{
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStringAsync();

    apiSeriesViewModels = JsonConvert.DeserializeObject<List<ApiSeriesViewModel>>(body);

    foreach (var series in apiSeriesViewModels)
    {
        Console.WriteLine(series.rank + "-" + series.title + " -Rating: " + series.rating + " -Year: " + series.year);
    }
}

Console.ReadLine();