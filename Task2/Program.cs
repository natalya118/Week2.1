using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.IO;

namespace Task2
{
    public class Program
    {
        public static void ExecuteAndMessage(SqliteConnection connection, string commandString, string message)
        {
            using (connection)
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(commandString, connection);

                SqliteDataReader reader = command.ExecuteReader();

                // Call Read before accessing data.
                Console.WriteLine("--------------------------------------------------------------------- \r\n");
                Console.WriteLine(message);
                while (reader.Read())
                {
                    ReadRow((IDataRecord)reader);
                }
                Console.WriteLine("");
            }
        }

        public static void Main(string[] args)
        {
            var dirname = Directory.GetCurrentDirectory();
            System.Console.WriteLine(dirname);
            string connectionString =  "Data Source=" + dirname + @"\northwind.db";
            SqliteConnection connection = new SqliteConnection(connectionString);
            ExecuteAndMessage(connection, "SELECT ContactName, CompanyName FROM Customers WHERE ContactName LIKE 'D%'; ", "Customers whose name starts with letter 'D':");

            ExecuteAndMessage(connection, "SELECT UPPER(ContactName), CompanyName FROM Customers; ", "Convert names of all customers to Upper Case:");

            ExecuteAndMessage(connection, "SELECT DISTINCT Country FROM Customers; ", "Select distinct country from Customers:");

            ExecuteAndMessage(connection, "SELECT ContactName FROM Customers WHERE City='London' and ContactTitle LIKE 'Sales%'; ", "Contact name from Customers Table from London and title like “Sales”:");

            ExecuteAndMessage(connection, "SELECT  OrderID FROM 'Order Details' as OrderDetails, Products WHERE Products.ProductName='Tofu' AND Products.ProductId=OrderDetails.ProductID; ", "Orders id where was bought “Tofu”:");

            ExecuteAndMessage(connection, "SELECT ProductName FROM Orders, Products, 'Order Details' as OrderDetails WHERE ShipCountry='Germany' AND Orders.OrderID=OrderDetails.OrderID and OrderDetails.ProductID=Products.ProductID; ", "Select all product names that were shipped to Germany:");
            

            ExecuteAndMessage(connection, @"SELECT CustomerID, ContactName FROM Customers WHERE CustomerId IN (SELECT  CustomerID FROM Orders 
                                            INNER JOIN 'Order Details' as OrderDetails ON Orders.OrderID = OrderDetails.OrderID
                                            WHERE ProductId =(SELECT ProductId FROM Products WHERE ProductName ='Ikura')); ",
                                            "All customers that ordered 'Ikura':");

            
            ExecuteAndMessage(connection, "SELECT FirstName, LastName, OrderID FROM Employees LEFT JOIN Orders ON Employees.EmployeeID = Orders.EmployeeID; ", "All employees, and all orders:");

            ExecuteAndMessage(connection, "SELECT FirstName, LastName, OrderID FROM Employees, Orders WHERE Employees.EmployeeID = Orders.EmployeeID; ", "All employees and any orders they might have:");

            ExecuteAndMessage(connection, "SELECT Phone FROM Shippers UNION SELECT Phone FROM Suppliers; ", "All phones from Shippers and Suppliers:");

            ExecuteAndMessage(connection, "SELECT City , COUNT(City) FROM Customers GROUP BY City; ", "Count all customers grouped by city:");

            ExecuteAndMessage(connection, @" SELECT * FROM Customers 
                                            WHERE(SELECT COUNT(CustomerID) from Orders) > 10
                                            AND(SELECT  AVG(UnitPrice) as PriceAv FROM Orders, 'Order Details' as OrderD
                                            WHERE Customers.CustomerID = Orders.CustomerID and  Orders.OrderID = OrderD.OrderID) < 17; ",
                                            "Select all customers that placed more than 10 orders with average Unit Price less than 17:");

            ExecuteAndMessage(connection, @"SELECT * FROM Customers WHERE Phone GLOB '\d{4}-\d{4}$'; ", "Select all customers with phone that has format ('NNNN-NNNN'):");

            ExecuteAndMessage(connection, "SELECT  CustomerID,max(OrdersCount) FROM (SELECT CustomerID, count(Orders.CustomerID) as OrdersCount FROM Orders GROUP BY CustomerID); ", "Select customer that ordered the greatest amount of goods (not price):");

            ExecuteAndMessage(connection, @"SELECT CustomerId FROM Customers out 
                                            WHERE  (SELECT count(*) FROM (SELECT  ProductId FROM  Orders ,'Order Details' as OrderDetails  
                                            WHERE OrderDetails.OrderID=Orders.OrderID 
                                            AND out.CustomerID=Orders.CustomerID EXCEPT SELECT ProductId FROM Orders,'Order Details' as OrderDetails  
                                            WHERE OrderDetails.OrderID=Orders.OrderID  
                                            AND CustomerID='FAMIA'))=0; ", 
                                            "Select only these customers that ordered the absolutely the same products as customer \"FAMIA\":");




            Console.ReadKey(true);
        }

        private static void ReadRow(IDataRecord record)
        {
            string row = "";
            for (int i = 0; i < record.FieldCount-1; i++)
            {
                row += record[i] + " | ";
            }
            row += record[record.FieldCount-1];
            Console.WriteLine(row+"\r\n");
        }
    
    }
}
