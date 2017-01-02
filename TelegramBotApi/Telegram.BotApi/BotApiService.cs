using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.BotApi.Model;

namespace Telegram.BotApi
{
    public class BotApiService
    {
        private const string baseUrl = "https://api.telegram.org/bot{0}/{1}";

        private readonly string token;
        private long currentUpdateId = -1;
        private CancellationTokenSource tokenSource;

        private BotApiService() { }

        public BotApiService(string token)
        {
            this.token = token;
        }

        public event EventHandler<TUpdateEventArgs> UpdateReceived;        

        /// <summary>
        /// Starts long polling requests using Telegram API "getUpdates" method.
        /// UpdateReceived event is raised on every new update found.
        /// </summary>
        public void Start()
        {
            tokenSource = new CancellationTokenSource();
            var cancellationToken = tokenSource.Token;
                   
            Task.Run(() =>
            {
                try
                {
                    ProcessUpdates(GetUpdates());
                    while (!cancellationToken.IsCancellationRequested)
                        ProcessUpdates(GetUpdates(offset: currentUpdateId + 1, timeout: 10));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                    currentUpdateId++;
                    Stop();
                    Start();
                }
            },
            cancellationToken);
        }

        public void Stop()
        {
            tokenSource?.Cancel();
        }

        private void ProcessUpdates(IEnumerable<TUpdate> updates)
        {
            foreach (var update in updates)
            {
                currentUpdateId = update.Id;
                OnUpdateReceived(new TUpdateEventArgs() { Update = update });
            }
        }

        protected virtual void OnUpdateReceived(TUpdateEventArgs e)
        {
            var temp = Volatile.Read(ref UpdateReceived);
            if (temp != null)
                UpdateReceived(this, e);
        }

        public class TUpdateEventArgs : EventArgs
        {
            public TUpdate Update { get; set; }
        }

        #region Methods

        /// <summary>
        /// Use this method to receive incoming updates using long polling.
        /// </summary>
        /// <param name="offset">Identifier of the first update to be returned. Must be greater by one than the highest among the identifiers of previously received updates.</param>
        /// <param name="limit">Limits the number of updates to be retrieved. Values between 1—100 are accepted. Defaults to 100.</param>
        /// <param name="timeout">Timeout in seconds for long polling. Defaults to 0, i.e. usual short polling.</param>
        /// <returns>List of deserialized Update objects.</returns>
        public IList<TUpdate> GetUpdates(long offset = -1, int limit = 100, int timeout = 0)
        {
            var _offset = (offset == -1) ? string.Empty : offset.ToString();

            var request = string.Format(baseUrl, token, $@"getUpdates?offset={_offset}
                                                                     &limit={limit}
                                                                     &timeout={timeout}");

            var json = GetResponse(request);

            var response = JsonConvert.DeserializeObject<TResponse>(json);
            return response.Result;
        }

        /// <summary>
        /// Use this method to send text messages.
        /// </summary>
        /// <param name="chatId">Unique identifier for the message recipient — User or GroupChat id.</param>
        /// <param name="text">Text of the message to be sent.</param>
        /// <param name="disableWebPagePreview">Disables link previews for links in this message.</param>
        /// <param name="replyToMessageId">If the message is a reply, ID of the original message.</param>
        /// <param name="replyMarkup">Additional interface options. A JSON-serialized object for a custom reply keyboard, instructions to hide keyboard or to force a reply from the user.</param>
        /// <returns>On success, the sent Message is returned.</returns>
        public TSendMessageResult SendMessage(int chatId, string text, bool disableWebPagePreview = false, int replyToMessageId = -1, object replyMarkup = null)
        {
            var _replyToMessageId = (replyToMessageId == -1) ? string.Empty : replyToMessageId.ToString();

            var _replyMarkup = (replyMarkup == null) ? string.Empty
                                                     : JsonConvert.SerializeObject(replyMarkup);

            var request = string.Format(baseUrl, token, $@"sendMessage?chat_id={chatId}
                                                                      &text={text}
                                                                      &disable_web_page_preview={disableWebPagePreview}
                                                                      &reply_to_message_id={_replyToMessageId}
                                                                      &reply_markup={_replyMarkup}");

            var json = GetResponse(request);

            return JsonConvert.DeserializeObject<TSendMessageResult>(json);
        }

        /// <summary>
        /// Use this method to forward messages of any kind.
        /// </summary>
        /// <param name="chatId">Unique identifier for the message recipient — User or GroupChat id.</param>
        /// <param name="fromChatId">Unique identifier for the chat where the original message was sent — User or GroupChat id.</param>
        /// <param name="messageId">Unique message identifier.</param>
        /// <returns>On success, the sent Message is returned.</returns>
        public TSendMessageResult ForwardMessage(int chatId, int fromChatId, int messageId)
        {
            var request = string.Format(baseUrl, token, $@"forwardMessage?chat_id={chatId}
                                                                         &from_chat_id={fromChatId}
                                                                         &message_id={messageId}");

            var json = GetResponse(request);

            return JsonConvert.DeserializeObject<TSendMessageResult>(json);
        }

        #endregion

        #region Utils

        private static string GetResponse(string request)
        {
            using (var client = new WebClient())
                return client.DownloadString(request);
        }

        #endregion
    }
}
