using AutoMapper;
using MetricsAgent.DAL.Models;
using MetricsAgent.Responses.DTO;

namespace MetricsAgent
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CpuMetric, CpuMetricDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time)));
            CreateMap<GcHeapSizeMetric, GcHeapSizeMetricDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time)));
            CreateMap<HddMetric, HddMetricDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time)));
            CreateMap<NetworkMetric, NetworkMetricDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time)));
            CreateMap<RamMetric, RamMetricDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time)));
        }
    }
}

