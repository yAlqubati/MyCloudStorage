using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyCloudStorage.Domain.Entities;
using MyCloudStorage.DTOs.File;

namespace MyCloudStorage.Application.Mapping
{
    public class FileProfile : Profile
    {
        public FileProfile()
        {
            CreateMap<CreateFileRequestDto, FileEntity>();
            CreateMap<FileEntity, FileResponseDto>();
            CreateMap<FileShareEntity, FileShareResponseDto>();
        }
    }
}