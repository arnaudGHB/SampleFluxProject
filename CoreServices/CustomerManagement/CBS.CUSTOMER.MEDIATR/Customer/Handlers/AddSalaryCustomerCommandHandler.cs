
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using Microsoft.AspNetCore.Hosting;
using CBS.CUSTOMER.MEDIATR.Customer.Command;
using NPOI.XSSF.UserModel;
using CBS.Customer.MEDIATR;
using NPOI.SS.Formula.Functions;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using Microsoft.IdentityModel.Tokens;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Customer.
    /// </summary>
    public class AddSalaryCustomerCommandHandler : IRequestHandler<AddNewSalaryCustomerCommand, ServiceResponse<CreateCustomer>>
    {
      
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddSalaryCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;
        private readonly SmsHelper _SmsHelper;
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for initializing the AddCustomerCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddSalaryCustomerCommandHandler(
            IMapper mapper,
            ILogger<AddSalaryCustomerCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper,
            UserInfoToken userInfoToken,
            IMediator mediator)
        {
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _PathHelper = pathHelper;
            _UserInfoToken = userInfoToken;
            _SmsHelper = new SmsHelper(_PathHelper);
            _mediator = mediator;
           
        }

        /// <summary>
        /// Handles the AddCustomerCommand to add a new Customer.
        /// </summary>
        /// <param name="request">The AddCustomerCommand containing Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateCustomer>> Handle(AddNewSalaryCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                    var CustomerCommand = ConvertCustomerFileMemberDataToCustomer(request);
                    var result = await _mediator.Send(CustomerCommand, cancellationToken);

                    if (result.StatusCode == 200)
                    {
                       return result;
                    
                    }
                    else
                    {

                        var NewCustomerCommand = verifyError(CustomerCommand, result.Message);
                       return await _mediator.Send(CustomerCommand, cancellationToken);


                    }
                                   
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Customer: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), e.Message, $"Internal Server Error occurred while saving new Customer ", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);

                return ServiceResponse<CreateCustomer>.Return500(e);
            }
        }


        public static bool ValidateCameroonNumber(string number)
        {
            // Remove any spaces or dashes from the number
            number = number.Replace(" ", "").Replace("-", "");

            // Regex pattern for Cameroonian mobile and fixed-line numbers with 237 prefix
            string mobilePattern = @"^2376\d{8}$";

            // Check if the number matches either pattern
            if (Regex.IsMatch(number, mobilePattern))
            {
                // "Cameroonian Mobile number")
                return true;
            }
            else
            {
                //"Invalid Cameroonian number"
                return false;
            }
        }

        AddCustomerCommand verifyError(AddCustomerCommand request, string error)
        {
            Random random = new Random();
            int ran = random.Next(0, 10);
            if (error == $"Customer With Phone {(BaseUtilities.Add237Prefix(request.Phone))} already exists.")
            {
                request.Phone = request.Phone + ran;
            }

            return request;
        }


        AddGroupCommand verifyError(AddGroupCommand request, string error)
        {
            Random random = new Random();
            int ran = random.Next(0, 10);
            if (error == $"Customer With Phone {(BaseUtilities.Add237Prefix(request.Phone))} already exists.")
            {
                request.Phone = request.Phone + ran;
            }
            if (error == $"Group {request.GroupName} already exists.")
            {
                request.GroupName = request.GroupName + " " + ran;
            }

            return request;
        }


        private AddCustomerCommand ConvertCustomerFileMemberDataToCustomer(AddNewSalaryCustomerCommand data)
        {
            /*  Random random = new Random();
               int ran=random.Next(0, 10);*/
            return new AddCustomerCommand
            {
                FirstName = data.MemberName,
                LastName = "N/A",
                Phone =  "S" + BaseUtilities.GenerateUniqueNumber(9),
                BankId = data.BankId,
                BranchId = data.BranchId,
                BranchCode = data.BranchCode,
                BankCode = data.BankCode,
                LegalForm = LegalForm.Physical_Person.ToString(),
                Address ="N/A",
                CountryId = "N/A",
                RegionId = "N/A",
                DivisionId = "N/A",
                SubDivisionId = "N/A",
                CompanyCreationDate = /*BaseUtilities.UtcToLocal(*/DateTime.Now/*)*/,
                ActiveStatus = ActiveStatus.Active.ToString(),
                DateOfBirth =DateTime.MinValue,
                Email = null,
                Gender =  GenderType.Other.ToString() ,
                Language = "English",
                GroupId = null,
                TownId = "N/A",
                FormalOrInformalSector = "N/A",
                TaxIdentificationNumber = "N/A",
                CustomerCategoryId = "092529097708186",
                IsNotDirectRequest = !data.IsFileData,
                IsMemberOfACompany = false,
                IsMemberOfAGroup = false,
                EconomicActivitiesId = "N/A",
                PlaceOfBirth = "N/A",
                Fax = "N/A",
                IDNumber = "N/A",
                BankingRelationship = BankingRelationship.Member.ToString(),
                IDNumberIssueAt = "N/A",
                IDNumberIssueDate = "N/A",
                Income = 0.0,
                OrganizationId = data.BankId,
                BankName = "N/A",
                POBox = "N/A",
                Occupation = "N/A",
                RegistrationNumber = data.CustomerId,
                ProfileType = ProfileType.Member.ToString(),
                PlaceOfCreation = "N/A",
                CustomerPackageId = "N/A",
                NumberOfKids = 0,
                VillageOfOrigin = "N/A",
                WorkingStatus = WorkingStatus.SelfEmploy.ToString(),
                IsFileData = data.IsFileData,
                CustomerCode = "",
                SecretAnswer = "N/A",
                SecretQuestion = "N/A",
                EmployerAddress = "N/A",
                EmployerName = "N/A",
                EmployerTelephone = "N/A",
                MaritalStatus = "N/A",
                Matricule=data.Matricule,
                SpouseName = "N/A",
                SpouseAddress = "N/A",
                SpouseContactNumber = "N/A",
                SpouseOccupation = "N/A",
                CustomerId =/* _UserInfoToken.BranchCode + "" + */data.CustomerId,
                CustomerType= data.CustomerType

            };
        }

     


    }

}
