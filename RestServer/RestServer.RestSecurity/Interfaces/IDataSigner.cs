using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.RestSecurity.Interfaces
{
    public interface IDataSigner
    {
        string GetSignature(string data, string key);

        string GetEncodedData(string data);

        string GetDecodedData(string data);

        bool IsValidSignature(string data, string key, string signature);
    }
}
