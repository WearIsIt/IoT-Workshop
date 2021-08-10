using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;

namespace CustomerApp.Services
{
    public class BlobServices
    {
        static CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=projectstorageaccount0;AccountKey=3tz3yTsXHKBMAhiXuImOSzXXt3OOf2K39Fy+WqQowSRj2HviBUe8amvunXsgQ3laSLCq/HcAYjt42szRcc8j6Q==;EndpointSuffix=core.windows.net");
        public static string imageNameSuffixIncluded = null;
        public static string imageType = null;
        public static CloudBlockBlob currBlob=null;


        public static bool CheckIfExistsInBlob(string item)
        {
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference("images");
            var blobs = cloudBlobClient.ListBlobs().OfType<CloudBlockBlob>().ToList();
            foreach (var blob in blobs)
            {
                if (blob.Name.Contains(item))
                {
                    imageNameSuffixIncluded = blob.Name;
                    imageType = getImageTye(blob);

                    // Set type
                    blob.Properties.ContentType = "image" + "/" + imageType;
                    currBlob = blob;

                    return true;
                }
            }

            return false;
        }


        public static string getImageFullname(string item)
        {
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference("images");
            var blobs = cloudBlobClient.ListBlobs().OfType<CloudBlockBlob>().ToList();
            foreach (var blob in blobs)
            {
                if (blob.Name.Contains(item))
                {
                    return blob.Name;
                }
            }

            return null;
        }


        public static string getImageTye(CloudBlockBlob blob)
        {
            int lenFullName = blob.Name.Length;
            for (int i=0; i< lenFullName; i++)
            {
                if (blob.Name[i]=='.')
                {
                    return blob.Name.Substring(i, lenFullName - i);
                }
            }

            return null;
        }


        public static string getImageFullName(string itemName)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=projectstorageaccount0;AccountKey=3tz3yTsXHKBMAhiXuImOSzXXt3OOf2K39Fy+WqQowSRj2HviBUe8amvunXsgQ3laSLCq/HcAYjt42szRcc8j6Q==;EndpointSuffix=core.windows.net");
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference("images");

            var blobs = cloudBlobClient.ListBlobs().OfType<CloudBlockBlob>().ToList();
            foreach (var blob in blobs)
            {
                if (blob.Name.Contains(itemName))
                {
                    return blob.Name;
                }
            }

            return null; // Only if itemName has no image
        }


    }
}
