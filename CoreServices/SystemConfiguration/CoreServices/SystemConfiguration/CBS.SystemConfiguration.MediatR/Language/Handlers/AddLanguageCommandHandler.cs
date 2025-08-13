using AutoMapper;
using CBS.SystemConfiguration.COMMON.UnitOfWork;
using CBS.SystemConfiguration.Data.Dto.Languages;
using CBS.SystemConfiguration.Data.Entity;
using CBS.SystemConfiguration.DOMAIN.Context;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Language.Commands;
using CBS.SystemConfiguration.Repository.Language;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using CBS.SystemConfiguration.Helper.DataModel;

namespace CBS.SystemConfiguration.MediatR.Language.Handlers
{
    public class AddLanguageCommandHandler : IRequestHandler<AddLanguageCommand, ServiceResponse<LanguageDto>>
    {
        private readonly ILanguageRepository _repository;
        private readonly IUnitOfWork<SystemContext> _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<AddLanguageCommandHandler> _logger;

        public AddLanguageCommandHandler(ILanguageRepository repository, IUnitOfWork<SystemContext> uow, IMapper mapper, ILogger<AddLanguageCommandHandler> logger)
        {
            _repository = repository;
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<LanguageDto>> Handle(AddLanguageCommand request, CancellationToken cancellationToken)
        {
            var newEntity = _mapper.Map<Language>(request);
            newEntity.Id = BaseUtilities.GenerateUniqueId(36);
            newEntity.IsActive = true;

            _repository.Add(newEntity);
            await _uow.SaveAsync();

            var dto = _mapper.Map<LanguageDto>(newEntity);
            _logger.LogInformation("New language created: {Label}", dto.Label);
            return ServiceResponse<LanguageDto>.ReturnResultWith201(dto, "Language created successfully.");
        }
    }
}
