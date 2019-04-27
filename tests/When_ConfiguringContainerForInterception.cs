using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.ConfigFiles;
using Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests.TestObjects;
using Microsoft.Practices.Unity.TestSupport;
using Microsoft.Practices.Unity.TestSupport.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Runtime.Remoting;
using Unity;
using Unity.Interception;
using Unity.Interception.InterceptionBehaviors;

namespace Microsoft.Practices.Unity.InterceptionExtension.Configuration.Tests
{
    /// <summary>
    /// Summary description for When_ConfiguringContainerForInterception
    /// </summary>
    [TestClass]
    public class When_ConfiguringContainerForInterception : SectionLoadingFixture<ConfigFileLocator>
    {
        public When_ConfiguringContainerForInterception()
            : base("InterceptionInjectionMembers")
        {
        }

        protected override void Arrange()
        {
            GlobalCountInterceptionBehavior.Calls.Clear();
            base.Arrange();
        }

        protected override void Teardown()
        {
            GlobalCountInterceptionBehavior.Calls.Clear();
            base.Teardown();
        }

        private IUnityContainer ConfiguredContainer(string containerName)
        {
            return new UnityContainer().LoadConfiguration(this.section, containerName);
        }

        [TestMethod]
        public void Then_CanConfigureInterceptorThroughConfigurationFile()
        {
            var container = this.ConfiguredContainer("configuringInterceptorThroughConfigurationFile");

            var instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, GlobalCountInterceptionBehavior.Calls.Values.First());
        }

        [TestMethod]
        public void Then_CanConfigureAdditionalInterfaceThroughConfigurationFile()
        {
            IUnityContainer container = this.ConfiguredContainer("configuringAdditionalInterfaceThroughConfigurationFile");

            var instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, GlobalCountInterceptionBehavior.Calls.Values.First());
            Assert.IsTrue(instance is IServiceProvider);
        }

        [TestMethod]
        public void Then_CanConfigureResolvedInterceptionBehavior()
        {
            IUnityContainer container =
                this.ConfiguredContainer("configuringInterceptionBehaviorWithTypeThroughConfigurationFile");

            var instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, GlobalCountInterceptionBehavior.Calls.Values.First());
        }

        [TestMethod]
        public void Then_CanConfigureNamedResolvedBehavior()
        {
            IUnityContainer container = this.ConfiguredContainer("canConfigureNamedBehavior");

            var instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, GlobalCountInterceptionBehavior.Calls.Values.First());
        }

        [TestMethod]
        public void Then_CanConfigureBehaviorWithNameOnly()
        {
            var callCount = new CallCountInterceptionBehavior();

            IUnityContainer container = this.ConfiguredContainer("canConfigureBehaviorWithNameOnly")
                .RegisterInstance<IInterceptionBehavior>("call count", callCount);

            var instance = container.Resolve<Interceptable>();
            instance.DoSomething();

            Assert.AreEqual(1, callCount.CallCount);
        }

        [TestMethod]
        [Ignore]
        public void Then_CanConfigureDefaultInterceptor()
        {
            IUnityContainer container = this.ConfiguredContainer("configuringDefaultInterceptor");
            container.Configure<Interception>()
                     .AddPolicy("all")
                     .AddMatchingRule<AlwaysMatchingRule>()
                     .AddCallHandler<GlobalCountCallHandler>();

            var instance1 = container.Resolve<Wrappable>();
            var instance2 = container.Resolve<Wrappable>("two");

            Assert.IsTrue(RemotingServices.IsTransparentProxy(instance1));
            Assert.IsTrue(RemotingServices.IsTransparentProxy(instance2));
        }

        [TestMethod]
        public void Then_CanAddInterfaceThroughConfiguredBehavior()
        {
            IUnityContainer container = this.ConfiguredContainer("addingInterfacesImplicitlyThroughBehavior");

            var instance = container.Resolve<Interceptable>();
            Assert.IsNotNull(instance as IAdditionalInterface);
        }

        [TestMethod]
        public void Then_CanAddInterfaceThroughExplicitConfiguration()
        {
            IUnityContainer container = this.ConfiguredContainer("addingInterfacesExplicitlyWithBehavior");

            var instance = container.Resolve<Interceptable>();
            Assert.IsNotNull(instance as IAdditionalInterface);
        }

        [TestMethod]
        public void Then_MultipleBehaviorsCanBeConfigured()
        {
            var container = this.ConfiguredContainer("multipleBehaviorsOnOneRegistration");
            var instance = container.Resolve<Interceptable>();

            instance.DoSomething();

            var countHandler = (CallCountInterceptionBehavior)container.Resolve<IInterceptionBehavior>("fixed");

            Assert.AreEqual(1, countHandler.CallCount);

            Assert.IsNotNull(instance as IAdditionalInterface);
        }
    }
}
