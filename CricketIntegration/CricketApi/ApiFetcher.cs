using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CricketIntegration.CricketApi.Cricinfo;

namespace CricketIntegration.CricketApi
{
    public static class ApiFetcher
    {
        public const string matchesUrl = "http://www.cricscore-api.appspot.com/csa";
        public const string url = "http://www.cricscore-api.appspot.com/csa?id={0}";

        public static List<CricinfoMatchInformation> GetMatches()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                try
                {
                    json_data = w.DownloadString(matchesUrl);
                }
                catch (Exception) { }

                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<List<CricinfoMatchInformation>>(json_data) : new List<CricinfoMatchInformation>();
            }
        }

        public static List<CricinfoMatchDetails> GetMatch(int id)
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(string.Format(url, id));
                }
                catch (Exception)
                {
                    return null;
                }

                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<List<CricinfoMatchDetails>>(json_data) : new List<CricinfoMatchDetails>();
            } 
        }
    }
}
