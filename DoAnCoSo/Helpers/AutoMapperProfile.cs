using AutoMapper;
using DoAnCoSo.Models;
using DoAnCoSo.ViewModels;
namespace DoAnCoSo.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<RegisterVM, TaiKhoan>();
                //.ForMember(kh => kh.Ten, option => option.MapFrom(RegisterVM => RegisterVM.Ten))
                //.ReverseMap();
        }
    }
}
