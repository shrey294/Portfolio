using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Models;

[Table("User_mst")]
public partial class UserMst
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? UserName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Password { get; set; }

    public string? Token { get; set; }

    public string? RefreshToken { get; set; }

    [Column("expirytime")]
    public DateTime? Expirytime { get; set; }
}
