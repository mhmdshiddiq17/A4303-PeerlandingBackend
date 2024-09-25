using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Table("trn_funding")]
    public class TrnFunding
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("loan_id")]
        [ForeignKey("MstLoans")]
        public string LoanId { get; set; }

        [Required]
        [Column("lender_id")]
        [ForeignKey("User")]
        public string LenderId { get; set; }
        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }
        [Required]
        [Column("funded_at")]
        public DateTime FundedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }

        public MstLoans Loans { get; set; }

    }
}
