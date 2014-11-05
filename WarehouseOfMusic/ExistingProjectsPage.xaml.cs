namespace WarehouseOfMusic
{
    using Microsoft.Phone.Controls;

    public partial class ExistingProjectsPage : PhoneApplicationPage
    {
        public ExistingProjectsPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
        }
    }
}