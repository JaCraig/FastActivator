using Fast.Activator.Utils;
using FastActivator.PropertyTests.BaseClasses;
using FastActivator.PropertyTests.Data;
using System;
using System.Reflection;

namespace FastActivator.PropertyTests.Utils
{
    /// <summary>
    /// Constructor tests
    /// </summary>
    /// <seealso cref="TestBaseClass{Constructor}"/>
    public class ConstructorTests : TestBaseClass<Constructor>
    {
        public ConstructorTests()
        {
            TestObject = new Constructor(typeof(TestClass).GetConstructor(Array.Empty<Type>()), Array.Empty<ParameterInfo>());
        }
    }
}