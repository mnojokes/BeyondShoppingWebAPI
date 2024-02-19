using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondShopping.Contracts.Requests;

public record CreateOrderRequest(int UserId, List<int> Items);
