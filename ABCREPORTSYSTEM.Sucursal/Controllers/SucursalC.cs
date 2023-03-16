using Microsoft.AspNetCore.Mvc;
using ABCREPORTSYSTEM.Sucursal.Models;
using ABCREPORTSYSTEM.Sucursal.Services;

namespace ABCREPORTSYSTEM.Sucursal.Controllers;



[ApiController]
[Route("[controller]")]


public class SucursalC : ControllerBase
{
    private readonly IAutomobiles _auto;
    private readonly ISucursal _Sucursal;
    private readonly IEmpleado _empleado;
    public SucursalC(IAutomobiles auto, ISucursal sucursal, IEmpleado empleado)
    {
     
        _auto = auto;   
        _Sucursal = sucursal;
        _empleado = empleado;   
    }

    [HttpGet("Employee/{username}/{id}")]
    public async Task<ActionResult<Employee>> GetEmployeById(string username, int id)
    {


        var employee = await _empleado.GetemployeeByIdAsync(username);

        var response = new Employee
        {
            EmployeeId = employee.EmployeeId,
            Username= employee.Username,
            FirstName= employee.FirstName,
            LastName= employee.LastName,
            BranchOfficeId= employee.BranchOfficeId,
        };

        if (employee is null)
        {
            return NotFound("No se encontro el empleado");
        }

        if (response.BranchOfficeId == id)
        {
            return Ok(response);
        }
        else
        {
            return BadRequest("ESTE EMPLEADO NO PERTENECE A ESTA SUCURSAL XD");
        }
      
     
    }

    [HttpGet("Automobile/{vin}/{id}")]
    public async Task<ActionResult<Automobile>> GetmobilebyId(Guid vin, int id){

        var auto= await _auto.GetAutomobileByIdAsync(vin);
        var sucursal = await _Sucursal.GetBranchOfficeByIdAsync(id);


        var responseauto = new Automobile
        {
            AutomobileId = auto.AutomobileId,
            Vin = auto.Vin,
            Make = auto.Make,
            Model = auto.Model,
            Year = auto.Year,
            BranchOfficeId = auto.BranchOfficeId,

        };

        var responsesucursal = new BranchOffice
        {
            BranchOfficeId = sucursal.BranchOfficeId,
            BranchOfficeCountry = sucursal.BranchOfficeCountry,
            BranchOfficeState= sucursal.BranchOfficeState,

        };

        if (auto is null && sucursal is null)
        {
            return NotFound("No se encontro ni la sucursal ni auto");
        }

        if(responseauto.BranchOfficeId == responsesucursal.BranchOfficeId)
        {
            return Ok("Si existe este auto en la sucursal");
        }
        else
        {
            return NotFound("No existe este auto en la sucursal");
        }
        

    }

}
