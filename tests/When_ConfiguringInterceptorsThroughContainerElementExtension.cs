using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Remoting;
using Unity;
using Unity.Interception;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringInterceptorsThroughContainerElementExtension
    /// </summary>
    [TestClass]
    public class When_ConfiguringInterceptorsThroughContainerElementExtension : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringInterceptorsThroughContainerElementExtension()
            : base("InterceptorsThroughContainerElementExtension")
        {
        }

        private IUnityContainer GetContainer(string containerName)
        {
            return new UnityContainer()
                .LoadConfiguration(section, containerName)
                .Configure<Interception>()
                .AddPolicy("policy")
                .AddMatchingRule<AlwaysMatchingRule>()
                .AddCallHandler<CallCountHandler>()
                .Container;
        }

        [TestMethod]
        public void Then_CanConfigureVirtualMethodInterceptor()
        {
            IUnityContainer container = this.GetContainer("configuringDefaultInterceptorForTypeWithVirtualMethodInterceptor");

            var anonymous = container.Resolve<WrappableWithVirtualMethods>();
            var named = container.Resolve<WrappableWithVirtualMethods>("name");

            Assert.AreSame(typeof(WrappableWithVirtualMethods), anonymous.GetType().BaseType);
            Assert.AreSame(typeof(WrappableWithVirtualMethods), named.GetType().BaseType);
        }
    }
}
