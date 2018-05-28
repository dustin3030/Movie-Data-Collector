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
        public Dictionary<string, List<string>> listProperties { get; set; }
        //Contains all statics
        public Dictionary<string,string> staticProperties { get; set; }

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
            listProperties = new Dictionary<string, List<string>>(); //Instantiate Dictionary
            for (int i = 0; i < ListDictKeyList.Count ; i++)
            {
                listProperties[ListDictKeyList[i]] = new List<string>();
            }

            staticProperties = new Dictionary<string, string>(); //Instantiate Dictionary
            for (int i = 0; i < StaticKeyList.Count; i++)
            {
                staticProperties[StaticKeyList[i]] = "";
            }

            //Fill dictionary with passed in values
            staticProperties["TMDB_ID"] = TMDBID;
            staticProperties["API_Key"] = APIKey;

        }
        /// <summary>
        /// Returns MPAA rating for movies
        /// </summary>
        /// <param name="staticProperties["TMDB_ID"]"></param>
        public void getRating()
        {
            string responseContent;
            string URL = "https://api.themoviedb.org/3/movie/" + staticProperties["TMDB_ID"] + "/releases?api_key=" + staticProperties["API_Key"];
            responseContent = MyWebRequest(URL,"getRating");

            //With a null response there is nothing to parse so escape the method on set staticProperties["MPAA_Rating"] to Error
            if (string.IsNullOrEmpty(responseContent)) { staticProperties["MPAA_Rating"] = "Error"; return; }

            //Split reponse string into array to prepare for parsing
            char[] delim = { '}' };
            string[] ratingTokens = responseContent.Split(delim);

            //Parse out ratings from responseContent
            foreach (string s in ratingTokens)
            {
                if (s.Contains(":\"US\""))
                {
                    staticProperties["MPAA_Rating"] = GeneralParser(s, "\"certification\":\"", "\",\"", "\"certification\":null");
                }
            }

            if (string.IsNullOrEmpty(staticProperties["MPAA_Rating"])) { staticProperties["MPAA_Rating"] = "NR"; } //Fixes errors related with no rating being listed
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
                    listProperties["GenreList"].Add(GeneralParser(s, "name\":\"", "\"", "name\":null"));
                }
                else if (s.Contains("name\":\""))
                {
                    tempString = (GeneralParser(s, "name\":\"", "\"", "name\":null"));
                    if (!string.IsNullOrEmpty(tempString))
                    {
                        genreBuilder.Append(", " + tempString);
                        listProperties["GenreList"].Add(tempString);
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
        /// <param name="staticProperties["TMDB_ID"]"></param>
        public void getBasicInfo()
        {

            string responseContent;
            string URL = "https://api.themoviedb.org/3/movie/" + staticProperties["TMDB_ID"] + "?api_key=" + staticProperties["API_Key"];
            responseContent = MyWebRequest(URL,"getBasicInfo");

            //set defaults if response is null
            if (string.IsNullOrEmpty(responseContent))
            {
                staticProperties["Adult"] = "False";
                staticProperties["OriginalTitle"] = "Title Not Found";
                staticProperties["Title"] = "Title Not Found";
                staticProperties["ReleaseDate"] = "";
                staticProperties["ReleaseYear"] = "Year Not Found";
                staticProperties["FormattedTitle"] = "";
                staticProperties["Plot"] = "Plot Not Found";
                staticProperties["IMDB_ID"] = "";
                staticProperties["TMDB_ID"] = "";
                staticProperties["Popularity"] = "";
                staticProperties["Revenue"] = "";
                staticProperties["RunTime"] = "0";
                staticProperties["Tag_Line"] = "";
                staticProperties["Collection"] = "Not Found";
                staticProperties["Genres"] = "Not Found";
                staticProperties["Studio"] = "";
                staticProperties["ProductionCountry"] = "";
                staticProperties["VoteAverage"] = "";
                staticProperties["VoteCount"] = "";
                staticProperties["Budget"] = "";
                staticProperties["PosterPath"] = "";
                staticProperties["BackDropPath"] = "";
                return;
            }

            //Fix malformed Json / HTML code
            if (responseContent.Contains(@"\n")) { responseContent = responseContent.Replace(@"\n", ""); } //Remove NewLine characters
            if (responseContent.Contains(@"\r")) { responseContent = responseContent.Replace(@"\r", ""); } //Remove Return Characers
            if (responseContent.Contains(@"\t")) { responseContent = responseContent.Replace(@"\t", ""); } //Remove Tab Characters

            //Default to false
            string isAdult;
            isAdult = GeneralParser(responseContent, "adult\":", ",", "adult\":null");
            if (isAdult.Contains("false") || string.IsNullOrEmpty(isAdult)) { staticProperties["Adult"] = "False"; }
            else if (isAdult.Contains("true")) { staticProperties["Adult"] = "True"; }
            else { staticProperties["Adult"] = "False"; }

            staticProperties["OriginalTitle"] = GeneralParser(responseContent, "original_title\":\"", "\",", "original_title\":null");
            staticProperties["Title"] = GeneralParser(responseContent, "\"title\":\"", "\",", "\"title\":null");
            staticProperties["ReleaseDate"] = GeneralParser(responseContent, "release_date\":\"", "\",", "release_date\":null");
            staticProperties["IMDB_ID"] = GeneralParser(responseContent, "\"imdb_id\":\"", "\",", "imdb_id\":null");

            //Set ReleaseYear to an empty string as default
            if (!string.IsNullOrEmpty(staticProperties["ReleaseDate"]) && staticProperties["ReleaseDate"].Length > 4)
            {
                staticProperties["ReleaseYear"] = staticProperties["ReleaseDate"].Remove(4, staticProperties["ReleaseDate"].Length - 4);
            }
            else { staticProperties["ReleaseYear"] = ""; }

            //Get valid Title by removing invalid file name characters
            staticProperties["FormattedTitle"] = validTitle(staticProperties["Title"] + " (" + staticProperties["ReleaseYear"] + ")");

            staticProperties["Plot"] = GeneralParser(responseContent, "overview\":\"", "\",\"popularity\"", "overview\":null");
            if (staticProperties["Plot"].Contains("\\r")) { staticProperties["Plot"] = staticProperties["Plot"].Replace("\\r", ""); }
            if (staticProperties["Plot"].Contains("\\n")) { staticProperties["Plot"] = staticProperties["Plot"].Replace("\\n", ""); }
            if (staticProperties["Plot"].Contains("\\")) { staticProperties["Plot"] = staticProperties["Plot"].Replace("\\", ""); }

            staticProperties["Popularity"] = GeneralParser(responseContent, "popularity\":", ",", "popularity\":null");
            staticProperties["Revenue"] = GeneralParser(responseContent, "revenue\":", ",", "revenue\":null");
            staticProperties["RunTime"] = GeneralParser(responseContent, "runtime\":", ",", "runtime\":null");
            staticProperties["Tag_Line"] = GeneralParser(responseContent, "tagline\":\"", "\",", "tagline\":null");

            //Removes backslash characters from the tagline.
            if (staticProperties["Tag_Line"].Contains(@"\")) { staticProperties["Tag_Line"] = staticProperties["Tag_Line"].Replace(@"\", ""); }

            staticProperties["Collection"] = GeneralParser(responseContent, "belongs_to_collection\":{\"id\":", "\",", "belongs_to_collection\":null");
            if (!string.IsNullOrEmpty(staticProperties["Collection"])) { staticProperties["Collection"] = GeneralParser(staticProperties["Collection"], "name\":\"", "Collection", "name\":null"); }
            if (!string.IsNullOrEmpty(staticProperties["Collection"])) { staticProperties["Collection"] = staticProperties["Collection"].Trim() + " Collection"; }

            staticProperties["Genres"] = BuildGenreList(responseContent);
            staticProperties["Studio"] = ProductionCompanies(responseContent);
            staticProperties["ProductionCountry"] = ProductionCountries(responseContent);
            staticProperties["VoteAverage"] = GeneralParser(responseContent, "vote_average\":", ",", "vote_average\":null");
            staticProperties["VoteCount"] = GeneralParser(responseContent, "vote_count\":", "}", "vote_count\":null");
            staticProperties["Budget"] = GeneralParser(responseContent, "budget\":", ",", "budget\":null");
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
                    listProperties["StudioList"].Add(GeneralParser(s, "\"name\":\"", "\",", "\"name\":null"));
                }
                else if (s.Contains("\"name\":\""))
                {
                    returnString.Append(", " + GeneralParser(s, "\"name\":\"", "\",", "\"name\":null"));
                    listProperties["StudioList"].Add(GeneralParser(s, "\"name\":\"", "\",", "\"name\":null"));
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
        public void getCredits()
        {

            string castJson; //holds split string with cast information
            string crewJson; //holds split string with crew information

            //Creating Web Request
            string responseContent;
            string URL = "https://api.themoviedb.org/3/movie/" + staticProperties["TMDB_ID"] + "/credits?api_key=" + staticProperties["API_Key"] + "&language=en&include_image_language=en,null";
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
                    listProperties["ActorNames"].Add(GeneralParser(s, "\"name\":\"", "\",\"", "\"name\":null"));
                    listProperties["ActorRoles"].Add(GeneralParser(s, "\"character\":\"", "\",\"", "\"character\":null"));
                    if (s.Contains(".jpg"))
                    {
                        listProperties["ActorImages"].Add("https://image.tmdb.org/t/p/original/" + GeneralParser(s, "\"profile_path\":\"/", "\"}", "\"profile_path\":null"));
                    }
                    else { listProperties["ActorImages"].Add("Null"); }
                }
            }

            foreach (string s in crewTokens)
            {
                if (s.Contains("\"name\":") && s.Contains("Director"))
                {
                    staticProperties["Director"] = GeneralParser(s, "\"name\":\"", "\",\"", "\"name\":null"); //Pulls Directors Name from the string
                    listProperties["CrewNames"].Add(staticProperties["Director"]); //Returns Directors name to the CrewNames list
                    listProperties["CrewJobs"].Add(staticProperties["Director"]); //Lists Directors Job as Director
                }
                else if (s.Contains("\"name\":"))
                {
                    listProperties["CrewNames"].Add(GeneralParser(s, "\"name\":\"", "\",\"", "\"name\":null"));
                    listProperties["CrewJobs"].Add(GeneralParser(s, "\"job\":\"", "\",\"", "\"job\":null"));
                }
            }

        }
        /// <summary>
        /// Removes invalid filename characters from the title \ / : * ? " < > |
        /// </summary>
        /// <param name="unformattedTitle"></param>
        /// <returns></returns>
        private string validTitle(string unformattedTitle)
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
        /// <param name="staticProperties["TMDB_ID"]"></param>
        public void GetUSTitles()
        {
            listProperties["USTitles"] = new List<string>(); //instantiates list
            string responseContent;
            string URL = "https://api.themoviedb.org/3/movie/" + staticProperties["TMDB_ID"] + "/alternative_titles?api_key=" + staticProperties["API_Key"];
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
                    listProperties["USTitles"].Add(GeneralParser(s, "\"title\":\"", "\",\"type\":", "\"title\":null"));
                }
                if (s.Contains("\":\"en\""))
                {
                    listProperties["USTitles"].Add(GeneralParser(s, "\"title\":\"", "\",\"type\":", "\"title\":null"));
                }
            }
        }
        /// <summary>
        /// Gathers images for US and region coded images
        /// </summary>
        public void getFilmImages()
        {
            string backdrops = ""; //string to hold the backdrop portion of the response
            string posters = ""; //string to hold the poster portion of the string

            string responseContent;
            //string URL = "https://api.themoviedb.org/3/movie/" + staticProperties["TMDB_ID"] + "/images?staticProperties["API_Key"]=" + APIKey;
            string URL = "https://api.themoviedb.org/3/movie/" + staticProperties["TMDB_ID"] + "/images?api_key=" + staticProperties["API_Key"] + "&language=en&include_image_language=en,null";
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
                    listProperties["Backdrops"].Add("http://image.tmdb.org/t/p/w300/" + GeneralParser(s, "\"file_path\":\"/", "\",", "\"file_path\":null"));
                }
            }
            foreach (string s in posterTokens)
            {
                if (s.Contains("\"file_path\":\"/"))
                {
                    listProperties["Posters"].Add("http://image.tmdb.org/t/p/w154/" + GeneralParser(s, "\"file_path\":\"/", "\",", "\"file_path\":null"));
                }
            }

            if (listProperties["Posters"].Count > 0) { staticProperties["PosterPath"] = listProperties["Posters"][0]; } else { staticProperties["PosterPath"] = ""; }
            if (listProperties["Backdrops"].Count > 0) { staticProperties["BackDropPath"] = listProperties["Backdrops"][0]; } else { staticProperties["BackDropPath"] = ""; }
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
                                listProperties["Errors"].RemoveAt(listProperties.Count() - 1); //Remove last error added since it was finally sucessfull.
                            }
                        }
                    }
                    retries = i; //Forces out of loop
                }
                catch (Exception e)
                {
                    if (exception == "")
                    {
                        exception = callingMethod + " ----- " + URL;
                        listProperties["Errors"].Add(exception);
                        /*if (exception.ToString().Contains("404") && !listProperties["Errors"].Contains("Request to TMDB.org Failed"))
                        {
                            listProperties["Errors"].Add("Request to TMDB.org Failed");
                        }
                        if (!listProperties["Errors"].Contains(exception.ToString())) { listProperties["Errors"].Add(exception.ToString()); }*/
                    }
                    //Wait 1 second when encoutering error
                    System.Threading.Thread.Sleep(1000);
                }
            }
            return responseContent;
            

        }

    }
}
