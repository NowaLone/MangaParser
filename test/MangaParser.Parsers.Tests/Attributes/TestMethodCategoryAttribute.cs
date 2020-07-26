using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MangaParser.Parsers.Tests.Attributes
{
    /// <summary>
    /// Used to specify the categories of a unit test method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class TestMethodCategoryAttribute : TestCategoryBaseAttribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethodCategoryAttribute"/>class and applies the categories to the test method.
        /// </summary>
        /// <param name="testCategories">The test categories.</param>
        public TestMethodCategoryAttribute(params string[] testCategories)
        {
            TestCategories = testCategories.ToList();
        }

        #endregion Constructors

        #region Properties

        public override IList<string> TestCategories { get; }

        #endregion Properties
    }
}