using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CricketIntegration.CricketApi;
using CricketIntegration.CricketApi.Cricinfo;

namespace CricketIntegration.MessageHandler
{
    public static class MessageHandler
    {
        private const string STATE_MATCH_OVER = "Match Over";
        private const string STATE_TEA = "Tea";
        private const string STATE_RAIN_DELAY = "Match delayed by rain";
        private const string STATE_PLAY = "Play";
        private const string STATE_INNINGS_BREAK = "Innings break";
        private const string STATE_LUNCH = "Lunch";
        private const string STATE_DRINKS = "Drinks";
        private const string STATE_STUMPS = "Stumps";

        private static bool IsBatterOut(string batter, CricinfoMatchDetails currentResult)
        {
            if (batter == null)
            {
                return false;
            }

            return !(currentResult.CurrentBatter == batter || currentResult.OtherBatter == batter);
        }

        private static bool IsBatterFour(string batter, CricinfoMatchDetails previousResult, CricinfoMatchDetails currentResult)
        {
            if (string.IsNullOrEmpty(batter))
            {
                return false;
            }

            int currentScore = -1;
            int previousScore = -1;

            if (currentResult.CurrentBatter == batter)
            {
                currentScore = currentResult.CurrentBatterScore;
            }

            if (currentResult.OtherBatter == batter)
            {
                currentScore = currentResult.OtherBatterScore.Value;
            }

            if (previousResult.CurrentBatter == batter)
            {
                previousScore = previousResult.CurrentBatterScore;
            }

            if (previousResult.OtherBatter == batter)
            {
                previousScore = previousResult.OtherBatterScore.Value;
            }

            if ((currentScore - previousScore) >= 4 && (currentScore - previousScore) < 6)
            {
                return true;
            }

            return false;
        }

        private static bool IsBatterSix(string batter, CricinfoMatchDetails previousResult, CricinfoMatchDetails currentResult)
        {
            if (string.IsNullOrEmpty(batter))
            {
                return false;
            }

            int currentScore = -1;
            int previousScore = -1;

            if (currentResult.CurrentBatter == batter)
            {
                currentScore = currentResult.CurrentBatterScore;
            }

            if (currentResult.OtherBatter == batter)
            {
                currentScore = currentResult.OtherBatterScore.Value;
            }

            if (previousResult.CurrentBatter == batter)
            {
                previousScore = previousResult.CurrentBatterScore;
            }

            if (previousResult.OtherBatter == batter)
            {
                previousScore = previousResult.OtherBatterScore.Value;
            }

            if ((currentScore - previousScore) >= 6)
            {
                return true;
            }

            return false;
        }

        private static bool IsBatterMilestone(string batter, int milestone, CricinfoMatchDetails previousResult,
            CricinfoMatchDetails currentResult)
        {
            if (string.IsNullOrEmpty(batter))
            {
                return false;
            }

            int currentScore = -1;
            int previousScore = -1;

            if (currentResult.CurrentBatter == batter)
            {
                currentScore = currentResult.CurrentBatterScore;
            }

            if (currentResult.OtherBatter == batter)
            {
                currentScore = currentResult.OtherBatterScore.Value;
            }

            if (previousResult.CurrentBatter == batter)
            {
                previousScore = previousResult.CurrentBatterScore;
            }

            if (previousResult.OtherBatter == batter)
            {
                previousScore = previousResult.OtherBatterScore.Value;
            }


            if (previousScore == -1 || currentScore == -1)
            {
                return false;
            }

            if (previousScore < milestone && currentScore >= milestone)
            {
                return true;
            }
            
            return false;
        }

        public static bool IsTeamMilestone(int milestone, CricinfoMatchDetails previousResult,
            CricinfoMatchDetails currentResult)
        {
            if (previousResult.BattingTeam != currentResult.BattingTeam)
            {
                return false;
            }

            if (previousResult.BattingTeamRuns < milestone && currentResult.BattingTeamRuns >= milestone)
            {
                return true;
            }

            return false;
        }

        public static bool IsTeamJustDeclared(CricinfoMatchDetails previousResult, CricinfoMatchDetails currentResult)
        {
            return previousResult.BattingTeamDeclared != currentResult.BattingTeamDeclared && currentResult.BattingTeamDeclared;
        }

        public static bool HasStateJustChanged(CricinfoMatchDetails previousResult, CricinfoMatchDetails currentResult)
        {
            return (previousResult.State != currentResult.State) ;
        }

