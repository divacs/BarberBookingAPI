namespace BarberBookingAPI.DTOs.Apointment
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }  // Ovo polje iz Appointment modela
        public int WorkerId { get; set; }
    }
}
