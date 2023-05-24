using System.ComponentModel.DataAnnotations;

namespace BugEyeD.Models
{
	public class TicketComment
	{
		public int Id { get; set; }

		[Required]
		public string Comment { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime Created { get; set; }

		public int TicketId { get; set; }

		public string UserId { get; set; }

		// Navigation properties
		public virtual Ticket Ticket { get; set; }

		public virtual BTUser User { get; set; }
	}
}
