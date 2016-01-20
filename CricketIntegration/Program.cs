using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CricketIntegration.CricketApi;
using CricketIntegration.CricketApi.Cricinfo;
using CricketIntegration.Storage;
using CricketIntegration.ExternalServices.Slack;
using Serilog;

namespace CricketIntegration
{
    class Program
    {
        public static List<CricinfoMatchInformation> matchesOfInterest = new List<CricinfoMatchInformation>();

        public static string[] followingTeams = ConfigurationManager.AppSettings["CricketIntegration.FollowingTeams"].Split(',');


        static void MonitorMatchesOfInterest()
        {

            while(true)
            {
                var availableMatches = ApiFetcher.GetMatches();
                matchesOfInterest = availableMatches.Where(m => m.HasTeamPlaying(followingTeams)).ToList();

                if (!matchesOfInterest.Any())
                {
                    Console.WriteLine("No matches of interest");
                }
                else
                {
                    Console.WriteLine($"Found {matchesOfInterest.Count()} matches of interest");
                }

                Thread.Sleep(160000);
            }
        }

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            new Thread((new ThreadStart(MonitorMatchesOfInterest))).Start();

            while (true)
            {
                foreach (var matchOfInterest in matchesOfInterest)
                {
                    try
                    {
                        var result = ApiFetcher.GetMatch(matchOfInterest.id).FirstOrDefault();

                        if (result != null)
                        {
                            result.id = Guid.NewGuid().ToString();

                            var previousResult = DataStore.GetLastStore();
                            DataStore.StoreNew(result);

                            if (previousResult == null)
                            {
                                continue;
                            }

                            var messages = MessageHandler.MessageHandler.GetMessages(previousResult, result, matchOfInterest.id);

                            foreach (var message in messages)
                            {
                                var score = string.Format("{0} {1}/{2}", result.BattingTeam, result.BattingTeamOuts, result.BattingTeamRuns);

                                var messageSender = new MessageSender();
                                messageSender.Send(message, score);

                                Console.WriteLine(DateTime.Now + ": " + message + " --- Score: " + score);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception caught: " + e.Message);
                        Log.Logger.Error(e, "An exception was caught by the main thread");
                    }
                    
                }

                Thread.Sleep(30000);
            }            
        }
    }
}
