using DataAccessLibrary.Models;
using System.Windows.Controls;

namespace RotationTracker.Models
{
    public class RotationUIModel
    {
        public FullRotationModel FullRotationModel { get; set; }
        public TextBlock CurrentEmployeeTextBlock { get; set; }
        public TextBlock DateTimeTextBlock { get; set; }
        public ListBox RotationListBox { get; set; }
        public Label RotationNameLabel { get; set; }
        public TextBox RotationNotesTextBox { get; set; }
        public Button AdvanceButton { get; set; }
        public Button EditButton { get; set; }
    }
}
