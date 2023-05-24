using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugEyeD.Models
{
	public class Project
	{
		public int Id { get; set; }
		public int CompanyId { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Description { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime Created { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime StartDate { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime EndDate { get; set; }
		public int ProjectPriorityId { get; set; }

		[NotMapped]
		public IFormFile? ImageFile { get; set; }
		public byte[]? ImageData { get; set; }
		public string? ImageType { get; set; }

		public bool Archived { get; set; }

		// Navigation properties
		public virtual Company Company { get; set; }
		public virtual ProjectPriority ProjectPriority { get; set; }
		public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
		public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
	}
}
