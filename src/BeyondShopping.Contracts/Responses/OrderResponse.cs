using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondShopping.Contracts.Responses;

public record OrderResponse(int Id, string Status);
public record OrderResponseList(List<OrderResponse> Orders);
