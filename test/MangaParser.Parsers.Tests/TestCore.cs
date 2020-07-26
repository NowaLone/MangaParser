using MangaParser.Core.Interfaces;
using MangaParser.Parsers.Tests.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;

namespace MangaParser.Parsers.Tests
{
    [TestClass]
    public abstract class TestCore
    {
        #region Fields

        protected const string dataPath = "Data";

        #endregion Fields

        #region Properties

        public virtual TestContext TestContext { get; set; }
        protected virtual string ParserPath => String.Empty;

        protected abstract IParser GetNewParser { get; }

        #endregion Properties

        #region Methods

        [TestCleanup]
        public virtual void TestCleanup()
        {
            // After passing a test, we wait to avoid HTTP flooding and getting caught by DDoS detection.
            if (GetType().GetMethod(TestContext.TestName)?.GetCustomAttribute(typeof(TestMethodDelayAttribute)) is TestMethodDelayAttribute attribute)
            {
                System.Threading.Thread.Sleep(attribute.Delay);
            }
        }

        protected virtual string GetDataPath(string parserPath, string methodPath, string name, string extension = ".json")
        {
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", dataPath ?? String.Empty, parserPath ?? String.Empty, methodPath ?? String.Empty, name + extension ?? String.Empty));
        }

        #endregion Methods
    }
}