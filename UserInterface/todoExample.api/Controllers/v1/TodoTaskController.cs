using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using todoExample.api.Extensions;
using todoExample.domain.DTOs;
using todoExample.domain.Service;

namespace todoExample.api.Controllers.v1;

[ApiController]
[Authorize]
[Route("/api/v1/[controller]")]
public class TodoTaskController(ITodoTaskService todoTaskService, 
        ILogger<TodoTaskController> logger) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IReadOnlyList<TodoTaskDto>>>> GetByList([FromQuery] Guid listId)
    {
        try
        {
            var tasks = await todoTaskService.GetByListAsync(User.GetUserId(), listId);
            if (tasks is null)
            {
                return NotFound(new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
                {
                    Error = true,
                    Message = "Lista no encontrada."
                });
            }

            return Ok(new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
            {
                Error = false,
                Message = "Tareas obtenidas correctamente.",
                Data = tasks
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(GetByList));
            return StatusCode(500, new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
            {
                Error = true,
                Message = "Ocurrió un error inesperado."
            });
        }
    }
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponseDto<IReadOnlyList<TodoTaskDto>>>> Search(

        [FromQuery] bool? completed,
        [FromQuery] string? tag,
        [FromQuery] string? text)
    {
        try
        {
            var tasks = await todoTaskService.SearchAsync(User.GetUserId(), completed, tag, text);
            return Ok(new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
            {
                Error = false,
                Message = "Tareas obtenidas correctamente.",
                Data = tasks
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(Search));
            return StatusCode(500, new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
            {
                Error = true,
                Message = "Ocurrió un error inesperado."
            });
        }
    }

    [HttpGet("pending")]
    public async Task<ActionResult<ApiResponseDto<IReadOnlyList<TodoTaskDto>>>> GetPending([FromQuery] Guid? listId)
    {
        try
        {
            if (listId is null)
            {
                var userTasks = await todoTaskService.GetPendingByUserAsync(User.GetUserId());
                return Ok(new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
                {
                    Error = false,
                    Message = "Tareas pendientes obtenidas correctamente.",
                    Data = userTasks
                });
            }

            var listTasks = await todoTaskService.GetPendingByListAsync(User.GetUserId(), listId.Value);
            if (listTasks is null)
            {
                return NotFound(new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
                {
                    Error = true,
                    Message = "Lista no encontrada."
                });
            }

            return Ok(new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
            {
                Error = false,
                Message = "Tareas pendientes obtenidas correctamente.",
                Data = listTasks
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(GetPending));
            return StatusCode(500, new ApiResponseDto<IReadOnlyList<TodoTaskDto>>
            {
                Error = true,
                Message = "Ocurrió un error inesperado."
            });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<TodoTaskDto>>> GetById(Guid id)
    {
        try
        {
            var task = await todoTaskService.GetByIdAsync(User.GetUserId(), id);
            if (task is null)
            {
                return NotFound(new ApiResponseDto<TodoTaskDto>
                {
                    Error = true,
                    Message = "Tarea no encontrada."
                });
            }

            return Ok(new ApiResponseDto<TodoTaskDto>
            {
                Error = false,
                Message = "Tarea obtenida correctamente.",
                Data = task
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = ex.Message,
                Data = null
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(GetById));
            return StatusCode(500, new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = "Ocurrió un error inesperado."
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<TodoTaskDto>>> Create([FromQuery] Guid listId, TodoTaskCreateDto dto)
    {
        try
        {
            var created = await todoTaskService.CreateAsync(User.GetUserId(), listId, dto);
            if (created is null)
            {
                return NotFound(new ApiResponseDto<TodoTaskDto>
                {
                    Error = true,
                    Message = "Lista no encontrada."
                });
            }

            var response = new ApiResponseDto<TodoTaskDto>
            {
                Error = false,
                Message = "Tarea creada correctamente.",
                Data = created
            };
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = ex.Message,
                Data = null
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(Create));
            return StatusCode(500, new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = "Ocurrió un error inesperado."
            });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<TodoTaskDto>>> Update(Guid id, TodoTaskUpdateDto dto)
    {
        try
        {
            var updated = await todoTaskService.UpdateAsync(User.GetUserId(), id, dto);
            if (updated is null)
            {
                return NotFound(new ApiResponseDto<TodoTaskDto>
                {
                    Error = true,
                    Message = "Tarea no encontrada."
                });
            }

            return Ok(new ApiResponseDto<TodoTaskDto>
            {
                Error = false,
                Message = "Tarea actualizada correctamente.",
                Data = updated
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(Update));
            return StatusCode(500, new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = "Ocurrió un error inesperado."
            });
        }
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<ActionResult<ApiResponseDto<TodoTaskDto>>> Complete(Guid id)
    {
        try
        {
            var task = await todoTaskService.CompleteAsync(User.GetUserId(), id);
            if (task is null)
            {
                return NotFound(new ApiResponseDto<TodoTaskDto>
                {
                    Error = true,
                    Message = "Tarea no encontrada."
                });
            }

            return Ok(new ApiResponseDto<TodoTaskDto>
            {
                Error = false,
                Message = "Tarea completada correctamente.",
                Data = task
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(Complete));
            return StatusCode(500, new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = "Ocurrió un error inesperado."
            });
        }
    }

    [HttpPatch("{id:guid}/reopen")]
    public async Task<ActionResult<ApiResponseDto<TodoTaskDto>>> Reopen(Guid id)
    {
        try
        {
            var task = await todoTaskService.ReopenAsync(User.GetUserId(), id);
            if (task is null)
            {
                return NotFound(new ApiResponseDto<TodoTaskDto>
                {
                    Error = true,
                    Message = "Tarea no encontrada."
                });
            }

            return Ok(new ApiResponseDto<TodoTaskDto>
            {
                Error = false,
                Message = "Tarea reabierta correctamente.",
                Data = task
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(Reopen));
            return StatusCode(500, new ApiResponseDto<TodoTaskDto>
            {
                Error = true,
                Message = "Ocurrió un error inesperado."
            });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> Delete(Guid id)
    {
        try
        {
            var deleted = await todoTaskService.DeleteAsync(User.GetUserId(), id);
            if (!deleted)
            {
                return NotFound(new ApiResponseDto<bool>
                {
                    Error = true,
                    Message = "Tarea no encontrada."
                });
            }

            return Ok(new ApiResponseDto<bool>
            {
                Error = false,
                Message = "Tarea eliminada correctamente.",
                Data = true
            });
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
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(Delete));
            return StatusCode(500, new ApiResponseDto<bool>
            {
                Error = true,
                Message = "Ocurrió un error inesperado."
            });
        }
    }
}