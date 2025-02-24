using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceClaim.Models
{
    public class USDConverterModel
    {
        public int Id { get; set; }
        public int CurrencyId { get; set; }
        public string Currency { get; set; }
        public decimal CurrentUsdToRtgs { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
