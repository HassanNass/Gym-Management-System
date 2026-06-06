using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement_Project.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserRepository
    {
        public bool Login(User user)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = @"SELECT UserID, Username, Role FROM [User] WHERE Username=@username AND Password=@password";
                    using(SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", user.Username);
                        cmd.Parameters.AddWithValue("@password", user.Password);
                        conn.Open();
                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user.Role = (string)reader["Role"];
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Login Error" + ex.Message);
            }
        }

        public List<User> GetAll()
        {
            List<User> user = new List<User>();
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = @"SELECT UserID, Username, Password, Role, CreatedAt FROM [User]";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            user.Add(new User
                            {
                                UserID = (int)reader["UserID"],
                                Username = reader["Username"].ToString(),
                                Password = reader["Password"].ToString(),
                                Role = reader["Role"].ToString(),
                                CreatedAt = (DateTime)reader["CreatedAt"]
                            });
                        }
                    }
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(User user)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = @"INSERT INTO [User] (Username, Password, Role) VALUES (@username, @password, @role)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", user.Username);
                        cmd.Parameters.AddWithValue("@password", user.Password);
                        cmd.Parameters.AddWithValue("@role", user.Role);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(User user)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = @"UPDATE [User] SET Username=@username, Password=@password, Role=@role WHERE UserID = @userID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userID", user.UserID);
                        cmd.Parameters.AddWithValue("@username", user.Username);
                        cmd.Parameters.AddWithValue("@password", user.Password);
                        cmd.Parameters.AddWithValue("@role", user.Role);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Delete(int userID)
        {
            using (SqlConnection conn = DBHelper.GetConnection())
            {
                string query = @"DELETE FROM [User] WHERE UserID=@userID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userID", userID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
