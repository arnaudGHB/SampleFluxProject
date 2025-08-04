using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
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
using CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Command;
using CBS.CUSTOMER.DATA.Dto.CMoney;

namespace CBS.CUSTOMER.MEDIATR.CMoney.Configuration.AndriodVersion.Handler
{

    public class DeleteAndriodVersionByIdHandler : IRequestHandler<DeleteAndriodVersionByIdCommand, ServiceResponse<bool>>
    {
        private readonly IAndriodVersionConfigurationRepository _andriodVersionConfigurationRepository;
        private readonly ILogger<DeleteAndriodVersionByIdHandler> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IUnitOfWork<POSContext> _unitOfWork;

        public DeleteAndriodVersionByIdHandler(
            ILogger<DeleteAndriodVersionByIdHandler> logger,
            IAndriodVersionConfigurationRepository andriodVersionConfigurationRepository,
            UserInfoToken userInfoToken,
            IUnitOfWork<POSContext> unitOfWork
        )
        {
            _logger = logger;
            _andriodVersionConfigurationRepository = andriodVersionConfigurationRepository;
            _userInfoToken = userInfoToken;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteAndriodVersionByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Android version configuration by string ID.
                var andriodVersionConfiguration = await _andriodVersionConfigurationRepository.FindAsync(request.Id);

                if (andriodVersionConfiguration == null)
                {
                    string errorMessage = "Android Version Configuration not found";
                   
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Remove entity
                _andriodVersionConfigurationRepository.Delete(andriodVersionConfiguration);
                await _unitOfWork.SaveAsync();

                string successMessage = "Android Version Configuration deleted successfully";
           

                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
               
                return ServiceResponse<bool>.Return500();
            }
        }
    }


}
