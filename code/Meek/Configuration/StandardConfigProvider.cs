using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Meek.Configuration
{
    class StandardConfigProvider : ConfigProvider
    {
        public T Section<T>(string name) where T : ConfigurationSection
        {
            return (T) (ConfigurationManager.GetSection(name));
        }
    }
}
