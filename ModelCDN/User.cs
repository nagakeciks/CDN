using System;
using System.Collections.Generic;

namespace HafizDemoAPI.ModelCDN;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? Mail { get; set; }

    public string? PhoneNo { get; set; }

    public string? Password { get; set; }

    public string? Salt { get; set; }
    public DateTime? DateCreated { get; set; }
}
