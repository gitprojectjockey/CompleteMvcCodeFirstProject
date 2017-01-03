using ContosoWebUI.DAL;
using ContosoWebUI.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace ContosoWebUI.Controllers
{
    public class HomeController : Controller
    {
        private SchoolContext db = new SchoolContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            //IQueryable<EnrollmentDateGroup> data = from student in db.Students
            //                                       group student by student.EnrollmentDate into dateGroup
            //                                       select new EnrollmentDateGroup()
            //                                       {
            //                                           EnrollmentDate = dateGroup.Key,
            //                                           StudentCount = dateGroup.Count()
            //                                       };

            //I used lamda expression instead
            IQueryable<EnrollmentDateGroup> data = db.Students
                .GroupBy(s => s.EnrollmentDate)
                .Select(s => new EnrollmentDateGroup()
                {
                    EnrollmentDate = s.Key,
                    StudentCount = s.Count()
                });

            return View(data.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //Don't forget to dispose
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}