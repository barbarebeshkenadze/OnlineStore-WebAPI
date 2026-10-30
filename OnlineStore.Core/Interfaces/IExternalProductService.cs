using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OnlineStore.Core.Interfaces
{
    public interface IExternalProductService
    {
        Task<string> GetProductsAsync();
    }
}

