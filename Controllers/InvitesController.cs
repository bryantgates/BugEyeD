﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugEyeD.Data;
using BugEyeD.Models;
using Microsoft.AspNetCore.Authorization;
using BugEyeD.Models.Enums;
using BugEyeD.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using BugEyeD.Extensions;

namespace BugEyeD.Controllers
{
    [Authorize(Roles = nameof(BTRoles.Admin))]
    public class InvitesController : Controller
    {
        private readonly IBTInviteService _inviteService;
        private readonly IBTProjectService _projectService;
        private readonly IBTCompanyService _companyService;
        private readonly IEmailSender _emailSender;
        private readonly IDataProtector _protector;
        private readonly string _protectorPurpose;
        private readonly UserManager<BTUser> _userManager;

        public InvitesController(IBTInviteService inviteService,
                                 IBTProjectService projectService,
                                 IBTCompanyService companyService,
                                 IEmailSender emailSender,
                                 UserManager<BTUser> userManager,
                                 IDataProtectionProvider protectionProvider)
        {
            _inviteService = inviteService;
            _projectService = projectService;
            _companyService = companyService;
            _emailSender = emailSender;
            _userManager = userManager;

            _protectorPurpose = "Bug!@I$$Dee4723??";
            _protector = protectionProvider.CreateProtector(_protectorPurpose);
        }

        // GET: Invites/Create
        public async Task<IActionResult> Create()
        {
            List<Project> companyProjects = await _projectService.GetAllProjectsByCompanyIdAsync(User.Identity!.GetCompanyId());

            ViewData["ProjectId"] = new SelectList(companyProjects, "Id", "Name");

            return View();
        }

        // POST: Invites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,InviteeEmail,InviteeFirstName,InviteeLastName,Message")] Invite invite)
        {
            int companyId = User.Identity!.GetCompanyId();
            ModelState.Remove(nameof(Invite.InvitorId));

            if (ModelState.IsValid)
            {
                try
                {
                    Guid guid = Guid.NewGuid();

                    invite.CompanyToken = guid;
                    invite.CompanyId = companyId;
                    invite.InviteDate = DateTime.UtcNow;
                    invite.InvitorId = _userManager.GetUserId(User);
                    invite.IsValid = true;

                    await _inviteService.AddNewInviteAsync(invite);

                    // send the invite email

                    // encrypting our top secret ivite information
                    string token = _protector.Protect(guid.ToString());
                    string email = _protector.Protect(invite.InviteeEmail!);
                    string company = _protector.Protect(companyId.ToString());

                    string? callbackUrl = Url.Action("ProcessInvite", "Invites", new { token, email, company });

                    string body = $@"<h4>You've been invited to join the BugEyeD bug tracker!</h4><br />
                                         {invite.Message}<br /><br />
                                         <a href=""{callbackUrl}"">Click here</a> to join our team.";

                    string subject = "You've been invite to join BugEyeD Bug Tracker";

                    await _emailSender.SendEmailAsync(invite.InviteeEmail!, subject, body);

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception)
                {

                    throw;
                }
            }

            List<Project> companyProjects = await _projectService.GetAllProjectsByCompanyIdAsync(User.Identity!.GetCompanyId());
            ViewData["ProjectId"] = new SelectList(companyProjects, "Id", "Name", invite.ProjectId);
            return View(invite);
        }


        public async Task<IActionResult> ProcessInvites(string? token, string? email, string? company)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(company))
            {
                return NotFound();
            }

            Guid companyToken = Guid.Parse(_protector.Unprotect(token));
            string inviteeEmail = _protector.Unprotect(email);
            int companyId = int.Parse(_protector.Unprotect(company));

            try
            {
                Invite? invite = await _inviteService.GetInviteAsync(companyToken, inviteeEmail, companyId);

                if (invite == null)
                {
                    return NotFound();
                }

                return View(invite);
            }
            catch (Exception)
            {

                throw;
            }

            return View();

        }
    }
}
