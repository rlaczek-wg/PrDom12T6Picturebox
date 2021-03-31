using StudentsDiary.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace StudentsDiary
{
    public partial class Main : Form
    {
        private FileHelper<List<Student>> _fileHelper = 
            new FileHelper<List<Student>>(Program.FilePath);
        private List<Group> _groups;
       

        public bool IsMaximize 
        { 
            get
            {
                return Settings.Default.isMaximize; 
            }
            set
            {
                Settings.Default.isMaximize = value;
            }
        }
            
        public Main()
        {
            InitializeComponent();
            _groups = _groups = GroupsHelper.GetGroups("Wszyscy");
            InitGroupsCombobox();
            RefreshDiary();  

           SetColumnsHeader();
            HideColums();
            btnDeletePicture.Visible = false;
            ptbPciture.Visible = false;

            if (IsMaximize)
            {
                WindowState = FormWindowState.Maximized;
            }

        }

        private void InitGroupsCombobox()
        {
            cmbGroups.DataSource = _groups;
            cmbGroups.DisplayMember = "Name";  //Dodajemy nazwe wlasciwosci z klasy Group
            cmbGroups.ValueMember = "Id";

        }

        private void HideColums()
        {
            dgvDiary.Columns[nameof(Student.GroupId)].Visible = false;
        }

        private void RefreshDiary()
        {
            var students = _fileHelper.DeserializeFromFile();

            var selectedGroupId = (cmbGroups.SelectedItem as Group).Id;
            if (selectedGroupId != 0)
            {
                students = students.Where(x => x.GroupId == selectedGroupId).ToList();
            }
            dgvDiary.DataSource = students;
            
        }

      
        private void SetColumnsHeader()
        {
            dgvDiary.Columns[nameof(Student.Id)].HeaderText = "Number";
            dgvDiary.Columns[nameof(Student.FirstName)].HeaderText = "Imie";
            dgvDiary.Columns[nameof(Student.LastName)].HeaderText = "Nazwisko";
            dgvDiary.Columns[nameof(Student.Comments)].HeaderText = "Uwagi";
            dgvDiary.Columns[nameof(Student.Math)].HeaderText = "Matematyka";
            dgvDiary.Columns[nameof(Student.Technology)].HeaderText = "Technologia";
            dgvDiary.Columns[nameof(Student.Physics)].HeaderText = "Fizyka";
            dgvDiary.Columns[nameof(Student.PolishLang)].HeaderText = "Jezyk polski";
            dgvDiary.Columns[nameof(Student.ForeignLang)].HeaderText = "Jezyk obcy";
            dgvDiary.Columns[nameof(Student.AdditionalClasses)].HeaderText = "Zajecia dodatkowe";
        }
        


        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addEitStudent = new AddEditStudent();
            addEitStudent.FormClosing += addEitStudent_FormClosing;
            addEitStudent.ShowDialog();
        }

        private void addEitStudent_FormClosing(object sender, FormClosingEventArgs e)
        {
            RefreshDiary();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvDiary.SelectedRows.Count == 0)
            {
                MessageBox.Show("Prosze zaznacz ucznia, ktorego dane chcesz edytowac");
                return;
            }

            var addEitStudent = new AddEditStudent(
                Convert.ToInt32(dgvDiary.SelectedRows[0].Cells[0].Value));
            addEitStudent.FormClosing += addEitStudent_FormClosing;
            addEitStudent.ShowDialog();

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDiary.SelectedRows.Count == 0)
            {
                MessageBox.Show("Prosze zaznacz ucznia, ktorego dane chcesz usunac");
                return;
            }

            var selectedStudent = dgvDiary.SelectedRows[0];  //Zaznaczony student
           var confirmDelete = 
                MessageBox.Show("Czy na pewno chcesz usunac ucznia" + selectedStudent.Cells[1].Value.ToString() +
           " "+ selectedStudent.Cells[2].Value.ToString().Trim(), 
           "Usuwanie ucznia", MessageBoxButtons.OKCancel);


            if (confirmDelete == DialogResult.OK)
            {
                DeleteStudent(Convert.ToInt32(selectedStudent.Cells[0].Value));
                RefreshDiary();
            }

        }

        private void DeleteStudent(int id)
        {
            var students = _fileHelper.DeserializeFromFile();
            students.RemoveAll(x => x.Id == id);
            _fileHelper.SerializeToFile(students);
          
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDiary();

        }

        private void dgvDiary_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                IsMaximize = true;
            }
            else
            {
                IsMaximize = false;
            }

            Settings.Default.Save();
        }


        private void btnAddPicture_Click(object sender, EventArgs e)
        {       
            OpenFileDialog open = new OpenFileDialog();   
            open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new System.IO.FileStream(@"C:\Users\RAPLA\Documents\C sharp\Repo\SDPicrureboxPrD13T6\Picture\ZostanProgrDNet.jpg", FileMode.Open, FileAccess.Read);
               // FileStream fs = new System.IO.FileStream(@"StudentsDiary\\ZostanProgrDNet.jpg", FileMode.Open, FileAccess.Read);
                ptbPciture.Image = Image.FromStream(fs);
                fs.Close();
                ptbPciture.Visible = true;
                btnDeletePicture.Visible = true;
            }

        }

        private void btnDeletePicture_Click(object sender, EventArgs e)
        {
            ptbPciture.Visible = false;
            btnDeletePicture.Visible = false;
        }
    }
}
