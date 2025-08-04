using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Dto.Customers;
namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the retrieval of all Questions based on the GetAllQuestionQuery.
    /// </summary>
    public class GetAllQuestionsQueryHandler : IRequestHandler<GetAllQuestionsQuery, ServiceResponse<List<GetQuestion>>>
    {
        private readonly IQuestionRepository _QuestionRepository; // Repository for accessing Questions data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllQuestionsQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllQuestionQueryHandler.
        /// </summary>
        /// <param name="QuestionRepository">Repository for Questions data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllQuestionsQueryHandler(
            IQuestionRepository QuestionRepository,
       
            IMapper mapper, ILogger<GetAllQuestionsQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _QuestionRepository = QuestionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllQuestionQuery to retrieve all Questions.
        /// </summary>
        /// <param name="request">The GetAllQuestionQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<GetQuestion>>> Handle(GetAllQuestionsQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Retrieve all Questions entities from the repository
                var entities = _mapper.Map< List<GetQuestion>> (await _QuestionRepository.All.Where(x=>x.IsDeleted==false).ToListAsync());


                return ServiceResponse<List<GetQuestion>>.ReturnResultWith200(entities);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Questions: {e.Message}");
                return ServiceResponse<List<GetQuestion>>.Return500(e, "Failed to get all Questions");
            }
        }
    }
}
