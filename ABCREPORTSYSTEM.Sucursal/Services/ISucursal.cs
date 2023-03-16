using ABCREPORTSYSTEM.Sucursal.Models;

namespace ABCREPORTSYSTEM.Sucursal.Services
{
    public interface ISucursal
    {
        Task<BranchOffice> GetBranchOfficeByIdAsync(int branchOfficeId);
    }
}
