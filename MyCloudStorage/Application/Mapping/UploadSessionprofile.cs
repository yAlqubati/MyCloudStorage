using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyCloudStorage.Domain.Entities;
using MyCloudStorage.DTOs.File;

namespace MyCloudStorage.Application.Mapping
{
    public class UploadSessionprofile : Profile
    {
        public UploadSessionprofile()
        {
            CreateMap<FileEntity, FileResponseDto>();
            CreateMap<FileResponseDto, FileEntity>();
        }
    }
}