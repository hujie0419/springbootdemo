using System.Configuration;

namespace Tuhu.C.SyncProductPriceJob
{
    public class OpenApiConfigurationSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty Property = new ConfigurationProperty(string.Empty, typeof(OpenApiConfigurationElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public OpenApiConfigurationElementCollection OpenApiConfig => (OpenApiConfigurationElementCollection)base[Property];
    }

    [ConfigurationCollection(typeof(OpenApiConfigurationElement))]
    public class OpenApiConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new OpenApiConfigurationElement();

        protected override object GetElementKey(ConfigurationElement element) => ((OpenApiConfigurationElement)element).Name;

        public OpenApiConfigurationElement Get(string name) => BaseGet(name) as OpenApiConfigurationElement;
    }

    public class OpenApiConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"].ToString(); }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("appKey", IsRequired = true)]
        public string AppKey
        {
            get { return this["appKey"].ToString(); }
            set { this["appKey"] = value; }
        }

        [ConfigurationProperty("appSecret", IsRequired = true)]
        public string AppSecret
        {
            get { return this["appSecret"].ToString(); }
            set { this["appSecret"] = value; }
        }

        [ConfigurationProperty("apiUrl", IsRequired = true)]
        public string ApiUrl
        {
            get { return this["apiUrl"].ToString(); }
            set { this["apiUrl"] = value; }
        }

        [ConfigurationProperty("authUrl", IsRequired = false)]
        public string AuthUrl
        {
            get { return this["authUrl"].ToString(); }
            set { this["authUrl"] = value; }
        }
    }
}
