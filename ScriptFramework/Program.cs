using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptFramework
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataSource = ConfigurationManager.AppSettings["DataSource"];
            string initialCatalog = ConfigurationManager.AppSettings["InitialCatalog"];
            string userID = ConfigurationManager.AppSettings["UserID"];
            string password = ConfigurationManager.AppSettings["Password"];

            Console.ReadLine();
        }
    }
}
