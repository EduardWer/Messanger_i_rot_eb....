using Messanger.MVVM.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Messanger.MVVM.View
{
    public partial class ConnectDialogWindow : Window
    {
        public ConnectDialogWindow()
        {
            InitializeComponent();
            DataContext = new ConnectDialogWindowVM();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
