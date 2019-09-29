using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.MVCUI.Models.MyTools
{
    public class Cart
    {
        Dictionary<int, CartItem> _sepetim = new Dictionary<int, CartItem>();

        public List<CartItem> Sepetim
        {
            get
            {
                return _sepetim.Values.ToList();
            }
        }

        public void SepeteEkle(CartItem item)
        {
            if (_sepetim.ContainsKey(item.ID))
            {
                _sepetim[item.ID].Amount += 1;
                return;

            }

            _sepetim.Add(item.ID, item);
        }

        public void SepettenSil(int id)
        {
            if (_sepetim[id].Amount > 1)
            {

                _sepetim[id].Amount -= 1;
                return;
            }
            _sepetim.Remove(id);
        }

        //normal sartlar altında bu alttaki sekilde de Cart'taki CartItem'larin toplam tutarını bulabilirsiniz. Ancak Dictionary yapısı icerisinde daha kısa koddan bu amaca ulasmayı saglıyor
        //public decimal ToplamTutar()
        // {
        //     decimal totalPrice = 0;
        //     foreach (CartItem item in _sepetim.Values)
        //     {
        //         totalPrice += item.SubTotal;
        //     }

        //     return totalPrice;
        // }

        public decimal? TotalPrice
        {
            get
            {

                return _sepetim.Sum(x => x.Value.SubTotal);
            }
        }
    }
}