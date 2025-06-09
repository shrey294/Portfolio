using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Models;

public partial class Project
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? Link { get; set; }

    [Column("imageurl")]
    [Unicode(false)]
    public string? Imageurl { get; set; }

    [StringLength(150)]
    [Unicode(false)]
    public string? Title { get; set; }

    [Column("Short_description")]
    [Unicode(false)]
    public string? ShortDescription { get; set; }

    [Column("Stack_pill")]
    [StringLength(500)]
    [Unicode(false)]
    public string? StackPill { get; set; }
}
