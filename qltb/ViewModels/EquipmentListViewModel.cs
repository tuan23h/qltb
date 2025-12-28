using System.Collections.ObjectModel;
using qltb.Models;

namespace qltb.ViewModels
{
    public class EquipmentListViewModel : BaseViewModel
    {
        public ObservableCollection<EquipmentItem> Equipments { get; set; }

        public EquipmentListViewModel()
        {
            Equipments = new ObservableCollection<EquipmentItem>();
        }
    }
}
