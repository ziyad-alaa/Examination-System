﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Models;
public partial class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string name { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string st_city { get; set; }

    public int? dept_id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string password { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string email { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string phone { get; set; }

    public int? branch_id { get; set; }

    [InverseProperty("ins")]
    public virtual Instructor Instructor { get; set; }

    [InverseProperty("std")]
    public virtual Student Student { get; set; }

    [ForeignKey("branch_id")]
    [InverseProperty("Users")]
    public virtual Branch branch { get; set; }

    [ForeignKey("dept_id")]
    [InverseProperty("Users")]
    public virtual Department dept { get; set; }

    [ForeignKey("UserID")]
    [InverseProperty("Users")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    public void SetPassword(string plainPassword)
    {
        password = BCrypt.Net.BCrypt.HashPassword(plainPassword);
    }

    public bool VerifyPassword(string plainPassword)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, password);
    }


    public Boolean isActive { get; set; }

}