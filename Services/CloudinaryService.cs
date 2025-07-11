using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Google.Protobuf.Reflection;

public class CloudinaryService
{
    private readonly Cloudinary cloudinary;

    public CloudinaryService()
    {
        string cloudName = "dko6mdw2i"; 
        string apiKey = "772159427754568"; 
        string apiSecret = "terBVTW9cFaETm3pqx6LqcXUjZc";

        if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
        {
            throw new Exception("Thông tin tài khoản Cloudinary chưa được cấu hình.");
        }

        Account account = new Account(cloudName, apiKey, apiSecret);
        cloudinary = new Cloudinary(account);
    }



    public async Task<string> UploadImageToCloudinary(string localImagePath)
    {
        try
        {
            if (string.IsNullOrEmpty(localImagePath) || !System.IO.File.Exists(localImagePath))
            {
                throw new Exception("File ảnh không tồn tại.");
            }

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(localImagePath)  
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return uploadResult.SecureUrl.ToString();
            }
            else
            {
                throw new Exception($"Lỗi khi upload ảnh lên Cloudinary: {uploadResult.Error?.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi: {ex.Message}", "Lỗi Upload Ảnh", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Console.WriteLine($"Error: {ex.Message}");

            return null;
        }
    }

}