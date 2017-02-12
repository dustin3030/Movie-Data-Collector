using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public MovieInfo(string IMDBID, string APIKey)
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
            staticProperties["IMDB_ID"] = IMDBID;
            staticProperties["API_Key"] = APIKey;

        }
        /// <summary>
        /// Returns MPAA rating for movies
        /// </summary>
        /// <param name="IMDB_ID"></param>
        public void getRating()
        {
            string responseContent;
            string URL = "https://api.themoviedb.org/3/movie/" + IMDB_ID + "/releases?api_key=" + API_Key;
            responseContent = MyWebRequest(URL);

            //With a null response there is nothing to parse so excape the method on set MPAA_Rating to Error
            if (string.IsNullOrEmpty(responseContent)) { MPAA_Rating = "Error"; return; }

            //Split reponse string into array to prepare for parsing
            char[] delim = { '}' };
            string[] ratingTokens = responseContent.Split(delim);

            //Parse out ratings from responseContent
            foreach (string s in ratingTokens)
            {
                if (s.Contains(":\"US\""))
                {
                    MPAA_Rating = GeneralParser(s, "\"certification\":\"", "\",\"", "\"certification\":null");
                }
            }

            if (string.IsNullOrEmpty(MPAA_Rating)) { MPAA_Rating = "NR"; } //Fixes errors related with no rating being listed
        }
        /// <summary>
        /// Builds list of Currently Available Generes from the website themoviedb.org
        /// </summary>
        private string BuildGenreList(string responseContent)
        {
            string parsedGenres;
            string tempString;
            GenreList = new List<string>(); //instantiates list

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
                    GenreList.Add(GeneralParser(s, "name\":\"", "\"", "name\":null"));
                }
                else if (s.Contains("name\":\""))
                {
                    tempString = (GeneralParser(s, "name\":\"", "\"", "name\":null"));
                    if (!string.IsNullOrEmpty(tempString))
                    {
                        genreBuilder.Append(", " + tempString);
                        GenreList.Add(tempString);
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
        /// <param name="IMDB_ID"></param>
        public void getBasicInfo()
        {

            string responseContent;
            string URL = "https://api.themoviedb.org/3/movie/" + IMDB_ID + "?api_key=" + API_Key;
            responseContent = MyWebRequest(URL);

            //set defaults if response is null
            if (string.IsNullOrEmpty(responseContent))
            {
                Adult = false;
                OriginalTitle = "Title Not Found";
                Title = "Title Not Found";
                ReleaseDate = "";
                ReleaseYear = "Year Not Found";
                FormattedTitle = "";
                Plot = "Plot Not Found";
                IMDB_ID = "";
                Popularity = "";
                Revenue = "";
                RunTime = "0";
                Tag_Line = "";
                Collection = "Not Found";
                Genres = "Not Found";
                Studio = "";
                ProductionCountry = "";
                VoteAverage = "";
                VoteCount = "";
                Budget = "";
                PosterPath = "";
                BackDropPath = "";
                return;
            }

            //Fix malformed Json / HTML code
            if (responseContent.Contains(@"\n")) { responseContent = responseContent.Replace(@"\n", ""); } //Remove NewLine characters
            if (responseContent.Contains(@"\r")) { responseContent = responseContent.Replace(@"\r", ""); } //Remove Return Characers
            if (responseContent.Contains(@"\t")) { responseContent = responseContent.Replace(@"\t", ""); } //Remove Tab Characters

            //Default to false
            string isAdult;
            isAdult = GeneralParser(responseContent, "adult\":", ",", "adult\":null");
            if (isAdult.Contains("false") || string.IsNullOrEmpty(isAdult)) { Adult = false; }
            else if (isAdult.Contains("true")) { Adult = true; }
            else { Adult = false; }

            OriginalTitle = GeneralParser(responseContent, "original_title\":\"", "\",", "original_title\":null");
            Title = GeneralParser(responseContent, "\"title\":\"", "\",", "\"title\":null");
            ReleaseDate = GeneralParser(responseContent, "release_date\":\"", "\",", "release_date\":null");
            IMDB_ID = GeneralParser(responseContent, "\"imdb_id\":\"", "\",", "imdb_id\":null");

            //Set ReleaseYear to "" as default
            if (!string.IsNullOrEmpty(ReleaseDate) && ReleaseDate.Length > 4)
            {
                ReleaseYear = ReleaseDate.Remove(4, ReleaseDate.Length - 4);
            }
            else { ReleaseYear = ""; }

            //Get valid Title by removing invalid file name characters
            FormattedTitle = validTitle(Title + " (" + ReleaseYear + ")");

            Plot = GeneralParser(responseContent, "overview\":\"", "\",\"popularity\"", "overview\":null");
            if (Plot.Contains("\\r")) { Plot = Plot.Replace("\\r", ""); }
            if (Plot.Contains("\\n")) { Plot = Plot.Replace("\\n", ""); }
            if (Plot.Contains("\\")) { Plot = Plot.Replace("\\", ""); }

            Popularity = GeneralParser(responseContent, "popularity\":", ",", "popularity\":null");
            Revenue = GeneralParser(responseContent, "revenue\":", ",", "revenue\":null");
            RunTime = GeneralParser(responseContent, "runtime\":", ",", "runtime\":null");
            Tag_Line = GeneralParser(responseContent, "tagline\":\"", "\",", "tagline\":null");

            //Removes backslash characters from the tagline.
            if (Tag_Line.Contains(@"\")) { Tag_Line = Tag_Line.Replace(@"\", ""); }

            Collection = GeneralParser(responseContent, "belongs_to_collection\":{\"id\":", "\",", "belongs_to_collection\":null");
            if (!string.IsNullOrEmpty(Collection)) { Collection = GeneralParser(Collection, "name\":\"", "Collection", "name\":null"); }
            if (!string.IsNullOrEmpty(Collection)) { Collection = Collection.Trim() + " Collection"; }

            Genres = BuildGenreList(responseContent);
            Studio = ProductionCompanies(responseContent);
            ProductionCountry = ProductionCountries(responseContent);
            VoteAverage = GeneralParser(responseContent, "vote_average\":", ",", "vote_average\":null");
            VoteCount = GeneralParser(responseContent, "vote_count\":", "}", "vote_count\":null");
            Budget = GeneralParser(responseContent, "budget\":", ",", "budget\":null");
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
            StudioList = new List<string>(); //Instantiate list

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
                    StudioList.Add(GeneralParser(s, "\"name\":\"", "\",", "\"name\":null"));
                }
                else if (s.Contains("\"name\":\""))
                {
                    returnString.Append(", " + GeneralParser(s, "\"name\":\"", "\",", "\"name\":null"));
                    StudioList.Add(GeneralParser(s, "\"name\":\"", "\",", "\"name\":null"));
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
            ActorImages = new List<string>();
            ActorNames = new List<string>();
            ActorRoles = new List<string>();
            CrewJobs = new List<string>();
            CrewNames = new List<string>();

            string castJson; //holds split string with cast information
            string crewJson; //holds split string with crew information

            //Creating Web Request
            string responseContent;
            string URL = "https://api.themoviedb.org/3/movie/" + IMDB_ID + "/credits?api_key=" + API_Key + "&language=en&include_image_language=en,null";
            responseContent = MyWebRequest(URL);



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
                    ActorNames.Add(GeneralParser(s, "\"name\":\"", "\",\"", "\"name\":null"));
                    ActorRoles.Add(GeneralParser(s, "\"character\":\"", "\",\"", "\"character\":null"));
                    if (s.Contains(".jpg"))
                    {
                        ActorImages.Add("https://image.tmdb.org/t/p/original/" + GeneralParser(s, "\"profile_path\":\"/", "\"}", "\"profile_path\":null"));
                    }
                    else { ActorImages.Add("Null"); }
                }
            }

            foreach (string s in crewTokens)
            {
                if (s.Contains("\"name\":") && s.Contains("Director"))
                {
                    Director = GeneralParser(s, "\"name\":\"", "\",\"", "\"name\":null"); //Pulls Directors Name from the string
                    CrewNames.Add(Director); //Returns Directors name to the CrewNames list
                    CrewJobs.Add("Director"); //Lists Directors Job as Director
                }
                else if (s.Contains("\"name\":"))
                {
                    CrewNames.Add(GeneralParser(s, "\"name\":\"", "\",\"", "\"name\":null"));
                    CrewJobs.Add(GeneralParser(s, "\"job\":\"", "\",\"", "\"job\":null"));
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
        /// <param name="IMDB_ID"></param>
        public void GetUSTitles()
        {
            USTitles = new List<string>(); //instantiates list
            string responseContent;
            string URL = "https://api.themoviedb.org/3/movie/" + IMDB_ID + "/alternative_titles?api_key=" + API_Key;
            responseContent = MyWebRequest(URL);
            if (string.IsNullOrEmpty(responseContent)) { return; }

            //Split reponse string into array to prepare for parsing
            char[] delim = { '{' }; //character array holds deliminating values for splitting string
            string[] Tokens = responseContent.Split(delim);
            //Parse out genres from responseContent and add to list
            foreach (string s in Tokens)
            {
                if (s.Contains("\":\"US\""))
                {
                    USTitles.Add(GeneralParser(s, "\"title\":\"", "\"}", "\"title\":null"));
                }
                if (s.Contains("\":\"en\""))
                {
                    USTitles.Add(GeneralParser(s, "\"title\":\"", "\"}", "\"title\":null"));
                }
            }
        }
        /// <summary>
        /// Gathers images for US and region coded images
        /// </summary>
        public void getFilmImages()
        {
            Backdrops = new List<string>(); //instantiate Backdrop image list
            Posters = new List<string>(); //instantiate poster image list
            string backdrops = ""; //string to hold the backdrop potion of the response
            string posters = ""; //string to hold the poster portion of the string

            string responseContent;
            //string URL = "https://api.themoviedb.org/3/movie/" + IMDB_ID + "/images?api_key=" + APIKey;
            string URL = "https://api.themoviedb.org/3/movie/" + IMDB_ID + "/images?api_key=" + API_Key + "&language=en&include_image_language=en,null";
            responseContent = MyWebRequest(URL);
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
                    Backdrops.Add("http://image.tmdb.org/t/p/w300/" + GeneralParser(s, "\"file_path\":\"/", "\",", "\"file_path\":null"));
                }
            }
            foreach (string s in posterTokens)
            {
                if (s.Contains("\"file_path\":\"/"))
                {
                    Posters.Add("http://image.tmdb.org/t/p/w154/" + GeneralParser(s, "\"file_path\":\"/", "\",", "\"file_path\":null"));
                }
            }

            if (Posters.Count > 0) { PosterPath = Posters[0]; } else { PosterPath = ""; }
            if (Backdrops.Count > 0) { BackDropPath = Backdrops[0]; } else { BackDropPath = ""; }
        }
        /// <summary>
        /// Handles web requests to themoviedatabase.org
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
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
                if (e.ToString().Contains("404") && !Errors.Contains("Request to TMDB.org Failed"))
                {
                    Errors.Add("Request to TMDB.org Failed");
                    return "";
                }
                if (!Errors.Contains(e.ToString())) { Errors.Add(e.ToString()); }
                return "";
            }

        }

    }
}
