using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.Models;

namespace BarberBookingAPI.Mapper
{
    public static class AppointmentMappers 
    {
        public static AppointmentDto ToAppointmentDto(this Appointment appointmentModel)
        {
            return new AppointmentDto
            {
                Id = appointmentModel.Id,
                StartTime = appointmentModel.StartTime,
                EndTime = appointmentModel.EndTime,
                ReminderSent = appointmentModel.ReminderSent,
                ReminderJobId = appointmentModel.ReminderJobId,

            };
        }

        public static Appointment ToAppointmentFromCreateDto(this CreateAppointmentRequestDto appointmentDto)
        {
            return new Appointment
            {
                StartTime = appointmentDto.StartTime,
                EndTime = appointmentDto.EndTime,
                ApplicationUserId = appointmentDto.ApplicationUserId ?? string.Empty,
                BarberServiceId = appointmentDto.BarberServiceId
            };
        }
    }
}
