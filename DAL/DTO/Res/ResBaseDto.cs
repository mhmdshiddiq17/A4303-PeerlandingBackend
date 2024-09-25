using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DTO.Res
{
    public class ResBaseDto<T>
    {
        public bool Success { get; set; }
        public object Message { get; set; }
        public T Data { get; set; }
        public ResBaseDto() 
        {
            Success = false;
            Message = string.Empty;
            Data = default;
        }
        public void SetMessage(string message)
        {
            Message = message;
        }
        public void SetMessage(Dictionary<string, List<string>> message)
        {
            Message = message;
        }
        public void SetMessage(IEnumerable<string> message) 
        {
            Message = message;
        }
    }
}
