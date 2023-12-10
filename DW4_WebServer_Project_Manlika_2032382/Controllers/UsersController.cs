using DW4_WebServer_Project_Manlika_2032382.Data;
using DW4_WebServer_Project_Manlika_2032382.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DW4_WebServer_Project_Manlika_2032382.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult Post(Dictionary<string, string> listOfUsers)
        {
            if(listOfUsers.Count == 2) // 2 are email and password to login
            {
                var email = listOfUsers["email"];
                var password = listOfUsers["password"];

                var userChecked = _context.Users.FirstOrDefault(x => x.Email == email);

                if (userChecked == null)
                {
                    return BadRequest("Email is incorrect or User does not exist");
                }

                Session newSession = new Session(email);
                _context.Add(newSession);

                //Validate the password
                if (userChecked.CheckPassword(password))
                {
                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }
                }
                else
                {
                    return BadRequest("Password is incorrect, please try again...");
                }

                //get the auto generated token number: for example "1d52b561dse3d366fb561ds"
                return Ok(newSession.Token);

            }
            else
            {
                //Create a new user
                var name = listOfUsers["name"];
                var email = listOfUsers["email"];
                var password = listOfUsers["password"];

                var user = _context.Users.FirstOrDefault(x => x.Email == email);

                if (user != null)
                {
                    return BadRequest("This email already existed");
                }

                //If the email is valid -> Add a new user -> Save
                User newUser = new User(name, email, password);
                _context.Add(newUser);

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

                return Ok(newUser);
            }
        }
    }
}
