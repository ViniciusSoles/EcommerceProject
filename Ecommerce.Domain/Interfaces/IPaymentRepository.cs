using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ECommerceApi.Domain.Entities;

namespace ECommerceApi.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByOrderIdAsync(Guid orderId);
    Task AddAsync(Payment payment);
    Task UpdateAsync(Payment payment);
}