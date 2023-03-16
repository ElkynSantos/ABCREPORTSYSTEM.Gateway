using ABCREPORTSYSTEM.Sucursal.Models;
using Microsoft.EntityFrameworkCore;

namespace ABCREPORTSYSTEM.Sucursal.Services
{
    public class Empleado : IEmpleado
    {
        private readonly AbcdataBaseContext _context;



        public Empleado(AbcdataBaseContext context)
        {
            _context = context;
        }
        public async Task<Employee> GetemployeeByIdAsync(string username)
        {
            return await _context.Employees.SingleOrDefaultAsync(x => x.Username == username);
        }
    }
}
