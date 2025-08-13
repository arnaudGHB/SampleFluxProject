using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Data.SalaryFilesDto.SalaryFiles;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.FileUploadP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Loan by its unique identifier.
    /// </summary>
    public class GetFileUploadQuery : IRequest<ServiceResponse<FileUploadDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Fee to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
