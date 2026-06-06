using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement_Project.Models
{
    public class Plan
    {
        public int PlanID { get; set; }
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public int DurationMonths { get; set; }
        public string Description { get; set; }
    }

    public class PlanRepository
    {
        public List<Plan> GetAll()
        {
            List<Plan> plans = new List<Plan>();
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = "SELECT PlanID, PlanName, Price, DurationMonths, Description FROM [Plan]";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                plans.Add(new Plan
                                {
                                    PlanID = (int)reader["PlanID"],
                                    PlanName = reader["PlanName"].ToString(),
                                    Price = (decimal)reader["Price"],
                                    DurationMonths = (int)reader["DurationMonths"],
                                    Description = reader["Description"] == DBNull.Value ? "" : reader["Description"].ToString()
                                });
                            }
                        }
                    }
                }
                return plans;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(Plan plan)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = @"INSERT INTO [Plan] (PlanName, Price, DurationMonths, Description) 
                                     VALUES (@planName, @price, @durationMonths, @description)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@planName", plan.PlanName);
                        cmd.Parameters.AddWithValue("@price", plan.Price);
                        cmd.Parameters.AddWithValue("@durationMonths", plan.DurationMonths);
                        cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(plan.Description) ? (object)DBNull.Value : plan.Description);
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

        public void Update(Plan plan)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = @"UPDATE [Plan] 
                                     SET PlanName       = @planName, 
                                         Price          = @price, 
                                         DurationMonths = @durationMonths, 
                                         Description    = @description 
                                     WHERE PlanID = @planID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@planID", plan.PlanID);
                        cmd.Parameters.AddWithValue("@planName", plan.PlanName);
                        cmd.Parameters.AddWithValue("@price", plan.Price);
                        cmd.Parameters.AddWithValue("@durationMonths", plan.DurationMonths);
                        cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(plan.Description) ? (object)DBNull.Value : plan.Description);
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

        public void Delete(int planID)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = "DELETE FROM [Plan] WHERE PlanID = @planID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@planID", planID);
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
    }
}
