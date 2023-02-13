using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CurrencyManager : ICurrencyService
    {
        ICurrencyDal _currencyDal;

        public CurrencyManager(ICurrencyDal currencyDal)
        {
            _currencyDal = currencyDal;
        }

        public IDataResult<List<CurrencyReport>> GetAllByDate(DateTime? date)
        {
            return new SuccessDataResult<List<CurrencyReport>>(_currencyDal.GetList(c => c.CurrencyDate == date));
        }
    }
}
