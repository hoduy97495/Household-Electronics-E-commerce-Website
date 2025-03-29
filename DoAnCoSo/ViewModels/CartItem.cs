namespace DoAnCoSo.ViewModels
{
    public class CartItem
    {
        public int MaSanPham {  get; set; }
        public string HinhSp { get; set; }
        public string TenSp { get; set; }
        public double Gia { get; set; }
        public int SoLuong { get; set; }
        public double ThanhTien => SoLuong * Gia;
      
       

    }
}
