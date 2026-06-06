using GymManagement_Project.Forms;
using GymManagement_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GymManagement_Project
{
    public partial class MembersForm : Form
    {
        private readonly MemberRepository _memberRepo = new MemberRepository();
        private readonly PlanRepository _planRepo = new PlanRepository();
        private readonly User _currentUser;
        private string _imageName = "";
        private int _selectedID = -1;
        public MembersForm(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void LoadMembers()
        {
            dgvMembers.DataSource = _memberRepo.GetAll();
            dgvMembers.Columns["MemberID"].Visible = false;
            dgvMembers.Columns["PlanID"].Visible = false;
            dgvMembers.Columns["ImageName"].Visible = false;
        }

        private void LoadPlansComboBox()
        {
            cmPlan.DataSource = _planRepo.GetAll();
            cmPlan.DisplayMember = "PlanName";
            cmPlan.ValueMember = "PlanID";
            cmPlan.SelectedIndex = -1;
        }

        private void MembersForm_Load(object sender, EventArgs e)
        {
            LoadPlansComboBox();
            LoadMembers();
        }

        private void dgvMembers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvMembers.Rows[e.RowIndex];

            _selectedID = (int)row.Cells["MemberID"].Value;
            txtFirstName.Text = row.Cells["FirstName"].Value.ToString();
            txtLastName.Text = row.Cells["LastName"].Value.ToString();
            txtPhone.Text = row.Cells["Phone"].Value.ToString();
            dtpJoinDate.Value = (DateTime)row.Cells["JoinDate"].Value;
            cbIsActive.Checked = (bool)row.Cells["IsActive"].Value;

            if (row.Cells["PlanID"].Value != DBNull.Value)
                cmPlan.SelectedValue = (int)row.Cells["PlanID"].Value;
            else
                cmPlan.SelectedIndex = -1;

            string imageFromDB = row.Cells["ImageName"].Value == DBNull.Value ? "" : row.Cells["ImageName"].Value.ToString();
            _imageName = imageFromDB;
            string fullPath = Application.StartupPath + @"\Images\" + imageFromDB;
            if (File.Exists(fullPath))
                pbPicture.ImageLocation = fullPath;
            else
                pbPicture.ImageLocation = null;
        }

        private Member GetMemberFormInputs()
        {
            return new Member
            {
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                JoinDate = dtpJoinDate.Value,
                PlanID = cmPlan.SelectedIndex == -1 ? 0 : (int)cmPlan.SelectedValue,
                IsActive = cbIsActive.Checked,
                ImageName = _imageName
            };
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                Member member = GetMemberFormInputs();
                _memberRepo.Add(member);
                MessageBox.Show("Member added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMembers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedID == -1)
            {
                MessageBox.Show("Please select a member to update.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!ValidateInputs()) return;

            try
            {
                Member member = GetMemberFormInputs();
                member.MemberID = _selectedID;
                _memberRepo.Update(member);
                MessageBox.Show("Member updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMembers();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedID == -1)
            {
                MessageBox.Show("Please select a member to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this member?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _memberRepo.Delete(_selectedID);
                    MessageBox.Show("Member deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMembers();
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

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void ClearInputs()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtPhone.Clear();
            dtpJoinDate.Value = DateTime.Now;
            cmPlan.SelectedIndex = -1;
            cbIsActive.Checked = false;
            pbPicture.ImageLocation = null;
            _imageName = "";
            _selectedID = -1;
            dgvMembers.ClearSelection();
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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var memberList = _memberRepo.GetAll();

            dgvMembers.DataSource = memberList.Where(m => m.LastName.ToLower().Contains(txtSearch.Text.ToLower())).ToList();
        }

        private void pbPicture_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images|*.jpg;*.jpeg;*.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string folder = Path.Combine(Application.StartupPath, "Images");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                _imageName = Guid.NewGuid().ToString() + Path.GetExtension(ofd.FileName);
                string fullPath = Path.Combine(folder, _imageName);
                File.Copy(ofd.FileName, fullPath);
                pbPicture.ImageLocation = fullPath;
            }
        }

        private void reportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ReportForm().Show();
        }
    }
}
