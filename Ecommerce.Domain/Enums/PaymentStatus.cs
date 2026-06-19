using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,
        Approved = 2,
        Refused = 3,
        Refunded = 4
    }
}
