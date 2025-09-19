using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Analysis_Middle_Server.DI;
using Analysis_Middle_Server.Manager.DBManager;
using Analysis_Middle_Server.TRD;
using Ninject;

namespace Analysis_Middle_Server
{
    internal class Program
    {
        private static IKernel kernel;
        static void Main(string[] args)
        {
            SetAnlysisStart();
        }

        private static void SetAnlysisStart()
        {
            kernel = new StandardKernel(new AppModuleClass());
            ///kernel.Get<DBManagerClass>().SetAnlysisAndStart();
        }
    }
}
