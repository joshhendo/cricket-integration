using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketIntegration.CricketApi.Cricinfo
{
    public class CricinfoMatchInformation
    {
        public int id { get; set; }
        public string t1 { get; set; }
        public string t2 { get; set; }

        public bool HasTeamPlaying(string[] teams)
        {
            return teams.Aggregate(false, (current, team) => current || HasTeamPlaying(team));
        }

        public bool HasTeamPlaying(string team)
        {
            return ((t1 == team) || (t2 == team));
        }
    }
}
