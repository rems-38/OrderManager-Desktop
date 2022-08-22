using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MySqlConnector;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace OrderManager
{
    public sealed partial class AddCommand : Page
    {
        public AddCommand()
        {
            this.InitializeComponent();

            foreach (string service in ConfigurationManager.AppSettings["service_name"].Split(',')) { serviceBox.Items.Add(service); }
            foreach (string status in ConfigurationManager.AppSettings["status"].Split(',')) { statusBox.Items.Add(status); }
            foreach (string platform in ConfigurationManager.AppSettings["platform"].Split(',')) { platformBox.Items.Add(platform); }
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
            string addCommandQuery = "INSERT INTO `commandes` (`buyer_name`, `service_name`, `platform`, `price`, `status`, `description`) VALUES ('" + MySQLEscape(name.Text) + "', '" + serviceBox.SelectedItem + "', '" + ChangeValue(platformBox.SelectedItem) + "', '" + MySQLEscape(price.Text) + "', '" + ChangeValue(statusBox.SelectedItem) + "', '" + MySQLEscape(description.Text) + "')";
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

        public static string MySQLEscape(string str)
        {
            return Regex.Replace(str, @"[\x00'""\b\n\r\t\cZ\\%_]",
                delegate (Match match)
                {
                    string v = match.Value;
                    switch (v)
                    {
                        case "\x00":            // ASCII NUL (0x00) character
                            return "\\0";
                        case "\b":              // BACKSPACE character
                            return "\\b";
                        case "\n":              // NEWLINE (linefeed) character
                            return "\\n";
                        case "\r":              // CARRIAGE RETURN character
                            return "\\r";
                        case "\t":              // TAB
                            return "\\t";
                        case "\u001A":          // Ctrl-Z
                            return "\\Z";
                        default:
                            return "\\" + v;
                    }
                });
        }

        public static string ChangeValue(object obj)
        {
            switch (obj)
            {
                case "En cours":
                    return "en_cours";
                case "Terminée":
                    return "terminée";
                case "Pas vendu":
                    return "pas_vendu";
                case "Pas payé":
                    return "pas_payé";
                case "Abandon":
                    return "abandon";

                case "Fiverr":
                    return "fiverr";
                case "5euros":
                    return "5euros";
                case "Leboncoin":
                    return "leboncoin";

                default:
                    return "";
            }
        }
    }

}
