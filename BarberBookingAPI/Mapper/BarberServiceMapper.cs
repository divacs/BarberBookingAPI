﻿using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.DTOs.BarberService;
using BarberBookingAPI.Models;
using System.Runtime.CompilerServices;

namespace BarberBookingAPI.Mapper
{
    public static class BarberServiceMapper
    {
        public static BarberServiceDto ToBarberServiceDto(this BarberService barberServiceModel)
        {
            return new BarberServiceDto
            {
                Id = barberServiceModel.Id,
                Name = barberServiceModel.Name,
                Duration = barberServiceModel.Duration,
                Price = barberServiceModel.Price,

            };
        }
        public static BarberService ToBarberServiceFromCreateDto(this CreateBarberServiceRequestDto barberServiceDto)
        {
            return new BarberService
            {
                Name = barberServiceDto.Name,
                Duration = barberServiceDto.Duration,
                Price = barberServiceDto.Price,
            };
        }
    }
}
