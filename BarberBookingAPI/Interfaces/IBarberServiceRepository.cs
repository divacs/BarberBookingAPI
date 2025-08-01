﻿using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.DTOs.BarberService;
using BarberBookingAPI.Models;

namespace BarberBookingAPI.Interfaces
{
    public interface IBarberServiceRepository
    {
        Task<List<BarberService>> GetAllAsync(int pageNumber, int pageSize);
        Task<List<BarberService>> GetAllAsnc();
        Task<BarberService?> GetByIdAsync(int id);
        Task<BarberService> CreateBarberServiceAsync(BarberService barberService);
        Task<BarberService?> UpdateAsync(int id, UpdateBarberServiceRequestDto barberService);
        Task<BarberService?> DeleteAsync(int id);
    }
}
