using AutoMapper;
using CBS.BankMGT.Common.Repository.Uow;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data.Pagging;
using CBS.BankMGT.Helper;
using CBS.BankMGT.MediatR.AuditTrailP.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.MediatR.AuditTrailP.Handlers
{
    public class GetPagedAuditTrailQueryHandler : IRequestHandler<GetPagedAuditTrailQuery, ServiceResponse<PaginatedResult<AuditTrailDto>>>
    {
        private readonly ILogger<GetPagedAuditTrailQueryHandler> _logger;
        private readonly IMongoUnitOfWork _mongoUnitOfWork;
        private readonly IMapper _mapper;

        public GetPagedAuditTrailQueryHandler(
            IMongoUnitOfWork mongoUnitOfWork,
            IMapper mapper,
            ILogger<GetPagedAuditTrailQueryHandler> logger)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<PaginatedResult<AuditTrailDto>>> Handle(GetPagedAuditTrailQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var auditTrailRepository = _mongoUnitOfWork.GetRepository<AuditTrail>();

                // Get total count for pagination
                var totalRecords = await auditTrailRepository.CountAsync();

                // Calculate pagination details
                var skip = (request.PageNumber - 1) * request.PageSize;
                var entities = await auditTrailRepository.GetPagedAsync(skip, request.PageSize);
                var auditTrailDtos = _mapper.Map<List<AuditTrailDto>>(entities);

                // Build paginated result
                var result = new PaginatedResult<AuditTrailDto>
                {
                    Items = auditTrailDtos,
                    TotalCount = totalRecords,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                return ServiceResponse<PaginatedResult<AuditTrailDto>>.ReturnResultWith200(result);
            }
            catch (Exception e)
            {
                var errorMessage = $"Failed to get paged AuditTrails: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<PaginatedResult<AuditTrailDto>>.Return500(e, "Failed to get paged AuditTrails");
            }
        }
      

    }

}
