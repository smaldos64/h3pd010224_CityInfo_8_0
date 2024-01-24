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
      TypeAdapterConfig<City, CityDto>.NewConfig().Map(dest => dest.CityLanguages, src => src.CityLanguages.Select(x => x.Language)).Map(dest => dest.CityId, src => src.CityId);
      // Mapning herover bevirker, at man får LanguageName med ud, når man konverterer fra 
      // City Objekter(er) til CityDTO Objekt(er)
      //TypeAdapterConfig<CityDto, City>.NewConfig();
      TypeAdapterConfig<CityForUpdateDto, City>.NewConfig();

      TypeAdapterConfig<Country, CountryDto>.NewConfig().Map(dest => dest.CountryID, src => src.CountryID);
      TypeAdapterConfig<Language, LanguageDto>.NewConfig().Map(dest => dest.CityLanguages, src => src.CityLanguages.Select(x => x.City));
      TypeAdapterConfig<CityLanguage, CityLanguageDto>.NewConfig().Map(dest => dest.CityId, src => src.CityId).Map(dest => dest.LanguageId, src => src.LanguageId).
         Map(dest => dest.City, src => src.City);
    }
  }
}
