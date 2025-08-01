﻿using BarberBookingAPI.Data;
using BarberBookingAPI.DTOs.Apointment;
using BarberBookingAPI.DTOs.BarberService;
using BarberBookingAPI.Interfaces;
using BarberBookingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BarberBookingAPI.Repository
{
    public class BarberServiceRepository : IBarberServiceRepository
    {
        private readonly ApplicationDBContext _context;
        public BarberServiceRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<BarberService> CreateBarberServiceAsync(BarberService barberServiceModel)
        {
            await _context.BarberServices.AddAsync(barberServiceModel);
            await _context.SaveChangesAsync();
            return barberServiceModel;
        }

        public async Task<BarberService?> DeleteAsync(int id)
        {
            var barberServiceModel = await _context.BarberServices.FindAsync(id);

            if (barberServiceModel == null)
            {
                return null;
            }

            _context.BarberServices.Remove(barberServiceModel);
            await _context.SaveChangesAsync();
            return barberServiceModel;
        }

        public async Task<List<BarberService>> GetAllAsnc()
        {
            return await _context.BarberServices.ToListAsync();
        }


        public async Task<List<BarberService>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.BarberServices
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<BarberService?> GetByIdAsync(int id)
        {
            return await _context.BarberServices.FindAsync(id);
        }

        public async Task<BarberService?> UpdateAsync(int id, UpdateBarberServiceRequestDto barberServiceModel)
        {
            var existingBarberService = await _context.BarberServices.FindAsync(id);

            existingBarberService.Name = barberServiceModel.Name;
            existingBarberService.Duration = barberServiceModel.Duration;
            existingBarberService.Price = barberServiceModel.Price;

            await _context.SaveChangesAsync();

            return existingBarberService;
        }

    }
}
