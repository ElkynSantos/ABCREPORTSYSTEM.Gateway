using System;
using System.Collections.Generic;

namespace ABCREPORTSYSTEM.Sucursal.Models;

public partial class BranchOffice
{
    public int BranchOfficeId { get; set; }

    public string? BranchOfficeCountry { get; set; }

    public string? BranchOfficeState { get; set; }
}
