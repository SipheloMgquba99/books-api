using AutoMapper;
using Library_System.Domain.Dtos;
using Library_System.Domain.Entities;

public class Mapping : Profile
{
    public Mapping()
    {
        CreateMap<BookDto, Book>();
        CreateMap<Book, BookDto>();
        CreateMap<BookRequest, BookRequestDto>();
        CreateMap<BookRequestor, BookRequestorDto>();
    }
}