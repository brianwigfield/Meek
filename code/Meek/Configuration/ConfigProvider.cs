using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Meek.Configuration
{
    interface ConfigProvider
    {
        T Section<T>(string name) where T : ConfigurationSection;
    }
}
