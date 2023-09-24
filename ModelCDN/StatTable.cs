using System;
using System.Collections.Generic;

namespace HafizDemoAPI.ModelCDN;

public partial class StatTable
{
    public int Id { get; set; }

    public string? StatGroup { get; set; }

    public string? StatName { get; set; }

    public int? StatValue { get; set; }
}
