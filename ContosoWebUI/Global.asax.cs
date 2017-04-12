using ContosoWebUI.DAL;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ContosoWebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
           // ModelBinders.Binders.Add(typeof(CustomBinders.TrimModelBinder), new CustomBinders.TrimModelBinder());
            ModelBinders.Binders.Add(typeof(CustomBinders.TrimModelBinder), new CustomBinders.TrimModelBinder());

            //YYZ
            //--------------------------------------------------------------------------------------------------
            //These lines of code are what causes your interceptor code to be run when Entity Framework sends queries 
            //to the database.Notice that because you created separate interceptor classes for transient error 
            //simulation and logging, you can independently enable and disable them.

            //You can add interceptors using the DbInterception.Add method anywhere in your code; it doesn't 
            //have to be in the Application_Start method. Another option is to put this code in the DbConfiguration 
            //class that you created earlier to configure the execution policy.

            //Interceptors are executed in the order of registration (the order in which the DbInterception.Add method is called). 
            //The order might matter depending on what you're doing in the interceptor. For example, an interceptor 
            //might change the SQL command that it gets in the CommandText property. If it does change the SQL command, 
            //the next interceptor will get the changed SQL command, not the original SQL command.

            System.Data.Entity.Infrastructure.Interception.DbInterception.Add(new SchoolInterceptorTransientErrors());
            System.Data.Entity.Infrastructure.Interception.DbInterception.Add(new SchoolInterceptorLogging());
        }
    }
}
