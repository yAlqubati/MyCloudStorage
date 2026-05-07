
using MyCloudStorage.DTOs.Folder;
using AutoMapper;
using MyCloudStorage.Domain.Entities;

namespace MyCloudStorage.Application.Mapping
{
    public class FolderProfile : Profile
    {
        public FolderProfile()
        {
            CreateMap<Folder, FolderResponseDto>();

            CreateMap<CreateFolderRequestDto, Folder>();
        }
    }
}