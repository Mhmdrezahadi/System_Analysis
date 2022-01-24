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
                .ForMember(dst => dst.From, opt => opt.MapFrom(x => x.FromUser))
                .ForMember(dst => dst.Group, opt => opt.MapFrom(x => x.ToGroup))
                .ForMember(dst => dst.Avatar, opt => opt.MapFrom(x => x.FromUser.SnapShot))
                .ForMember(dst => dst.Content, opt => opt.MapFrom(x => BasicEmojis.ParseEmojis(x.Content)))
                .ForMember(dst => dst.Timestamp, opt => opt.MapFrom(x => x.Timestamp));
            CreateMap<MessageViewModel, GroupMessage>();
        }
    }
}
