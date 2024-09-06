using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuelPriceWizard.DataAccess
{
    public interface IRepository<TDomainModel>
    {
        public Task<IEnumerable<TDomainModel>> GetAllAsync();
    }
}
