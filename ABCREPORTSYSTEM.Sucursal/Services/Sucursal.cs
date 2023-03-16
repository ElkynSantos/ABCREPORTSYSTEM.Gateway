using ABCREPORTSYSTEM.Sucursal.Models;
using Microsoft.EntityFrameworkCore;

namespace ABCREPORTSYSTEM.Sucursal.Services
{
    public class Sucursal : ISucursal
    {

        private readonly AbcdataBaseContext _context;



        public Sucursal(AbcdataBaseContext context)
        {
            _context = context;
        }
        public async Task<BranchOffice> GetBranchOfficeByIdAsync(int branchOfficeId)
        {
            return await _context.BranchOffices.SingleOrDefaultAsync(x => x.BranchOfficeId == branchOfficeId);
        }
    }
}
