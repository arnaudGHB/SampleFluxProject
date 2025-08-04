using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.MediatR.ChangeManagement.Command;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a Currency notes.
    /// </summary>
    public class AddCurrencyNotesCommandHandler : IRequestHandler<AddCurrencyNotesCommand, ServiceResponse<List<CurrencyNotesDto>>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly ICurrencyNotesRepository _CurrencyNotesRepository; // Repository for accessing CurrencyNotes data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCurrencyNotesCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddCurrencyNotesCommandHandler.
        /// </summary>
        /// <param name="CurrencyNotesRepository">Repository for CurrencyNotes data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCurrencyNotesCommandHandler(
            ICurrencyNotesRepository CurrencyNotesRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<AddCurrencyNotesCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow)
        {
            _CurrencyNotesRepository = CurrencyNotesRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = UserInfoToken;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddCurrencyNotesCommand to add a Currency notes.
        /// </summary>
        /// <param name="request">The AddCurrencyNotesCommand containing CurrencyNotes data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CurrencyNotesDto>>> Handle(AddCurrencyNotesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var notes = request.CurrencyNote;
                string Reference = request.Reference;
                var currencyNotesList = CurrencyNotesMapper.ComputeCurrencyNote(notes, Reference);
                _CurrencyNotesRepository.AddRange(currencyNotesList);
                var CurrencyNotesDto = _mapper.Map<List<CurrencyNotesDto>>(currencyNotesList);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "Notes added to context.", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<CurrencyNotesDto>>.ReturnResultWith200(CurrencyNotesDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving CurrencyNotes: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<CurrencyNotesDto>>.Return500(e);
            }
        }
    }

}
