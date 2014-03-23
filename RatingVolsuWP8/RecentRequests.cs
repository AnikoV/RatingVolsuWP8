using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RatingVolsuWP8
{
    static public class RecentRequests
    {
        public async static void SaveRequests(List<SavedRequest> objList)
        {
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream isoStream =
                new IsolatedStorageFileStream("template.json", FileMode.Create, FileAccess.Write, isoFile);
            StreamWriter writer = new StreamWriter(isoStream);

            string json = JsonConvert.SerializeObject(objList);
            await writer.WriteAsync(json);
        }

        public async static Task<string> ReadSavedRequests()
        {
            IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream isoStream =
                new IsolatedStorageFileStream("template.json", FileMode.Open, FileAccess.Read, isoFile);
            StreamReader reader = new StreamReader(isoStream);
            string lineOfData = String.Empty;
            string str = String.Empty;
            while ((lineOfData = reader.ReadLine()) != null)
                str += lineOfData;
            var list = JsonConvert.DeserializeObject<List<SavedRequest>>(str);
            return str;
        }


    }
}
