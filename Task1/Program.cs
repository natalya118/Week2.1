using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Data;

namespace Task1
{

    public class Company
    {
        public string Title, Country, AddedDate;
        public Company(string title, string country, string date)
        {
            this.Title = title;
            this.Country = country;
            this.AddedDate = date;

        }



    }
    public class Program
    {
        public static Company ReadFromJSON(string json)
        {

            return JsonConvert.DeserializeObject<Company>(json);
        }




        //creates new table if it doesn't exist and adds 10 new records
        public static void CreateAndFillCompanies(string constring)
        {

            using (SqliteConnection connection =
                       new SqliteConnection(constring))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("CREATE TABLE IF NOT EXISTS Companies (id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, Title VARCHAR(255), Country VARCHAR(50), AddedDate DATE);", connection);
                command.ExecuteNonQuery();
                command = new SqliteCommand("INSERT INTO Companies (Title, Country, AddedDate) VALUES('NewtonIdeas', 'Ukraine', '2002-06-10')", connection);
                command.ExecuteNonQuery();

                command = new SqliteCommand("INSERT INTO Companies (Title, Country, AddedDate) VALUES('Terrasoft', 'Ukraine', '2010-05-17')", connection);
                command.ExecuteNonQuery();

                command = new SqliteCommand("INSERT INTO Companies (Title, Country, AddedDate) VALUES('Global Logic', 'Ukraine', '2013-02-21')", connection);
                command.ExecuteNonQuery();

                command = new SqliteCommand("INSERT INTO Companies (Title, Country, AddedDate) VALUES('Luxoft', 'Ukraine', '2000-11-27')", connection);
                command.ExecuteNonQuery();

                command = new SqliteCommand("INSERT INTO Companies (Title, Country, AddedDate) VALUES('Microsoft', 'USA', '2010-05-01')", connection);
                command.ExecuteNonQuery();

                command = new SqliteCommand("INSERT INTO Companies (Title, Country, AddedDate) VALUES('Meizu', 'China', '2010-05-23')", connection);
                command.ExecuteNonQuery();

                command = new SqliteCommand("INSERT INTO Companies (Title, Country, AddedDate) VALUES('Gameloft', 'France', '2013-04-12')", connection);
                command.ExecuteNonQuery();

                command = new SqliteCommand("INSERT INTO Companies (Title, Country, AddedDate) VALUES('Ubisoft', 'Canada', '2005-02-19')", connection);
                command.ExecuteNonQuery();

                command = new SqliteCommand("INSERT INTO Companies (Title, Country, AddedDate) VALUES('Sparx Systems', 'Australia', '2007-12-11')", connection);
                command.ExecuteNonQuery();

                command = new SqliteCommand("INSERT INTO Companies (Title, Country, AddedDate) VALUES('SamLogic', 'Sweden', '2002-10-01')", connection);
                command.ExecuteNonQuery();

                Console.WriteLine("table filled");


            }
        }

        //info about the company with max id
        private static void ReadWithMaxID(string connectionString)
        {
            string queryString =
                "SELECT id, Title FROM Companies WHERE id = (SELECT max(id) FROM Companies); ";

            using (SqliteConnection connection =
                       new SqliteConnection(connectionString))
            {
                SqliteCommand command =
                    new SqliteCommand(queryString, connection);
                connection.Open();

                SqliteDataReader reader = command.ExecuteReader();

                // Call Read before accessing data.
                Console.WriteLine("Company with max ID is:");
                while (reader.Read())
                {
                    ReadSingleRow((IDataRecord)reader);
                }

            }
        }

        private static void ReadSingleRow(IDataRecord record)
        {
            Console.WriteLine(String.Format("{0}, {1}", record[0], record[1]));
        }


