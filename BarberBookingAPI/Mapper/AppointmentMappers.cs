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
                EndTime = appointment.EndTime
               
            };
        }
    }
}
