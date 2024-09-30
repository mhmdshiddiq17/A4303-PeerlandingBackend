using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqUpdateSaldoLenderDto
    {
        [Range(0, double.MaxValue, ErrorMessage = "Balance must be a positive value")]
        public decimal? Balance { get; set; }
    }
}
