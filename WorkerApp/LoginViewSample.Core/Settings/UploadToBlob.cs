using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using Plugin.Media.Abstractions;

namespace LoginViewSample.Core.Settings
{
    public class UploadToBlob 
    {
        CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=projectstorageaccount0;AccountKey=3tz3yTsXHKBMAhiXuImOSzXXt3OOf2K39Fy+WqQowSRj2HviBUe8amvunXsgQ3laSLCq/HcAYjt42szRcc8j6Q==;EndpointSuffix=core.windows.net");
        public static string imageNameSuffixIncluded = null;
        public static string imageType = null;
        public static CloudBlockBlob currBlob = null;

        public async Task UploadToAzureAsync(MediaFile file, string fileName)
        {
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference("images");

            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Off });
            }
            string[] parts = file.Path.ToString().Split('.');
            string fileType = parts[parts.Length - 1];

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName+"." + fileType);
            cloudBlockBlob.Properties.ContentType = "image/" + fileType;
            await cloudBlockBlob.UploadFromStreamAsync(file.GetStream());
        }


        public static bool CheckIfExistsInBlob(string item)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=projectstorageaccount0;AccountKey=3tz3yTsXHKBMAhiXuImOSzXXt3OOf2K39Fy+WqQowSRj2HviBUe8amvunXsgQ3laSLCq/HcAYjt42szRcc8j6Q==;EndpointSuffix=core.windows.net");
            var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient().GetContainerReference("images");

            var blobs = cloudBlobClient.ListBlobs().OfType<CloudBlockBlob>().ToList();
            foreach (var blob in blobs)
            {
                string f = blob.Name;
                if (blob.Name.Contains(item))              
                {
                    imageNameSuffixIncluded = blob.Name;
                    imageType = getImageTye(blob);

                    // set type
                    blob.Properties.ContentType = "image" + "/" + imageType;

                    currBlob = blob;
                    return true;
                }
            }

            return false;
        }


        public static string getImageTye(CloudBlockBlob blob)
        {
            int lenFullName = blob.Name.Length;
            for (int i = 0; i < lenFullName; i++)
            {
                if (blob.Name[i] == '.')
                {
                    return blob.Name.Substring(i, lenFullName - i);
                }
            }
            return null;
        }


    }
}
