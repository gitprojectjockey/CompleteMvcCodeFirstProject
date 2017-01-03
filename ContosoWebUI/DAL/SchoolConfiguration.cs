//The Entity Framework automatically runs the code it finds in a class that derives from DbConfiguration.You can use the 
//DbConfiguration class to do configuration tasks in code that you would otherwise do in the Web.config file.


namespace ContosoWebUI.DAL
{
    public class SchoolConfiguration : System.Data.Entity.DbConfiguration
    {
        public SchoolConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new System.Data.Entity.SqlServer.SqlAzureExecutionStrategy());
        }
    }
}