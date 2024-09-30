using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;



namespace BEPeer.Controllers
{
    [Route("rest/v1/lender/[action]")]
    [ApiController]
    public class LenderController : ControllerBase
    {

        private readonly ILenderServices _lenderServices;
        public LenderController(ILenderServices lenderServices)
        {
            _lenderServices = lenderServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetBalance(string userId)
        {
            // Memanggil service untuk mendapatkan balance pengguna
            var userBalance = await _lenderServices.GetBalance(userId);

            if (userBalance == null)
            {
                // Jika user tidak ditemukan, kembalikan 404
                return BadRequest(new { message = "User not found" });
            }

            // Jika user ditemukan, kembalikan saldo
            return Ok(new { balance = userBalance.Balance });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSaldo(string lenderId, decimal amount)
        {
            try
            {
                var response = await _lenderServices.UpdateSaldo(lenderId, amount);
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "User Update Success"
                });
            }
            catch (Exception ex)
            {
                if (ex.Message == "User not found")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<List<ResLoginDto>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetListPeminjam()
        {
            try
            {
                var response = await _lenderServices.GetListPeminjam();
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "Success load loan"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<List<ResLoginDto>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
