using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
 
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class BudgetPeriodRepository : GenericRepository<BudgetPeriod, POSContext>, IBudgetPeriodRepository
    {
        public BudgetPeriodRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
        public   List<BudgetPeriod> GenerateBudgetPeriods(int startYear, int endYear)
        {
            List<BudgetPeriod> periods = new List<BudgetPeriod>();

            for (int year = startYear; year <= endYear; year++)
            {
                periods.AddRange(GenerateAnnualBudgetPeriods(year));
            }

            return periods;
        }

        private   List<BudgetPeriod> GenerateAnnualBudgetPeriods(int year)
        {
            List<BudgetPeriod> periods = new List<BudgetPeriod>();

            // Add annual period
            periods.Add(new BudgetPeriod
            {
                Id = (year).ToString(),
                Name = $"{year} Annual Budget",
                StartDate = new DateTime(year, 1, 1),
                EndDate = new DateTime(year, 12, 31)
            });
            // Add semesterly periods
            for (int semester = 1; semester <= 2; semester++)
            {
                DateTime semesterStart = new DateTime(year, (semester - 1) * 2 + 1, 1);
                DateTime semesterEnd = semesterStart.AddMonths(4).AddDays(-1);

                periods.Add(new BudgetPeriod
                {
                    Id = (periods.Count + 1).ToString(),
                    Name = $"{year} S{semester}",
                    StartDate = semesterStart,
                    EndDate = semesterEnd
                });
            }
            // Add quarterly periods
            for (int quarter = 1; quarter <= 4; quarter++)
            {
                DateTime quarterStart = new DateTime(year, (quarter - 1) * 3 + 1, 1);
                DateTime quarterEnd = quarterStart.AddMonths(3).AddDays(-1);

                periods.Add(new BudgetPeriod
                {
                    Id = (periods.Count + 1).ToString(),
                    Name = $"{year} Q{quarter}",
                    StartDate = quarterStart,
                    EndDate = quarterEnd
                });
            }
            // Add Termly periods
            for (int Termly = 1; Termly <= 3; Termly++)
            {
                DateTime TermStart = new DateTime(year, (Termly - 1) * 4 + 1, 1);
                DateTime TermEnd = TermStart.AddMonths(4).AddDays(-1);

                periods.Add(new BudgetPeriod
                {
                    Id = (periods.Count + 1).ToString(),
                    Name = $"{year} T{Termly}",
                    StartDate = TermStart,
                    EndDate = TermEnd
                });
            }
            // Add monthly periods
            for (int month = 1; month <= 12; month++)
            {
                DateTime monthStart = new DateTime(year, month, 1);
                DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

                periods.Add(new BudgetPeriod
                {
                    Id = (periods.Count + 1).ToString(),
                    Name = $"{year} M{month.ToString("D2")}",
                    StartDate = monthStart,
                    EndDate = monthEnd
                });
            }

            return periods;
        }
    }
}