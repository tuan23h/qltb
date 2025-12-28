using qltb.Views;

namespace qltb.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public object CurrentView { get; set; }

        public MainViewModel()
        {
            CurrentView = new EquipmentListView();
        }
    }
}
