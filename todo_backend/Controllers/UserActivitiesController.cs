//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using todo_backend.Data;
//using todo_backend.Models;

//namespace MyApp.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class UserActivitiesController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public UserActivitiesController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/useractivities
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<UserActivity>>> GetActivities()
//        {
//            return await _context.Activities
//                //.Include(a => a.User) // żeby zwrócić też dane właściciela
//                .ToListAsync();
//        }

//        // GET: api/useractivities/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<UserActivity>> GetActivity(int id)
//        {
//            var activity = await _context.Activities
//                //.Include(a => a.User)
//                .FirstOrDefaultAsync(a => a.ActivityId == id);

//            if (activity == null) return NotFound();

//            return activity;
//        }

//        // POST: api/useractivities
//        [HttpPost]
//        public async Task<ActionResult<UserActivity>> CreateActivity(UserActivity activity)
//        {
//            _context.Activities.Add(activity);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetActivity), new { id = activity.ActivityId }, activity);
//        }

//        // PUT: api/useractivities/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateActivity(int id, UserActivity activity)
//        {
//            if (id != activity.ActivityId) return BadRequest();

//            _context.Entry(activity).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!_context.Activities.Any(e => e.ActivityId == id))
//                    return NotFound();
//                else
//                    throw;
//            }

//            return NoContent();
//        }

//        // DELETE: api/useractivities/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteActivity(int id)
//        {
//            var activity = await _context.Activities.FindAsync(id);
//            if (activity == null) return NotFound();

//            _context.Activities.Remove(activity);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}
