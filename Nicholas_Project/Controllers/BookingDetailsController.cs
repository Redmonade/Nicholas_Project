using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nicholas_Project.Data;
using Nicholas_Project.Models;
using System.Linq;

namespace Nicholas_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BookingDetails
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.BookingDetails.ToList());
        }

        // GET: api/BookingDetails/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var bookingDetail = _context.BookingDetails.Find(id);
            if (bookingDetail == null)
            {
                return NotFound();
            }
            return Ok(bookingDetail);
        }


        // GET: api/BookingDetails/user
        [HttpGet("user")]
        public IActionResult GetUserBookings()
        {

            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            // Log the username for debugging
            Console.WriteLine($"Fetching bookings for user: {username}");

            var bookings = _context.BookingDetails
                .Where(b => b.BookedBy == username)
                .ToList();

            // Log the number of bookings found
            Console.WriteLine($"Number of bookings found: {bookings.Count}");

            return Ok(bookings);
        }

        // POST: api/BookingDetails
        [HttpPost]
        public IActionResult Create([FromBody] BookingDetail bookingDetail)
        {
            if (bookingDetail == null)
            {
                return BadRequest();
            }

            // Set the BookedBy field to the logged-in user
            bookingDetail.BookedBy = User.Identity.Name;

            _context.BookingDetails.Add(bookingDetail);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = bookingDetail.BookingId }, bookingDetail);
        }

        // PUT: api/BookingDetails/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] BookingDetail updatedBookingDetail)
        {
            if (updatedBookingDetail == null || updatedBookingDetail.BookingId != id)
            {
                return BadRequest();
            }

            var existingBookingDetail = _context.BookingDetails.Find(id);
            if (existingBookingDetail == null)
            {
                return NotFound();
            }

            // Update fields
            existingBookingDetail.FacilityDescription = updatedBookingDetail.FacilityDescription;
            existingBookingDetail.BookoingDateFrom = updatedBookingDetail.BookoingDateFrom;
            existingBookingDetail.BookingDateTo = updatedBookingDetail.BookingDateTo;

            // Check if the user is an admin or a regular user
            if (User.IsInRole("User")) // Assuming role name for regular users is "User"
            {
                // Set BookedBy to the logged-in user for regular users only
                existingBookingDetail.BookedBy = User.Identity.Name;
            }
            // If the user is an admin, don't modify the BookedBy field

            existingBookingDetail.Status = updatedBookingDetail.Status;

            _context.BookingDetails.Update(existingBookingDetail);
            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/BookingDetails/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var bookingDetail = _context.BookingDetails.Find(id);
            if (bookingDetail == null)
            {
                return NotFound();
            }

            _context.BookingDetails.Remove(bookingDetail);
            _context.SaveChanges();

            return NoContent();
        }

        // GET: api/BookingDetails/filter
        [HttpGet("filter")]
        public IActionResult GetFiltered([FromQuery] string room = null, [FromQuery] string name = null, [FromQuery] string status = null)
        {
            var query = _context.BookingDetails.AsQueryable();

            // Filter by room if it's provided
            if (!string.IsNullOrEmpty(room))
            {
                query = query.Where(b => b.FacilityDescription == room);
            }

            // Filter by name if it's provided
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(b => b.BookedBy.Contains(name));
            }

            // Filter by status if it's provided
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(b => b.Status == status);
            }

            // Return the filtered list
            return Ok(query.ToList());
        }

        // GET: api/BookingDetails/filter/user
        [HttpGet("filter/user")]
        public IActionResult GetFilteredUserBookings([FromQuery] string room = null, [FromQuery] string status = null)
        {
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var query = _context.BookingDetails
                .Where(b => b.BookedBy == username) // Only return bookings for the logged-in user
                .AsQueryable();

            // Filter by room if it's provided
            if (!string.IsNullOrEmpty(room))
            {
                query = query.Where(b => b.FacilityDescription == room);
            }

            // Filter by status if it's provided
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(b => b.Status == status);
            }

            // Return the filtered list
            return Ok(query.ToList());
        }
    }
}
