using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieDataCollector
{
    class TVSeriesSearch
    {
        public string InputString { get; set; }
        public string APIKey { get; set; }
        public List<Dictionary<string, string>> SeriesList { get; set; }
        public TVSeriesSearch(string Input_String)
        {
            InputString = Input_String;
            ReturnSeriesInfo();
        }
        private void ReturnSeriesInfo()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            string responseContent;
            //https://thetvdb.com/api/GetSeries.php?seriesname=
            string URL = "https://thetvdb.com/api/GetSeries.php?seriesname=" + InputString;

            responseContent = MyWebRequest(URL);
            //Replace xml escape characters
            responseContent = responseContent.Replace("&amp;", "&");
            responseContent = responseContent.Replace("&quot;", "\"");
            responseContent = responseContent.Replace("&apos;", "'");
            responseContent = responseContent.Replace("&lt;", "<");
            responseContent = responseContent.Replace("&gt;", ">");

            BuildSeriesList(responseContent);
        }
        private string MyWebRequest(string URL)
        {
            if (string.IsNullOrEmpty(URL)) { return ""; }

            var request = System.Net.WebRequest.Create(URL) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.Accept = "application/json";
            request.ContentLength = 0;
            string responseContent;

            try
            {
                using (var response = request.GetResponse() as System.Net.HttpWebResponse)
                {
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
                return responseContent;
            }

            catch (Exception e)
            {
                if (e.ToString().Contains("404"))
                {
                    CustomMessageBox.Show("Request to TheTVDB.com returned Page not found Error 404", 170, 310);
                    return "";
                }
                CustomMessageBox.Show(e.ToString(), 300, 300);
                return "";
            }
        }
        private void BuildSeriesList(string WebResponse)
        {
            string[] tokens = WebResponse.Split(new[] { "</Series>" }, StringSplitOptions.None);

            SeriesList = new List<Dictionary<string, string>>();
            List<string> tags = new List<string>()
            { "seriesid",
                "language",
                "SeriesName",
                "Overview",
                "Network",
                "id",
                "banner",
                "IMDB_ID",
                "zap2it_id",
                "AliasNames",
                "FirstAired"
            };

            foreach (string s in tokens)
            {
                if (s.Contains("<seriesid>"))
                {
                    Dictionary<string, string> SeriesDictionary = new Dictionary<string, string>();

                    for (int i = 0; i < tags.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(GeneralParser(s, "<" + tags[i] + ">", "</" + tags[i] + ">")))
                        {
                            SeriesDictionary.Add(tags[i], GeneralParser(s, "<" + tags[i] + ">", "</" + tags[i] + ">"));
                        }
                        else //Adds dictionary key with no value if not found.
                        {
                            SeriesDictionary.Add(tags[i], "");
                        }
                    }

                    if (SeriesDictionary["SeriesName"] != "** 403: Series Not Permitted **")
                    {
                        SeriesList.Add(SeriesDictionary);
                    }

                }
            }
            tags.Clear();
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
    }
}
