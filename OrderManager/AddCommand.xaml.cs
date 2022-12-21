using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using MySqlConnector;
using System;
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

            Database db = new Database();
            string typeQuery = "SELECT data FROM settings";
            MySqlCommand typeCommand = new MySqlCommand(typeQuery, db.dbConnection);
            db.OpenConnection();
            MySqlDataReader typeResult = typeCommand.ExecuteReader();

            int i = 0;
            while (typeResult.Read())
            {
                if (i != 1)
                {
                    foreach (string data in typeResult.GetString(0).Split(',')) {
                        if (i == 0) platformBox.Items.Add(data);
                        if (i == 2) serviceBox.Items.Add(data);
                    }
                }
                else foreach (string data in typeResult.GetString(0).Split(',')) statusBox.Items.Add(data.Split(':')[0]);
                i++;
            }

            db.CloseConnection();
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
            string addCommandQuery = "INSERT INTO `commandes` (`buyer_name`, `service_name`, `platform`, `price`, `status`, `description`) VALUES ('" + MySQLEscape(name.Text) + "', '" + serviceBox.SelectedItem + "', '" + platformBox.SelectedItem + "', '" + MySQLEscape(price.Text) + "', '" + statusBox.SelectedItem + "', '" + MySQLEscape(description.Text) + "')";
            MySqlCommand addCommandCommand = new MySqlCommand(addCommandQuery, db.dbConnection);
            db.OpenConnection();
            addCommandCommand.ExecuteScalar();
            db.CloseConnection();

            addFrame.Navigate(typeof(Home), null, new SuppressNavigationTransitionInfo());
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

        private void NavView_PaneOpening(NavigationView sender, object args)
        {
        }

        private void NavView_PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs args)
        {
        }

        // New feature (maybe) -> add more platforms (ex) in the config file (permanent way : in the App.config file; and temporally way : in the cache/memory)
        // Need :
        //  - TextBox for the name of the platform (x:Name="addPlatform")
        //  - Button for validate the addition (x:Name="addPlatformButton" Click="addPlatformButton_Click")
        //
        //private void addPlatformButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        //{
        //    Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    configFile.AppSettings.Settings["platform"].Value = ConfigurationManager.AppSettings.Get("platform") + "," + addPlatform.Text;
        //    // Save in memory (for modify during the execution)
        //    configFile.Save(ConfigurationSaveMode.Modified);
        //    // Save in the App.config file (for persistence)
        //    configFile.SaveAs(AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\..\\..\\..\\OrderManager\\App.config");
        //    ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

        //    // Refresh page
        //    addFrame.Navigate(typeof(AddCommand), null, new SuppressNavigationTransitionInfo());
        //}
    }

}
