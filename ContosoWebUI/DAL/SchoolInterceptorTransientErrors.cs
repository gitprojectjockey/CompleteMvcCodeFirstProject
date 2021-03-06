﻿using ContosoWebUI.Logging;
using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace ContosoWebUI.DAL
{

    //This code only overrides the ReaderExecuting method, which is called for 
    //queries that can return multiple rows of data. If you wanted to check 
    //connection resiliency for other types of queries, you could also override the 
    //NonQueryExecuting and ScalarExecuting methods, as the logging interceptor does.

    //When you run the Student page and enter "Throw" as the search string, this code 
    //creates a dummy SQL Database exception for error number 20, a type known to be 
    //typically transient. Other error numbers currently recognized as transient are 
    //64, 233, 10053, 10054, 10060, 10928, 10929, 40197, 40501, abd 40613, but these 
    //are subject to change in new versions of SQL Database.

    //The code returns the exception to Entity Framework instead of running the query 
    //and passing back query results. The transient exception is returned four times, 
    //and then the code reverts to the normal procedure of passing the query to the 
    //database.

    //The number of times the Entity Framework will retry is configurable; the code specifies four times because 
    //that's the default value for the SQL Database execution policy. If you change the execution policy, 
    //you'd also change the code here that specifies how many times transient errors are generated.You could also 
    //change the code to generate more exceptions so that Entity Framework will throw the 
    //RetryLimitExceededException exception.

    //NOTE: In order for this interception code to run entries were made in global asax file
    public class SchoolInterceptorTransientErrors : DbCommandInterceptor
    {
        private int _counter = 0;
        private ILogger _logger = new Logger();

        public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            bool throwTransientErrors = false;
            if (command.Parameters.Count > 0 && command.Parameters[0].Value.ToString() == "%Throw%")
            {
                throwTransientErrors = true;
                command.Parameters[0].Value = "%an%";
                command.Parameters[1].Value = "%an%";
            }

            if (throwTransientErrors && _counter < 4)
            {
                _logger.Information("Returning transient error for command: {0}", command.CommandText);
                _counter++;
                interceptionContext.Exception = CreateDummySqlException();
            }
        }

        private SqlException CreateDummySqlException()
        {
            // The instance of SQL Server you attempted to connect to does not support encryption
            var sqlErrorNumber = 20;

            var sqlErrorCtor = typeof(SqlError).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 7).Single();
            var sqlError = sqlErrorCtor.Invoke(new object[] { sqlErrorNumber, (byte)0, (byte)0, "", "", "", 1 });

            var errorCollection = Activator.CreateInstance(typeof(SqlErrorCollection), true);
            var addMethod = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic);
            addMethod.Invoke(errorCollection, new[] { sqlError });

            var sqlExceptionCtor = typeof(SqlException).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Where(c => c.GetParameters().Count() == 4).Single();
            var sqlException = (SqlException)sqlExceptionCtor.Invoke(new object[] { "Dummy", errorCollection, null, Guid.NewGuid() });

            return sqlException;
        }
    }
}
