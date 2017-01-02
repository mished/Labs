using System.Collections.Generic;

namespace Telegram.BotApi.Model
{
    public class TResponse
    {
        public bool Ok { get; set; }
        public List<TUpdate> Result { get; set; }
    }
    
    public class TSendMessageResult
    {
        public bool Ok { get; set; }
        public TUpdate Result { get; set; }
    }
}
