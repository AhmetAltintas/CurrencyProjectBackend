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
            var existingDate = _currencyDal.GetList(c => c.CurrencyDate == date);

            if (existingDate.Count == 0)
            {
                int maxDaysToCheck = 5; // Kaç gün geriye gidileceğini belirleyin
                DateTime currentDate = date;
                bool dataFound = false;

                for (int i = 0; i < maxDaysToCheck; i++)
                {
                    ExchRateManager manager = new ExchRateManager(currentDate);

                    try
                    {
                        manager.LoadExchRate();
                        dataFound = true;
                    }
                    catch
                    {
                        currentDate = currentDate.AddDays(-1);
                    }

                    if (dataFound)
                    {
                        var newDateData = _currencyDal.GetList(c => c.CurrencyDate == currentDate);
                        return new SuccessDataResult<List<CurrencyReport>>(newDateData);
                    }
                }

                return new ErrorDataResult<List<CurrencyReport>>("There is no data available for the last " + maxDaysToCheck + " days.");
            }
            else return new SuccessDataResult<List<CurrencyReport>>(existingDate);
        }
    }
}