using AIChatWebServer.DTO.Response;
using AIChatWebServer.Models.Files;
using AIChatWebServer.Utils.Interfaces.Mapper;

namespace AIChatWebServer.Utils.Implementations.Mappers
{
    public class UploadSessionResponseMapper(ICollectionResponseMapper<UploadSessionFile, UploadSessionFileResponse> fileMapper) : IResponseMapper<UploadSession, UploadSessionResponse>
    {
        private readonly ICollectionResponseMapper<UploadSessionFile, UploadSessionFileResponse> _fileMapper = fileMapper;

        public UploadSessionResponse ToResponse(UploadSession model)
        {
            return new UploadSessionResponse(model.Id, _fileMapper.ToResponse(model.Files));
        }
    }
}
