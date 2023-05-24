using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BugEyeD.Models
{
	public class BTUser
	{
		public string Id { get; set; }

		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[NotMapped]
		public string FullName { get { return $"{FirstName} {LastName}"; } }

		[NotMapped]
		public IFormFile? ImageFile { get; set; }

		public byte[]? ImageData { get; set; }

		public string? ImageType { get; set; }

		public int CompanyId { get; set; }

		// Navigation properties
		public virtual Company Company { get; set; }

		public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
	}
}
