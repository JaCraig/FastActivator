using Fast.Activator.Tests.BaseClasses;
using Fast.Activator.Tests.HelperData;
using Fast.Activator.Utils;

namespace Fast.Activator.Tests.Utils
{
    public class ConstructorListTests : TestBaseClass<ConstructorList>
    {
        public ConstructorListTests()
        {
            TestObject = new ConstructorList(typeof(TestDataClass), typeof(TestDataClass).GetHashCode());
        }
    }
}