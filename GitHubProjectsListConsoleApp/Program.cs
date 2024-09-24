using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("-- Welcome to GitHub Project List --");

        bool app = true;

        while (app)
        {
            Console.WriteLine("-- Type a GitHub user name --");
            string input = Console.ReadLine();

            if (input.ToLower() == "exit")
            {
                app = false;
                break;
            }

            await FetchGitHubRepositories(input);
        }
    }

    static async Task FetchGitHubRepositories(string userName)
    {
        string apiUrl = $"https://github-api.laurenskosters.nl/api/repo/{userName}";

        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseBody);

                bool success = jsonResponse.Value<bool>("success");

                if (success)
                {
                    JArray repositories = jsonResponse["repositories"] as JArray;

                    Console.WriteLine($"\n-- Repositories of {userName} --");

                    foreach (var repo in repositories)
                    {
                        string title = repo.Value<string>("title") ?? "No Title";
                        string href = repo.Value<string>("href") ?? "No URL";
                        string language = repo.Value<string>("language") ?? "Not specified";
                        string description = repo.Value<string>("description") ?? "No description";

                        Console.WriteLine($"Title: {title}");
                        Console.WriteLine($"Link: {href}");
                        Console.WriteLine($"Language: {language}");
                        Console.WriteLine($"Description: {description}");
                        Console.WriteLine("-------------------------------");
                    }
                }
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e.Message}");
        }
    }
}