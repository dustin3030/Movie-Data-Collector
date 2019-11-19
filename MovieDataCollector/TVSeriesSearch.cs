using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace MovieDataCollector
{
    class TVSeriesSearch
    {
        public string InputString { get; set; }
        public string APIKey { get; set; }
        public string Token { get; set; }
        public List<Dictionary<string, string>> SeriesList { get; set; }
        public TVSeriesSearch(string Input_String, string Authorization_Token)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            InputString = Input_String;
            Token = Authorization_Token;
            //ReturnSeriesInfo();
            SearchSeries(InputString);
        }
        private string GeneralParser(string InputString, string start, string end)
        {
            if (string.IsNullOrEmpty(InputString)) { return ""; }
            int startPosition = 0;
            int endPosition = 0;
            try
            {
                if (InputString.Contains(start) & InputString.Length > start.Length)
                {
                    startPosition = InputString.IndexOf(start) + start.Length;
                }
                else { return ""; }

                if (InputString.Contains(end) & InputString.Length > end.Length)
                {
                    endPosition = InputString.IndexOf(end, startPosition);
                }
                else { return ""; }

                if (startPosition == -1 || endPosition == -1) { return ""; }

                if (startPosition >= endPosition) { return ""; }

                if (InputString.Length - startPosition > endPosition - startPosition)
                {
                    return InputString.Substring(startPosition, endPosition - startPosition);
                }
                else { return ""; }
            }
            catch
            {
                return "";
            }
        }
        private void SearchSeries(string SeriesName)
        {
            string series = SeriesName;
            string authToken = Token;
            string responseFromSite = "";
            SeriesList = new List<Dictionary<string, string>>();

            List<string> tags = new List<string>()
            {
                "seriesName",
                "aliases",
                "banner",
                "firstAired",
                "id",
                "network",
                "overview",
                "slug",
                "status"
            };

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.thetvdb.com/search/series?name=" + series))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json"); //Specifies return as JSON
                    request.Headers.TryAddWithoutValidation("Accept-Language", "eng"); //Specifies English Language
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + authToken); //Authenticates to website using the token granted by them earlier...Only lasts 24 hours at a time

                    var response = httpClient.SendAsync(request).Result;
                    var result = response.Content.ReadAsStringAsync().Result;

                    responseFromSite = result.ToString();

                }
            }

            string[] delim = { "},{" };
            string[] seriesTokens = responseFromSite.ToString().Split(delim, StringSplitOptions.RemoveEmptyEntries);
            foreach (var textBlock in seriesTokens)
            {
                //create dictionary
                Dictionary<string, string> SeriesDictionary = new Dictionary<string, string>();

                for (int i = 0; i < tags.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(GeneralParser(textBlock, "\"" + tags[i] + "\":", ",").Replace("\"", "")))
                    {
                        //add information to dictionary
                        SeriesDictionary.Add(tags[i], GeneralParser(textBlock, "\"" + tags[i] + "\":", ",").Replace("\"",""));
                    }
                }

                SeriesList.Add(SeriesDictionary); //add dictionary to list
            }
            tags.Clear();
        }
    }
}
