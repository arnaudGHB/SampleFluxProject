namespace CBS.CUSTOMER.REPOSITORY
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}
