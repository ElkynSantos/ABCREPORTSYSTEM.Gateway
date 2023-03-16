using ABCREPORTSYSTEM.Sucursal.Models;

namespace ABCREPORTSYSTEM.Sucursal.Services
{
    public interface IEmpleado
    {
        Task<Employee> GetemployeeByIdAsync(string username);
    }
}
