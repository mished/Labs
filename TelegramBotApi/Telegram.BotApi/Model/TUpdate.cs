namespace Telegram.BotApi.Model
{
    using Newtonsoft.Json;
      
    public class TUpdate
    {
        [JsonProperty("update_id")]
        public int Id { get; set; }

        public TMessage Message { get; set; }
    }

    public class TMessage
    {
        [JsonProperty("message_id")]
        public int Id { get; set; }

        public TUser From { get; set; }
        public int Date { get; set; }
        public TChat Chat { get; set; }

        [JsonProperty("forward_from")]
        public TUser ForwardFrom { get; set; }

        [JsonProperty("forward_date")]
        public int ForwardDate { get; set; }

        [JsonProperty("reply_to_message")]
        public TMessage ReplyToMessage { get; set; }

        public string Text { get; set; }
        public TAudio Audio { get; set; }
        public TDocument Document { get; set; }
        public TPhotoSize[] Photo { get; set; }
        public TSticker Sticker { get; set; }
        public TVideo Video { get; set; }
        public string Caption { get; set; }
        public TContact Contact { get; set; }
        public TLocation Location { get; set; }

        [JsonProperty("new_chat_participant")]
        public TUser NewChatParticipant { get; set; }

        [JsonProperty("left_chat_participant")]
        public TUser LeftChatParticipant { get; set; }

        [JsonProperty("new_chat_title")]
        public string NewChatTitle { get; set; }

        [JsonProperty("new_chat_photo")]
        public TPhotoSize[] NewChatPhoto { get; set; }

        [JsonProperty("delete_chat_photo")]
        public bool DeleteChatPhoto { get; set; }

        [JsonProperty("group_chat_created")]
        public bool GroupChatCreated { get; set; }
    }

    public class TUser
    {
        public int Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        public string UserName { get; set; }
    }

    public class TGroupChat
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class TChat
    {
        public int Id { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Title { get; set; }
    }

    public class TPhotoSize
    {
        [JsonProperty("file_id")]
        public string Id { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        [JsonProperty("file_size")]
        public int FileSize { get; set; }
    }

    public class TAudio
    {
        [JsonProperty("file_id")]
        public string Id { get; set; }

        public int Duration { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }

        [JsonProperty("file_size")]
        public int FileSize { get; set; }
    }

    public class TDocument
    {
        [JsonProperty("file_id")]
        public string Id;

        public TPhotoSize Thumb { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }

        [JsonProperty("file_size")]
        public int FileSize { get; set; }
    }

    public class TSticker
    {
        [JsonProperty("file_id")]
        public string Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public TPhotoSize Thumb { get; set; }

        [JsonProperty("file_size")]
        public int FileSize { get; set; }
    }

    public class TVideo
    {
        [JsonProperty("file_id")]
        public string Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Duration { get; set; }
        public TPhotoSize Thumb { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }

        [JsonProperty("file_size")]
        public int FileSize { get; set; }
    }

    public class TContact
    {
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }
    }

    public class TLocation
    {
        public float Longitude { get; set; }
        public float Latitude { get; set; }
    }

    public class TInputFile
    {
    }

    public class TUserProfilePhotos
    {
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        public TPhotoSize[][] Photos { get; set; }
    }

    public class TReplyKeyboardMarkup
    {
        public string[][] Keyboard { get; set; }

        [JsonProperty("resize_keyboard")]
        public bool ResizeKeyboard { get; set; }

        [JsonProperty("one_time_keyboard")]
        public bool OneTimeKeyboard { get; set; }

        public bool Selective { get; set; }
    }

    public class TReplyKeyboardHide
    {
        [JsonProperty("hide_keyboard")]
        public bool HideKeyboard { get; set; }

        public bool Selective { get; set; }
    }

    public class TForceReply
    {
        [JsonProperty("force_reply")]
        public bool ForceReply { get; set; }

        public bool Selective { get; set; }
    }
}
