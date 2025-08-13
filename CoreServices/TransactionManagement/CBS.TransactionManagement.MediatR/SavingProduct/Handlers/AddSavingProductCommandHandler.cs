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
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Command;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new SavingProduct.
    /// </summary>
    public class AddSavingProductCommandHandler : IRequestHandler<AddSavingProductCommand, ServiceResponse<SavingProductDto>>
    {
        private readonly ISavingProductRepository _SavingProductRepository; // Repository for accessing SavingProduct data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddSavingProductCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        public IMediator _mediator { get; set; }
        public readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the AddSavingProductCommandHandler.
        /// </summary>
        /// <param name="SavingProductRepository">Repository for SavingProduct data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddSavingProductCommandHandler(
            ISavingProductRepository SavingProductRepository,
            IMapper mapper,
            ILogger<AddSavingProductCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator,
            UserInfoToken userInfoToken = null)
        {
            _SavingProductRepository = SavingProductRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddSavingProductCommand to add a new SavingProduct.
        /// </summary>
        /// <param name="request">The AddSavingProductCommand containing SavingProduct data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SavingProductDto>> Handle(AddSavingProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingProduct = await CheckForExistingProduct(request.Name, request.Code);
                if (existingProduct != null)
                {
                    var errorMessage = $"{existingProduct.Name} Or {request.Code} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<SavingProductDto>.Return409(errorMessage);
                }

                var savingProductEntity = CreateSavingProductEntity(request);
                _SavingProductRepository.Add(savingProductEntity);
                var savingProductDto = _mapper.Map<SavingProductDto>(savingProductEntity);
                savingProductDto.BankId = _userInfoToken.BankID;
                await _uow.SaveAsync();
                string message = $"Product created successfully.";
                return ServiceResponse<SavingProductDto>.ReturnResultWith200(savingProductDto, message);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while saving SavingProduct: {e}";
                _logger.LogError(errorMessage);
                return ServiceResponse<SavingProductDto>.Return500(e);
            }
        }

        private async Task<SavingProduct> CheckForExistingProduct(string name, string code)
        {
            return await _SavingProductRepository.FindBy(p => p.Name == name || p.Code == code).FirstOrDefaultAsync();
        }

        private SavingProduct CreateSavingProductEntity(AddSavingProductCommand request)
        {
            var savingProductEntity = _mapper.Map<SavingProduct>(request);
            savingProductEntity.CreatedDate = savingProductEntity.ModifiedDate = DateTime.Now;
            savingProductEntity.Id = BaseUtilities.GenerateUniqueNumber();
            return savingProductEntity;
        }


        public string GenerateUniqueCode(string bankID)
        {
            var lastElement = _SavingProductRepository.FindBy(p => p.BankId == bankID).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
            if (lastElement != null)
            {
                int number = int.Parse(lastElement.Code);
                number++;
                return number.ToString("D3");
            }
            else
            {
                return "000";
            }
        }
    }

}
