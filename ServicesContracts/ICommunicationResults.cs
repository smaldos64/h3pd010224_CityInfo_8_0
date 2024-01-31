using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.StatusCodes;

namespace ServicesContracts
{
  public interface ICommunicationResults
  {
    int NumberOfObjetsChanged { get; set; }
    int IdToReturn { get; set; }
    //HttpStatusCode HttpStatusCodeResult { get; set; }
    int HttpStatusCodeResult { get; set; }
    string ResultString { get; set; }
    bool HasErrorOccured { get; set; }
  }
}
