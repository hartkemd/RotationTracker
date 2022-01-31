using DataAccessLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RotationTracker.Models
{
    public class RotationUIModel
    {
        public FullRotationModel FullRotationModel { get; set; }
        public TextBlock CurrentEmployeeTextBlock { get; set; }
        public ListBox RotationListBox { get; set; }
        public Label RotationNameLabel { get; set; }
        public TextBox RotationNotesTextBox { get; set; }
    }
}
