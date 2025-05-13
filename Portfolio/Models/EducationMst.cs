using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Models;

[Table("Education_mst")]
public partial class EducationMst
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(100)]
    public string? Qualification { get; set; }

    [Column("college_name")]
    [StringLength(150)]
    public string? CollegeName { get; set; }

    [Column("description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Description { get; set; }
}
