using System;
using System.ComponentModel.DataAnnotations;

namespace MaxSystemWebSite.Models.DE
{
    public class DE_SUBSCRIPTIONModel
    {
        public Guid DE_SUBCRIPTION_ID { get; set; } = Guid.NewGuid();
        public string USER_ID { get; set; }
        public string USER_EMAIL { get; set; }
        public bool STATUS { get; set; } = false;
        public int TYPE_PLAN { get; set; } 
        public DateTime START_SUBCRIPT_DATE { get; set; }
        public DateTime END_SUBCRIPT_DATE { get; set; }
        public string OPT_MSG { get; set; }
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;
        public DateTime UPDATED_DATE { get; set; } = DateTime.Now;
    }
    public class ToyyibPayBillRequest
    {
        public string Validity { get; set; }
        public string Amount { get; set; }
    }
}



