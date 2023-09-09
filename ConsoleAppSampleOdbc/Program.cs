using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace ConsoleAppSampleOdbc
{
    internal class Program
    {
        static void Main(string[] args)
        {
            OdbcConnection DbConnection = new OdbcConnection("DSN=dbf_reniec");
            DbConnection.Open();
            // Your code here
            Console.WriteLine("conected");
            Console.ReadLine();
            DbConnection.Close();
        }
    }
}
