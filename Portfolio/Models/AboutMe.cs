using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Models;

[Table("About_me")]
public partial class AboutMe
{
    [Key]
    public int Id { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("experience")]
    public double? Experience { get; set; }

    [Column("skillids")]
    [StringLength(10)]
    [Unicode(false)]
    public string? Skillids { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Experienceids { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? Educationids { get; set; }

    [Column("imageurl")]
    public string? Imageurl { get; set; }
}
