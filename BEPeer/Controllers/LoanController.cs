using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/loan/[action]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanServices _loanServices;
        

        public LoanController(ILoanServices loanServices)
        {
            _loanServices = loanServices;
        }

        [HttpPost]
        public async Task<IActionResult> NewLoan(ReqLoanDto loan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();
                    var errorMessage = new StringBuilder("validation errors occured!");
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                }
                var res = await _loanServices.CreateLoan(loan);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Loan Successfully",
                    Data = res
                });

            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                });
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateLoan(string userId, ReqUpdateLoanDto loanUpdate)
        {

            try
            {
                var response = await _loanServices.UpdateLoan(userId, loanUpdate);

                
                if (loanUpdate.Status == "done")
                {
                    return Ok(new ResBaseDto<object>
                    {
                        Data = response,
                        Success = true,
                        Message = "Loan update successfully"
                    });
                }
                else if (loanUpdate.Status == "failed")
                {
                    return Ok(new ResBaseDto<object>
                    {
                        Data = response,
                        Success = false,
                        Message = "Loan failed, please wait 1 month again soon"
                    });
                }

                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "Loan Status Update Success"
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
        public async Task<IActionResult> LoanList(string status)
        {
            try
            {
                var response = await _loanServices.LoanList(status);
                return Ok(new ResBaseDto<object>
                {
                    Data = response,
                    Success = true,
                    Message = "Success load loan"
                });
            }
            catch(Exception ex)
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
