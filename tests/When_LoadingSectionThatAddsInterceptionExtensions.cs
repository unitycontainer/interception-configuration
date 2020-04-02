

using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Configuration;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    [TestClass]
    public class When_LoadingSectionThatAddsInterceptionExtensions : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_LoadingSectionThatAddsInterceptionExtensions()
            : base("SectionExtensionBasics")
        {
        }

        [TestMethod]
        public void Then_SectionExtensionIsPresent()
        {
            Assert.IsInstanceOfType(section.SectionExtensions[0].ExtensionObject,
                typeof(InterceptionConfigurationExtension));
        }

        [TestMethod]
        public void Then_InterceptionElementHasBeenAdded()
        {
            Assert.IsNotNull(ExtensionElementMap.GetContainerConfiguringElementType("interception"));
        }
    }
}
