using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HandyControl;
using Dnd_BBB;

namespace DndGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.Show();
        }

        private void TworzeniePButton_Click(object sender, RoutedEventArgs e)
        {
            TworzeniePostaciWindow tworzeniePostaciWindow = new TworzeniePostaciWindow();
            tworzeniePostaciWindow.Show();
        }

        private void EdycjaPButton_Click(object sender, RoutedEventArgs e)
        {
            EdycjaPostaciWindow edycjaPostaciWindow = new EdycjaPostaciWindow();
            edycjaPostaciWindow.Show();
        }

        private void TworzenieDButton_Click(object sender, RoutedEventArgs e)
        {
            TworzenieDruzynyWindow tworzenieDruzynyWindow = new TworzenieDruzynyWindow();
            tworzenieDruzynyWindow.Show();
        }

        private void EdycjaDButton_Click(object sender, RoutedEventArgs e)
        {
            EdycjaDruzynyWindow edycjaDruzynyWindow = new EdycjaDruzynyWindow();
            edycjaDruzynyWindow.Show();
        }
    }

}