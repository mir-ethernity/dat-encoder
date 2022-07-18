using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Mir.Ethernity.Dat.Editor
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string FILTER_DIALOG = "Mir Files|*.dat;*.edt;*.exp;*.mif";
        public FileEditorContext? FileContext { get; set; } = null;

        public MainWindow()
        {
            InitializeComponent();
            txtContent.IsReadOnly = true;
            UpdateViewState();
        }

        private void File_New_Click(object sender, RoutedEventArgs e)
        {
            if (FileContext?.IsModified ?? false)
            {
                var boxResult = MessageBox.Show("You have pending changes, are you sure you want to discard them and create a new file?", "New file confirmation", MessageBoxButton.OKCancel);
                if (boxResult != MessageBoxResult.OK) return;
            }

            FileContext = new FileEditorContext();
            UpdateViewState();
        }

        private void File_Open_Click(object sender, RoutedEventArgs e)
        {
            if (FileContext?.IsModified ?? false)
            {
                var boxResult = MessageBox.Show("You have pending changes, are you sure you want to discard them and open a file?", "Open file confirmation", MessageBoxButton.OKCancel);
                if (boxResult != MessageBoxResult.OK) return;
            }

            var openFileDialog = new OpenFileDialog() { Filter = FILTER_DIALOG };
            if (openFileDialog.ShowDialog() == true)
            {
                if (!File.Exists(openFileDialog.FileName))
                {
                    MessageBox.Show($"File not found: {openFileDialog.FileName}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var buffer = File.ReadAllBytes(openFileDialog.FileName);
                var output = DatEncoder.Decode(buffer);

                FileContext = new FileEditorContext();
                FileContext.FilePath = openFileDialog.FileName;
                FileContext.OriginalContent = System.Text.Encoding.ASCII.GetString(output);
                FileContext.Content = FileContext.OriginalContent;

                UpdateViewState();
            }

        }

        private void File_Save_Click(object sender, RoutedEventArgs e)
        {
            if (FileContext == null || !FileContext.IsModified) return;

            if (FileContext.IsNew || string.IsNullOrEmpty(FileContext.FilePath))
            {
                var saveFileDialog = new SaveFileDialog() { Filter = FILTER_DIALOG };
                if (saveFileDialog.ShowDialog() == false) return;
                FileContext.FilePath = saveFileDialog.FileName;
            }

            var buffer = System.Text.Encoding.ASCII.GetBytes(FileContext.Content);
            var output = DatEncoder.Encode(buffer);

            File.WriteAllBytes(FileContext.FilePath, output);

            FileContext.OriginalContent = FileContext.Content;

            UpdateViewState();
        }

        private void File_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (FileContext == null) return;

            var saveFileDialog = new SaveFileDialog() { Filter = FILTER_DIALOG };
            if (saveFileDialog.ShowDialog() == false) return;
            FileContext.FilePath = saveFileDialog.FileName;

            var buffer = System.Text.Encoding.ASCII.GetBytes(FileContext.Content);
            var output = DatEncoder.Encode(buffer);

            File.WriteAllBytes(FileContext.FilePath, output);

            FileContext.OriginalContent = FileContext.Content;

            UpdateViewState();
        }


        private void TxtContent_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (FileContext == null) return;
            FileContext.Content = txtContent.Text;
            UpdateViewState();
        }

        private void UpdateViewState()
        {
            txtContent.IsReadOnly = FileContext == null;
            txtContent.Text = FileContext?.Content ?? "";
            UpdateLabelStatus();
        }

        private void UpdateLabelStatus()
        {
            if (FileContext == null)
            {
                lblStatus.Content = "No file opened or created";
                return;
            }

            lblStatus.Content = $"File name: {FileContext.FileName}, Length: {FileContext.Content.Length}, Is modified: {(FileContext.IsModified ? "Yes" : "No")}";
        }
    }
}
