using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CBS.UserSkillManagement.API;
using CBS.UserSkillManagement.Data.Dto;
using CBS.UserSkillManagement.Data;
using CBS.UserSkillManagement.Common;
using CBS.UserSkillManagement.Repository;
using CBS.UserSkillManagement.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CBS.UserSkillManagement.API
{
    /// <summary>
    /// Gère toutes les opérations relatives aux compétences (skills) du système.
    /// Ce contrôleur expose les endpoints pour créer, lire, mettre à jour et supprimer des compétences.
    /// Les opérations d'administration (création, mise à jour, suppression) sont restreintes aux utilisateurs avec le rôle Admin.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : BaseController
    {
        private readonly ILogger<SkillsController> _logger;
        private readonly ISkillRepository _skillRepository;
        private readonly IMapper _mapper;

        public SkillsController(
            ILogger<SkillsController> logger,
            ISkillRepository skillRepository,
            IMapper mapper)
        {
            _logger = logger;
            _skillRepository = skillRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _skillRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<SkillDto>>(skills);
            return HandleResult(ServiceResponse<IEnumerable<SkillDto>>.SuccessResponse(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSkillById(int id)
        {
            var skill = await _skillRepository.GetByIdAsync(id);
            if (skill == null)
            {
                return NotFound(ServiceResponse<SkillDto>.ErrorResponse("Skill not found"));
            }

            var result = _mapper.Map<SkillDto>(skill);
            return HandleResult(ServiceResponse<SkillDto>.SuccessResponse(result));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSkill([FromBody] CreateSkillDto createSkillDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ServiceResponse<SkillDto>.ErrorResponse("Invalid model state", errors));
            }

            // Check if skill with same name and category already exists
            if (await _skillRepository.SkillExistsAsync(createSkillDto.Name, createSkillDto.Category))
            {
                return Conflict(ServiceResponse<SkillDto>.ErrorResponse("A skill with this name and category already exists"));
            }

            var skill = _mapper.Map<Skill>(createSkillDto);
            await _skillRepository.AddAsync(skill);
            await _skillRepository.SaveChangesAsync();

            var result = _mapper.Map<SkillDto>(skill);
            return CreatedAtAction(nameof(GetSkillById), new { id = skill.Id },
                ServiceResponse<SkillDto>.SuccessResponse(result, "Skill created successfully"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSkill(int id, [FromBody] UpdateSkillDto updateSkillDto)
        {
            if (id != updateSkillDto.Id)
            {
                return BadRequest(ServiceResponse<SkillDto>.ErrorResponse("ID mismatch"));
            }

            var existingSkill = await _skillRepository.GetByIdAsync(id);
            if (existingSkill == null)
            {
                return NotFound(ServiceResponse<SkillDto>.ErrorResponse("Skill not found"));
            }

            _mapper.Map(updateSkillDto, existingSkill);
            _skillRepository.Update(existingSkill);
            await _skillRepository.SaveChangesAsync();

            var result = _mapper.Map<SkillDto>(existingSkill);
            return HandleResult(ServiceResponse<SkillDto>.SuccessResponse(result, "Skill updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var skill = await _skillRepository.GetSkillWithUserSkillsAsync(id);
            if (skill == null)
            {
                return NotFound(ServiceResponse<object>.ErrorResponse("Skill not found"));
            }

            if (skill.UserSkills?.Count > 0)
            {
                return BadRequest(ServiceResponse<object>.ErrorResponse("Cannot delete skill as it is currently assigned to users"));
            }

            _skillRepository.Remove(skill);
            await _skillRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
