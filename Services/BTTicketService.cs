using System.ComponentModel.Design;
using BugEyeD.Data;
using BugEyeD.Models;
using BugEyeD.Models.Enums;
using BugEyeD.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugEyeD.Services
{

    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;

        public BTTicketService(ApplicationDbContext context, IBTRolesService rolesService)
        {
            _context = context;
            _rolesService = rolesService;
        }

        public async Task AddTicketAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task ArchiveTicketAsync(Ticket ticket, int companyId)
        {
            try
            {
                var existingTicket = await GetTicketByIdAsync(ticket.Id, companyId);
                if (existingTicket != null)
                {
                    existingTicket.Archived = true;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
        {
            try
            {
                return await _context.Tickets.Where(t => t.Project!.CompanyId == companyId && t.Archived == true).ToListAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<List<Ticket>> GetTicketsByCompanyIdAsync(int companyId)
        {
            return await _context.Tickets.Where(t => t.Project!.CompanyId == companyId).ToListAsync();
        }

        public async Task<Ticket?> GetTicketByIdAsync(int ticketId, int companyId)
        {
            return await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId && t.Project!.CompanyId == companyId);
        }
        public async Task<List<TicketType>> GetTicketTypes()
        {
            return await _context.TicketTypes.ToListAsync();
        }

        public async Task<List<TicketPriority>> GetTicketPriorities()
        {
            return await _context.TicketPriorities.ToListAsync();
        }

        public async Task RestoreTicketAsync(Ticket ticket, int companyId)
        {
            try
            {
                var existingTicket = await GetTicketByIdAsync(ticket.Id, companyId);
                if (existingTicket != null)
                {
                    existingTicket.Archived = false;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task UpdateTicketAsync(Ticket ticket, int companyId)
        {
            if (await _context.Projects.AnyAsync(p => p.CompanyId == companyId && p.Id == ticket.ProjectId))
            {
                _context.Update(ticket);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Project not found");
            }

        }

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId)
        {
            try
            {
                BTUser? user = await _context.Users.FindAsync(userId);
                if (user is null) return new List<Ticket>();

                //admin
                if (await _rolesService.IsUserInRole(user, nameof(BTRoles.Admin)))
                {
                    return await GetTicketsByCompanyIdAsync(user.CompanyId);
                }
                //PM
                else if (await _rolesService.IsUserInRole(user, nameof(BTRoles.ProjectManager)))
                {
                    return await _context.Tickets
                                         .Include(t => t.TicketStatus)
                                         .Include(t => t.TicketType)
                                         .Include(t => t.TicketPriority)
                                         .Include(t => t.SubmitterUser)
                                         .Include(t => t.DeveloperUser)
                                         .Include(t => t.Project)
                                            .ThenInclude(p => p.Members)
                                         .Where(t => !t.Archived && t.Project!.Members.Contains(user))
                                         .ToListAsync();
                }
                else
                {
                    return await _context.Tickets
                                         .Include(t => t.TicketStatus)
                                         .Include(t => t.TicketType)
                                         .Include(t => t.TicketPriority)
                                         .Include(t => t.SubmitterUser)
                                         .Include(t => t.DeveloperUser)
                                         .Include(t => t.Project)
                                            .ThenInclude(p => p.Members)
                                         .Where(t => !t.Archived && (t.DeveloperUserId == userId || t.SubmitterUserId == userId))
                                         .ToListAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<TicketStatus>> GetTicketStatuses()
        {
            return await _context.TicketStatuses.ToListAsync();
        }
    }
}