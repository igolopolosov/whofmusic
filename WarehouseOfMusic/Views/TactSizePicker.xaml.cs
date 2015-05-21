using System;
using System.Windows;
using System.Windows.Controls;

namespace WarehouseOfMusic.Views
{
    public partial class TactSizePicker : UserControl
    {
        /// <summary>
        /// Keep current chosen tact size
        /// </summary>
        public byte TactSize = 2;
        
        public TactSizePicker()
        {
            InitializeComponent();
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            var toggleButton = e.OriginalSource as RadioButton;
            if (toggleButton != null) TactSize = Convert.ToByte(toggleButton.Content);
        }
    }
}
