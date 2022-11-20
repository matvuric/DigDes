using System.Collections.Concurrent;

namespace Api.Services
{
    public class DdosGuard
    {
        public class TooManyRequestsException : Exception
        {

        }

        public ConcurrentDictionary<string, int> RequestControlDictionary { get; set; } = new();

        public void CheckRequest(string? token)
        {
            if (token == null)
            {
                return;
            }

            var dateTimeNow = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

            var key = $"{token}_{dateTimeNow}";

            if (RequestControlDictionary.ContainsKey(key))
            {
                var requests = RequestControlDictionary.TryGetValue(key, out var request) ? request : 0;

                if (requests >= 9)
                {
                    throw new TooManyRequestsException();
                }

                var newRequest = requests + 1;
                RequestControlDictionary.TryUpdate(key, newRequest, requests);
            }

            RequestControlDictionary.TryAdd(key, 0);
        }
    }
}

/*var jq = document.createElement('script');
jq.src = "https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js";
document.getElementsByTagName('head')[0].appendChild(jq);

for (i = 0; i < 30; i++)
{
    $.ajax("https://localhost:7266/api/User/GetCurrentUser", {
        headers:
        {
            "Authorization": "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoic3RyaW5nIiwiaWQiOiJlYmU3Y2FmNi05YWQ2LTRmM2YtYmY3OS02MjFkZTc1ZDFhMTciLCJzZXNzaW9uSWQiOiI3NjhhYzViZi01NDNhLTRjNjItYjYyNS0xZWVjOWE3NGYwYTgiLCJuYmYiOjE2Njg5NjE3MTQsImV4cCI6MTY2ODk3MDcxNCwiaXNzIjoiRGlnRGVzIiwiYXVkIjoibW9iaWxlQXBwIn0.ZbLfL90_lJ2E_gyC8jdnE7JfGXHlL6VZfMr1MsCtTvw"}
    }).done(function(res) { console.log(res); })}*/
