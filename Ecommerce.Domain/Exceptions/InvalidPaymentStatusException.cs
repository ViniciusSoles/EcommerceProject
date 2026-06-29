using ECommerceApi.Domain.Exceptions;

namespace EcommerceProject;

public class InvalidPaymentStatusException : DomainException
{
    public InvalidPaymentStatusException(string action, string currentStatus)


     : base($"Não é possível realizar '{action}' com o status '{currentStatus} atual."){}
    
}
