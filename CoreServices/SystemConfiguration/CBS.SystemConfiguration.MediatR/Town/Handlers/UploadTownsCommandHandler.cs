
using AutoMapper;
 
using MediatR;
using Microsoft.Extensions.Logging;
 
using CBS.APICaller.Helper.LoginModel.Authenthication;
using Microsoft.EntityFrameworkCore;
 
using CBS.SystemConfiguration.MediatR.Commands;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.Repository;
using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Common;
using CBS.SystemConfiguration.Domain;
using System.CodeDom;


namespace CBS.AccountManagement.MediatR.Handlers
{

    public class UploadTownsCommandHandler : IRequestHandler<UploadTownsCommand, ServiceResponse<bool>>
    {
        // Dependencies
        private readonly ITownRepository _townRepository;
        private readonly ILogger<UploadTownsCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _madiator;
        private readonly PathHelper _pathHelper;
        private readonly IUnitOfWork<SystemContext> _uow;
        private readonly ISubdivisionRepository _subdivisionRepository;
        private readonly IDivisionRepository _divisionRepository;

        // Constructor to inject dependencies
        public UploadTownsCommandHandler(ITownRepository townRepository, ILogger<UploadTownsCommandHandler> logger, IMapper mapper, UserInfoToken userInfoToken, IDivisionRepository? divisionRepository, ISubdivisionRepository? subdivisionRepository, IUnitOfWork<SystemContext>? uow, IMediator? madiator, PathHelper? pathHelper)
        {
 
            _logger = logger;
            _townRepository = townRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _uow = uow;
            _madiator = madiator;
            _pathHelper = pathHelper;

        }

     
        public async Task<ServiceResponse<bool>> Handle(UploadTownsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var item in request.TownFile)
                {
                    var sub = await _subdivisionRepository.FindAsync(item.SubdivisonId);
                    if (sub != null)
                    {
                        var model = new AddTownCommand
                        {
                            Id = item.Id,
                            Name = item.Name,
                            SubdivisionId = item.SubdivisonId,
                            DivisionId = item.DivisionId


                        };
                        var result = await _madiator.Send(model);
                        if (!result.Success)
                        {
                            throw new ArgumentException("Town registration failed");
                        }
                        else
                        {

                        }
                    }
                    else 
                    {
                        var div = await _divisionRepository.FindAsync(item.DivisionId);
                    }
                   
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception ex)
            {

                throw (ex);
            }
        }
    }
}


