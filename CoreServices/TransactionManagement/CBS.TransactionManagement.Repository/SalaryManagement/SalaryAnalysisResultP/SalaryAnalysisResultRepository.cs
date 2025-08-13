using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.FileUploadP;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.Repository.SalaryManagement.SalaryAnalysisResultP
{

    public class SalaryAnalysisResultRepository : GenericRepository<SalaryAnalysisResult, TransactionContext>, ISalaryAnalysisResultRepository
    {
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IFileUploadRepository _fileUploadRepository;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<SalaryAnalysisResultRepository> _logger;
        public SalaryAnalysisResultRepository(
            IUnitOfWork<TransactionContext> unitOfWork,
            IFileUploadRepository fileUploadRepository,
            ILogger<SalaryAnalysisResultRepository> logger,
            PathHelper pathHelper,
            UserInfoToken userInfoToken) : base(unitOfWork)
        {
            _uow = unitOfWork;
            _fileUploadRepository = fileUploadRepository;
            _logger = logger;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

    }



}
