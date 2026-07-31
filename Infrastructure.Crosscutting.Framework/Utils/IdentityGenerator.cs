using System;
using System.Runtime.InteropServices;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public static class IdentityGenerator
    {
        [DllImport("rpcrt4.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int UuidCreateSequential(ref Guid guid);

        /// 
        /// call UuidCreateSequential and swap bytes for SQL Server format
        /// http://www.dbdelta.com/improving-uniqueidentifier-performance/
        /// sequential guid for SQL Server
        public static Guid NewSequentialGuid()
        {
            const int S_OK = 0;
            const int RPC_S_UUID_LOCAL_ONLY = 1824;

            Guid oldGuid = Guid.Empty;

            int result = UuidCreateSequential(ref oldGuid);
            if (result != S_OK && result != RPC_S_UUID_LOCAL_ONLY)
            {
                throw new ExternalException("UuidCreateSequential call failed", result);
            }

            byte[] oldGuidBytes = oldGuid.ToByteArray();
            byte[] newGuidBytes = new byte[16];
            oldGuidBytes.CopyTo(newGuidBytes, 0);

            // swap low timestamp bytes (0-3)
            newGuidBytes[0] = oldGuidBytes[3];
            newGuidBytes[1] = oldGuidBytes[2];
            newGuidBytes[2] = oldGuidBytes[1];
            newGuidBytes[3] = oldGuidBytes[0];

            // swap middle timestamp bytes (4-5)
            newGuidBytes[4] = oldGuidBytes[5];
            newGuidBytes[5] = oldGuidBytes[4];

            // swap high timestamp bytes (6-7)
            newGuidBytes[6] = oldGuidBytes[7];
            newGuidBytes[7] = oldGuidBytes[6];

            //remaining 8 bytes are unchanged (8-15) 
            return new Guid(newGuidBytes);
        }
    }
}
