using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CsAPI
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public long ChatId { get; set; }
        public string Username { get; set; }
    }
    public class UpdateUsernameRequest
    {
        public long ChatId { get; set; }
        public string NewUsername { get; set; }
    }
}
