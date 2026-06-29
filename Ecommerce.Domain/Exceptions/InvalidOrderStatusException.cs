using ECommerceApi.Domain.Exceptions;

namespace EcommerceProject;

public class InvalidOrderStatusException : DomainException
{

    public InvalidOrderStatusException(string from,string to)
        : base($"Não foi possível mudar o Status de'{from}' para'{to}'.") { }

}
