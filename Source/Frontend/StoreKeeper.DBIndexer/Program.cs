using System;
using System.IO;
using CommonBase.Log;
using CommonBase.Log.Logs;
using StoreKeeper.Common;
using StoreKeeper.Server;

namespace StoreKeeper.DBIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            new ApplicationContext(new FileLog());

            ILogger logger = LogManager.GetLogger(typeof(Program));

            logger.Info("DBIndexer - start");

            try
            {
                Console.WriteLine();
                Console.Write("File ready? [y/n] : ");

                string line;

                do
                {
                    line = Console.ReadLine();
                } while (line != "y" && line != "n");

                ConnectionStringHolder.Initialize(@"Server=MATESC-PC\SQLSERVER2012;Initial Catalog=StoreKeeper;User ID=StoreKeeperUser;Password=Welcome1;Integrated Security=false;MultipleActiveResultSets=True");

                IDataManager dataManager = DataManagerFactory.CreateDataManager();
                dataManager.IndexDatabase(line == "y");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Info("DBIndexer - end");
            Console.WriteLine("Press ENTER to continue ...");
            Console.ReadLine();
        }
    }
}
