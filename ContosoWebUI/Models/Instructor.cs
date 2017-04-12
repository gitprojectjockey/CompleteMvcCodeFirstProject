using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace ContosoWebUI.Models
{
    [ModelBinder(typeof(CustomBinders.TrimModelBinder))]
    public class Instructor
    {
        public int ID { get; set; }

        //Multiple Attributes on single line :>
        [Required,Display(Name = "Last Name"), StringLength(50)]
        public string LastName { get; set; }

       
        [Column("FirstName")]
        [Display(Name = "First Name")]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [DataType(DataType.Date), Display(Name = "Hire Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return LastName + ", " + FirstName; }
        }

        public virtual ICollection<Course> Courses { get; set; }
        public virtual OfficeAssignment OfficeAssignment { get; set; }
    }
}
