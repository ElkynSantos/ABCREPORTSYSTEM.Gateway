using ABCREPORTSYSTEM.Sucursal.Models;
using Microsoft.EntityFrameworkCore;

namespace ABCREPORTSYSTEM.Sucursal.Services
{

    
    public interface IAutomobiles
        {
        Task<Automobile> GetAutomobileByIdAsync(Guid vin);
        }
        

    
}
