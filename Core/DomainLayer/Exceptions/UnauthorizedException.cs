using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions
{
    public sealed class UnauthorizedException(string message = "Invalid Email Or Password") : Exception(message)
    {
    }
}
