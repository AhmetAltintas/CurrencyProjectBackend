using Business.Abstract;
using Business.Concrete;
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
        public IDataResult<List<CurrencyReport>> GetAllByDate(DateTime date)
        {
            int maxDaysToCheck = 5; // Kaç gün geriye gidileceğini belirleyin
            bool dataFound = false;
            DateTime dateOnly = date.Date;

            for (int i = 0; i < maxDaysToCheck; i++)
            {
                var existingDate = _currencyDal.GetList(c => c.CurrencyDate == dateOnly);

                if (existingDate.Count == 0)
                {
                    ExchRateManager manager = new ExchRateManager(dateOnly);

                    try
                    {
                        manager.LoadExchRate();
                        dataFound = true;
                    }
                    catch
                    {
                        dateOnly = dateOnly.AddDays(-1);
                    }

                    if (dataFound)
                    {
                        var newDateData = _currencyDal.GetList(c => c.CurrencyDate == dateOnly.Date);
                        return new SuccessDataResult<List<CurrencyReport>>(newDateData);
                    }
                }
                else return new SuccessDataResult<List<CurrencyReport>>(existingDate);
            }
            return new ErrorDataResult<List<CurrencyReport>>("There is no data available for the last " + maxDaysToCheck + " days.");
        }
    }
}