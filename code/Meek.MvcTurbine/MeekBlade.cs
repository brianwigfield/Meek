using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MvcTurbine;
using MvcTurbine.Blades;
using MvcTurbine.ComponentModel;

namespace Meek.MvcTurbine
{
    public class MeekBlade : IBlade
    {
        public void Dispose()
        {
        }

        public void Initialize(IRotorContext context)
        {
            Configuration.Configuration config = null;

            try
            {
                config = context.ServiceLocator.Resolve<Configuration.Configuration>();
            }
            catch (Exception)
            {
            }

            if (config != null)
                BootStrapper.Initialize(config);
            else
                BootStrapper.Initialize();
        }

        public void Spin(IRotorContext context)
        {
        }

    }
}
