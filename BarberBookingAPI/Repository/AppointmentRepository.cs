using BarberBookingAPI.Data;
using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberBookingAPI.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDBContext _context;
        public AppointmentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Appointment> CreateAsync(Appointment appointmentModel)
        {
            await _context.Appointments.AddAsync(appointmentModel);
            await _context.SaveChangesAsync();
            return appointmentModel;
        }

        public async Task<Appointment> DeleteAsync(int id)
        {
            var appointmentModel = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            if (appointmentModel == null)
            {
                return null;
            }

            _context.Appointments.Remove(appointmentModel);
            await _context.SaveChangesAsync();
            return appointmentModel;
        }

        public async Task<List<Appointment>> GetAllAsnc()
        {
            return await _context.Appointments.ToListAsync();
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            return await _context.Appointments.FindAsync(id);

        }

        public async Task<Appointment?> UpdateAsync(int id, UpdateAppointmentRequestDto appointmentDto)
        {
            var existingAppointment = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            existingAppointment.StartTime = appointmentDto.StartTime;
            existingAppointment.EndTime = appointmentDto.EndTime;
            existingAppointment.ApplicationUserId = appointmentDto.ApplicationUserId;
            existingAppointment.BarberServiceId = appointmentDto.BarberServiceId;

            await _context.SaveChangesAsync();

            return existingAppointment;
        }
    }
}
