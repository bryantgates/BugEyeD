using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugEyeD.Models.ViewModels
{
    public class AssignPmViewModel
    {
        public Project? Project { get; set; }
        public SelectList? PMList { get; set; }
        public string? PMId { get; set; }
    }
}
