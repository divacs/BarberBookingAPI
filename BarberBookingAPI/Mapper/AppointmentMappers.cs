using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.Models;

namespace BarberBookingAPI.Mapper
{
    public static class AppointmentMappers 
    {
        public static AppointmentDto ToAppointmentDto(this Appointment appointment)
        {
            return new AppointmentDto
            {
                Id = appointment.Id,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                WorkerId = int.TryParse(appointment.WorkerId, out var workerId) ? workerId : 0 // Safely parse WorkerId to int, default to 0 if parsing fails
            };
        }
    }
}
