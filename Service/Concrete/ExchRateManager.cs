using Business.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Abstract;
using Entities.Concrete;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Transports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Business.Concrete
{
    public partial class ExchRateManager : IExchRateService
    {
        /// TCMB base api linki
        private const string ApiBaseUrl = "http://www.tcmb.gov.tr/kurlar/{0}.xml";


        /// Kurları çekme işlemi için kullanılan, tarih bilgisi eklenmiş api linki
        public string ApiUrl { get; set; }


        /// Kur'un çekilmek istendiği tarih
        public DateTime CurrencyDate { get; set; }


        /// Çekilen kur verisinin saklandığı değişken
        private XmlDocument XmlDoc { get; set; }


        /// <param name="currencyDate">Kurun alınacağı tarihi</param>
        public ExchRateManager(DateTime currencyDate)
        {
            CurrencyDate = currencyDate;
        }


        /// Belirtilen tarihe göre kullanılacak api url'i oluşturulur
        private void GenerateApiUrl()
        {
            ApiUrl = string.Format(ApiBaseUrl, CurrencyDate.ToString("yyyyMM") + "/" + CurrencyDate.ToString("ddMMyyyy"));
        }


        /// Belirtilen tarihteki TCMB'deki bütün kurları çeker
        public void LoadExchRate()
        {
            GenerateApiUrl();

            XmlDoc = new XmlDocument();
            XmlDoc.Load(ApiUrl);

            DataSet ds = new DataSet();
            ds.ReadXml(new XmlNodeReader(XmlDoc));

            using (CurrencyContext entity = new CurrencyContext())
            {
                List<CurrencyReport> NewDatas = new List<CurrencyReport>();
                //Çekilen data Insert edilir..
                foreach (DataRow drow in ds.Tables[1].Rows)
                {
                    if (drow["Isim"].ToString().Trim() != "")
                    {
                        CurrencyReport report = new CurrencyReport();
                        report.CurrencyName = drow["Isim"].ToString();
                        report.ForexBuying = decimal.Parse(drow["ForexBuying"].ToString() != "" ? drow["ForexBuying"].ToString().Replace(".", ",") : "0");
                        report.ForexSelling = decimal.Parse(drow["ForexSelling"].ToString() != "" ? drow["ForexSelling"].ToString().Replace(".", ",") : "0");
                        report.BanknoteBuying = decimal.Parse(drow["BanknoteBuying"].ToString() != "" ? drow["BanknoteBuying"].ToString().Replace(".", ",") : "0");
                        report.BanknoteSelling = decimal.Parse(drow["BanknoteSelling"].ToString() != "" ? drow["BanknoteSelling"].ToString().Replace(".", ",") : "0");
                        report.CrossRateUSD = decimal.Parse(drow["CrossRateUSD"].ToString() != "" ? drow["CrossRateUSD"].ToString().Replace(".", ",") : "0");
                        report.CurrencyDate = CurrencyDate;
                        entity.CurrencyReport.Add(report);
                        NewDatas.Add(report);
                    }
                }
                entity.SaveChanges();
            }
        }
    }
}
