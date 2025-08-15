using System.ComponentModel.DataAnnotations;

namespace BarberBookingAPI.DTOs.Apointment
{
    public class UpdateAppointmentRequestDto
    {
        [Required]
        public DateTime StartTime { get; set; } // Start time of the appointment

        [Required]
        public int Duration { get; set; } // End time of the appointment
        public DateTime EndTime => StartTime.AddMinutes(Duration); 
        public string ApplicationUserId { get; set; } = string.Empty;
        public int BarberServiceId { get; set; }

        // Custom validation to ensure the appointment is within working hours

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var opening = new TimeSpan(10, 0, 0);  // 10:00 AM
            var closing = new TimeSpan(20, 0, 0);  // 8:00 PM

            if (StartTime.TimeOfDay < opening)
            {
                yield return new ValidationResult(
                    "Start time must be after 10:00 AM.",
                    new[] { nameof(StartTime) }
                );
            }

            if (EndTime.TimeOfDay > closing)
            {
                yield return new ValidationResult(
                    "End time must be before 8:00 PM.",
                    new[] { nameof(StartTime), nameof(Duration) }
                );
            }

            if (StartTime >= EndTime)
            {
                yield return new ValidationResult(
                    "Start time must be earlier than end time.",
                    new[] { nameof(StartTime), nameof(Duration) }
                );
            }
        }
    }
}
