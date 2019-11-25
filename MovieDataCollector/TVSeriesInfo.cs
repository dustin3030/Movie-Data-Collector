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
        public List<string> EpisodeTags { get; set; }
        public string Series_ID { get; set; }
        public string Authorization_Token { get; set; }
        public string API_Version { get; set; }
        public Dictionary<string, string> Series { get; set; }
        public List<Dictionary<string, string>> Episodes { get; set; }
        private List<string> SeriesTags { get; set; }
        public TVSeriesInfo(string Token, string APIVersion, string SeriesID)
        {
            EpisodeList = new List<Dictionary<string, string>>();
            Series = new Dictionary<string, string>();
            EpisodeTags = new List<string>
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
                    "SeriesId",
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
            SeriesTags = new List<string>()
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
            Episodes = new List<Dictionary<string, string>>();
            Series = new Dictionary<string, string>();

            API_Version = APIVersion;
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
                    request.Headers.TryAddWithoutValidation("Accept", "application/vnd.thetvdb.v" + API_Version); //Set Version Number
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + Authorization_Token);

                    var response = httpClient.SendAsync(request).Result;
                    var result = response.Content.ReadAsStringAsync().Result;

                    responseFromSite = result.ToString().Replace("{\"data\":{", "").Replace("}", "");
                }
            }


            //populate Series dictionary

            for (int i = 0; i < SeriesTags.Count(); i++)
            {
                if (!string.IsNullOrEmpty(Program.GeneralParser(responseFromSite, "\"" + SeriesTags[i] + "\":", ",")))
                {
                    if (SeriesTags[i] == "genre" || SeriesTags[i] == "aliases")
                    {
                        //genre & aliases potentially have multiple entries and uses a different parse filter.
                        Series.Add(SeriesTags[i], Program.GeneralParser(responseFromSite, "\"" + SeriesTags[i] + "\":[", "],").Replace("\"", "").Replace("]", "").Replace("[", ""));
                    }
                    else if (SeriesTags[i] == "seriesName")
                    {
                        Series.Add(SeriesTags[i], Program.GeneralParser(responseFromSite, "\"" + SeriesTags[i] + "\":", ",").Replace("\"", "").Replace("]", "").Replace("[", ""));
                    }
                    else
                    {
                        //add information to dictionary
                        Series.Add(SeriesTags[i], Program.GeneralParser(responseFromSite, "\"" + SeriesTags[i] + "\":", ",").Replace("\"", "").Replace("]", "").Replace("[", ""));
                    }

                }
            }



        }
        private void VerifyDictionarySeriesInfo()
        {

            for (int i = 0; i < EpisodeList.Count(); i++)
            {
                //ensure all episode keys have a value
                for (int a = 0; a < EpisodeTags.Count(); a++)
                {
                    //If key is missing, create key with value "No Info Provided by Web"
                    if (!EpisodeList[i].ContainsKey(EpisodeTags[a]))
                    {
                        //If no entry exists in dictionary add one so there are no null values.
                        EpisodeList[i].Add(EpisodeTags[a], "No Info Provided by Web");
                    }
                    if (EpisodeList[i][EpisodeTags[a]] == "null" || string.IsNullOrEmpty(EpisodeList[i][EpisodeTags[a]]))
                    {
                        //change value to no info provided... for null entries
                        EpisodeList[i][EpisodeTags[a]] = "No Info Provided by Web";
                    }
                }

                //ensure all Series keys have value
                for (int a = 0; a < SeriesTags.Count(); a++)
                {
                    //If key is missing, create key with empty string as value
                    if (!Series.ContainsKey(SeriesTags[a]))
                    {
                        Series.Add(SeriesTags[a], "No Info Provided by Web");
                    }
                }
            }

        }
        private void GetEpisodes(string SeriesID)
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
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.thetvdb.com/series/" + SeriesID + "/episodes?page=" + i))
                    {
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");
                        request.Headers.TryAddWithoutValidation("Accept-Language", "eng");
                        request.Headers.TryAddWithoutValidation("Accept", "application/vnd.thetvdb.v" + API_Version); //Set Version Number
                        request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + authToken); //Authenticates to website using the token granted by them earlier...Only lasts 24 hours at a time

                        var response = httpClient.SendAsync(request).Result;
                        var result = response.Content.ReadAsStringAsync().Result;

                        responseFromSite = result.ToString().Replace("{\"data\":", "").Replace("\"data\":[","");
                        pagesInResult = int.Parse(Program.GeneralParser(responseFromSite, "\"last\":", ","));
                        episodesString.Append(responseFromSite);


                    }
                }
            }

            //"data":[
            string[] delim = { "},{" };
            string[] episodes = episodesString.ToString().Split(delim, StringSplitOptions.RemoveEmptyEntries);
            foreach (var textBlock in episodes)
            {
                //create dictionary
                Dictionary<string, string> episodeDictionary = new Dictionary<string, string>();
                string modifiedTextBlock = "";
                modifiedTextBlock = textBlock + ",";

                if(modifiedTextBlock.Contains("\"id\":"))
                {
                    //Add check to see if textblock holds any tags
                    for (int i = 0; i < EpisodeTags.Count(); i++)
                    {
                        if (!string.IsNullOrEmpty(Program.GeneralParser(modifiedTextBlock, "\"" + EpisodeTags[i] + "\":", ",\"")))
                        {
                            if (modifiedTextBlock.Contains(EpisodeTags[i]))
                            {
                                //select case
                                switch (EpisodeTags[i])
                                {
                                    case "language":
                                        //add information to dictionary
                                        episodeDictionary.Add(EpisodeTags[i], Program.GeneralParser(modifiedTextBlock, "\"" + EpisodeTags[i] + "\":{\"episodeName\":", ",\"").Replace("\"", "").Replace("[", "").Replace("]", ""));
                                        break;
                                    case "guestStars":
                                        episodeDictionary.Add(EpisodeTags[i], Program.GeneralParser(modifiedTextBlock, "\"" + EpisodeTags[i] + "\":[", "],\"").Replace("\"", "").Replace("[", "").Replace("]", ""));
                                        break;
                                    case "directors":
                                        episodeDictionary.Add(EpisodeTags[i], Program.GeneralParser(modifiedTextBlock, "\"" + EpisodeTags[i] + "\":[", "],\"").Replace("\"", "").Replace("[", "").Replace("]", ""));
                                        break;
                                    case "writers":
                                        episodeDictionary.Add(EpisodeTags[i], Program.GeneralParser(modifiedTextBlock, "\"" + EpisodeTags[i] + "\":[", "],\"").Replace("\"", "").Replace("[", "").Replace("]", ""));
                                        break;
                                    default:
                                        //add information to dictionary
                                        episodeDictionary.Add(EpisodeTags[i], Program.GeneralParser(modifiedTextBlock, "\"" + EpisodeTags[i] + "\":", ",\"").Replace("\"", "").Replace("[", "").Replace("]", ""));
                                        break;
                                }

                            }

                        }
                    }

                    EpisodeList.Add(episodeDictionary); //add dictionary to list
                }

            }

        }

    }
}
