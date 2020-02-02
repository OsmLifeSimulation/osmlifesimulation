using System;
using System.Collections.Generic;
using System.Text;

namespace OSMLSGlobalLibrary.Modules
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomInitializationOrderAttribute : Attribute
    {
        public double InitializationOrder { get; }
        public CustomInitializationOrderAttribute(double initializationOrder = 0)
        {
            InitializationOrder = initializationOrder;
        }
    }
}
