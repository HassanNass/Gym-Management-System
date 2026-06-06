using GymManagement_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GymManagement_Project
{
    public partial class DashboardForm : Form
    {
        private readonly User _currentUser;
        public DashboardForm(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            lblMorning.Text = $"Welcome, {_currentUser.Username} ({_currentUser.Role})";
            if (_currentUser.Role == "Staff")
            {
                lblPlans.Enabled = false;
                lblUsers.Enabled = false;
            }
            LoadDashboardStatus();
        }

        private void LoadDashboardStatus()
        {
            DashboardRepository repo = new DashboardRepository();
            Dashboard stats = repo.GetStats();

            lblActiveNumber.Text = stats.ActiveMembers.ToString();
            lblExpiring.Text = stats.ExpiringMembers.ToString();
            lblMembersCount.Text = stats.TotalMembers.ToString();
        }

        private void lblMembers_Click(object sender, EventArgs e)
        {
            new MembersForm(_currentUser).Show();
            this.Close();
        }

        private void lblPlans_Click(object sender, EventArgs e)
        {
            new PlansForm(_currentUser).Show();
            this.Close();
        }

        private void lblUsers_Click(object sender, EventArgs e)
        {
            new UsersForm(_currentUser).Show();
            this.Close();
        }

        private void lblLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                new LoginForm().Show();
                this.Close();
            }
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
