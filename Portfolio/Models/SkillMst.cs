using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Models;

[Table("Skill_mst")]
public partial class SkillMst
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("Skill_name")]
    [StringLength(100)]
    public string? SkillName { get; set; }

    [Column("Skill_percantage")]
    public int? SkillPercantage { get; set; }
}
