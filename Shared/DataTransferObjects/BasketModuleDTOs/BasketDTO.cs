using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.BasketModuleDTOs
{
    public class BasketDTO
    {
        public string Id { get; set; }

        public ICollection<BasketItemDTO> Items { get; set; } = [];
    }
}
