using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly FileUploadHelper _fileUploadHelper;
        public UserController(UserService userService, FileUploadHelper fileUploadHelper)
        {
            _userService = userService;
            _fileUploadHelper = fileUploadHelper;
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
            var result = await _fileUploadHelper.UploadFileAsync(userModel.File);

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
                return Ok(result.message);
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
