using AutoMapper;
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
            List<Todo> todo = _context.Todos.Where(x=>x.IsDeleted == false).ToList();
            List<TodoGetDTO> dto = _mapper.Map<List<TodoGetDTO>>(todo);
            _logger.LogInformation("Retrieved all Todo items successfully.");
            return Ok(dto);
        }

        [HttpGet("get/{id}")]
        [ProducesResponseType(typeof(TodoGetDTO), 200)]
        [ProducesResponseType(404)]
        public IActionResult Get(int id)
        {
            Todo todo = _context.Todos.FirstOrDefault(x => x.Id == id && x.IsDeleted == false)!;
            if (todo is null)
            {
                _logger.LogError($"Todo item not found with {id} ID");
                return NotFound();
            }
            _logger.LogInformation($"Retrieved Todo item successfully. ID: {id}");
            TodoGetDTO dto = _mapper.Map<TodoGetDTO>(todo);
            return Ok(dto);
        }



        [HttpPost("create")]
        [ProducesResponseType(typeof(TodoCreateDTO), 200)]
        public IActionResult Create([FromForm] TodoCreateDTO newTodo)
        {
            Todo todo = new Todo()
            {
                Name = newTodo.Name,
                Date = DateTime.Now,
                IsDeleted = false,
            };

            _logger.LogInformation($"Created Todo item successfully. ID: {todo.Id}");

            _context.Todos.Add(todo);
            _context.SaveChanges();

            return Ok();
        }


        [HttpPut("update/{id}")]
        [ProducesResponseType(typeof(TodoUpdateDTO), 200)]
        [ProducesResponseType(404)]
        public IActionResult Update(int id, [FromForm] TodoUpdateDTO updatedTodo)
        {
            Todo todo = _context.Todos.FirstOrDefault(x => x.Id == id && x.IsDeleted == false)!;
            if (todo is null)
            {
                _logger.LogError($"Todo item not found with {id} ID");
                return NotFound();
            }

            if (!string.IsNullOrEmpty(updatedTodo.Name))
            {
                todo.Name = updatedTodo.Name;
                _context.SaveChanges();
                _logger.LogInformation($"Updated Todo item successfully. ID: {id}");

            }

            return Ok(updatedTodo);
        }

        [HttpPut("delete/{id}")]
        public IActionResult Delete(int id)
        {
            Todo todo = _context.Todos.FirstOrDefault(x=>x.Id == id)!;
            if (todo is null)
            {
                _logger.LogError($"Todo item not found with {id} ID");
                return NotFound();
            }
            if(todo.IsDeleted == true)
            {
                _logger.LogError("This Todo item is already removed");
                return BadRequest();
            }

            todo.IsDeleted = true;
            _context.SaveChanges();

            return Ok(todo);
        }



    }
}
