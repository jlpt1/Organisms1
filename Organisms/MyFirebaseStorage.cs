using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Organisms
{
    class MyFirebaseStorage
    {
        string jsonPath = "C:\\Users\\kcjan\\source\\repos\\Organisms1351\\Organisms\\Key\\evosim-821ac-firebase-adminsdk-oywci-8205d40a23.json";
        string bucket = "evosim-821ac.appspot.com";  // Your Firebase storage bucket

        public MyFirebaseStorage()
        {
            Debug.WriteLine("Firebase Storage client initialized.");
        }

        public async Task UploadFileAsync(string localPath, string destinationPath)
        {
            Debug.WriteLine("Starting file upload...");

            // Create a FirebaseStorage instance using the default app credentials
            var storage = new FirebaseStorage(bucket);

            // Upload the file
            var task = storage.Child("files").Child(destinationPath).PutAsync(File.OpenRead(localPath));

            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Debug.WriteLine($"Progress: {e.Percentage} %");

            // Await the task to get the download URL
            var downloadUrl = await task;

            Debug.WriteLine($"File uploaded successfully: {downloadUrl}");
        }
        public async Task DownloadFileAsync(string storagePath, string localPath)
        {
            Debug.WriteLine("Starting file download...");
            var storage = new FirebaseStorage(bucket);
            var task = storage.Child("files").Child(storagePath).GetDownloadUrlAsync();
            var url = await task;  // Gets the download URL
            
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var fileBytes = await httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(localPath, fileBytes);
            }
            Debug.WriteLine($"File downloaded successfully to {localPath}");
        }

      


    }
}
