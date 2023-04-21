﻿// (c) 2023 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using AutoMapper;
using CriticalSectionPrediction.Prediction.Entity;

namespace CriticalSectionPrediction.Orchestrator.Configuration
{
    public static class Mapping
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg => {
                // This line ensures that internal properties are also mapped over.
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        });

        public static IMapper Mapper => Lazy.Value;
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Config, Data>();
        }
    }
}
