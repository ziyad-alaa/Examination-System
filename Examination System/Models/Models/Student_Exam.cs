﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Models;

[PrimaryKey("stdid", "ExamId")]
[Table("Student_Exam")]
public partial class Student_Exam
{
    [Key]
    public int stdid { get; set; }

    [Key]
    public int ExamId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Grade { get; set; }

    [ForeignKey("ExamId")]
    [InverseProperty("Student_Exams")]
    public virtual Exam Exam { get; set; }

    [ForeignKey("stdid")]
    [InverseProperty("Student_Exams")]
    public virtual Student std { get; set; }

    public Boolean isActive { get; set; }

}