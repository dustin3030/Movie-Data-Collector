using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDataCollector
{
    class TVSeriesInfo
    {
        public string API_Key { get; set; }
        public string Series_ID { get; set; }
        public Dictionary<string, string> series { get; set; }
        public List<Dictionary<string, string>> episodes { get; set; }
        public TVSeriesInfo(string APIKey, string SeriesID)
        {
            episodes = new List<Dictionary<string, string>>();
            series = new Dictionary<string, string>();

            API_Key = APIKey;
            Series_ID = SeriesID;
            GatherSeriesInfo();
        }
        private void GatherSeriesInfo()
        {

            string responseContent; //stores response from web request
            string URL = "http://thetvdb.com/api/" + API_Key + "/series/" + Series_ID + "/all";

            responseContent = MyWebRequest(URL);
            //Replace xml escape characters
            responseContent = responseContent.Replace("&amp;", "&");
            responseContent = responseContent.Replace("&quot;", "\"");
            responseContent = responseContent.Replace("&apos;", "'");
            responseContent = responseContent.Replace("&lt;", "<");
            responseContent = responseContent.Replace("&gt;", ">");

            ExtractEpisodes(responseContent);
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
            }
        }
        private void ExtractEpisodes(string InputString)
        {
            string[] tokens = InputString.Split(new[] { "</Series>", "</Episode>" }, StringSplitOptions.None);

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
            }
        }
        private void CreateSeriesDictionary(string inputString)
        {
            List<string> tags = new List<string>()
            { "Actors",
                "ContentRating",
                "Airs_DayOfWeek",
                "Airs_Time",
                "FirstAired",
                "Genre",
                "IMDB_ID",
                "Language",
                "Network",
                "NetworkID",
                "Overview",
                "Rating",
                "RatingCount",
                "Runtime",
                "SeriesID",
                "SeriesName",
                "Status",
                "banner",
                "fanart",
                "poster",
            };

            for (int i = 0; i < tags.Count(); i++)
            {
                if (!string.IsNullOrEmpty(Program.GeneralParser(inputString, "<" + tags[i] + ">", "</" + tags[i] + ">")))
                {
                    series.Add(tags[i], Program.GeneralParser(inputString, "<" + tags[i] + ">", "</" + tags[i] + ">"));
                }
            }

            tags.Clear();

        }
        private void CreateEpisodeDictionary(string inputString)
        {
            //create dictionary
            Dictionary<string, string> episodeDictionary = new Dictionary<string, string>();

            List<string> tags = new List<string>
            { "id",
                "Combined_episodenumber",
                "Combined_season",
                "DVD_chapter",
                "DVD_discid",
                "DVD_episodenumber",
                "DVD_season",
                "Director",
                "EpImgFlag",
                "EpisodeName",
                "EpisodeNumber",
                "FirstAired",
                "GuestStars",
                "IMDB_ID",
                "Language",
                "Overview",
                "ProductionCode",
                "Rating",
                "RatingCount",
                "SeasonNumber",
                "Writer",
                "absolute_number",
                "airsafter_season",
                "airsbefore_episode",
                "airsbefore_season",
                "filename",
                "lastupdated",
                "seasonid",
                "seriesid",
            };

            for (int i = 0; i < tags.Count(); i++)
            {
                if (!string.IsNullOrEmpty(Program.GeneralParser(inputString, "<" + tags[i] + ">", "</" + tags[i] + ">")))
                {
                    //add information to dictionary
                    episodeDictionary.Add(tags[i], Program.GeneralParser(inputString, "<" + tags[i] + ">", "</" + tags[i] + ">"));
                }
            }

            episodes.Add(episodeDictionary); //add dictionary to list
            tags.Clear();
        }
    }
}
