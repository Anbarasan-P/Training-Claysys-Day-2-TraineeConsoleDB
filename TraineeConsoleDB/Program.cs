using System;
using System.Collections.Generic;

//using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;


namespace TraineeConsoleDB
{
    class Program
    {
        static string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=NewTraineeDB;Integrated Security=True";
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Trainee Console DB Application!");
                Console.WriteLine("1. Add Trainee");
                Console.WriteLine("2. View Trainees");
                Console.WriteLine("3. Update Trainee");
                Console.WriteLine("4. Delete Trainee");
                Console.WriteLine("5. Exit");
                Console.Write("Please select an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTrainee();
                        break;
                    case "2":
                        ViewTrainees();
                        break;
                    case "3":
                        UpdateTrainee();
                        break;
                    case "4":
                        DeleteTrainee();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }
        }
        static void AddTrainee()
        {
            Console.Write("Enter Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Email: ");
            string email = Console.ReadLine();

            Console.Write("Enter Phone Number: ");
            string phone = Console.ReadLine();

            Console.Write("Enter Department: ");
            string department = Console.ReadLine();

            Console.Write("Enter Joining Date (yyyy-mm-dd): ");
            DateTime joiningDate = DateTime.Parse(Console.ReadLine());

            Console.Write("Enter Gender (Male/Female/Others): ");
            string gender = Console.ReadLine();

            Console.Write("Enter Photo File Path: ");
            string photoPath = Console.ReadLine();

            string query = @"INSERT INTO Trainees 
                            (Name, Email, PhoneNumber, Department, JoiningDate, Gender, Photo)
                            VALUES 
                            (@Name, @Email, @Phone, @Department, @JoiningDate, @Gender, @Photo)";

            using (SqlConnection connection = new SqlConnection(connectionString))

            using (SqlCommand command = new SqlCommand(query, connection)) 
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Phone", phone);
                command.Parameters.AddWithValue("@Department", department);
                command.Parameters.AddWithValue("@JoiningDate", joiningDate);
                command.Parameters.AddWithValue("@Gender", gender);

                if (!string.IsNullOrWhiteSpace(photoPath) && File.Exists(photoPath))
                {
                    byte[] photoData = File.ReadAllBytes(photoPath); 
                    command.Parameters.AddWithValue("@Photo", photoData); 
                }
                else
                {
                    command.Parameters.AddWithValue("@Photo", DBNull.Value);
                }

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.WriteLine("Trainee added successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void ViewTrainees()
        {
            Console.WriteLine("Enter Trainee Email to view");
            string email = Console.ReadLine();

            string query = "SELECT TraineeID, Name, Email, PhoneNumber, Department, JoiningDate, Gender FROM Trainees WHERE Email = @Email";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine("Trainee Details");
                    Console.WriteLine($"ID: {reader["TraineeID"]}");
                    Console.WriteLine($"Name: {reader["Name"]}");
                    Console.WriteLine($"Email: {reader["Email"]}");
                    Console.WriteLine($"Phone: {reader["PhoneNumber"]}");
                    Console.WriteLine($"Department: {reader["Department"]}");
                    Console.WriteLine($"Join Date: {reader["JoiningDate"]:yyyy-MM-dd}");
                    Console.WriteLine($"Gender: {reader["Gender"]}");
                }
                else
                {
                    Console.WriteLine("Trainee not found with that Email.");
                }

                //while (reader.Read())
                //{
                //    Console.WriteLine($"ID: {reader["TraineeID"]} | Name: {reader["Name"]} | Email: {reader["Email"]} | Phone: {reader["PhoneNumber"]} | Department: {reader["Department"]} | Join Date: {reader["JoiningDate"]:yyyy-MM-dd} | Gender: {reader["Gender"]}");
                //}

                reader.Close();
            }
        }

        static void UpdateTrainee()
        {
            Console.Write("Enter Trainee ID to update: ");
            int id = int.Parse(Console.ReadLine());

            Console.WriteLine("Select fields to update:");
            Console.WriteLine("1. Name");
            Console.WriteLine("2. Email");
            Console.WriteLine("3. Phone Number");
            Console.WriteLine("4. Department");
            Console.WriteLine("5. Joining Date");
            Console.WriteLine("6. Gender");
            Console.WriteLine("7. Photo");
            Console.Write("Your choices: ");
            string[] choices = Console.ReadLine().Split(',');

            Dictionary<string, object> updates = new Dictionary<string, object>();
            //Dictionary is a key value storage like that key= "Name" and value = "Anbu" 
            //Here we store into updates

            foreach (string choice in choices) //.Select(c => c.Trim())) 
                //using trim to remove any whitespace around the choices
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("Enter new Name: ");
                        updates["Name"] = Console.ReadLine(); //Here we store into updates["Name"] = "Anbu"
                        break;
                    case "2":
                        Console.Write("Enter new Email: ");
                        updates["Email"] = Console.ReadLine();
                        break;
                    case "3":
                        Console.Write("Enter new Phone Number: ");
                        updates["PhoneNumber"] = Console.ReadLine();
                        break;
                    case "4":
                        Console.Write("Enter new Department: ");
                        updates["Department"] = Console.ReadLine();
                        break;
                    case "5":
                        Console.Write("Enter new Joining Date (yyyy-mm-dd): ");
                        updates["JoiningDate"] = DateTime.Parse(Console.ReadLine());
                        break;
                    case "6":
                        Console.Write("Enter new Gender: ");
                        updates["Gender"] = Console.ReadLine();
                        break;
                    case "7":
                        Console.Write("Enter Photo File Path: ");
                        string photoPath = Console.ReadLine();
                        if (File.Exists(photoPath))
                        {
                            updates["Photo"] = File.ReadAllBytes(photoPath); 
                        }
                        else
                        {
                            Console.WriteLine("Photo file not found. Skipping photo update.");
                        }
                        break;
                    default:
                        Console.WriteLine($"Invalid choice: {choice}");
                        break;
                }
            }

            if (updates.Count == 0)
            {
                Console.WriteLine("No valid fields selected for update.");
                return;
            }

            string setClause = string.Join(", ", updates.Keys.Select(field => $"{field} = @{field}")); //Converts each Column name into SQL parameterized "Name" =>"Name" = "@Name"
            //setClause will be like "Name = @Name, Email = @Email, PhoneNumber = @PhoneNumber" etc.
            string query = $"UPDATE Trainees SET {setClause} WHERE TraineeID = @ID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                foreach (var pair in updates) // This part adds placeholder in the query
                {
                    command.Parameters.AddWithValue("@" + pair.Key, pair.Value);
                }
                command.Parameters.AddWithValue("@ID", id);

                connection.Open();
                int rows = command.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? "Trainee updated successfully." : "Trainee not updated.");
            }
        }




        static void DeleteTrainee()
        {
            Console.Write("\nEnter Trainee ID to delete: ");
            int id = int.Parse(Console.ReadLine());

            string query = "DELETE FROM Trainees WHERE TraineeID = @ID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID", id);

                connection.Open();
                int rows = command.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? "Trainee deleted successfully." : "Trainee not found.");
            }
        }
    }
}