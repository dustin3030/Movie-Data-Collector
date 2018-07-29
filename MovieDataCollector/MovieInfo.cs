using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieDataCollector
{
    

    public class MovieInfo
    {
        //Contains all Lists
        public Dictionary<string, List<string>> ListProperties { get; set; }
        //Contains all statics
        public Dictionary<string,string> StaticProperties { get; set; }

        List<string> ListDictKeyList = new List<string>()
        {"ActorName",
            "ActorRoles",
            "ActorImages",
            "ActorNames",
            "Backdrops",
            "CrewJobs",
            "CrewNames",
            "Errors",
            "GenreList",
            "Posters",
            "StudioList",
            "USTitles"
        };
        List<string> StaticKeyList = new List<string>()
        {   "Adult", //Flags Adult Content
            "API_Key",
            "BackDropPath",
            "Budget",
            "Collection",
            "Director",
            "FormattedTitle",
            "Genres",
            "IMDB_ID",
            "TMDB_ID",
            "MPAA_Rating",
            "OriginalTitle",
            "Plot",
            "PosterPath",
            "Popularity",
            "ProductionCountry",
            "ReleaseDate",
            "ReleaseYear",
            "Revenue",
            "RunTime",
            "Studio",
            "Title",
            "Tag_Line",
            "VoteCount",
            "VoteAverage"
        };

                                          /// <summary>
                                          /// Constructor - Accepts IMDB Number
                                          /// </summary>
        public MovieInfo(string TMDBID, string APIKey)
        {
            ListProperties = new Dictionary<string, List<string>>(); //Instantiate Dictionary
            for (int i = 0; i < ListDictKeyList.Count ; i++)
            {
                ListProperties[ListDictKeyList[i]] = new List<string>();
            }

            StaticProperties = new Dictionary<string, string>(); //Instantiate Dictionary
            for (int i = 0; i < StaticKeyList.Count; i++)
            {
                StaticProperties[StaticKeyList[i]] = "";
            }

            //Fill dictionary with passed in values
            StaticProperties["TMDB_ID"] = TMDBID;
            StaticProperties["API_Key"] = APIKey;

        }
        /// <summary>
        /// Returns MPAA rating for movies
        /// </summary>
        /// <param name="StaticProperties["TMDB_ID"]"></param>
        public void GetRating()
        {
            string responseContent;
            string URL = "https://api.themoviedb.org/3/movie/" + StaticProperties["TMDB_ID"] + "/releases?api_key=" + StaticProperties["API_Key"];
            responseContent = MyWebRequest(URL,"getRating");

            //With a null response there is nothing to parse so escape the method on set StaticProperties["MPAA_Rating"] to Error
            if (string.IsNullOrEmpty(responseContent)) { StaticProperties["MPAA_Rating"] = "Error"; return; }

            //Split reponse string into array to prepare for parsing
            char[] delim = { '}' };
            string[] ratingTokens = responseContent.Split(delim);

            //Parse out ratings from responseContent
            foreach (string s in ratingTokens)
            {
                if (s.Contains(":\"US\""))
                {
                    StaticProperties["MPAA_Rating"] = GeneralParser(s, "\"certification\":\"", "\",\"", "\"certification\":null");
                }
            }

            if (string.IsNullOrEmpty(StaticProperties["MPAA_Rating"])) { StaticProperties["MPAA_Rating"] = "NR"; } //Fixes errors related with no rating being listed
        }
        /// <summary>
        /// Builds list of Currently Available Generes from the website themoviedb.org
        /// </summary>
        private string BuildGenreList(string responseContent)
        {
            string parsedGenres;
            string tempString;

            //split string and search instead of matching to list, this will also remove necessity for genres list.
            parsedGenres = GeneralParser(responseContent, "\"genres\":[", "]", "\"genres\":null");
            if (string.IsNullOrEmpty(parsedGenres)) { return ""; }


            char[] delim = { '}' }; //character array holds deliminating values for splitting string
            string[] Tokens = parsedGenres.Split(delim);

            StringBuilder genreBuilder = new StringBuilder();
            foreach (string s in Tokens)
            {
                if (s.Contains("name\":") && string.IsNullOrEmpty(genreBuilder.ToString()))
                {
                    genreBuilder.Append(GeneralParser(s, "name\":\"", "\"", "name\":null"));
                    ListProperties["GenreList"].Add(GeneralParser(s, "name\":\"", "\"", "name\":null"));
                }
                else if (s.Contains("name\":\""))
                {
                    tempString = (GeneralParser(s, "name\":\"", "\"", "name\":null"));
                    if (!string.IsNullOrEmpty(tempString))
                    {
                        genreBuilder.Append(", " + tempString);
                        ListProperties["GenreList"].Add(tempString);
                    }
                }
            }

            return genreBuilder.ToString();
        }
        /// <summary>
        /// Returns basic info from themoviedb.org
        /// Adult, OriginalTitle, Title, Plot, Popularity, Release Date, 
        /// Revenue, Run time, Tag Line, Collection, Genres, Studio, 
        /// Production Country, Vote Average, Vote Count, Budget,
        /// Poster Path, Background Path
        /// </summary>
        /// <param name="StaticProperties["TMDB_ID"]"></param>
        public void GetBasicInfo()
        {

            string responseContent;
            string URL = "https://api.themoviedb.org/3/movie/" + StaticProperties["TMDB_ID"] + "?api_key=" + StaticProperties["API_Key"];
            responseContent = MyWebRequest(URL,"getBasicInfo");

            //set defaults if response is null
            if (string.IsNullOrEmpty(responseContent))
            {
                StaticProperties["Adult"] = "False";
                StaticProperties["OriginalTitle"] = "Title Not Found";
                StaticProperties["Title"] = "Title Not Found";
                StaticProperties["ReleaseDate"] = "";
                StaticProperties["ReleaseYear"] = "Year Not Found";
                StaticProperties["FormattedTitle"] = "";
                StaticProperties["Plot"] = "Plot Not Found";
                StaticProperties["IMDB_ID"] = "";
                StaticProperties["TMDB_ID"] = "";
                StaticProperties["Popularity"] = "";
                StaticProperties["Revenue"] = "";
                StaticProperties["RunTime"] = "0";
                StaticProperties["Tag_Line"] = "";
                StaticProperties["Collection"] = "Not Found";
                StaticProperties["Genres"] = "Not Found";
                StaticProperties["Studio"] = "";
                StaticProperties["ProductionCountry"] = "";
                StaticProperties["VoteAverage"] = "";
                StaticProperties["VoteCount"] = "";
                StaticProperties["Budget"] = "";
                StaticProperties["PosterPath"] = "";
                StaticProperties["BackDropPath"] = "";
                return;
            }

            //Fix malformed Json / HTML code
            if (responseContent.Contains(@"\n")) { responseContent = responseContent.Replace(@"\n", ""); } //Remove NewLine characters
            if (responseContent.Contains(@"\r")) { responseContent = responseContent.Replace(@"\r", ""); } //Remove Return Characers
            if (responseContent.Contains(@"\t")) { responseContent = responseContent.Replace(@"\t", ""); } //Remove Tab Characters

            //Default to false
            string isAdult;
            isAdult = GeneralParser(responseContent, "adult\":", ",", "adult\":null");
            if (isAdult.Contains("false") || string.IsNullOrEmpty(isAdult)) { StaticProperties["Adult"] = "False"; }
            else if (isAdult.Contains("true")) { StaticProperties["Adult"] = "True"; }
            else { StaticProperties["Adult"] = "False"; }

            StaticProperties["OriginalTitle"] = GeneralParser(responseContent, "original_title\":\"", "\",", "original_title\":null");
            StaticProperties["Title"] = GeneralParser(responseContent, "\"title\":\"", "\",", "\"title\":null");
            StaticProperties["ReleaseDate"] = GeneralParser(responseContent, "release_date\":\"", "\",", "release_date\":null");
            StaticProperties["IMDB_ID"] = GeneralParser(responseContent, "\"imdb_id\":\"", "\",", "imdb_id\":null");

            //Set ReleaseYear to an empty string as default
            if (!string.IsNullOrEmpty(StaticProperties["ReleaseDate"]) && StaticProperties["ReleaseDate"].Length > 4)
            {
                StaticProperties["ReleaseYear"] = StaticProperties["ReleaseDate"].Remove(4, StaticProperties["ReleaseDate"].Length - 4);
            }
            else { StaticProperties["ReleaseYear"] = ""; }

            //Get valid Title by removing invalid file name characters
            StaticProperties["FormattedTitle"] = ValidTitle(StaticProperties["Title"] + " (" + StaticProperties["ReleaseYear"] + ")");

            StaticProperties["Plot"] = GeneralParser(responseContent, "overview\":\"", "\",\"popularity\"", "overview\":null");
            if (StaticProperties["Plot"].Contains("\\r")) { StaticProperties["Plot"] = StaticProperties["Plot"].Replace("\\r", ""); }
            if (StaticProperties["Plot"].Contains("\\n")) { StaticProperties["Plot"] = StaticProperties["Plot"].Replace("\\n", ""); }
            if (StaticProperties["Plot"].Contains("\\")) { StaticProperties["Plot"] = StaticProperties["Plot"].Replace("\\", ""); }

            StaticProperties["Popularity"] = GeneralParser(responseContent, "popularity\":", ",", "popularity\":null");
            StaticProperties["Revenue"] = GeneralParser(responseContent, "revenue\":", ",", "revenue\":null");
            StaticProperties["RunTime"] = GeneralParser(responseContent, "runtime\":", ",", "runtime\":null");
            StaticProperties["Tag_Line"] = GeneralParser(responseContent, "tagline\":\"", "\",", "tagline\":null");

            //Removes backslash characters from the tagline.
            if (StaticProperties["Tag_Line"].Contains(@"\")) { StaticProperties["Tag_Line"] = StaticProperties["Tag_Line"].Replace(@"\", ""); }

            StaticProperties["Collection"] = GeneralParser(responseContent, "belongs_to_collection\":{\"id\":", "\",", "belongs_to_collection\":null");
            if (!string.IsNullOrEmpty(StaticProperties["Collection"])) { StaticProperties["Collection"] = GeneralParser(StaticProperties["Collection"], "name\":\"", "Collection", "name\":null"); }
            if (!string.IsNullOrEmpty(StaticProperties["Collection"])) { StaticProperties["Collection"] = StaticProperties["Collection"].Trim() + " Collection"; }

            StaticProperties["Genres"] = BuildGenreList(responseContent);
            StaticProperties["Studio"] = ProductionCompanies(responseContent);
            StaticProperties["ProductionCountry"] = ProductionCountries(responseContent);
            StaticProperties["VoteAverage"] = GeneralParser(responseContent, "vote_average\":", ",", "vote_average\":null");
            StaticProperties["VoteCount"] = GeneralParser(responseContent, "vote_count\":", "}", "vote_count\":null");
            StaticProperties["Budget"] = GeneralParser(responseContent, "budget\":", ",", "budget\":null");
        }
        /// <summary>
        /// General Parser - string to substring
        /// </summary>
        /// <param name="InputString"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private string GeneralParser(string InputString, string start, string end, string nullCheck = "")
        {
            if (string.IsNullOrEmpty(InputString)) { return ""; }
            if (!string.IsNullOrEmpty(nullCheck) && InputString.Contains(nullCheck)) { return ""; }

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
        /// <summary>
        /// Splits out production companies and lists them comma separated
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        private string ProductionCompanies(string InputString)
        {
            if (string.IsNullOrEmpty(InputString)) { return ""; }

            string studios = "";
            StringBuilder returnString = new StringBuilder();

            //Parse out production Companies
            studios = GeneralParser(InputString, "\"production_companies\":[", "]", "\"production_companies\":null");
            if (string.IsNullOrEmpty(studios)) { return ""; }

            //Split reponse string into array to prepare for parsing
            char[] delim = { '}' }; //character array holds deliminating values for splitting string
            string[] Tokens = studios.Split(delim);

            //Parse out genres from responseContent and add to list
            foreach (string s in Tokens)
            {
                if (s.Contains("\"name\":\"") && string.IsNullOrEmpty(returnString.ToString()))
                {
                    returnString.Append(GeneralParser(s, "\"name\":\"", "\",", "\"name\":null"));
                    ListProperties["StudioList"].Add(GeneralParser(s, "\"name\":\"", "\",", "\"name\":null"));
                }
                else if (s.Contains("\"name\":\""))
                {
                    returnString.Append(", " + GeneralParser(s, "\"name\":\"", "\",", "\"name\":null"));
                    ListProperties["StudioList"].Add(GeneralParser(s, "\"name\":\"", "\",", "\"name\":null"));
                }
            }

            return returnString.ToString();
        }
        /// <summary>
        /// Splits out production countries and lists them comma separated
        /// </summary>
        /// <param name="InputString"></param>
        /// <returns></returns>
        private string ProductionCountries(string InputString)
        {
            if (string.IsNullOrEmpty(InputString)) { return ""; }

            string countries = "";
            StringBuilder returnString = new StringBuilder();

            //Parse out production Companies
            countries = GeneralParser(InputString, "\"production_countries\":[", "]", "\"production_countries\":null");
            if (string.IsNullOrEmpty(countries)) { return ""; }


            //Split reponse string into array to prepare for parsing
            char[] delim = { ',' }; //character array holds deliminating values for splitting string
            string[] Tokens = countries.Split(delim);

            //Parse out genres from responseContent and add to list
            foreach (string s in Tokens)
            {
                if (s.Contains("\"name\":") && string.IsNullOrEmpty(returnString.ToString()))
                {
                    returnString.Append(GeneralParser(s, "\"name\":\"", "\"}", "\"name\":null"));
                }
                else if (s.Contains("\"name\":\""))
                {
                    returnString.Append(", " + GeneralParser(s, "\"name\":\"", "\"}", "\"name\":null"));
                }
            }

            return returnString.ToString();
        }
        /// <summary>
        /// Retrieves cast and crew information for the film
        /// </summary>
        public void GetCredits()
        {

            string castJson; //holds split string with cast information
            string crewJson; //holds split string with crew information

            //Creating Web Request
            string responseContent;
            string URL = "https://api.themoviedb.org/3/movie/" + StaticProperties["TMDB_ID"] + "/credits?api_key=" + StaticProperties["API_Key"] + "&language=en&include_image_language=en,null";
            responseContent = MyWebRequest(URL,"getCredits");

            //Create castJson string using parser
            castJson = GeneralParser(responseContent, "\"cast\":[", "]", "\"cast\":null");
            //Create crewJson string using parser
            crewJson = GeneralParser(responseContent, "\"crew\":[", "]", "\"crew\":null");

            //Tokenize output for Cast Information
            //Split reponse string into array to prepare for parsing
            char[] delim = { '{' }; //character array holds deliminating values for splitting string
            string[] castTokens = castJson.Split(delim);
            string[] crewTokens = crewJson.Split(delim);
            //Parse out genres from responseContent and add to list
            foreach (string s in castTokens)
            {
                if (s.Contains("\"name\":"))
                {
                    ListProperties["ActorNames"].Add(GeneralParser(s, "\"name\":\"", "\",\"", "\"name\":null"));
                    ListProperties["ActorRoles"].Add(GeneralParser(s, "\"character\":\"", "\",\"", "\"character\":null"));
                    if (s.Contains(".jpg"))
                    {
                        ListProperties["ActorImages"].Add("https://image.tmdb.org/t/p/original/" + GeneralParser(s, "\"profile_path\":\"/", "\"}", "\"profile_path\":null"));
                    }
                    else { ListProperties["ActorImages"].Add("Null"); }
                }
            }

            foreach (string s in crewTokens)
            {
                if (s.Contains("\"name\":") && s.Contains("Director"))
                {
                    StaticProperties["Director"] = GeneralParser(s, "\"name\":\"", "\",\"", "\"name\":null"); //Pulls Directors Name from the string
                    ListProperties["CrewNames"].Add(StaticProperties["Director"]); //Returns Directors name to the CrewNames list
                    ListProperties["CrewJobs"].Add(StaticProperties["Director"]); //Lists Directors Job as Director
                }
                else if (s.Contains("\"name\":"))
                {
                    ListProperties["CrewNames"].Add(GeneralParser(s, "\"name\":\"", "\",\"", "\"name\":null"));
                    ListProperties["CrewJobs"].Add(GeneralParser(s, "\"job\":\"", "\",\"", "\"job\":null"));
                }
            }

        }
        /// <summary>
        /// Removes invalid filename characters from the title \ / : * ? " < > |
        /// </summary>
        /// <param name="unformattedTitle"></param>
        /// <returns></returns>
        private string ValidTitle(string unformattedTitle)
        {
            if (string.IsNullOrEmpty(unformattedTitle)) { return ""; }

            string charsToRemove = @"\/:*?<>|"; //\ / : * ? " < > |
            unformattedTitle = unformattedTitle.Replace("\"", ""); //Removes quotes
            string pattern = string.Format("[{0}]", Regex.Escape(charsToRemove)); //Creates removal pattern
            return Regex.Replace(unformattedTitle, pattern, ""); //Removes the rest and returns the string
        }
        /// <summary>
        /// Creates list of titles for the film unique to the US only
        /// </summary>
        /// <param name="StaticProperties["TMDB_ID"]"></param>
        public void GetUSTitles()
        {
            ListProperties["USTitles"] = new List<string>(); //instantiates list
            string responseContent;
            string URL = "https://api.themoviedb.org/3/movie/" + StaticProperties["TMDB_ID"] + "/alternative_titles?api_key=" + StaticProperties["API_Key"];
            responseContent = MyWebRequest(URL, "GetUSTitles");
            if (string.IsNullOrEmpty(responseContent)) { return; }

            //Split reponse string into array to prepare for parsing
            char[] delim = { '{' }; //character array holds deliminating values for splitting string
            string[] Tokens = responseContent.Split(delim);
            //Parse out genres from responseContent and add to list
            foreach (string s in Tokens)
            {
                if (s.Contains("\":\"US\""))
                {
                    ListProperties["USTitles"].Add(GeneralParser(s, "\"title\":\"", "\",\"type\":", "\"title\":null"));
                }
                if (s.Contains("\":\"en\""))
                {
                    ListProperties["USTitles"].Add(GeneralParser(s, "\"title\":\"", "\",\"type\":", "\"title\":null"));
                }
            }
        }
        /// <summary>
        /// Gathers images for US and region coded images
        /// </summary>
        public void GetFilmImages()
        {
            string backdrops = ""; //string to hold the backdrop portion of the response
            string posters = ""; //string to hold the poster portion of the string

            string responseContent;
            //string URL = "https://api.themoviedb.org/3/movie/" + StaticProperties["TMDB_ID"] + "/images?StaticProperties["API_Key"]=" + APIKey;
            string URL = "https://api.themoviedb.org/3/movie/" + StaticProperties["TMDB_ID"] + "/images?api_key=" + StaticProperties["API_Key"] + "&language=en&include_image_language=en,null";
            responseContent = MyWebRequest(URL,"getFilmImages");
            if (string.IsNullOrEmpty(responseContent)) { return; }

            //Split backdrop and poster images out from string
            posters = GeneralParser(responseContent, "\"posters\":[", "]", "\"posters\":null");
            backdrops = GeneralParser(responseContent, "\"backdrops\":[", "]", "\"backdrops\":null");

            char[] delim = { '{' }; //character array holds deliminating values for splitting string
            string[] backdropTokens = backdrops.Split(delim);
            string[] posterTokens = posters.Split(delim);
            //Parse out backdrops from responseContent and add to list
            foreach (string s in backdropTokens)
            {
                if (s.Contains("\"file_path\":\"/"))
                {
                    ListProperties["Backdrops"].Add("http://image.tmdb.org/t/p/w300/" + GeneralParser(s, "\"file_path\":\"/", "\",", "\"file_path\":null"));
                }
            }
            foreach (string s in posterTokens)
            {
                if (s.Contains("\"file_path\":\"/"))
                {
                    ListProperties["Posters"].Add("http://image.tmdb.org/t/p/w154/" + GeneralParser(s, "\"file_path\":\"/", "\",", "\"file_path\":null"));
                }
            }

            if (ListProperties["Posters"].Count > 0) { StaticProperties["PosterPath"] = ListProperties["Posters"][0]; } else { StaticProperties["PosterPath"] = ""; }
            if (ListProperties["Backdrops"].Count > 0) { StaticProperties["BackDropPath"] = ListProperties["Backdrops"][0]; } else { StaticProperties["BackDropPath"] = ""; }
        }
        /// <summary>
        /// Handles web requests to themoviedatabase.org
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        private string MyWebRequest(string URL, string callingMethod)
        {
            if (string.IsNullOrEmpty(URL)) { return ""; }

            var request = System.Net.WebRequest.Create(URL) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.Accept = "application/json";
            request.ContentLength = 0;
            string responseContent = "";
            int retries = 10;
            string exception = "";

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    using (var response = request.GetResponse() as System.Net.HttpWebResponse)
                    {
                        using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                        {
                            responseContent = reader.ReadToEnd();
                            if (!string.IsNullOrEmpty(exception)) //skips removal of error if it wasn't caused by most recent request.
                            {
                                ListProperties["Errors"].RemoveAt(ListProperties.Count() - 1); //Remove last error added since it was finally sucessfull.
                            }
                        }
                    }
                    retries = i; //Forces out of loop
                }
                catch (Exception)
                {
                    if (exception == "")
                    {
                        exception = callingMethod + " ----- " + URL;
                        ListProperties["Errors"].Add(exception);
                        /*if (exception.ToString().Contains("404") && !ListProperties["Errors"].Contains("Request to TMDB.org Failed"))
                        {
                            ListProperties["Errors"].Add("Request to TMDB.org Failed");
                        }
                        if (!ListProperties["Errors"].Contains(exception.ToString())) { ListProperties["Errors"].Add(exception.ToString()); }*/
                    }
                    //Wait 1 second when encoutering error
                    System.Threading.Thread.Sleep(1000);
                }
            }
            return responseContent;
            

        }

    }
}
