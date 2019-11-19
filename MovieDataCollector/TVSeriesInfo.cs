using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MovieDataCollector
{
    class TVSeriesInfo
    {

        public List<Dictionary<string, string>> EpisodeList {get; set;}
        public List<string> episodeTags { get; set; }
        public string Series_ID { get; set; }
        public string Authorization_Token { get; set; }
        public Dictionary<string, string> series { get; set; }
        public List<Dictionary<string, string>> episodes { get; set; }
        private List<string> seriesTags { get; set; }
        public TVSeriesInfo(string Token, string SeriesID)
        {
            EpisodeList = new List<Dictionary<string, string>>();
            episodeTags = new List<string>
                {
                    "id",
                    "airedSeason",
                    "airedSeasonID",
                    "airedEpisodeNumber",
                    "episodeName",
                    "firstAired",
                    "guestStars",
                    "directors",
                    "writers",
                    "overview",
                    "language",
                    "productionCode",
                    "showUrl",
                    "lastUpdated",
                    "dvdDiscid",
                    "dvdSeason",
                    "dvdEpisodeNumber",
                    "dvdChapter",
                    "absoluteNumber",
                    "filename",
                    "seriesId",
                    "lastUpdatedBy",
                    "airsAfterSeason",
                    "airsBeforeSeason",
                    "airsBeforeEpisode",
                    "imdbId",
                    "contentRating",
                    "thumbAuthor",
                    "thumbAdded",
                    "thumbWidth",
                    "thumbHeight",
                    "siteRating",
                    "siteRatingCount",
                    "isMovie"
                };
            seriesTags = new List<string>()
            {
                "id",
                "seriesId",
                "seriesName",
                "aliases",
                "season",
                "poster",
                "banner",
                "fanart",
                "status",
                "firstAired",
                "network",
                "networkId",
                "runtime",
                "language",
                "genre",

            };
            episodes = new List<Dictionary<string, string>>();
            series = new Dictionary<string, string>();

            Authorization_Token = Token;
            Series_ID = SeriesID;
            GetEpisodes(Series_ID);
            //GatherSeriesInfo();
            VerifyDictionarySeriesInfo();

        }
        private void GatherSeriesInfo(string Series_ID)
        {
            string id = Series_ID;
            string responseFromSite = "";

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.thetvdb.com/series/" + id ))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE1NzQxNzA2MzgsImlkIjoiT2xkcm95ZCBNZWRpYSBGaWxlIFJlbmFtZXIiLCJvcmlnX2lhdCI6MTU3NDA4NDIzOCwidXNlcmlkIjoxMTU5NTksInVzZXJuYW1lIjoiRHVzdGluLk9sZHJveWRAZ21haWwuY29tIn0.2ZHj400dcPxG9FEfi-LF0hWEPG6V0L-rbqp4eNt_I7xmkxPU8H6fVI_BVtRfI7gGE31mBqvTPkOhR9nz1KL-lrZcz2JCtNkQeqwfY0eTGHFlbxt3YO-2NPTgT0Va2YhQp4Jxa9yMXaAXgyal52DMfPPKlRpyff7PYedshTuRR0TVl6uDIUyi4fKM0cMi3YvunbXpB2JklC4zf1Fwr02X12cwj03OQ-3peTQMH4swXwJtmE3duyYSTWWfbrF80Widz5-kwQX7lkjOPUO-DtsuUBUuJou1ZDYDlqcDEG-PXWdX0VItI7nGF7_YjL8e9HhzwsHSmdzmNDiEPpX2_s5veg");

                    var response = httpClient.SendAsync(request).Result;
                    var result = response.Content.ReadAsStringAsync().Result;
                    responseFromSite = result.ToString();
                }
            }

            //create series dictionary
            Dictionary<string, string> series = new Dictionary<string, string>();

            for (int i = 0; i < seriesTags.Count(); i++)
            {
                if (!string.IsNullOrEmpty(Program.GeneralParser(responseFromSite, "\"" + seriesTags[i] + "\":", ",")))
                {
                    //add information to dictionary
                    series.Add(seriesTags[i], Program.GeneralParser(responseFromSite, "\"" + seriesTags[i] + "\":", ","));
                }
            }


            /*System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            string responseContent; //stores response from web request
            string URL = "https://thetvdb.com/api/" + API_Key + "/series/" + Series_ID + "/all";

            responseContent = MyWebRequest(URL);
            //Replace xml escape characters
            responseContent = responseContent.Replace("&amp;", "&");
            responseContent = responseContent.Replace("&quot;", "\"");
            responseContent = responseContent.Replace("&apos;", "'");
            responseContent = responseContent.Replace("&lt;", "<");
            responseContent = responseContent.Replace("&gt;", ">");

            ExtractEpisodes(responseContent);*/
        }
        private string MyWebRequest(string URL)
        {
            /*if (string.IsNullOrEmpty(URL)) { return ""; }

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
                if (e.ToString().Contains("400"))
                {
                    CustomMessageBox.Show("Request to TheTVDB.com returned Bad Request Error", 170, 310);
                    return "";
                }
                else if (e.ToString().Contains("401"))
                {
                    CustomMessageBox.Show("Request to TheTVDB.com returned Authorization Required Error", 170, 310);
                    return "";
                }
                else if (e.ToString().Contains("403"))
                {
                    CustomMessageBox.Show("Request to TheTVDB.com returned Forbidden Page Error", 170, 310);
                    return "";
                }
                else if (e.ToString().Contains("404"))
                {
                    CustomMessageBox.Show("Request to TheTVDB.com returned Page not found Error 404", 170, 310);
                    return "";
                }
                else if (e.ToString().Contains("408"))
                {
                    CustomMessageBox.Show("Request to TheTVDB.com returned Request Timeout Error", 170, 310);
                    return "";
                }
                else if (e.ToString().Contains("500"))
                {
                    CustomMessageBox.Show("Request to TheTVDB.com returned Internal Server Error", 170, 310);
                    return "";
                }
                else if (e.ToString().Contains("502"))
                {
                    CustomMessageBox.Show("Request to TheTVDB.com returned Bad Gateway Error", 170, 310);
                    return "";
                }
                else if (e.ToString().Contains("503"))
                {
                    CustomMessageBox.Show("Request to TheTVDB.com returned Service Temporarily Unavailable Error", 170, 310);
                    return "";
                }
                else if (e.ToString().Contains("504"))
                {
                    CustomMessageBox.Show("Request to TheTVDB.com returned Gateway Timeout Error", 170, 310);
                    return "";
                }
                else if (e.ToString().Contains("500"))
                {
                    CustomMessageBox.Show("Request to TheTVDB.com returned Internal Server Error", 170, 310);
                    return "";
                }
                CustomMessageBox.Show(e.ToString(), 300, 300);
                return "";
            }*/
            return "";
        }
        private void ExtractEpisodes(string InputString)
        {
            /*string[] tokens = InputString.Split(new[] { "</Series>", "</Episode>" }, StringSplitOptions.None);

            //use this to create a list of dictionaries containing the information for each episode and series.

            foreach (string s in tokens)
            {
                if (s.Contains("<Series>"))
                {
                    CreateSeriesDictionary(s);
                }
                else if (s.Contains("<Episode>"))
                {
                    CreateEpisodeDictionary(s);
                }
            }*/
        }
        private void CreateSeriesDictionary(string inputString)
        {
            /*for (int i = 0; i < seriesTags.Count(); i++)
            {
                if (!string.IsNullOrEmpty(Program.GeneralParser(inputString, "<" + seriesTags[i] + ">", "</" + seriesTags[i] + ">")))
                {
                    series.Add(seriesTags[i], Program.GeneralParser(inputString, "<" + seriesTags[i] + ">", "</" + seriesTags[i] + ">"));
                }
            }*/

        }
        private void CreateEpisodeDictionary(string inputString)
        {
            /*//create dictionary
            Dictionary<string, string> episodeDictionary = new Dictionary<string, string>();

            for (int i = 0; i < episodeTags.Count(); i++)
            {
                if (!string.IsNullOrEmpty(Program.GeneralParser(inputString, "<" + episodeTags[i] + ">", "</" + episodeTags[i] + ">")))
                {
                    //add information to dictionary
                    episodeDictionary.Add(episodeTags[i], Program.GeneralParser(inputString, "<" + episodeTags[i] + ">", "</" + episodeTags[i] + ">"));
                }
            }

            episodes.Add(episodeDictionary); //add dictionary to list*/
        }
        private void VerifyDictionarySeriesInfo()
        {

            for (int i = 0; i < EpisodeList.Count(); i++)
            {
                //ensure all episode keys have a value
                for (int a = 0; a < episodeTags.Count(); a++)
                {
                    //If key is missing, create key with empty string as value
                    if (!EpisodeList[i].ContainsKey(episodeTags[a]))
                    {
                        EpisodeList[i].Add(episodeTags[a], "No Info Provided by Web");
                    }
                }

                //ensure all series keys have value
                for (int a = 0; a < seriesTags.Count(); a++)
                {
                    //If key is missing, create key with empty string as value
                    if (!series.ContainsKey(seriesTags[a]))
                    {
                        series.Add(seriesTags[a], "No Info Provided by Web");
                    }
                }
            }

        }
        private void GetEpisodes(string seriesID)
        {
            string authToken = Authorization_Token;
            string responseFromSite = "";
            int pagesInResult = 1;
            StringBuilder episodesString = new StringBuilder();
            EpisodeList.Clear();

            for (int i = 1; i < pagesInResult + 1; i++)
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.thetvdb.com/series/" + seriesID + "/episodes?page=" + i))
                    {
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");
                        request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + authToken); //Authenticates to website using the token granted by them earlier...Only lasts 24 hours at a time

                        var response = httpClient.SendAsync(request).Result;
                        var result = response.Content.ReadAsStringAsync().Result;

                        responseFromSite = result.ToString();
                        pagesInResult = int.Parse(Program.GeneralParser(responseFromSite, "\"last\":", ","));
                        episodesString.Append(responseFromSite);


                    }
                }
            }

            string[] delim = { "},{" };
            string[] episodes = episodesString.ToString().Split(delim, StringSplitOptions.RemoveEmptyEntries);
            foreach (var textBlock in episodes)
            {
                //create dictionary
                Dictionary<string, string> episodeDictionary = new Dictionary<string, string>();

                for (int i = 0; i < episodeTags.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(Program.GeneralParser(textBlock, "\"" + episodeTags[i] + "\":", ",")))
                    {
                        //add information to dictionary
                        episodeDictionary.Add(episodeTags[i], Program.GeneralParser(textBlock, "\"" + episodeTags[i] + "\":", ","));
                    }
                }

                EpisodeList.Add(episodeDictionary); //add dictionary to list
            }

        }

    }
}
