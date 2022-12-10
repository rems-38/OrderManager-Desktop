using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using MySqlConnector;
using System.Configuration;
using System.Diagnostics;
using Windows.UI;
using System;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using System.Collections.Generic;

// "Console Log" : Debug.WriteLine() -> using System.Diagnostics;

namespace OrderManager
{
    public sealed partial class Home : Page
    {
        public Home()
        {
            this.InitializeComponent();
            this.SizeChanged += Home_SizeChanged;

            Database db = new Database();
            string commandsQuery = "SELECT * FROM commandes";
            MySqlCommand commandsCommand = new MySqlCommand(commandsQuery, db.dbConnection);
            db.OpenConnection();
            MySqlDataReader commandsResult = commandsCommand.ExecuteReader();

            int[] nbr = { 0, 0 };

            while (commandsResult.Read())
            {
                Content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
                Content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });

                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(7) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(75) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(110) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(575) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(75) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(75) });
                Grid.SetRow(grid, (2 * nbr[0]) + 1);
                Content.Children.Add(grid);

                Database db2 = new Database();
                string statusQuery = "SELECT data FROM settings WHERE type='status'";
                MySqlCommand statusCommand = new MySqlCommand(statusQuery, db2.dbConnection);
                db2.OpenConnection();
                MySqlDataReader statusResult = statusCommand.ExecuteReader();

                var statusColor = new Dictionary<string, string>();

                while (statusResult.Read())
                {
                    string[] statusArray = statusResult.GetString(0).Split(',');
                    foreach (string statusCouple in statusArray)
                    {
                        statusColor.Add(statusCouple.Split(':')[0], statusCouple.Split(':')[1]);
                    }

                }

                db2.CloseConnection();

                foreach (KeyValuePair<string, string> pair in statusColor)
                {
                    if (pair.Key == commandsResult.GetString(commandsResult.GetOrdinal("status"))) {
                        var myColor = (Color)Microsoft.UI.Xaml.Markup.XamlBindingHelper.ConvertValue(typeof(Color), pair.Value);
                        Rectangle rectStatus1 = new Rectangle { Fill = new SolidColorBrush(myColor) };
                        Grid.SetColumn(rectStatus1, 1);
                        grid.Children.Add(rectStatus1);

                        TextBlock textStatus1 = new TextBlock { Text = pair.Key, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                        Grid.SetColumn(textStatus1, 6);
                        grid.Children.Add(textStatus1);
                        
                        if (pair.Key == "En cours") nbr[1]++;
                    }
                }

                Image img = new Image {
                    Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Assets/" + commandsResult.GetString(commandsResult.GetOrdinal("platform")) + "_200px.png")),
                    Height = 50,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetColumn(img, 2);
                grid.Children.Add(img);

                TextBlock textBuyer = new TextBlock { Text = commandsResult.GetString(commandsResult.GetOrdinal("buyer_name")), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetColumn(textBuyer, 3);
                grid.Children.Add(textBuyer);

                TextBlock textService = new TextBlock { Text = commandsResult.GetString(commandsResult.GetOrdinal("service_name")), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetColumn(textService, 4);
                grid.Children.Add(textService);

                TextBlock textPrice = new TextBlock { Text = commandsResult.GetFloat(commandsResult.GetOrdinal("price")).ToString() + "€", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetColumn(textPrice, 5);
                grid.Children.Add(textPrice);

                TextBlock textDesc = new TextBlock { Text = commandsResult.GetString(commandsResult.GetOrdinal("description")), VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(textDesc, 7);
                grid.Children.Add(textDesc);

                // peut etre ajouter un bouton "open folder"
                // ajouter un bouton "goto url" avec la page associé à la commande

                Button btnModif = new Button { Content = new Image { Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Assets/edit_96px.png")) }, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetColumn(btnModif, 8);
                grid.Children.Add(btnModif);
                
                Button btnDelete = new Button { Content = new Image { Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/Assets/waste_96px.png")) }, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
                Grid.SetColumn(btnDelete, 10);
                grid.Children.Add(btnDelete);

                nbr[0]++;
            }

            title.Text = "Vous avez " + nbr[1] + " commandes en cours"; // nombre total de commande -> nbr[0]

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
                string port = ConfigurationManager.AppSettings.Get("port");
                string user = ConfigurationManager.AppSettings.Get("user");
                string password = ConfigurationManager.AppSettings.Get("password");
                string database = ConfigurationManager.AppSettings.Get("database");

                dbConnection = new MySqlConnection("Server=" + server + ";Port=" + port + ";User ID=" + user + ";Password=" + password + ";Database=" + database);
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

        private void NavView_PaneOpening(NavigationView sender, object args)
        {
            NavView.IsPaneOpen = true;
            Content.Margin = new Thickness(200, 0, 0, 0);

            double[] size = Value_SizeChanged();
            Content.Width = size[0] - 200;
        }

        private void NavView_PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs args)
        {
            NavView.IsPaneOpen = false;
            Content.Margin = new Thickness(40, 0, 0, 0);

            double[] size = Value_SizeChanged();
            Content.Width = size[0] - 40;
        }

        private void Home_SizeChanged(object sender, RoutedEventArgs e)
        {
            double[] size = new double[2];
            size[0] = this.ActualWidth;
            size[1] = this.ActualHeight;

            Content.Width = size[0] - 40;
        }

        private double[] Value_SizeChanged()
        {
            double[] size = new double[2];
            size[0] = this.ActualWidth;
            size[1] = this.ActualHeight;

            return size;
        }
    }
}
