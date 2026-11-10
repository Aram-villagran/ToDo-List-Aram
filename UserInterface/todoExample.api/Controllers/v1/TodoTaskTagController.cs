using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using todoExample.api.Extensions;
using todoExample.domain.DTOs;
using todoExample.domain.Service;

namespace todoExample.api.Controllers.v1;

[ApiController]
[Authorize]
[Route("/api/v1")]
public class TodoTaskTagController(ITodoTaskTagService todoTaskTagService, ILogger<TodoTaskTagController> logger) : ControllerBase
{
    [HttpPost("TodoTask/{taskId:guid}/tags/{tagId:guid}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Assign(Guid taskId, Guid tagId)
    {
        try
        {
            var assigned = await todoTaskTagService.AssignAsync(User.GetUserId(), taskId, tagId);
            if (!assigned)
            {
                return NotFound(new ApiResponseDto<bool> 
                {
                    Error = true, 
                    Message = "Tarea o etiqueta no encontrada."
                });
            }

            return Ok(new ApiResponseDto<bool> { Error = false, 
                Message = "Etiqueta asignada correctamente.", 
                Data = true
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<bool> { Error = true, Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<bool> { Error = true, Message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(Assign));
            return StatusCode(500, new ApiResponseDto<bool> { Error = true, Message = "Ocurrió un error inesperado." });
        }
    }

    [HttpDelete("TodoTask/{taskId:guid}/tags/{tagId:guid}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Remove(Guid taskId, Guid tagId)
    {
        try
        {
            var removed = await todoTaskTagService.RemoveAsync(User.GetUserId(), taskId, tagId);
            if (!removed)
            {
                return NotFound(new ApiResponseDto<bool> { Error = true, Message = "Asignación no encontrada." });
            }

            return Ok(new ApiResponseDto<bool> { Error = false, Message = "Etiqueta eliminada correctamente.", Data = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<bool> { Error = true, Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<bool> { Error = true, Message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(Remove));
            return StatusCode(500, new ApiResponseDto<bool> { Error = true, Message = "Ocurrió un error inesperado." });
        }
    }

    [HttpGet("TodoTask/{taskId:guid}/tags")]
    public async Task<ActionResult<ApiResponseDto<IReadOnlyList<TagDto>>>> GetTagsByTask(Guid taskId)
    {
        try
        {
            var tags = await todoTaskTagService.GetTagsByTaskAsync(User.GetUserId(), taskId);
            if (tags is null)
            {
                return NotFound(new ApiResponseDto<IReadOnlyList<TagDto>> { Error = true, Message = "Tarea no encontrada." });
            }

            return Ok(new ApiResponseDto<IReadOnlyList<TagDto>> { Error = false, Message = "Etiquetas obtenidas correctamente.", Data = tags });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<IReadOnlyList<TagDto>> { Error = true, Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<IReadOnlyList<TagDto>> { Error = true, Message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(GetTagsByTask));
            return StatusCode(500, new ApiResponseDto<IReadOnlyList<TagDto>> { Error = true, Message = "Ocurrió un error inesperado." });
        }
    }

    [HttpGet("Tag/{tagId:guid}/tasks")]
    public async Task<ActionResult<ApiResponseDto<IReadOnlyList<TodoTaskDto>>>> GetTasksByTag(Guid tagId)
    {
        try
        {
            var tasks = await todoTaskTagService.GetTasksByTagAsync(User.GetUserId(), tagId);
            if (tasks is null)
            {
                return NotFound(new ApiResponseDto<IReadOnlyList<TodoTaskDto>> { Error = true, Message = "Etiqueta no encontrada." });
            }

            return Ok(new ApiResponseDto<IReadOnlyList<TodoTaskDto>> { Error = false, Message = "Tareas obtenidas correctamente.", Data = tasks });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<IReadOnlyList<TodoTaskDto>> { Error = true, Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<IReadOnlyList<TodoTaskDto>> { Error = true, Message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(GetTasksByTag));
            return StatusCode(500, new ApiResponseDto<IReadOnlyList<TodoTaskDto>> { Error = true, Message = "Ocurrió un error inesperado." });
        }
    }
}
