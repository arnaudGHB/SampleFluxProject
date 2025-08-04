using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.DATA.Dto;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to delete a Customer based on DeleteQuestionCommand.
    /// </summary>
    public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, ServiceResponse<bool>>
    {
        private readonly IQuestionRepository _QuestionRepository; // Repository for accessing Customer data.
        private readonly ILogger<DeleteQuestionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteQuestionCommandHandler.
        /// </summary>
        /// <param name="QuestionRepository">Repository for Question data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteQuestionCommandHandler(
            IQuestionRepository QuestionRepository, IMapper mapper,
            ILogger<DeleteQuestionCommandHandler> logger

, IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _QuestionRepository = QuestionRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteQuestionCommand to delete a Question.
        /// </summary>
        /// <param name="request">The DeleteQuestionCommand containing Customer ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Question entity with the specified ID exists
                var existingQuestion = await _QuestionRepository.FindAsync(request.QuestionId);
                if (existingQuestion == null)
                {
                    errorMessage = $"Customer with ID {request.QuestionId} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingQuestion.IsDeleted = true;
                existingQuestion.DeletedBy =_UserInfoToken.Id;
                _QuestionRepository.Update(existingQuestion);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Question: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
