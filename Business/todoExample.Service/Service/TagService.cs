using Microsoft.AspNetCore.Identity;
using todoExample.domain.DTOs;
using todoExample.domain.Models.Identity;
using todoExample.domain.Respository.Base;
using todoExample.domain.Service;

namespace todoExample.Service.Service
{
    public class TagService : ITagService
    {
        private readonly IRepository<TagModel> _tagRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TagService(
            IRepository<TagModel> tagRepository,
            UserManager<ApplicationUser> userManager)
        {
            _tagRepository = tagRepository;
            _userManager = userManager;
        }

        public async Task<IEnumerable<TagDto>> GetByUserAsync(Guid userId)
        {
            var tags = await _tagRepository.GetAllAsync(
                t => t.UserId == userId,
                q => q.OrderBy(t => t.Name));

            return tags.Select(ToDto);
        }

        public async Task<TagDto?> GetByIdAsync(Guid userId, Guid tagId)
        {
            var tag = await _tagRepository.GetByIdAsync(tagId);

            if (tag == null || tag.UserId != userId)
            {
                return null;
            }

            return ToDto(tag);
        }

        public async Task<TagDto> CreateAsync(Guid userId, TagCreateDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new InvalidOperationException("El usuario no existe.");
            }

            if (!user.Enabled)
            {
                throw new InvalidOperationException("El usuario esta deshabilitado");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new InvalidOperationException("El nombre del tag es obligatorio");
            }

            var name = dto.Name.Trim();

            var exists = await _tagRepository.AnyAsync(t => t.UserId == userId && t.Name == name);

            if (exists)
            {
                throw new InvalidOperationException("Ya existe un tag activo con ese nombre para el usuario.");
            }

            var model = new TagModel
            {
                UserId = userId,
                Name = name,
                Enabled = true
            };

            await _tagRepository.AddAsync(model);

            return ToDto(model);
        }

        public async Task<bool> DisableAsync(Guid userId, Guid tagId)
        {
            var tag = await _tagRepository.GetByIdAsync(tagId);

            if (tag == null || tag.UserId != userId)
            {
                return false;
            }

            await _tagRepository.DeleteAsync(tag);

            return true;
        }

        private static TagDto ToDto(TagModel model) => new()
        {
            Id = model.Id,
            UserId = model.UserId,
            Name = model.Name,
            CreatedAt = model.CreatedAt,
            Enabled = model.Enabled
        };
    }
}