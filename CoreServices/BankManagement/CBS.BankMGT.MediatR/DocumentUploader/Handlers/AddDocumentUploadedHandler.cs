using AutoMapper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Data;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Data.Dto;
using Newtonsoft.Json.Linq;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new DocumentUploaded.
    /// </summary>
    public class AddDocumentUploadedCommandHandler : IRequestHandler<AddDocumentUploadedCommand, ServiceResponse<DocumentUploadedDto>>
    {
        private readonly IDocumentUploadedRepository _DocumentUploadedRepository; // Repository for accessing DocumentUploaded data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddDocumentUploadedCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        public readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the AddDocumentUploadedCommandHandler.
        /// </summary>
        /// <param name="DocumentUploadedRepository">Repository for DocumentUploaded data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddDocumentUploadedCommandHandler(
            IDocumentUploadedRepository DocumentUploadedRepository,
            IMapper mapper,
            ILogger<AddDocumentUploadedCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper = null,
            UserInfoToken userInfoToken = null)
        {
            _DocumentUploadedRepository = DocumentUploadedRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddDocumentUploadedCommand to add a new DocumentUploaded.
        /// </summary>
        /// <param name="request">The AddDocumentUploadedCommand containing DocumentUploaded data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DocumentUploadedDto>> Handle(AddDocumentUploadedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string fullPath = string.Empty;
                var filePath = $"{request.RootPath}/{_pathHelper.DocumentUploadPath}";
                var documentUploaded = new DocumentUploaded();
              
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                if (request.FormFiles.Any())
                {
                    var profileFile = request.FormFiles[0];
                    var fileExtension = Path.GetExtension(profileFile.FileName);
                    documentUploaded.DocumentName = Path.GetFileName(profileFile.FileName);
                    var newProfilePhoto = $"{Guid.NewGuid()}{fileExtension}";
                    fullPath = Path.Combine(filePath, newProfilePhoto);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        profileFile.CopyTo(stream);
                    }
                    documentUploaded.FullPath = $"{request.BaseURL}/{_pathHelper.DocumentUploadPath}{newProfilePhoto}";
                    documentUploaded.DocumentType = request.DocumentType;
                    documentUploaded.Extension = fileExtension;
                    documentUploaded.UrlPath = $"{_pathHelper.DocumentUploadPath}{newProfilePhoto}";
                    documentUploaded.OperationId = request.OperationID;
                    documentUploaded.Id=BaseUtilities.GenerateUniqueNumber();
                    documentUploaded.BaseUrl = request.BaseURL;
                    documentUploaded.FullLocalPath = fullPath;
                    documentUploaded.ServiceType = request.ServiceType;
                    var s = await APICallHelper.SaveUploadData(documentUploaded, _userInfoToken.Token);
                    if (s)
                    {
                        var message = $"File uploaded successfully with response from remote server: {s}";
                        _logger.LogError(message);
                    }
                    else
                    {
                        var message = $"File Failed to be uploaded with response from remote server: {s}";
                        _logger.LogError(message);
                    }
                   
                }
                else
                {
                    documentUploaded.FullPath = "";
                }
                if (!string.IsNullOrWhiteSpace(documentUploaded.FullPath))
                {
                    documentUploaded.FullPath = $"{documentUploaded.FullPath}";
                }
                var DocumentUploadedDto = _mapper.Map<DocumentUploadedDto>(documentUploaded);
                return ServiceResponse<DocumentUploadedDto>.ReturnResultWith200(DocumentUploadedDto);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving DocumentUploaded: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DocumentUploadedDto>.Return500(e);
            }
        }
    }

}
