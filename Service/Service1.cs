﻿using DataAccess.Concrete.EntityFramework;
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

namespace Service
{
    public class Service1
    {
        /// TCMB base api linki
        private const string ApiBaseUrl = "http://www.tcmb.gov.tr/kurlar/{0}.xml";


        /// Kurları çekme işlemi için kullanılan, tarih bilgisi eklenmiş api linki
        public string ApiUrl { get; set; }


        /// Kur'un çekilmek istendiği tarih
        public DateTime CurrencyDate { get; set; }


        /// TCMB'den kurun alındığı gerçek tarih
        public DateTime ActualCurrencyDate { get; set; }


        /// Kullanıcının belirttiği tarihte kur yok ise geçmişe doğru kontrol edilecek gün sayısı
        private const int ExchRateAttempts = 5;


        /// Çekilen kur verisinin saklandığı değişken
        private XmlDocument XmlDoc { get; set; }


        /// <param name="currencyDate">Kurun alınacağı tarihi</param>
        public Service1(DateTime currencyDate)
        {
            CurrencyDate = currencyDate;
        }


        /// Belirtilen tarihe göre kullanılacak api url'i oluşturulur
        private void GenerateApiUrl()
        {
            ApiUrl = String.Format(ApiBaseUrl, this.ActualCurrencyDate.ToString("yyyyMM") + "/" + this.ActualCurrencyDate.ToString("ddMMyyyy"));
        }


        /// Belirtilen tarihteki TCMB'deki bütün kurları çeker
        public void LoadExchRate()
        {
            ActualCurrencyDate = CurrencyDate;

            // kullanıcının belirttiği tarihte kur var ise alınır
            // yok ise en yakın kur olan gün bulunur
            for (int attempts = 0; attempts < ExchRateAttempts; attempts++)
            {
                try
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
                                report.ForexBuying = Decimal.Parse(drow["ForexBuying"].ToString() != "" ? drow["ForexBuying"].ToString().Replace(".", ",") : "0");
                                report.ForexSelling = Decimal.Parse(drow["ForexSelling"].ToString() != "" ? drow["ForexSelling"].ToString().Replace(".", ",") : "0");
                                report.BanknoteBuying = Decimal.Parse(drow["BanknoteBuying"].ToString() != "" ? drow["BanknoteBuying"].ToString().Replace(".", ",") : "0");
                                report.BanknoteSelling = Decimal.Parse(drow["BanknoteSelling"].ToString() != "" ? drow["BanknoteSelling"].ToString().Replace(".", ",") : "0");
                                report.CrossRateUSD = Decimal.Parse(drow["CrossRateUSD"].ToString() != "" ? drow["CrossRateUSD"].ToString().Replace(".", ",") : "0");
                                report.CurrencyDate = ActualCurrencyDate;
                                entity.CurrencyReport.Add(report);
                                NewDatas.Add(report);
                            }
                        }
                        entity.SaveChanges();

                    } 

                    break;
                }
                catch (WebException ex)
                {
                    if (ex.Response != null)
                    {
                        // 404 not found
                        HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
                        if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                        {
                            // bir gün öncesi kontrol edilir
                            ActualCurrencyDate = ActualCurrencyDate.AddDays(-1);
                        }
                        else
                        {
                            throw new Exception("Kur bilgisi bulunamadı.");
                        }
                    }
                    else
                    {
                        throw new Exception("Kur bilgisi bulunamadı.");
                    }
                }
            }


            if (XmlDoc == null)
            {
                throw new Exception("Kur bilgisi bulunamadı.");
            }
        }


        /// Kur'u getirir
        /// <param name="currency">Kurun alınmak istendiği para birimi</param>
        /// <param name="exchRateType">Alınmak istenen kur tipi</param>
        public Decimal GetExchRate(string currency, ExchRateType exchRateType)
        {
            // eğer daha önce load edilmemiş ise bu aşamada yapılır
            if (XmlDoc == null)
            {
                LoadExchRate();
            }

            // TCMB noktayı (.) ondalık ayracı olarak kullanıyor.
            // string'den decimal'e çevrim sırasında windows region ayarlarından etkilenmeden doğru çevrilmesi için en-us culture'ı kullanılır
            System.Globalization.CultureInfo culInfo = new System.Globalization.CultureInfo("en-US", true);

            // xml içinde okunacak node ayarlanır
            string nodeStr = String.Format("Tarih_Date/Currency[@CurrencyCode='{0}']/{1}", currency.ToUpper(), GetExchRateTypeNodeStr(exchRateType));

            // string olarak alınan kur decimal'e çevrilip dönülür
            return Decimal.Parse(XmlDoc.SelectSingleNode(nodeStr).InnerXml, culInfo);
        }


        private string GetExchRateTypeNodeStr(ExchRateType exchRateType)
        {
            string ret = "";

            switch (exchRateType)
            {
                case ExchRateType.ForexBuying:
                    ret = "ForexBuying";
                    break;

                case ExchRateType.ForexSelling:
                    ret = "ForexSelling";
                    break;

                case ExchRateType.BanknoteBuying:
                    ret = "BanknoteBuying";
                    break;

                case ExchRateType.BanknoteSelling:
                    ret = "BanknoteSelling";
                    break;
            }

            return ret;
        }


        public List<CurrencyReport> changeDatas;
        /// Bu önbellekten tüm verilerin değerini kontrol edeceğiz. Ve herhangi bir Değişiklik olursa onu yenileyeceğiz.
        public List<CurrencyReport> cacheDatas;
        public void FillCache()
        {
            changeDatas = new List<CurrencyReport>();
            if (cacheDatas == null)
            {
                cacheDatas = new List<CurrencyReport>();
            }
            else
            {
                cacheDatas.Clear();
            }
            using (CurrencyContext entity = new CurrencyContext())
            {
                cacheDatas = entity.CurrencyReport.ToList();
            }
        }


        /// Mevcut Döviz Verilerinin değişip değişmediğini burada kontrol ettik.
        public bool isDataChange(CurrencyReport data)
        {
            return cacheDatas.Any(cd => cd.CurrencyName == data.CurrencyName && cd.BanknoteBuying == data.BanknoteBuying && cd.BanknoteSelling == data.BanknoteSelling && cd.ForexBuying == data.ForexBuying && cd.ForexSelling == data.ForexSelling);
        }
    }

    public enum ExchRateType
    {
        /// <summary>
        /// Döviz Alış
        /// </summary>
        ForexBuying,

        /// <summary>
        /// Döviz Satış
        /// </summary>
        ForexSelling,

        /// <summary>
        /// Efektif Alış
        /// </summary>
        BanknoteBuying,

        /// <summary>
        /// Efektif Satış
        /// </summary>
        BanknoteSelling
    }
}