using ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
  public class CommunicationResults : ICommunicationResults
  {
        private int _numberOfObjetsSaved { get; set; }
        //private HttpStatusCode _httpStatusCodeResult { get; set; }
        private int _httpStatusCodeResult { get; set; }
        private int _idToReturn { get; set; }
        private string _resultString { get; set; }
        private bool _hasErrorOccured;

        public int NumberOfObjetsSaved
        { 
          get { return _numberOfObjetsSaved;}
          set { _numberOfObjetsSaved = value; }
        }

        public int HttpStatusCodeResult
        {
          get { return _httpStatusCodeResult;}
          set { _httpStatusCodeResult = value; }
        }

        public int IdToReturn
        { 
          get { return _idToReturn; }
          set   { _idToReturn = value; }
        }

        public string ResultString
        { 
          get { return _resultString;}
          set { _resultString = value; }
        }

        public bool HasErrorOccured 
        {
          get { return _hasErrorOccured; }
          set { _hasErrorOccured = value; }
        }

        public CommunicationResults(bool HasErrorOccured)
        {
            this._hasErrorOccured = HasErrorOccured;
        }
    }
}
