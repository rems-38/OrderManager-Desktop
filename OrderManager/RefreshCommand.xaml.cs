using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OrderManager
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RefreshCommand : Page
    {
        public RefreshCommand()
        {
            this.InitializeComponent();
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItem item = args.SelectedItem as NavigationViewItem;
            switch (item.Tag.ToString())
            {
                case "home":
                    refreshFrame.Navigate(typeof(MainWindow), null, new SuppressNavigationTransitionInfo());
                    break;

                case "add":
                    refreshFrame.Navigate(typeof(AddCommand), null, new SuppressNavigationTransitionInfo());
                    break;

                case "refresh":
                    refreshFrame.Navigate(typeof(RefreshCommand), null, new SuppressNavigationTransitionInfo());
                    break;
            }
        }
    }

}
