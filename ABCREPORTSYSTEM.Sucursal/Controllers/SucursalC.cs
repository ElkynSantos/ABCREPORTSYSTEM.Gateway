using Microsoft.AspNetCore.Mvc;
using ABCREPORTSYSTEM.Sucursal.Models;
using ABCREPORTSYSTEM.Sucursal.Services;
using System;

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

        string message2 = "No se encontro el users con el ID: " + username +" en la sucursal:"+ id;
        var employee = await _empleado.GetemployeeByIdAsync(username);
        if (employee is null)
        {
            return NotFound(message2);
        }
        var response = new Employee
        {
            EmployeeId = employee.EmployeeId,
            Username= employee.Username,
            FirstName= employee.FirstName,
            LastName= employee.LastName,
            BranchOfficeId= employee.BranchOfficeId,
        };

   
        if (response.BranchOfficeId == id)
        {
            return Ok();
        }
        else
        {
            return BadRequest("ESTE EMPLEADO NO PERTENECE A ESTA SUCURSAL");
        }
      
     
    }

    [HttpGet("Automobile/{vin}/{id}")]
    public async Task<ActionResult<Automobile>> GetmobilebyId(Guid vin, int id){

        var auto= await _auto.GetAutomobileByIdAsync(vin);
        var sucursal = await _Sucursal.GetBranchOfficeByIdAsync(id);
        string message = "No se encontro el auto con el ID: " + vin.ToString();
        string message2 = "No se encontro el auto con el ID: " + id.ToString();

        if (auto is null && sucursal is null)
        {
            return NotFound("No se encontro ni la sucursal ni auto");
        }

        if (auto is null)
        {
            return NotFound(message);
        }
        if ( sucursal is null)
        {
            return NotFound(message2);
        }
      
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

        

        if(responseauto.BranchOfficeId == responsesucursal.BranchOfficeId)
        {
            return Ok(null);
        }
        else
        {
            return NotFound("No existe este auto en la sucursal");
        }


        return Ok(null);
    }

    [HttpGet("Sucursal/{id}")]

    public async Task<ActionResult<Automobile>> GetsucursalbyId(Guid vin, int id)
    {

       
        var sucursal = await _Sucursal.GetBranchOfficeByIdAsync(id);


 

        var responsesucursal = new BranchOffice
        {
            BranchOfficeId = sucursal.BranchOfficeId,
            BranchOfficeCountry = sucursal.BranchOfficeCountry,
            BranchOfficeState = sucursal.BranchOfficeState,

        };

        if (sucursal is null)
        {
            return NotFound("No se encontro ni la sucursal");
        }
        else
        {
            return Ok(responsesucursal);
        }


    }

}
