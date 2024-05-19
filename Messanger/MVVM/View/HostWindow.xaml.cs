using Messanger.MVVM.Model;
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
    /// <summary>
    /// Логика взаимодействия для HostWindow.xaml
    /// </summary>
    public partial class HostWindow : Window
    {
        public HostWindow()
        {
            InitializeComponent();
            DataContext = new HostVM();
        }
    }
}
