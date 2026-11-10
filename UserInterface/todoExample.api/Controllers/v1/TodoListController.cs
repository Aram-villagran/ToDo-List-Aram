using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using todoExample.api.Extensions;
using todoExample.domain.DTOs;
using todoExample.domain.Service;

namespace todoExample.api.Controllers.v1;

[ApiController]
[Authorize]
[Route("/api/v1/[controller]")]
public class TodoListController(ITodoListService todoListService, ILogger<TodoListController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<TodoListDto>>>> GetByUser()
    {
        try
        {
            var lists = await todoListService.GetByUserAsync(User.GetUserId());
            return Ok(new ApiResponseDto<IEnumerable<TodoListDto>> { Error = false, Message = "Listas obtenidas correctamente.", Data = lists });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<IEnumerable<TodoListDto>> { Error = true, Message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<IEnumerable<TodoListDto>> { Error = true, Message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(GetByUser));
            return StatusCode(500, new ApiResponseDto<IEnumerable<TodoListDto>> { Error = true, Message = "Ocurrió un error inesperado." });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<TodoListDto>>> GetById(Guid id)
    {
        try
        {
            var list = await todoListService.GetByIdAsync(User.GetUserId(), id);
            if (list is null)
            {
                return NotFound(new ApiResponseDto<TodoListDto>
                {
                    Error = true,
                    Message = "Lista no encontrada."
                });
            }


            //http://localhost:5031/api/v1/

            //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdWQiOiJ0b2RvRXhhbXBsZS5jbGllbnRzIiwiaXNzIjoidG9kb0V4YW1wbGUuYXBpIiwiZXhwIjoxNzkzMzE2MTY5LCJpYXQiOjE3OTMzMDUzNjksIm5iZiI6MTc5MzMwNTM2OSwic3ViIjoiOGIzZTRjMGEtNWQ2Zi00MDcxLTljMzItM2Q0ZTVmNjA3MTgyIiwiZW1haWwiOiJkb2VAc2FjYWkuY29tLm14IiwianRpIjoiNTNiYTBhMTktODY5Zi00Y2JhLWI5OGUtMGJiMzg2YWFhMTNjIiwiZ2l2ZW5fbmFtZSI6IkpvaG4iLCJmYW1pbHlfbmFtZSI6IkRvZSIsInJvbGUiOiJhZG1pbiJ9.xjNta53_EZkhQLD3RipBkO0Gbof-kVeEj9L5WImgfmk
            return Ok(new ApiResponseDto<TodoListDto> 
            { Error = false,
                Message = "Lista obtenida correctamente.",
                Data = list 
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<TodoListDto>
            { 
                Error = true,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<TodoListDto>
            { 
                Error = true,
                Message = ex.Message 
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(GetById));
            return StatusCode(500, new ApiResponseDto<TodoListDto> 
            {
                Error = true,
                Message = "Ocurrió un error inesperado."
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<TodoListDto>>> Create(TodoListCreateDto dto)
    {
        try
        {
            var created = await todoListService.CreateAsync(User.GetUserId(), dto);
            var response = new ApiResponseDto<TodoListDto> 
            { 
                Error = false, 
                Message = "Lista creada correctamente.",
                Data = created 
            };
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<TodoListDto>
            { 
                Error = true,
                Message = ex.Message
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<TodoListDto>
            { 
                Error = true, 
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(Create));
            return StatusCode(500, new ApiResponseDto<TodoListDto> 
            {
                Error = true,
                Message = "Ocurrió un error inesperado."
            });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<TodoListDto>>> Update(Guid id, TodoListUpdateDto dto)
    {
        try
        {
            var updated = await todoListService.UpdateAsync(User.GetUserId(), id, dto);
            if (updated is null)
            {
                return NotFound(new ApiResponseDto<TodoListDto> 
                {
                    Error = true,
                    Message = "Lista no encontrada."
                });
            }

            return Ok(new ApiResponseDto<TodoListDto> 
            { 
                Error = false, 
                Message = "Lista actualizada correctamente.", 
                Data = updated 
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponseDto<TodoListDto> 
            {
                Error = true, 
                Message = ex.Message 
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponseDto<TodoListDto> 
            {
                Error = true, 
                Message = ex.Message 
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado en {Endpoint}", nameof(Update));
            return StatusCode(500, new ApiResponseDto<TodoListDto> 
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
            var deleted = await todoListService.DeleteAsync(User.GetUserId(), id);
            if (!deleted)
            {
                return NotFound(new ApiResponseDto<bool> 
                { 
                    Error = true, 
                    Message = "Lista no encontrada." 
                });
            }

            return Ok(new ApiResponseDto<bool> 
            { 
                Error = false, 
                Message = "Lista eliminada correctamente.", 
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