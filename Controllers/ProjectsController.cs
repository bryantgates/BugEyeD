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
using BugEyeD.Models.Enums;
using Microsoft.AspNetCore.Identity;
using BugEyeD.Services.Interfaces;
using BugEyeD.Extensions;
using BugEyeD.Models.ViewModels;
using System.ComponentModel.Design;

namespace BugEyeD.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTFileService _fileService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectService;


        public ProjectsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTFileService fileService, IBTRolesService rolesService, IBTProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _fileService = fileService;
            _rolesService = rolesService;
            _projectService = projectService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            BTUser? user = await _userManager.GetUserAsync(User);
            int companyId = user!.CompanyId;

            var projects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);
            return View(projects);
        }

        [HttpGet]
        [Authorize(Roles = nameof(BTRoles.Admin))]
        public async Task<IActionResult> AssignPm(int? id)
        {
            if (id is null or 0)
            {
                return NotFound();
            }

            int companyId = User.Identity!.GetCompanyId();

            Project? project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project is null)
            {
                return NotFound();
            }

            List<BTUser> projectManagers = await _rolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId);
            BTUser? currentPm = await _projectService.GetProjectManagerAsync(id.Value, companyId);

            AssignPmViewModel viewModel = new AssignPmViewModel()
            {
                Project = project,
                PMId = currentPm?.Id,
                PMList = new SelectList(projectManagers, "Id", "FullName", currentPm?.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = nameof(BTRoles.Admin))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPM(AssignPmViewModel viewModel)
        {
            if (viewModel.Project?.Id != null)
            {
                if (string.IsNullOrEmpty(viewModel.PMId))
                {
                    await _projectService.RemoveProjectManagerAsync(viewModel.Project.Id, User.Identity!.GetCompanyId());
                } else
                {
                    await _projectService.AddProjectManagerAsync(viewModel.PMId, viewModel.Project.Id, User.Identity!.GetCompanyId());
                }

                return RedirectToAction(nameof(Details), new { id = viewModel.Project!.Id });
            }

            return BadRequest();
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BTUser? user = await _userManager.GetUserAsync(User);

            int companyId = user!.CompanyId;

            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

           
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }


        [Authorize(Roles = $"{nameof(BTRoles.Admin)}, {nameof(BTRoles.ProjectManager)}")]
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectPriorityId"] = new SelectList(await _projectService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{nameof(BTRoles.Admin)}, {nameof(BTRoles.ProjectManager)}")]
        public async Task<IActionResult> Create([Bind("Name,Description,StartDate,EndDate,ProjectPriorityId,ImageFormFile")] Project project)
        {
            ModelState.Remove("project.CompanyId");

            if (ModelState.IsValid)
            {
                project.Created = DateTime.UtcNow;
                project.StartDate = DateTime.SpecifyKind(project.StartDate, DateTimeKind.Utc);
                project.EndDate = DateTime.SpecifyKind(project.EndDate, DateTimeKind.Utc);

                BTUser? user = await _userManager.GetUserAsync(User);
                project.CompanyId = user!.CompanyId;

                if (project.ImageFormFile is not null)
                {
                    project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(project.ImageFormFile);
                    project.ImageFileType = project.ImageFormFile.ContentType;
                }

                if (User.IsInRole(nameof(BTRoles.ProjectManager)))
                {
                    project.Members.Add(user);
                }

                await _projectService.AddProjectAsync(project);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectPriorityId"] = new SelectList( await _projectService.GetProjectPrioritiesAsync(), "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = $"{nameof(BTRoles.Admin)}, {nameof(BTRoles.ProjectManager)}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BTUser? user = await _userManager.GetUserAsync(User);
            int companyId = user!.CompanyId;

            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);


            if (project == null)
            {
                return NotFound();
            }
            ViewData["ProjectPriorityId"] = new SelectList(await _projectService.GetProjectPrioritiesAsync(), "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{nameof(BTRoles.Admin)}, {nameof(BTRoles.ProjectManager)}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,Name,Created,Description,StartDate,EndDate,ProjectPriorityId,ImageFormFile,ImageFileData,ImageFileType,Archived")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    project.Created = DateTime.SpecifyKind(project.Created, DateTimeKind.Utc);
                    project.StartDate = DateTime.SpecifyKind(project.StartDate, DateTimeKind.Utc);
                    project.EndDate = DateTime.SpecifyKind(project.EndDate, DateTimeKind.Utc);



                        if (project.ImageFormFile != null)
                        {
                            project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(project.ImageFormFile);
                            project.ImageFileType = project.ImageFormFile.ContentType;
                        }

                    int companyId = User.Identity!.GetCompanyId();
                    await _projectService.UpdateProjectAsync(project, companyId);

                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
                return RedirectToAction(nameof(Details), new { id = project.Id });
            }

            ViewData["ProjectPriorityId"] = new SelectList(await _projectService.GetProjectPrioritiesAsync(), "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }


        // GET: Projects/Archive/5
        [Authorize(Roles = $"{nameof(BTRoles.Admin)}, {nameof(BTRoles.ProjectManager)}")]
        public async Task<IActionResult> Archive(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                int companyId = User.Identity!.GetCompanyId();
                var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

                if (project == null)
                {
                    return NotFound();
                }

                return View(project);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // POST: Projects/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{nameof(BTRoles.Admin)}, {nameof(BTRoles.ProjectManager)}")]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            try
            {
                int companyId = User.Identity!.GetCompanyId();
                var project = await _projectService.GetProjectByIdAsync(id, companyId);

                if (project != null)
                {
                    project.Archived = true;
                }

                await _projectService.ArchiveProjectAsync(project!, companyId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
