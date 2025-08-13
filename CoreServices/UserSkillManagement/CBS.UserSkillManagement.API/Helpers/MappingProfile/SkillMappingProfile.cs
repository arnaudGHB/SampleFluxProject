using System;
using AutoMapper;
using CBS.UserSkillManagement.Data.Dto;
using CBS.UserSkillManagement.Data;

namespace CBS.UserSkillManagement.API.Helpers.MappingProfile
{
    public class SkillMappingProfile : Profile
    {
        public SkillMappingProfile()
        {
            // Skill mappings
            CreateMap<Skill, SkillDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));

            CreateMap<CreateSkillDto, Skill>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

            CreateMap<UpdateSkillDto, Skill>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // UserSkill mappings
            CreateMap<UserSkill, UserSkillDto>()
                .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill.Name))
                .ForMember(dest => dest.SkillCategory, opt => opt.MapFrom(src => src.Skill.Category))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));

            CreateMap<AddUserSkillDto, UserSkill>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Skill, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<UpdateUserSkillDto, UserSkill>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.SkillId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Skill, opt => opt.Ignore());
        }
    }
}
