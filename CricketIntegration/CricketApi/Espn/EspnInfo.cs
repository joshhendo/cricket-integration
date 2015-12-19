using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CricketIntegration.CricketApi.Espn
{
    public class EspnInfo
    {
        public class Live
        {
            public class Fow
            {
                public class FowOutPlayer
                {
                    public string dismissal_string { get; set; }
                }

                public string live_current_name { get; set; }
                public FowOutPlayer out_player { get; set; }
            }

            public List<Fow> fow { get; set; }
        }

        public Live live { get; set; }
    }
}
