using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Models;

[Table("enquiry")]
public partial class Enquiry
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Name { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    public string? Subject { get; set; }

    public string? Message { get; set; }

    public DateOnly? Date { get; set; }
}
