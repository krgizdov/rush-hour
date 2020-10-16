namespace RushHour.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore.Internal;
    using RushHour.Data.Entities;
    using RushHour.Domain.Entities;
    using RushHour.Domain.Services;
    using RushHour.ViewModels.Appointment;

    [Route("api/appointments")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppointmentController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;

        public AppointmentController(UserManager<ApplicationUser> userManager,
            IAppointmentService appointmentService, IMapper mapper)
        {
            _userManager = userManager;
            _appointmentService = appointmentService;
            _mapper = mapper;
        }

        //// GET: api/appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentViewModel>>> GetPaginatedAppointments(int page = 1, int size = 10)
        {
            if (page <= 0 || size <= 0)
            {
                return BadRequest();
            }

            var username = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByNameAsync(username);

            var userAppointments = await _appointmentService.GetPaginatedAppointmentsAsync(user.Id, page, size);

            var appointmentViews = _mapper.Map<IEnumerable<AppointmentViewModel>>(userAppointments);

            return Ok(appointmentViews);
        }

        //// GET: api/appointments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentViewModel>> GetAppointmentById(string id)
        {
            var username = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByNameAsync(username);

            var appointmentDto = await _appointmentService.GetAppointmentByIdAsync(id, user.Id);

            if (appointmentDto == null)
            {
                return NotFound();
            }

            var appointmentView = _mapper.Map<AppointmentViewModel>(appointmentDto);

            return Ok(appointmentView);
        }

        //// POST: api/appointments
        [HttpPost]
        public async Task<ActionResult<AppointmentViewModel>> CreateAppointment([FromQuery]string[] activityIds, AppointmentInput appointmentInput)
        {
            if (!activityIds.Any() || activityIds.Any(a => a == null))
            {
                return BadRequest();
            }

            if (appointmentInput.StartDate.Hour < 9 || appointmentInput.StartDate.Hour > 21)
            {
                return BadRequest("Please make an appointment for hours from 09:00 to 21:00!");
            }

            var username = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByNameAsync(username);

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

            var appointmentView = _mapper.Map<AppointmentViewModel>(appointmentDto);

            return CreatedAtAction("GetAppointmentById", new { id = appointmentView.Id }, appointmentView);
        }

        //// PUT: api/appointments/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<AppointmentDto>> UpdateAppointment(string id, AppointmentInput appointmentInput)
        {
            if (appointmentInput.StartDate.Hour < 9 || appointmentInput.StartDate.Hour > 21)
            {
                return BadRequest("Please make an appointment for hours from 09:00 to 21:00!");
            }

            var username = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByNameAsync(username);

            var appointmentDto = await _appointmentService.GetAppointmentByIdAsync(id, user.Id);

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

        //// DELETE: api/appointments/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAppointment(string id)
        {

            var username = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByNameAsync(username);

            var appointmentDto = await _appointmentService.GetAppointmentByIdAsync(id, user.Id);

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
