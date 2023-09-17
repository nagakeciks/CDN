using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HafizDemoAPI.ModelCDN;

public partial class Simpleuser
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Mail { get; set; }

    public string? Phoneno { get; set; }

    public string? Skills { get; set; }

    public string? Hobbies { get; set; }
}
