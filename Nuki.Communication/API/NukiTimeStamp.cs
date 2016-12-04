using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuki.Communication.API
{
    public class NukiTimeStamp
    {
        public UInt16 Year { get; private set; }
        public byte Month { get; private set; }
        public byte Day { get; private set; }
        public  byte Hour { get; private set; }
        public  byte Minute { get; private set; }
        public byte Secound { get; private set; }

        public NukiTimeStamp(DateTime dt)
        {
            dt = dt.ToUniversalTime();
            Year = (UInt16)dt.Year;
            Month =(byte) dt.Month;
            Day = (byte)dt.Day;
            Hour = (byte)dt.Hour;
            Minute = (byte)dt.Minute;
            Secound = (byte)dt.Second;
        }

        private NukiTimeStamp()
        { }

        public static NukiTimeStamp FromBytes(byte[] values)
        {
            return FromBytes(values, 0);
        }
        public static NukiTimeStamp FromBytes(byte[] values, int startIndex)
        {
            return new NukiTimeStamp
            {
                Year = BitConverter.ToUInt16(values, startIndex + 0),
                Month = values[startIndex + 2],
                Day = values[startIndex + 3],
                Hour = values[startIndex + 4],
                Minute = values[startIndex + 5],
                Secound = values[startIndex + 6]
            };
        }
   
        public DateTime ToUTCDateTime()
        {
            return new DateTime(Year, Month, Day, Hour, Minute, Secound, DateTimeKind.Utc);
        }
        public override string ToString()
        {
            return ToUTCDateTime().ToString();
        }
    }
}
