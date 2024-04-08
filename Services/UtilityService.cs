using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects;
using Entities.Models;
using Mapster;

namespace Services
{
    public class UtilityService
    {
        public static void SetupMapsterConfiguration()
        {
            // Mapster
            TypeAdapterConfig<City, CityDto>.NewConfig();
            TypeAdapterConfig<City, CityDto>.NewConfig().Map(dest => dest.CityLanguages, src => src.CityLanguages.Select(l => l.Language)).Map(dest => dest.CityId, src => src.CityId);
            // Mapning herover bevirker, at man får LanguageName med ud, når man konverterer fra 
            // City Objekter(er) til CityDTO Objekt(er)
            //TypeAdapterConfig<Country, CountryDto>.NewConfig().Map(dest => dest.Cities, src => src.Cities.Select(x => x.CityLanguages.Select(l => l.Language))).Map(dest => dest.CountryID, src => src.CountryID);
            //TypeAdapterConfig<Country, CountryDto>.NewConfig().Map(dest => dest.Cities, src => src.Cities.Select(c => c.CityLanguages.Select(l => l.Language))).Map(dest => dest.CountryID, src => src.CountryID);
            //TypeAdapterConfig<Country, CountryDto>.NewConfig().Map(dest => dest.Cities, src => src.Cities.Select(c => c.CityLanguages)).Map(dest => dest.CountryID, src => src.CountryID);
            //TypeAdapterConfig<Country, CountryDto>.NewConfig().Map(dest => dest.Cities, src => src.Cities.Select(x => CityMapping(x)));

            // Mapning herover bevirker, at man får LanguageName med ud, når man konverterer fra 
            // City Objekter(er) til CityDTO Objekt(er)

            TypeAdapterConfig<CityForUpdateDto, City>.NewConfig();

            //TypeAdapterConfig<Country, CountryDto>.NewConfig().Map(dest => dest.CountryID, src => src.CountryID);
            TypeAdapterConfig<Language, LanguageDto>.NewConfig().Map(dest => dest.CityLanguages, src => src.CityLanguages.Select(x => x.City));
            TypeAdapterConfig<CityLanguage, CityLanguageDto>.NewConfig().Map(dest => dest.CityId, src => src.CityId).Map(dest => dest.LanguageId, src => src.LanguageId).
               Map(dest => dest.City, src => src.City);
        }

        //public static CityLanguageDto MapCityLanguage(CityLanguage source)
        //{
        //    return new CityLanguageDto
        //    {
        //        CityId = source.CityId,
        //        LanguageId = source.LanguageId,
        //        // Add other properties of CityLanguageDto as needed
        //    };
        //}

        //public static CityDto MapCity(City source)
        //{
        //    var cityDto = new CityDto
        //    {
        //        CityId = source.CityId,
        //        CityName = source.CityName,
        //        CountryID = source.CountryID,
        //        CityDescription = source.CityDescription,
        //        // Add other properties of CityDto as needed
        //    };

        //    cityDto.PointsOfInterest = (ICollection<PointOfInterestForUpdateDto>)source.PointsOfInterest;
        //    cityDto.CityLanguages = (ICollection<LanguageDtoMinusRelations>)source.CityLanguages.Select(cl => MapCityLanguage(cl));

        //    return cityDto;
        //}

        //public static CountryDto MapCountry(Country source, bool includeCities = true)
        //{
        //    var countryDto = new CountryDto
        //    {
        //        CountryID = source.CountryID,
        //        CountryName = source.CountryName,
        //    };

        //    if (includeCities)
        //    {
        //        countryDto.Cities = (ICollection<CityDtoMinusCountryRelations>)source.Cities.Select(c => MapCity(c));
        //    }

        //    return countryDto;
        //}

        //public static List<CountryDto> MapCountryList(IEnumerable<Country> source, bool includeCities = true)
        //{
        //    List<CountryDto> countryDtos = new List<CountryDto>();
        //    List<Country> countries = source.ToList();

        //    foreach (var country in countries)
        //    {
        //        countryDtos.Add(MapCountry(country, includeCities));
        //    }

        //    return countryDtos;
        //}

    }
}
