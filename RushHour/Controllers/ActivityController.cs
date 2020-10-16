namespace RushHour.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RushHour.Domain;
    using RushHour.Domain.Entities;
    using RushHour.Domain.Services;
    using RushHour.ViewModels.Activity;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Route("api/activities")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = GlobalConstants.UserRoleName)]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly IMapper _mapper;
        private readonly IAppointmentService _appointmentService;

        public ActivityController(IActivityService activityService, IMapper mapper, IAppointmentService appointmentService)
        {
            _activityService = activityService;
            _mapper = mapper;
            _appointmentService = appointmentService;
        }

        //// GET: api/activities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityViewModel>>> GetPaginatedActivities(int page = 1, int size = 10)
        {
            if (page <= 0 || size <= 0)
            {
                return BadRequest();
            }

            var activites = await _activityService.GetPaginatedActivitiesAsync(page, size);

            var activityViews = _mapper.Map<IEnumerable<ActivityViewModel>>(activites);

            return Ok(activityViews);
        }

        //// GET: api/activities/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityViewModel>> GetActivityById(string id)
        {
            var activityDto = await _activityService.GetActivityByIdAsync(id);

            if (activityDto == null)
            {
                return NotFound();
            }

            var activityView = _mapper.Map<ActivityViewModel>(activityDto);

            return Ok(activityView);
        }

        //// POST: api/activities
        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<ActionResult<ActivityViewModel>> CreateActivity(ActivityInput activityInput)
        {
            var activityDto = _mapper.Map<ActivityDto>(activityInput);

            try
            {
                await _activityService.AddActivityAsync(activityDto);
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }
            
            var activityView = _mapper.Map<ActivityViewModel>(activityDto);

            return CreatedAtAction("GetActivityById", new { id = activityView.Id }, activityView);
        }

        //// PUT: api/activities/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<ActionResult> UpdateActivity(string id, ActivityUpdate activityUpdate)
        {
            var activityDto = await _activityService.GetActivityByIdAsync(id);

            if (activityDto == null)
            {
                return NotFound();
            }

            try
            {
                await _appointmentService.ConfigureAppointmentsUponActivityChangeAsync(activityDto);

                _mapper.Map(activityUpdate, activityDto);

                await _activityService.UpdateActivityAsync(activityDto);
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }

            return NoContent();
        }

        //// DELETE: api/activities/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<ActionResult> DeleteActivity(string id)
        {
            var activityDto = await _activityService.GetActivityByIdAsync(id);

            if (activityDto == null)
            {
                return NotFound();
            }

            try
            {
                await _appointmentService.ConfigureAppointmentsUponActivityChangeAsync(activityDto);

                await _activityService.DeleteActivityAsync(id);
            }
            catch (Exception ex)
            {
                BadRequest(ex.Message);
            }

            return NoContent();
        }
    }
}
