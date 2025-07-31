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
            ApiResponse apiResponse = new ApiResponse();
            var users = await _userService.GetAllAsync();
            apiResponse.IsSuccess = true;
            apiResponse.Message = "";
            apiResponse.Data = null;
            return Ok(apiResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GetById(int id)
        {
            ApiResponse apiResponse = new ApiResponse();
            if (id == 0)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "";
                apiResponse.Data = null;
                return BadRequest(apiResponse);
            }
            var users = await _userService.GetByIdAsync(id);
            apiResponse.IsSuccess = true;
            apiResponse.Message = "";
            apiResponse.Data = users;
            return Ok(apiResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserModel userModel)
        {
            ApiResponse apiResponse = new ApiResponse();

            bool isSuccess = await _userService.CreateAsync(userModel);
            if (isSuccess)
            {
                apiResponse.IsSuccess = isSuccess;
                apiResponse.Message = "";
                apiResponse.Data = userModel;
                return Ok(apiResponse);
            }
            else
            {
                apiResponse.IsSuccess = isSuccess;
                apiResponse.Message = "§ó·s¥¢±Ñ";
                apiResponse.Data = userModel;
                return Ok(apiResponse);
            }
        }



    }
}
