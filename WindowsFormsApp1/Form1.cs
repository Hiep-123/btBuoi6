using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadComboBox();
            LoadData();
        }
        StudentContextDB context = new StudentContextDB();  
        List<viewModel> lstModels = new List<viewModel>();
        public void LoadComboBox()
        {
            var tenKhoa = context.Faculties.ToList();
            cmbKhoa.DataSource = tenKhoa;
            cmbKhoa.DisplayMember = "FacultyName";
            cmbKhoa.ValueMember = "FacultyID";
        }
        public void LoadData()
        {
            lstModels.Clear();
            var lstSinhVien = context.Students.ToList();
            foreach (var item in lstSinhVien)
            {
                viewModel vm = new viewModel();
                vm.StudentID = item.StudentID;  
                vm.FullName = item.FullName;
                vm.FacultyName = item.Faculty.FacultyName;
                vm.AverageScore = item.AverageScore;   
                lstModels.Add(vm);  
            }
            dtgSinhVien.DataSource = null;
            dtgSinhVien.DataSource = lstModels;   
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            kiemTraDuLieu();
            if (context.Students.Any(s => s.StudentID == txtMa.Text)) 
            {
                MessageBox.Show("MSSV đã tồn tại!");
                return;
            }
            var addStudent = new Student
            {
                StudentID = txtMa.Text,
                FullName = txtTen.Text,
                AverageScore = float.Parse(txtDiem.Text),
                FacultyID =(int)cmbKhoa.SelectedValue
            };
            context.Students.Add(addStudent);
            context.SaveChanges();
            MessageBox.Show("Thêm mới dữ liệu thành công!");
            RefeshTextBox();
            LoadData();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            kiemTraDuLieu();
            var existingStudent = context.Students.FirstOrDefault(s => s.StudentID == txtMa.Text);
            var studentID = dtgSinhVien.SelectedRows[0].Cells[0].Value.ToString();
            Student updateStudent = context.Students.FirstOrDefault(s => s.StudentID == studentID);
            if (existingStudent == null)
            {     
                MessageBox.Show("Không tìm thấy MSSV cần sửa!");
                return;
            }
            if (studentID != null)
            {
                updateStudent.StudentID = txtMa.Text;
                updateStudent.FullName = txtTen.Text;
                updateStudent.AverageScore = float.Parse(txtDiem.Text);
                updateStudent.FacultyID = (int)cmbKhoa.SelectedValue;
            }
            context.SaveChanges();
            MessageBox.Show("Cập nhật dữ liệu thành công!");
            RefeshTextBox();
            LoadData();
            }
        public void kiemTraDuLieu()
        {
            if (string.IsNullOrEmpty(txtMa.Text) ||
                string.IsNullOrEmpty(txtTen.Text) ||
                string.IsNullOrEmpty(txtDiem.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!");
                return;
            }
            if (txtMa.Text.Length != 10)
            {
                MessageBox.Show("Mã số sinh viên phải có 10 kí tự!");
                return;
            }
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            var studentID = dtgSinhVien.SelectedRows[0].Cells[0].Value.ToString();
            var student = context.Students.FirstOrDefault(u => u.StudentID == studentID);
             if (student == null)
             {
                 MessageBox.Show("Không tìm thấy MSSV cần xóa!");
                 return;
             }
             var confirm = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này ?", "Xác nhận xóa", MessageBoxButtons.YesNo);
             if (confirm == DialogResult.Yes)
             {
                 context.Students.Remove(student);
                 context.SaveChanges();
                 MessageBox.Show("Xóa sinh viên thành công !");
                 RefeshTextBox();
                 LoadData();
             }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit(); // Thoát chương trình
            }
        }

        private void dtgSinhVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) { return; }
            DataGridViewRow row = dtgSinhVien.Rows[e.RowIndex];
            if (row != null)
            {
                txtMa.Text = row.Cells[0].Value.ToString();
                txtTen.Text = row.Cells[1].Value.ToString();
                cmbKhoa.Text = row.Cells[2].Value.ToString();
                txtDiem.Text = row.Cells[3].Value.ToString();   
            }
        }
        public void RefeshTextBox()
        {
            txtMa.Clear();
            txtTen.Clear();
            cmbKhoa.SelectedIndex = -1;
            txtDiem.Clear();
        }
    }
}
