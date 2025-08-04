using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using Tesseract;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to verify  IDVerificationCommand.
    /// </summary>
    public class IDVerificationCommandHandler : IRequestHandler<IDVerificationCommand, ServiceResponse<String>>
    {
       
        private readonly ILogger<IDVerificationCommandHandler> _logger; // Logger for logging handler actions and errors.
        
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the IDVerificationCommandHandler.
        /// </summary>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public IDVerificationCommandHandler(
       
            ILogger<IDVerificationCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
        
            _logger = logger;
       
            _uow = uow;
        }

        /// <summary>
        /// Handles the IDVerificationCommand to delete a Customer.
        /// </summary>
        /// <param name="request">The IDVerificationCommand containing Customer ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<String>> Handle(IDVerificationCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {

                // Provide the absolute path to your Tesseract installation directory
                string tesseractDataPath = @"C:\Program Files\Tesseract-OCR\tessdata";

                string extractedText = ExtractTextFromImage(request.ImageSrc,tesseractDataPath);
                return ServiceResponse<string>.ReturnResultWith200(extractedText);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Customer: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<string>.Return500(e);
            }
        }

        static string ExtractTextFromImage(string imagePath, string tesseractDataPath)
        {
            using (var engine = new TesseractEngine(tesseractDataPath, "eng", EngineMode.Default))
            {
                using (var image = Pix.LoadFromFile(imagePath))
                {
                    using (var page = engine.Process(image))
                    {
                        return page.GetText();
                    }
                }
            }
        }
    }

}
