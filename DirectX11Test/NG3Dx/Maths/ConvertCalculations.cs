using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NG3Dx.Maths
{
    public class ConvertCalculations
    {
        public static float DegreeToRadian(float degree)
        {
            return (float)(degree * Math.PI / 180);
        }
    }
}
