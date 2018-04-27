using System;
using System.Collections.Generic;
using System.Text;

namespace StoneCo.Utils.IO.Exceptions
{
    public class ForbiddenOperationException : Exception
    {
        public ForbiddenOperationException(string message) : base(message)
        {

        }
    }
}
