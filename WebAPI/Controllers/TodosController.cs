using Application.LogicInterfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodoLogic todoLogic;

        public TodosController(ITodoLogic todoLogic)
        { this.todoLogic = todoLogic; }


        [HttpPost]
        public async Task<ActionResult<Todo>> CreateAsync([FromBody] TodoCreationDto dto)
        {
            try
            {
                Todo created = await todoLogic.CreateAsync(dto);
                return Created($"/todos/{created.Id}", created);
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, e.Message);
            }


        }
    }

}
