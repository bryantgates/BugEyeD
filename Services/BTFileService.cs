using BugEyeD.Models.Enums;
using BugEyeD.Services.Interfaces;

namespace BugEyeD.Services
{
    public class BTFileService : IBTFileService
    {
        private readonly string _defaultImage = "~/img/BEDDefault.png";
        private readonly string _defaultBTUserImageSrc = "~/img/BEDDefaultUser.png";
        private readonly string _defaultCompanyImageSrc = "~/img/BEDDefaultCompany.png";
        private readonly string _defaultProjectImageSrc = "~/img/BEDDefaultProject.png";

        public string ConvertByteArrayToFile(byte[]? fileData, string? extension, DefaultImage defaultImage)
        {
            if (fileData is null || string.IsNullOrEmpty(extension))
            {
                return defaultImage switch
                {
                    DefaultImage.BTUserImage => _defaultBTUserImageSrc,
                    DefaultImage.CompanyImage => _defaultCompanyImageSrc,
                    DefaultImage.ProjectImage => _defaultProjectImageSrc,
                    _ => _defaultImage,
                };
            }
            try
            {
                string? imageBase64Data = Convert.ToBase64String(fileData);
                imageBase64Data = string.Format($"data:{extension};base64,{imageBase64Data}");

                return string.Format($"data:{extension};base64,{Convert.ToBase64String(fileData)}");
            }
            catch
            {
                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                using MemoryStream memoryStream = new MemoryStream();
                await file!.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                memoryStream.Close();

                return byteFile;
            }
            catch
            {
                throw;
            }
        }
    }
}
