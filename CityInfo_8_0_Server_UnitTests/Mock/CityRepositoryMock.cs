using CityInfo_8_0_TestSetup.Setup;
using CityInfo_8_0_TestSetup.ViewModels;
using Contracts;
using Entities;
using Entities.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityInfo_8_0_Server_UnitTests.Mock
{
    public class CityRepositoryMock
    {
        private static DatabaseViewModel _databaseViewModel;

        public static ICityRepository GetMock()
        {
            _databaseViewModel = new DatabaseViewModel();
            SetupDatabaseData.SeedDatabaseDataWithObject(null, _databaseViewModel);
            List<City> ListOfCities = _databaseViewModel.CityList;

            DatabaseContext dbContextMock = DbContextMock.GetMock<City, DatabaseContext>(ListOfCities, x => x.Core_8_0_Cities);
            
            return new CityRepository(dbContextMock);
        }

        //private static List<Employee> GenerateTestData()
        //{
        //    List<Employee> lstUser = new();
        //    Random rand = new Random();
        //    for (int index = 1; index <= 10; index++)
        //    {
        //        lstUser.Add(new Employee
        //        {
        //            Id = index,
        //            Name = "User-" + index,
        //            Age = rand.Next(1, 100)

        //        });
        //    }
        //    return lstUser;
        //}
    }
}
