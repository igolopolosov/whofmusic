using System.Linq;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace WarehouseOfMusic.Views
{
    public partial class HelpPage : PhoneApplicationPage
    {
        public HelpPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the page is activated
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var previousPage = NavigationService.BackStack.First();
            if (previousPage.Source.ToString().Contains("ProjectEditorPage.xaml"))
            {
                HelpPanorama.DefaultItem = HelpPanorama.Items[1];
            }
            if (previousPage.Source.ToString().Contains("TrackEditorPage.xaml"))
            {
                HelpPanorama.DefaultItem = HelpPanorama.Items[2];
            }
            if (previousPage.Source.ToString().Contains("SampleEditorPage.xaml"))
            {
                HelpPanorama.DefaultItem = HelpPanorama.Items[3];
            }
        }
    }
}