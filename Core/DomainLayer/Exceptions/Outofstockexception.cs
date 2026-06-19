namespace DomainLayer.Exceptions
{
    public sealed class OutOfStockException(string productName, int available)
        : Exception($"'{productName}' is out of stock. Available quantity: {available}.")
    {
    }
}