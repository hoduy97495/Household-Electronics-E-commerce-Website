using DoAnCoSo.Models;
using DoAnCoSo.Repository;
using Microsoft.AspNetCore.Mvc;
namespace DoAnCoSo.ViewComponents
{
    public class DanhMucMenuViewComponent : ViewComponent
    {
        private readonly IDanhMucRepository _danhMuc;
        public DanhMucMenuViewComponent(IDanhMucRepository danhMucRepository)
        {
            _danhMuc = danhMucRepository;
        }
        public IViewComponentResult Invoke() 
        { 
            var danhmuc = _danhMuc.GetAllDanhMuc().OrderBy(x=>x.TenDanhMuc);
            return View(danhmuc);
        }
    }
}
