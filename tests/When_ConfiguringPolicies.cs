using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.PolicyInjection;
using Unity.Interception.PolicyInjection.Policies;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringPolicies
    /// </summary>
    [TestClass]
    public class When_ConfiguringPolicies : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringPolicies()
            : base("Policies")
        {
        }

        private IUnityContainer GetConfiguredContainer(string containerName)
        {
            IUnityContainer container = new UnityContainer();
            section.Configure(container, containerName);
            return container;
        }

        [TestMethod]
        public void Then_CanConfigureAnEmptyPolicy()
        {
            IUnityContainer container = this.GetConfiguredContainer("oneEmptyPolicy");

            var policies = new List<InjectionPolicy>(container.ResolveAll<InjectionPolicy>());

            Assert.AreEqual(2, policies.Count);
            Assert.IsInstanceOfType(policies[0], typeof(AttributeDrivenPolicy));
            Assert.IsInstanceOfType(policies[1], typeof(RuleDrivenPolicy));
            Assert.AreEqual("policyOne", policies[1].Name);
        }

    }
}
