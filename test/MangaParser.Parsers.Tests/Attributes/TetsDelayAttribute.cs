using System;

namespace MangaParser.Parsers.Tests.Attributes
{
    /// <summary>
    /// Used to specify the delay of a unit test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    internal sealed class TestMethodDelayAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethodDelayAttribute"/> class and applies the delay to the test.
        /// </summary>
        /// <param name="delay">The number of milliseconds for which the test is suspended.</param>
        public TestMethodDelayAttribute(int delay = 1500)
        {
            Delay = delay >= 0 ? delay : 0;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the delay that has been applied to the test.
        /// </summary>
        /// <returns>The delay that has been applied to the test.</returns>
        public int Delay { get; }

        #endregion Properties
    }
}