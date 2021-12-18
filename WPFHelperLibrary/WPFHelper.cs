using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WPFHelperLibrary
{
    public static class WPFHelper
    {
        public static void RefreshContents(this ListBox listBox, List<string> list)
        {
            listBox.ItemsSource = null;
            listBox.ItemsSource = list;
        }

        public static MessageBoxResult ShowYesNoExclamationMessageBox(string message, string caption)
        {
            MessageBoxResult output;

            output = MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            return output;
        }

        public static void TextBoxInputWasInvalid(string message, TextBox textBox)
        {
            MessageBox.Show(message);
            textBox.Clear();
            textBox.Focus();
        }
    }
}
