using System;
using AutoMapper;
using MetricsManager.DAL.Models;
using MetricsManager.Responses.DTO;

namespace MetricsManager
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AgentInfo, AgentDto>();
            CreateMap<CpuMetricsApi, CpuMetricApiDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time)));
            CreateMap<GcHeapSizeMetricsApi, GcHeapSizeMetricApiDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time)));
            CreateMap<HddMetricsApi, HddMetricApiDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time)));
            CreateMap<NetworkMetricsApi, NetworkMetricApiDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time)));
            CreateMap<RamMetricsApi, RamMetricApiDto>()
                .ForMember("Time", dto => dto.MapFrom(metric => metric.Transfer(metric.Time)));
        }
    }
}

