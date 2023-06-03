using MongoDB.Bson;

namespace CsAPI
{
    public class Player
    {
        public ObjectId Id { get; set; }
        public long ChatId { get; set; }
        public string Name { get; set; }
    }
    public class UpdatePlayerRequest
    {
        public long ChatId { get; set; }
        public string NewName { get; set; }
    }
}
