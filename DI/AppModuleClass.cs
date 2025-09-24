using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analysis_Middle_Server.Manager;
using Analysis_Middle_Server.Manager.DBManager;
using Analysis_Middle_Server.Manager.RenderManager;
using Analysis_Middle_Server.Manager.StreamManager;
using Analysis_Middle_Server.Manager.Systeminfomanager;
using Analysis_Middle_Server.Manager.SystemInfoManager;
using Ninject.Modules;

namespace Analysis_Middle_Server.DI
{
    public class AppModuleClass : NinjectModule
    {
        public override void Load()
        {
            Bind<ISystemInfoManagerClass>().To<SystemInfoManagerClass>().InSingletonScope();
            Bind<IDBManagerClass>().To<DBManagerClass>().InSingletonScope();
            Bind<IStreamManger>().To<StreamManger>().InSingletonScope();
            Bind<IAnalysisReceiverManger>().To<AnalysisReceiverManger>().InSingletonScope();
            Bind<IRenderManager>().To<RenderManager>().InSingletonScope();
            Bind<ISocketManagerClass>().To<SocketManagerClass>().InSingletonScope();


            Bind<SystemInfoManagerClass>().ToSelf().InSingletonScope();
            Bind<DBManagerClass>().ToSelf().InSingletonScope();
            Bind<StreamManger>().ToSelf().InSingletonScope();
            Bind<AnalysisReceiverManger>().ToSelf().InSingletonScope();

        }
    }
}
