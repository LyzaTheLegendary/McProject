using Common.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class ErrorHelper
    {
        public static string GetErrorMsg(Exception e)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1); // 1 means the previous frame in the call stack.

            var method = stackFrame.GetMethod();
            string functionName = method.Name;
            string className = method.DeclaringType.Name;
            int line = stackFrame.GetFileLineNumber();

            return $"{className}::{functionName}() {e.Message}";
            //return $"Class: {className}, Function: {functionName}, Line: {line}";

            //return sf.GetFileName() + "::" + sf.ToString() + $" error: {e.Message}";
        }
        public static string GetErrorMsg(mException e)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(2); // 1 means the previous frame in the call stack.

            var method = stackFrame.GetMethod();
            string functionName = method.Name;
            string className = method.DeclaringType.Name;
            int line = stackFrame.GetFileLineNumber();

            return $"{className}::{functionName}() {e.Message}";
            //return $"Class: {className}, Function: {functionName}, Line: {line}";

            //return sf.GetFileName() + "::" + sf.ToString() + $" error: {e.Message}";
        }
    }
}
