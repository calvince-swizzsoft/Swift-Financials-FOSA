using AutoMapper;
using Domain.MainBoundedContext.Aggregates.AuditLogAgg;
using Domain.MainBoundedContext.Aggregates.EnumerationAgg;
using Infrastructure.Crosscutting.Framework.Models;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Application.MainBoundedContext.DTO
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            //AuditLog => AuditLogDTO
            CreateMap<AuditLog, AuditLogDTO>()
                .ForMember(dest => dest.AdditionalNarration, opt => opt.MapFrom(src => FlattenAuditInfo(src.AdditionalNarration)));

            //Enumeration => EnumerationDTO
            CreateMap<Enumeration, EnumerationDTO>();
        }

        static string FlattenAuditInfo(string content)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(content) && content.StartsWith("<?xml"))
                {
                    var serializer = new XmlSerializer(typeof(AuditInfoWrapper));

                    var wrapper = (AuditInfoWrapper)serializer.Deserialize(new StringReader(content));

                    var sb = new StringBuilder();

                    if (wrapper.AuditInfoCollection != null && wrapper.AuditInfoCollection.Any())
                    {
                        foreach (var item in wrapper.AuditInfoCollection)
                            sb.AppendLine(string.Format("ColumnName: {0}, OriginalValue: {1}, NewValue: {2},", item.ColumnName, item.OriginalValue, item.NewValue));
                    }

                    return string.Format("{0}", sb);
                }
                else return content;
            }
            catch { return content; }
        }
    }
}
