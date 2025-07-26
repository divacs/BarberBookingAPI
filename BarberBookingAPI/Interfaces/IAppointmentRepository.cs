using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.Models;

namespace BarberBookingAPI.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAllAsnc();
        Task<Appointment?> GetByIdAsync(int id);
        Task<Appointment> CreateAsync (Appointment appointmentModel);
        Task<Appointment?> UpdateAsync(int id, UpdateAppointmentRequestDto appointmentDto);
        Task<Appointment?> DeleteAsync(int id);
    }
}
