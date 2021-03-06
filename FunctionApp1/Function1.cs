using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace FunctionApp1
{
    
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            //string connstr = System.Environment.GetEnvironmentVariable("mysql_connection");
            //log.Info(connstr); //localhost:7071/api/Function1?departmentname=������ // localhost:7071/api/Function1?departmentname=DMS
            log.Info("C# HTTP trigger function processed a request.");

            MySqlConnection mysqlConnection = new MySqlConnection(GetEnvironmentVariable("mysql_connection"));
            List<StudentData> studentdataList = new List<StudentData>();

            // parse query parameter            
            string departmentname = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "departmentname", true) == 0)
                .Value;

            if (departmentname == null)
            {
                // Get request body
                dynamic data = await req.Content.ReadAsAsync<object>();
                departmentname = data?.departmentname;
            }

            //try connecing mysql
            try
            {
                mysqlConnection.Open();

                string connectionState = mysqlConnection.State.ToString();
                log.Info("connection {" + connectionState + "}.");
                //excute query to call data
                QueryEmployee(mysqlConnection,log, studentdataList, departmentname);
                mysqlConnection.Close();

            }
            catch (Exception ex)
            {
                log.Warning("connecting to mysql failed : " + ex);
            }

            return departmentname == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a departmentname on the query string or in the request body")
             : req.CreateResponse(HttpStatusCode.OK, studentdataList, "application/json");

        }
        public static string GetEnvironmentVariable(string variablename)
        {
               return System.Environment.GetEnvironmentVariable(variablename);
        }
        private static void QueryEmployee(MySqlConnection conn, TraceWriter log, List<StudentData> studentdataList, string departmentname)
        {
            string sql = "SELECT DEPARTMENT, NAME, EMAIL, STUDENT_GROUP, MANAGER_YN, ENABLED_USER_YN FROM bpdb.ba_student where DEPARTMENT like '"+ departmentname +"';";
            // create command
            MySqlCommand cmd = new MySqlCommand();

            try
            {
               
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Prepare();

            }
            catch (Exception ex)
            {
                log.Info("connecting to mysql failed : " + ex);
            }

            using (DbDataReader reader = cmd.ExecuteReader())
            {             
                
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        studentdataList.Add(new StudentData(
                            reader.GetString(0),
                            reader.GetString(1),
                           reader.GetString(2),
                           reader.GetString(3),
                           reader.GetString(4),
                           reader.GetString(5)
                           ));
                    }
                    reader.Close();
                }
            }
        }
    }
}
