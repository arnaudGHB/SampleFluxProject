
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
using CBS.CUSTOMER.DATA.Dto.Customers;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Customer.
    /// </summary>
    public class DownloadSuccessfullyUploadedCustomersCommandHandler : IRequestHandler<DownloadSuccessfullyUploadedCustomersCommand, ServiceResponse<DownloadSuccessfullyUploadedCustomersDto>>
    {
        private readonly IHostingEnvironment _webHostEnvironment;
        private readonly ILogger<DeleteCustomerFileCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing  Customers .
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;

        /// <summary>
        /// Constructor for initializing the AddCustomerCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DownloadSuccessfullyUploadedCustomersCommandHandler(
            ILogger<DeleteCustomerFileCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken,
            IHostingEnvironment webHostEnvironment,
            PathHelper pathHelper
,
            ICustomerRepository customerRepository)
        {
            _logger = logger;
            _uow = uow;
            _UserInfoToken = userInfoToken;
            _webHostEnvironment = webHostEnvironment;
            _PathHelper = pathHelper;
            _CustomerRepository = customerRepository;
        }

        /// <summary>
        /// Handles the AddCustomerCommand to add a new Customer.
        /// </summary>
        /// <param name="request">The AddCustomerCommand containing Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DownloadSuccessfullyUploadedCustomersDto>> Handle(DownloadSuccessfullyUploadedCustomersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string fileName =  _UserInfoToken.BranchName+ "SuccessfulUploaded" + ".csv";
                _webHostEnvironment.WebRootPath = _webHostEnvironment.WebRootPath == null ? _webHostEnvironment.ContentRootPath : _webHostEnvironment.WebRootPath;
                string path = Path.Combine(_webHostEnvironment.WebRootPath, _PathHelper.UploadFileSubPath, fileName);
                string directoryPath = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                
                StreamWriter sw = new StreamWriter(path, true);
                var csv = new StringBuilder();
                var head = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", "CustomerId", "Phone", "FirstName", "LastName", "IDNumber", "Address", "DateOfBirth", "CreatedDate");
                csv.AppendLine(head);
                sw.WriteLine(head);
                var datas = _CustomerRepository.FindBy(r => r.BranchId == _UserInfoToken.BranchID).ToList();
                foreach (var data in datas)
                {
                    var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", data.CustomerId, data.Phone, data.FirstName, data.LastName, data.IDNumber, data.Address,data.DateOfBirth,data.CreatedDate);
                    csv.AppendLine(newLine);
                    sw.WriteLine(newLine);
                }

                sw.Close();
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                return ServiceResponse<DownloadSuccessfullyUploadedCustomersDto>.ReturnResultWith200(new DownloadSuccessfullyUploadedCustomersDto() { FileName=fileName,Stream=stream});
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Customer: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), e.Message, $"Internal Server Error occurred while saving new Customer ", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);

                return ServiceResponse<DownloadSuccessfullyUploadedCustomersDto>.Return500(e);
            }
        }


    }

}
