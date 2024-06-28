using System;

namespace PMTs.DataAccess.Utils
{
    public class PresaleTools
    {
        public static string GetPlantShortName(string plantCode)
        {

            switch (plantCode)
            {
                case "259B": //TCCB (ชลบุรี)
                    return "TCCB";
                case "252B": //บริษัท กลุ่มสยามบรรจุภัณฑ์ จำกัด (นวนคร)
                    return "TCNK";
                case "253B": //บริษัท กลุ่มสยามบรรจุภัณฑ์ จำกัด (สมุทรปราการ)
                    return "TCSP";
                case "254B": //บริษัท กลุ่มสยามบรรจุภัณฑ์ จำกัด (ราชบุรี)
                    return "TCRB";
                case "255B": //บริษัท กลุ่มสยามบรรจุภัณฑ์ จำกัด (กำแพงเพชร)
                    return "TCKP";
                case "256B": //บริษัท กลุ่มสยามบรรจุภัณฑ์ จำกัด (สงขลา)
                    return "TCS";
                case "257B": //บริษัท กลุ่มสยามบรรจุภัณฑ์ จำกัด (สระบุรี)
                    return "TCSB";
                case "258B": //บริษัท กลุ่มสยามบรรจุภัณฑ์ จำกัด (ปราจีน)
                    return "TCPB";
                case "25AB": //บริษัท กลุ่มสยามบรรจุภัณฑ์ จำกัด (ปทุมธานี)
                    return "TCPT";
                case "C621": //บริษัท ไทยคอนเทนเนอร์ ขอนแก่น จำกัด
                    return "TCKK";
                case "D621": //บริษัท ไทยคอนเทนเนอร์ ระยอง จำกัด
                    return "TCRY";
                case "J721": //บริษัท ตะวันนา
                    return "TWN";
                case "214FB-1": //บริษัท ไดน่า
                    return "DYNA";
                case "214EB-1": //บริษัท โอเรียน
                    return "ORIENT";
                case "214GB-1": //บริษัท ดีอิน
                    return "DIN";
                case "251B": //บริษัท TCG
                    return "TCG";
                case "251D": //บริษัท TCGTWN
                    return "TCG-TWN";
                case "R221": //บริษัท TCGTWN
                    return "PPC";
                case "L42B": //บริษัท โอเรียนท์คอนเทนเนอร์ จำกัด (OCON)
                    return "DYNA";
                case "L41B": //บริษัท โอเรียนท์คอนเทนเนอร์ จำกัด (OCSS)
                    return "ORIENT";
                case "L43B": //บริษัท โอเรียนท์คอนเทนเนอร์ จำกัด (OCNP)
                    return "DIN";
                default:
                    throw new Exception("ไม่พบข้อมูล Plant Code");

            }

        }

    }
}
