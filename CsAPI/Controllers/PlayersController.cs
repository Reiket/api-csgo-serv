using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace CsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<Player> _playersCollection;
        public PlayersController(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;

            var database = _mongoClient.GetDatabase("test");
            _playersCollection = database.GetCollection<Player>("players");
        }
        [HttpPost]
        [Route("setplayers")]
        public async Task<IActionResult> SetPlayer([FromBody] UpdatePlayerRequest request)
        {
            long chatId = request.ChatId;

            string name = request.NewName;

            var existingUser = await _playersCollection.Find(u => u.ChatId == chatId).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                existingUser.Name = name;
                await _playersCollection.ReplaceOneAsync(u => u.Id == existingUser.Id, existingUser);
            }
            else
            {
                var newPlayer = new Player
                {
                    ChatId = chatId,
                    Name = name
                };
                await _playersCollection.InsertOneAsync(newPlayer);
            }

            return Ok();
        }
        [HttpPut]
        [Route("updateplayers")]
        public async Task<IActionResult> UpdatePlayer([FromBody] UpdatePlayerRequest request)
        {

            long chatId = request.ChatId;

            string newName = request.NewName;

            var existingUser = await _playersCollection.Find(u => u.ChatId == chatId).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                existingUser.Name = newName;
                await _playersCollection.ReplaceOneAsync(u => u.Id == existingUser.Id, existingUser);
            }

            return Ok();
        }
        [HttpDelete]
        public IActionResult DeletePlayers(string players)
        {
            if (string.IsNullOrEmpty(players))
            {
                return BadRequest("Invalid players");
            }

            var player = _playersCollection.FindOneAndDelete(u => u.Name == players);

            if (player == null)
            {
                return NotFound("Players not found");
            }

            return Ok($"Players '{players}' was deleted");
        }
        
        [HttpGet]
        public IActionResult GetPlayers(string players)
        {

            if (string.IsNullOrEmpty(players))
            {
                return BadRequest("Invalid players");
            }


            var player = _playersCollection.Find(u => u.Name == players).FirstOrDefault();

            if (players == null)
            {
                return NotFound("Players not found");
            }

            return Ok(player.Name);
        }
    }
}
