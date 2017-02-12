using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieDataCollector
{
    public partial class MovieSelection : Form
    {
        public List<Dictionary<string, string>> movieList { get; set; }
        public string selectedID { get; set; }

        public MovieSelection(string apiKey, string searchString)
        {
            InitializeComponent();
            movieList = new List<Dictionary<string, string>>();
            //parse information from search into the result list

            /*Parameters for search
            page - minimum 1, maximum 1000
            language - ISO 639-1 code
            include_adult - true or false
            year - match release date year
            search_type - phrase (everyday searches) ngram (autocomplete searches)*/

            string URL = "http://api.themoviedb.org/3/search/movie?api_key=" + apiKey + "&search_type=phrase&include_adult=false&language=en&query=" + searchString;
            buildList(MyWebRequest(URL));

            if (movieList.Count > 0)
            {
                posterPB.ImageLocation = "";
                backdropPB.ImageLocation = "";
                //sort movieList by date descending
                sortList();
                populateForm();
            }
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

                        //fix malformed json
                        if (responseContent.Contains(@"\n")) { responseContent = responseContent.Replace(@"\n", ""); } //Remove NewLine characters
                        if (responseContent.Contains(@"\r")) { responseContent = responseContent.Replace(@"\r", ""); } //Remove Return Characers
                        if (responseContent.Contains(@"\t")) { responseContent = responseContent.Replace(@"\t", ""); } //Remove Tab Characters
                    }
                }
                return responseContent;
            }

            catch (Exception e)
            {
                CustomMessageBox.Show(e.ToString(), 300, 300);
                return "";
            }

        }
        private void buildList(string InputString)
        {
            Dictionary<string, string> Movie;
            //parse out movie info from list and add it to the resultList
            //each movie to a dictionary.
            //Split Strings by }
            char[] delim = { '}' };
            string[] Tokens = InputString.Split(delim);

            foreach (string s in Tokens)
            {
                //Check if token contains movie information, and is not an "Adult" film
                if (s.Contains("title") & s.Contains("\"adult\":false"))
                {
                    //create a new dictionary to store video information
                    Movie = new Dictionary<string, string>();

                    //title
                    if (s.Contains("\"title\":\"")) { Movie.Add("Title", GeneralParser(s, "\"title\":\"", "\",")); }
                    else { Movie.Add("Title", ""); }

                    //id
                    if (s.Contains("\"id\":")) { Movie.Add("ID", GeneralParser(s, "\"id\":", ",")); }
                    else { Movie.Add("ID", ""); }

                    //release_date
                    if (s.Contains("\"release_date\":\"")) { Movie.Add("Release_Date", GeneralParser(s, "\"release_date\":\"", "\",")); }
                    else { Movie.Add("Release_Date", ""); }

                    //Release_Year
                    if (!string.IsNullOrEmpty(Movie["Release_Date"]))
                    {
                        Movie.Add("Release_Year", Movie["Release_Date"].Remove(Movie["Release_Date"].Length - 6, 6));
                    }
                    else { Movie.Add("Release_Year", ""); }

                    //backdrop path
                    if (s.Contains("\"backdrop_path\":\"")) { Movie.Add("Backdrop_Path", "http://image.tmdb.org/t/p/w300" + GeneralParser(s, "\"backdrop_path\":\"", "\",")); }
                    else { Movie.Add("Backdrop_Path", ""); }

                    //poster_path
                    if (s.Contains("\"poster_path\":\"")) { Movie.Add("Poster_Path", "http://image.tmdb.org/t/p/w154" + GeneralParser(s, "\"poster_path\":\"", "\",")); }
                    else { Movie.Add("Poster_Path", ""); }

                    //Popularity
                    if (s.Contains("\"popularity\":")) { Movie.Add("Popularity", "http://image.tmdb.org/t/p/w154" + GeneralParser(s, "\"popularity\":", ",")); }
                    else { Movie.Add("Popularity", ""); }

                    //overview
                    if (s.Contains("\"overview\":\"")) { Movie.Add("Overview", GeneralParser(s, "\"overview\":\"", "\",").Replace("\\", "").Replace("/", "")); }
                    else { Movie.Add("Overview", ""); }

                    //after all items are added to the dictionary, add the dictionary to the list.
                    movieList.Add(Movie);
                }
            }
        }
        private void populateForm()
        {
            moviesLB.Items.Clear();
            for (int i = 0; i < movieList.Count; i++)
            {
                if (!string.IsNullOrEmpty(movieList[i]["Release_Date"]) & movieList[i]["Release_Date"].Length == 10)
                {
                    moviesLB.Items.Add(movieList[i]["Title"] + " (" + movieList[i]["Release_Date"].Remove(movieList[i]["Release_Date"].Length - 6, 6) + ")");
                }
                else
                {
                    moviesLB.Items.Add(movieList[i]["Title"] + " (NA)");
                }
            }

            posterPB.ImageLocation = movieList[0]["Poster_Path"];
            backdropPB.ImageLocation = movieList[0]["Backdrop_Path"];
            overviewTB.Text = movieList[0]["Overview"];
            moviesLB.SelectedIndex = 0;
        }
        private string GeneralParser(string InputString, string start, string end)
        {
            if (string.IsNullOrEmpty(InputString)) { return ""; }
            int startPosition = InputString.IndexOf(start) + start.Length;
            int endPosition = InputString.IndexOf(end, startPosition);

            try
            {
                if (startPosition == -1 || endPosition == -1) { return ""; }

                if (startPosition >= endPosition) { return ""; }

                if (InputString.Length - startPosition > endPosition - startPosition)
                {
                    return InputString.Substring(startPosition, endPosition - startPosition);
                }
                else { return ""; }
            }
            catch (Exception e)
            {
                CustomMessageBox.Show(e.ToString() + "\n\n\n"
                    + "StartPosition " + startPosition.ToString() + "\n"
                    + "EndPosition " + endPosition.ToString() + "\n"
                    + "AdjustedStartPosition " + startPosition.ToString() + "\n"
                    + "InputString " + InputString + "\n"
                    + "Start " + start.ToString() + "\n"
                    + "End " + end.ToString(), 300, 300);
                return "";
            }
        }
        private void acceptBtn_Click(object sender, EventArgs e)
        {
            //search movie and return IMDBID number
            selectedID = movieList[moviesLB.SelectedIndex]["ID"];
            this.DialogResult = DialogResult.OK;
        }
        private void exitBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void moviesLB_SelectedIndexChanged(object sender, EventArgs e)
        {
            posterPB.ImageLocation = movieList[moviesLB.SelectedIndex]["Poster_Path"];
            backdropPB.ImageLocation = movieList[moviesLB.SelectedIndex]["Backdrop_Path"];
            overviewTB.Text = movieList[moviesLB.SelectedIndex]["Overview"];
        }
        private void sortList()
        {
            //movieList = movieList.OrderByDescending(d => d["Release_Year"]).ToList();

            if (PopularityRBtn.Checked)
            {
                movieList = (from x in movieList
                             orderby x["Popularity"] descending, x["Release_Year"] descending
                             select x).ToList();
            }
            if (releaseYearRBtn.Checked)
            {
                movieList = (from x in movieList
                             orderby x["Release_Year"] descending
                             select x).ToList();
            }

            populateForm();
        }
        private void PopularityRBtn_Click(object sender, EventArgs e)
        {
            sortList();
        }
        private void releaseYearRBtn_Click(object sender, EventArgs e)
        {
            sortList();
        }
    }
}
