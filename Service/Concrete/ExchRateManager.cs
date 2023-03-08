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
                        report.CurrencyCode = drow["CurrencyCode"].ToString();
                        report.ForexBuying = drow["ForexBuying"].ToString() != "" ? decimal.Parse(drow["ForexBuying"].ToString().Replace(".", ",")) : (decimal?)null;
                        report.ForexSelling = drow["ForexSelling"].ToString() != "" ? decimal.Parse(drow["ForexSelling"].ToString().Replace(".", ",")) : (decimal?)null;
                        report.BanknoteBuying = drow["BanknoteBuying"].ToString() != "" ? decimal.Parse(drow["BanknoteBuying"].ToString().Replace(".", ",")) : (decimal?)null;
                        report.BanknoteSelling = drow["BanknoteSelling"].ToString() != "" ? decimal.Parse(drow["BanknoteSelling"].ToString().Replace(".", ",")) : (decimal?)null;
                        report.CrossRateUSD = drow["CrossRateUSD"].ToString() != "" ? decimal.Parse(drow["CrossRateUSD"].ToString().Replace(".", ",")) : (decimal?)null;
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
