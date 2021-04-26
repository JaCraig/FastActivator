using Fast.Activator.Tests.BaseClasses;
using Fast.Activator.Tests.HelperData;
using Fast.Activator.Utils;

namespace Fast.Activator.Tests.Utils
{
    public class ConstructorTests : TestBaseClass<Constructor>
    {
        public ConstructorTests()
        {
            TestObject = new Constructor(TestMethod);
        }

        private object TestMethod(params object[] args)
        {
            return new TestDataClass();
        }
    }
}