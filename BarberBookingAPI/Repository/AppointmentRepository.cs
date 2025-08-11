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
        public async Task<List<Appointment>> GetAllAsnc(int pageNumber, int pageSize)
        {
            return await _context.Appointments
                .Skip((pageNumber - 1) * pageSize) // Skip the correct number of records
                .Take(pageSize)                   // Take the specified number of records
                .ToListAsync();                   // Convert to a list asynchronously
        }

        public async Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date)
        {
            return await _context.Appointments
                .Where(a => a.StartTime.Date == date.Date)
                .ToListAsync();
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
        public async Task UpdateReminderJobIdAsync(int appointmentId, string jobId)
        {
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId);
            if (appointment == null) return;

            appointment.ReminderJobId = jobId;
            appointment.ReminderSent = false;

            await _context.SaveChangesAsync();
        }

    }
}
