using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoApp.DAL;
using ToDoApp.DTOs;
using ToDoApp.Entities;

namespace ToDoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoDBContext _context;
        private readonly ILogger<ToDoController> _logger;
        private readonly IMapper _mapper;

        public ToDoController(ToDoDBContext context, ILogger<ToDoController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<Todo> todo = _context.Todos.ToList();
            List<TodoGetDTO> dto = _mapper.Map<List<TodoGetDTO>>(todo);
            _logger.LogInformation("Retrieved all Todo items successfully.");
            return Ok(dto);
        }

    }
}
