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

            foreach (string service in ConfigurationManager.AppSettings.Get("service_name").Split(',')) { serviceBox.Items.Add(service); }
            foreach (string status in ConfigurationManager.AppSettings.Get("status").Split(',')) { statusBox.Items.Add(status); }
            foreach (string platform in ConfigurationManager.AppSettings.Get("platform").Split(',')) { platformBox.Items.Add(platform); }
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

        // New feature (maybe) -> add more platforms (ex) in the config file (permanent way : in the App.config file; and temporally way : in the "cache")
        // Need :
        //  - TextBox for the name of the platform (x:Name="addPlatform")
        //  - Button for validate the addition (x:Name="addPlatformButton" Click="addPlatformButton_Click")
        //
        //private void addPlatformButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        //{
        //    // "Cache" modification
        //    Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    configFile.AppSettings.Settings["platform"].Value = ConfigurationManager.AppSettings.Get("platform") + "," + addPlatform.Text;
        //    configFile.Save(ConfigurationSaveMode.Modified);
        //    ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

        //    // App.config modification
        //    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap { ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\..\\..\\..\\OrderManager\\App.config" };
        //    Configuration configFile2 = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        //    configFile2.AppSettings.Settings["platform"].Value = ConfigurationManager.AppSettings.Get("platform");
        //    configFile2.Save(ConfigurationSaveMode.Modified);
        //    ConfigurationManager.RefreshSection(configFile2.AppSettings.SectionInformation.Name);

        //    // Refresh page
        //    addFrame.Navigate(typeof(AddCommand), null, new SuppressNavigationTransitionInfo());
        //}
    }

}
