using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryFiles;
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

namespace CBS.TransactionManagement.Repository.SalaryManagement.StandingOrderP
{

    public class StandingOrderRepository : GenericRepository<StandingOrder, TransactionContext>, IStandingOrderRepository
    {
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IFileUploadRepository _fileUploadRepository;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<StandingOrderRepository> _logger;
        public StandingOrderRepository(
            IUnitOfWork<TransactionContext> unitOfWork,
            IFileUploadRepository fileUploadRepository,
            ILogger<StandingOrderRepository> logger,
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
