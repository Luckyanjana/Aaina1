using System;
using System.Collections.Generic;
using System.Text;

namespace Aaina.Dto
{
    public class JsonResponse
    {
        /// <summary>
        /// Initializes a new instance of the JsonResponse class.
        /// </summary>
        public JsonResponse()
        {
            this.Status_Code = "200";
            this.Status = "Success";
        }
        /// <summary> 
        /// Gets or sets the Status_Code of the jsonResponse. 
        /// </summary>        
        public string Status_Code { get; set; }

        /// <summary> 
        /// Gets or sets the Status of the jsonResponse. 
        /// </summary>      
        public string Status { get; set; }

        /// <summary> 
        /// Gets or sets the Message of the jsonResponse. 
        /// </summary>
        public string Message { get; set; }

        /// <summary> 
        /// Gets or sets the Error_Field of the jsonResponse. 
        /// </summary>
        public string Error_Field { get; set; }

        /// <summary> 
        /// Gets or sets the Api_Type of the jsonResponse. 
        /// </summary>
        public string Api_Type { get; set; }

        /// <summary> 
        /// Gets or sets the Value of the jsonResponse. 
        /// </summary>
        public string Value { get; set; }
        public string Value2 { get; set; }
    }
}
