using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to delete a SavingProduct based on DeleteSavingProductCommand.
    /// </summary>
    public class DeleteSavingProductCommandHandler : IRequestHandler<DeleteSavingProductCommand, ServiceResponse<bool>>
    {
        private readonly ISavingProductRepository _SavingProductRepository; // Repository for accessing SavingProduct data.
        private readonly ILogger<DeleteSavingProductCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IAccountRepository _AccountRepository;
        /// <summary>
        /// Constructor for initializing the DeleteSavingProductCommandHandler.
        /// </summary>
        /// <param name="SavingProductRepository">Repository for SavingProduct data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteSavingProductCommandHandler(
            ISavingProductRepository SavingProductRepository, IMapper mapper,
            ILogger<DeleteSavingProductCommandHandler> logger
, IUnitOfWork<TransactionContext> uow, IAccountRepository accountRepository = null)
        {
            _SavingProductRepository = SavingProductRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _AccountRepository = accountRepository;
        }

        /// <summary>
        /// Handles the DeleteSavingProductCommand to delete a SavingProduct.
        /// </summary>
        /// <param name="request">The DeleteSavingProductCommand containing SavingProduct ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteSavingProductCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the SavingProduct entity with the specified ID exists
                var existingSavingProduct = await _SavingProductRepository.FindAsync(request.Id);
                if (existingSavingProduct == null)
                {
                    errorMessage = $"SavingProduct with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                var checkIfInUse = await _AccountRepository.FindBy(x=>x.ProductId==existingSavingProduct.Id).ToListAsync();
                if (checkIfInUse.Any())
                {
                    errorMessage = $"{existingSavingProduct.Name} is in used as a service already. You cannot delete it.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }
                existingSavingProduct.IsDeleted = true;
                _SavingProductRepository.Update(existingSavingProduct);
                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting SavingProduct: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
