using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MySqlConnector;
using System.Configuration;
using System.Diagnostics;

namespace OrderManager
{
    public sealed partial class AddCommand : Page
    {
        public AddCommand()
        {
            this.InitializeComponent();
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            switch (item.Tag.ToString())
            {
                case "home":
                    addFrame.Navigate(typeof(Home), null, new SuppressNavigationTransitionInfo());
                    break;

                case "add":
                    break;
            }
        }

        private void addCommandButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Database db = new Database();
            string addCommandQuery = "INSERT INTO `commandes` (`buyer_name`, `price`, `status`, `platform`) VALUES ('" + name.Text + "', '" + price.Text + "', '" + statusBox.SelectedItem + "', '" + platformBox.SelectedItem + "')";
            MySqlCommand addCommandCommand = new MySqlCommand(addCommandQuery, db.dbConnection);
            db.OpenConnection();
            addCommandCommand.ExecuteScalar();
            db.CloseConnection();
        }

        class Database
        {
            public MySqlConnection dbConnection;

            public Database()
            {
                string server = ConfigurationManager.AppSettings.Get("server");
                string user = ConfigurationManager.AppSettings.Get("user");
                string password = ConfigurationManager.AppSettings.Get("password");
                string database = ConfigurationManager.AppSettings.Get("database");

                dbConnection = new MySqlConnection("Server=" + server + ";User ID=" + user + ";Password=" + password + ";Database=" + database);
            }

            public void OpenConnection()
            {
                dbConnection.Open();
            }

            public void CloseConnection()
            {
                dbConnection.Close();
            }

        }
    }

}
