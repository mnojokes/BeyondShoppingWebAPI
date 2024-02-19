using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondShopping.Contracts.Responses;

public record CreateOrderResponse(int Id, int userId, DateTime ExpiresAt);
