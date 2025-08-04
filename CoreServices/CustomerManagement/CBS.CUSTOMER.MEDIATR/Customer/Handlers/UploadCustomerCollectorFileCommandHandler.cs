
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
    public class UploadCustomerCollectorFileCommandHandler : IRequestHandler<UploadCustomerCollectorFileCommand, ServiceResponse<List<CustomerFileMemberData>>>
    {
        private readonly IHostingEnvironment _webHostEnvironment;
        private readonly IUploadedCustomerWithErrorRepository _UploadedCustomerWithErrorRepository; // Repository for accessing Uploaded Customers with Error data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<UploadCustomerCollectorFileCommandHandler> _logger; // Logger for logging handler actions and errors.
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
        public UploadCustomerCollectorFileCommandHandler(
            IMapper mapper,
            ILogger<UploadCustomerCollectorFileCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper,
            UserInfoToken userInfoToken,
            IMediator mediator,
            IHostingEnvironment webHostEnvironment,
            IUploadedCustomerWithErrorRepository uploadedCustomerWithErrorRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _PathHelper = pathHelper;
            _UserInfoToken = userInfoToken;
            _SmsHelper = new SmsHelper(_PathHelper);
            _mediator = mediator;
            _webHostEnvironment = webHostEnvironment;
            _UploadedCustomerWithErrorRepository = uploadedCustomerWithErrorRepository;
        }

        /// <summary>
        /// Handles the AddCustomerCommand to add a new Customer.
        /// </summary>
        /// <param name="request">The AddCustomerCommand containing Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CustomerFileMemberData>>> Handle(UploadCustomerCollectorFileCommand request, CancellationToken cancellationToken)
        {
            try
            {


                List<CustomerFileMemberData> fileMemberDatas = new List<CustomerFileMemberData>();
                var filePath = await FileData.SaveFileAsync(request.formFile, _PathHelper.UploadPath, _webHostEnvironment);
                // Get the file extension
                string fileExtension = Path.GetExtension(filePath)?.ToLower();
                // Check if the file extension is either XLSX or XLS
                if (fileExtension == ".xlsx" || fileExtension == ".xls")
                {

                    XSSFWorkbook xssfwb;
                    using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        xssfwb = new XSSFWorkbook(file);
                    }

                    var sheet = xssfwb.GetSheetAt(0); // Change this to the worksheet you want to import.
                    var rows = sheet.GetRowEnumerator();
                    rows.MoveNext();
                    while (rows.MoveNext())
                    {
                    
                        var row = (XSSFRow)rows.Current;
                        var data = new CustomerFileMemberData()
                        {
                            MemberNumber = row.GetCell(0)?.ToString(),
                            MemberName = row.GetCell(1)?.ToString(),
                            MemberSurName = row.GetCell(2)?.ToString(),
                            Type = null,
                            CreationDate = row.GetCell(3)?.ToString(),
                            DateOfBirth = row.GetCell(3)?.ToString(),
                            PlaceOfBirth = null,
                            Genre = null,
                            Quater = null,
                            Town = null,
                            Telephone = null,
                            Cni = null,
                            CniDeliveranceDate = null,
                            CniLocation = null,
                            BranchCode = _UserInfoToken.BranchCode,
                        };
                        fileMemberDatas.Add(data);
                    }

                    if (fileMemberDatas.Count > 0)
                    {
                        foreach (var data in fileMemberDatas)
                        {


                            data.MemberNumber = Convert.ToInt32(data.MemberNumber).ToString("D7");
                            var CustomerCommand = ConvertCustomerFileMemberDataToCustomer(data);
                            var result = await _mediator.Send(CustomerCommand, cancellationToken);

                            if (result.StatusCode == 200)
                            {
                                var verifyCameroonNumber = ValidateCameroonNumber(BaseUtilities.Add237Prefix(CustomerCommand.Phone));
                                if (!verifyCameroonNumber)
                                {
                                    WriteToDatabase(data, BaseUtilities.Add237Prefix(CustomerCommand.Phone), "Modified Telephone From " + data.Telephone + " To " + CustomerCommand.Phone, true);
                                }
                            }
                            else
                            {

                                var NewCustomerCommand = verifyError(CustomerCommand, result.Message);
                                var result2 = await _mediator.Send(CustomerCommand, cancellationToken);
                                if (result2.StatusCode == 200)
                                {
                                    var verifyCameroonNumber = ValidateCameroonNumber(BaseUtilities.Add237Prefix(NewCustomerCommand.Phone));
                                    if (!verifyCameroonNumber)
                                    {
                                        WriteToDatabase(data, BaseUtilities.Add237Prefix(NewCustomerCommand.Phone), "Had Error : " + result.Message + "  , Modified Telephone From " + data.Telephone + " To " + NewCustomerCommand.Phone, true);
                                    }
                                }
                                else
                                {
                                    result.Message = result2.Message != null ? result2.Message + ": Not Resolved" : "Not Resolved";
                                    WriteToDatabase(data, BaseUtilities.Add237Prefix(NewCustomerCommand.Phone), result2.Message, !result.Message.Contains("Not Resolved"));
                                }

                            }



                        }

                    }
                    await _uow.SaveAsync();
                    return ServiceResponse<List<CustomerFileMemberData>>.ReturnResultWith200(fileMemberDatas);

                }
                else
                {

                    return ServiceResponse<List<CustomerFileMemberData>>.Return500("The file is not an XLS or XLSX file.");

                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Customer: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), e.Message, $"Internal Server Error occurred while saving new Customer ", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);

                return ServiceResponse<List<CustomerFileMemberData>>.Return500(e);
            }
        }

        private void WriteToFile(CustomerFileMemberData data, List<string> Errors)
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, _PathHelper.UploadFileSubPath, "File" + _UserInfoToken.BranchCode + ".csv");
            StreamWriter sw = new StreamWriter(path, true);
            var csv = new StringBuilder();

            var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", data.MemberNumber, data.MemberName, data.MemberSurName, data.Cni, data.CniDeliveranceDate, data.CniLocation, data.Telephone, data.CreationDate, data.Type, data.Genre, data.DateOfBirth, data.BranchCode, Errors != null ? Errors[0] : null);
            csv.AppendLine(newLine);
            sw.WriteLine(newLine);
            sw.Close();
        }

        private async void WriteToDatabase(CustomerFileMemberData data, string telephone, string Errors, bool resolved)
        {
            var uploadedCustomerWithError = new UploadedCustomerWithError()
            {
                Id = BaseUtilities.GenerateUniqueNumber(9),
                Genre = data.Genre,
                InitialError = Errors != null ? Errors : null,
                CreationDate = data.CreationDate,
                BankCode = _UserInfoToken.BankCode,
                BranchCode = _UserInfoToken.BranchCode,
                Cni = data.Cni,
                CniDeliveranceDate = data.CniDeliveranceDate,
                MemberNumber = data.MemberNumber,
                MemberName = data.MemberName,
                MemberSurName = data.MemberSurName,
                CniLocation = data.CniLocation,
                Telephone = telephone,
                Type = data.Type,
                resolved = resolved,
                Quater = data.Quater,
                DateOfBirth = data.DateOfBirth,
                Town = data.Town,
                PlaceOfBirth = data.PlaceOfBirth
            };

            _UploadedCustomerWithErrorRepository.Add(uploadedCustomerWithError);

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


        private AddCustomerCommand ConvertCustomerFileMemberDataToCustomer(CustomerFileMemberData data)
        {
            /*  Random random = new Random();
               int ran=random.Next(0, 10);*/
            return new AddCustomerCommand
            {
                FirstName = data.MemberName,
                LastName =data.MemberSurName,
                Phone =BaseUtilities.GenerateUniqueNumber(8)+"D",
                BankId = _UserInfoToken.BankID,
                BranchId = _UserInfoToken.BranchID,
                BranchCode = _UserInfoToken.BranchCode,
                BankCode = _UserInfoToken.BankCode,
                LegalForm = LegalForm.Physical_Person.ToString(),
                Address = data.CniLocation == null ? "N/A" : data.CniLocation,
                CountryId = "N/A",
                RegionId = "N/A",
                DivisionId = "N/A",
                SubDivisionId = "N/A",
                CompanyCreationDate = BaseUtilities.UtcToLocal(DateTime.Now),
                ActiveStatus = ActiveStatus.Active.ToString(),
                DateOfBirth = Convert.ToDateTime(data.DateOfBirth),
                Email = null,
                Gender = data.Genre == "M" ? GenderType.Male.ToString() : GenderType.Female.ToString(),
                Language = "English",
                GroupId = null,
                TownId = "N/A",
                FormalOrInformalSector = "N/A",
                TaxIdentificationNumber = "N/A",
                CustomerCategoryId = "092529097708186",
                IsNotDirectRequest = false,
                IsMemberOfACompany = false,
                IsMemberOfAGroup = true,
                EconomicActivitiesId = "N/A",
                PlaceOfBirth = data.PlaceOfBirth == null ? "N/A" : data.PlaceOfBirth,
                Fax = "N/A",
                IDNumber = data.Cni == null ? "N/A" : data.Cni,
                BankingRelationship = BankingRelationship.Member.ToString(),
                IDNumberIssueAt = data.CniDeliveranceDate == null ? "N/A" : data.CniDeliveranceDate,
                IDNumberIssueDate = data.CniLocation == null ? "N/A" : data.CniLocation,
                Income = 0.0,
                OrganizationId = _UserInfoToken.BankID,
                BankName = "N/A",
                POBox = "N/A",
                Occupation = "N/A",
                RegistrationNumber = data.MemberNumber,
                ProfileType = ProfileType.Member.ToString(),
                PlaceOfCreation = "N/A",
                CustomerPackageId = "N/A",
                NumberOfKids = 0,
                VillageOfOrigin = "N/A",
                WorkingStatus = WorkingStatus.SelfEmploy.ToString(),
                IsFileData = true,
                IsDailyCollector = true,
                CustomerCode = data.MemberNumber.Substring(3),
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
                CustomerId = /*_UserInfoToken.BranchCode + "" +*/ data.MemberNumber

            };
        }




    }

}
