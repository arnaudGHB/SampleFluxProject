
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
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.ColorSpaces;
using System.Text;
using NPOI.OpenXml4Net.OPC.Internal;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Customer.
    /// </summary>
    public class VerifyCustomerFileCommandHandler : IRequestHandler<VerifyCustomerFileCommand, ServiceResponse<List<CustomerFileMemberData>>>
    {
        private readonly IHostingEnvironment _webHostEnvironment;
        private readonly ILogger<DeleteCustomerFileCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly ICustomerRepository _customerRepository;
        private readonly IGroupCustomerRepository _groupCustomerRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly PathHelper _PathHelper;

        /// <summary>
        /// Constructor for initializing the AddCustomerCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public VerifyCustomerFileCommandHandler(
            ILogger<DeleteCustomerFileCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken,
            IHostingEnvironment webHostEnvironment,
            ICustomerRepository customerRepository,
            IGroupCustomerRepository groupCustomerRepository,
            IGroupRepository groupRepository,
            PathHelper pathHelper)
        {
            _logger = logger;
            _uow = uow;
            _UserInfoToken = userInfoToken;
            _webHostEnvironment = webHostEnvironment;
            _customerRepository = customerRepository;
            _groupCustomerRepository = groupCustomerRepository;
            _groupRepository = groupRepository;
            _PathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the AddCustomerCommand to add a new Customer.
        /// </summary>
        /// <param name="request">The AddCustomerCommand containing Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CustomerFileMemberData>>> Handle(VerifyCustomerFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                List<CustomerFileMemberData> fileMemberDatas = new List<CustomerFileMemberData>();
                List<CustomerFileMemberData> fileUnSavedMemberDatas = new List<CustomerFileMemberData>();
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
                            Type = row.GetCell(3)?.ToString(),
                            CreationDate = row.GetCell(4)?.ToString(),
                            DateOfBirth = row.GetCell(5)?.ToString(),
                            PlaceOfBirth = row.GetCell(6)?.ToString(),
                            Genre = row.GetCell(7)?.ToString(),
                            Quater = row.GetCell(8)?.ToString(),
                            Town = row.GetCell(9)?.ToString(),
                            Telephone = row.GetCell(10)?.ToString(),
                            Cni = row.GetCell(12)?.ToString(),
                            CniDeliveranceDate = row.GetCell(14)?.ToString(),
                            CniLocation = row.GetCell(13)?.ToString(),
                            BranchCode = _UserInfoToken.BranchCode,
                        };
                        fileMemberDatas.Add(data);
                    }

                    if (fileMemberDatas.Count > 0)
                    {
                        foreach (var data in fileMemberDatas)
                        {
                            switch (data.Type)
                            {

                                case "P":
                                    {
                                        var result =await  GetCustomer(request.BranchCode, data);
                                        if (result != null)fileUnSavedMemberDatas.Add(result);
                                    }
                                    break;
                                case "M":
                                    {

                                        var result=await GetCustomer(request.BranchCode,data);
                                        if (result != null) fileUnSavedMemberDatas.Add(result);
                                    }
                                    break;


                            }
                        }

                    }
                    return ServiceResponse<List<CustomerFileMemberData>>.ReturnResultWith200(fileUnSavedMemberDatas);

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


        private  async Task< CustomerFileMemberData> GetCustomer(string BranchCode,CustomerFileMemberData data)
        {
            var groupCustomer =await (_customerRepository.FindAsync(BranchCode+"" + data.MemberNumber));
            if (groupCustomer == null)
            {
                return data;
            }

            return null;

        }




    }

}
