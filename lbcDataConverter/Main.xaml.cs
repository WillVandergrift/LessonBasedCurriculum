using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using lbcDataConverter.ViewModels;

namespace lbcDataConverter
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        // =======================================================
        // Private Variables
        // =======================================================
        MainViewModel _MainVM = new MainViewModel();

        public Main()
        {
            InitializeComponent();
        }

        private void btnLoadCurriculum_Click(object sender, RoutedEventArgs e)
        {
            if (_MainVM.LoadCurriculum() == true)
            {
                MessageBox.Show("Curriculum loaded successfully!");
            }
            else
            {
                MessageBox.Show("Curriculum failed to load!");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _MainVM.SaveChanges();
        }

        private void btnConvertCourse_Click(object sender, RoutedEventArgs e)
        {
            if (_MainVM.ConvertCourseFields() == true)
            {
                MessageBox.Show("Course fields converted successfully!");
            }
            else
            {
                MessageBox.Show("Course Conversion failed!");
            }
        }

        private void btnConvertLesson_Click(object sender, RoutedEventArgs e)
        {
            if (_MainVM.ConvertLessonFields() == true)
            {
                MessageBox.Show("Lesson fields converted successfully!");
            }
            else
            {
                MessageBox.Show("Lesson Conversion failed!");
            }
        }

        private void btnConvertMyWorkspace_Click(object sender, RoutedEventArgs e)
        {
            if (_MainVM.ConvertMyWorkspace() == true)
            {
                MessageBox.Show("My Workspace converted successfully!");
            }
            else
            {
                MessageBox.Show("My Workspace Conversion failed!");
            }
        }

        private void btnConvertHTML_Click(object sender, RoutedEventArgs e)
        {
            if (_MainVM.ConvertHTMLFields() == true)
            {
                MessageBox.Show("HTML fields converted successfully!");
            }
            else
            {
                MessageBox.Show("HTML field Conversion failed!");
            }
        }
    }
}
