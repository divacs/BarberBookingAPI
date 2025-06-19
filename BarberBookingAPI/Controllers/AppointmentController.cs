using BarberBookingAPI.Data;
using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.Mapper;
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
            var appointment = _context.Appointments.ToList().Select(a => a.ToAppointmentDto());

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

            return Ok(appointment.ToAppointmentDto());
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateAppointmentRequestDto appointmentDto)
        {
            if (appointmentDto == null)
            {
                return BadRequest("Appointment data is required.");
            }
            // Validate that the selected barber service exists in the database
            if (!_context.BarberServices.Any(s => s.Id == appointmentDto.BarberServiceId))
            {
                return BadRequest("Selected service does not exist.");
            }

            // Validate that the specified user exists in the database
            if (!_context.Users.Any(u => u.Id == appointmentDto.ApplicationUserId))
            {
                return BadRequest("Selected user does not exist.");
            }
            var appointment = appointmentDto.ToAppointmentFromCreateDto();
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment.ToAppointmentDto());
        }
    }
}
