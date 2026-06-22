using AIChatWebServer.DTO.Request;
using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Files;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class UploadSessionFileMapper : IMapper<UploadSessionFileRequest, UploadSessionFile, UploadSessionFileResponse>
    {
        public UploadSessionFile ToModel(UploadSessionFileRequest request)
        {
            return UploadSessionFile.Create(
                request.Id,
                request.ExpectedFileName, 
                request.ExpectedFileType, 
                request.ExpectedFileSize, 
                DateTime.UtcNow);
        }

        public UploadSessionFileResponse ToResponse(UploadSessionFile model)
        {
            return new UploadSessionFileResponse(
                model.Id, 
                model.ExpectedFileName,
                model.ExpectedFileType, 
                model.ExpectedFileSize);
        }
    }
}
