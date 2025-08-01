using Microsoft.AspNetCore.Mvc;
using SampleAPI.Helper;
using SampleAPI.Models;
using SampleAPI.Service;

namespace SampleAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly FileHelper _fileHelper;
        public UserController(UserService userService, FileHelper fileHelper)
        {
            _userService = userService;
            _fileHelper = fileHelper;
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
            var result = await _fileHelper.UploadFileAsync(userModel.File, userModel.Id.ToString());

            if (result.isSuccess)
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
            else
            {
                return BadRequest(result.message);
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
