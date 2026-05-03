using Shared.DataTransferObjects.BasketModuleDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstracion
{
    public interface IPaymentService
    {
        Task<BasketDTO> CreateOrUpdatePaymentIntentAsync(string basketId);
    }
}
