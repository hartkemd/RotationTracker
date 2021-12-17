using System.Windows;
using System.Windows.Controls;

namespace WPFHelperLibrary
{
    public static class WPFHelper
    {
        public static void TextBoxInputWasInvalid(string message, TextBox textBox)
        {
            MessageBox.Show(message);
            textBox.Clear();
            textBox.Focus();
        }
    }
}
