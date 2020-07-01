using System;
using System.Collections.Generic;
using System.Text;

namespace DIPS.AptSMS.ConfigClient.Common.FormatFunction
{
    public enum TagFormat
    {
        U  = 0, //Upper Case
        CC = 1,//Camel Case
        PA = 2,//pretty print Address
        PN = 3,//pretty print Name
        T1 = 4,//DateTme format 01 :Friday, 29 May 2015 05:50
        T2 = 5,//DateTme format 02 :05/29/2015 05:50
        T3 = 6,//DateTme format 03 :May 29
        TX  = 7, //Special own date format
        N = 8 //No format
    }
    
}
