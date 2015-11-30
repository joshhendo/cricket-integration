using CricketIntegration.ExternalServices.Slack.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CricketIntegration.ExternalServices.Slack
{
    public class MessageSender
    {
        public static string WebHookUrl = ConfigurationManager.AppSettings["CricketIntegration.ExternalServices.Slack.WebhookUrl"];

        public async void Send(string message, string score, string webHookUrl = null)
        {
            if (webHookUrl == null)
            {
                webHookUrl = WebHookUrl;
            }

            using (var client = new HttpClient())
            {
                var content = new Attachment();
                content.Fallback = $"{message}. The current score is {score}.";
                content.Fields = new List<Field>();

                var messageField = new Field();
                messageField.Value = message;
                messageField.Short = true;
                content.Fields.Add(messageField);

                var scoreField = new Field();
                scoreField.Title = "Score";
                scoreField.Value = score;
                scoreField.Short = true;
                content.Fields.Add(scoreField);

                var contentAsString = JsonConvert.SerializeObject(content);

                await client.PostAsync(webHookUrl, new StringContent(contentAsString));
            }
        }
    }
}
