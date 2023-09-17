using System;
using System.Collections.Generic;

namespace HafizDemoAPI.ModelCDN;

public partial class Hobby
{
    public int HobbyId { get; set; }

    public int? UserId { get; set; }

    public string? HobbyName { get; set; }
}
