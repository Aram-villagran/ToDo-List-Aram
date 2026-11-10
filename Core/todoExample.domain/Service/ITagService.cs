using todoExample.domain.DTOs;

namespace todoExample.domain.Service
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetByUserAsync(Guid userId);
        Task<TagDto?> GetByIdAsync(Guid userId, Guid tagId);
        Task<TagDto> CreateAsync(Guid userId, TagCreateDto dto);
        Task<bool> DisableAsync(Guid userId, Guid tagId);
    }
}