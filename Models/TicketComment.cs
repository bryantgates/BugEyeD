using System.ComponentModel.DataAnnotations;

namespace BugEyeD.Models
{
	public class TicketComment
	{
		public int Id { get; set; }

		[Required]
		[StringLength(500, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
		public string? Comment { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime Created { get; set; }

		public int TicketId { get; set; }
		[Required]
		public string? UserId { get; set; }

		// Navigation properties
		public virtual Ticket? Ticket { get; set; }

		public virtual BTUser? User { get; set; }
	}
}
