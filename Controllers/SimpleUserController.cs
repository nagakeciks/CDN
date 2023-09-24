using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static HafizDemoAPI.Controllers.UserController;
using HafizDemoAPI.ModelCDN;
using Microsoft.EntityFrameworkCore;

namespace HafizDemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimpleUserController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetUsers()
        {
            using (CDNContext cdnCtxt = new())
            {
                var users = cdnCtxt.Simpleuser.ToList();
                return Ok(users);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            using (CDNContext cdnCtxt = new())
            {
                var user = cdnCtxt.Simpleuser.Where(u => u.Id == id).FirstOrDefault();
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] SimpleUser newUser)
        {
            using (CDNContext cdnCtxt = new())
            {
                var user = new ModelCDN.Simpleuser
                {
                    Username = newUser.Username,
                    Mail = newUser.mail,
                    Phoneno = newUser.phoneno,
                    Skills = newUser.skills,
                    Hobbies = newUser.hobbies
                };
                cdnCtxt.Simpleuser.Add(user);
                cdnCtxt.SaveChanges();
                return Ok(user);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] SimpleUser updatedUser)
        {
            using (CDNContext cdnCtxt = new())
            {
                var recordUpdated = cdnCtxt.Simpleuser.Where(u => u.Id == id).ExecuteUpdate(setters => setters
                .SetProperty(b => b.Username, updatedUser.Username)
                .SetProperty(b => b.Mail, updatedUser.mail)
                .SetProperty(b => b.Phoneno, updatedUser.phoneno)
                .SetProperty(b => b.Skills, updatedUser.skills)
                .SetProperty(b => b.Hobbies, updatedUser.hobbies)
                );
                if (recordUpdated > 0)
                {
                    return Ok(recordUpdated);
                }
                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            using (CDNContext cdnCtxt = new())
            {
                var recordDeleted = cdnCtxt.Simpleuser.Where(u => u.Id == id).ExecuteDelete();
                if (recordDeleted > 0)
                {
                    return Ok(recordDeleted);
                }
                return NoContent();
            }
        }

        public class SimpleUser
        {
            public string Username { get; set; }
            public string mail { get; set; }
            public string phoneno { get; set; }
            public string skills { get; set; }
            public string hobbies { get; set; }
        }
    }
}
  