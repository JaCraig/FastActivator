namespace FastActivator.Example
{
    /// <summary>
    /// Test class used to demonstrate FastActivator
    /// </summary>
    public class TestClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestClass"/> class.
        /// </summary>
        public TestClass()
        {
            System.Console.WriteLine("I was created");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClass"/> class.
        /// </summary>
        /// <param name="test">The test string.</param>
        public TestClass(string test)
        {
            System.Console.WriteLine("I was created with a string: {0}", test);
        }
    }

    /// <summary>
    /// Main program
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            // Create an object using the default constructor
            var MyObject = Fast.Activator.FastActivator.CreateInstance<TestClass>();

            // Create an object using the constructor with a string parameter
            var MyObject2 = Fast.Activator.FastActivator.CreateInstance<TestClass>("Hello World");
        }
    }
}