        //Changes companies' country value
        private static void UpdateCompaniesCountries(String countryNew,
    string countryOld, string connectionString)
        {
            string commandText = "UPDATE Companies SET Country = @countrynew "
                + "WHERE Country = @countryold;";

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                SqliteCommand command = new SqliteCommand(commandText, connection);
                command.Parameters.AddWithValue("@countrynew", countryNew);
                command.Parameters.AddWithValue("@countryold", countryOld);

                try
                {
                    connection.Open();

                    command.ExecuteNonQuery();
                    Console.WriteLine("Set Country = " + countryNew + " for companies with Country = " + countryOld);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }


        public static void DeleteCompaniesByCountry(string Country, string constring)
        {
            string commandText = "DELETE FROM Companies WHERE Country = @country ;";

            using (SqliteConnection connection = new SqliteConnection(constring))
            {

                SqliteCommand command = new SqliteCommand(commandText, connection);
                command.Parameters.AddWithValue("@country", Country);

                try
                {
                    connection.Open();

                    command.ExecuteNonQuery();
                    Console.WriteLine("Deleted companies from " + Country);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }


        }



        private static void GetNumberOfRecords(string connectionString)
        {
            string queryString =
                "SELECT COUNT(*) FROM Companies; ";

            using (SqliteConnection connection =
                       new SqliteConnection(connectionString))
            {
                SqliteCommand command =
                    new SqliteCommand(queryString, connection);
                connection.Open();

                SqliteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("Total number of records:" + reader[0]);
                }

            }
        }







        private static void ReadAllCompanies(string connectionString)
        {
            string queryString =
                "SELECT * FROM Companies; ";

            using (SqliteConnection connection =
                       new SqliteConnection(connectionString))
            {
                SqliteCommand command =
                    new SqliteCommand(queryString, connection);
                connection.Open();

                SqliteDataReader reader = command.ExecuteReader();

                // Call Read before accessing data.
                Console.WriteLine("All the companies:");
                while (reader.Read())
                {
                    ReadFullRow((IDataRecord)reader);
                }

            }
        }

        private static void ReadFullRow(IDataRecord record)
        {
            Console.WriteLine(String.Format("{0}, {1}, {2}, {3}", record[0], record[1], record[2], record[3]));
        }




        private static void ExecuteSqlTransaction(string connectionString)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                SqliteCommand command = connection.CreateCommand();
                SqliteTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction();

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;

                string help = "To add a record to the database please \r\na)input  info about new company in format: 'CompanyTitle,Country,AddedDate' (without ' , for example: Ololo,Ololand,2012-12-12) \r\nb)or input JSON({'Title': 'Ololo','Country': 'Ololand','AddedDate': '2001-10-12'} \r\nTo commit and exit input q";

                Console.WriteLine(help);

                string s;
                string[] values;
                Company newCompany;

                while (true)
                {
                    Console.Write(">");
                    s = Console.ReadLine();
                    if (s.Equals("q"))
                    {
                        break;
                    }
                    if (s.StartsWith("{"))
                    {
                        try
                        {
                            newCompany = ReadFromJSON(s);
                            command.CommandText = String.Format("INSERT INTO Companies (Title, Country, AddedDate) VALUES('{0}', '{1}', '{2}');", newCompany.Title, newCompany.Country, newCompany.AddedDate);
                            Console.WriteLine(String.Format("Title: {0}, Country: {1}, AddedDate: {2}", newCompany.Title, newCompany.Country, newCompany.AddedDate));
                            command.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Incorrect input!");
                        }
                    }
                    else
                    {
                        try
                        {
                            values = s.Split(',');
                            newCompany = new Company(values[0], values[1], values[2]);
                            command.CommandText = String.Format("INSERT INTO Companies (Title, Country, AddedDate) VALUES('{0}', '{1}', '{2}');", newCompany.Title, newCompany.Country, newCompany.AddedDate);
                            Console.WriteLine(String.Format("Title: {0}, Country: {1}, AddedDate: {2}", newCompany.Title, newCompany.Country, newCompany.AddedDate));
                            command.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Incorrect input!");
                        }

                    }


                }
                try
                {

                    // Attempt to commit the transaction.
                    transaction.Commit();
                    Console.WriteLine("All records are written to database.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);

                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }
            }
        }


        public static void Main(string[] args)
        {

            string connectionString = @"Data Source =test.db;";
            //Tasks 1-3
            CreateAndFillCompanies(connectionString);
            //to check if the previous void is correct
            ReadAllCompanies(connectionString);
            //Task 4
            ReadWithMaxID(connectionString);
            //Task 5
            UpdateCompaniesCountries("USA", "Ukraine", connectionString);
            //Task 6
            DeleteCompaniesByCountry("USA", connectionString);

            GetNumberOfRecords(connectionString);
            //Task 7
            ReadAllCompanies(connectionString);
            //Task 8
            ExecuteSqlTransaction(connectionString);
            Console.ReadKey(true);

        }




    }
}
