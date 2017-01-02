using Telegram.BotApi;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web;
using System.Diagnostics;

namespace Telegram.YodaBot
{
    public class YodaBot
    {
        private const string notSupportedMessage = "Afraid, I am afraid, help you I cannot. Yeesssssss...";

        private BotApiService service;
        private BotMode mode = BotMode.Answers;

        public YodaBot() { }

        public void Start()
        {
            service = new BotApiService(Properties.Resources.BotToken);
            service.UpdateReceived += OnUpdateReceived;
            service.Start();
        }

        public void Stop()
        {
            service?.Stop();
        }

        private void OnUpdateReceived(object sender, BotApiService.TUpdateEventArgs e)
        {
            var chatId = e.Update.Message.Chat.Id;
            var text = e.Update.Message.Text;
            Trace.WriteLine($"Update: {e.Update.Id}\nMessage: {text}\nFrom: {e.Update.Message.From.UserName}");

            if (string.IsNullOrEmpty(text))
                service.SendMessage(chatId, notSupportedMessage);
            else
            {
                if (text.StartsWith("/"))
                {
                    switch (text)
                    {                        
                        case "/quotes":
                            mode = BotMode.Quotes; return;
                        case "/answers":
                        default:
                            mode = BotMode.Answers; return;
                    }
                }

                switch(mode)
                {
                    case BotMode.Quotes:
                        service.SendMessage(chatId, GetYodaSpeak(GetQuote(text)));
                        break;
                    case BotMode.Answers:
                    default:
                        service.SendMessage(chatId, GetYodaSpeak(GetAnswer(text)));
                        break;
                }
            }
        }

        private string GetYodaSpeak(string source)
        {
            var request = $"https://yoda.p.mashape.com/yoda?sentence={HttpUtility.UrlEncode(source)}";

            if (string.IsNullOrEmpty(source))
                return notSupportedMessage;

            return GetMashapeWebClient().DownloadString(request);
        }

        private string GetAnswer(string question)
        {
            var request = $"http://api.duckduckgo.com/?q={HttpUtility.UrlEncode(question)}&format=json";

            using (var client = new WebClient())
            {
                var json = client.DownloadString(request);
                return (string)JObject.Parse(json)["AbstractText"];
            }
        }

        private string GetQuote(string category)
        {
            var request = $"https://andruxnet-random-famous-quotes.p.mashape.com/cat={category}";

            var json = GetMashapeWebClient().DownloadString(request);
            var parsed = JObject.Parse(json);

            return (string)parsed["quote"] + "\n" + (string)parsed["author"];
        }

        private WebClient GetMashapeWebClient()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("X-Mashape-Key", Properties.Resources.MashapeKey);
                return client;
            }
        }

        private enum BotMode
        {
            Answers,
            Quotes
        }
    }
}
