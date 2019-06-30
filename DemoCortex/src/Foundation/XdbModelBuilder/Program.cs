using System;
using Demo.Foundation.ProcessingEngine.Models.XConnect;
using Sitecore.XConnect.Schema;
using Sitecore.XConnect.Serialization;

namespace XdbModelBuilder
{
    class Program
    {
        /*

    - According to the xConnect documentation, the name of the json-model file should exactly be: "{Fully_Qualified_Model_Name}, {Major}.{Minor}.json".

    - The version should math the XdbModelVersion from the concerning model.

    - Once the json-model file has been created, it should be deployed to:
      * C:\inetpub\wwwroot\XConnect\App_data\Models\
      * C:\inetpub\wwwroot\XConnect\App_data\jobs\continuous\IndexWorker\App_data\Models\

   */

        static void Main(string[] args)
        {
            CreateJsonModelForFacetModel(XdbPurchaseModel.Model);
            CreateJsonModelForContactFacetModel(XdbPurchaseContactModel.Model);

            Console.ReadLine();
        }

        private static void CreateJsonModelForFacetModel(XdbModel model)
        {
            try
            {
                var fileName = XdbPurchaseModel.Model.FullName + ".json";
                var json = XdbModelWriter.Serialize(model);
                System.IO.File.WriteAllText(fileName, json);

                Console.WriteLine($"Json-model file successfully created for {model.GetType()}.{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while creating json-model for {model.GetType()}: {ex.Message}{Environment.NewLine}");
                Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}Make sure that the console app is launched WITHIN the bin-folder of the RAI-wwwRoot folder!{Environment.NewLine}");
            }

            Console.WriteLine($"--------------------------------------------------{Environment.NewLine}");
        }

        private static void CreateJsonModelForContactFacetModel(XdbModel model)
        {
            try
            {
                var fileName = XdbPurchaseContactModel.Model.FullName + ".json";
                Console.WriteLine($"Creating json-model for {model.GetType()}: '{fileName}'.{Environment.NewLine}");

                var json = XdbModelWriter.Serialize(model);

                System.IO.File.WriteAllText(fileName, json);

                Console.WriteLine($"Json-model file successfully created for {model.GetType()}.{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while creating json-model for {model.GetType()}: {ex.Message}{Environment.NewLine}");
                Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}Make sure that the console app is launched WITHIN the bin-folder of the RAI-wwwRoot folder!{Environment.NewLine}");
            }

            Console.WriteLine($"--------------------------------------------------{Environment.NewLine}");
        }
    }
}
