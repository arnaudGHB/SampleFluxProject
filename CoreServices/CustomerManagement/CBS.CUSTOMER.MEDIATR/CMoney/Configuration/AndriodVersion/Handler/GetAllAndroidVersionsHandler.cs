using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands.Global;
using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Configuration;
using CBS.CUSTOMER.DATA.Entity.Document;
using CBS.CUSTOMER.REPOSITORY.DocumentRepo;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Entity;
using Microsoft.IdentityModel.Tokens;
using CBS.CUSTOMER.DATA.Entity.Groups;
using CBS.CUSTOMER.REPOSITORY.ConfigRepo;
using CBS.CUSTOMER.DATA.Entity.CMoney;
using CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Queries;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using Microsoft.EntityFrameworkCore;

namespace CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Handler
{


    public class GetAllAndroidVersionsHandler : IRequestHandler<GetAllAndroidVersionsQuery, ServiceResponse<List<AndriodVersionConfigurationDto>>>
    {
        private readonly IAndriodVersionConfigurationRepository _andriodVersionConfigurationRepository;
        private readonly ILogger<GetAllAndroidVersionsHandler> _logger;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;

        public GetAllAndroidVersionsHandler(
            ILogger<GetAllAndroidVersionsHandler> logger,
            IAndriodVersionConfigurationRepository andriodVersionConfigurationRepository,
            IMapper mapper,
            UserInfoToken userInfoToken
        )
        {
            _logger = logger;
            _andriodVersionConfigurationRepository = andriodVersionConfigurationRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
        }

        public async Task<ServiceResponse<List<AndriodVersionConfigurationDto>>> Handle(GetAllAndroidVersionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var androidVersions = await _andriodVersionConfigurationRepository.All.ToListAsync();
                if (androidVersions == null || !androidVersions.Any())
                {
                    return ServiceResponse<List<AndriodVersionConfigurationDto>>.Return404("No Android versions found.");
                }

                var dtoList = _mapper.Map<List<AndriodVersionConfigurationDto>>(androidVersions);
                return ServiceResponse<List<AndriodVersionConfigurationDto>>.ReturnResultWith200(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all Android versions");
                return ServiceResponse<List<AndriodVersionConfigurationDto>>.Return500();
            }
        }
    }


}
