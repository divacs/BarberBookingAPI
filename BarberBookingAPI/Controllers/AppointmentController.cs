using BarberBookingAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace BarberBookingAPI.Controllers
{
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public AppointmentController(ApplicationDBContext context)
        {
                _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var appointment = _context.Appointments.ToList();

            return Ok(appointment);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var appointment = _context.Appointments.Find(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }
    }
}
