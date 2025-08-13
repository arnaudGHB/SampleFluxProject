// Source: Votre modèle CountryRepository appliqué à notre contexte.
using CBS.UserServiceMGT.Common;
using CBS.UserServiceMGT.Data;
using CBS.UserServiceMGT.Domain;


namespace CBS.UserServiceMGT.Repository
{
    public class UserRepository : GenericRepository<User, UserContext>, IUserRepository
    {
        public UserRepository(IUnitOfWork<UserContext> unitOfWork) : base(unitOfWork)
        {
            // LE CONSTRUCTEUR EST VIDE, COMME DANS LE MODÈLE.
            // Toute la logique de base est gérée par la classe GenericRepository.
        }
    }
}