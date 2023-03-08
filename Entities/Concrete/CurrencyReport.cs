using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Abstract;

namespace Entities.Concrete
{
    [Table("CurrencyReport")]
    public class CurrencyReport : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Key]
        [StringLength(100)]
        public string CurrencyName { get; set; }
        public decimal? ForexBuying { get; set; }
        public decimal? ForexSelling { get; set; }
        public decimal? BanknoteBuying { get; set; }
        public decimal? BanknoteSelling { get; set; }
        public decimal? CrossRateUSD { get; set; }
        public DateTime? CurrencyDate { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyImage
        {
            get
            {
                string imageName = CurrencyCode.Replace(" ", "") + ".jpg";
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", imageName);
                if (File.Exists(imagePath))
                {
                    byte[] imageBytes = null;
                    using (var stream = new FileStream(imagePath, FileMode.Open))
                    {
                        using (var ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            imageBytes = ms.ToArray();
                        }
                    }
                    return "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
