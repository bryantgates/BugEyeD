﻿using BugEyeD.Models;
using Microsoft.AspNetCore.Identity;

namespace BugEyeD.Services.Interfaces
{
	public interface IBTTicketHistoryService
	{
		Task AddHistoryAsync(Ticket? oldTicket, Ticket newTicket, string userId);
		Task AddHistoryAsync(int ticketId, string model, string userId);
		Task<List<TicketHistory>> GetProjectTicketHistoriesAsync(int projectid, int companyId);
		Task<List<TicketHistory>> GetCompanyTicketHistoriesAsync(int companyId);
	}
}
