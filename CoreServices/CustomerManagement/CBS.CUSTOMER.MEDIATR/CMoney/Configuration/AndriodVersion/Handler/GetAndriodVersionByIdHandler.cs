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

namespace CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Handler
{


    public class GetAndriodVersionByIdHandler : IRequestHandler<GetAndriodVersionByIdQuery, ServiceResponse<AndriodVersionConfigurationDto>>
    {
        private readonly IAndriodVersionConfigurationRepository _andriodVersionConfigurationRepository;
        private readonly ILogger<GetAndriodVersionByIdHandler> _logger;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;

        public GetAndriodVersionByIdHandler(
            ILogger<GetAndriodVersionByIdHandler> logger,
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

        public async Task<ServiceResponse<AndriodVersionConfigurationDto>> Handle(GetAndriodVersionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Android version configuration by ID.
                AndriodVersionConfiguration andriodVersionConfiguration = await _andriodVersionConfigurationRepository
                    .FindAsync(request.Id);

                if (andriodVersionConfiguration == null)
                {
                    string errorMessage = "Android Version Configuration not found";
                    return ServiceResponse<AndriodVersionConfigurationDto>.Return404(errorMessage);
                }

                // Use AutoMapper to map entity to DTO
                var dto = _mapper.Map<AndriodVersionConfigurationDto>(andriodVersionConfiguration);

                return ServiceResponse<AndriodVersionConfigurationDto>.ReturnResultWith200(dto);
            }
            catch (Exception ex)
            {
                return ServiceResponse<AndriodVersionConfigurationDto>.Return500();
            }
        }
    }


}
