
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
    public class DownloadCustomerFileCommandHandler : IRequestHandler<DownloadCustomerFileCommand, ServiceResponse<Stream>>
    {
        private readonly IHostingEnvironment _webHostEnvironment;
        private readonly ILogger<DeleteCustomerFileCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUploadedCustomerWithErrorRepository _UploadedCustomerWithErrorRepository; // Repository for accessing Uploaded Customers with Error data.
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
        public DownloadCustomerFileCommandHandler(
            ILogger<DeleteCustomerFileCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken,
            IHostingEnvironment webHostEnvironment,
            PathHelper pathHelper,
            IUploadedCustomerWithErrorRepository uploadedCustomerWithErrorRepository)
        {
            _logger = logger;
            _uow = uow;
            _UserInfoToken = userInfoToken;
            _webHostEnvironment = webHostEnvironment;
            _PathHelper = pathHelper;
            _UploadedCustomerWithErrorRepository = uploadedCustomerWithErrorRepository;
        }

        /// <summary>
        /// Handles the AddCustomerCommand to add a new Customer.
        /// </summary>
        /// <param name="request">The AddCustomerCommand containing Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<Stream>> Handle(DownloadCustomerFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _webHostEnvironment.WebRootPath = _webHostEnvironment.WebRootPath == null ? _webHostEnvironment.ContentRootPath : _webHostEnvironment.WebRootPath;
                string path = Path.Combine(_webHostEnvironment.WebRootPath, _PathHelper.UploadFileSubPath, "ErrorFile" + request.BranchCode + ".csv");
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
                var head = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", "ClientNo", "Name", "SurName", "Type", "CreationDate", "DateOfBirth", "PlaceOfBirth", "Genre", "Quarter", "Town", "Telephone", "Cni", "CniLocation", "CniDeliveranceDate","InitialError", "resolved");
                csv.AppendLine(head);
                sw.WriteLine(head);
                var datas = _UploadedCustomerWithErrorRepository.FindBy(r => r.BranchCode.StartsWith(request.BranchCode) && r.resolved==false).ToList();
                foreach (var data in datas)
                {
                    var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}", data.MemberNumber, data.MemberName, data.MemberSurName, data.Type, data.CreationDate, data.DateOfBirth, data.PlaceOfBirth, data.Genre, data.Quater, data.Town, data.Telephone, data.Cni, data.CniLocation,data.CniDeliveranceDate, data.InitialError,data.resolved);
                    csv.AppendLine(newLine);
                    sw.WriteLine(newLine);
                }

                sw.Close();
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                return ServiceResponse<Stream>.ReturnResultWith200(stream);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Customer: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), e.Message, $"Internal Server Error occurred while saving new Customer ", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);

                return ServiceResponse<Stream>.Return500(e);
            }
        }


    }

}
