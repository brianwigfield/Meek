using System.Configuration;

namespace Meek.Configuration
{

    public class MeekConfigurationSection : ConfigurationSection
    {

        [ConfigurationProperty("repository", IsRequired=true)]
        public MeekConfigurationRepository Repository
        {
            get { return (MeekConfigurationRepository)this["repository"]; }
            set { this["repository"] = value; }
        }

        [ConfigurationProperty("altManageContentRoute", IsRequired = false)]
        public string AltManageContentRoute
        {
            get { return (string)this["altManageContentRoute"]; }
            set { this["altManageContentRoute"] = value; }
        }

        [ConfigurationProperty("ckEditorPath", IsRequired = false)]
        public string CkEditorPath
        {
            get { return (string)this["ckEditorPath"]; }
            set { this["ckEditorPath"] = value; }
        }

        [ConfigurationProperty("notFoundView", IsRequired = false)]
        public string NotFoundView
        {
            get { return (string)this["notFoundView"]; }
            set { this["notFoundView"] = value; }
        }

        [ConfigurationProperty("viewEngine", IsRequired = false)]
        public string ViewEngine
        {
            get { return (string)this["viewEngine"]; }
            set { this["viewEngine"] = value; }
        }

        [ConfigurationProperty("contentAdmin", IsRequired = false)]
        [ConfigurationCollection(typeof(ContentAdminCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public ContentAdminCollection ContentAdmin
        {
            get { return (ContentAdminCollection)this["contentAdmin"]; }
            set { this["contentAdmin"] = value; }
        }

        [ConfigurationProperty("aspxConfig", IsRequired = false)]
        public MeekConfigurationAspx AspxConfig
        {
            get { return (MeekConfigurationAspx)this["aspxConfig"]; }
            set { this["aspxConfig"] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public class MeekConfigurationRepository : ConfigurationElement
        {
            [ConfigurationProperty("type", IsRequired = true)]
            public string Type
            {
                get { return (string)this["type"]; }
                set { this["type"] = value; }
            }

            [ConfigurationProperty("source", IsRequired = false)]
            public string Source
            {
                get { return (string)this["source"]; }
                set { this["source"] = value; }
            }
        }

        public class MeekConfigurationAspx : ConfigurationElement
        {
            [ConfigurationProperty("masterPage", IsRequired = true)]
            public string MasterPage
            {
                get { return (string)this["masterPage"]; }
                set { this["masterPage"] = value; }
            }

            [ConfigurationProperty("contentPlaceHolderId", IsRequired = false)]
            public string ContentPlaceHolderId
            {
                get { return (string)this["contentPlaceHolderId"]; }
                set { this["contentPlaceHolderId"] = value; }
            }

            [ConfigurationProperty("includeFormTag", IsRequired = false, DefaultValue = true)]
            public bool IncludeFormTag
            {
                get { return (bool)this["includeFormTag"]; }
                set { this["includeFormTag"] = value; }
            }
        }

        public class ContentAdminCollection : ConfigurationElementCollection
        {
            protected override ConfigurationElement CreateNewElement()
            {
                return new RoleElement();
            }

            protected override object GetElementKey(ConfigurationElement element)
            {
                return (element as RoleElement).Role;
            }
        }

        public class RoleElement : ConfigurationSection
        {

            [ConfigurationProperty("role", IsRequired = true)]
            public string Role
            {
                get { return (string)this["role"]; }
                set { this["role"] = value; }
            }

        }


    }
}
