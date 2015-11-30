using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CricketIntegration.CricketApi.Cricinfo
{
    public class CricinfoMatchDetails
    {
        private const string pattern = @"([A-Za-z]+) ([0-9]+)/([0-9]{1,2})(d)? \(([0-9\.]+) ov, ([A-Za-z ]+) ([0-9]+)\*?,( ([A-Za-z ]+) ([0-9]+)\*,)? ([A-Za-z ]+) ([0-9]{1})/([0-9]+)\)( - (.*?))?$";

        public CricinfoMatchDetails()
        {
            this.RetrievedDate = DateTime.UtcNow;
        }

        public string de { get; set; }
        public string id { get; set; }
        public string di { get; set; }

        private string RegexHandler(int group)
        {
            Regex compiledPattern = new Regex(pattern);

            var matched = compiledPattern.Match(this.de);

            if (matched.Success)
            {
                return matched.Groups[group].Value;
            }

            return null;
        }

        public DateTime RetrievedDate { get; set; }

        public string BattingTeam
        {
            get { return RegexHandler(1); }
        }

        public int BattingTeamRuns
        {
            get { return int.Parse(RegexHandler(2)); }
        }

        public int BattingTeamOuts
        {
            get { return int.Parse(RegexHandler(3)); }
        }

        public bool BattingTeamDeclared
        {
            get { return RegexHandler(4) == "d"; }
        }

        public double OverInInnings
        {
            get { return double.Parse(RegexHandler(5)); }
        }

        public string CurrentBatter 
        {
            get { return RegexHandler(6); }
        }

        public int CurrentBatterScore
        {
            get { return int.Parse(RegexHandler(7)); }
        }

        public string OtherBatter
        {
            get { return RegexHandler(9); }
        }

        public int? OtherBatterScore
        {
            get
            {
                try
                {
                    return int.Parse(RegexHandler(10));
                }
                catch (Exception)
                { }

                return null;
            }
        }

        public string[] Batters
        {
            get
            {
                return new string[] { CurrentBatter, OtherBatter };
            }
        }

        public string Bowler
        {
            get { return RegexHandler(11); }
        }

        public int BowlerOuts
        {
            get { return int.Parse(RegexHandler(12)); }
        }

        public int BowlerRuns
        {
            get { return int.Parse(RegexHandler(13)); }
        }

        public string State
        {
            get { return RegexHandler(15); }
        }
    }
}
