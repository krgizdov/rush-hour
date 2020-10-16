namespace RushHour.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore.Internal;
    using RushHour.Data.Entities;
    using RushHour.Domain;
    using RushHour.Domain.Entities;
    using RushHour.Domain.Services;
    using RushHour.ViewModels.Appointment;

    [Route("api/admin/appointments")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = GlobalConstants.AdministratorRoleName)]
    public class AdminAppointmentController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;

        public AdminAppointmentController(UserManager<ApplicationUser> userManager,
            IAppointmentService appointmentService, IMapper mapper)
        {
            _userManager = userManager;
            _appointmentService = appointmentService;
            _mapper = mapper;
        }

        //// GET: api/admin/appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentAdminViewModel>>> GetPaginatedAppointments(int page = 1, int size = 10)
        {
            if (page <= 0 || size <= 0)
            {
                return BadRequest();
            }

            var userAppointments = await _appointmentService.GetPaginatedAppointmentsAsync(page, size);

            var appointmentAdminViews = _mapper.Map<IEnumerable<AppointmentAdminViewModel>>(userAppointments);

            return Ok(appointmentAdminViews);
        }

        //// GET: api/admin/appointments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentAdminViewModel>> GetAppointmentById(string id)
        {
            var appointmentDto = await _appointmentService.GetAppointmentByIdAsync(id);

            if (appointmentDto == null)
            {
                return NotFound();
            }

            var appointmentAdminView = _mapper.Map<AppointmentAdminViewModel>(appointmentDto);

            return Ok(appointmentAdminView);
        }

        //// POST: api/admin/appointments
        [HttpPost]
        public async Task<ActionResult<AppointmentAdminViewModel>> CreateAppointment(string userId, [FromQuery]string[] activityIds, AppointmentInput appointmentInput)
        {
            if (!activityIds.Any() || activityIds.Any(a => a == null))
            {
                return BadRequest();
            }

            if (appointmentInput.StartDate.Hour < 9 || appointmentInput.StartDate.Hour > 21)
            {
                return BadRequest("Please make an appointment for hours from 09:00 to 21:00!");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var appointmentDto = _mapper.Map<AppointmentDto>(appointmentInput);
            appointmentDto.ApplicationUserId = user.Id;

            try
            {
                await _appointmentService.AddAppointmentAsync(activityIds, appointmentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var appointmentView = _mapper.Map<AppointmentAdminViewModel>(appointmentDto);

            return CreatedAtAction("GetAppointmentById", new { id = appointmentView.Id }, appointmentView);
        }

        //// PUT: api/admin/appointments/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAppointment(string id, AppointmentInput appointmentInput)
        {
            if (appointmentInput.StartDate.Hour < 9 || appointmentInput.StartDate.Hour > 21)
            {
                return BadRequest("Please make an appointment for hours from 09:00 to 21:00!");
            }

            var appointmentDto = await _appointmentService.GetAppointmentByIdAsync(id);

            if (appointmentDto == null)
            {
                return NotFound();
            }

            _mapper.Map(appointmentInput, appointmentDto);

            try
            {
                await _appointmentService.UpdateAppointmentAsync(appointmentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        //// DELETE: api/admin/appointments/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAppointment(string id)
        {
            var appointmentDto = await _appointmentService.GetAppointmentByIdAsync(id);

            if (appointmentDto == null)
            {
                return NotFound();
            }

            try
            {
                await _appointmentService.DeleteAppointmentAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            return NoContent();
        }
    }
}
