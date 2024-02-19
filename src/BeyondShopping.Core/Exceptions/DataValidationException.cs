using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondShopping.Core.Exceptions;

public class DataValidationException : Exception
{
    public DataValidationException(string message) : base(message) { }
}
