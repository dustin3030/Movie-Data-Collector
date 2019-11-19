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
            series = new Dictionary<string, string>();
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
                "overview,",
                "lastUpdated",
                "airsDayOfWeek",
                "airsTime",
                "rating",
                "imdbId",
                "zap2itId",
                "added",
                "addedBy",
                "siteRating",
                "siteRatingCount",
                "slug"
            };
            episodes = new List<Dictionary<string, string>>();
            series = new Dictionary<string, string>();

            Authorization_Token = Token;
            Series_ID = SeriesID;
            GetEpisodes(Series_ID);
            GatherSeriesInfo(Series_ID);
            VerifyDictionarySeriesInfo();

        }
        private void GatherSeriesInfo(string Series_ID)
        {
            string responseFromSite = "";
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.thetvdb.com/series/" + Series_ID))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Accept-Language", "eng");
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + Authorization_Token);

                    var response = httpClient.SendAsync(request).Result;
                    var result = response.Content.ReadAsStringAsync().Result;

                    responseFromSite = result.ToString().Replace("{\"data\":{", "").Replace("}","");
                }
            }


            //populate series dictionary

            for (int i = 0; i < seriesTags.Count(); i++)
            {
                if (!string.IsNullOrEmpty(Program.GeneralParser(responseFromSite, "\"" + seriesTags[i] + "\":", ",")))
                {
                    if(seriesTags[i] == "genre" || seriesTags[i] == "aliases")
                    {
                        //genre & aliases potentially have multiple entries and uses a different parse filter.
                        series.Add(seriesTags[i], Program.GeneralParser(responseFromSite, "\"" + seriesTags[i] + "\":[", "],").Replace("\"", "").Replace("]", "").Replace("[", ""));
                    }
                    else
                    {
                        //add information to dictionary
                        series.Add(seriesTags[i], Program.GeneralParser(responseFromSite, "\"" + seriesTags[i] + "\":", ",").Replace("\"", "").Replace("]", "").Replace("[", ""));
                    }
                    
                }
            }



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
        private void VerifyDictionarySeriesInfo()
        {

            for (int i = 0; i < EpisodeList.Count(); i++)
            {
                //ensure all episode keys have a value
                for (int a = 0; a < episodeTags.Count(); a++)
                {
                    //If key is missing, create key with value "No Info Provided by Web"
                    if (!EpisodeList[i].ContainsKey(episodeTags[a]))
                    {
                        //If no entry exists in dictionary add one so there are no null values.
                        EpisodeList[i].Add(episodeTags[a], "No Info Provided by Web");
                    }
                    if (EpisodeList[i][episodeTags[a]] == "null" || string.IsNullOrEmpty(EpisodeList[i][episodeTags[a]]))
                    {
                        //change value to no info provided... for null entries
                        EpisodeList[i][episodeTags[a]] = "No Info Provided by Web";
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

                        responseFromSite = result.ToString().Replace("{\"data\":", "");
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
                string modifiedTextBlock = "";
                modifiedTextBlock = textBlock + ",";
                for (int i = 0; i < episodeTags.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(Program.GeneralParser(modifiedTextBlock, "\"" + episodeTags[i] + "\":", ",")))
                    {
                        //select case
                        switch (episodeTags[i])
                        {
                            case "language":
                                //add information to dictionary
                                episodeDictionary.Add(episodeTags[i], Program.GeneralParser(modifiedTextBlock, "\"" + episodeTags[i] + "\":{\"episodeName\":", ",").Replace("\"", "").Replace("[", "").Replace("]", ""));
                                break;
                            case "guestStars":
                                episodeDictionary.Add(episodeTags[i], Program.GeneralParser(modifiedTextBlock, "\"" + episodeTags[i] + "\":[", "]").Replace("\"", "").Replace("[", "").Replace("]", ""));
                                break;
                            case "directors":
                                episodeDictionary.Add(episodeTags[i], Program.GeneralParser(modifiedTextBlock, "\"" + episodeTags[i] + "\":[", "]").Replace("\"", "").Replace("[", "").Replace("]", ""));
                                break;
                            case "writers":
                                episodeDictionary.Add(episodeTags[i], Program.GeneralParser(modifiedTextBlock, "\"" + episodeTags[i] + "\":[", "]").Replace("\"", "").Replace("[", "").Replace("]", ""));
                                break;
                            default:
                                //add information to dictionary
                                episodeDictionary.Add(episodeTags[i], Program.GeneralParser(modifiedTextBlock, "\"" + episodeTags[i] + "\":", ",").Replace("\"", "").Replace("[", "").Replace("]", ""));
                                break;
                        }
                        
                    }

                }

                EpisodeList.Add(episodeDictionary); //add dictionary to list
            }

        }

    }
}
