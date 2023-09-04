using Common.Console;
using Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class mException : Exception
    {
        public mException(string? message) : base(message)
        {
            Display.WriteFatal(ErrorHelper.GetErrorMsg(this));
            // do logging of errors or something
        }

    }
}
