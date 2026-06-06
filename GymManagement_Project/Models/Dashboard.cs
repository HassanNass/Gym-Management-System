using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement_Project.Models
{
    public class Dashboard
    {
        public int ActiveMembers { get; set; }
        public int ExpiringMembers { get; set; }
        public int TotalMembers { get; set; }
    }

    public class DashboardRepository
    {
        public Dashboard GetStats()
        {
            Dashboard stats = new Dashboard();
            try
            {
                using (SqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string activeQuery = @"SELECT COUNT(*) FROM [Member] WHERE IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(activeQuery, conn))
                    {
                        stats.ActiveMembers = (int)cmd.ExecuteScalar();
                    }

                    string expiringQuery = @"SELECT COUNT(*) FROM [Member] m JOIN [Plan] p ON m.PlanID = p.PlanID 
                                             WHERE m.IsActive = 1
                                             AND DATEADD(MONTH, p.DurationMonths, m.JoinDate) >= GETDATE()
                                             AND DATEADD(MONTH, p.DurationMonths, m.JoinDate) <= DATEADD(DAY, 30, GETDATE())";
                    using (SqlCommand cmd = new SqlCommand(expiringQuery, conn))
                    {
                        stats.ExpiringMembers = (int)cmd.ExecuteScalar();
                    }

                    string totalQuery = "SELECT COUNT(*) FROM [Member]";
                    using (SqlCommand cmd = new SqlCommand(totalQuery, conn))
                    {
                        stats.TotalMembers = (int)cmd.ExecuteScalar();
                    }
                }
                return stats;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
