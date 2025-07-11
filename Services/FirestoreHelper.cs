using Google.Cloud.Firestore;
using Google.Api.Gax.Grpc;
using Grpc.Core;
using System.IO;
using System;
using Google.Cloud.Firestore.V1;

namespace WordUp.Helpers
{
    public static class FirestoreHelper
    {
        private static FirestoreDb _db;

        public static FirestoreDb GetDb()
        {
            if (_db != null)
                return _db;

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "serviceAccountKey.json");

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Đường dẫn key bị null hoặc rỗng.");

            if (!File.Exists(path))
                throw new FileNotFoundException("Không tìm thấy file serviceAccountKey.json", path);

            var builder = new FirestoreClientBuilder
            {
                CredentialsPath = path
            };

            var client = builder.Build(); // tạo client từ credential
            _db = FirestoreDb.Create("ltm-wu", client); // tạo db với client

            return _db;
        }
    }
}