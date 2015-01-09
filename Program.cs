using System.ServiceProcess;

namespace AssetsCopier
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
          var servicesToRun = new ServiceBase[] 
            { 
              new AssetsCopier() 
            };
          ServiceBase.Run(servicesToRun);
        }
    }
}
