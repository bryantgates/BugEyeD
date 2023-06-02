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
using BugEyeD.Services;
using System.ComponentModel.Design;
using BugEyeD.Services.Interfaces;

namespace BugEyeD.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTTicketService _ticketService;
        private readonly IBTProjectService _projectService;

        public TicketsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTTicketService ticketService, IBTProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _ticketService = ticketService;
            _projectService = projectService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            BTUser? user = await _userManager.GetUserAsync(User);
            int companyId = user!.CompanyId;

            var tickets = await _ticketService.GetTicketsByCompanyIdAsync(companyId);
            return View(tickets);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BTUser? user = await _userManager.GetUserAsync(User);
            int companyId = user!.CompanyId;

            var ticket = await _ticketService.GetTicketByIdAsync(id.Value, companyId);

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
            int companyId = user!.CompanyId;

            List<Project> projects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);

            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(await _ticketService.GetTicketPriorities(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _ticketService.GetTicketTypes(), "Id", "Name");
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
                List<TicketStatus> ticketStatuses = await _ticketService.GetTicketStatuses();

                TicketStatus ticketStatus = ticketStatuses.FirstOrDefault(ts => ts.Name == BTTicketStatuses.New.ToString());


                if (ticketStatus != null)
                {
                    ticket.TicketStatusId = ticketStatus.Id;
                }
                else
                {
                    return View();
                }

                await _ticketService.AddTicketAsync(ticket);
                return RedirectToAction(nameof(Index));
            }

            BTUser? user = await _userManager.GetUserAsync(User);
            int companyId = user!.CompanyId;

            List<Project> projects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);

            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(await _ticketService.GetTicketPriorities(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _ticketService.GetTicketTypes(), "Id", "Name");
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BTUser? user = await _userManager.GetUserAsync(User);
            int companyId = user!.CompanyId;

            var ticket = await _ticketService.GetTicketByIdAsync(id.Value, companyId);

            if (ticket == null)
            {
                return NotFound();
            }

            List<Project> projects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);

            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(await _ticketService.GetTicketPriorities(), "Id", "Id", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _ticketService.GetTicketStatuses(), "Id", "Id", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _ticketService.GetTicketTypes(), "Id", "Id", ticket.TicketTypeId);
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

            BTUser? user = await _userManager.GetUserAsync(User);
            int companyId = user!.CompanyId;

            if (ModelState.IsValid)
            {
                try
                {
                    ticket.Created = DateTime.SpecifyKind(ticket.Created, DateTimeKind.Utc);
                    ticket.Updated = DateTime.UtcNow;

                    await _ticketService.UpdateTicketAsync(ticket, companyId);
                }
                catch (DbUpdateConcurrencyException)
                {
                   throw;
                    
                }
                return RedirectToAction(nameof(Index));
            }
            

            List<Project> projects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);

            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["TicketPriorityId"] = new SelectList(await _ticketService.GetTicketPriorities(), "Id", "Id", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _ticketService.GetTicketStatuses(), "Id", "Id", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _ticketService.GetTicketTypes(), "Id", "Id", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            BTUser? user = await _userManager.GetUserAsync(User);
            int companyId = user!.CompanyId;

            var ticket = await _ticketService.GetTicketByIdAsync(id.Value, companyId);
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
            BTUser user = await _userManager.GetUserAsync(User);
            int companyId = user.CompanyId;

            var ticket = await _ticketService.GetTicketByIdAsync(id, companyId);
            if (ticket != null)
            {
                ticket.Archived = true;
            }

            await _ticketService.UpdateTicketAsync(ticket, companyId);
            return RedirectToAction(nameof(Index));
        }

    }
}
