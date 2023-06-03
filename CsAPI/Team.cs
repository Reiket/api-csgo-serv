using MongoDB.Bson;

namespace CsAPI
{
    public class Team
    {
        public ObjectId Id { get; set; }
        public long ChatId { get; set; }
        public string Name { get; set; }
    }
    public class UpdateTeamRequest
    {
        public long ChatId { get; set; }
        public string NewName { get; set; }
    }
}
