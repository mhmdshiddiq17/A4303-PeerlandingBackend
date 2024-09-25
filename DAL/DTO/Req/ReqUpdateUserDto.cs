using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Req
{
    public class ReqUpdateUserDto
    {
        [Required(ErrorMessage = "Name is Required")]
        [MaxLength(30, ErrorMessage = "Name Cannot exceed 30 characters")]

        public string Name { get; set; }


        [Required(ErrorMessage = "Role is requires")]
        [MaxLength(30, ErrorMessage = "Role cannot exceed 30 characters")]

        public string Role { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance must be a positive value")]

        public decimal? Balance { get; set; }
    }
}
