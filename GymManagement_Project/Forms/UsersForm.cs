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
    public partial class UsersForm : Form
    {
        private readonly UserRepository _userRepo = new UserRepository();
        private readonly User _currentUser;
        private int _selectedID = -1;
        public UsersForm(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void UsersForm_Load(object sender, EventArgs e)
        {
            LoadRolesComboBox();
            LoadUsers();
        }

        private void LoadUsers()
        {
            dgvUsers.DataSource = _userRepo.GetAll();
            dgvUsers.Columns["UserID"].Visible = false;
        }

        private void LoadRolesComboBox()
        {
            cbRole.Items.Clear();
            cbRole.Items.Add("Admin");
            cbRole.Items.Add("Staff");
            cbRole.SelectedIndex = -1;
        }

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvUsers.Rows[e.RowIndex];

            _selectedID = (int)row.Cells["UserID"].Value;
            txtUserName.Text = row.Cells["Username"].Value.ToString();
            txtPassword.Text = "";
            cbRole.SelectedItem = row.Cells["Role"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                User user = GetUserFromInputs();
                _userRepo.Add(user);
                MessageBox.Show("User added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedID == -1)
            {
                MessageBox.Show("Please select a user to update.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs()) return;

            try
            {
                User user = GetUserFromInputs();
                user.UserID = _selectedID;
                _userRepo.Update(user);
                MessageBox.Show("User updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadUsers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedID == -1)
            {
                MessageBox.Show("Please select a user to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    _userRepo.Delete(_selectedID);
                    MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUsers();
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private User GetUserFromInputs()
        {
            return new User
            {
                Username = txtUserName.Text.Trim(),
                Password = txtPassword.Text.Trim(),
                Role = cbRole.SelectedItem.ToString()
            };
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("Please enter a username.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter a password.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cbRole.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a role.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtUserName.Clear();
            txtPassword.Clear();
            cbRole.SelectedIndex = -1;
            _selectedID = -1;
            dgvUsers.ClearSelection();
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblBack_Click(object sender, EventArgs e)
        {
            new DashboardForm(_currentUser).Show();
            this.Close();
        }
    }
}
