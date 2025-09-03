using RTSAct2015Services.Models.DTOs;

namespace RTSAct2015Services.Services.Interfaces
{
    public interface ITrackApplicationService
    {
        Task<TrackApplicationResponseDto> TrackApplicationAsync(TrackApplicationDto request);
    }
}
