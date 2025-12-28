using qltb.Models;

namespace qltb.ViewModels
{
    public class EquipmentDetailViewModel : BaseViewModel
    {
        private EquipmentItem _equipment;

        public EquipmentItem Equipment
        {
            get => _equipment;
            set
            {
                _equipment = value;
                OnPropertyChanged();
            }
        }

        public EquipmentDetailViewModel()
        {
        }

        public EquipmentDetailViewModel(EquipmentItem item)
        {
            Equipment = item;
        }
    }
}
