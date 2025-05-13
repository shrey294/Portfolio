using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Models;

[Table("Experience_mst")]
public partial class ExperienceMst
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Title { get; set; }

    [Column("company_name")]
    [StringLength(150)]
    public string? CompanyName { get; set; }

    [Column("description")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Description { get; set; }
}
