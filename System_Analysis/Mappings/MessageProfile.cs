using AutoMapper;
using Chat.Web.Helpers;
using Chat.Web.ViewModels;
using System_Analysis.Models;

namespace Chat.Web.Mappings
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<GroupMessage, MessageViewModel>()
                .ForMember(dst => dst.From, opt => opt.MapFrom(x => x.FromUser.FirstName + " " + x.FromUser.LastName))
                .ForMember(dst => dst.Room, opt => opt.MapFrom(x => x.ToGroup.Name))
                .ForMember(dst => dst.Avatar, opt => opt.MapFrom(x => x.FromUser.SnapShot))
                .ForMember(dst => dst.Content, opt => opt.MapFrom(x => BasicEmojis.ParseEmojis(x.Content)))
                .ForMember(dst => dst.Timestamp, opt => opt.MapFrom(x => x.Timestamp));
            CreateMap<MessageViewModel, GroupMessage>();
        }
    }
}
