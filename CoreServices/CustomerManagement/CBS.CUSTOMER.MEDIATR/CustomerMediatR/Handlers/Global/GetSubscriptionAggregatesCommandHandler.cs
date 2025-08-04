
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.REPOSITORY;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Handlers.Global
{
    /// <summary>
    /// Handles the command to add a new Subscription Aggregates.
    /// </summary>
    public class GetSubscriptionAggregatesCommandHandler : IRequestHandler<GetAllSubscriptionAggregatesQuery, ServiceResponse<SubscriptionAggregatesDto>>
    {
        private readonly ICustomerCategoryRepository _CustomerCategoryRepository; // Repository for accessing CustomerCategory data.

        private readonly ILogger<GetSubscriptionAggregatesCommandHandler> _logger; // Logger for logging handler actions and errors.


        public GetSubscriptionAggregatesCommandHandler(
            ICustomerCategoryRepository CustomerCategoryRepository,
            ILogger<GetSubscriptionAggregatesCommandHandler> logger
        )
        {
            _CustomerCategoryRepository = CustomerCategoryRepository;
            _logger = logger;
        }

        private List<DropDownListDto> EnumToList<T>()
        {
            return  Enum.GetValues(typeof(T)).Cast<T>().Select(enumValue => new DropDownListDto() { Value = enumValue.ToString(), Text = enumValue.ToString()
            }).ToList();
        }

        /// <summary>
        /// Handles the AddCustomerDependentCommand to add a new CustomerDependent.
        /// </summary>
        /// <param name="request">The AddCustomerDependentCommand containing CustomerDependent data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SubscriptionAggregatesDto>> Handle(GetAllSubscriptionAggregatesQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var getAllCategory= _CustomerCategoryRepository.All.Where(x=>!x.IsDeleted).ToList();

                List<DropDownListDto> dropDownListDtos = new List<DropDownListDto>();
                
                if(getAllCategory.Any())
                {
                    getAllCategory.ForEach(x =>
                    {
                        var dropDownListDto = new DropDownListDto()
                        {
                            Text=x.CategoryName, Value=x.CustomerCategoryId
                        };

                        dropDownListDtos.Add(dropDownListDto);
                    });
                }



                var subscriptionAggregates = new SubscriptionAggregatesDto()
                {
                    LegalForms = EnumToList<LegalForm>(),
                    DocumentTypes = EnumToList<DocumentTypes>(),
                    WorkingStatuses = EnumToList<WorkingStatus>(),
                    ActiveStatuses = EnumToList<ActiveStatus>(),
                    MaritalStatuses = EnumToList<MaritalStatus>(),
                    BankingRelationships = EnumToList<BankingRelationship>(),
                    Relationships = EnumToList<RelationshipType>(),
                    FormalOrInformalSectors = EnumToList<FormalOrInformalSector>(),
                    Genders = EnumToList<GenderType>(),
                    MembershipApprovalStatuses = EnumToList<MembershipApprovalStatus>(),
                    CustomerCategories=dropDownListDtos


                };





                return ServiceResponse<SubscriptionAggregatesDto>.ReturnResultWith200(subscriptionAggregates);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving CustomerDependent: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<SubscriptionAggregatesDto>.Return500(e);
            }
        }
    }

}
