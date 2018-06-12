using System.Configuration;

namespace ConfigLogService
{
    public class MessageConsumerSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty Property = new ConfigurationProperty(string.Empty, typeof(MessageConsumerConfigurationElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public MessageConsumerConfigurationElementCollection MessageConsumers => (MessageConsumerConfigurationElementCollection)base[Property];
    }

    [ConfigurationCollection(typeof(MessageConsumerConfigurationElement))]
    public class MessageConsumerConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new MessageConsumerConfigurationElement();

        protected override object GetElementKey(ConfigurationElement element) => ((MessageConsumerConfigurationElement)element).Name;

        public MessageConsumerConfigurationElement Get(string name) => BaseGet(name) as MessageConsumerConfigurationElement;
    }
    public class MessageConsumerConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"].ToString(); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return this["type"].ToString(); }
            set { this["type"] = value; }
        }
    }
}
