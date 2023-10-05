using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Visualizer.View
{
    /// <summary>
    /// Interaction logic for DatabaseConnectView.xaml
    /// </summary>
    public partial class DatabaseConnectView : UserControl
    {
        public static readonly RoutedEvent SettingTapEvent = EventManager.RegisterRoutedEvent(
            "Connect", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DatabaseConnectView));

        // Provide CLR accessors for the event
        public event RoutedEventHandler SettingTap
        {
            add { AddHandler(SettingTapEvent, value); }
            remove { RemoveHandler(SettingTapEvent, value); }
        }

        void OnSettingTap()
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(SettingTapEvent);
            RaiseEvent(newEventArgs);
        }

        public DatabaseConnectView()
        {
            InitializeComponent();
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
