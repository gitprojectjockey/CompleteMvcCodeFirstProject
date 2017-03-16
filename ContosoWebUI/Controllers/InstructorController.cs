using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ContosoWebUI.DAL;
using ContosoWebUI.Models;
using ContosoWebUI.ViewModels;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;

namespace ContosoWebUI.Controllers
{
    public class InstructorController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: Instructor
        public ActionResult Index(int? id, int? courseID)
        {
            //Eager loading...
            var viewModel = new InstructorIndexData();
            viewModel.Instructors = db.Instructors
               .Include(i => i.OfficeAssignment)
               .Include(i => i.Courses.Select(c => c.Department))
               .OrderBy(i => i.LastName);
            if (id != null)
            {
                ViewBag.InstructorID = id.Value;
                viewModel.Courses = viewModel.Instructors.Where(
                i => i.ID == id.Value).Single().Courses;
            }
            if (courseID != null)
            {
                ViewBag.CourseID = courseID.Value;

                //Lazy loading...
                //viewModel.Enrollments = viewModel.Courses.Where(
                //x => x.CourseID == courseID).Single().Enrollments;

                //Explicit Loading...
                var selectedCourse = viewModel.Courses
                    .Where(x => x.CourseID == courseID)
                    .Single();
                db.Entry(selectedCourse).Collection(x => x.Enrollments).Load();

                foreach (Enrollment enrollment in selectedCourse.Enrollments)
                {
                    db.Entry(enrollment).Reference(x => x.Student).Load();
                }
                viewModel.Enrollments = selectedCourse.Enrollments;
            }
            return View(viewModel);
        }

        // GET: Instructor/Details/5
        public ActionResult Details(int? id)
        {
            //Eager loading office assignment
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Instructor instructor = db.Instructors.Include(i => i.OfficeAssignment)
                .Single(i => i.ID == id) as Instructor;
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // GET: Instructor/Create
        public ActionResult Create()
        {
            var instructor = new Instructor();
            instructor.Courses = new List<Course>();
            PopulateAssignedCourseData(instructor);
            return View();
        }

        // POST: Instructor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,HireDate,OfficeAssignment")] Instructor instructor, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                instructor.Courses = new List<Course>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = db.Courses.Find(int.Parse(course));
                    instructor.Courses.Add(courseToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                db.Instructors.Add(instructor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            //eager load office assignment and courses
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Where(i => i.ID == id).Single();

            PopulateAssignedCourseData(instructor);

            if (instructor == null)
            {
                return HttpNotFound();
            }

            return View(instructor);
        }

        // POST: Instructor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost([Bind(Include = "ID,LastName,FirstMidName,HireDate")] int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var instructorToUpdate = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Where(i => i.ID == id)
                .Single();

            if (TryUpdateModel(instructorToUpdate, new string[] { "LastName", "FirstMidName", "HireDate", "OfficeAssignment" }))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                    {
                        instructorToUpdate.OfficeAssignment = null;
                    }
                    UpdateIstructorAssignedCourses(selectedCourses, instructorToUpdate);
                    db.Entry(instructorToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }

            }
            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }


        // GET: Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Instructor instructor = db.Instructors.Find(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }
            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Instructor instructor = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Where(i => i.ID == id)
                .Single();

            instructor.OfficeAssignment = null;
            db.Instructors.Remove(instructor);

            var department = db.Departments
                .Where(d => d.InstructorID == id)
                .SingleOrDefault();

            if (department != null)
            {
                department.InstructorID = null;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allCourses = db.Courses;
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }
            ViewBag.Courses = viewModel;
        }

        //The code then loops through all courses in the database and checks each course against the ones currently assigned to 
        //the instructor versus the ones that were selected in the view.To facilitate efficient lookups, the latter two collections 
        //are stored in HashSet objects.

        //If the check box for a course was selected but the course isn't in the Instructor.Courses navigation property, the course 
        //is added to the collection in the navigation property.

        //If the check box for a course wasn't selected, but the course is in the Instructor.Courses navigation property, 
        //the course is removed from the navigation property.
        private void UpdateIstructorAssignedCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                instructorToUpdate.Courses = new List<Course>();
                return;
            }
            var selectedCoursesHash = new HashSet<string>(selectedCourses);
            var instructorCoursesHash = new HashSet<int>(instructorToUpdate.Courses.Select(c => c.CourseID));
            foreach (var course in db.Courses)
            {
                if (selectedCoursesHash.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCoursesHash.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Add(course);
                    }
                }
                else
                {
                    if (instructorCoursesHash.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Remove(course);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
