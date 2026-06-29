namespace ECommerceApi.Domain.Exceptions;

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string productName, int available, int requested)
        : base($"Estoque insuficiente para '{productName}'. disponível: {available}, requerido: {requested}.") { }
}