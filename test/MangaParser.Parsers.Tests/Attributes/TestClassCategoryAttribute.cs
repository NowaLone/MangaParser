using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MangaParser.Parsers.Tests.Attributes
{
    /// <summary>
    /// Used to specify the categories of a unit test class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TestClassCategoryAttribute : TestCategoryBaseAttribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClassCategoryAttribute"/>class and applies the categories to the test class.
        /// </summary>
        /// <param name="testCategories">The test categories.</param>
        public TestClassCategoryAttribute(params string[] testCategories)
        {
            TestCategories = testCategories.ToList();
        }

        #endregion Constructors

        #region Properties

        public override IList<string> TestCategories { get; }

        #endregion Properties
    }
}