using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace CsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<User> _usersCollection;

        public UserController(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;

            var database = _mongoClient.GetDatabase("test");
            _usersCollection = database.GetCollection<User>("users");
        }

        [HttpPost]
        [Route("setusername")]
        public async Task<IActionResult> SetUsername([FromBody] UpdateUsernameRequest request)
        {
            long chatId = request.ChatId;

            string username = request.NewUsername;

            var existingUser = await _usersCollection.Find(u => u.ChatId == chatId).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                existingUser.Username = username;
                await _usersCollection.ReplaceOneAsync(u => u.Id == existingUser.Id, existingUser);
            }
            else
            {
                var newUser = new User
                {
                    ChatId = chatId,
                    Username = username
                };
                await _usersCollection.InsertOneAsync(newUser);
            }

            return Ok();
        }
        [HttpPut]
        [Route("updateusername")]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameRequest request)
        {
            long chatId = request.ChatId;

            string newUsername = request.NewUsername;

            var existingUser = await _usersCollection.Find(u => u.ChatId == chatId).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                existingUser.Username = newUsername;
                await _usersCollection.ReplaceOneAsync(u => u.Id == existingUser.Id, existingUser);
            }

            return Ok();
        }
        [HttpGet]
        public IActionResult GetUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Invalid username");
            }

            var user = _usersCollection.Find(u => u.Username == username).FirstOrDefault();

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user.Username);
        }
        [HttpDelete]
        public IActionResult DeleteUsername(string username)
        {

            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Invalid username");
            }

            var user = _usersCollection.FindOneAndDelete(u => u.Username == username);


            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok($"Username '{username}' was deleted");
        }
    }
}