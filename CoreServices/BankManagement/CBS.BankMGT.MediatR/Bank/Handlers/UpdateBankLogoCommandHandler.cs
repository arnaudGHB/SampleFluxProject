using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;

using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{
 
    public class UpdateBankLogoCommandHandler : IRequestHandler<UpdateBankLogoCommand, ServiceResponse<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IBankRepository _bankRepository;
        private readonly ILogger<UpdateBankLogoCommandHandler> _logger;
        public readonly PathHelper _pathHelper;
        public UpdateBankLogoCommandHandler(
            IMapper mapper,
            IUnitOfWork<POSContext> uow,


            ILogger<UpdateBankLogoCommandHandler> logger,
            PathHelper pathHelper
,
            IBankRepository bankRepository = null)
        {
            _mapper = mapper;
            _uow = uow;
            _logger = logger;
            _pathHelper = pathHelper;
            _bankRepository = bankRepository;
        }
        public async Task<ServiceResponse<bool>> Handle(UpdateBankLogoCommand request, CancellationToken cancellationToken)
        {
            try
            {
               
                var bank = await _bankRepository.FindAsync(request.Id);
                bank.LogoUrl = request.UrlPath;
                // Update Bank
                _bankRepository.Update(bank);
                await _uow.SaveAsync();

                return ServiceResponse<bool>.ReturnResultWith200(true, $"{bank.Name} logo was updated successfully.");
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Bank logo: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e, errorMessage);
            }
        }


    }

}
