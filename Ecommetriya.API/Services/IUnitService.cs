using Ecommetriya.API.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Ecommetriya.API.Services
{
    //service
    public interface IUnitService
    {
        Task<IEnumerable<Unit>> GetAllUnitsAsync();
        Task<Unit> GetUnitByIdAsync(int id);
        Task AddUnitAsync(Unit unit);
        Task UpdateUnitAsync(Unit unit);
        Task DeleteUnitAsync(int id);
    }

    public class UnitService : IUnitService
    {
        private readonly ApplicationDbContext _context;

        public UnitService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Unit>> GetAllUnitsAsync()
        {
            return await _context.Unit.ToListAsync();
        }

        public async Task<Unit> GetUnitByIdAsync(int id)
        {
            return await _context.Unit.FindAsync(id);
        }

        public async Task AddUnitAsync(Unit unit)
        {
            _context.Unit.Add(unit);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUnitAsync(Unit unit)
        {
            _context.Unit.Update(unit);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUnitAsync(int id)
        {
            var unit = await _context.Unit.FindAsync(id);
            if (unit != null)
            {
                _context.Unit.Remove(unit);
                await _context.SaveChangesAsync();
            }
        }
    }
}
