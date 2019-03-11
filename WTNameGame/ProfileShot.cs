

namespace WTNameGame
{
    /// <summary>
    /// ProfileShot class to hold employee faces, names, etc. for the game
    /// </summary>
    public class ProfileShot : ModelBase
    {
        private const string URL_PREFIX = "http:";
        private string url_;
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string Url
        {
            get
            {
                return URL_PREFIX + url_;
            }
            set
            {
                // Using conditional to prevent runtime errors for null url's from json data (WillowTree logo)
                url_ = value != null ? value : "//images.ctfassets.net/3cttzl4i3k1h/1PoufpRNis4mmAmiqkA0ge/ef1fc7606584d54b5892010a65a5a262/WT_Logo-Hye-tTeI0Z.png";
            }
        }
        public ProfileShot(string fullname, string jobTitle, string url)
        {
            this.FullName = fullname;
            this.JobTitle = jobTitle;
            this.Url = url;
        }
    }
}
