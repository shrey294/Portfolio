using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Models;

[Table("My_Skills")]
public partial class MySkill
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Title { get; set; }

    [Column("imageurl")]
    public string? Imageurl { get; set; }

    [Column("pill_text")]
    [Unicode(false)]
    public string? PillText { get; set; }
}
