using System;
using System.Collections.Generic;

namespace ABCREPORTSYSTEM.Sucursal.Models;

public partial class Employee
{
    public Guid EmployeeId { get; set; }

    public string? Username { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int BranchOfficeId { get; set; }
}
