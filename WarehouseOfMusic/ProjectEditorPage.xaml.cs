using Microsoft.Phone.Controls;

namespace WarehouseOfMusic
{
    public partial class ProjectEditorPage : PhoneApplicationPage
    {
        public ProjectEditorPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
        }
    }
}