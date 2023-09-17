using System;
using System.Collections.Generic;

namespace HafizDemoAPI.ModelCDN;

public partial class Skill
{
    public int SkillId { get; set; }

    public int? UserId { get; set; }

    public string? SkillName { get; set; }

    public int? SkillRating { get; set; }
}
