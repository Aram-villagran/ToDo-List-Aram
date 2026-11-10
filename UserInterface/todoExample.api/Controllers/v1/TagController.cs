using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using todoExample.api.Extensions;
using todoExample.domain.DTOs;
using todoExample.domain.Service;

namespace todoExample.api.Controllers.v1;

[ApiController]
[Authorize]
[Route("/api/v1/[controller]")]
public class TagController(ITagService tagService, ILogger<TagController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<TagDto>>>> GetByUser()
    {
        try
        {
            var tags = await tagService.GetByUserAsync(User.GetUserId());
            return Ok(new ApiResponseDto<IEnumerable<TagDto>>
            {
                Error = false,
                Message = "Etiquetas obtenidas correctamente.",
                Data = tags
            });

        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<IEnumerable<TagDto>>
            { 
                Error = true,
                Message = ex.Message,
                Data = null
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<IEnumerable<TagDto>>
            {
                Error = true,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(GetByUser));
            return StatusCode(500, new ApiResponseDto<IEnumerable<TagDto>> 
            { 
                Error = true, 
                Message = "Ocurrió un error inesperado." 
            });

        }
    } 

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<TagDto>>> GetById(Guid id)
    {
        try
        {
            var tag = await tagService.GetByIdAsync(User.GetUserId(), id);
            if (tag is null)
            {
                return NotFound(new ApiResponseDto<TagDto> 
                { 
                    Error = true, 
                    Message = "Etiqueta no encontrada."
                });
            }

            return Ok(new ApiResponseDto<TagDto> 
            {
                Error = false, 
                Message = "Etiqueta obtenida correctamente.", 
                Data = tag 
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<TagDto> 
            { 
                Error = true,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<TagDto> 
            { 
                Error = true, 
                Message = ex.Message 
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(GetById));
            return StatusCode(500, new ApiResponseDto<TagDto> 
            {
                Error = true,
                Message = "Ocurrió un error inesperado."
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<TagDto>>> Create(TagCreateDto dto)
    {
        try
        {
            var created = await tagService.CreateAsync(User.GetUserId(), dto);
            var response = new ApiResponseDto<TagDto> 
            { Error = false,
                Message = "Etiqueta creada correctamente.", 
                Data = created 
            };
            return CreatedAtAction(nameof(GetById), new 
            {
                id = created.Id 
            }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<TagDto>
            { 
                Error = true,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<TagDto> 
            { 
                Error = true,
                Message = ex.Message 
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(Create));
            return StatusCode(500, new ApiResponseDto<TagDto> 
            { 
                Error = true, 
                Message = "Ocurrió un error inesperado."
            });
        }
    }

    [HttpPatch("{id:guid}/disable")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Disable(Guid id)
    {
        try
        {
            var disabled = await tagService.DisableAsync(User.GetUserId(), id);
            if (!disabled)
            {
                return NotFound(new ApiResponseDto<bool> 
                { 
                    Error = true, 
                    Message = "Etiqueta no encontrada."
                });
            }

            return Ok(new ApiResponseDto<bool>
            { 
                Error = false, 
                Message = "Etiqueta deshabilitada correctamente.", 
                Data = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<bool>
            { 
                Error = true,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<bool> 
            {
                Error = true, 
                Message = ex.Message 
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(Disable));
            return StatusCode(500, new ApiResponseDto<bool> 
            { 
                Error = true, 
                Message = "Ocurrió un error inesperado."
            });
        }
    }
}
