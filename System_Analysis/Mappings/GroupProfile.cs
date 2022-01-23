using AutoMapper;
using Chat.Web.ViewModels;
using System_Analysis.Models;

namespace Chat.Web.Mappings
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<Group, GroupViewModel>();
            CreateMap<GroupViewModel, Group>();
        }
    }
}
