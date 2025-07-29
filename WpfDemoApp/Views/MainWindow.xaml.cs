using System.Windows;
using WpfDemoApp.ViewModels;

namespace WpfDemoApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
        }
    }
}