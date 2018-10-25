using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OLAF
{
    public interface ILogger
    {
        void Info(string messageTemplate, params object[] propertyValues);

        void Debug(string messageTemplate, params object[] propertyValues);

        void Error(string messageTemplate, params object[] propertyValues);

        void Error(Exception e, string messageTemplate, params object[] propertyValues);

        void Verbose(string messageTemplate, params object[] propertyValues);

        void Warn(string messageTemplate, params object[] propertyValues);

        IOperationContext Begin(string messageTemplate, params object[] args);

        void Close();
    }
}
