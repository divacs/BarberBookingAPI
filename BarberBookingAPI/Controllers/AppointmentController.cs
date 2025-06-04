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
    }
}