        public static List<string> GetMessages(CricinfoMatchDetails previousResult, CricinfoMatchDetails currentResult)
        {

            var result = new List<string>();

            // If the game has just started, that's the only information we can get
            if (previousResult == null && currentResult != null)
            {
                var sydneyTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Sydney");

                if (sydneyTime.Hour < 12)
                {
                    result.Add("Morning, everyone");
                }
                else
                {
                    result.Add("Afternoon, everyone");
                }

                result.Add("We start a new game for Australia");
            }

            // If the team has changed, that's the only information we can get from it
            if (previousResult.BattingTeam != currentResult.BattingTeam)
            {
                var message = string.Format("{0} comes into bat", currentResult.BattingTeam);
                result.Add(message);
                return result;
            }

            // Has there been a wicket?
            if (previousResult.BattingTeamOuts < currentResult.BattingTeamOuts)
            {
                // who was it?
                var batter = IsBatterOut(previousResult.CurrentBatter, currentResult)
                    ? previousResult.CurrentBatter
                    : previousResult.OtherBatter;

                var score = IsBatterOut(previousResult.CurrentBatter, currentResult)
                    ? previousResult.CurrentBatterScore
                    : previousResult.OtherBatterScore;

                var message = string.Format("OUT! {0} on {1}", batter, score);

                result.Add(message);
            }

            // Has a batter for a four or a six?
            foreach (var batter in currentResult.Batters)
            {
                if (IsBatterFour(batter, previousResult, currentResult))
                {
                    result.Add($"And that's a *four* for {batter}");
                }

                if (IsBatterSix(batter, previousResult, currentResult))
                {
                    result.Add($"And that's a *six* for {batter}");
                }
            }

            // Has there been a batter milestone for the current batter?
            for (int milestone = 50; milestone < 300; milestone += 50)
            {
                foreach (var batter in currentResult.Batters)
                {
                    if (IsBatterMilestone(batter, milestone, previousResult, currentResult))
                    {
                        var message = string.Format("{0} for {1}!", milestone, batter);

                        result.Add(message);
                    }
                }
            }

            // Has there been a milestone for the team?
            for (int milestone = 50; milestone < 700; milestone += 50)
            {
                if (IsTeamMilestone(milestone, previousResult, currentResult))
                {
                    var message = string.Format("{0} has made it to {1}",
                        currentResult.BattingTeam, milestone);

                    result.Add(message);
                }
            }

            // Has the team declared?
            if (IsTeamJustDeclared(previousResult, currentResult))
            {
                var message = string.Format("{0} has declared", currentResult.BattingTeam);

                result.Add(message);
            }
            
            // Has the state changed
            if (HasStateJustChanged(previousResult, currentResult))
            {
                var state = currentResult.State;

                if (!string.IsNullOrWhiteSpace(state))
                {
                    var message = string.Format("State has changed to {0}", state);

                    switch(state)
                    {
                        case STATE_MATCH_OVER:
                            message = "And that's it folks, the *match is over*";
                            break;
                        case STATE_TEA:
                            message = "The boys have gone off for *tea*, and they will be back shortly.";
                            break;
                        case STATE_RAIN_DELAY:
                            message = "Unfortunately there's a bit of a *rain delay* going on, we will hopefully be back shortly.";
                            break;
                        case STATE_INNINGS_BREAK:
                            message = "That's the *end of the innings*!";
                            break;
                        case STATE_LUNCH:
                            message = "We're just off to grab some *lunch*, we'll be back in a bit";
                            break;
                        case STATE_DRINKS:
                            message = "Just a quick couple of minutes for *drinks*";
                            break;
                        case STATE_STUMPS:
                            message = "That's it for the day!";
                            break;
                    }

                    if (state == STATE_PLAY)
                    {
                        switch (previousResult.State)
                        {
                            case STATE_TEA:
                                message = "We're back from the Tea break";
                                break;
                            case STATE_RAIN_DELAY:
                                message = "Fortunately we've been able to get back from the rain delay";
                                break;
                            case STATE_INNINGS_BREAK:
                                message = $"And we're into a marvelous *new* innings as {currentResult.BattingTeam} come into bat!";
                                break;
                            default:
                                message = "And we're back into play";
                                break;
                        }
                    }

                    result.Add(message);
                }
            }

            return result;
        }
    }
}
