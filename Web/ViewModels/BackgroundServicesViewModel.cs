using Application.Models;
using Domain.Entities;

namespace Web.ViewModels
{
    public class BackgroundServicesViewModel
    {
        public List<BackgroundServices> ListOfBackgroundServicesName { get; set; } = new();
        public List<BackgroundServiceTracking> ListOfRunningServices { get; set; } = new();
    }
}
