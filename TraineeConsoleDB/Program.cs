using System;
//using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;


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

            Console.WriteLine("Select the field to update:");
            Console.WriteLine("1. Name");
            Console.WriteLine("2. Email");
            Console.WriteLine("3. Phone Number");
            Console.WriteLine("4. Department");
            Console.WriteLine("5. Joining Date");
            Console.WriteLine("6. Gender");
            Console.WriteLine("7. Photo");

            Console.Write("Your choice: ");
            string choice = Console.ReadLine();

            string field = "", inputValue = "";
            byte[] photoData = null;
            string query = "";

            switch (choice)
            {
                case "1":
                    field = "Name";
                    Console.Write("Enter new Name: ");
                    inputValue = Console.ReadLine();
                    query = $"UPDATE Trainees SET {field} = @Value WHERE TraineeID = @ID";
                    break;

                case "2":
                    field = "Email";
                    Console.Write("Enter new Email: ");
                    inputValue = Console.ReadLine();
                    query = $"UPDATE Trainees SET {field} = @Value WHERE TraineeID = @ID";
                    break;

                case "3":
                    field = "PhoneNumber";
                    Console.Write("Enter new Phone Number: ");
                    inputValue = Console.ReadLine();
                    query = $"UPDATE Trainees SET {field} = @Value WHERE TraineeID = @ID";
                    break;

                case "4":
                    field = "Department";
                    Console.Write("Enter new Department: ");
                    inputValue = Console.ReadLine();
                    query = $"UPDATE Trainees SET {field} = @Value WHERE TraineeID = @ID";
                    break;

                case "5":
                    field = "JoiningDate";
                    Console.Write("Enter new Joining Date (yyyy-mm-dd): ");
                    DateTime date = DateTime.Parse(Console.ReadLine());
                    query = $"UPDATE Trainees SET {field} = @Value WHERE TraineeID = @ID";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Value", date);
                        command.Parameters.AddWithValue("@ID", id);

                        connection.Open();
                        int rows = command.ExecuteNonQuery();
                        Console.WriteLine(rows > 0 ? "Joining Date updated successfully." : "Trainee not found.");
                    }
                    return;

                case "6":
                    field = "Gender";
                    Console.Write("Enter new Gender: ");
                    inputValue = Console.ReadLine();
                    query = $"UPDATE Trainees SET {field} = @Value WHERE TraineeID = @ID";
                    break;

                case "7":
                    field = "Photo";
                    Console.Write("Enter new Photo File Path: ");
                    string path = Console.ReadLine();

                    if (File.Exists(path))
                    {
                        photoData = File.ReadAllBytes(path);
                        query = $"UPDATE Trainees SET {field} = @Value WHERE TraineeID = @ID";
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Value", photoData);
                            command.Parameters.AddWithValue("@ID", id);

                            connection.Open();
                            int rows = command.ExecuteNonQuery();
                            Console.WriteLine(rows > 0 ? "Photo updated successfully." : "Trainee not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Photo file not found.");
                    }
                    return;

                default:
                    Console.WriteLine("Invalid choice.");
                    return;
            }

            //Console.Write("New Name: ");
            //string name = Console.ReadLine();   

            //Console.Write("New Email: ");
            //string email = Console.ReadLine();

            //Console.Write("New Phone Number: ");
            //string phone = Console.ReadLine();

            //Console.Write("New Department: ");
            //string department = Console.ReadLine();

            //Console.Write("New Joining Date (yyyy-mm-dd): ");
            //DateTime joiningDate = DateTime.Parse(Console.ReadLine());

            //Console.Write("Change Gender: ");
            //string gender = Console.ReadLine();

            //Console.Write("Enter Photo File Path: ");
            //string photoPath = Console.ReadLine();

            //string query = "UPDATE Trainees SET Name = @Name, Email = @Email, PhoneNumber = @Phone, Department = @Department, JoiningDate = @JoiningDate, Gender = @Gender, Photo = @Photo WHERE TraineeID = @ID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Value", inputValue);
                command.Parameters.AddWithValue("@ID", id);

                //command.Parameters.AddWithValue("@Name", name);
                //command.Parameters.AddWithValue("@Email", email);
                //command.Parameters.AddWithValue("@Phone", phone);
                //command.Parameters.AddWithValue("@Department", department);
                //command.Parameters.AddWithValue("@JoiningDate", joiningDate);
                //command.Parameters.AddWithValue("@Gender", gender);

                //if (!string.IsNullOrWhiteSpace(photoPath) && File.Exists(photoPath))
                //{
                //    byte[] photoData = File.ReadAllBytes(photoPath);
                //    command.Parameters.AddWithValue("@Photo", photoData);
                //}
                //else
                //{
                //    command.Parameters.AddWithValue("@Photo", DBNull.Value);
                //}

                //command.Parameters.AddWithValue("@Phone", phone);
                //command.Parameters.AddWithValue("@Department", department);
                //command.Parameters.AddWithValue("@ID", id);

                connection.Open();
                int rows = command.ExecuteNonQuery();
                Console.WriteLine(rows > 0 ? "Trainee updated successfully." : "Trainee not found.");
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