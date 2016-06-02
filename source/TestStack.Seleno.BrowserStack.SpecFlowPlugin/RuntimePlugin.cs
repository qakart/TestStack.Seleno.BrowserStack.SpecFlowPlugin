using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.UnitTestProvider;
using TestStack.Seleno.BrowserStack.SpecFlowPlugin;

[assembly: RuntimePlugin(typeof(RuntimePlugin))]

namespace TestStack.Seleno.BrowserStack.SpecFlowPlugin
{
    public class RuntimePlugin : IRuntimePlugin
    {
        public void RegisterConfigurationDefaults(RuntimeConfiguration runtimeConfiguration)
        {

        }

        public void RegisterCustomizations(ObjectContainer container, RuntimeConfiguration runtimeConfiguration)
        {

        }

        public void RegisterDependencies(ObjectContainer container)
        {
            var runtimeProvider = new XUnit2RuntimeProvider();

            container.RegisterInstanceAs<IUnitTestRuntimeProvider>(runtimeProvider, "SeleniumXUnit");
        }

        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters)
        {
            throw new System.NotImplementedException();
        }
    }
}