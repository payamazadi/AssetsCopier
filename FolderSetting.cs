using System;
using System.Configuration;

namespace AssetsCopier
{
    //http://msdn.microsoft.com/en-us/library/system.configuration.configurationelement.aspx

    //Section to wrap the directories
    public class FoldersSection : ConfigurationSection
    {
        [ConfigurationProperty("directories", IsDefaultCollection = false)]
        public FoldersSetting Directories
        {
            get { return (FoldersSetting)base["directories"]; }
        }
    }

    // Collection of paths
    [ConfigurationCollection(typeof(DirectorySetting), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class FoldersSetting : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DirectorySetting();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((DirectorySetting)element).Path;
        }

        public new int Count
        {
            get { return base.Count; }
        }
    }

    // <add path="C:\folder" filter="*.txt|*.html"/>
    public class DirectorySetting : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true, IsKey = true)]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }

        [ConfigurationProperty("filter", IsRequired = false, IsKey = false)]
        public string Filter
        {
            get { return (string)this["filter"]; }
            set { this["filter"] = value; }
        }

        [ConfigurationProperty("prepend", IsRequired = false, IsKey = false)]
        public string Prepend
        {
            get { return (string)this["prepend"]; }
            set { this["prepend"] = value; }
        }

				[ConfigurationProperty("destinationRoot", IsRequired = false, IsKey = false)]
				public string DestinationRoot
				{
					get { return (string)this["destinationRoot"]; }
					set { this["destinationRoot"] = value; }
				}

        public DirectorySetting() { }
        public DirectorySetting(String newPath, String newFilter)
        {
            Path = newPath;
            Filter = newFilter;
        }
    }
}
