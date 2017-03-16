using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ContosoWebUI.DAL;
using ContosoWebUI.Models;
using PagedList;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;


namespace ContosoWebUI.Controllers
{
    public class StudentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        //This code receives a sortOrder parameter from the query string in the URL.The query string value is provided by ASP.NET MVC 
        //as a parameter to the action method. The parameter will be a string that's either "Name" or "Date", optionally followed by an underscore and the string "desc" to specify descending order. The default sort order is ascending.
        //The first time the Index page is requested, there's no query string. The students are displayed in ascending order by LastName, 
        //which is the default as established by the fall-through case in the switch statement. When the user clicks a column heading hyperlink, the appropriate sortOrder value is provided in the query string.


        //The search string value is received from a text box that you'll add to the Index view. You've also added to the LINQ statement a where clause that selects only students whose first name or last name contains the search string.
        //The statement that adds the where clause is executed only if there's a value to search for.

        //PagedList.Mvc package is used to enable paging of student data.

        // GET: Student
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            //This value must be included in the paging links in order to maintain the filter settings during paging, and it must be restored to the text box when the page is redisplayed.If the search 
            //string is changed during paging, the page has to be reset to 1, because the new filter can result in different data to display.
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            var students = from s in db.Students select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstMidName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(students.ToPagedList(pageNumber, pageSize));
        }

        // GET: Student/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Student student = db.Students
                    .Include(s => s.Enrollments)
                    .Include(s => s.Enrollments.Select(e => e.Course))
                    .Single(s => s.ID == id) as Student;

            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Student/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //DataException Represents the exception that is thrown when errors are generated using ADO.NET components.
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(student);
        }

        //This loads the view controls with current values so user can edit them.
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //As a best practice to prevent overposting, the fields that you want to be updateable by the Edit page are 
        //whitelisted in the TryUpdateModel parameters.Currently there are no extra fields that you're protecting, 
        //but listing the fields that you want the model binder to bind ensures that if you add fields to the data 
        //model in the future, they're automatically protected until you explicitly add them here.

        [HttpPost ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentToUpdate = db.Students.Find(id);
            if (TryUpdateModel(studentToUpdate, "", new string[] { "LastName", "FirstMidName", "EnrollmentDate" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }

        // GET: Student/Delete/5

        //bool? saveChangesError = false
        //This code accepts an optional parameter that indicates whether the method was called after a failure to save changes.
        //This parameter is false when the HttpGet Delete method is called without a previous failure.When it is called by the 
        //HttpPost Delete method in response to a database update error, the parameter is true and an error message is passed to the view.
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Student/Delete/5

        //This code retrieves the selected entity, then calls the Remove method to set the entity's status to Deleted. When SaveChanges is called, a SQL DELETE command is generated. You have also changed the action method name from DeleteConfirmed to Delete. The scaffolded code named the HttpPost Delete method DeleteConfirmed to give the HttpPost  
        //method a unique signature. ( The CLR requires overloaded methods to have different method parameters.) Now that the signatures are unique, you can stick with the MVC convention and use the same name for the HttpPost and HttpGet delete methods.
        //If improving performance in a high-volume application is a priority, you could avoid an unnecessary SQL query to retrieve the row by replacing the lines of code that call the Find and Remove methods with the following code:
        //Student studentToDelete = new Student() { ID = id };
        //db.Entry(studentToDelete).State = EntityState.Deleted;
        //This code instantiates a Student entity using only the primary key value and then sets the entity state to Deleted.
        //That's all that the Entity Framework needs in order to delete the entity.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                //Redirect to the Get Delete in order to show the save changes error
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
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
