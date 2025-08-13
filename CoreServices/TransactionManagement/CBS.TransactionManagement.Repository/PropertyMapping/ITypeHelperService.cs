namespace CBS.TransactionManagement.Repository
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
