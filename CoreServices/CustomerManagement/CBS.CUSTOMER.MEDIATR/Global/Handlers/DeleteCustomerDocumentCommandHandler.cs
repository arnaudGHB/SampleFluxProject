using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY.DocumentRepo;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to delete a Customer based on DeleteCustomerDocumentCommand.
    /// </summary>
    public class DeleteCustomerDocumentCommandHandler : IRequestHandler<DeleteCustomerDocumentCommand, ServiceResponse<bool>>
    {
        private readonly IDocumentRepository _CustomerDocumentRepository; // Repository for accessing Customer Document data.
        private readonly ILogger<DeleteCustomerDocumentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteCustomerDocumentCommandHandler.
        /// </summary>
        /// <param name="CustomerDocumentRepository">Repository for CustomerDocument data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteCustomerDocumentCommandHandler(
            IDocumentRepository CustomerDocumentRepository, IMapper mapper,
            ILogger<DeleteCustomerDocumentCommandHandler> logger

, IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _CustomerDocumentRepository = CustomerDocumentRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteCustomerDocumentCommand to delete a CustomerDocument.
        /// </summary>
        /// <param name="request">The DeleteCustomerDocumentCommand containing Customer ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteCustomerDocumentCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the CustomerDocument entity with the specified ID exists
                var existingCustomerDocument = await _CustomerDocumentRepository.FindAsync(request.Id);
                if (existingCustomerDocument == null)
                {
                    errorMessage = $"Customer with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingCustomerDocument.IsDeleted = true;
                existingCustomerDocument.DeletedBy =_UserInfoToken.Id;
                _CustomerDocumentRepository.Update(existingCustomerDocument);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting CustomerDocument: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
