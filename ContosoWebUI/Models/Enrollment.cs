using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ContosoWebUI.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }

    [ModelBinder(typeof(CustomBinders.TrimModelBinder))]
    public class Enrollment
    {
        [Key]
        public int EnrollmentID { get; set; }

        public int CourseID { get; set; }

        public int StudentID { get; set; }

        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }

        public virtual Course Course { get; set; }

        public virtual Student Student { get; set; }

    }
}