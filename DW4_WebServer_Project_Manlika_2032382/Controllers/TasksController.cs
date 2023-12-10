using DW4_WebServer_Project_Manlika_2032382.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DW4_WebServer_Project_Manlika_2032382.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext db)
        {
            _context = db;
        }
        [HttpPost]
        public ActionResult Post(Dictionary<string, string> listOfTasks)
        {
            //Create a task
            var token = listOfTasks["token"];
            var description = listOfTasks["description"];
            var assignedToUid = listOfTasks["assignedToUid"];

            //Find the Token assigned in Token table
            var sessionChecked = _context.Sessions.FirstOrDefault(x => x.Token == token);

            if (sessionChecked is null)
            {
                return BadRequest("Token does not exist!");
            }

            //Find a user who logged in and id of the user who is going to get tis task
            var userCreatedTask = _context.Users.FirstOrDefault(x => x.Email == sessionChecked!.Email);
            var userAssignedTask = _context.Users.FirstOrDefault(x => x.Uid == assignedToUid);

            if (userAssignedTask is null)
            {
                return BadRequest("AssignedToUid does not exist!");
            }

            //Add a new task
            Model.Task newTask = new Model.Task(userCreatedTask!.Uid, userCreatedTask.Name, userAssignedTask.Uid, userAssignedTask.Name, description);
            _context.Add(newTask);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(newTask);
        }

        [HttpGet]
        [Route("createdby")]
        public ActionResult<IEnumerable<Model.Task>> CreatedBy(Dictionary<string, string> listOfToken)
        {
            //Get and validate the Token form the body request
            var token = listOfToken["token"];
            var checkTokenLogin = _context.Sessions.FirstOrDefault(x => x.Token == token);

            if (checkTokenLogin == null)
            {
                return BadRequest("Token does not exist!");
            }

            //Use Email from Token to identify CreatedByUid
            var findUser = _context.Users.FirstOrDefault(x => x.Email == checkTokenLogin!.Email);
            var listOfTasks = _context.Tasks.Where(x => x.CreatedByUid == findUser!.Uid).ToList();

            if (listOfTasks.Count == 0)
            {
                return BadRequest("No Tasks created");
            }

            return Ok(listOfTasks);
        }

        [HttpGet]
        [Route("assignedto")]
        public ActionResult<IEnumerable<Model.Task>> AssignedTo(Dictionary<string, string> listOfToken)
        {
            //Get and validate the Token form the body request
            var token = listOfToken["token"];
            var checkTokenLogin = _context.Sessions.FirstOrDefault(x => x.Token == token);

            if (checkTokenLogin == null)
            {
                return BadRequest("Token does not exist!");
            }

            //Use Email from Token to identify AssignedToUid
            var findUser = _context.Users.FirstOrDefault(x => x.Email == checkTokenLogin!.Email);
            var listOfTasks = _context.Tasks.Where(x => x.AssignedToUid == findUser!.Uid).ToList();

            if (listOfTasks.Count == 0)
            {
                return BadRequest("No Tasks created");
            }

            return Ok(listOfTasks);
        }

        [HttpPut("{taskUid}")]
        public ActionResult Put(string taskUid, Dictionary<string, dynamic> listOfTasks)
        {
            //Check the taskUid on the URL
            var findTaskUid = _context.Tasks.FirstOrDefault(x => x.TaskUid == taskUid);

            if (findTaskUid == null)
            {
                return NotFound("Task not found!");
            }

            //Check the taskUid on the request body
            string taskUidString = Convert.ToString(listOfTasks["taskUid"]);
            if (taskUid != taskUidString)
            {
                return BadRequest("Task ids do not match");
            }

            // Check the Token
            string token = Convert.ToString(listOfTasks["token"]);
            var findUserCreatedTask = _context.Sessions.FirstOrDefault(x => x.Token == token);

            if (findUserCreatedTask is null)
            {
                return BadRequest("Token does not exist!");
            }

            //Find a user who is assigned to the task by using Token (on the body request) / who login?
            var userAssignedTask = _context.Users.FirstOrDefault(x => x.Email == findUserCreatedTask!.Email);

            if (userAssignedTask!.Uid != findTaskUid.CreatedByUid)
            {
                return BadRequest("AssignedToUid does not exist!...Failure updating");
            }

            //Checking data information before update
            string createByUid = Convert.ToString(listOfTasks["createdByUid"]);
            string createdByName = Convert.ToString(listOfTasks["createdByName"]);
            string assignedToUid = Convert.ToString(listOfTasks["assignedToUid"]);
            string assignedToName = Convert.ToString(listOfTasks["assignedToName"]);
            if (findTaskUid.CreatedByUid != createByUid || findTaskUid.CreatedByName != createdByName ||
                findTaskUid.AssignedToUid != assignedToUid || findTaskUid.AssignedToName != assignedToName)
            {
                return BadRequest("Sorry!...'createdByUid' or 'createdByName' or "
                    + "'assignedToUid' or 'assignedToName' cannot be changed!");
            }

            //data is allow to update
            string description = Convert.ToString(listOfTasks["description"]);
            string doneString = Convert.ToString(listOfTasks["done"]);
            bool done = Convert.ToBoolean(doneString);

            //Save the modification
            findTaskUid.Description = description;
            findTaskUid.Done = done;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(findTaskUid);

        }

        [HttpDelete("{taskUid}")]
        public ActionResult Delete(string taskUid,Dictionary<string, string> listOfToken)
        {
            //Check the taskUid assigned on the URL
            var taskDelete = _context.Tasks.FirstOrDefault(x => x.TaskUid == taskUid);

            if (taskDelete == null)
            {
                return NotFound("Task not found!");
            }

            //Get the Token on the body request and check if it is valid
            var token = listOfToken["token"];
            var findUserLogin = _context.Sessions.FirstOrDefault(x => x.Token == token);

            if (findUserLogin is null)
            {
                return BadRequest("Token does not exist!");
            }

            //Find a user who created the task that assigned by Token (on the body request) / who login?
            var userCreatedTask = _context.Users.FirstOrDefault(x => x.Email == findUserLogin!.Email);

            if (userCreatedTask!.Uid != taskDelete.CreatedByUid)
            {
                return BadRequest("You did not create this task... Failure Deletion");
            }

            //Remove the specific task and save changed
            _context.Remove(taskDelete);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(taskDelete);
        }
    }
}
