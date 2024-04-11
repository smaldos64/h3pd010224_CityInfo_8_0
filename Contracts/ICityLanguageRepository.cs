using Entities.DataTransferObjects;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Contracts
{
    public interface ICityLanguageRepository : IRepositoryBase<CityLanguage>
    {
        #region From_CityLanguage
        Task<IEnumerable<CityLanguage>> GetAllCitiesLanguages(bool IncludeRelations = false);

        Task<IEnumerable<CityLanguage>> GetAllCitiesFromLanguageId(int LanguageId);

        Task<IEnumerable<CityLanguage>> GetAllLanguagesFromCityId(int CityId);

        Task AddCityLanguage(CityLanguage cityLanguage);

        Task<IEnumerable<CityLanguage>> GetAllCitiesWithLanguageId(int LanguageId);
        
        Task<IEnumerable<CityLanguage>> GetAllLanguagesWithCityId(int CityId);

        Task<CityLanguage> GetCityIdLanguageIdCombination(int CityId, int LanguageId);

        Task<bool> UpdateCityLanguageCombination(CityLanguage CityLanguageToDelete_Object,
                                                 CityLanguage CityLanguageToSave_Object);

        Task<bool> UpdateCityLanguageList(List<CityLanguageForSaveAndUpdateDto> CityLanguageNew_Object_list,
                                          List<CityLanguageDto> CityLanguageOld_Object_List);

        public bool LanguageIdFoundInCityLanguageList(List<CityLanguageForSaveAndUpdateDto> CityLanguageNew_Object_list,
                                                      int LanguageId);
        #endregion
    }
}
