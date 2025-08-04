using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA;
using CBS.CUSTOMER.DOMAIN.Context;


namespace CBS.CUSTOMER.REPOSITORY
{

    public class QuestionRepository : GenericRepository<Question, POSContext>, IQuestionRepository
    {
        public QuestionRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
