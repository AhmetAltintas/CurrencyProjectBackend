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
            DateTime dateTime = date.HasValue ? date.Value : DateTime.MinValue;

            var existingDate = _currencyDal.GetList(c=> c.CurrencyDate == date);

            if (existingDate.Count == 0)
            {
                ExchRateManager manager = new ExchRateManager(dateTime);

                manager.LoadExchRate();

                return new SuccessDataResult<List<CurrencyReport>>(_currencyDal.GetList(c => c.CurrencyDate == date));
            }
            else return new SuccessDataResult<List<CurrencyReport>>(_currencyDal.GetList(c => c.CurrencyDate == date));
        }
    }
}
