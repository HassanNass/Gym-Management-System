using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement_Project.Models
{
    public class Member
    {
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public DateTime JoinDate { get; set; }
        public int PlanID { get; set; }
        public string PlanName { get; set; }
        public bool IsActive { get; set; }
        public string ImageName { get; set; }
    }

    public class MemberRepository
    {
        public List<Member> GetAll()
        {
            List<Member> members = new List<Member>();
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = @"SELECT m.MemberID, m.FirstName, m.LastName, m.Phone,
                                     m.JoinDate, m.PlanID, m.IsActive, m.ImageName, p.PlanName
                                     FROM [Member] m LEFT JOIN [Plan] p ON m.PlanID = p.PlanID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                members.Add(new Member
                                {
                                    MemberID = (int)reader["MemberID"],
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    Phone = reader["Phone"].ToString(),
                                    JoinDate = (DateTime)reader["JoinDate"],
                                    PlanID = reader["PlanID"] == DBNull.Value ? 0 : (int)reader["PlanID"],
                                    PlanName = reader["PlanName"] == DBNull.Value ? "No Plan" : reader["PlanName"].ToString(),
                                    IsActive = (bool)reader["IsActive"],
                                    ImageName = reader["ImageName"] == DBNull.Value ? "" : reader["ImageName"].ToString()
                                });
                            }
                        }
                    }
                }
                return members;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(Member member)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = @"INSERT INTO [Member] (FirstName, LastName, Phone, JoinDate, PlanID, IsActive, ImageName)
                                     VALUES (@firstName, @lastName, @phone, @joinDate, @planID, @isActive, @imageName)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@firstName", member.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", member.LastName);
                        cmd.Parameters.AddWithValue("@phone", member.Phone);
                        cmd.Parameters.AddWithValue("@joinDate", member.JoinDate);
                        cmd.Parameters.AddWithValue("@planID", member.PlanID == 0 ? (object)DBNull.Value : member.PlanID);
                        cmd.Parameters.AddWithValue("@isActive", member.IsActive);
                        cmd.Parameters.AddWithValue("@imageName", member.ImageName);
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

        public void Update(Member member)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = @"UPDATE [Member] 
                                     SET FirstName = @firstName, 
                                         LastName = @lastName, 
                                         Phone = @phone, 
                                         PlanID = @planID, 
                                         IsActive = @isActive,
                                         ImageName = @imageName
                                     WHERE MemberID = @memberID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@memberID", member.MemberID);
                        cmd.Parameters.AddWithValue("@firstName", member.FirstName);
                        cmd.Parameters.AddWithValue("@lastName", member.LastName);
                        cmd.Parameters.AddWithValue("@phone", member.Phone);
                        cmd.Parameters.AddWithValue("@planID", member.PlanID == 0 ? (object)DBNull.Value : member.PlanID);
                        cmd.Parameters.AddWithValue("@isActive", member.IsActive);
                        cmd.Parameters.AddWithValue("@imageName", member.ImageName);
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

        public void Delete(int memberID)
        {
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = @"DELETE FROM [Member] WHERE MemberID=@memberID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@memberID", memberID);
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

        public static DataTable GET_MEMBERS_REPORT(string number)
        {
            try
            {
                using(SqlConnection conn = DBHelper.GetConnection())
                {
                    string query = "";
                    if (string.IsNullOrWhiteSpace(number))
                    {
                        query = "SELECT * FROM MEMBERS_VIEW";
                    }
                    else
                    {
                        query = "SELECT * FROM MEMBERS_VIEW WHERE MemberID=@memberID";
                    }
                    using (SqlCommand cmd = new SqlCommand(query,conn))
                    {
                        if (!string.IsNullOrWhiteSpace(number))
                        {
                            cmd.Parameters.AddWithValue("@memberID", number);
                        }
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        conn.Open();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
