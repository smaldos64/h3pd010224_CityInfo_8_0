using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
  public class CommunicationResults
  {
        private int _numberOfObjetsSaved { get; set; }
        private HttpStatusCode _httpStatusCodeResult { get; set; }
        private int _idToReturn { get; set; }

        public int NumberOfObjetsSaved
        { 
          get { return _numberOfObjetsSaved;}
          set { _numberOfObjetsSaved = value; }
        }

        public HttpStatusCode HttpStatusCodeResult
        {
          get { return _httpStatusCodeResult;}
          set { _httpStatusCodeResult = value; }
        }

        public int IdToReturn
        { 
          get { return _idToReturn; }
          set   { _idToReturn = value; }
        }
    }
}
