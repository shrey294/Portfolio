using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Models;

[Table("Header_information")]
public partial class HeaderInformation
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("initials")]
    [StringLength(2)]
    [Unicode(false)]
    public string? Initials { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Designation { get; set; }

    [Column("short_description")]
    [StringLength(500)]
    public string? ShortDescription { get; set; }

    [Column("icons")]
    public string? Icons { get; set; }

    [Column("is_delete")]
    [StringLength(1)]
    [Unicode(false)]
    public string? IsDelete { get; set; }
}
