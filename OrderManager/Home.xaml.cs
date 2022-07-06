using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using MySqlConnector;
using System.Configuration;
using Windows.UI;

// "Console Log" : Debug.WriteLine() -> using System.Diagnostics;

namespace OrderManager
{
    public sealed partial class Home : Page
    {
        public Home()
        {
            this.InitializeComponent();

            Database db = new Database();
            string commandsQuery = "SELECT * FROM commandes";
            MySqlCommand commandsCommand = new MySqlCommand(commandsQuery, db.dbConnection);
            db.OpenConnection();
            MySqlDataReader commandsResult = commandsCommand.ExecuteReader();

            while (commandsResult.Read())
            {
                myStack stack = new myStack();
                cmd_list.Children.Add(stack);

                stack.Children.Add(MyText("Commande de " + commandsResult.GetString(commandsResult.GetOrdinal("buyer_name"))));
                stack.Children.Add(MyText("Prix : " + commandsResult.GetInt32(commandsResult.GetOrdinal("price")).ToString()));
                stack.Children.Add(MyText("Status : " + commandsResult.GetString(commandsResult.GetOrdinal("status"))));
            }

            db.CloseConnection();
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            switch (item.Tag.ToString())
            {
                case "home":
                    break;

                case "add":
                    homeFrame.Navigate(typeof(AddCommand), null, new SuppressNavigationTransitionInfo());
                    break;
            }
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

        class myStack : StackPanel
        {
            public myStack()
            {
                Orientation = Orientation.Horizontal;
                HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Center;
                BorderThickness = new Thickness(3);
                BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 100));
            }
        }

        public TextBlock MyText(string t)
        {
            TextBlock text = new TextBlock();
            text.Text = t;
            text.Margin = new Thickness(0, 0, 20, 0);

            return text;
        }
    }
}
