// Source: Votre modèle CountryRepository appliqué à notre contexte.
using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;


namespace CBS.UserServiceManagement.Repository
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