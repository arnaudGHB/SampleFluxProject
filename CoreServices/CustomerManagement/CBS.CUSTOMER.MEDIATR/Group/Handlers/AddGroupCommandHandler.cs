
using MediatR;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.Customer.MEDIATR;
using CBS.CUSTOMER.REPOSITORY;
using CBS.Customer.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using BusinessServiceLayer.Objects.SmsObject;

namespace CBS.Group.MEDIATR.GroupMediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Group.
    /// </summary>
    public class AddGroupCommandHandler : IRequestHandler<AddGroupCommand, ServiceResponse<CreateGroup>>
    {
        private readonly IGroupRepository _GroupRepository; // Repository for accessing Group data.
        private readonly IGroupTypeRepository _GroupTypeRepository; // Repository for accessing Group Type data.
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing Group Customer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddGroupCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IMediator _mediator;
        private readonly UserInfoToken _userInfoToken;
        private readonly ICustomerSmsConfigurationRepository _CustomerSmsConfigurationRepository; // Repository for accessing CustomerSmsConfiguration data.
        private readonly PathHelper _PathHelper;
        private readonly SmsHelper _SmsHelper;

        /// <summary>
        /// Constructor for initializing the AddGroupCommandHandler.
        /// </summary>
        /// <param name="GroupRepository">Repository for Group data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddGroupCommandHandler(
            IGroupRepository GroupRepository,
            IMapper mapper,
            ILogger<AddGroupCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            ICustomerRepository customerRepository,
            IGroupCustomerRepository groupCustomerRepository,
            IGroupTypeRepository groupTypeRepository,
            IMediator mediator,
            UserInfoToken userInfoToken = null,
            ICustomerSmsConfigurationRepository customerSmsConfigurationRepository = null,
            PathHelper pathHelper = null,
            SmsHelper smsHelper = null)
        {
            _GroupRepository = GroupRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _CustomerRepository = customerRepository;
            _GroupCustomerRepository = groupCustomerRepository;
            _GroupTypeRepository = groupTypeRepository;
            _mediator = mediator;
            _userInfoToken = userInfoToken;
            _CustomerSmsConfigurationRepository = customerSmsConfigurationRepository;
            _PathHelper = pathHelper;
            _SmsHelper = smsHelper;
        }

        /// <summary>
        /// Handles the addition of a new group.
        /// </summary>
        /// <param name="request">The AddGroupCommand containing information about the group to be added.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A service response containing information about the created group.</returns>
        public async Task<ServiceResponse<CreateGroup>> Handle(AddGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Generate a unique GroupId
                string groupId = BaseUtilities.GenerateInsuranceUniqueNumber(8, "G-");

                // Check if a group with the same name already exists
                var existingGroup = await _GroupRepository.FindBy(c => c.GroupName == request.GroupName && !c.IsDeleted).FirstOrDefaultAsync();

                // If a group with the same name already exists, return a conflict response
                if (existingGroup != null)
                {
                    var errorMessage = $"Group {request.GroupName} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CreateGroup>.Return409(errorMessage);
                }

                // Check if the specified group type exists
                var existingGroupType = await _GroupTypeRepository.FindBy(s => s.GroupTypeId == request.GroupTypeId && !s.IsDeleted).FirstOrDefaultAsync();

                // If the group type does not exist, return a not found response
                if (existingGroupType == null)
                {
                    var errorMessage = $"Group type {request.GroupTypeId} does not exist.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CreateGroup>.Return404(errorMessage);
                }
                // Create a command to add a customer
                var addCustomerCommand = new AddCustomerCommand
                {
                    FirstName = request.GroupName,
                    Phone = request.Phone,
                    BankId = request.BankId,
                    BranchId = request.BranchId,
                    BranchCode = request.BranchCode,
                    BankCode = request.BankCode,
                    LegalForm = LegalForm.Moral_Person.ToString(),
                    Address = request.Address,
                    CountryId = request.CountryId,
                    RegionId = request.RegionId,
                    DivisionId = request.DivisionId,
                    SubDivisionId = request.SubDivisionId,
                    CompanyCreationDate = BaseUtilities.UtcNowToDoualaTime(),
                    ActiveStatus = ActiveStatus.Active.ToString(),
                    DateOfBirth =DateTime.MinValue, //Convert.ToDateTime(request.DateOfEstablishment),
                    Email = request.Email,
                    Gender = GenderType.Other.ToString(),
                    Language = "English",
                    GroupId = groupId,
                    TownId = request.TownId,
                    FormalOrInformalSector=request.FormalOrInformalSector,
                    TaxIdentificationNumber = request.TaxPayerNumber,
                    CustomerCategoryId = request.CustomerCategoryId,
                    IsNotDirectRequest = true,
                    IsMemberOfACompany = false,
                    IsMemberOfAGroup = true,
                    EconomicActivitiesId = request.EconomicActivitiesId,
                    PlaceOfBirth = request.Address,
                    Fax = request.Fax,
                    IDNumber = request.IDNumber,
                    BankingRelationship = BankingRelationship.Member.ToString(),
                    IDNumberIssueAt = request.IDNumberIssueAt,
                    IDNumberIssueDate = request.IDNumberIssueDate,
                    Income = request.Income,
                    OrganizationId = request.BankId,
                    BankName = request.BankName,
                    LastName = string.Empty,
                    POBox = request.POBox,
                    Occupation = request.Occupation,
                    RegistrationNumber = request.RegistrationNumber,
                    ProfileType = ProfileType.Member.ToString(),
                    PlaceOfCreation = "N/A",
                    CustomerPackageId = "N/A",
                    NumberOfKids = 0,
                    VillageOfOrigin = request.Address,
                    WorkingStatus = WorkingStatus.SelfEmploy.ToString(),
                    IsFileData=request.IsFileData,
                    CustomerCode=request.CustomerCode,
                    SecretAnswer = "N/A",
                    SecretQuestion = "N/A",
                    EmployerAddress = "N/A",
                    EmployerName = "N/A",
                    EmployerTelephone = "N/A",
                    MaritalStatus = "N/A",
                    SpouseName = "N/A",
                    SpouseAddress = "N/A",
                    SpouseContactNumber = "N/A",
                    SpouseOccupation = "N/A",
                    CustomerId=request.CustomerId,
                    CustomerType=request.CustomerType,
                };
                //MemberAccount
                // Add the customer
                var result = await _mediator.Send(addCustomerCommand, cancellationToken);

                // If adding the customer fails, log the error and return a 500 Internal Server Error response
                if (result.StatusCode != 200)
                {
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, result.Message, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<CreateGroup>.Return500(result.Message);
                }

                // Retrieve the added customer
                var customer = result.Data;

                // Set the group leader ID to the ID of the added customer
                request.GroupLeaderId = customer.CustomerId;

                // Map the AddGroupCommand to a Group entity
                var groupEntity = _mapper.Map<CUSTOMER.DATA.Entity.Group>(request);
                groupEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                groupEntity.GroupId = groupId; // Set the GroupId
                groupEntity.Active = true;
                // Add the new Group entity to the repository
                _GroupRepository.Add(groupEntity);

                AddGroupCustomerCommand groupCustomerCommand = new AddGroupCustomerCommand()
                {
                    CustomerIds = new List<string>() { customer.CustomerId },
                    GroupId = groupId,
                    commit = false
                };
                var resultGroupCustomer = await _mediator.Send(groupCustomerCommand);
                if (resultGroupCustomer.StatusCode!=200)
                {
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, result.Message, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<CreateGroup>.Return500(result.Message);
                }
                await _uow.SaveAsync();

                if (!request.IsFileData)
                {
                   
                    //await SendCustomerCreationSms(customer);

                }
                // Map the Group entity to CreateGroup and return it with a success response
                var createGroup = _mapper.Map<CreateGroup>(groupEntity);
                return ServiceResponse<CreateGroup>.ReturnResultWith200(createGroup);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Group: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateGroup>.Return500(e);
            }
        }

        private async Task SendCustomerCreationSms(CreateCustomer CustomerEntity)
        {

            if (_SmsHelper!=null)
            {
                CUSTOMER.DATA.Entity.CustomerSmsConfigurations getSmsTemplate = new();
                if (CustomerEntity != null && CustomerEntity.Language != null && CustomerEntity.Language.ToLower() == "english")
                {
                    getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("CUSTOMER_CREATION_NOTIFICATION_ENGLISH");
                }
                else
                {
                    getSmsTemplate = await _CustomerSmsConfigurationRepository.FindAsync("CUSTOMER_CREATION_NOTIFICATION_FRENCH");
                }

                if (getSmsTemplate != null && getSmsTemplate.SmsTemplate != null)
                {
                    var smsRequest = new SubSmsRequestDto()
                    {
                        Message = getSmsTemplate.SmsTemplate.Replace("{FirstName}", CustomerEntity.FirstName).Replace("{LastName}", $"{CustomerEntity.LastName}").Replace("{BankName}", $"{CustomerEntity.BankName}").Replace("{CustomerId}", $"{CustomerEntity.CustomerId}"),
                        Msisdn = CustomerEntity.Phone,
                        Token = _userInfoToken.Token
                    };
                    await _SmsHelper.SendSms(smsRequest);
                }
                else
                {
                    if (CustomerEntity != null && CustomerEntity.Language != null && CustomerEntity.Language.ToLower() == "english")
                    {
                        // English message
                        string msg = $"Dear {CustomerEntity.FirstName} {CustomerEntity.LastName}, Welcome to {CustomerEntity.BankName}! We are delighted to have you onboard. Your membership has been successfully created. Membership code: {CustomerEntity.CustomerId}.\nYour For any assistance or inquiries, feel free to visit our nearest branch or contact our customer support at 8080.\nThank you for choosing {CustomerEntity.BankName}.";

                        var smsRequest = new SubSmsRequestDto()
                        {
                            Message = msg,
                            Msisdn = CustomerEntity.Phone
                        };
                        await _SmsHelper.SendSms(smsRequest);
                    }
                    else
                    {
                        // French message
                        string msg = $"Cher {CustomerEntity.FirstName} {CustomerEntity.LastName}, Bienvenue a {CustomerEntity.BankName}! Nous sommes ravis de vous accueillir à bord. Votre adhésion a ete cree avec succes. Code d adhesion : {CustomerEntity.CustomerId}.\nPour toute assistance ou question, n hesitez pas à vous rendre dans notre succursale la plus proche ou a contacter notre service clientele au 8080.\nMerci d avoir choisi {CustomerEntity.BankName}.";

                        var smsRequest = new SubSmsRequestDto()
                        {
                            Message = msg,
                            Msisdn = CustomerEntity.Phone
                        };
                        await _SmsHelper.SendSms(smsRequest);
                    }
                }
            }
            
        }

    }

}
