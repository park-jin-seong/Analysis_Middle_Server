using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;

namespace Analysis_Middle_Server.DI
{
    public class AppModuleClass : NinjectModule
    {
        public override void Load()
        {
        //    Bind<ISystemInfoManagerClass>().To<SystemInfoManagerClass>().InSingletonScope();
        //    Bind<IDBManagerClass>().To<DBManagerClass>().InSingletonScope();
        //    Bind<IAnalysisThreadManagerClass>().To<AnalysisThreadManagerClass>().InSingletonScope();
        //    Bind<ISenderThreadManagerClass>().To<SenderThreadManagerClass>().InSingletonScope();

        //    Bind<SystemInfoManagerClass>().ToSelf().InSingletonScope();
        //    Bind<DBManagerClass>().ToSelf().InSingletonScope();
        //    Bind<AnalysisThreadManagerClass>().ToSelf().InSingletonScope();
        //    Bind<SenderThreadManagerClass>().ToSelf().InSingletonScope();
        }
    }
}
