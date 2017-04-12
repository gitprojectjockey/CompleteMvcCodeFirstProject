using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ContosoWebUI.Models
{
    [ModelBinder(typeof(CustomBinders.TrimModelBinder))]
    public class Department
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Department Name")]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Creation Date")]

        public DateTime StartDate { get; set; }

        public int? InstructorID { get; set; }

        public virtual Instructor Administrator { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}