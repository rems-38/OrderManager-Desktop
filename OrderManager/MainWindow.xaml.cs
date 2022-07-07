using Microsoft.UI.Xaml;

namespace OrderManager
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            rootFrame.Navigate(typeof(Home));

            
        }
    }
}
