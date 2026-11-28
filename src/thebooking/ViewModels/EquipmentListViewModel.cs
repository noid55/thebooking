using thebooking.Models;

namespace thebooking.ViewModels;

// The equipment to show in the equipment table, plus the page title
public class EquipmentListViewModel
{
    public IEnumerable<Equipment> EquipmentList;
    public string? CurrentViewName;

    public EquipmentListViewModel(IEnumerable<Equipment> equipmentList, string? currentViewName)
    {
        EquipmentList = equipmentList;
        CurrentViewName = currentViewName;
    }
}