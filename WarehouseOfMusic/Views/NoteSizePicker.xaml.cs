using System;
using System.Windows;
using System.Windows.Controls;
using WarehouseOfMusic.UIElementContexts;

namespace WarehouseOfMusic.Views
{
    public partial class NoteSizePicker : UserControl
    {
        public byte Duration;
        
        public NoteSizePicker()
        {
            InitializeComponent();
        }

        private void NoteSizeSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var newValue = (int)Math.Round(e.NewValue);
            if (NoteSizeSlider != null) NoteSizeSlider.Value = newValue;
            Duration = (byte)newValue;
            if (SliderValueBlock != null) SliderValueBlock.Text = string.Empty + Duration;
        }

        private void NoteSizeSlider_OnLoaded(object sender, RoutedEventArgs e)
        {
            var slider = sender as Slider;
            if (slider == null) return;
            slider.Value = PianoRollContext.NoteDuration;
        }

        private void SliderValueBlock_OnLoaded(object sender, RoutedEventArgs e)
        {
            SliderValueBlock.Text = string.Empty + Duration;
        }
    }
}
