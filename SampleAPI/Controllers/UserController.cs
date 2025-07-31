using Microsoft.AspNetCore.Mvc;
using SampleAPI.Models;
using SampleAPI.Service;

namespace SampleAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }
         
        [HttpPost]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            var users = await _userService.GetByIdAsync(id);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserModel userModel)
        {
            bool isSuccess = await _userService.CreateAsync(userModel);
            if (isSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserModel userModel)
        {
            bool isSuccess = await _userService.UpdateAsync(userModel);
            if (isSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            bool isSuccess = await _userService.DeleteAsync(id);
            if (isSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }


    }
}
