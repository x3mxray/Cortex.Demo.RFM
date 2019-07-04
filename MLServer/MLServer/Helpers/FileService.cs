using System;
using System.Collections.Generic;
using System.IO;
using MLServer.Models;

namespace MLServer.Helpers
{
    public class FileService
    {
        public void ExportToCsv(List<TestCase> list, string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                filePath = "rfm_clusters_" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")+".csv";
            }

            using (var file = File.CreateText(filePath))
            {
                file.WriteLine("Cluster,R,F,M");
                foreach (var arr in list)
                {
                    file.WriteLine($"{arr.Cluster},{arr.Data.R},{arr.Data.F},{arr.Data.M}");
                }
            }
        }
    }
}
