using Messanger.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Messanger.MVVM.ViewModel
{
    internal class ConnectDialogWindowVM : BindingsHelper
    {
        #region Свойства
            private string _text;
            public string Text
            {
                get { return _text; }
                set
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        #endregion

        #region Комманды
        #endregion
    }
}
