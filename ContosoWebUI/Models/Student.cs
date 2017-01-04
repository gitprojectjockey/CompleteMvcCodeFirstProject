using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoWebUI.Models
{
    public class Student
    {
        [Key]
        public int ID { get; set; }

        //Regular expression requires the first character to be upper case and the remaining characters to be alphabetical
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$", ErrorMessage = "Last name must be alphabetical and first letter must be uppercase")]
        [Required]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        //Regular expression requires the first character to be upper case and the remaining characters to be alphabetical
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$", ErrorMessage = "First name must be alphabetical and first letter must be uppercase")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Display(Name = "First Name")]
        [Column("FirstName")]
        public string FirstMidName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
        }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}