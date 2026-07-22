using ECommerceApi.Domain.Exceptions;

namespace EcommerceProject;

public class CannotCancelShippedOrderException : DomainException
{
    public CannotCancelShippedOrderException()
        : base($"Não é possível cancelar um pedido que já foi enviado ou entregue.", "CANNOT_CANCEL_SHIPPED_ORDER") { }
}






