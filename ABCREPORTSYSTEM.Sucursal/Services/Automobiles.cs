using ABCREPORTSYSTEM.Sucursal.Models;
using Microsoft.EntityFrameworkCore;

namespace ABCREPORTSYSTEM.Sucursal.Services
{
    public class Automobiles : IAutomobiles
    {
        private readonly AbcdataBaseContext _context;



        public Automobiles(AbcdataBaseContext context)
        {
            _context = context;
        }

        public async Task<Automobile> GetAutomobileByIdAsync(Guid vin)
        {
            return await _context.Automobiles.SingleOrDefaultAsync(x => x.Vin == vin);

        }
    }
}
