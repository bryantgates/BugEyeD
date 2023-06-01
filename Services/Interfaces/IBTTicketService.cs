using BugEyeD.Models;

namespace BugEyeD.Services.Interfaces
{
    public interface IBTTicketService
    {
        public interface IBTTicketService
        {
           public Task AddTicketAsync(Ticket ticket);
           public Task ArchiveTicketAsync(Ticket ticket, int companyId);
           public Task<List<Ticket>> GetArchivedTicketsAsync(int companyId);
           public Task<List<Ticket>> GetTicketsByCompanyIdAsync(int companyId);
           public Task<Ticket?> GetTicketByIdAsync(int ticketId, int companyId);
           public Task<List<TicketStatus>> GetTicketStatuses();
           public Task<List<TicketType>> GetTicketTypes();
           public Task<List<TicketPriority>> GetTicketPriorities();
           public Task RestoreTicketAsync(Ticket ticket, int companyId);
           public Task UpdateTicketAsync(Ticket ticket, int companyId);
           public Task<List<Ticket>> GetTicketsByUserIdAsync(string userId);
        }
    }
}
