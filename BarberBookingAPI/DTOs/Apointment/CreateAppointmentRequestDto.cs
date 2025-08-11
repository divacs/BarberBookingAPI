using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BarberBookingAPI.DTOs.Appointment
{
    public class CreateAppointmentRequestDto : IValidatableObject
    {
        [Required]
        public DateTime StartTime { get; set; } // Start time of the appointment

        public int Duration { get; set; } // Duration in minutes, nullable

        public string ApplicationUserId { get; set; } = string.Empty;

        public int BarberServiceId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            DateTime endTime = StartTime.AddMinutes(Duration);

            var opening = new TimeSpan(10, 0, 0);  // 10:00 AM
            var closing = new TimeSpan(20, 0, 0);  // 8:00 PM

            if (StartTime.TimeOfDay < opening)
            {
                yield return new ValidationResult(
                    "Start time must be after 10:00 AM.",
                    new[] { nameof(StartTime) }
                );
            }

            if (endTime.TimeOfDay > closing)
            {
                yield return new ValidationResult(
                    "End time must be before 8:00 PM.",
                    new[] { nameof(StartTime), nameof(Duration) }
                );
            }

            if (StartTime >= endTime)
            {
                yield return new ValidationResult(
                    "Start time must be earlier than end time.",
                    new[] { nameof(StartTime), nameof(Duration) }
                );
            }
        }
    }
}
