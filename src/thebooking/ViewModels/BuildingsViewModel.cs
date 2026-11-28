using thebooking.Models;

namespace thebooking.ViewModels;

// The buildings to show in the building table, plus the page title
public class BuildingsViewModel
{
    public IEnumerable<Building> Buildings;
    public string? CurrentViewName;

    public BuildingsViewModel(IEnumerable<Building> buildings, string? currentViewName)
    {
        Buildings = buildings;
        CurrentViewName = currentViewName;
    }
}