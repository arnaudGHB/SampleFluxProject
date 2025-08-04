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
    public class GetAuditTrailsDataTableQueryHandler : IRequestHandler<GetAuditTrailsDataTableQuery, ServiceResponse<CustomDataTable>>
    {
        private readonly ILogger<GetAuditTrailsDataTableQueryHandler> _logger;
        private readonly IMongoUnitOfWork _mongoUnitOfWork;
        private readonly IMapper _mapper;

        public GetAuditTrailsDataTableQueryHandler(
            IMongoUnitOfWork mongoUnitOfWork,
            IMapper mapper,
            ILogger<GetAuditTrailsDataTableQueryHandler> logger)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ServiceResponse<CustomDataTable>> Handle(GetAuditTrailsDataTableQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var options = request.DataTableOptions;
                var auditTrailRepository = _mongoUnitOfWork.GetRepository<AuditTrail>();
                var filters = new List<FilterDefinition<AuditTrail>>();

                // Date range filter
                if (request.StartDate != default && request.EndDate != default)
                {
                    filters.Add(Builders<AuditTrail>.Filter.And(
                        Builders<AuditTrail>.Filter.Gte(a => a.Timestamp, request.StartDate),
                        Builders<AuditTrail>.Filter.Lte(a => a.Timestamp, request.EndDate)
                    ));
                }

                FilterDefinition<AuditTrail> combinedFilter = FilterDefinition<AuditTrail>.Empty;

                if (!string.IsNullOrWhiteSpace(options.searchValue))
                {
                    var indexCursor = await auditTrailRepository.Collection.Indexes.ListAsync();
                    var textIndexes = await indexCursor.ToListAsync();
                    bool textIndexExists = textIndexes.Any(index =>
                        index.TryGetValue("name", out var name) && name.AsString.Contains("text"));

                    if (textIndexExists)
                    {
                        // Perform initial text search
                        var textFilter = Builders<AuditTrail>.Filter.Text(options.searchValue);
                        filters.Add(textFilter);
                        combinedFilter = Builders<AuditTrail>.Filter.And(filters);

                        // Check if text search returns any results
                        var initialEntities = await auditTrailRepository.GetPagedFilteredAndSortedAsync(combinedFilter, Builders<AuditTrail>.Sort.Descending("Timestamp"), 0, 1);

                        if (!initialEntities.Any())
                        {
                            // Remove text filter and apply regex search if no results
                            filters.Remove(textFilter);
                            AddSimpleRegexFilter(options.searchValue, request.Feild, filters);
                            combinedFilter = Builders<AuditTrail>.Filter.And(filters);
                        }
                    }
                    else
                    {
                        // Directly fallback to regex search if no text index exists
                        AddSimpleRegexFilter(options.searchValue, request.Feild, filters);
                        combinedFilter = Builders<AuditTrail>.Filter.And(filters);
                    }
                }

                // Validate and set the sort column
                var sortColumn = typeof(AuditTrail).GetProperty(options.sortColumnName) != null ? options.sortColumnName : "Timestamp";

                // Sorting logic
                var sort = Builders<AuditTrail>.Sort.Combine(
                    Builders<AuditTrail>.Sort.Descending("Timestamp"),
                    options.sortDirection?.ToLower() == "asc"
                        ? Builders<AuditTrail>.Sort.Ascending(sortColumn)
                        : Builders<AuditTrail>.Sort.Descending(sortColumn)
                );

                // Pagination
                var skip = Math.Max(options.skip, 0);
                var pageSize = Math.Max(options.pageSize, 10);

                // Execute queries concurrently
                var pagedDataTask = auditTrailRepository.GetPagedFilteredAndSortedAsync(combinedFilter, sort, skip, pageSize);
                var filteredCountTask = auditTrailRepository.CountAsync(combinedFilter);
                var totalCountTask = auditTrailRepository.CountAsync(FilterDefinition<AuditTrail>.Empty);

                await Task.WhenAll(pagedDataTask, filteredCountTask, totalCountTask);

                // Retrieve results
                var entities = await pagedDataTask;
                var filteredRecords = await filteredCountTask;
                var totalRecords = await totalCountTask;

                // Map to DTOs
                var auditTrailDtos = _mapper.Map<List<AuditTrailDto>>(entities);

                // Parse draw safely
                var draw = int.TryParse(options.draw, out var parsedDraw) ? parsedDraw : 1;

                // Create the CustomDataTable response
                var dataTable = new CustomDataTable(
                    draw: parsedDraw,
                    recordsTotal: totalRecords,
                    recordsFiltered: filteredRecords,
                    data: auditTrailDtos,
                    dataTableOptions: options
                );

                return ServiceResponse<CustomDataTable>.ReturnResultWith200(dataTable);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to get AuditTrails for DataTable: {Message}", e.Message);
                return ServiceResponse<CustomDataTable>.Return500(e, "Failed to get AuditTrails for DataTable");
            }
        }

        /// <summary>
        /// Adds a simple regex filter based on the search term provided.
        /// </summary>
        private void AddSimpleRegexFilter(string searchValue, string searchField, List<FilterDefinition<AuditTrail>> filters)
        {
            var regex = new MongoDB.Bson.BsonRegularExpression(searchValue, "i");

            if (!string.IsNullOrWhiteSpace(searchField))
            {
                if (typeof(AuditTrail).GetProperty(searchField) == null)
                {
                    throw new ArgumentException($"Invalid search field: {searchField}");
                }

                // Apply regex to the specified field
                filters.Add(Builders<AuditTrail>.Filter.Regex(searchField, regex));
            }
            else
            {
                // Apply regex across multiple fields if no specific field is provided
                filters.Add(Builders<AuditTrail>.Filter.Or(
                    Builders<AuditTrail>.Filter.Regex(a => a.Action, regex),
                    Builders<AuditTrail>.Filter.Regex(a => a.FullName, regex),
                    Builders<AuditTrail>.Filter.Regex(a => a.DetailMessage, regex),
                    Builders<AuditTrail>.Filter.Regex(a => a.Level, regex),
                    Builders<AuditTrail>.Filter.Regex(a => a.StringifyObject, regex)
                ));
            }
        }

        //public async Task<ServiceResponse<CustomDataTable>> Handle(GetAuditTrailsDataTableQuery request, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var options = request.DataTableOptions;
        //        var auditTrailRepository = _mongoUnitOfWork.GetRepository<AuditTrail>();

        //        // List of filters to combine
        //        var filters = new List<FilterDefinition<AuditTrail>>();

        //        // Date range filter
        //        if (request.StartDate != default && request.EndDate != default)
        //        {
        //            filters.Add(Builders<AuditTrail>.Filter.And(
        //                Builders<AuditTrail>.Filter.Gte(a => a.Timestamp, request.StartDate),
        //                Builders<AuditTrail>.Filter.Lte(a => a.Timestamp, request.EndDate)
        //            ));
        //        }

        //        // Dynamic search filter based on user-provided field or fallback to multiple fields
        //        if (!string.IsNullOrWhiteSpace(options.searchValue))
        //        {
        //            var indexCursor = await auditTrailRepository.Collection.Indexes.ListAsync();
        //            var textIndexes = await indexCursor.ToListAsync();
        //            bool textIndexExists = textIndexes.Any(index =>
        //                index.TryGetValue("name", out var name) && name.AsString.Contains("text"));

        //            if (textIndexExists)
        //            {
        //                // Use efficient text search if a text index is present
        //                filters.Add(Builders<AuditTrail>.Filter.Text(options.searchValue));
        //            }
        //            else
        //            {
        //                // Split search value into keywords (e.g., "error system crash" → ["error", "system", "crash"])
        //                var keywords = options.searchValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        //                // Handle single dynamic field if provided
        //                if (!string.IsNullOrWhiteSpace(request.Feild))
        //                {
        //                    if (typeof(AuditTrail).GetProperty(request.Feild) == null)
        //                    {
        //                        throw new ArgumentException($"Invalid search field: {request.Feild}");
        //                    }

        //                    // Create prefix-based regex filters for each keyword targeting the provided field
        //                    var keywordFilters = keywords.Select(keyword =>
        //                    {
        //                        var prefixRegex = new MongoDB.Bson.BsonRegularExpression($"^{keyword}", "i");
        //                        return Builders<AuditTrail>.Filter.Regex(request.Feild, prefixRegex);
        //                    }).ToList();

        //                    filters.Add(Builders<AuditTrail>.Filter.Or(keywordFilters));
        //                }
        //                else
        //                {
        //                    // Search across multiple fields if no specific field is provided
        //                    var keywordFilters = keywords.Select(keyword =>
        //                    {
        //                        var prefixRegex = new MongoDB.Bson.BsonRegularExpression($"^{keyword}", "i");
        //                        return Builders<AuditTrail>.Filter.Or(
        //                            Builders<AuditTrail>.Filter.Regex(a => a.Action, prefixRegex),
        //                            Builders<AuditTrail>.Filter.Regex(a => a.FullName, prefixRegex),
        //                            Builders<AuditTrail>.Filter.Regex(a => a.DetailMessage, prefixRegex),
        //                            Builders<AuditTrail>.Filter.Regex(a => a.Level, prefixRegex),
        //                            Builders<AuditTrail>.Filter.Regex(a => a.StringifyObject, prefixRegex)
        //                        );
        //                    }).ToList();

        //                    filters.Add(Builders<AuditTrail>.Filter.Or(keywordFilters));
        //                }
        //            }
        //        }

        //        var combinedFilter = filters.Any() ? Builders<AuditTrail>.Filter.And(filters) : FilterDefinition<AuditTrail>.Empty;

        //        // Validate and set the sort column
        //        var sortColumn = typeof(AuditTrail).GetProperty(options.sortColumnName) != null ? options.sortColumnName : "Timestamp";

        //        // Sorting logic with fallback to Timestamp
        //        var sort = Builders<AuditTrail>.Sort.Combine(
        //            Builders<AuditTrail>.Sort.Descending("Timestamp"),
        //            options.sortDirection?.ToLower() == "asc"
        //                ? Builders<AuditTrail>.Sort.Ascending(sortColumn)
        //                : Builders<AuditTrail>.Sort.Descending(sortColumn)
        //        );

        //        // Pagination parameters with safety checks
        //        var skip = Math.Max(options.skip, 0);
        //        var pageSize = Math.Max(options.pageSize, 10);

        //        // Execute queries concurrently
        //        var pagedDataTask = auditTrailRepository.GetPagedFilteredAndSortedAsync(combinedFilter, sort, skip, pageSize);
        //        var filteredCountTask = auditTrailRepository.CountAsync(combinedFilter);
        //        var totalCountTask = auditTrailRepository.CountAsync(FilterDefinition<AuditTrail>.Empty);

        //        try
        //        {
        //            await Task.WhenAll(pagedDataTask, filteredCountTask, totalCountTask);
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError("Failed to execute queries concurrently: {Exception}", ex);
        //            throw;  // Let the outer catch handle this properly
        //        }

        //        // Retrieve the results
        //        var entities = await pagedDataTask;
        //        var filteredRecords = await filteredCountTask;
        //        var totalRecords = await totalCountTask;

        //        // Map to DTOs
        //        var auditTrailDtos = _mapper.Map<List<AuditTrailDto>>(entities);

        //        // Parse draw safely
        //        var draw = int.TryParse(options.draw, out var parsedDraw) ? parsedDraw : 1;

        //        // Create the CustomDataTable response
        //        var dataTable = new CustomDataTable(
        //            draw: parsedDraw,
        //            recordsTotal: totalRecords,
        //            recordsFiltered: filteredRecords,
        //            data: auditTrailDtos,
        //            dataTableOptions: options
        //        );

        //        return ServiceResponse<CustomDataTable>.ReturnResultWith200(dataTable);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Failed to get AuditTrails for DataTable: {Message}", e.Message);
        //        return ServiceResponse<CustomDataTable>.Return500(e, "Failed to get AuditTrails for DataTable");
        //    }
        //}

    }

}
