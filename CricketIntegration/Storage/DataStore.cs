using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CricketIntegration.CricketApi;
using CricketIntegration.CricketApi.Cricinfo;
using LiteDB;

namespace CricketIntegration.Storage
{
    public static class DataStore
    {
        // TODO: Clear out data after it has gone stale
        // TODO: Allow for multiple matches to be stored simultaneously

        public static CricinfoMatchDetails GetLastStore()
        {
            using (var db = new LiteDatabase(@"C:\Temp\Cricket.db"))
            {
                var col = db.GetCollection<CricinfoMatchDetails>("score");
                return col.FindAll().OrderByDescending(x => x.RetrievedDate).FirstOrDefault();
            }
        }

        public static void StoreNew(CricinfoMatchDetails current)
        {
            using (var db = new LiteDatabase(@"C:\Temp\Cricket.db"))
            {
                // Get customer collection
                var col = db.GetCollection<CricinfoMatchDetails>("score");

                // Insert new customer document (Id will be auto-incremented)
                col.Insert(current);
            }
        }
    }
}
