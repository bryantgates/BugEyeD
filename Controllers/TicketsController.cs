using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugEyeD.Data;
using BugEyeD.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Net.Sockets;
using BugEyeD.Models.Enums;

namespace BugEyeD.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;

        public TicketsController(ApplicationDbContext context, UserManager<BTUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            BTUser? user = await _userManager.GetUserAsync(User);
            int companyIds = user!.CompanyId;

            var applicationDbContext = _context.Tickets.Where(c => c.Project!.CompanyId == user!.CompanyId).Include(t => t.DeveloperUser).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            BTUser? user = await _userManager.GetUserAsync(User);

            var ticket = await _context.Tickets
                .Where(c => c.Project!.CompanyId == user!.CompanyId)
                .Include(t => t.DeveloperUser)
                .Include(t => t.Project)
                .Include(t => t.SubmitterUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            BTUser? user = await _userManager.GetUserAsync(User);

            List<Project> projects = await _context.Projects.Where(c => c.CompanyId == user!.CompanyId).ToListAsync();

            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,TicketTypeId,TicketPriorityId,ProjectId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Created = DateTime.UtcNow;

                TicketStatus ticketStatus = await _context.TicketStatuses.FirstOrDefaultAsync(t => t.Name == BTTicketStatuses.New.ToString());
                if (ticketStatus != null)
                {
                    ticket.TicketStatusId = ticketStatus.Id;
                }
                else
                {
                    return View();
                }

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            BTUser? user = await _userManager.GetUserAsync(User);

            List<Project> projects = await _context.Projects.Where(c => c.CompanyId == user!.CompanyId).ToListAsync();

            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }

            BTUser? user = await _userManager.GetUserAsync(User);

            var ticket = await _context.Tickets.Where(c => c.Project!.CompanyId == user!.CompanyId).FirstOrDefaultAsync(t => t.Id == id);
           
            if (ticket == null)
            {
                return NotFound();
            }

            List<Project> projects = await _context.Projects.Where(c => c.CompanyId == user!.CompanyId).ToListAsync();

            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,Archived,ArchivedByProject,ProjectId,TicketTypeId,TicketStatusId,TicketPriorityId,DeveloperUserId,SubmitterUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ticket.Created = DateTime.SpecifyKind(ticket.Created, DateTimeKind.Utc);
                    ticket.Updated = DateTime.UtcNow;

                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            BTUser? user = await _userManager.GetUserAsync(User);

            List<Project> projects = await _context.Projects.Where(c => c.CompanyId == user!.CompanyId).ToListAsync();

            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null || _context.Tickets == null)
            {
                return NotFound();
            }
            BTUser? user = await _userManager.GetUserAsync(User);

            var ticket = await _context.Tickets
                .Where(c => c.Project!.CompanyId == user!.CompanyId)
                .Include(t => t.DeveloperUser)
                .Include(t => t.Project)
                .Include(t => t.SubmitterUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            if (_context.Tickets == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tickets'  is null.");
            }
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                ticket.Archived = true;
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
          return (_context.Tickets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
