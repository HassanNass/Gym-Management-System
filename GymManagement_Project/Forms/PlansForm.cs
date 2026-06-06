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
    public partial class PlansForm : Form
    {
        private readonly PlanRepository _planRepo = new PlanRepository();
        private readonly User _currentUser;
        private int _selectedID = -1;
        public PlansForm(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void PlansForm_Load(object sender, EventArgs e)
        {
            LoadPlans();
        }

        private void LoadPlans()
        {
            dgvPlans.DataSource = _planRepo.GetAll();
            dgvPlans.Columns["PlanID"].Visible = false;
        }

        private void dgvPlans_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvPlans.Rows[e.RowIndex];

            _selectedID = (int)row.Cells["PlanID"].Value;
            txtPlanName.Text = row.Cells["PlanName"].Value.ToString();
            txtPlanPrice.Text = row.Cells["Price"].Value.ToString();
            txtPlanDuration.Text = row.Cells["DurationMonths"].Value.ToString();
            txtPlanDescription.Text = row.Cells["Description"].Value == DBNull.Value ? "" : row.Cells["Description"].Value.ToString();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                Plan plan = GetPlanFromInputs();
                _planRepo.Add(plan);
                MessageBox.Show("Plan added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPlans();
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
                MessageBox.Show("Please select a plan to update.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs()) return;

            try
            {
                Plan plan = GetPlanFromInputs();
                plan.PlanID = _selectedID;
                _planRepo.Update(plan);
                MessageBox.Show("Plan updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPlans();
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
                MessageBox.Show("Please select a plan to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this plan?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    _planRepo.Delete(_selectedID);
                    MessageBox.Show("Plan deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadPlans();
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

        private Plan GetPlanFromInputs()
        {
            return new Plan
            {
                PlanName = txtPlanName.Text.Trim(),
                Price = decimal.Parse(txtPlanPrice.Text.Trim()),
                DurationMonths = int.Parse(txtPlanDuration.Text.Trim()),
                Description = txtPlanDescription.Text.Trim()
            };
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtPlanName.Text))
            {
                MessageBox.Show("Please enter a plan name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtPlanPrice.Text, out _))
            {
                MessageBox.Show("Please enter a valid price.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtPlanDuration.Text, out _))
            {
                MessageBox.Show("Please enter a valid duration in months.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtPlanName.Clear();
            txtPlanPrice.Clear();
            txtPlanDuration.Clear();
            txtPlanDescription.Clear();
            _selectedID = -1;
            dgvPlans.ClearSelection();
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
