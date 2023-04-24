using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Text2Txt
{
    public class AI
    {
        // API Key for OpenAI
        public string apiKey = "API KEY HERE";

        public static async Task<string> APICall(string prompt, string apiKey) // Takes the prompt and the API key and returns the response
        {
            // API call
            var client = new HttpClient();
            prompt = prompt.Replace("\r", "").Replace("\n", " ").Replace("\"", " ");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.openai.com/v1/completions"),
                Content = new StringContent("{\n   \"model\": \"text-davinci-003\", \"prompt\": \"" + $"{prompt}\",\n    \"max_tokens\": 2000,\n    \"temperature\": 0.5\n}}")
            };
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // Response Handling
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Returns the Response Deserialized
            dynamic json = JsonConvert.DeserializeObject(responseContent);
            return json.choices[0].text;

        }


    }
}
