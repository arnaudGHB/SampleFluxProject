using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CBS.UserSkillManagement.API.Helpers.MappingProfile;
using CBS.UserSkillManagement.Data.Dto;
using CBS.UserSkillManagement.Data;
using CBS.UserSkillManagement.Common;
using CBS.UserSkillManagement.Repository;
using CBS.UserSkillManagement.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CBS.UserSkillManagement.API
{
    /// <summary>
    /// Gère toutes les opérations relatives aux compétences des utilisateurs (user skills).
    /// Ce contrôleur expose les endpoints pour associer, consulter, mettre à jour et supprimer des compétences pour les utilisateurs.
    /// Les utilisateurs peuvent gérer leurs propres compétences, tandis que les rôles Admin et Manager peuvent gérer les compétences de tous les utilisateurs.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserSkillsController : BaseController
    {
        private readonly ILogger<UserSkillsController> _logger;
        private readonly IUserSkillRepository _userSkillRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly IMapper _mapper;

        public UserSkillsController(
            ILogger<UserSkillsController> logger,
            IUserSkillRepository userSkillRepository,
            ISkillRepository skillRepository,
            IMapper mapper)
        {
            _logger = logger;
            _userSkillRepository = userSkillRepository;
            _skillRepository = skillRepository;
            _mapper = mapper;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMySkills()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ServiceResponse<IEnumerable<UserSkillDto>>.ErrorResponse("User not authenticated"));
            }

            var userSkills = await _userSkillRepository.GetUserSkillsAsync(userId);
            var result = _mapper.Map<IEnumerable<UserSkillDto>>(userSkills);
            return HandleResult(ServiceResponse<IEnumerable<UserSkillDto>>.SuccessResponse(result));
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetUserSkills(string userId)
        {
            var userSkills = await _userSkillRepository.GetUserSkillsAsync(userId);
            var result = _mapper.Map<IEnumerable<UserSkillDto>>(userSkills);
            return HandleResult(ServiceResponse<IEnumerable<UserSkillDto>>.SuccessResponse(result));
        }

        [HttpPost("me")]
        public async Task<IActionResult> AddSkillToMe([FromBody] AddUserSkillDto addUserSkillDto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ServiceResponse<UserSkillDto>.ErrorResponse("User not authenticated"));
            }

            return await AddSkillToUser(userId, addUserSkillDto);
        }

        [HttpPost("user/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddSkillToUser(string userId, [FromBody] AddUserSkillDto addUserSkillDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ServiceResponse<UserSkillDto>.ErrorResponse("Invalid model state", errors));
            }

            // Check if skill exists
            var skill = await _skillRepository.GetByIdAsync(addUserSkillDto.SkillId);
            if (skill == null)
            {
                return NotFound(ServiceResponse<UserSkillDto>.ErrorResponse("Skill not found"));
            }

            // Check if user already has this skill
            if (await _userSkillRepository.UserHasSkillAsync(userId, addUserSkillDto.SkillId))
            {
                return Conflict(ServiceResponse<UserSkillDto>.ErrorResponse("User already has this skill"));
            }

            var userSkill = _mapper.Map<UserSkill>(addUserSkillDto);
            userSkill.UserId = userId;

            await _userSkillRepository.AddAsync(userSkill);
            await _userSkillRepository.SaveChangesAsync();

            // Reload the skill to get the navigation property
            var result = await _userSkillRepository.GetUserSkillAsync(userId, addUserSkillDto.SkillId);
            var mappedResult = _mapper.Map<UserSkillDto>(result);

            return CreatedAtAction(nameof(GetUserSkill), new { userId, skillId = addUserSkillDto.SkillId },
                ServiceResponse<UserSkillDto>.SuccessResponse(mappedResult, "Skill added to user successfully"));
        }

        [HttpGet("me/{skillId}")]
        public async Task<IActionResult> GetMySkill(int skillId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ServiceResponse<UserSkillDto>.ErrorResponse("User not authenticated"));
            }

            return await GetUserSkill(userId, skillId);
        }

        [HttpGet("user/{userId}/skill/{skillId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetUserSkill(string userId, int skillId)
        {
            var userSkill = await _userSkillRepository.GetUserSkillAsync(userId, skillId);
            if (userSkill == null)
            {
                return NotFound(ServiceResponse<UserSkillDto>.ErrorResponse("User skill not found"));
            }

            var result = _mapper.Map<UserSkillDto>(userSkill);
            return HandleResult(ServiceResponse<UserSkillDto>.SuccessResponse(result));
        }

        [HttpPut("me/{skillId}")]
        public async Task<IActionResult> UpdateMySkill(int skillId, [FromBody] UpdateUserSkillDto updateUserSkillDto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ServiceResponse<UserSkillDto>.ErrorResponse("User not authenticated"));
            }

            return await UpdateUserSkill(userId, skillId, updateUserSkillDto);
        }

        [HttpPut("user/{userId}/skill/{skillId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateUserSkill(string userId, int skillId, [FromBody] UpdateUserSkillDto updateUserSkillDto)
        {
            if (updateUserSkillDto == null || updateUserSkillDto.UserSkillId <= 0)
            {
                return BadRequest(ServiceResponse<UserSkillDto>.ErrorResponse("Invalid request data"));
            }

            var userSkill = await _userSkillRepository.GetUserSkillAsync(userId, skillId);
            if (userSkill == null)
            {
                return NotFound(ServiceResponse<UserSkillDto>.ErrorResponse("User skill not found"));
            }

            _mapper.Map(updateUserSkillDto, userSkill);
            _userSkillRepository.Update(userSkill);
            await _userSkillRepository.SaveChangesAsync();

            var updatedSkill = await _userSkillRepository.GetUserSkillAsync(userId, skillId);
            var result = _mapper.Map<UserSkillDto>(updatedSkill);

            return HandleResult(ServiceResponse<UserSkillDto>.SuccessResponse(result, "User skill updated successfully"));
        }

        [HttpDelete("me/{skillId}")]
        public async Task<IActionResult> RemoveMySkill(int skillId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ServiceResponse<object>.ErrorResponse("User not authenticated"));
            }

            return await RemoveUserSkill(userId, skillId);
        }

        [HttpDelete("user/{userId}/skill/{skillId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveUserSkill(string userId, int skillId)
        {
            var userSkill = await _userSkillRepository.GetUserSkillAsync(userId, skillId);
            if (userSkill == null)
            {
                return NotFound(ServiceResponse<object>.ErrorResponse("User skill not found"));
            }

            _userSkillRepository.Remove(userSkill);
            await _userSkillRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
