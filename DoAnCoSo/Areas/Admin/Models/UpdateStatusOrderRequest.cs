namespace DoAnCoSo.Areas.Admin.Models
{
    public class UpdateStatusOrderRequest
    {
        public int OrderId { get; set; }
        public int NewStatus { get; set; }
    }
}
