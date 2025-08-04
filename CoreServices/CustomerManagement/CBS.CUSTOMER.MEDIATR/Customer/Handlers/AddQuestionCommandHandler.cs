
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;

using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Question.
    /// </summary>
    public class AddQuestionCommandHandler : IRequestHandler<AddQuestionCommand, ServiceResponse<CreateQuestion>>
    {
        private readonly IQuestionRepository _QuestionRepository; // Repository for accessing Question data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;

        /// <summary>
        /// Constructor for initializing the AddQuestionCommandHandler.
        /// </summary>
        /// <param name="QuestionRepository">Repository for Question data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddQuestionCommandHandler(
            IQuestionRepository QuestionRepository,
            IMapper mapper,
            ILogger<AddCustomerCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper
,
            UserInfoToken userInfoToken)
        {
            _QuestionRepository = QuestionRepository;
             _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _PathHelper = pathHelper;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddQuestionCommand to add a new Question.
        /// </summary>
        /// <param name="request">The AddQuestionCommand containing Question data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateQuestion>> Handle(AddQuestionCommand request, CancellationToken cancellationToken)
        {
            try
            {

                // Map the AddQuestionCommand to a Question entity
                var QuestionEntity = _mapper.Map<Question>(request);


                
                QuestionEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                QuestionEntity.QuestionId = BaseUtilities.GenerateUniqueNumber();
             
           
                // Add the new Customer entity to the repository
                _QuestionRepository.Add(QuestionEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, $"Internal Server Error occurred while saving new Question, Question : {request.SecretQuestion}", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);
                    return ServiceResponse<CreateQuestion>.Return500();
                }


                // Map the Question entity to CreateQuestion and return it with a success response
                var CreateCustomer = _mapper.Map<CreateQuestion>(QuestionEntity);
               
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), CreateCustomer, $"Creating New Customer   {request.SecretQuestion}  Successful  ", LogLevelInfo.Information.ToString(), 200, _UserInfoToken.Token);

                return ServiceResponse<CreateQuestion>.ReturnResultWith200(CreateCustomer);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Question: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), e.Message, $"Internal Server Error occurred while saving new Question, QuestionName : {request.SecretQuestion} ", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);

                return ServiceResponse<CreateQuestion>.Return500(e);
            }
        }

    }

}
