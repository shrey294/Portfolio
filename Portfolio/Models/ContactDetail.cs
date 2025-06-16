using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Models;

[Table("Contact_Details")]
public partial class ContactDetail
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Location { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [Column("Github_url")]
    public string? GithubUrl { get; set; }

    [Column("Intsa_url")]
    public string? IntsaUrl { get; set; }

    [Column("LinkedIn_url")]
    public string? LinkedInUrl { get; set; }
}
