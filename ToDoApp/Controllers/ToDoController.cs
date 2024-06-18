using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<CustomUser> _userMananager;
        private readonly SignInManager<CustomUser> _signInManager;

        public ToDoController(ToDoDBContext context, ILogger<ToDoController> logger, IMapper mapper, SignInManager<CustomUser> signInManager, UserManager<CustomUser> userManager)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _signInManager = signInManager;
            _userMananager = userManager;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            CustomUser user = await _userMananager.GetUserAsync(User);
            List<Todo> todo = _context.Todos.Where(x=>x.IsDeleted == false && x.CustomUserId == user.Id).ToList();
            List<TodoGetDTO> dto = _mapper.Map<List<TodoGetDTO>>(todo);
            _logger.LogInformation("Retrieved all Todo items successfully.");
            return Ok(dto);
        }

        [HttpGet("get/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] TodoCreateDTO newTodo)
        {
            CustomUser user = await _userMananager.GetUserAsync(User);
            

            Todo todo = new Todo()
            {
                Name = newTodo.Name,
                Date = DateTime.Now,
                IsDeleted = false,
                CustomUserId = user.Id
            };

            _logger.LogInformation($"Created Todo item successfully. ID: {todo.Id}");
            TodoCreateDTO dto = _mapper.Map<TodoCreateDTO>(todo);
            _context.Todos.Add(todo);
            _context.SaveChanges();

            return Ok(dto);
        }


        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public IActionResult Update(int id, [FromForm] TodoUpdateDTO updatedTodo)
        {
            // Check if the Todo item exists
            Todo todo = _context.Todos.FirstOrDefault(x => x.Id == id && x.IsDeleted == false)!;
            if (todo is null)
            {
                _logger.LogError($"Todo item not found with ID: {id}");
                return NotFound();
            }

            // Update relevant fields
            bool isUpdated = false;
            if (!string.IsNullOrEmpty(updatedTodo.Name) && updatedTodo.Name != todo.Name)
            {
                todo.Name = updatedTodo.Name;
                isUpdated = true;
            }

            if (updatedTodo.Completed.HasValue && updatedTodo.Completed != todo.Completed)
            {
                todo.Completed = updatedTodo.Completed.Value;
                isUpdated = true;
            }

            // Save changes if any updates were made
            if (isUpdated)
            {
                _context.SaveChanges();
                _logger.LogInformation($"Updated Todo item successfully. ID: {id}");
                return Ok(todo); // Return the updated entity
            }

            _logger.LogInformation($"No updates were made to the Todo item. ID: {id}");
            return Ok(todo); // Return the entity as it was not modified
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
