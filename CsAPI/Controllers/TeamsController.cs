using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<Team> _teamsCollection;

        public TeamController(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            var database = _mongoClient.GetDatabase("test");
            _teamsCollection = database.GetCollection<Team>("teams");
        }
        [HttpDelete]
        public IActionResult DeleteTeams(string teams)
        {
            if (string.IsNullOrEmpty(teams))
            {
                return BadRequest("Invalid teams");
            }

            var team = _teamsCollection.FindOneAndDelete(u => u.Name == teams);

            if (team == null)
            {
                return NotFound("Teams not found");
            }

            return Ok($"Teams '{teams}' was deleted");
        }
        [HttpPost]
        [Route("setteam")]
        public async Task<IActionResult> SetTeam([FromBody] UpdateTeamRequest request)
        {
            long chatId = request.ChatId;

            string name = request.NewName;

            var existingUser = await _teamsCollection.Find(u => u.ChatId == chatId).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                existingUser.Name = name;
                await _teamsCollection.ReplaceOneAsync(u => u.Id == existingUser.Id, existingUser);
            }
            else
            {
                var newTeam = new Team
                {
                    ChatId = chatId,
                    Name = name
                };
                await _teamsCollection.InsertOneAsync(newTeam);
            }
            return Ok();
        }
        [HttpPut]
        [Route("updateteam")]
        public async Task<IActionResult> UpdateTeam([FromBody] UpdateTeamRequest request)
        {
            long chatId = request.ChatId;

            string newName = request.NewName;

            var existingUser = await _teamsCollection.Find(u => u.ChatId == chatId).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                existingUser.Name = newName;
                await _teamsCollection.ReplaceOneAsync(u => u.Id == existingUser.Id, existingUser);
            }
            return Ok();
        }
        [HttpGet]
        public IActionResult GetTeam(string teams)
        {
            if (string.IsNullOrEmpty(teams))
            {
                return BadRequest("Invalid team");
            }
            var team = _teamsCollection.Find(u => u.Name == teams).FirstOrDefault();
            if (team == null)
            {
                return NotFound("team not found");
            }
            return Ok(team.Name);
        }
    }
}