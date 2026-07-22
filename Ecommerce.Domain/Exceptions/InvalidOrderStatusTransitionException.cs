using ECommerceApi.Domain.Exceptions;

namespace EcommerceProject;

public class InvalidOrderStatusTransitionException : DomainException
{

    public InvalidOrderStatusTransitionException(string from,string to)
        : base($"Não foi possível mudar o Status de'{from}' para'{to}'.", "INVALID_ORDER_STATUS_TRANSITION") { }

}
